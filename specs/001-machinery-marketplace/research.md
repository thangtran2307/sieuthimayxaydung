# Phase 0 Research: Construction Machinery Marketplace

**Date**: 2026-08-09 (rev. 2 — backend → .NET) | **Feature**: 001-machinery-marketplace

Decisions that resolve the Technical Context unknowns. Format: **Decision → Rationale →
Alternatives considered**.

---

## 1. Backend framework — .NET 10 (ASP.NET Core) [REVISED]

**Decision**: Build the backend as an ASP.NET Core (.NET 10) solution organized by DDD layers.

**Rationale**:
- The user has more experience in .NET and wants direct control over data access (Dapper/SQL)
  rather than an ORM — this materially affects delivery speed and confidence for a solo dev.
- .NET is fully open-source (MIT), free, and cheap to host on Linux/containers; C# with nullable
  reference types + analyzers gives strong compile-time safety for a DDD domain.
- First-class support for the constitution's needs: middleware/filters (envelope + fail-loud),
  DI (explicit dependencies), and built-in OpenAPI generation.

**Alternatives considered**:
- **NestJS + Prisma** (previous decision): enabled a shared TypeScript contract package, but the
  user found Prisma/ORM control unsatisfactory and prefers .NET. Reversed.
- **.NET + EF Core**: less control over SQL and heavier than the user wants; Dapper chosen instead
  (see §2).

**Trade-off accepted**: losing a single shared cross-stack DTO package. Mitigated by generating
frontend types from the API's OpenAPI document (see §7).

---

## 2. Data access — EF Core (writes) + Dapper (reads) [REVISED rev.3]

**Decision**: Split persistence CQRS-style: **EF Core** owns the write side (aggregate persistence
via the Repository + Unit of Work patterns) and the schema (auto-generated migrations); **Dapper**
owns the read side (hand-written SQL, run over the EF DbContext's connection). Enum columns are
stored as text (EF `HasConversion<string>()`); `specs` is a `ListingSpecs` value object mapped to
`jsonb` via `OwnsOne(...).ToJson()`.

> rev. 2 chose Dapper-only + DbUp; the user asked to use EF Core migrations (auto-generated) and
> AutoMapper/Autofac/CQRS. rev. 3 keeps Dapper for reads (per the original preference) while EF Core
> handles writes + migrations.

**Rationale**:
- The user explicitly wants Dapper. It gives full control and visibility over SQL, which pairs
  well with Postgres-specific features this project needs: JSONB `specs`, `tsvector` full-text
  search, `pg_trgm` fuzzy matching, and the boosted-first ranking query.
- No heavy ORM runtime; queries are explicit and tunable (Constitution VII), and repositories keep
  SQL isolated from the domain (Constitution IV/V).

**Alternatives considered**:
- **EF Core**: richer change-tracking/migrations but abstracts SQL away and is heavier — rejected
  per the user's preference for direct control.
- **Raw ADO.NET**: too much boilerplate; Dapper is the pragmatic middle ground.

**Notes**: Postgres enums are mapped via `NpgsqlDataSourceBuilder.MapEnum<T>()`; JSONB is read/
written as `jsonb`. A small set of Dapper type handlers covers `DateTimeOffset`/enum edge cases.

---

## 3. Migrations — EF Core (auto-generated) [REVISED rev.3]

**Decision**: Manage schema with EF Core migrations (`dotnet ef migrations add`), generated from the
DbContext model. The initial migration includes the `jsonb` specs column, the Postgres-generated
`tsvector` search column (`HasGeneratedTsVectorColumn`), and GIN + `pg_trgm` indexes — no
hand-written SQL scripts. DbUp is no longer used.

**Rationale**:
- With Dapper there is no ORM migration engine; DbUp is a tiny, transparent library that runs
  plain SQL scripts — a perfect fit for SQL-first development and for the constitution's
  expand→migrate→contract rule (forward-only scripts, committed separately from dependent code).
- Full-text search objects (tsvector column, trigger, GIN/trgm indexes) are naturally expressed as
  SQL here.

**Alternatives considered**:
- **FluentMigrator**: C#-DSL migrations with up/down; more capable but more abstraction than a
  SQL-first project needs. Rejected to stay SQL-explicit and minimal.
- **EF Core migrations**: not applicable (no EF Core).

---

## 4. Database & search — PostgreSQL native FTS

**Decision**: PostgreSQL 16. Full-text search via a weighted `tsvector` column (title = A,
description = B) with a GIN index; fuzzy matching via `pg_trgm`. Listing specifications stored as
JSONB. (Unchanged from rev. 1.)

**Rationale**: Meets SC-003 at target scale without an external search engine (minimize
dependencies). Filter columns + FTS live in one tuned SQL query executed by Dapper.

**Alternatives considered**: Elasticsearch/Meilisearch — deferred; unjustified operational cost at
MVP scale. The repository boundary keeps this swappable.

---

## 5. CQRS orchestration — source-generated mediator [REVISED rev.3]

**Decision**: Use the free, MIT-licensed **martinothamar/Mediator** (`Mediator.Abstractions` +
`Mediator.SourceGenerator`) for CQRS command/query dispatch (`IQuery<T>`/`IQueryHandler<,>`, etc.).
Handlers are resolved through Autofac (registrations populated from `AddMediator`).

**Rationale**:
- The user wants a CQRS mediator. **MediatR** is now commercially licensed; the source-generated
  Mediator gives the same ergonomics with zero license cost and high performance (no reflection).

**Alternatives considered**:
- **MediatR**: familiar but paid for commercial use — rejected.
- **Plain handlers via DI** (rev. 2 choice): fine, but the source-gen mediator adds pipeline
  behaviors for free and is what the user requested.

---

## 6. Mapping & validation — AutoMapper + FluentValidation [REVISED rev.3]

**Decision**: Map domain ↔ DTO with **AutoMapper** (profiles scanned from the Application assembly),
pinned to **13.0.1** (the last MIT-licensed release). Validate inbound requests with
**FluentValidation** validators executed at the API boundary (`FluentValidationFilter`), including
pagination clamping and the "exactly one of price/priceContact" rule.

**Rationale**:
- The user requested AutoMapper. Pinning to 13.0.1 keeps it free (MIT); later majors moved to a paid
  license. FluentValidation is free/open-source and gives clean, testable boundary validation
  (Constitution II).

**Alternatives considered**:
- **Manual mapping** (rev. 2 choice): explicit but the user prefers AutoMapper.
- **AutoMapper (latest)**: commercially licensed — avoided by pinning 13.0.1.

> Note: AutoMapper 13.0.1 carries a NuGet audit advisory (GHSA-rvv3-g6hj-g44x); it surfaces as a
> non-fatal `NU1903` warning. Revisit if a patched MIT release appears or the licensing budget allows.

---

## 7. Cross-stack contract — OpenAPI + openapi-typescript [NEW]

**Decision**: The API exposes OpenAPI via **Swashbuckle** with **Swagger UI** and **URL-segment API
versioning** (`Asp.Versioning`, e.g. `/api/v1/...`, one Swagger doc per version). The frontend
generates TypeScript types from `/swagger/v1/swagger.json` using `openapi-typescript`, and keeps
hand-written Zod schemas (in `apps/frontend/src/contracts`) for runtime validation.

**Rationale**:
- Recovers most of the anti-drift benefit of the previously-shared contract package across a
  now-polyglot stack: the contract has one source (the API), and the frontend regenerates types
  from it. The committed `contracts/openapi.yaml` remains the design-time contract of record.

**Alternatives considered**:
- **Hand-duplicating DTOs in TS**: guarantees drift — rejected.
- **NSwag client generation**: heavier; `openapi-typescript` (types only) is lighter and pairs with
  the existing fetch-based client.

---

## 8. Authentication & authorization

**Decision**: Backend owns auth. Email/password with the ASP.NET Core `PasswordHasher` (framework,
no third-party), plus optional Google OAuth. Issue short-lived JWT access + refresh tokens in
**httpOnly, Secure, SameSite** cookies; `JwtBearer` is configured to read the token from the
cookie (OnMessageReceived). Authorization via policies/handlers that re-derive identity and
resource ownership server-side.

**Rationale**: Buyers need no account (FR-008); only sellers/admins authenticate. httpOnly cookies
keep tokens out of JS (XSS-resistant) and work with Next.js SSR for SEO. Ownership/role checks live
in the application layer (Constitution VI).

**Alternatives considered**:
- **ASP.NET Core Identity (full)**: more than needed (UI, stores); we use only its `PasswordHasher`.
- **Bearer token in localStorage**: XSS-exposed and not SSR-friendly. Rejected.

---

## 9. Image storage & upload

**Decision**: S3-compatible object storage (Cloudflare R2 in prod; MinIO in dev via
docker-compose). Uploads via backend-issued **presigned URLs** (AWSSDK.S3 against the S3-compatible
endpoint); the DB stores image URLs + metadata. (Unchanged intent from rev. 1.)

**Rationale**: Keeps large files off the API process; R2 avoids egress cost. Content-type/size
validated when issuing the presign and on listing submission.

**Alternatives considered**: DB/disk storage (doesn't scale), Cloudinary (extra cost) — rejected/
deferred.

---

## 10. Boost lifecycle, ranking, i18n (unchanged)

- **Boost lifecycle** (FR-024–FR-027): explicit `REQUESTED → ACTIVE → EXPIRED` model; seller
  requests, admin (or rule) activates after offline payment, background job expires. Activation is
  an isolated use-case so an automated payment trigger can replace the manual step later. In .NET
  the expiry job runs as a hosted `BackgroundService`.
- **Ranking** (FR-006, SC-006): a single Dapper SQL query returns only `ACTIVE` listings ordered
  boosted-first (by boost priority, then start), then the user sort (recency / price / FTS rank).
- **i18n** (FR-010): frontend `next-intl`, VI default, cookie-persisted; category labels stored
  bilingually in the DB; user content stored as-authored.

---

## 11. Observability, testing, formatting, migrations safety

**Decision**:
- **Logging**: Serilog with compact JSON output + ASP.NET request logging; enrichers add a
  correlation/request id and the authenticated user id — trace tags per constitution.
- **Errors**: global exception middleware maps domain/validation/not-found errors to the standard
  error envelope; nothing swallowed.
- **Testing**: xUnit unit tests for pure domain; integration tests against a real Postgres via
  Testcontainers + `WebApplicationFactory`; Playwright E2E for P1 on the frontend.
- **Formatting**: `dotnet format` with a repo `.editorconfig` (backend); Prettier/ESLint
  (frontend). Enforced pre-merge.
- **Migrations**: DbUp forward-only SQL scripts, committed separately from dependent code and
  sequenced expand→migrate→contract.

**Rationale**: Directly satisfies the constitution's fail-loud, structured-logging, real-data
testing, formatting-convention, and migration-safety requirements.

---

## Resolved unknowns summary

| Unknown | Resolution |
|---------|------------|
| Backend framework | .NET 10 (ASP.NET Core), DDD layered, CQRS |
| Data access | EF Core (writes) + Dapper (reads); Repository + Unit of Work |
| Migrations | EF Core auto-generated migrations |
| IoC / mapping / mediator | Autofac / AutoMapper 13.0.1 / martinothamar Mediator (source-gen) |
| Search engine | Postgres native FTS (tsvector/GIN + pg_trgm) |
| App orchestration | Plain command/query handlers (no MediatR) |
| Mapping / validation | Manual mapping + FluentValidation |
| Cross-stack contract | OpenAPI (API-emitted) + openapi-typescript on the frontend |
| Auth | JWT in httpOnly cookies; ASP.NET PasswordHasher; Google optional |
| Image storage | S3-compatible (R2/MinIO) via presigned URLs |
| Boost payment (v1) | Manual admin/auto activation; hosted expiry BackgroundService |
| Logging | Serilog compact JSON + correlation/user enrichers |
| Formatting | dotnet format + .editorconfig (backend); Prettier/ESLint (frontend) |
| Hosting | Linux containers; managed Postgres |

No NEEDS CLARIFICATION items remain. Ready for Phase 1.

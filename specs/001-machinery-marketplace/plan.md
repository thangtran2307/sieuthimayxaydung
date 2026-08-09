# Implementation Plan: Construction Machinery Marketplace (Foundation / MVP)

**Branch**: `001-machinery-marketplace` | **Date**: 2026-08-09 (rev. 3 — EF+Dapper, Autofac, CQRS) | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-machinery-marketplace/spec.md`

## Summary

Build a bilingual (VI/EN) classifieds marketplace for construction machinery where buyers
discover and contact sellers, sellers register and publish listings (gated by per-listing
moderation), the business monetizes via manually-confirmed listing boosts, and admins moderate.

Technical approach (rev. 3): a **.NET 10 (ASP.NET Core) backend** organized by **Domain-Driven
Design** layers with a **CQRS** command/query split. Persistence uses **EF Core for the write side
and auto-generated migrations** and **Dapper for the read side** over **PostgreSQL** (JSONB for
flexible specs via a `ListingSpecs` value object mapped OwnsOne→jsonb; native `tsvector` full-text
search via `HasGeneratedTsVectorColumn`). It uses the **Repository + Unit of Work** patterns,
**Autofac** as the IoC container, **AutoMapper** for mapping, and a free source-generated mediator
(martinothamar/Mediator). Aggregates derive from a base `Entity` (Id + domain events). The
**Next.js + TypeScript** frontend (already scaffolded) stays as the SEO-first web client. The
cross-stack API contract is the backend's generated **OpenAPI** document; the frontend generates
its types from it (via `openapi-typescript`) and keeps Zod schemas for runtime response
validation. Code style is enforced by `dotnet format` (+ `.editorconfig`) on the backend and
Prettier/ESLint on the frontend.

> **Why the change**: The user prefers .NET (greater experience). rev. 2 chose Dapper-only; rev. 3
> refines this to **EF Core (writes + auto-generated migrations) + Dapper (reads)** — a CQRS
> read/write split — plus Autofac, AutoMapper, Repository + Unit of Work, and a CQRS mediator, per
> user direction. Losing a shared TypeScript contract is mitigated by generating frontend types
> from the backend OpenAPI. See research.md §1–§6.

**Backend build status**: the solution compiles (0 errors) and the EF `InitialCreate` migration is
generated. Applying migrations/seed and running the API need a live PostgreSQL (unavailable in the
build environment) — see quickstart.md.

## Technical Context

**Language/Version**:
- Backend: C# 13 on .NET 10 (SDK 10.0.201)
- Frontend: TypeScript 5.x on Node.js 20+

**Primary Dependencies**:
- Backend: ASP.NET Core (Web API); **EF Core + Npgsql.EntityFrameworkCore.PostgreSQL**
  (write side + migrations) + **EFCore.NamingConventions** (snake_case); **Dapper** (read side);
  **Autofac** (IoC) + **AutoMapper** (mapping); **Mediator** (martinothamar, source-generated CQRS);
  **FluentValidation** (boundary validation); **Serilog** (structured JSON logging);
  `Microsoft.AspNetCore.Authentication.JwtBearer` (+ optional Google); `Microsoft.AspNetCore.OpenApi`.
  MediatR was avoided (paid) in favor of the free source-gen mediator; AutoMapper pinned to the last
  MIT version (13.0.1). See research §2–§6.
- Frontend: Next.js 15 (App Router), React 19, Tailwind CSS, shadcn/ui, next-intl, plus
  `openapi-typescript` for generated API types. Frontend-side Zod schemas/enums (in
  `apps/frontend/src/contracts`) mirror the wire contract for runtime validation.

**Storage**: PostgreSQL 16 — JSONB for listing specifications, `tsvector` + GIN for full-text
search, `pg_trgm` for fuzzy matching, native enums mapped via Npgsql. Object storage
(S3-compatible, e.g. Cloudflare R2 / MinIO in dev) for listing images, referenced by URL.

**Testing**:
- Backend: xUnit for unit tests (pure domain), integration tests against a real Postgres via
  **Testcontainers for .NET** + `WebApplicationFactory`. Fixture/seed data over mocks per
  constitution.
- Frontend: Vitest (unit) + Playwright (E2E for the P1 buyer flow).

**Target Platform**: Linux server / containers (Docker). Backend published as a self-contained /
framework-dependent container; frontend to Vercel or a container; managed Postgres (Neon /
Supabase / RDS) or self-hosted.

**Project Type**: Web application — two independent projects in one repository: `apps/backend`
(.NET solution) and `apps/frontend` (self-contained Next.js app). No JS monorepo/workspace — the
frontend is the only TypeScript project; future scale-out is planned as separate C# microservices,
not JS packages.

**Performance Goals**: Filtered search results in < 2s at expected catalog size (SC-003); read API
p95 < 300ms; listing detail p95 < 400ms including gallery metadata.

**Constraints**: No online payment gateway in v1 (boosts manually confirmed); bilingual VI/EN with
persisted preference; responsive mobile + desktop for all P1 flows; no buyer accounts / chat inbox
in v1.

**Scale/Scope**: Early-stage catalog — thousands of listings, growing. ~7 primary frontend screens
and ~5 backend bounded contexts (Catalog, Identity, Moderation, Promotion, Inquiry). Postgres FTS
is sufficient at this scale (no external search engine).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Plan compliance |
|-----------|-----------------|
| I. API Consistency & Contracts | REST with resource nouns; standardized `{ data, meta }` / `{ error: { code, message, details? } }` envelope via ASP.NET result filter + exception middleware; OpenAPI generated from the API. **PASS** |
| II. Zero-Trust Data Validation | FluentValidation validators run at the API boundary; pagination clamped; identity/ownership re-derived from the authenticated principal, never trusted from the client. **PASS** |
| III. Fail Loud, Never Silent | Global exception middleware → structured error envelope + Serilog error log with stack; no swallowed exceptions; missing IDs → 404, validation → 422. **PASS** |
| IV. Explicit Dependencies & Deletable Modules | DDD layered projects with constructor DI; no static/global mutable state; no import-time side effects. Layering is a *deliberate* justified deviation from "inline until 3×" — see Complexity Tracking. **PASS (with justification)** |
| V. Clean, Typed, Layered Code | Nullable reference types on, C# analyzers, DDD layers (Domain / Application / Infrastructure / Api); domain pure and framework-independent. **PASS** |
| VI. Security & Privacy by Default | JWT-in-httpOnly-cookie auth; role + ownership checks in the application layer; DTOs exclude credentials/other users' private contact; secrets via env / user-secrets. **PASS** |
| VII. Performance & Scalability | Hand-written Dapper SQL selecting only needed columns; indexed filter/sort columns; GIN index for FTS; mandatory pagination; no N+1 (single tuned queries / batched loads). **PASS** |
| Ops: structured JSON logging | Serilog compact JSON + request logging with correlation-id and user-id enrichers. **PASS** |
| Ops: minimize external deps | Dapper (not a heavy ORM); Postgres FTS (not Elasticsearch); no MediatR/AutoMapper (commercial licensing) — plain handlers + manual mapping; no payment SDK in v1. **PASS** |
| Ops: migrations expand→migrate→contract | DbUp forward-only versioned SQL scripts, committed separately from dependent code. **PASS** |
| Workflow: real/fixture data over mocks | Integration tests hit a real Postgres via Testcontainers. **PASS** |
| Workflow: formatting convention | `dotnet format` + `.editorconfig` (backend); Prettier/ESLint (frontend), enforced in CI/pre-merge. **PASS** |

**Result**: PASS (one justified deviation in Complexity Tracking). Proceed to Phase 0.

## Project Structure

### Documentation (this feature)

```text
specs/001-machinery-marketplace/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (README + openapi.yaml — the cross-stack contract)
└── tasks.md             # Phase 2 output (/speckit-tasks)
```

### Source Code (repository root)

```text
sieuthimayxaydung/
├── apps/
│   ├── backend/                              # .NET 10 solution (Marketplace.sln)
│   │   ├── Directory.Build.props             # shared build settings (nullable, analyzers, langversion)
│   │   ├── .editorconfig                     # code style enforced by `dotnet format`
│   │   ├── src/
│   │   │   ├── Marketplace.Domain/           # entities, value objects, enums (pure; no deps)
│   │   │   │   └── {Catalog,Identity,Moderation,Promotion,Inquiry}/
│   │   │   ├── Marketplace.Application/       # use-cases (command/query handlers), port
│   │   │   │   │                             #   interfaces, DTOs, FluentValidation validators
│   │   │   │   └── {Catalog,Identity,Moderation,Promotion,Inquiry}/
│   │   │   ├── Marketplace.Infrastructure/    # Dapper repositories (Npgsql), DbUp migrations,
│   │   │   │   ├── Persistence/               #   connection factory, repository impls, SQL
│   │   │   │   ├── Migrations/                #   DbUp embedded .sql scripts (forward-only)
│   │   │   │   ├── Auth/                      #   JWT, password hasher, cookie helpers
│   │   │   │   ├── Storage/                   #   S3-compatible image storage
│   │   │   │   └── Logging/                   #   Serilog config
│   │   │   └── Marketplace.Api/               # ASP.NET Core host: controllers, middleware,
│   │   │       ├── Controllers/               #   filters (envelope), DI composition, OpenAPI
│   │   │       ├── Middleware/
│   │   │       └── Program.cs
│   │   └── tests/
│   │       ├── Marketplace.Domain.Tests/      # xUnit unit tests (pure domain)
│   │       └── Marketplace.Integration.Tests/ # Testcontainers Postgres + WebApplicationFactory
│   │
│   └── frontend/                             # Next.js + TypeScript (self-contained app)
│       ├── package.json / tsconfig.json / .prettierrc.json   # own tooling (no root workspace)
│       └── src/
│           ├── {app,components,features,lib,i18n,messages}
│           └── contracts/                    # frontend Zod schemas/enums mirroring the wire
│                                             #   contract (+ generated OpenAPI types in lib/api)
│
└── docker-compose.yml                        # local Postgres + MinIO (shared infra)
```

**Structure Decision**: Two independent projects in one repository — a **.NET DDD solution** in
`apps/backend` and a **self-contained Next.js app** in `apps/frontend`. There is no JS
monorepo/workspace: the frontend is the only TypeScript project, so pnpm/Turborepo and the shared
`packages/contracts` were removed (the contract schemas now live in `apps/frontend/src/contracts`).
The backend uses classic four-layer DDD (Domain → Application → Infrastructure → Api) with
bounded-context folders per layer (Catalog, Identity, Moderation, Promotion, Inquiry); Dapper keeps
data access as explicit, tunable SQL. Each project builds with its own toolchain (`dotnet` CLI;
`next`/`pnpm`). The cross-stack contract is the OpenAPI document emitted by the API and consumed by
the frontend via codegen. Future scale-out is planned as additional C# microservices.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Four-project DDD layering + repository pattern from day one, rather than inlining until a pattern repeats 3× | The user's primary goal is long-term scale and maintainability; the domain (listing lifecycle, moderation, boost ranking, ownership rules) is genuinely non-trivial and multi-actor. Layering keeps domain rules pure/testable and isolates Dapper/SQL behind repository ports. | A single-project CRUD API directly over Dapper would be faster initially but mixes SQL, business rules, and HTTP — contradicting "separate state from presentation," making ownership/state-transition rules hard to test in isolation, and raising rework risk as the catalog grows. Deviation is scoped: simple read queries use thin query handlers; heavy layering applies to mutation/domain-rich flows. |
| No single shared cross-stack DTO package (backend C#, frontend TS) | The user chose .NET for data-access control and familiarity; a shared TS contract package is not possible across languages. | Instead of hand-syncing DTOs, the API emits an OpenAPI document and the frontend generates types from it (`openapi-typescript`) — preserving the anti-drift benefit at the contract boundary. Duplicating DTO definitions by hand was rejected for the same drift reason that motivated the original shared package. |

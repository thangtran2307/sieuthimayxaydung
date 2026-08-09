---
description: "Task list for Construction Machinery Marketplace (Foundation / MVP) — rev. 3 (.NET backend)"
---

# Tasks: Construction Machinery Marketplace (Foundation / MVP)

**Input**: Design documents from `/specs/001-machinery-marketplace/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Stack (rev. 3)**: Backend = **.NET 10 (ASP.NET Core)**, DDD layers, **CQRS** via source-generated
mediator (martinothamar/Mediator), **Autofac** DI, **AutoMapper**, **Repository + Unit of Work**.
Persistence: **EF Core (write side + auto-generated migrations)** + **Dapper (read side)** over
**PostgreSQL**; base `Entity` with domain events; nullable disabled; SonarAnalyzer + `dotnet format`.
Frontend = **Next.js + TypeScript** (already built). Cross-stack contract = OpenAPI.

**Tests**: Included — the constitution mandates real/fixture-data tests (Testcontainers Postgres
integration + xUnit unit on the backend; Playwright E2E for the P1 flow on the frontend).

**Status legend**: `[X]` done · `[ ]` pending. Frontend + shared foundation from rev. 1 is already
built; the backend was re-scoped from NestJS to .NET and is pending.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files/projects, no dependency on incomplete tasks)
- **[Story]**: US1 / US2 / US3 / US4 (maps to spec.md user stories)
- Backend paths under `apps/backend/`; frontend under `apps/frontend/`; shared TS under `packages/contracts/`.

---

## Phase 1: Setup (Shared Infrastructure)

- [X] T001 Repository layout — two independent projects (`apps/backend` .NET, `apps/frontend` Next.js); no JS monorepo/workspace (removed pnpm-workspace/turbo/root package.json)
- [X] T002 [P] Scaffold `apps/frontend` — self-contained Next.js 15 (App Router) + TypeScript + Tailwind + shadcn/ui foundations, with its own `package.json`/`tsconfig.json`/Prettier
- [X] T003 [P] Frontend contract schemas folded into `apps/frontend/src/contracts` (Zod enums/pagination/envelope); `gen:api` (openapi-typescript) script wired
- [X] T004 Create .NET solution `apps/backend/Marketplace.slnx` with projects `Marketplace.Domain`, `Marketplace.Application`, `Marketplace.Infrastructure`, `Marketplace.Api`, and test projects `Marketplace.Domain.Tests`, `Marketplace.Integration.Tests`; DDD project references wired
- [X] T005 [P] Backend build/style conventions: `apps/backend/Directory.Build.props` (net10.0, nullable disabled, implicit usings) + `apps/backend/.editorconfig` + SonarAnalyzer (advisory); `dotnet format` runs
- [X] T006 [P] Frontend strict TS config + ESLint (next) + Prettier in `apps/frontend`
- [X] T007 [P] Add `docker-compose.yml` (PostgreSQL 16 + MinIO) and `.env.example` files
- [X] T008 Frontend scripts (dev/build/lint/typecheck/test/format/gen:api) in `apps/frontend/package.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**⚠️ CRITICAL**: No user story work begins until this phase is complete.

### Frontend & shared (already built)

- [X] T009 [P] Frontend contract common schemas — response envelope, error envelope, pagination, shared enums (Zod) in `apps/frontend/src/contracts/common/`
- [X] T010 [P] Frontend API client using `@/contracts` with response validation + SSR cookie forwarding in `apps/frontend/src/lib/api/`
- [X] T011 [P] Frontend i18n — next-intl VI/EN catalogs + locale-cookie persistence in `apps/frontend/src/i18n/` and `src/messages/`
- [X] T012 [P] Frontend base layout + shadcn theme (steel-blue/safety-orange) + header with language toggle in `apps/frontend/src/app/` and `src/components/`

### Backend (.NET) — complete

- [X] T013 Domain shared kernel — all enums, base `Entity` (Id + domain events via `IDomainEvent`), `IClock`, `ValidationError`, `ErrorCodes`, exception hierarchy, and the entity model per bounded context (`Marketplace.Domain/{Common,Identity,Catalog,Promotion,Moderation,Inquiry}`) with private setters + `ListingSpecs` value object
- [X] T014 Persistence base — EF Core `MarketplaceDbContext` (snake_case naming, `timestamp without time zone` / UTC via legacy timestamp behavior) + per-entity `IEntityTypeConfiguration` files in `Persistence/Configurations/`; Dapper reads use the DbContext connection (`NpgsqlBootstrap` sets Dapper underscore mapping). `IDbConnectionFactory` removed per feedback
- [X] T015 EF Core migrations — auto-generated `InitialCreate` (all tables, FKs, indexes, enum→text conversions, **jsonb** `specs` via OwnsOne+ToJson, **generated `tsvector`** via `HasGeneratedTsVectorColumn` + GIN + `pg_trgm` per Npgsql docs); `migrate`/`seed` CLI verbs in `Marketplace.Api`
- [X] T016 [P] Application ports — `IRepository<T>` (Repository pattern), `IUnitOfWork`, `IDatabaseMigrator`, `IDataSeeder`, `IPasswordHasher`, `IJwtTokenService`, and `PagedResult<T>` in `Marketplace.Application/Common/`; EF impls `EfRepository`/`EfUnitOfWork`
- [X] T017 [P] Api response envelope — `EnvelopeResultFilter` wrapping `{ data }` / paged `{ data, meta }` in `Marketplace.Api/Http/`
- [X] T018 [P] Api exception middleware — maps domain/validation/not-found → `{ error: { code, message, details } }`; logs unexpected errors with stack (fail loud)
- [X] T019 [P] Api validation — `FluentValidationFilter` running registered validators at the boundary (422 + field details)
- [X] T020 [P] Logging — Serilog compact JSON + request logging (correlation id) wired in `Program.cs`
- [X] T021 Auth infrastructure — JWT issue/validate from the `access_token` httpOnly cookie, ASP.NET `PasswordHasher`, `RolesGuard`/`Admin` policy, cookie helpers
- [X] T022 [P] Options/config — bind + validate `Jwt`/`Storage` (+ `ConnectionStrings:Postgres`, `FrontendUrl`) with `ValidateOnStart`
- [X] T023 [P] Host wiring — Autofac service-provider factory + `InfrastructureModule`, source-generated Mediator (`AddMediator`), AutoMapper, `Microsoft.AspNetCore.OpenApi` at `/openapi/v1.json`, `/api` prefix convention, CORS to `FrontendUrl`
- [X] T024 [P] Seed command — `seed` verb populating categories/subcategories, boost packages, and an admin (Dapper over the DbContext connection)
- [X] T025 [P] Health endpoint — `GET /api/health` (DB connectivity + a CQRS `PingQuery` proving the mediator/Autofac wiring)
- [X] T026 [P] Frontend codegen — `openapi-typescript` `gen:api` script wired (generation runs once the backend is up)

**Checkpoint**: Foundation ready — solution builds (0 errors); user stories can begin.

> **Note**: Applying migrations + seeding (`dotnet run --project src/Marketplace.Api -- migrate|seed`)
> and running the API require a live PostgreSQL (Docker was unavailable in the build environment),
> so those runtime steps are pending a database. Compilation, EF model, and migration generation
> are verified.

---

## Phase 3: User Story 1 - Discover and contact a seller (Priority: P1) 🎯 MVP

**Goal**: Buyers browse/search/filter listings, view detail, and contact the seller — no account.

**Independent Test**: With seeded ACTIVE listings, run a category+keyword search with a filter, open
a listing, reveal phone / send a message, report a listing; boosted listings pinned above others.

### Tests for User Story 1

- [ ] T027 [P] [US1] Integration tests (Testcontainers) — `GET /listings` search/filter/sort/paginate, ACTIVE-only, boosted-first, clamped pagination in `apps/backend/tests/Marketplace.Integration.Tests/Catalog/`
- [ ] T028 [P] [US1] Integration tests — `GET /listings/{slug}`, create inquiry (phone reveal + message), create report in `apps/backend/tests/Marketplace.Integration.Tests/`
- [ ] T029 [P] [US1] Playwright E2E — home → search → filter → detail → contact in `apps/frontend/tests/e2e/discover-contact.spec.ts`

### Implementation for User Story 1

- [ ] T030 [P] [US1] Domain — `Listing`, `Category`, `ListingPhoto`, `Inquiry`, `Report` entities/value objects + boosted-first ranking rule in `Marketplace.Domain/{Catalog,Inquiry,Moderation}/`
- [ ] T031 [P] [US1] Application — DTOs + `SearchListings`/`GetListingBySlug` queries, `CreateInquiry`, `CreateReport` handlers, validators, and repository ports in `Marketplace.Application/{Catalog,Inquiry,Moderation}/`
- [ ] T032 [US1] Infrastructure — Dapper `CatalogRepository` (FTS + boosted-first ordering + filters + clamped paging), `InquiryRepository`, `ReportRepository` with SQL in `Marketplace.Infrastructure/Persistence/` (depends on T030, T031)
- [ ] T033 [US1] Api — `CategoriesController`, `ListingsController` (search + detail, view-count increment), `InquiriesController`, `ReportsController` in `Marketplace.Api/Controllers/` (depends on T032)
- [ ] T034 [P] [US1] Frontend contracts — category/listing/search/inquiry/report Zod schemas in `packages/contracts/src/` + regenerate OpenAPI types
- [ ] T035 [P] [US1] Frontend homepage — hero search, category grid, promoted/featured row, recent listings in `apps/frontend/src/app/[locale]/page.tsx`
- [ ] T036 [P] [US1] Frontend search page — filter sidebar, sort, results grid with promoted badge, pagination in `apps/frontend/src/app/[locale]/search/page.tsx`
- [ ] T037 [US1] Frontend listing detail — gallery, specs, seller card, contact (phone reveal + message form), report in `apps/frontend/src/app/[locale]/listing/[slug]/page.tsx`
- [ ] T038 [US1] Frontend empty-state + "no longer available" state + US1 i18n strings

**Checkpoint**: US1 fully functional — searchable catalog connecting buyers to sellers (MVP).

---

## Phase 4: User Story 2 - Register as a seller and publish a listing (Priority: P2)

**Goal**: Register/sign-in required to post; guided listing (photos + fields) → PENDING moderation;
seller dashboard to manage listings.

**Independent Test**: Register, submit a complete listing with a photo → PENDING & not public;
invalid submission blocked with field errors; dashboard shows status and blocks editing others'.

### Tests for User Story 2

- [ ] T039 [P] [US2] Integration — register → create listing → PENDING (not in public search); ownership 403 on another seller's listing in `apps/backend/tests/Marketplace.Integration.Tests/Publish/`
- [ ] T040 [P] [US2] Integration — validation 422 preserves input; presign rejects unsupported/oversized media

### Implementation for User Story 2

- [ ] T041 [P] [US2] Domain — `User` + role + credential value objects; listing create/edit → PENDING transition rules in `Marketplace.Domain/Identity/` and `Catalog/`
- [ ] T042 [US2] Application — `Register`/`Login`/`Logout`/`Refresh`/`Me` + `CreateListing`/`UpdateListing`/`DeleteListing`/`MarkSold` + dashboard queries, with validators and ownership checks in `Marketplace.Application/{Identity,Catalog}/`
- [ ] T043 [US2] Infrastructure — Dapper `UserRepository`, listing write SQL, presigned upload via `AWSSDK.S3`, password hashing wiring in `Marketplace.Infrastructure/`
- [ ] T044 [US2] Api — `AuthController` (sets/clears cookies), `ListingsController` writes + `mark-sold`, `MeController` (`/me/listings`, `/me/dashboard`), `UploadsController` (`/uploads/presign`) in `Marketplace.Api/Controllers/`
- [ ] T045 [P] [US2] Optional Google OAuth via `Microsoft.AspNetCore.Authentication.Google` in `Marketplace.Infrastructure/Auth/`
- [ ] T046 [P] [US2] Frontend auth — login/register page (tabs, Google, show/hide password) + session handling in `apps/frontend/src/app/[locale]/login/`
- [ ] T047 [US2] Frontend post-listing — guided form, photo upload via presign, inline validation in `apps/frontend/src/app/[locale]/post/page.tsx`
- [ ] T048 [US2] Frontend seller dashboard — KPIs + listings table with status, edit/delete/mark-sold in `apps/frontend/src/app/[locale]/dashboard/page.tsx`

**Checkpoint**: US1 + US2 both work independently.

---

## Phase 5: User Story 3 - Moderate listings and sellers (Priority: P3)

**Goal**: Admins review the queue, approve/reject/remove, resolve reports, act in bulk. Admin-only.

**Independent Test**: As the seeded admin, approve a pending listing → public; reject another →
hidden; resolve a report with removal → REMOVED; non-admin gets 403.

### Tests for User Story 3

- [ ] T049 [P] [US3] Integration — approve→ACTIVE/public, reject→hidden, bulk action, resolve report→REMOVED, non-admin 403 in `apps/backend/tests/Marketplace.Integration.Tests/Moderation/`

### Implementation for User Story 3

- [ ] T050 [P] [US3] Domain — `ModerationDecision`, report-resolution rules, listing status transitions (approve/reject/remove) in `Marketplace.Domain/Moderation/`
- [ ] T051 [US3] Application — `ApproveListing`/`RejectListing`/`RemoveListing`/`BulkModerate`/`ResolveReport` handlers + queue queries + validators in `Marketplace.Application/Moderation/`
- [ ] T052 [US3] Infrastructure — Dapper `ModerationRepository` + decision-audit SQL in `Marketplace.Infrastructure/Persistence/`
- [ ] T053 [US3] Api — admin moderation controllers (queue, approve/reject/remove, bulk, reports, resolve) behind the ADMIN policy in `Marketplace.Api/Controllers/Admin/`
- [ ] T054 [US3] Frontend admin panel — moderation queue, approve/reject/remove, bulk actions, reports view in `apps/frontend/src/app/[locale]/admin/page.tsx`

**Checkpoint**: Listings can go public safely — trust layer complete.

---

## Phase 6: User Story 4 - Promote a listing with a paid boost (Priority: P4)

**Goal**: Seller requests a boost for an ACTIVE listing; admin activates after offline payment (v1
manual); boosted listings rank first and revert on expiry.

**Independent Test**: Request boost on ACTIVE→REQUESTED (non-ACTIVE→422, dup→409); admin activate→
ranks first; after expiry it reverts.

### Tests for User Story 4

- [ ] T055 [P] [US4] Integration — request rules (ACTIVE-only, no dup), activate→ranks first, expiry service→revert in `apps/backend/tests/Marketplace.Integration.Tests/Promotion/`

### Implementation for User Story 4

- [ ] T056 [P] [US4] Domain — `Boost` + `BoostPackage` + lifecycle rules (REQUESTED→ACTIVE→EXPIRED, one active per listing, ACTIVE-listing guard) in `Marketplace.Domain/Promotion/`
- [ ] T057 [US4] Application — `RequestBoost`/`ActivateBoost`/`ExpireBoosts` handlers + package queries + validators in `Marketplace.Application/Promotion/`
- [ ] T058 [US4] Infrastructure — Dapper `PromotionRepository` + boost SQL in `Marketplace.Infrastructure/Persistence/`
- [ ] T059 [US4] Api — `GET /boost-packages`, `POST /listings/{id}/boost-requests`, admin list + activate; hosted `BackgroundService` expiring boosts in `Marketplace.Api/` and `Marketplace.Infrastructure/`
- [ ] T060 [P] [US4] Frontend — boost packages UI + request-boost action on the dashboard in `apps/frontend/src/features/promotion/`
- [ ] T061 [US4] Frontend admin — boost-request activation view (enter payment reference, activate) in `apps/frontend/src/app/[locale]/admin/boosts/page.tsx`

**Checkpoint**: All four user stories independently functional — monetization live.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [ ] T062 [P] Backend unit tests (xUnit) — ranking rule, boost lifecycle, price/pagination validation in `apps/backend/tests/Marketplace.Domain.Tests/`
- [ ] T063 [P] Accessibility + responsive pass for all P1 buyer pages in `apps/frontend/`
- [ ] T064 [P] Security hardening — rate limiting on inquiries/reports/auth, security headers, secret-in-config audit in `Marketplace.Api/`
- [ ] T065 [P] SEO — per-page metadata, `sitemap.xml`, listing JSON-LD in `apps/frontend/src/app/`
- [ ] T066 [P] Documentation — root `README.md` + run docs referencing `quickstart.md`
- [ ] T067 Performance — verify filtered search < 2s at seed scale; audit Dapper SQL, indexes, and N+1 in `Marketplace.Infrastructure/`
- [ ] T068 Gates green — `dotnet format --verify-no-changes`, `dotnet build`, `dotnet test`, `pnpm typecheck && pnpm lint`; run `quickstart.md` end-to-end (all US scenarios + SC-001…SC-010)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: no dependencies. Remaining: T004, T005 (.NET solution + conventions).
- **Foundational (Phase 2)**: frontend/shared done; **backend T013–T025 block all backend user
  stories**; T026 (codegen) depends on T023 (OpenAPI) being runnable.
- **User Stories (Phase 3–6)**: depend on Foundational; then independent of each other.
- **Polish (Phase 7)**: after the desired user stories.

### User Story Dependencies

- **US1 (P1)**: after backend Foundational. No dependency on other stories (uses seeded listings).
- **US2 (P2)**: after Foundational. Independent; shares the Catalog context with US1 (write side).
- **US3 (P3)**: after Foundational. Independent (seeded admin + seeded pending listings).
- **US4 (P4)**: after Foundational. Independent (seeded ACTIVE listings + admin).

### Within Each User Story (backend)

- Tests first (write, see them fail) → Domain → Application (ports + handlers + validators) →
  Infrastructure (Dapper repos + SQL) → Api (controllers) → Frontend.

### Parallel Opportunities

- Backend Foundational `[P]` tasks (T016–T020, T022–T025) run together after T013–T015; T021 (auth)
  after T013–T014; T026 after T023.
- After Foundational, US1–US4 can proceed in parallel if staffed.
- Within a story, `[P]` items touch different projects/files (e.g. US1: Domain T030, Application
  T031, frontend pages T035/T036 in parallel; T032 depends on T030+T031; T033 on T032).

---

## Implementation Strategy

### MVP First (User Story 1)

1. Finish Setup (T004–T005) and backend Foundational (T013–T026).
2. Complete US1 (T027–T038).
3. **STOP and VALIDATE** with the US1 quickstart scenario; deploy/demo the MVP.

### Incremental Delivery

Foundational → US1 (discovery + contact) → US2 (publish, moderation-gated) → US3 (moderation) →
US4 (boosts). Each story is an independently testable, deployable increment.

---

## Notes

- `[P]` = different files/projects, no dependency on incomplete tasks.
- Domain stays pure; Dapper/SQL is confined to `Marketplace.Infrastructure`; the frontend consumes
  the API via generated OpenAPI types + Zod runtime validation (no shared code across languages).
- Integration tests run against a real Postgres (Testcontainers) — no heavy mocks (constitution).
- DbUp migrations are forward-only SQL, committed separately from dependent code
  (expand→migrate→contract).
- Backend style is enforced by `dotnet format` (+ `.editorconfig`); frontend by Prettier/ESLint.

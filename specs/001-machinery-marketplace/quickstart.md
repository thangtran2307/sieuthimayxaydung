# Quickstart & Validation Guide: Construction Machinery Marketplace

**Date**: 2026-08-09 (rev. 2 — .NET backend) | **Feature**: 001-machinery-marketplace

How to run the app locally and validate the feature end-to-end. Implementation details (domain
types, SQL, handlers, full test suites) live in `tasks.md` and the implementation phase.

## Prerequisites

- **.NET SDK 10** (backend) — verify with `dotnet --version` (10.0.x)
- **Node.js 20+** and **pnpm** (`corepack enable`) — frontend
- **Docker** — local PostgreSQL 16 and MinIO (S3-compatible) via `docker-compose.yml`
- A modern browser

## One-time setup

```bash
# from repo root
docker compose up -d                 # start Postgres (:5432) + MinIO (:9000/:9001)

# Frontend workspace
pnpm install
cp apps/frontend/.env.example apps/frontend/.env.local

# Backend config (never commit secrets — use user-secrets or environment variables)
cd apps/backend
dotnet restore
dotnet user-secrets init --project src/Marketplace.Api
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=smxd;Username=smxd;Password=smxd" --project src/Marketplace.Api
dotnet user-secrets set "Jwt:AccessSecret" "dev-access-secret-change-me" --project src/Marketplace.Api
dotnet user-secrets set "Jwt:RefreshSecret" "dev-refresh-secret-change-me" --project src/Marketplace.Api
```

Key backend settings (via user-secrets / env; see `appsettings.json` for non-secret defaults):

| Setting | Purpose |
|---------|---------|
| `ConnectionStrings:Postgres` | Npgsql connection string |
| `Jwt:AccessSecret` / `Jwt:RefreshSecret` | Session token signing |
| `Jwt:AccessTtl` / `Jwt:RefreshTtl` | Token lifetimes |
| `FrontendUrl` | Allowed CORS origin |
| `Storage:*` (endpoint, bucket, keys) | S3-compatible image storage |
| `Google:ClientId` / `Google:ClientSecret` | Optional Google OAuth |

Frontend: `NEXT_PUBLIC_API_URL=http://localhost:5xxx/api` (the backend URL).

## Database migrate + seed

```bash
# Migrations run via DbUp — either on API startup (dev) or the migrator entrypoint:
dotnet run --project apps/backend/src/Marketplace.Api -- migrate     # apply DbUp SQL scripts
dotnet run --project apps/backend/src/Marketplace.Api -- seed        # seed reference + demo data
```

Seed mirrors the prototype: the six categories with subcategories, boost packages
(Basic / Featured / Max), an admin account, demo sellers, and sample listings (some boosted).

## Run

```bash
# Backend (terminal 1)
dotnet run --project apps/backend/src/Marketplace.Api          # https://localhost:5xxx/api

# Frontend (terminal 2)
pnpm --filter @repo/frontend dev                               # http://localhost:3000
```

Generate frontend API types from the running backend's OpenAPI (optional but recommended):

```bash
pnpm --filter @repo/frontend gen:api      # openapi-typescript from /openapi/v1.json
```

## Automated checks

```bash
# Backend
dotnet build apps/backend/Marketplace.sln
dotnet test apps/backend/Marketplace.sln        # xUnit unit + Testcontainers integration
dotnet format apps/backend/Marketplace.sln --verify-no-changes   # style gate

# Frontend
pnpm typecheck && pnpm lint
pnpm --filter @repo/frontend test               # Vitest
pnpm --filter @repo/frontend test:e2e           # Playwright (P1 flow)
```

Integration tests run against a real Postgres (Testcontainers) with seed fixtures — no heavy mocks
(constitution: real/fixture data).

---

## Validation scenarios (map to spec user stories)

Run against the seeded database. Each scenario proves an independently-testable slice.

### US1 — Discover & contact (P1) · MVP slice

1. Open `http://localhost:3000` → homepage shows search, categories, a **Promoted** row, and recent
   listings. *(FR-001)*
2. Search `Komatsu`, then filter Category = Excavators, Condition = Used, Province = TP.HCM, price
   range → results update and reflect filters. *(FR-002–FR-004)*
3. A boosted match appears **above** non-boosted matches, marked "Promoted". *(FR-006, SC-006)*
4. `page` beyond the last page → empty `data` with valid meta; `page=-5`/`pageSize=9999` are
   clamped, not errored. *(FR-005)*
5. Open a listing → gallery, specs, description, location, seller, contact options. *(FR-007)*
6. "Show phone" → `POST /listings/{id}/inquiries` (`PHONE_REVEAL`) returns the seller phone;
   submit the message form (`MESSAGE`) → 201 and the seller is notified. *(FR-008)*
7. Report the listing → `POST /listings/{id}/reports` returns 201; appears in the admin queue.
   *(FR-009)*
8. Toggle VI ↔ EN → UI switches and the choice persists after reload. *(FR-010)*

**Expected**: first-time visitor reaches a relevant listing in ≤ 3 interactions (SC-001) and can
contact the seller without an account.

### US2 — Register & publish (P2)

1. Without login, "Post listing" → redirected to `/login`. *(FR-013)*
2. Register → auth cookie set; can open the post form immediately. *(FR-012, FR-013a)*
3. Submit a complete listing + ≥ 1 photo → 201, status `PENDING`, not in public search.
   *(FR-014, FR-016)*
4. Missing required field → 422 with field `details`; entered data preserved. *(FR-015)*
5. Dashboard shows the listing as `PENDING` with counts; editing your own works; editing another
   seller's returns 403. *(FR-017, FR-018)*

### US3 — Moderate (P3)

1. Log in as the seeded admin; open `/admin` → pending listings listed. *(FR-019)*
2. Approve → `ACTIVE` and public; a moderation decision is recorded. *(FR-020, SC-005)*
3. Reject another with a reason → stays non-public; seller sees rejection.
4. Resolve a reported listing with "remove" → `REMOVED`, report `RESOLVED_REMOVED`. *(FR-021)*
5. Select several and bulk-act → all update. *(FR-022)*
6. Non-admin hits `/api/admin/*` → 403. *(FR-023)*

### US4 — Boost (P4)

1. Seller with an `ACTIVE` listing: `GET /boost-packages`, then
   `POST /listings/{id}/boost-requests` → 201 `REQUESTED`. *(FR-024)*
2. Boost on a non-`ACTIVE` listing → 422; a second concurrent boost → 409. *(FR-027)*
3. Admin `POST /admin/boost-requests/{id}/activate` with a payment reference → `ACTIVE`; listing
   ranks first + promoted treatment. *(FR-025, FR-006)*
4. Advance past `expiresAt` (or run the expiry `BackgroundService`) → `EXPIRED`, ranking reverts.
   *(FR-026, SC-006)*

---

## Definition of validated

- All four user-story scenario sets pass against seeded data.
- `dotnet build`, `dotnet test`, and `dotnet format --verify-no-changes` are green;
  `pnpm typecheck && pnpm lint` and the Playwright P1 flow pass.
- Success criteria SC-001…SC-010 observably hold (search < 2s at seed scale, moderation gate
  enforced, no sensitive data in responses, invalid inputs handled, VI/EN persists, responsive).

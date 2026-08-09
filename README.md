Sieuthimayxaydung

Construction machinery marketplace — a bilingual (VI/EN) classifieds platform.

## Structure

- `apps/backend` — .NET 10 (ASP.NET Core) DDD API. EF Core (writes + migrations) + Dapper (reads) over PostgreSQL. See [apps/backend/README.md](apps/backend/README.md).
- `apps/frontend` — Next.js + TypeScript (SEO-first web client). See [apps/frontend/README.md](apps/frontend/README.md).
- `specs/` — Spec Kit feature specs, plan, and tasks.
- `prototype/` — original static design prototype.

## Run the frontend

See **[apps/frontend/README.md](apps/frontend/README.md)** for details. Quick start (from `apps/frontend`):

```bash
pnpm install
cp .env.example .env.local     # NEXT_PUBLIC_API_URL defaults to http://localhost:5127/api
pnpm dev                       # http://localhost:3000
```

## Backend database commands (migrations, update, seeding)

See **[apps/backend/README.md](apps/backend/README.md)** for the full, copy‑pasteable command list.
Quick reference (run from `apps/backend`):

```bash
docker compose up -d                                             # start Postgres (from repo root)

# generate a migration after model changes
dotnet ef migrations add <Name> --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api

# apply migrations to the database
dotnet ef database update --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api
# ...or via the app: dotnet run --project src/Marketplace.Api -- migrate

# seed reference + admin data (migrates first, then seeds)
dotnet run --project src/Marketplace.Api -- seed

# run the API (Swagger at /swagger, health at /api/health)
dotnet run --project src/Marketplace.Api
```

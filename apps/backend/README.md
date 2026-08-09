# Marketplace Backend (.NET 10)

DDD solution: `Domain` → `Application` → `Infrastructure` → `Api`.
Persistence: **EF Core** (write side + migrations) + **Dapper** (read side) over **PostgreSQL**.

> Run all commands below from the **`apps/backend`** directory unless noted.

## Prerequisites (one-time)

```bash
# .NET 10 SDK required (verify)
dotnet --version                       # 10.0.x

# EF Core CLI tool (install once, then it's on PATH under ~/.dotnet/tools)
dotnet tool install --global dotnet-ef
# ...or upgrade it later:
dotnet tool update --global dotnet-ef
```

If `dotnet ef` isn't found after install, add the tools folder to PATH for the session:
- PowerShell: `$env:PATH += ";$env:USERPROFILE\.dotnet\tools"`
- bash: `export PATH="$PATH:$HOME/.dotnet/tools"`

## 1. Start infrastructure (PostgreSQL + MinIO)

```bash
# from the repository root
docker compose up -d
```

## 2. Configure connection string & secrets (first time)

`appsettings.json` already points `ConnectionStrings:Postgres` at the local Docker DB, so migrations/seed
work out of the box. To override values or set JWT secrets (needed to *run* the API), use user-secrets:

```bash
dotnet user-secrets init --project src/Marketplace.Api
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=smxd;Username=smxd;Password=smxd" --project src/Marketplace.Api
dotnet user-secrets set "Jwt:AccessSecret"  "change-me-to-a-32+char-random-secret-000" --project src/Marketplace.Api
dotnet user-secrets set "Jwt:RefreshSecret" "change-me-to-a-32+char-random-secret-111" --project src/Marketplace.Api
```

---

## 3. Generate a migration (after changing the EF model)

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Marketplace.Infrastructure \
  --startup-project src/Marketplace.Api
```

Related:

```bash
# list all migrations
dotnet ef migrations list --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api

# remove the LAST migration (only if not yet applied to a database)
dotnet ef migrations remove --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api
```

## 4. Update the database (apply pending migrations)

```bash
# Option A — EF CLI (applies migrations directly)
dotnet ef database update --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api

# Option B — app CLI verb (uses EfCoreMigrator → Database.Migrate())
dotnet run --project src/Marketplace.Api -- migrate
```

## 5. Seed reference + admin data

The `seed` verb **applies migrations first, then seeds** (categories/subcategories + boost packages
from `Seed/seed.sql`, plus an `admin@smxd.local` account).

```bash
dotnet run --project src/Marketplace.Api -- seed
```

## 6. Run the API

```bash
dotnet run --project src/Marketplace.Api
```

- Swagger UI: <http://localhost:5127/swagger>
- Health:     <http://localhost:5127/api/health>

---

## Handy extras

```bash
# Generate an idempotent SQL script (hand to a DBA / run in CI)
dotnet ef migrations script --idempotent \
  --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api \
  -o migrate.sql

# Drop the database (DANGER — deletes all data)
dotnet ef database drop --force --project src/Marketplace.Infrastructure --startup-project src/Marketplace.Api

# Build / test / format
dotnet build Marketplace.slnx
dotnet test  Marketplace.slnx
dotnet format Marketplace.slnx
```

### Notes

- **Design-time connection**: `dotnet ef migrations add` does **not** touch a database. `database update`
  does. The design-time factory reads the `MARKETPLACE_DB` env var and otherwise defaults to the local
  Docker connection string.
- **Typical first-run sequence**: `docker compose up -d` → `dotnet run --project src/Marketplace.Api -- seed`
  (migrates + seeds) → `dotnet run --project src/Marketplace.Api`.
- Migrations live in `src/Marketplace.Infrastructure/Migrations/`. Commit schema-changing migrations
  separately from the code that depends on them (expand → migrate → contract).

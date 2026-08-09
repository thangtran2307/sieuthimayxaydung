# Marketplace Frontend (Next.js + TypeScript)

SEO-first web client: **Next.js 15** (App Router) + **TypeScript**, **Tailwind CSS** + shadcn/ui
foundations, **next-intl** (VI default / EN), bilingual with locale in the URL (`/vi`, `/en`) and a
persisted cookie. Self-contained app — its own `package.json`/`tsconfig` (no monorepo).

> Run all commands below from the **`apps/frontend`** directory.

## Prerequisites (one-time)

```bash
node --version         # 20+ required
corepack enable        # activates the pnpm version pinned in package.json (packageManager)
pnpm --version         # should report 10.6.4 (pinned)
```

> The exact pnpm version is pinned via the `packageManager` field, so Corepack uses the **same
> pnpm** on every machine. This avoids lockfile churn and inconsistent build-script prompts.

## Setup

```bash
pnpm install                   # reproducible install from the committed pnpm-lock.yaml
cp .env.example .env.local     # then set NEXT_PUBLIC_API_URL if the backend runs elsewhere
```

Always commit **both** `package.json` and `pnpm-lock.yaml` together.

`.env.local`:

| Variable | Default | Purpose |
|----------|---------|---------|
| `NEXT_PUBLIC_API_URL` | `http://localhost:5127/api` | Base URL of the .NET backend API (includes `/api`) |

## Commands

```bash
pnpm dev          # start dev server → http://localhost:3000 (VI at /vi, EN at /en)
pnpm build        # production build
pnpm start        # run the production build
pnpm lint         # ESLint (next)
pnpm typecheck    # tsc --noEmit
pnpm format       # Prettier (writes)
pnpm test         # Vitest
```

## Generate API types from the backend (optional)

Types are generated from the backend's Swagger document with `openapi-typescript`. **Start the
backend first** (see [`apps/backend/README.md`](../backend/README.md)), then:

```bash
pnpm gen:api      # openapi-typescript http://localhost:5127/swagger/v1/swagger.json -> src/lib/api/schema.d.ts
```

Runtime response validation uses the Zod schemas in `src/contracts` (kept in sync with the API
contract); the generated `schema.d.ts` provides compile-time types.

## Structure

```text
src/
├── app/
│   ├── layout.tsx              # root passthrough
│   └── [locale]/               # locale-scoped routes (/vi, /en): layout, page, ...
├── components/                 # UI (shadcn/ui-based) + header/language switcher
├── features/                   # feature UI + data hooks (search, listing, ...)
├── lib/
│   ├── api/                    # typed fetch client (+ generated schema.d.ts)
│   └── utils.ts                # cn() helper
├── i18n/                       # next-intl routing, request config, navigation
├── contracts/                  # frontend Zod schemas mirroring the API wire contract
└── messages/                   # vi.json / en.json translation catalogs
```

## Troubleshooting

**"Lockfile is not up to date" / `ERR_PNPM_OUTDATED_LOCKFILE`** (e.g. on another machine or CI)
- Cause: `package.json` changed without regenerating the lock, or you're on a commit where the two
  are out of sync.
- Fix: `git pull` the latest commit first. If it persists, run `pnpm install` (updates the lock) and
  **commit `pnpm-lock.yaml`**. For CI, `pnpm install --frozen-lockfile` must pass — run it locally to
  verify before pushing.

**pnpm asks to approve build scripts (esbuild / sharp / unrs-resolver)**
- These native packages run install/build scripts. They are pre-approved via
  `pnpm.onlyBuiltDependencies` in `package.json`, so a fresh `pnpm install` builds them without
  prompting — **as long as you're on the latest commit** (older commits didn't have this).
- If you still get prompted (e.g. different pnpm version), either accept once with
  `pnpm approve-builds`, or run `pnpm rebuild esbuild sharp unrs-resolver`.
- Enabling Corepack (`corepack enable`) ensures the pinned pnpm version is used, which keeps this
  behavior consistent across machines.

## Notes

- **i18n**: `vi` is the default locale; the middleware persists the choice in a cookie and handles
  locale-prefixed routing. Add strings to `src/messages/{vi,en}.json`.
- **Auth**: the backend issues an httpOnly `access_token` cookie; the API client forwards cookies
  during SSR so authenticated server rendering works.
- `next lint` is deprecated in Next 16 — migrate to the ESLint CLI when upgrading.

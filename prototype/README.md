# MayXayDung — Marketplace Prototype

High-fidelity design prototype for a modern replacement of **sieuthimayxaydung.vn** — a
classifieds marketplace for construction machinery & equipment. Sellers post listings,
buyers search and contact sellers directly, listings can be **boosted** (paid promotion),
and admins moderate everything.

## How to view

These are self-contained static HTML pages (Tailwind + Lucide + Google Fonts via CDN, so
you need an internet connection). Just open `index.html` in a browser, **or** serve the folder:

```bash
# from the prototype/ folder
python -m http.server 5173
# then open http://localhost:5173
```

## Pages

| File | Screen | Notes |
|------|--------|-------|
| `index.html` | Homepage | Search-first hero, categories, boosted "Featured" row, recent listings, trust, seller CTA |
| `search.html` | Search results | Sidebar filters, sort, **Sponsored** listings pinned on top, pagination |
| `product.html` | Listing detail | Gallery, specs table, sticky seller contact card (call/message), safety tip, report |
| `post-listing.html` | Create listing | Photo upload, guided form, step indicator, boost upsell |
| `dashboard.html` | Seller dashboard | KPIs, listings table with status, per-listing **Boost**, boost packages |
| `admin.html` | Admin panel | Moderation queue, reported listings, approve / reject / delete, bulk actions |
| `login.html` | Auth | Login + register tabs, Google, show/hide password (accounts required to post) |

## Design system (locked here)

- **Style:** Industrial & trustworthy — steel grey + safety orange
- **Colors:** brand steel-blue `#1e3a5f`, accent safety-orange `#ea580c`, boost amber `#f59e0b`
- **Fonts:** Archivo (display / headings) + Inter (body)
- **Tokens & components:** `assets/styles.css`
- **i18n (EN / VI):** `assets/app.js` — toggle in the header; persists via `localStorage`.
  Translatable text uses `data-i18n="key"` / `data-i18n-ph="key"`.
- **Mock data:** `assets/listings-data.js`

## Monetization / boost model shown

- Listings with `boost: true` render with an amber border + "Promoted" badge and are pinned
  to the top of search results and the homepage Featured row.
- Boost packages (Basic / Featured / Max priority) are shown on the seller dashboard and as
  an upsell after posting.

## Maps to production stack

Built to translate 1:1 to **Next.js + Tailwind + shadcn/ui**:
- Each page → a Next.js route (`/`, `/search`, `/listing/[id]`, `/post`, `/dashboard`, `/admin`, `/login`)
- `styles.css` tokens → Tailwind theme / CSS variables
- i18n dictionary → `next-intl` / `next-i18next` message catalogs
- Listing cards, tables, forms → shadcn/ui components
- Auth (accounts required to post) → NextAuth / Auth.js with Google + email
```

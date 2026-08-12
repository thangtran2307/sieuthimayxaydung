# Phase 1 Data Model: Construction Machinery Marketplace

**Date**: 2026-08-09 (rev. 2 — Dapper/SQL) | **Feature**: 001-machinery-marketplace

Derived from the spec's Key Entities and Functional Requirements. This describes the **persistence
model** (PostgreSQL). The schema is created and evolved with **EF Core auto-generated migrations**
(write side + Repository/Unit of Work); the **read side uses Dapper** over the DbContext connection
(CQRS split). C# domain entities derive from a base `Entity` (Id + domain events) with private
setters; EF entity mappings live in `Infrastructure/Persistence/Configurations/`. They are not
identical to the API wire DTOs (which the frontend consumes via OpenAPI).

Conventions: all tables have `id` (`uuid` PK, `gen_random_uuid()`), `created_at`, `updated_at`
(`timestamp without time zone`, always UTC). Column names are `snake_case` (EFCore.NamingConventions);
tables are plural (e.g. `listings`, `boost_packages`). Money is whole VND stored as `bigint` (C#
`long`). Enums are stored as **text** (EF `HasConversion<string>()`; Dapper parses strings back to
enums). `specs` is a `ListingSpecs` value object stored as **jsonb** (OwnsOne + ToJson). `search_vector`
is a Postgres-generated `tsvector` (`HasGeneratedTsVectorColumn`). See the EF migration for exact SQL.

---

## Entity: User

A registered identity that can own listings (`SELLER`) or moderate (`ADMIN`). Buyers are not users.

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| email | citext | Unique; required for password accounts |
| passwordHash | text? | argon2; null when Google-only |
| googleId | text? | Unique when present |
| role | enum `UserRole` | `SELLER` \| `ADMIN`; default `SELLER` |
| displayName | text | Required |
| phone | text? | Seller contact number (revealed to buyers on their listings) |
| locationProvince | text? | VN province/city |
| description | text? | Seller profile blurb (bilingual free text) |
| coverImageUrl | text? | Seller profile cover |
| verified | boolean | Default false; trust badge only — NOT a gate to posting (FR-013a) |
| joinedAt | timestamptz | = createdAt |

**Relationships**: 1 User → many Listing; 1 User (admin) → many ModerationDecision; 1 User (admin)
→ many resolved Report.

**Derived/aggregate (not stored, computed or cached)**: listing count, total views, response time.

**Validation**: valid email format; either `passwordHash` or `googleId` present.

---

## Entity: Category

Two-level taxonomy (category → subcategory) with bilingual labels. Self-referential via `parentId`.

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| slug | text | Unique; URL-safe (e.g. `excavator`, `crawler`) |
| parentId | UUID? | Null = top-level category; set = subcategory |
| labelVi | text | Vietnamese label |
| labelEn | text | English label |
| icon | text? | Icon key (from prototype) |
| sortOrder | int | Display order |

**Relationships**: self-referential parent/children; 1 Category → many Listing (as category and as
subcategory).

**Seed**: from prototype `listings-data.js` (Excavators, Forklifts, Cranes, Concrete, Road,
Parts + their subcategories).

---

## Entity: Listing

The core aggregate — a machine/part offered for sale.

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| sellerId | UUID | FK → User; owner |
| categoryId | UUID | FK → Category (top-level) |
| subcategoryId | UUID? | FK → Category (child); optional |
| title | text | Required, 10–140 chars |
| slug | text | Unique; generated from title + short id (SEO URLs). See "Slug convention" below |
| condition | enum `Condition` | `NEW` \| `USED` |
| priceAmount | BigInt? | Whole VND; null when `priceContact` = true |
| priceContact | boolean | True = "Contact for price"; excluded from price-range filter |
| currency | text | Default `VND` |
| locationProvince | text | VN province/city; required |
| description | text | Required, ≥ 30 chars |
| specs | JSONB | Flexible key/value specifications (year, hours, brand, model, …) |
| status | enum `ListingStatus` | see lifecycle below |
| viewCount | int | Default 0 |
| searchVector | tsvector | Maintained by a trigger (title = A, description = B); GIN indexed |
| publishedAt | timestamptz? | Set on first approval |
| expiresAt | timestamptz? | Optional auto-expiry (see Assumptions) |

**Relationships**: many Listing → 1 User (seller); → 1 Category (+ optional subcategory); 1 Listing
→ many ListingPhoto; 1 Listing → 0..1 active Boost (many historically); 1 Listing → many Report;
1 Listing → many Inquiry; 1 Listing → many ModerationDecision.

**Validation (from FRs)**: required category, title, condition, location, description; exactly one
of `priceAmount`/`priceContact`; at least one ListingPhoto before submission (FR-014, FR-015).

**Indexes**: GIN on `searchVector`; GIN (pg_trgm) on `title`; btree on `status`, `categoryId`,
`subcategoryId`, `condition`, `locationProvince`, `priceAmount`, `publishedAt`, `sellerId`. These
back the P1 filter/sort/search paths (Principle VII).

### Listing status lifecycle

```
                approve                 mark-sold
   [PENDING] ───────────► [ACTIVE] ───────────────► [SOLD]
       │  \                  │  \
 reject│   \remove     expire│   \admin remove
       ▼    \                ▼    ▼
  [REJECTED] \          [EXPIRED] [REMOVED]
              ▼
          [REMOVED]
```

- **PENDING**: created/edited by seller; NOT public (FR-016). Awaits moderation.
- **ACTIVE**: approved & public/searchable; `publishedAt` set. Edits MAY return it to PENDING.
- **REJECTED**: moderation declined; not public; seller can see rejection (FR-004 US2).
- **SOLD**: seller marked sold; not returned in default search.
- **EXPIRED**: passed `expiresAt`; not public; seller may renew.
- **REMOVED**: admin takedown of a previously public listing (moderation/report outcome).

Only `ACTIVE` listings are eligible for boosted visibility (FR-027) and appear in search (FR-011).

### Slug convention

Slugs are **ASCII-folded from the (Vietnamese) title** — the market is VN-first, so keeping the folded
Vietnamese words is best for local SEO; titles are NOT translated to English for the slug. Rules:

- Lowercase → strip diacritics → replace non-alphanumeric runs with `-` → collapse/trim `-`.
- **Replace `đ`/`Đ` explicitly** (`đ→d`). `string.Normalize(FormD)` folds most Vietnamese marks (ơ, ư, ê,
  …) but NOT `đ` (it is a distinct letter, not base+mark), so it must be special-cased. Example:
  "Máy xúc đào Komatsu PC200-8" → `may-xuc-dao-komatsu-pc200-8-a1b2c3`.
- Append a short unique suffix (≈6 chars from the id) for uniqueness on title collisions.
- **Generated once at creation and kept stable** even if the title is later edited (avoid breaking inbound
  links/SEO; add a redirect if a change is ever required).
- No per-locale slugs: a listing has a single title; display language is carried by the `/vi` vs `/en`
  route prefix. (Category slugs are the exception — curated fixed keys like `excavator`.)
- Implemented by a `SlugGenerator` in User Story 2 (listing creation).

---

## Entity: ListingPhoto

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| listingId | UUID | FK → Listing (cascade delete) |
| url | text | Object-storage URL |
| sortOrder | int | Gallery order; 0 = primary/thumbnail |
| width | int? | Metadata |
| height | int? | Metadata |

**Rules**: ≥ 1 required per listing; content-type/size validated at upload (edge case handling).

---

## Entity: BoostPackage

Configuration of purchasable boost tiers (seeded; editable by admin later).

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| tier | enum `BoostTier` | `BASIC` \| `FEATURED` \| `MAX` (from prototype) |
| name | text | Display name |
| priorityLevel | int | Higher = ranked first among boosted (FR-006) |
| durationDays | int | Boost window length |
| priceAmount | BigInt | VND |
| active | boolean | Whether currently offered |

---

## Entity: Boost

A boost applied (or requested) for one listing.

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| listingId | UUID | FK → Listing |
| sellerId | UUID | FK → User (denormalized owner for authz/reporting) |
| packageId | UUID | FK → BoostPackage |
| priorityLevel | int | Snapshot from package at request time |
| status | enum `BoostStatus` | `REQUESTED` \| `ACTIVE` \| `EXPIRED` \| `CANCELLED` |
| paymentReference | text? | Offline payment note/reference (v1 manual) |
| requestedAt | timestamptz | |
| activatedAt | timestamptz? | Set when admin/rule activates |
| activatedByAdminId | UUID? | FK → User (admin); null if auto |
| startsAt | timestamptz? | Set at activation |
| expiresAt | timestamptz? | `startsAt + durationDays` |

### Boost status lifecycle

```
 [REQUESTED] ──activate(admin/auto)──► [ACTIVE] ──window elapsed──► [EXPIRED]
      │                                    │
      └────────── cancel ─────────────────┴────────► [CANCELLED]
```

**Rules (FR-024–FR-027)**: only the listing owner may request; activation requires the listing to
be `ACTIVE`; a boost never grants visibility to a non-public listing; a scheduled job transitions
`ACTIVE → EXPIRED` when `expiresAt` passes; at most one non-terminal (`REQUESTED`/`ACTIVE`) boost
per listing at a time.

---

## Entity: Report

A flag raised against a listing by a visitor (no account required).

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| listingId | UUID | FK → Listing |
| reason | enum `ReportReason` | `FRAUD` \| `INCORRECT_INFO` \| `SPAM` \| `PROHIBITED` \| `OTHER` |
| details | text? | Free-text |
| reporterContact | text? | Optional contact of reporter |
| status | enum `ReportStatus` | `OPEN` \| `RESOLVED_REMOVED` \| `RESOLVED_CLEARED` |
| resolvedByAdminId | UUID? | FK → User (admin) |
| resolvedAt | timestamptz? | |

**Rules (FR-009, FR-021)**: anyone may create; only admins resolve; resolving with removal sets the
listing to `REMOVED`.

---

## Entity: Inquiry (Contact Event)

A buyer's contact action against a listing — supports phone reveal and the message form (FR-008),
and measures listing performance (dashboard views/inquiries).

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| listingId | UUID | FK → Listing |
| sellerId | UUID | FK → User (denormalized) |
| type | enum `InquiryType` | `PHONE_REVEAL` \| `MESSAGE` |
| buyerName | text? | Required for `MESSAGE` |
| buyerPhone | text? | Optional |
| buyerEmail | text? | Optional; validated if present |
| message | text? | Required for `MESSAGE` |

**Rules**: no buyer account required; `PHONE_REVEAL` records that the number was revealed;
`MESSAGE` notifies the seller. Buyer contact details are never exposed to other buyers or in public
responses (Principle VI).

---

## Entity: ModerationDecision (audit log)

Immutable record of moderation actions for operational review & disputes (FR-020, FR-030).

| Field | Type | Notes / Rules |
|-------|------|---------------|
| id | UUID | PK |
| listingId | UUID | FK → Listing |
| adminId | UUID | FK → User (admin) |
| decision | enum `ModerationAction` | `APPROVED` \| `REJECTED` \| `REMOVED` |
| reason | text? | Optional note (shown to seller on rejection) |
| createdAt | timestamptz | Append-only |

---

## Enum summary

- `UserRole`: SELLER, ADMIN
- `Condition`: NEW, USED
- `ListingStatus`: PENDING, ACTIVE, REJECTED, SOLD, EXPIRED, REMOVED
- `BoostTier`: BASIC, FEATURED, MAX
- `BoostStatus`: REQUESTED, ACTIVE, EXPIRED, CANCELLED
- `ReportReason`: FRAUD, INCORRECT_INFO, SPAM, PROHIBITED, OTHER
- `ReportStatus`: OPEN, RESOLVED_REMOVED, RESOLVED_CLEARED
- `InquiryType`: PHONE_REVEAL, MESSAGE
- `ModerationAction`: APPROVED, REJECTED, REMOVED

## Relationship overview

```
User(SELLER) 1───* Listing *───1 Category (+0..1 subcategory self-ref)
Listing 1───* ListingPhoto
Listing 1───* Boost *───1 BoostPackage
Listing 1───* Report
Listing 1───* Inquiry
Listing 1───* ModerationDecision *───1 User(ADMIN)
```

## Search & ranking implementation notes

- `searchVector` = `setweight(to_tsvector('simple', title),'A') || setweight(to_tsvector('simple',
  description),'B')`, maintained by a trigger (created in a DbUp SQL migration); GIN indexed.
- Default listing query: `WHERE status = 'ACTIVE'` + optional filters (category, subcategory,
  condition, province, price range excluding `priceContact` rows when a price filter is set) +
  optional FTS match; `ORDER BY (has_active_boost) DESC, boost.priorityLevel DESC NULLS LAST,
  <user sort>`; paginated with clamped `page`/`pageSize` (Principle II, VII).

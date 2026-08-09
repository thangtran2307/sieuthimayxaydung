# API Contracts: Construction Machinery Marketplace

**Date**: 2026-08-09 (rev. 2 — .NET backend) | **Feature**: 001-machinery-marketplace

These contracts define the HTTP interface between the Next.js frontend and the **.NET (ASP.NET
Core)** backend. The backend emits an OpenAPI document at runtime (`Microsoft.AspNetCore.OpenApi`);
`openapi.yaml` here is the design-time contract of record and MUST match it. The frontend generates
TypeScript types from the OpenAPI document (`openapi-typescript`) and keeps Zod schemas in
`packages/contracts` for runtime validation — those schemas MUST match the shapes below.

- `openapi.yaml` — machine-readable REST contract for all endpoints (language-agnostic).
- This file — cross-cutting conventions (envelope, errors, auth, pagination) per the constitution.

The wire conventions below are language-agnostic; the .NET API implements the envelope via a result
filter + exception middleware, and validation via FluentValidation at the boundary.

## Base

- Base path: `/api`
- All request/response bodies are JSON (`application/json`), except image upload which uses a
  presigned URL flow.
- All endpoints follow REST resource-noun conventions (Principle I).

## Standard response envelope (Principle I)

**Success (single resource)** — 2xx:

```json
{ "data": { /* resource */ } }
```

**Success (collection)** — 2xx, always paginated:

```json
{
  "data": [ /* items */ ],
  "meta": { "page": 1, "pageSize": 20, "total": 137, "totalPages": 7 }
}
```

**Error** — 4xx/5xx, one consistent shape:

```json
{ "error": { "code": "LISTING_NOT_FOUND", "message": "Listing not found", "details": null } }
```

`details` carries field-level validation errors for 422:

```json
{ "error": { "code": "VALIDATION_FAILED", "message": "Invalid request",
  "details": [ { "path": "title", "message": "Must be at least 10 characters" } ] } }
```

## Status codes (Principle I & III)

| Code | Used when |
|------|-----------|
| 200 | Successful read / update |
| 201 | Resource created |
| 204 | Successful delete (no body) |
| 400 | Malformed request (unparseable) |
| 401 | Not authenticated |
| 403 | Authenticated but not authorized (e.g. not owner / not admin) |
| 404 | Unknown or non-public resource id |
| 409 | Conflict (e.g. active boost already exists) |
| 422 | Validation failed (schema/business rule) — `details` populated |
| 429 | Rate limited (report/inquiry/auth abuse protection) |
| 500 | Unexpected — surfaced via global filter, never swallowed |

## Error codes (stable, machine-readable)

`VALIDATION_FAILED`, `UNAUTHENTICATED`, `FORBIDDEN`, `NOT_FOUND`, `LISTING_NOT_FOUND`,
`LISTING_NOT_PUBLIC`, `NOT_LISTING_OWNER`, `EMAIL_IN_USE`, `INVALID_CREDENTIALS`,
`BOOST_CONFLICT`, `LISTING_NOT_ACTIVE_FOR_BOOST`, `UNSUPPORTED_MEDIA`, `RATE_LIMITED`,
`INTERNAL_ERROR`.

## Authentication & authorization

- Auth is cookie-based: the backend sets `httpOnly`, `Secure`, `SameSite=Lax` cookies for access
  and refresh tokens. Clients send cookies automatically; no `Authorization` header handling in the
  browser.
- **Public** endpoints require no auth (browse, search, listing detail, inquiry, report).
- **Seller** endpoints require an authenticated `SELLER`/`ADMIN` and enforce resource ownership
  server-side (Principle VI) — ownership is re-derived from the session, never trusted from input.
- **Admin** endpoints require role `ADMIN`; all others receive 403.

## Pagination (Principle II & VII)

- Query params `page` (≥ 1, default 1) and `pageSize` (1–50, default 20).
- Invalid values are **clamped** to bounds, not rejected (edge case). `page` beyond `totalPages`
  returns an empty `data` array with correct `meta`.

## Validation (Principle II)

- Every request body/query is validated against its Zod schema at the interface layer before
  reaching application/domain code. Failures return 422 with field `details`.

## Sensitive data (Principle VI)

- Responses never include `passwordHash`, other users' inquiry contact details, or internal payment
  identifiers. Seller phone is exposed only through the listing-scoped phone-reveal action.

## Endpoint groups (see openapi.yaml for full shapes)

**Public**
- `GET /api/categories`
- `GET /api/listings` — search/filter/sort/paginate
- `GET /api/listings/{slug}`
- `POST /api/listings/{id}/inquiries` — phone reveal or message
- `POST /api/listings/{id}/reports`

**Auth**
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `POST /api/auth/refresh`
- `GET  /api/auth/me`
- `GET  /api/auth/google` / `GET /api/auth/google/callback` (optional)

**Seller (auth + ownership)**
- `POST   /api/listings`
- `PATCH  /api/listings/{id}`
- `DELETE /api/listings/{id}`
- `POST   /api/listings/{id}/mark-sold`
- `GET    /api/me/listings`
- `GET    /api/me/dashboard`
- `GET    /api/boost-packages`
- `POST   /api/listings/{id}/boost-requests`
- `POST   /api/uploads/presign` — presigned image upload URL

**Admin (role ADMIN)**
- `GET  /api/admin/moderation/listings?status=pending`
- `POST /api/admin/moderation/listings/{id}/approve`
- `POST /api/admin/moderation/listings/{id}/reject`
- `POST /api/admin/moderation/listings/{id}/remove`
- `POST /api/admin/moderation/bulk`
- `GET  /api/admin/reports`
- `POST /api/admin/reports/{id}/resolve`
- `GET  /api/admin/boost-requests`
- `POST /api/admin/boost-requests/{id}/activate`

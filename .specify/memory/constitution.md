<!--
Sync Impact Report
==================
Version change: (template) → 1.0.0
Rationale: Initial ratification. The file previously contained only unfilled
template placeholders; this is the first concrete adoption of the constitution.

Principles defined (7):
  - I. API Consistency & Contracts
  - II. Zero-Trust Data Validation
  - III. Fail Loud, Never Silent
  - IV. Explicit Dependencies & Deletable Modules
  - V. Clean, Typed, Layered Code
  - VI. Security & Privacy by Default
  - VII. Performance & Scalability

Added sections:
  - Security & Operational Guardrails
  - Development Workflow & Quality Gates
  - Governance

Removed sections: none (template placeholders replaced)

Follow-up TODOs: none — RATIFICATION_DATE set to first adoption date (today),
as no earlier adoption date exists.
-->

# Sieuthimayxaydung Constitution

A construction-machinery marketplace (listings, search, user accounts, admin).
This constitution governs a single solo full-stack developer. It optimizes for a
small, fast, maintainable codebase that one person can hold in their head and
safely change alone. Rules use MUST / SHOULD deliberately: MUST is
non-negotiable; SHOULD is the default that requires a written reason to break.

## Core Principles

### I. API Consistency & Contracts

- REST endpoints MUST follow RESTful conventions: resource-based nouns, plural
  collections (`/listings`, `/listings/{id}`), and correct HTTP verbs
  (GET read, POST create, PUT/PATCH update, DELETE remove).
- Every endpoint MUST return a standardized response envelope: a success shape
  for 2xx and a consistent error shape (`{ error: { code, message, details? } }`)
  for 4xx/5xx. The shape MUST NOT vary per endpoint.
- HTTP status codes MUST be meaningful: 200/201/204 on success, 400 for bad
  input, 401 unauthenticated, 403 unauthorized, 404 not found, 409 conflict,
  422 validation failure, 500 for unexpected errors.
- Breaking changes to a request/response contract MUST be versioned, not
  silently altered. Rationale: the frontend and any future integrations depend on
  stable contracts; drift causes silent client breakage.

### II. Zero-Trust Data Validation

- All inputs MUST be validated at the system boundary (route/controller layer),
  not mixed into core business logic. Business logic MAY assume already-valid data.
- Query params and request bodies MUST be validated for type, range, and required
  fields before use.
- Pagination MUST reject invalid values: `page`/`limit` MUST be clamped to sane
  bounds (e.g. `limit` capped at a fixed maximum, negative/zero rejected).
- Never trust client-supplied identifiers, roles, or ownership claims; re-derive
  them from the authenticated session. Rationale: the boundary is the only place
  where untrusted data becomes trusted data, so validation belongs there and
  nowhere else.

### III. Fail Loud, Never Silent

- No silent failures. Errors MUST either bubble to a global error handler that
  returns a structured error response, or crash loudly with a clear stack trace.
- Empty catch blocks, swallowed exceptions, and "return null on error" patterns
  are FORBIDDEN unless the absence is an explicitly modeled, documented outcome.
- Missing resources and invalid IDs MUST be handled gracefully with the correct
  status code (404/422), never a 500 or a blank success. Rationale: silent
  failures are the most expensive bugs for a solo dev to diagnose; loud failures
  are cheap to find.

### IV. Explicit Dependencies & Deletable Modules

- Optimize for deletion over extension. Modules MUST stay small enough that they
  can be rewritten from scratch in a day.
- Dependencies MUST be explicit (passed in / imported deliberately). Hidden
  coupling, global mutable state singletons, and side effects at import time are
  FORBIDDEN.
- Avoid premature abstraction. Inline duplicated logic until the SAME pattern
  appears a third time, then extract. Rationale: for a solo codebase, wrong
  abstractions cost more than duplication; small deletable modules keep the whole
  system understandable.

### V. Clean, Typed, Layered Code

- Code MUST be typed and self-documenting: descriptive names over shorthand,
  abbreviations, or clever symbols.
- Layers MUST be separated: controller (HTTP/validation) → service (business
  logic) → repository (data access). Business logic MUST be pure and independent
  of the UI framework; state MUST be separated from presentation.
- Comments explain WHY, not WHAT; the code itself explains what. Rationale:
  layering keeps business rules testable and portable, and self-documenting code
  is the only documentation a solo dev reliably keeps current.

### VI. Security & Privacy by Default

- Endpoints that touch user data or mutate state MUST be authenticated and
  authorized. Authorization MUST verify resource ownership, not just a valid login.
- Sensitive user data (password hashes, tokens, emails, phone numbers when not
  the owner) MUST NEVER be included in API responses or logs.
- Secrets (API keys, tokens, DB passwords) MUST be read from environment
  variables and MUST NEVER be committed. Rationale: a public marketplace is an
  attack surface; the cheapest breach prevention is refusing to leak or hardcode
  secrets in the first place.

### VII. Performance & Scalability

- List/collection endpoints MUST be paginated; unbounded result sets are
  FORBIDDEN.
- Database queries MUST avoid N+1 patterns and MUST select only needed columns;
  responses MUST avoid unnecessary payloads (no dumping full entities when a
  summary suffices).
- Indexes SHOULD back every column used for filtering, sorting, or lookup on
  hot paths. Rationale: catalog and search endpoints are the core of the product;
  they must stay fast as listings grow.

## Security & Operational Guardrails

- Minimize external dependencies. Adding a third-party library REQUIRES a brief
  written trade-off justification (why it beats standard-library or in-house code).
- Logging MUST use structured JSON with semantic trace tags (e.g. request id,
  user id) so user context can be followed across asynchronous events.
- Database migrations MUST preserve backwards compatibility: schema changes
  deploy separately from the code that depends on them (expand → migrate →
  contract), so a rollback never breaks a running app.
- No secrets in source control, logs, or error responses (see Principle VI).

## Development Workflow & Quality Gates

- Tests MUST exercise real or fixture data over complex mocks that drift from
  actual component behavior; prefer integration tests at the service and API
  boundary for anything that touches persistence or contracts.
- Every endpoint MUST be documented with a request example, the response shape,
  and its notable edge cases (empty results, invalid id, unauthorized).
- Before merging to `master`, changes SHOULD pass: type checks, the test suite,
  and a self-review against this constitution.
- Commits SHOULD be small and focused; migrations and their dependent code
  SHOULD land in separate commits (see Operational Guardrails).

## Governance

- This constitution supersedes ad-hoc practice. When code and constitution
  conflict, the constitution wins or the constitution is amended — not ignored.
- Amendments: because this is a solo project, the developer MAY amend directly,
  but every amendment MUST update the version, the Last Amended date, and the
  Sync Impact Report at the top of this file.
- Versioning (semantic): MAJOR for backward-incompatible principle removals or
  redefinitions; MINOR for a new principle or materially expanded guidance;
  PATCH for clarifications and wording fixes.
- Compliance: each feature's plan and review SHOULD confirm adherence to these
  principles; deviations MUST be justified in writing (in the PR/commit or the
  feature's plan) or removed.

**Version**: 1.0.0 | **Ratified**: 2026-08-09 | **Last Amended**: 2026-08-09

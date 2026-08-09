# Feature Specification: Construction Machinery Marketplace (Foundation / MVP)

**Feature Branch**: `001-machinery-marketplace`

**Created**: 2026-08-09

**Status**: Draft

**Input**: User description: "I need to setup a code base first for this project. I haven't setup the project yet, but this project goal is to build a website for construction machine trader. An enhanced version of https://sieuthimayxaydung.vn/trang-chu. Ideas will get from https://www.machinerytrader.com/"

## Overview

A bilingual (Vietnamese / English) online marketplace where sellers list used and new
construction machinery, parts, and equipment, and buyers discover listings, compare them,
and contact sellers directly to negotiate a purchase. The platform earns revenue by selling
"boost" promotions that give a listing higher visibility. Administrators moderate listings and
sellers to keep the marketplace trustworthy.

This first feature establishes the foundational, end-to-end product: buyers can find and
contact; sellers can register and publish (subject to moderation); the business can monetize
via boosts; and admins can moderate. It is scoped as an MVP that can be extended later.

## Clarifications

### Session 2026-08-09

- Q: What transaction model should the marketplace use for v1? → A: Classifieds / lead-generation
  — buyers contact sellers directly; no on-platform cart, checkout, escrow, or shipping for the
  machine.
- Q: How should buyers contact sellers about a listing? → A: Reveal the seller's phone number
  plus a simple message/inquiry form that notifies the seller; no buyer accounts and no
  persistent chat inbox in v1.
- Q: How should boost/promotion payments be handled in the MVP? → A: Manual confirmation first —
  the seller requests a boost, pays offline (e.g. bank transfer), and an administrator (or an
  automated rule) activates it; no online payment gateway in v1.
- Q: What should a new seller be able to do right after registering? → A: Post immediately;
  every listing is gated by per-listing moderation before going public. No separate seller
  pre-approval or verification is required to post.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover and contact a seller about a machine (Priority: P1)

A prospective buyer arrives at the site, searches or browses by category for a specific type
of machine (e.g. a used crawler excavator), refines results with filters (category, condition,
location, price), opens a listing to review its details and photos, and contacts the seller to
ask about the machine — all without needing an account.

**Why this priority**: Buyer discovery and seller contact are the reason the marketplace
exists. Without this, listings have no audience and sellers get no value. This slice alone is a
viable product: a searchable catalog of machinery that connects buyers to sellers.

**Independent Test**: Load the site with a set of published listings, perform a category and
keyword search, apply at least one filter, open a listing detail page, and initiate contact with
the seller. Value is delivered when a buyer can go from landing page to seller contact for a
relevant machine.

**Acceptance Scenarios**:

1. **Given** published listings exist, **When** a visitor opens the homepage, **Then** they see
   a prominent search entry point, browsable categories, a row of promoted (boosted) listings,
   and a section of recent listings.
2. **Given** a visitor enters a keyword and/or selects a category, **When** they run the search,
   **Then** matching listings are shown with title, price, condition, location, and thumbnail,
   and boosted listings that match are pinned above non-boosted ones.
3. **Given** a results list, **When** the visitor applies filters (category/subcategory,
   condition new/used, location, price range) and/or a sort order, **Then** the results update to
   reflect the selected criteria and the applied filters are visible.
4. **Given** a results list spanning more items than one page, **When** the visitor navigates
   pages, **Then** additional results load without losing the active filters and sort.
5. **Given** a listing of interest, **When** the visitor opens it, **Then** they see the full
   photo gallery, specifications, description, location, seller identity, and one or more ways to
   contact the seller (e.g. call, message).
6. **Given** a listing detail page, **When** the visitor chooses to contact the seller, **Then**
   the contact action succeeds and the seller can receive the buyer's inquiry.
7. **Given** a listing that a visitor believes is fraudulent or incorrect, **When** they choose
   to report it, **Then** the report is recorded for admin review.

---

### User Story 2 - Register as a seller and publish a listing (Priority: P2)

A seller creates an account (or signs in), completes a guided listing form with photos,
category, condition, price, location, and specifications, and submits it. The listing enters a
moderation queue and becomes publicly visible once approved.

**Why this priority**: Supply (listings) is required for the buyer experience to have anything to
show. It depends on P1 existing as the destination for published listings, but is the second
pillar of the two-sided marketplace.

**Independent Test**: Register a new seller account, create a listing with the required fields
and at least one photo, submit it, and confirm it appears in the moderation queue in a pending
state and (after approval) becomes visible in search.

**Acceptance Scenarios**:

1. **Given** a visitor without an account, **When** they attempt to post a listing, **Then** they
   are required to register or sign in first.
2. **Given** a signed-in seller, **When** they complete the listing form with all required fields
   and at least one photo and submit, **Then** the listing is saved and marked pending review.
3. **Given** a listing form with a missing or invalid required field, **When** the seller submits,
   **Then** submission is blocked and the specific problems are clearly indicated.
4. **Given** a submitted listing, **When** an administrator approves it, **Then** it becomes
   publicly visible and searchable; **When** it is rejected, **Then** it does not become public and
   the seller can see it was rejected.
5. **Given** a signed-in seller, **When** they open their dashboard, **Then** they can see their
   listings with each listing's status (pending, active, rejected, sold/expired) and basic
   performance indicators (e.g. views), and can edit or remove their own listings.

---

### User Story 3 - Moderate listings and sellers (Priority: P3)

An administrator reviews newly submitted and reported listings, approves or rejects them,
removes policy-violating content, and can act on reported items in bulk to keep the marketplace
trustworthy.

**Why this priority**: Moderation protects buyers from fraud and low-quality listings, which is
essential to trust in a high-value machinery marketplace. It is required for P2 listings to go
live, but is an operator-facing capability rather than the core public value.

**Independent Test**: With pending and reported listings present, sign in as an administrator,
review the moderation queue, approve one listing, reject another, and resolve a reported listing.
Confirm the public site reflects each decision.

**Acceptance Scenarios**:

1. **Given** listings awaiting review, **When** an administrator opens the moderation queue,
   **Then** they see pending listings with the information needed to make a decision.
2. **Given** a pending listing, **When** the administrator approves or rejects it, **Then** the
   listing's public visibility and status update accordingly and the decision is recorded.
3. **Given** reported listings, **When** the administrator reviews a report, **Then** they can
   remove or clear the listing and the report is resolved.
4. **Given** multiple items in the queue, **When** the administrator selects several and applies a
   bulk action, **Then** the action is applied to all selected items.
5. **Given** only authorized administrators may moderate, **When** a non-admin attempts to access
   the moderation area, **Then** access is denied.

---

### User Story 4 - Promote a listing with a paid boost (Priority: P4)

A seller chooses a boost package for one of their listings to increase its visibility. Boosted
listings are visually highlighted and pinned above non-boosted listings in search results and in
the homepage featured area for the duration of the boost.

**Why this priority**: Boosts are the platform's revenue model. They depend on listings (P2) and
search ranking (P1) already existing. Valuable to the business but not required for the core
two-sided exchange to function.

**Independent Test**: As a seller, select a boost package for an active listing, complete the
purchase step, and confirm the listing displays a promoted treatment and is pinned above
non-boosted matches in search and on the homepage featured row for the boost period.

**Acceptance Scenarios**:

1. **Given** a seller with an active listing, **When** they request a boost package and it is
   confirmed (in v1, activated by an administrator or automated rule after offline payment),
   **Then** the listing becomes boosted for the package's duration.
2. **Given** a boosted listing, **When** it appears in search results or the homepage featured
   area, **Then** it is visually marked as promoted and ranked above non-boosted matching listings.
3. **Given** a boost period, **When** the boost duration elapses, **Then** the listing reverts to
   standard (non-promoted) ranking and treatment.
4. **Given** multiple boosted listings match a query, **When** results are ordered, **Then**
   boosted listings are ordered ahead of non-boosted ones using a defined, consistent rule.

---

### Edge Cases

- A search or filter combination returns no results → the visitor sees a clear empty state with
  guidance to broaden the search, not an error.
- A listing is deleted, sold, expired, or unpublished while a visitor holds its link → the visitor
  sees a clear "no longer available" message rather than a broken page.
- A seller uploads an unsupported file type or an oversized image → the upload is rejected with a
  clear message and the rest of the form is preserved.
- A price is entered as "Contact for price" rather than a number → the listing displays a
  contact-for-price indicator and is excluded from or handled sanely by price-range filtering.
- Invalid pagination input (page beyond the last page, negative page, oversized page size) → the
  system clamps to valid bounds instead of failing.
- A seller attempts to edit, boost, or delete a listing that is not theirs → the action is denied.
- A boost is purchased for a listing that is then rejected/removed by moderation → the boost does
  not grant visibility to a non-public listing, and the situation is handled without silently
  charging for no value.
- Duplicate or spam listings are submitted rapidly by one seller → moderation and/or rate
  protections prevent abuse.
- A visitor switches language (VI ↔ EN) mid-session → interface text updates and the choice
  persists on subsequent visits.

## Requirements *(mandatory)*

### Functional Requirements

**Discovery & Search (P1)**

- **FR-001**: System MUST present a homepage with a primary search entry point, browsable
  categories, a promoted/featured listings section, and a recent-listings section.
- **FR-002**: System MUST let visitors search listings by free-text keyword.
- **FR-003**: System MUST let visitors browse and filter listings by category and subcategory,
  condition (new/used), location, and price range.
- **FR-004**: System MUST let visitors sort results (e.g. by recency, price ascending/descending,
  and relevance).
- **FR-005**: System MUST paginate result lists and MUST clamp invalid pagination parameters to
  valid bounds.
- **FR-006**: System MUST rank matching boosted listings above non-boosted listings in search
  results and in the homepage featured area, using a consistent rule.
- **FR-007**: System MUST provide a listing detail view showing photo gallery, title, price (or a
  contact-for-price indicator), condition, location, specifications, description, and seller
  identity.
- **FR-008**: System MUST allow visitors to contact a seller from a listing by revealing the
  seller's phone number and by submitting a short message/inquiry form that notifies the seller,
  without requiring the visitor to have an account. Persistent buyer accounts and an on-platform
  chat inbox are out of scope for v1.
- **FR-009**: System MUST allow visitors to report a listing for review.
- **FR-010**: System MUST support a Vietnamese/English language toggle whose selection persists
  across visits.
- **FR-011**: System MUST display a clear empty state when a search or filter returns no results,
  and a clear "no longer available" state for listings that are not (or no longer) public.

**Accounts & Selling (P2)**

- **FR-012**: System MUST allow visitors to register and sign in to a seller account (email/password
  at minimum; social sign-in MAY be offered).
- **FR-013**: System MUST require an authenticated account before a user can create, edit, boost, or
  delete a listing.
- **FR-013a**: System MUST allow any newly registered seller to submit listings immediately, with
  no separate seller pre-approval or identity/business verification required to post; trust is
  enforced by per-listing moderation (FR-016) rather than by gating the account.
- **FR-014**: System MUST provide a guided listing-creation form capturing category/subcategory,
  title, condition, price (or contact-for-price), location, specifications, description, and at
  least one photo.
- **FR-015**: System MUST validate listing submissions and block submission with clear, specific
  messages when required fields are missing or invalid, without discarding entered data.
- **FR-016**: System MUST place newly submitted listings into a pending-review state and MUST NOT
  make them public until approved.
- **FR-017**: Sellers MUST be able to view, edit, mark as sold, and delete their own listings, and
  MUST be prevented from modifying listings they do not own.
- **FR-018**: System MUST provide sellers a dashboard showing their listings with status and basic
  performance indicators (at least view counts).

**Moderation & Trust (P3)**

- **FR-019**: System MUST provide administrators a moderation queue of pending listings with the
  information needed to approve or reject each.
- **FR-020**: System MUST let administrators approve, reject, or remove listings, and record each
  moderation decision.
- **FR-021**: System MUST let administrators review and resolve reported listings.
- **FR-022**: System MUST support bulk moderation actions across multiple selected items.
- **FR-023**: System MUST restrict moderation and administrative functions to authorized
  administrator accounts and deny access to all others.

**Monetization / Boost (P4)**

- **FR-024**: System MUST offer selectable boost packages (differing by duration and/or priority
  level) that a seller can apply to their own active listing.
- **FR-025**: System MUST require a boost to be confirmed before it takes effect. In v1 this is a
  manual confirmation flow: the seller requests a boost package, pays offline (e.g. bank
  transfer), and an administrator (or an automated rule) activates the boost. No online payment
  gateway is integrated in v1; the flow MUST be designed so an automated payment step can replace
  the manual confirmation later without changing the boost model.
- **FR-026**: System MUST apply a boost only for its purchased duration and MUST revert the listing
  to standard treatment when the boost expires.
- **FR-027**: System MUST NOT grant boosted visibility to a listing that is not currently public
  (e.g. pending, rejected, or removed).

**Cross-cutting**

- **FR-028**: System MUST never expose sensitive personal data (e.g. password credentials, full
  private contact details of other users, payment identifiers) in listing or search responses.
- **FR-029**: System MUST handle invalid identifiers and missing resources gracefully with clear,
  correct not-found responses rather than errors or blank pages.
- **FR-030**: System MUST record enough activity (moderation decisions, reports, boost purchases)
  to support operational review and dispute resolution.

### Key Entities *(include if feature involves data)*

- **Listing**: A machine/part/equipment offered for sale. Attributes: title, category &
  subcategory, condition (new/used), price or contact-for-price, location, specifications,
  description, photos, status (pending, active, rejected, sold, expired), boost state and expiry,
  owning seller, timestamps, view count. Relationships: belongs to one Seller; may have many
  Reports; may have one active Boost.
- **Category (and Subcategory)**: A taxonomy of machinery types (e.g. Excavators → Crawler /
  Wheeled / Mini; Forklifts; Cranes; Concrete Equipment; Road Equipment; Parts & Components).
  Used for browsing and filtering; carries display labels in both languages and listing counts.
- **User / Seller Account**: A registered identity that can own and manage listings. Attributes:
  display name, contact details, verification status, join date, location, aggregate stats
  (listing count, views, response time). Relationships: owns many Listings.
- **Administrator**: An authorized account with moderation privileges over all listings, reports,
  and sellers.
- **Report**: A flag raised against a Listing by a visitor or user, with a reason and a resolution
  state. Relationships: references one Listing; reviewed by an Administrator.
- **Boost / Promotion**: A paid promotion applied to a Listing. Attributes: package tier,
  duration, priority level, start/expiry, purchase/payment reference, status. Relationships:
  applies to one Listing purchased by its Seller.
- **Inquiry / Contact Event**: A buyer's contact action against a Listing (call reveal or message),
  used to connect buyer and seller and to measure listing performance. Relationships: references
  one Listing and its Seller.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A first-time visitor can go from the homepage to viewing a relevant listing's detail
  page in 3 or fewer interactions.
- **SC-002**: 90% of buyers who intend to contact a seller can locate and use a contact option on a
  listing without assistance.
- **SC-003**: A search with an applied filter returns results in under 2 seconds for a typical
  query at expected catalog size.
- **SC-004**: A seller can create and submit a complete listing in under 5 minutes.
- **SC-005**: 100% of newly submitted listings pass through moderation before becoming public (no
  listing becomes public without an approval decision).
- **SC-006**: Boosted listings that match a query appear ahead of all non-boosted matching listings
  100% of the time while the boost is active, and never appear boosted after expiry.
- **SC-007**: No search, listing, or profile response exposes another user's sensitive credentials
  or private payment identifiers (verified by review/testing of responses).
- **SC-008**: Invalid inputs (bad pagination, unknown listing id, unsupported upload) produce a
  clear, correct user-facing outcome in 100% of tested cases, with no unhandled failure.
- **SC-009**: The interface is fully usable in both Vietnamese and English, and the chosen language
  persists across sessions.
- **SC-010**: The site is usable on mobile and desktop screen sizes for all P1 buyer flows.

## Assumptions

- **Marketplace model**: This is a classifieds / lead-generation marketplace (confirmed in
  Clarifications 2026-08-09). Buyers contact sellers directly to negotiate and complete the
  transaction off-platform; the platform does not process the machine sale, hold funds in escrow,
  or run a shopping cart/checkout for machines.
- **Revenue model**: Revenue in this MVP comes from paid listing boosts. Additional monetization
  (subscriptions, dealer storefront tiers, banner ads) is out of scope for v1.
- **Boost payment**: Boosts are gated by a manual confirmation flow in v1 (confirmed in
  Clarifications 2026-08-09): the seller requests a package, pays offline, and an administrator or
  automated rule activates it. No online payment gateway is integrated in v1, but the boost model
  must allow an automated payment step to be substituted later.
- **Buyer contact**: Buyers reach sellers via a revealed phone number plus a short inquiry form
  that notifies the seller (confirmed in Clarifications 2026-08-09); there are no buyer accounts
  and no persistent chat inbox in v1.
- **Seller onboarding**: Any registered user may post listings immediately; trust is enforced by
  per-listing moderation rather than seller pre-approval or verification (confirmed in
  Clarifications 2026-08-09).
- **Languages**: Vietnamese and English are the supported languages for v1, with Vietnamese as the
  primary market.
- **Accounts**: Buyers do not need an account to search, view, or contact sellers. Accounts are
  required only to post/manage listings and to moderate. Email/password is the baseline sign-in;
  social sign-in (e.g. Google) is optional.
- **Currency & region**: Primary currency is Vietnamese Đồng (₫) and the primary market is Vietnam;
  locations are Vietnamese provinces/cities.
- **Content types**: Listings cover whole machines, vehicles, and parts/components. Rentals,
  auctions, and financing are out of scope for v1.
- **"Setup a code base"**: The concrete technology stack, repository structure, and project
  scaffolding are implementation concerns handled in `/speckit-plan`; this specification defines the
  product capabilities the codebase must deliver.
- **Scale**: v1 targets an early-stage catalog (thousands of listings) with room to grow; hard scale
  targets will be set during planning.

## Out of Scope (v1)

- On-platform purchase/checkout, payment escrow, or shipping/logistics for machines.
- Auctions, rentals, and financing/leasing workflows.
- Buyer accounts with saved searches, watchlists, and messaging inbox history (may be a later
  feature; v1 contact is direct).
- Native mobile applications (responsive web only).
- Advanced monetization beyond listing boosts (dealer subscriptions, display advertising).

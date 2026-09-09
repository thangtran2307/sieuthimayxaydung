import { z } from 'zod';
import { categorySchema } from './category';
import { Condition, ListingStatus } from './common/enums';

/** A photo on a listing (gallery order via `sortOrder`; 0 = primary). */
export const listingPhotoSchema = z.object({
  url: z.string(),
  sortOrder: z.number().int(),
  width: z.number().int().nullable(),
  height: z.number().int().nullable(),
});
export type ListingPhoto = z.infer<typeof listingPhotoSchema>;

/** Seller info safe for public exposure (never credentials). */
export const publicSellerSchema = z.object({
  id: z.string(),
  displayName: z.string(),
  locationProvince: z.string().nullable(),
  verified: z.boolean(),
  joinedAt: z.string(),
});
export type PublicSeller = z.infer<typeof publicSellerSchema>;

/** Card shape for search/listing rows — mirrors the API `ListingSummary`. */
export const listingSummarySchema = z.object({
  id: z.string(),
  slug: z.string(),
  title: z.string(),
  priceAmount: z.number().nullable(),
  priceContact: z.boolean(),
  currency: z.string(),
  condition: Condition,
  locationProvince: z.string(),
  thumbnailUrl: z.string().nullable(),
  boosted: z.boolean(),
  categorySlug: z.string(),
  createdAt: z.string(),
});
export type ListingSummary = z.infer<typeof listingSummarySchema>;

/** Full public listing detail — mirrors the API `Listing`. */
/** Technical specifications supplied on the listing form (all optional). */
export const listingSpecsInputSchema = z.object({
  year: z.number().int().optional(),
  brand: z.string().optional(),
  model: z.string().optional(),
  hours: z.number().int().optional(),
  origin: z.string().optional(),
  capacity: z.string().optional(),
});
export type ListingSpecsInput = z.infer<typeof listingSpecsInputSchema>;

/** A photo attached to a new listing (already-hosted URL; upload flow deferred). */
export const listingPhotoInputSchema = z.object({
  url: z.string().url(),
  sortOrder: z.number().int(),
  width: z.number().int().nullable().optional(),
  height: z.number().int().nullable().optional(),
});
export type ListingPhotoInput = z.infer<typeof listingPhotoInputSchema>;

/** Create-listing payload (mirrors the backend CreateListingRequest). At least one photo required. */
export const createListingSchema = z
  .object({
    categoryId: z.string().uuid(),
    subcategoryId: z.string().uuid().nullable().optional(),
    title: z.string().min(1).max(200),
    condition: Condition,
    priceAmount: z.number().int().positive().nullable().optional(),
    priceContact: z.boolean(),
    locationProvince: z.string().min(1).max(120),
    description: z.string().min(1).max(5000),
    specs: listingSpecsInputSchema.optional(),
    photos: z.array(listingPhotoInputSchema).min(1),
  })
  .refine((value) => value.priceContact || (value.priceAmount != null && value.priceAmount > 0), {
    message: 'A price is required unless "contact for price" is selected.',
    path: ['priceAmount'],
  });
export type CreateListingInput = z.infer<typeof createListingSchema>;

/** Result of creating a listing. */
export const createdListingSchema = z.object({
  id: z.string(),
  slug: z.string(),
  status: ListingStatus,
});
export type CreatedListing = z.infer<typeof createdListingSchema>;

/** Current editable state of a seller's own listing, used to pre-fill the edit form. */
export const sellerListingDetailSchema = z.object({
  id: z.string(),
  categoryId: z.string(),
  subcategoryId: z.string().nullable(),
  title: z.string(),
  condition: Condition,
  priceAmount: z.number().nullable(),
  priceContact: z.boolean(),
  locationProvince: z.string(),
  description: z.string(),
  specs: z.object({
    year: z.number().nullable(),
    brand: z.string().nullable(),
    model: z.string().nullable(),
    hours: z.number().nullable(),
    origin: z.string().nullable(),
    capacity: z.string().nullable(),
  }),
  status: ListingStatus,
});
export type SellerListingDetail = z.infer<typeof sellerListingDetailSchema>;

/** Update-listing payload (mirrors the backend UpdateListingRequest — photos are not editable). */
export const updateListingSchema = z
  .object({
    categoryId: z.string().uuid(),
    subcategoryId: z.string().uuid().nullable().optional(),
    title: z.string().min(1).max(200),
    condition: Condition,
    priceAmount: z.number().int().positive().nullable().optional(),
    priceContact: z.boolean(),
    locationProvince: z.string().min(1).max(120),
    description: z.string().min(1).max(5000),
    specs: listingSpecsInputSchema.optional(),
  })
  .refine((value) => value.priceContact || (value.priceAmount != null && value.priceAmount > 0), {
    message: 'A price is required unless "contact for price" is selected.',
    path: ['priceAmount'],
  });
export type UpdateListingInput = z.infer<typeof updateListingSchema>;

/** A row in the seller's own dashboard — includes non-public statuses (mirrors SellerListingDto). */
export const sellerListingSchema = z.object({
  id: z.string(),
  slug: z.string(),
  title: z.string(),
  status: ListingStatus,
  priceAmount: z.number().nullable(),
  priceContact: z.boolean(),
  currency: z.string(),
  viewCount: z.number().int(),
  thumbnailUrl: z.string().nullable(),
  createdAt: z.string(),
  publishedAt: z.string().nullable(),
});
export type SellerListing = z.infer<typeof sellerListingSchema>;

export const listingDetailSchema = z.object({
  id: z.string(),
  slug: z.string(),
  title: z.string(),
  condition: Condition,
  priceAmount: z.number().nullable(),
  priceContact: z.boolean(),
  currency: z.string(),
  locationProvince: z.string(),
  description: z.string(),
  specs: z.record(z.string(), z.unknown()),
  photos: z.array(listingPhotoSchema),
  status: ListingStatus,
  boosted: z.boolean(),
  category: categorySchema,
  subcategory: categorySchema.nullable(),
  seller: publicSellerSchema,
  viewCount: z.number().int(),
  createdAt: z.string(),
  publishedAt: z.string().nullable(),
});
export type ListingDetail = z.infer<typeof listingDetailSchema>;

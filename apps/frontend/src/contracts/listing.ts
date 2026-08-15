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

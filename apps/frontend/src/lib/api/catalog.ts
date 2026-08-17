import { unstable_cache } from 'next/cache';
import { z } from 'zod';
import {
  categorySchema,
  listingDetailSchema,
  listingSummarySchema,
  listEnvelope,
  successEnvelope,
  type Category,
  type Condition,
  type ListingDetail,
  type ListingSort,
  type ListingSummary,
  type PageMeta,
} from '@/contracts';
import { ApiError, apiFetch } from './client';

/** Catalog read API — categories, listing search, and listing detail. */

/**
 * Category taxonomy is seeded, stable reference data, so it is cached across requests (revalidated
 * hourly, or on demand via `revalidateTag('categories')` when an admin edits the taxonomy) instead
 * of re-fetching on every page/navigation. Also serves repeat calls within a single render.
 *
 * NOTE: we evaluated Next 16 Cache Components (`use cache`) but it prerenders cached data at build
 * time, which requires the backend running during the frontend build/CI — incompatible with our
 * decoupled always-live backend. `unstable_cache` (still supported in 16) is the right fit: it
 * caches the result and only calls the backend on a cache miss at runtime.
 */
export const getCategories = unstable_cache(
  async (): Promise<Category[]> => {
    const { data } = await apiFetch('/v1/categories', successEnvelope(z.array(categorySchema)));
    return data;
  },
  ['catalog:categories'],
  { revalidate: 3600, tags: ['categories'] },
);

export interface SearchListingsParams {
  q?: string;
  category?: string;
  subcategory?: string;
  condition?: Condition;
  province?: string;
  priceMin?: number;
  priceMax?: number;
  sort?: ListingSort;
  page?: number;
  pageSize?: number;
}

export interface SearchListingsResult {
  items: ListingSummary[];
  meta: PageMeta;
}

export async function searchListings(params: SearchListingsParams): Promise<SearchListingsResult> {
  const query = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null && value !== '') {
      query.set(key, String(value));
    }
  }

  const result = await apiFetch(
    `/v1/listings?${query.toString()}`,
    listEnvelope(listingSummarySchema),
  );
  return { items: result.data, meta: result.meta };
}

/** Returns the listing, or null when it is not found / no longer public (404). */
export async function getListingBySlug(slug: string): Promise<ListingDetail | null> {
  try {
    const { data } = await apiFetch(
      `/v1/listings/${encodeURIComponent(slug)}`,
      successEnvelope(listingDetailSchema),
    );
    return data;
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      return null;
    }
    throw error;
  }
}

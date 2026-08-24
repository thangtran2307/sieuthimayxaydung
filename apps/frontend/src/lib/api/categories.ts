import { unstable_cache } from 'next/cache';
import { z } from 'zod';
import { categorySchema, successEnvelope, type Category } from '@/contracts';
import { apiFetch } from './client';

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

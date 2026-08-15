import { z } from 'zod';
import { Condition } from './common/enums';

/** Listing sort options accepted by `GET /listings`. */
export const listingSortSchema = z.enum(['recent', 'price_asc', 'price_desc', 'relevance']);
export type ListingSort = z.infer<typeof listingSortSchema>;

/**
 * Parsed, sanitized search filters read from the URL query string. Invalid values are dropped or
 * defaulted (never throw) so a hand-edited URL still renders.
 */
export const searchParamsSchema = z.object({
  q: z.string().trim().optional().catch(undefined),
  category: z.string().optional().catch(undefined),
  subcategory: z.string().optional().catch(undefined),
  condition: Condition.optional().catch(undefined),
  province: z.string().optional().catch(undefined),
  priceMin: z.coerce.number().int().nonnegative().optional().catch(undefined),
  priceMax: z.coerce.number().int().nonnegative().optional().catch(undefined),
  sort: listingSortSchema.catch('recent'),
  page: z.coerce.number().int().min(1).catch(1),
});
export type SearchParams = z.infer<typeof searchParamsSchema>;

import { z } from 'zod';

/** Pagination bounds shared by every collection endpoint (Constitution: Principle II & VII). */
export const PAGE_DEFAULT = 1;
export const PAGE_SIZE_DEFAULT = 20;
export const PAGE_SIZE_MAX = 50;

/**
 * Query schema for paginated endpoints. Invalid values are CLAMPED, not rejected:
 * `page` floors at 1; `pageSize` is constrained to [1, PAGE_SIZE_MAX].
 */
export const paginationQuerySchema = z.object({
  page: z.coerce
    .number()
    .int()
    .catch(PAGE_DEFAULT)
    .transform((v) => (v < 1 ? PAGE_DEFAULT : v)),
  pageSize: z.coerce
    .number()
    .int()
    .catch(PAGE_SIZE_DEFAULT)
    .transform((v) => Math.min(Math.max(v, 1), PAGE_SIZE_MAX)),
});
export type PaginationQuery = z.infer<typeof paginationQuerySchema>;

export const pageMetaSchema = z.object({
  page: z.number().int(),
  pageSize: z.number().int(),
  total: z.number().int(),
  totalPages: z.number().int(),
});
export type PageMeta = z.infer<typeof pageMetaSchema>;

/** Build page metadata from a total count and the effective pagination query. */
export function buildPageMeta(total: number, page: number, pageSize: number): PageMeta {
  return {
    page,
    pageSize,
    total,
    totalPages: pageSize > 0 ? Math.ceil(total / pageSize) : 0,
  };
}

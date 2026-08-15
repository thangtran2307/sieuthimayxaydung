import { z } from 'zod';

/** Taxonomy node (category or subcategory) — mirrors the API `Category`. */
export const categorySchema = z.object({
  id: z.string(),
  slug: z.string(),
  parentId: z.string().nullable(),
  labelVi: z.string(),
  labelEn: z.string(),
  icon: z.string().nullable(),
  sortOrder: z.number().int(),
});
export type Category = z.infer<typeof categorySchema>;

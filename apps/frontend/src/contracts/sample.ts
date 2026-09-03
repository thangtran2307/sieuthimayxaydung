import { z } from 'zod';

export const sampleSchema = z.object({
  id: z.string(),
  fieldOne: z.string(),
  fieldTwo: z.string(),
});
export type Sample = z.infer<typeof sampleSchema>;

export const createUpdateSampleSchema = z.object({
  fieldOne: z.string(),
  fieldTwo: z.string(),
});
export type CreateUpdateSample = z.infer<typeof createUpdateSampleSchema>;
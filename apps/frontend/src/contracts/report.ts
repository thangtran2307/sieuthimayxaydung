import { z } from 'zod';
import { ReportReason } from './common/enums';

/** A visitor's flag against a listing — mirrors the API `CreateReportRequest`. */
export const createReportSchema = z.object({
  reason: ReportReason,
  details: z.string().max(2000).optional(),
  reporterContact: z.string().max(255).optional(),
});
export type CreateReport = z.infer<typeof createReportSchema>;

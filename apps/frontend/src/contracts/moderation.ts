import { z } from 'zod';
import { ListingStatus, ReportReason, ReportStatus } from './common/enums';

/** A pending listing in the admin review queue — mirrors the API `ModerationQueueItem`. */
export const moderationQueueItemSchema = z.object({
  id: z.string(),
  slug: z.string(),
  title: z.string(),
  sellerName: z.string(),
  categorySlug: z.string(),
  priceAmount: z.number().nullable(),
  priceContact: z.boolean(),
  currency: z.string(),
  locationProvince: z.string(),
  createdAt: z.string(),
});
export type ModerationQueueItem = z.infer<typeof moderationQueueItemSchema>;

/** A report shown in the admin report queue — mirrors the API `Report`. */
export const moderationReportSchema = z.object({
  id: z.string(),
  listingId: z.string(),
  listingSlug: z.string(),
  listingTitle: z.string(),
  listingStatus: ListingStatus,
  reason: ReportReason,
  details: z.string().nullable(),
  reporterContact: z.string().nullable(),
  status: ReportStatus,
  createdAt: z.string(),
});
export type ModerationReport = z.infer<typeof moderationReportSchema>;

/** How an admin resolves a report. */
export const ReportResolution = z.enum(['REMOVE', 'CLEAR']);
export type ReportResolution = z.infer<typeof ReportResolution>;

/** Result of a bulk moderation action. */
export const bulkModerationResultSchema = z.object({
  succeeded: z.number().int(),
  skipped: z.number().int(),
});
export type BulkModerationResult = z.infer<typeof bulkModerationResultSchema>;

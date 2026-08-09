import { z } from 'zod';

/**
 * Shared domain enums used across API contracts.
 * These mirror the Postgres enums / API OpenAPI schema exposed by the .NET backend.
 */

export const UserRole = z.enum(['SELLER', 'ADMIN']);
export type UserRole = z.infer<typeof UserRole>;

export const Condition = z.enum(['NEW', 'USED']);
export type Condition = z.infer<typeof Condition>;

export const ListingStatus = z.enum([
  'PENDING',
  'ACTIVE',
  'REJECTED',
  'SOLD',
  'EXPIRED',
  'REMOVED',
]);
export type ListingStatus = z.infer<typeof ListingStatus>;

export const BoostTier = z.enum(['BASIC', 'FEATURED', 'MAX']);
export type BoostTier = z.infer<typeof BoostTier>;

export const BoostStatus = z.enum(['REQUESTED', 'ACTIVE', 'EXPIRED', 'CANCELLED']);
export type BoostStatus = z.infer<typeof BoostStatus>;

export const ReportReason = z.enum(['FRAUD', 'INCORRECT_INFO', 'SPAM', 'PROHIBITED', 'OTHER']);
export type ReportReason = z.infer<typeof ReportReason>;

export const ReportStatus = z.enum(['OPEN', 'RESOLVED_REMOVED', 'RESOLVED_CLEARED']);
export type ReportStatus = z.infer<typeof ReportStatus>;

export const InquiryType = z.enum(['PHONE_REVEAL', 'MESSAGE']);
export type InquiryType = z.infer<typeof InquiryType>;

export const ModerationAction = z.enum(['APPROVED', 'REJECTED', 'REMOVED']);
export type ModerationAction = z.infer<typeof ModerationAction>;

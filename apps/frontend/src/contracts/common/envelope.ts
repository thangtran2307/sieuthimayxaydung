import { z } from 'zod';
import { pageMetaSchema } from './pagination';

/**
 * Standardized response envelope (Constitution: Principle I).
 * Success (single): { data }.  Success (collection): { data, meta }.  Error: { error }.
 */

export function successEnvelope<T extends z.ZodTypeAny>(data: T) {
  return z.object({ data });
}

export function listEnvelope<T extends z.ZodTypeAny>(item: T) {
  return z.object({ data: z.array(item), meta: pageMetaSchema });
}

/** Stable, machine-readable error codes shared by API and clients. */
export const ErrorCode = z.enum([
  'VALIDATION_FAILED',
  'UNAUTHENTICATED',
  'FORBIDDEN',
  'NOT_FOUND',
  'LISTING_NOT_FOUND',
  'LISTING_NOT_PUBLIC',
  'NOT_LISTING_OWNER',
  'EMAIL_IN_USE',
  'INVALID_CREDENTIALS',
  'BOOST_CONFLICT',
  'LISTING_NOT_ACTIVE_FOR_BOOST',
  'UNSUPPORTED_MEDIA',
  'RATE_LIMITED',
  'INTERNAL_ERROR',
]);
export type ErrorCode = z.infer<typeof ErrorCode>;

export const fieldErrorSchema = z.object({
  path: z.string(),
  message: z.string(),
});
export type FieldError = z.infer<typeof fieldErrorSchema>;

export const errorEnvelopeSchema = z.object({
  error: z.object({
    code: ErrorCode,
    message: z.string(),
    details: z.union([z.array(fieldErrorSchema), z.record(z.unknown()), z.null()]).optional(),
  }),
});
export type ErrorEnvelope = z.infer<typeof errorEnvelopeSchema>;

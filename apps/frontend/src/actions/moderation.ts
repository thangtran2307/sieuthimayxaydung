'use server';

import { z } from 'zod';
import {
  bulkModerationResultSchema,
  successEnvelope,
  type BulkModerationResult,
  type ModerationAction,
  type ReportResolution,
} from '@/contracts';
import { ApiError } from '@/lib/api/client';
import { serverApiFetch } from '@/lib/api/server';

/** Serializable result for a moderation mutation. */
export type ModerationActionResult = { ok: true } | { ok: false; code: string; message: string };

const idSchema = z.string().uuid();

/** Maps a moderation action to its endpoint verb. */
const ACTION_PATH: Record<ModerationAction, string> = {
  APPROVED: 'approve',
  REJECTED: 'reject',
  REMOVED: 'remove',
};

/** Approve / reject / remove a single listing. */
export async function moderateListing(
  id: string,
  action: ModerationAction,
  reason?: string,
): Promise<ModerationActionResult> {
  if (!idSchema.safeParse(id).success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid listing id.' };
  }

  return run(() =>
    serverApiFetch(`/v1/moderation/listings/${id}/${ACTION_PATH[action]}`, z.unknown(), {
      method: 'POST',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify({ reason: reason ?? null }),
    }),
  );
}

/** Apply one action to several listings at once; returns the succeeded/skipped counts. */
export async function bulkModerate(
  listingIds: string[],
  action: ModerationAction,
  reason?: string,
): Promise<
  { ok: true; result: BulkModerationResult } | { ok: false; code: string; message: string }
> {
  if (listingIds.length === 0 || !listingIds.every((id) => idSchema.safeParse(id).success)) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Select at least one valid listing.' };
  }

  try {
    const { data } = await serverApiFetch(
      '/v1/moderation/listings/bulk',
      successEnvelope(bulkModerationResultSchema),
      {
        method: 'POST',
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify({ listingIds, action, reason: reason ?? null }),
      },
    );
    return { ok: true, result: data };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, code: error.code, message: error.message };
    }
    throw error;
  }
}

/** Resolve a report by removing the listing or clearing the report. */
export async function resolveReport(
  id: string,
  resolution: ReportResolution,
  reason?: string,
): Promise<ModerationActionResult> {
  if (!idSchema.safeParse(id).success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid report id.' };
  }

  return run(() =>
    serverApiFetch(`/v1/moderation/reports/${id}/resolve`, z.unknown(), {
      method: 'POST',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify({ resolution, reason: reason ?? null }),
    }),
  );
}

/** Runs a mutating call and maps a thrown {@link ApiError} into a serializable result. */
async function run(call: () => Promise<unknown>): Promise<ModerationActionResult> {
  try {
    await call();
    return { ok: true };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, code: error.code, message: error.message };
    }
    throw error;
  }
}

'use server';

import {
  createListingSchema,
  createdListingSchema,
  successEnvelope,
  type CreateListingInput,
} from '@/contracts';
import { ApiError } from '@/lib/api/client';
import { serverApiFetch } from '@/lib/api/server';

/** Serializable result for the create-listing form. */
export type CreateListingResult =
  | { ok: true; slug: string; status: string }
  | { ok: false; code: string; message: string };

/** Creates a listing for the authenticated seller (starts pending review). */
export async function createListing(input: CreateListingInput): Promise<CreateListingResult> {
  const parsed = createListingSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid input.' };
  }

  try {
    const { data } = await serverApiFetch(
      '/v1/seller/listings',
      successEnvelope(createdListingSchema),
      {
        method: 'POST',
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(parsed.data),
      },
    );
    return { ok: true, slug: data.slug, status: data.status };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, code: error.code, message: error.message };
    }
    throw error;
  }
}

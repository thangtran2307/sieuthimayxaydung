'use server';

import { z } from 'zod';
import {
  createListingSchema,
  createdListingSchema,
  successEnvelope,
  updateListingSchema,
  type CreateListingInput,
  type UpdateListingInput,
} from '@/contracts';
import { ApiError } from '@/lib/api/client';
import { serverApiFetch } from '@/lib/api/server';

/** Serializable result for the create-listing form. */
export type CreateListingResult =
  | { ok: true; slug: string; status: string }
  | { ok: false; code: string; message: string };

/** Serializable result for a dashboard mutation (mark-sold / delete). */
export type ListingActionResult = { ok: true } | { ok: false; code: string; message: string };

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

/** Applies edits to one of the seller's own listings. */
export async function updateListing(
  id: string,
  input: UpdateListingInput,
): Promise<ListingActionResult> {
  if (!z.string().uuid().safeParse(id).success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid listing id.' };
  }

  const parsed = updateListingSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid input.' };
  }

  try {
    await serverApiFetch(`/v1/seller/listings/${id}`, z.unknown(), {
      method: 'PUT',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify(parsed.data),
    });
    return { ok: true };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, code: error.code, message: error.message };
    }
    throw error;
  }
}

/** Marks one of the seller's own listings as sold. */
export async function markListingSold(id: string): Promise<ListingActionResult> {
  return mutateOwnListing(id, `/v1/seller/listings/${id}/sold`, 'POST');
}

/** Removes one of the seller's own listings (soft delete). */
export async function deleteListing(id: string): Promise<ListingActionResult> {
  return mutateOwnListing(id, `/v1/seller/listings/${id}`, 'DELETE');
}

async function mutateOwnListing(
  id: string,
  path: string,
  method: 'POST' | 'DELETE',
): Promise<ListingActionResult> {
  if (!z.string().uuid().safeParse(id).success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid listing id.' };
  }

  try {
    // The endpoints return 204 No Content, so there is no body to validate. The dashboard refreshes
    // via router.refresh() on the client after the action resolves.
    await serverApiFetch(path, z.unknown(), { method });
    return { ok: true };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, code: error.code, message: error.message };
    }
    throw error;
  }
}

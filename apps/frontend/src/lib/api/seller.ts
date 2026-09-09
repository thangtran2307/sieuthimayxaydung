import 'server-only';
import { z } from 'zod';
import {
  sellerListingDetailSchema,
  sellerListingSchema,
  successEnvelope,
  type SellerListing,
  type SellerListingDetail,
} from '@/contracts';
import { ApiError } from './client';
import { serverApiFetch } from './server';

/** Returns the authenticated seller's own listings (any status) for their dashboard. */
export async function getMyListings(): Promise<SellerListing[]> {
  const { data } = await serverApiFetch(
    '/v1/seller/listings',
    successEnvelope(z.array(sellerListingSchema)),
  );
  return data;
}

/** Loads one of the seller's own listings to pre-fill the edit form, or null when not found. */
export async function getSellerListing(id: string): Promise<SellerListingDetail | null> {
  try {
    const { data } = await serverApiFetch(
      `/v1/seller/listings/${encodeURIComponent(id)}`,
      successEnvelope(sellerListingDetailSchema),
    );
    return data;
  } catch (error) {
    // 404 (missing) and 403 (not the owner) both mean "not editable by this user" → treat as absent.
    if (error instanceof ApiError && (error.status === 404 || error.status === 403)) {
      return null;
    }
    throw error;
  }
}

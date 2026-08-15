import { z } from 'zod';
import type { CreateReport } from '@/contracts';
import { apiFetch } from './client';

/** Reports a listing for admin review (201 No Content). No account required. */
export async function createReport(listingId: string, body: CreateReport): Promise<void> {
  await apiFetch(`/v1/listings/${listingId}/reports`, z.unknown(), {
    method: 'POST',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(body),
  });
}

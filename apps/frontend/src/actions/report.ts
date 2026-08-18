'use server';

import { z } from 'zod';
import type { CreateReport } from '@/contracts';
import { apiFetch } from '@/lib/api/client';

/** Flags a listing for admin review. Runs on the Next.js server. */
export async function createReport(listingId: string, body: CreateReport): Promise<void> {
  await apiFetch(`/v1/listings/${listingId}/reports`, z.unknown(), {
    method: 'POST',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(body),
  });
}

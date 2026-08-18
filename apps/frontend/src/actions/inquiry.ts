'use server';

import {
  inquiryResultSchema,
  successEnvelope,
  type CreateInquiry,
  type InquiryResult,
} from '@/contracts';
import { apiFetch } from '@/lib/api/client';

/** Records a buyer's contact (phone reveal or message). Runs on the Next.js server. */
export async function createInquiry(
  listingId: string,
  body: CreateInquiry,
): Promise<InquiryResult> {
  const { data } = await apiFetch(
    `/v1/listings/${listingId}/inquiries`,
    successEnvelope(inquiryResultSchema),
    {
      method: 'POST',
      headers: { 'content-type': 'application/json' },
      body: JSON.stringify(body),
    },
  );
  return data;
}

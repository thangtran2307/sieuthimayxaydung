import {
  inquiryResultSchema,
  successEnvelope,
  type CreateInquiry,
  type InquiryResult,
} from '@/contracts';
import { apiFetch } from './client';

/** Records a buyer's contact (phone reveal or message). No account required. */
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

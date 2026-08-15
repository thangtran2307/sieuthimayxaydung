import { describe, expect, it } from 'vitest';
import {
  createInquirySchema,
  errorEnvelopeSchema,
  listEnvelope,
  listingSummarySchema,
} from '@/contracts';

describe('createInquirySchema', () => {
  it('requires name and message for a MESSAGE inquiry', () => {
    expect(createInquirySchema.safeParse({ type: 'MESSAGE' }).success).toBe(false);
    expect(
      createInquirySchema.safeParse({ type: 'MESSAGE', buyerName: 'Buyer', message: 'Hi' }).success,
    ).toBe(true);
  });

  it('allows a bare PHONE_REVEAL inquiry', () => {
    expect(createInquirySchema.safeParse({ type: 'PHONE_REVEAL' }).success).toBe(true);
  });
});

describe('envelopes', () => {
  it('validates a paged list envelope', () => {
    const schema = listEnvelope(listingSummarySchema);
    const parsed = schema.safeParse({
      data: [],
      meta: { page: 1, pageSize: 20, total: 0, totalPages: 0 },
    });
    expect(parsed.success).toBe(true);
  });

  it('rejects a list envelope missing its meta', () => {
    const schema = listEnvelope(listingSummarySchema);
    expect(schema.safeParse({ data: [] }).success).toBe(false);
  });

  it('validates an error envelope with a known code', () => {
    expect(
      errorEnvelopeSchema.safeParse({ error: { code: 'LISTING_NOT_FOUND', message: 'nope' } })
        .success,
    ).toBe(true);
  });
});

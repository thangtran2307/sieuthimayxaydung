import { z } from 'zod';
import { InquiryType } from './common/enums';

/**
 * Buyer contact payload. For a MESSAGE, name and message are required; for a PHONE_REVEAL all
 * contact fields are optional (mirrors the API `CreateInquiryRequest` + its rules).
 */
export const createInquirySchema = z
  .object({
    type: InquiryType,
    buyerName: z.string().optional(),
    buyerPhone: z.string().optional(),
    buyerEmail: z.string().email().optional().or(z.literal('')),
    message: z.string().optional(),
  })
  .refine((v) => v.type !== 'MESSAGE' || (!!v.buyerName?.trim() && !!v.message?.trim()), {
    message: 'name-and-message-required',
    path: ['message'],
  });
export type CreateInquiry = z.infer<typeof createInquirySchema>;

/** Contact result — `sellerPhone` is present only for a phone reveal. */
export const inquiryResultSchema = z.object({
  type: InquiryType,
  sellerPhone: z.string().nullable(),
});
export type InquiryResult = z.infer<typeof inquiryResultSchema>;

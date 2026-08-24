import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { ContactSeller } from '@/features/inquiries/contact-seller';
import { createInquiry } from '@/actions/inquiry';
import { renderWithIntl } from '@/test/test-utils';

vi.mock('@/actions/inquiry', () => ({
  createInquiry: vi.fn().mockResolvedValue({ type: 'PHONE_REVEAL', sellerPhone: '0900000000' }),
}));

describe('ContactSeller', () => {
  it('reveals the seller phone number on request', async () => {
    renderWithIntl(<ContactSeller listingId="abc" />);

    await userEvent.click(screen.getByRole('button', { name: 'Show phone number' }));

    expect(await screen.findByText('0900000000')).toBeInTheDocument();
    expect(createInquiry).toHaveBeenCalledWith('abc', { type: 'PHONE_REVEAL' });
  });
});

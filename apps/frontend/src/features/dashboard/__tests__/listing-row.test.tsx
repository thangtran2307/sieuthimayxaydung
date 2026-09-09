import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { deleteListing, markListingSold } from '@/actions/listing';
import type { SellerListing } from '@/contracts';
import { ListingRow } from '@/features/dashboard/listing-row';
import { renderWithIntl } from '@/test/test-utils';

const { refresh } = vi.hoisted(() => ({ refresh: vi.fn() }));

vi.mock('@/actions/listing', () => ({
  markListingSold: vi.fn(),
  deleteListing: vi.fn(),
}));
vi.mock('@/i18n/navigation', () => ({
  useRouter: () => ({ refresh }),
  Link: ({ children }: { children: React.ReactNode }) => children,
}));

const LISTING_ID = '11111111-1111-1111-1111-111111111111';

function listing(overrides: Partial<SellerListing> = {}): SellerListing {
  return {
    id: LISTING_ID,
    slug: 'komatsu-pc200-abc',
    title: 'Komatsu PC200',
    status: 'ACTIVE',
    priceAmount: 850_000_000,
    priceContact: false,
    currency: 'VND',
    viewCount: 12,
    thumbnailUrl: null,
    createdAt: '2026-01-01T00:00:00Z',
    publishedAt: '2026-01-02T00:00:00Z',
    ...overrides,
  };
}

describe('ListingRow', () => {
  beforeEach(() => vi.clearAllMocks());
  afterEach(() => vi.restoreAllMocks());

  it('renders the status, title, and view count', () => {
    renderWithIntl(<ListingRow listing={listing()} />);

    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Komatsu PC200')).toBeInTheDocument();
    expect(screen.getByText('12 views')).toBeInTheDocument();
  });

  it('marks the listing sold and refreshes', async () => {
    vi.mocked(markListingSold).mockResolvedValue({ ok: true });
    renderWithIntl(<ListingRow listing={listing()} />);

    await userEvent.click(screen.getByRole('button', { name: 'Mark sold' }));

    await waitFor(() => expect(markListingSold).toHaveBeenCalledWith(LISTING_ID));
    expect(refresh).toHaveBeenCalled();
  });

  it('deletes only after the confirmation is accepted', async () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    vi.mocked(deleteListing).mockResolvedValue({ ok: true });
    renderWithIntl(<ListingRow listing={listing()} />);

    await userEvent.click(screen.getByRole('button', { name: 'Delete listing' }));
    expect(deleteListing).not.toHaveBeenCalled();

    vi.mocked(globalThis.confirm).mockReturnValue(true);
    await userEvent.click(screen.getByRole('button', { name: 'Delete listing' }));
    await waitFor(() => expect(deleteListing).toHaveBeenCalledWith(LISTING_ID));
  });

  it('hides mark-sold for a non-active listing and shows an error on failure', async () => {
    vi.mocked(deleteListing).mockResolvedValue({ ok: false, code: 'FORBIDDEN', message: 'no' });
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    renderWithIntl(<ListingRow listing={listing({ status: 'SOLD' })} />);

    expect(screen.queryByRole('button', { name: 'Mark sold' })).not.toBeInTheDocument();

    await userEvent.click(screen.getByRole('button', { name: 'Delete listing' }));
    expect(await screen.findByText('Something went wrong. Please try again.')).toBeInTheDocument();
  });
});

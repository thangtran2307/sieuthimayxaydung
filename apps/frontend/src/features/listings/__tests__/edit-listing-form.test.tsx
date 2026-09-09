import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { updateListing } from '@/actions/listing';
import type { Category, SellerListingDetail } from '@/contracts';
import { EditListingForm } from '@/features/listings/edit-listing-form';
import { renderWithIntl } from '@/test/test-utils';

const { push, refresh } = vi.hoisted(() => ({ push: vi.fn(), refresh: vi.fn() }));

vi.mock('@/actions/listing', () => ({ updateListing: vi.fn() }));
vi.mock('@/i18n/navigation', () => ({ useRouter: () => ({ push, refresh }) }));

const CATEGORY_ID = '11111111-1111-1111-1111-111111111111';
const LISTING_ID = '22222222-2222-2222-2222-222222222222';

const categories: Category[] = [
  { id: CATEGORY_ID, slug: 'excavators', parentId: null, labelVi: 'Máy đào', labelEn: 'Excavators', icon: 'truck', sortOrder: 0 },
];

const listing: SellerListingDetail = {
  id: LISTING_ID,
  categoryId: CATEGORY_ID,
  subcategoryId: null,
  title: 'Komatsu PC200',
  condition: 'USED',
  priceAmount: 850_000_000,
  priceContact: false,
  locationProvince: 'Hà Nội',
  description: 'Well maintained.',
  specs: { year: 2018, brand: 'Komatsu', model: null, hours: null, origin: null, capacity: null },
  status: 'ACTIVE',
};

describe('EditListingForm', () => {
  beforeEach(() => vi.clearAllMocks());

  it('prefills the existing values', () => {
    renderWithIntl(<EditListingForm listingId={LISTING_ID} listing={listing} categories={categories} />);

    expect(screen.getByLabelText('Title')).toHaveValue('Komatsu PC200');
    expect(screen.getByLabelText('Province / city')).toHaveValue('Hà Nội');
    expect(screen.getByLabelText('Price (VND)')).toHaveValue(850_000_000);
    expect(screen.getByLabelText('Year')).toHaveValue(2018);
  });

  it('submits the edits and returns to the dashboard on success', async () => {
    vi.mocked(updateListing).mockResolvedValue({ ok: true });
    renderWithIntl(<EditListingForm listingId={LISTING_ID} listing={listing} categories={categories} />);

    const title = screen.getByLabelText('Title');
    await userEvent.clear(title);
    await userEvent.type(title, 'Komatsu PC210');
    await userEvent.click(screen.getByRole('button', { name: 'Save changes' }));

    await waitFor(() =>
      expect(updateListing).toHaveBeenCalledWith(
        LISTING_ID,
        expect.objectContaining({ title: 'Komatsu PC210', categoryId: CATEGORY_ID }),
      ),
    );
    expect(push).toHaveBeenCalledWith('/dashboard');
  });

  it('shows a mapped error and stays on the page on failure', async () => {
    vi.mocked(updateListing).mockResolvedValue({ ok: false, code: 'CATEGORY_NOT_FOUND', message: 'x' });
    renderWithIntl(<EditListingForm listingId={LISTING_ID} listing={listing} categories={categories} />);

    await userEvent.click(screen.getByRole('button', { name: 'Save changes' }));

    expect(await screen.findByText('The selected category is invalid.')).toBeInTheDocument();
    expect(push).not.toHaveBeenCalled();
  });
});

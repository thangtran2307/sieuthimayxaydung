import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { createListing } from '@/actions/listing';
import type { Category } from '@/contracts';
import { CreateListingForm } from '@/features/listings/create-listing-form';
import { renderWithIntl } from '@/test/test-utils';

vi.mock('@/actions/listing', () => ({ createListing: vi.fn() }));
vi.mock('@/i18n/navigation', () => ({
  Link: ({ children }: { children: React.ReactNode }) => children,
}));

const CATEGORY_ID = '11111111-1111-1111-1111-111111111111';

const categories: Category[] = [
  {
    id: CATEGORY_ID,
    slug: 'excavators',
    parentId: null,
    labelVi: 'Máy đào',
    labelEn: 'Excavators',
    icon: 'truck',
    sortOrder: 0,
  },
];

async function fillRequiredFields() {
  await userEvent.type(screen.getByLabelText('Title'), 'Komatsu PC200');
  await userEvent.selectOptions(screen.getByLabelText('Category'), CATEGORY_ID);
  await userEvent.type(screen.getByLabelText('Province / city'), 'Hà Nội');
  await userEvent.type(screen.getByLabelText('Price (VND)'), '850000000');
  await userEvent.type(screen.getByLabelText('Description'), 'Well maintained.');
  await userEvent.type(screen.getByPlaceholderText('https://…'), 'https://cdn.example/1.jpg');
}

describe('CreateListingForm', () => {
  beforeEach(() => vi.clearAllMocks());

  it('submits the listing and shows the pending-review panel on success', async () => {
    vi.mocked(createListing).mockResolvedValue({ ok: true, slug: 'komatsu-pc200-abc', status: 'PENDING' });
    renderWithIntl(<CreateListingForm categories={categories} />);

    await fillRequiredFields();
    await userEvent.click(screen.getByRole('button', { name: 'Submit for review' }));

    await waitFor(() =>
      expect(createListing).toHaveBeenCalledWith(
        expect.objectContaining({
          categoryId: CATEGORY_ID,
          title: 'Komatsu PC200',
          condition: 'USED',
          priceContact: false,
          priceAmount: 850_000_000,
          locationProvince: 'Hà Nội',
          photos: [expect.objectContaining({ url: 'https://cdn.example/1.jpg', sortOrder: 0 })],
        }),
      ),
    );
    expect(await screen.findByText('Listing submitted')).toBeInTheDocument();
  });

  it('clears the price when "contact for price" is chosen', async () => {
    vi.mocked(createListing).mockResolvedValue({ ok: true, slug: 's', status: 'PENDING' });
    renderWithIntl(<CreateListingForm categories={categories} />);

    await userEvent.type(screen.getByLabelText('Title'), 'Crane');
    await userEvent.selectOptions(screen.getByLabelText('Category'), CATEGORY_ID);
    await userEvent.type(screen.getByLabelText('Province / city'), 'Hà Nội');
    await userEvent.type(screen.getByLabelText('Description'), 'No price.');
    await userEvent.type(screen.getByPlaceholderText('https://…'), 'https://cdn.example/1.jpg');
    await userEvent.click(screen.getByLabelText('Contact for price'));
    await userEvent.click(screen.getByRole('button', { name: 'Submit for review' }));

    await waitFor(() =>
      expect(createListing).toHaveBeenCalledWith(
        expect.objectContaining({ priceContact: true, priceAmount: null }),
      ),
    );
  });

  it('adds and removes photo URL rows', async () => {
    renderWithIntl(<CreateListingForm categories={categories} />);

    expect(screen.getAllByPlaceholderText('https://…')).toHaveLength(1);
    await userEvent.click(screen.getByRole('button', { name: 'Add photo' }));
    expect(screen.getAllByPlaceholderText('https://…')).toHaveLength(2);

    await userEvent.click(screen.getAllByRole('button', { name: 'Remove photo' })[0]);
    expect(screen.getAllByPlaceholderText('https://…')).toHaveLength(1);
  });

  it('maps a backend category error to a message', async () => {
    vi.mocked(createListing).mockResolvedValue({
      ok: false,
      code: 'CATEGORY_NOT_FOUND',
      message: 'bad',
    });
    renderWithIntl(<CreateListingForm categories={categories} />);

    await fillRequiredFields();
    await userEvent.click(screen.getByRole('button', { name: 'Submit for review' }));

    expect(await screen.findByText('The selected category is invalid.')).toBeInTheDocument();
  });
});

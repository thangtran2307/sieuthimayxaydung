import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { bulkModerate, moderateListing } from '@/actions/moderation';
import type { ModerationQueueItem } from '@/contracts';
import { ModerationQueue } from '@/features/moderation/moderation-queue';
import { renderWithIntl } from '@/test/test-utils';

const { refresh } = vi.hoisted(() => ({ refresh: vi.fn() }));

vi.mock('@/actions/moderation', () => ({
  moderateListing: vi.fn(),
  bulkModerate: vi.fn(),
}));
vi.mock('@/i18n/navigation', () => ({ useRouter: () => ({ refresh }) }));

function item(id: string, title: string): ModerationQueueItem {
  return {
    id,
    slug: `${title}-slug`,
    title,
    sellerName: 'Seller Co.',
    categorySlug: 'excavators',
    priceAmount: 100,
    priceContact: false,
    currency: 'VND',
    locationProvince: 'Hà Nội',
    createdAt: '2026-01-01T00:00:00Z',
  };
}

const A = '11111111-1111-1111-1111-111111111111';
const B = '22222222-2222-2222-2222-222222222222';

describe('ModerationQueue', () => {
  beforeEach(() => vi.clearAllMocks());

  it('shows an empty state when there is nothing to review', () => {
    renderWithIntl(<ModerationQueue items={[]} />);
    expect(screen.getByText('Nothing awaiting review.')).toBeInTheDocument();
  });

  it('approves a single listing and refreshes', async () => {
    vi.mocked(moderateListing).mockResolvedValue({ ok: true });
    renderWithIntl(<ModerationQueue items={[item(A, 'Excavator')]} />);

    await userEvent.click(screen.getByRole('button', { name: 'Approve' }));

    await waitFor(() => expect(moderateListing).toHaveBeenCalledWith(A, 'APPROVED'));
    expect(refresh).toHaveBeenCalled();
  });

  it('bulk-approves the selected listings', async () => {
    vi.mocked(bulkModerate).mockResolvedValue({ ok: true, result: { succeeded: 2, skipped: 0 } });
    renderWithIntl(<ModerationQueue items={[item(A, 'Excavator'), item(B, 'Crane')]} />);

    await userEvent.click(screen.getByLabelText('Select all'));
    // The bulk bar's Approve is the first "Approve" button once a selection exists.
    await userEvent.click(screen.getAllByRole('button', { name: 'Approve' })[0]);

    await waitFor(() =>
      expect(bulkModerate).toHaveBeenCalledWith(expect.arrayContaining([A, B]), 'APPROVED'),
    );
    expect(await screen.findByText('2 updated, 0 skipped.')).toBeInTheDocument();
  });
});

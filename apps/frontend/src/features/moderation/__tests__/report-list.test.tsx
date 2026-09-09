import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { resolveReport } from '@/actions/moderation';
import type { ModerationReport } from '@/contracts';
import { ReportList } from '@/features/moderation/report-list';
import { renderWithIntl } from '@/test/test-utils';

const { refresh } = vi.hoisted(() => ({ refresh: vi.fn() }));

vi.mock('@/actions/moderation', () => ({ resolveReport: vi.fn() }));
vi.mock('@/i18n/navigation', () => ({
  useRouter: () => ({ refresh }),
  Link: ({ children }: { children: React.ReactNode }) => children,
}));

const REPORT_ID = '33333333-3333-3333-3333-333333333333';

const report: ModerationReport = {
  id: REPORT_ID,
  listingId: '44444444-4444-4444-4444-444444444444',
  listingSlug: 'excavator-slug',
  listingTitle: 'Excavator',
  listingStatus: 'ACTIVE',
  reason: 'FRAUD',
  details: 'Looks fake',
  reporterContact: 'reporter@example.com',
  status: 'OPEN',
  createdAt: '2026-01-01T00:00:00Z',
};

describe('ReportList', () => {
  beforeEach(() => vi.clearAllMocks());
  afterEach(() => vi.restoreAllMocks());

  it('shows an empty state when there are no reports', () => {
    renderWithIntl(<ReportList reports={[]} />);
    expect(screen.getByText('No open reports.')).toBeInTheDocument();
  });

  it('clears a report and refreshes', async () => {
    vi.mocked(resolveReport).mockResolvedValue({ ok: true });
    renderWithIntl(<ReportList reports={[report]} />);

    await userEvent.click(screen.getByRole('button', { name: 'Clear' }));

    await waitFor(() => expect(resolveReport).toHaveBeenCalledWith(REPORT_ID, 'CLEAR'));
    expect(refresh).toHaveBeenCalled();
  });

  it('removes the listing only after confirmation', async () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(false);
    vi.mocked(resolveReport).mockResolvedValue({ ok: true });
    renderWithIntl(<ReportList reports={[report]} />);

    await userEvent.click(screen.getByRole('button', { name: 'Remove listing' }));
    expect(resolveReport).not.toHaveBeenCalled();

    vi.mocked(globalThis.confirm).mockReturnValue(true);
    await userEvent.click(screen.getByRole('button', { name: 'Remove listing' }));
    await waitFor(() => expect(resolveReport).toHaveBeenCalledWith(REPORT_ID, 'REMOVE'));
  });
});

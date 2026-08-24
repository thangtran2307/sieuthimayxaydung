import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { ReportListing } from '@/features/reports/report-listing';
import { createReport } from '@/actions/report';
import { renderWithIntl } from '@/test/test-utils';

vi.mock('@/actions/report', () => ({
  createReport: vi.fn().mockResolvedValue(undefined),
}));

describe('ReportListing', () => {
  it('opens the form, submits a report, and shows confirmation', async () => {
    renderWithIntl(<ReportListing listingId="abc" />);

    await userEvent.click(screen.getByRole('button', { name: 'Report this listing' }));
    await userEvent.click(screen.getByRole('button', { name: 'Submit report' }));

    await waitFor(() =>
      expect(createReport).toHaveBeenCalledWith('abc', { reason: 'FRAUD', details: undefined }),
    );
    expect(await screen.findByText('Thanks — your report has been submitted.')).toBeInTheDocument();
  });
});

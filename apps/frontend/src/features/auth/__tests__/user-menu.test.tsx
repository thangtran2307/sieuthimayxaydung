import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { logout } from '@/actions/auth';
import { UserMenu } from '@/features/auth/user-menu';
import { renderWithIntl } from '@/test/test-utils';

const { push, refresh } = vi.hoisted(() => ({ push: vi.fn(), refresh: vi.fn() }));

vi.mock('@/actions/auth', () => ({ logout: vi.fn().mockResolvedValue(undefined) }));
vi.mock('@/i18n/navigation', () => ({ useRouter: () => ({ push, refresh }) }));

describe('UserMenu', () => {
  beforeEach(() => vi.clearAllMocks());

  it('shows the account name', () => {
    renderWithIntl(<UserMenu name="Seller Co." />);
    expect(screen.getByText('Seller Co.')).toBeInTheDocument();
  });

  it('logs out and refreshes on click', async () => {
    renderWithIntl(<UserMenu name="Seller Co." />);

    await userEvent.click(screen.getByRole('button', { name: 'Log out' }));

    await waitFor(() => expect(logout).toHaveBeenCalled());
    expect(refresh).toHaveBeenCalled();
    expect(push).toHaveBeenCalledWith('/');
  });
});

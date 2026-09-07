import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { register } from '@/actions/auth';
import { RegisterForm } from '@/features/auth/register-form';
import { renderWithIntl } from '@/test/test-utils';

const { push, refresh } = vi.hoisted(() => ({ push: vi.fn(), refresh: vi.fn() }));

vi.mock('@/actions/auth', () => ({ register: vi.fn() }));
vi.mock('@/i18n/navigation', () => ({ useRouter: () => ({ push, refresh }) }));

describe('RegisterForm', () => {
  beforeEach(() => vi.clearAllMocks());

  it('submits the account details and navigates home on success', async () => {
    vi.mocked(register).mockResolvedValue({ ok: true });
    renderWithIntl(<RegisterForm />);

    await userEvent.type(screen.getByLabelText('Display name'), 'Seller Co.');
    await userEvent.type(screen.getByLabelText('Email'), 'seller@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'supersecret');
    await userEvent.click(screen.getByRole('button', { name: 'Create account' }));

    await waitFor(() =>
      expect(register).toHaveBeenCalledWith(
        expect.objectContaining({
          displayName: 'Seller Co.',
          email: 'seller@example.com',
          password: 'supersecret',
        }),
      ),
    );
    expect(push).toHaveBeenCalledWith('/');
  });

  it('surfaces a duplicate-email error', async () => {
    vi.mocked(register).mockResolvedValue({ ok: false, code: 'EMAIL_IN_USE', message: 'dup' });
    renderWithIntl(<RegisterForm />);

    await userEvent.type(screen.getByLabelText('Display name'), 'Seller Co.');
    await userEvent.type(screen.getByLabelText('Email'), 'taken@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'supersecret');
    await userEvent.click(screen.getByRole('button', { name: 'Create account' }));

    expect(
      await screen.findByText('An account with this email already exists.'),
    ).toBeInTheDocument();
    expect(push).not.toHaveBeenCalled();
  });
});

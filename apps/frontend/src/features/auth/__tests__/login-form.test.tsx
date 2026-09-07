import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { login } from '@/actions/auth';
import { LoginForm } from '@/features/auth/login-form';
import { renderWithIntl } from '@/test/test-utils';

const { push, refresh } = vi.hoisted(() => ({ push: vi.fn(), refresh: vi.fn() }));

vi.mock('@/actions/auth', () => ({ login: vi.fn() }));
vi.mock('@/i18n/navigation', () => ({ useRouter: () => ({ push, refresh }) }));

describe('LoginForm', () => {
  beforeEach(() => vi.clearAllMocks());

  it('submits credentials and navigates home on success', async () => {
    vi.mocked(login).mockResolvedValue({ ok: true });
    renderWithIntl(<LoginForm />);

    await userEvent.type(screen.getByLabelText('Email'), 'seller@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'supersecret');
    await userEvent.click(screen.getByRole('button', { name: 'Log in' }));

    await waitFor(() =>
      expect(login).toHaveBeenCalledWith({ email: 'seller@example.com', password: 'supersecret' }),
    );
    expect(push).toHaveBeenCalledWith('/');
    expect(refresh).toHaveBeenCalled();
  });

  it('shows a mapped error and does not navigate on failure', async () => {
    vi.mocked(login).mockResolvedValue({
      ok: false,
      code: 'INVALID_CREDENTIALS',
      message: 'bad',
    });
    renderWithIntl(<LoginForm />);

    await userEvent.type(screen.getByLabelText('Email'), 'seller@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'wrong-password');
    await userEvent.click(screen.getByRole('button', { name: 'Log in' }));

    expect(await screen.findByText('Incorrect email or password.')).toBeInTheDocument();
    expect(push).not.toHaveBeenCalled();
  });
});

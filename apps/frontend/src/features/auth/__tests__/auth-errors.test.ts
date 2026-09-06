import { describe, expect, it } from 'vitest';
import { authErrorKey } from '@/features/auth/auth-errors';

describe('authErrorKey', () => {
  it('maps known backend codes to specific message keys', () => {
    expect(authErrorKey('INVALID_CREDENTIALS')).toBe('errors.invalidCredentials');
    expect(authErrorKey('EMAIL_IN_USE')).toBe('errors.emailInUse');
    expect(authErrorKey('VALIDATION_FAILED')).toBe('errors.validation');
  });

  it('falls back to the generic key for unknown codes', () => {
    expect(authErrorKey('SOMETHING_ELSE')).toBe('errors.generic');
  });
});

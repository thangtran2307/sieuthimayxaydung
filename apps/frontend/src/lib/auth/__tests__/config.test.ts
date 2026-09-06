import { describe, expect, it } from 'vitest';
import { readSessionTokens } from '@/lib/auth/config';

describe('readSessionTokens', () => {
  it('extracts access and refresh values from Set-Cookie headers', () => {
    const tokens = readSessionTokens([
      'access_token=abc.def.ghi; Path=/; HttpOnly; SameSite=Lax; Max-Age=900',
      'refresh_token=rrr.sss.ttt; Path=/; HttpOnly; SameSite=Lax; Max-Age=2592000',
    ]);

    expect(tokens).toEqual({ access: 'abc.def.ghi', refresh: 'rrr.sss.ttt' });
  });

  it('returns null when either token is missing', () => {
    expect(readSessionTokens(['access_token=only; Path=/'])).toBeNull();
    expect(readSessionTokens([])).toBeNull();
  });
});

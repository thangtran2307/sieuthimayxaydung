import { describe, expect, it } from 'vitest';
import { decodeJwt, isExpiring } from '@/lib/auth/jwt';

/** Builds an unsigned JWT (header.payload.signature) with the given payload for decode tests. */
function makeToken(payload: Record<string, unknown>): string {
  const encode = (obj: unknown) =>
    Buffer.from(JSON.stringify(obj)).toString('base64url');
  return `${encode({ alg: 'HS256', typ: 'JWT' })}.${encode(payload)}.signature`;
}

describe('decodeJwt', () => {
  it('decodes the exp claim of a well-formed token', () => {
    const token = makeToken({ sub: 'user-1', exp: 123 });
    expect(decodeJwt(token)?.exp).toBe(123);
  });

  it('returns null for malformed tokens', () => {
    expect(decodeJwt('not-a-jwt')).toBeNull();
    expect(decodeJwt('')).toBeNull();
  });
});

describe('isExpiring', () => {
  const now = Math.floor(Date.now() / 1000);

  it('is false for a token well in the future', () => {
    expect(isExpiring({ exp: now + 600 }, 60)).toBe(false);
  });

  it('is true within the skew window', () => {
    expect(isExpiring({ exp: now + 30 }, 60)).toBe(true);
  });

  it('is true when exp is missing or the payload is null', () => {
    expect(isExpiring({}, 60)).toBe(true);
    expect(isExpiring(null, 60)).toBe(true);
  });
});

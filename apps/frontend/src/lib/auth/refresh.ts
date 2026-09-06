import type { NextRequest } from 'next/server';
import { ACCESS_COOKIE, API_BASE_URL, REFRESH_COOKIE, readSessionTokens } from './config';
import { decodeJwt, isExpiring } from './jwt';

// Refresh a little before the access token actually expires, to avoid racing the boundary.
const REFRESH_SKEW_SECONDS = 60;

/** What the middleware should do to the session cookies for this request. */
export type SessionMutation =
  | { type: 'set'; access: string; refresh: string }
  | { type: 'clear' }
  | null;

/**
 * Decides whether the session needs rotating. If the access token is missing or about to expire but a
 * refresh token is present, it exchanges it at the backend for a fresh pair. Returns `clear` when the
 * refresh token is rejected, or `null` when nothing needs to change (or the backend was unreachable).
 */
export async function computeSessionRefresh(request: NextRequest): Promise<SessionMutation> {
  const access = request.cookies.get(ACCESS_COOKIE)?.value;
  const refresh = request.cookies.get(REFRESH_COOKIE)?.value;

  if (!refresh) {
    return null; // anonymous — nothing to refresh with
  }

  if (access && !isExpiring(decodeJwt(access), REFRESH_SKEW_SECONDS)) {
    return null; // still valid
  }

  try {
    const response = await fetch(`${API_BASE_URL}/v1/auth/refresh`, {
      method: 'POST',
      headers: { cookie: `${REFRESH_COOKIE}=${refresh}` },
      cache: 'no-store',
    });

    if (!response.ok) {
      return { type: 'clear' }; // refresh token expired/invalid → sign out
    }

    const tokens = readSessionTokens(response.headers.getSetCookie());
    return tokens ? { type: 'set', ...tokens } : { type: 'clear' };
  } catch {
    // Backend unreachable — leave the existing session alone rather than logging the user out.
    return null;
  }
}

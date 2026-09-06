import 'server-only';
import { cookies } from 'next/headers';
import { cache } from 'react';
import { authUserSchema, successEnvelope, type AuthUser } from '@/contracts';
import { ACCESS_COOKIE, API_BASE_URL, REFRESH_COOKIE, sessionCookieOptions } from './config';

/**
 * Resolves the current account by asking the backend (`/auth/me`) — the frontend never parses the
 * JWT itself. The access-token cookie is forwarded server-side; a missing/invalid token yields null.
 * Wrapped in `cache` so a single render (e.g. header + page) makes at most one call.
 */
export const getCurrentUser = cache(async (): Promise<AuthUser | null> => {
  const store = await cookies();
  const token = store.get(ACCESS_COOKIE)?.value;
  if (!token) {
    return null;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/v1/auth/me`, {
      headers: { cookie: `${ACCESS_COOKIE}=${token}` },
      cache: 'no-store',
    });
    if (!response.ok) {
      return null;
    }

    const json: unknown = await response.json().catch(() => null);
    const parsed = successEnvelope(authUserSchema).safeParse(json);
    return parsed.success ? parsed.data.data : null;
  } catch {
    return null;
  }
});

/** Persists the backend-issued tokens as httpOnly cookies on this (Next.js) origin. */
export async function setSessionCookies(access: string, refresh: string): Promise<void> {
  const store = await cookies();
  store.set(ACCESS_COOKIE, access, sessionCookieOptions());
  store.set(REFRESH_COOKIE, refresh, sessionCookieOptions());
}

/** Clears the session cookies (logout). */
export async function clearSessionCookies(): Promise<void> {
  const store = await cookies();
  store.delete(ACCESS_COOKIE);
  store.delete(REFRESH_COOKIE);
}

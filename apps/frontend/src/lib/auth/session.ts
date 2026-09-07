import 'server-only';
import { cookies } from 'next/headers';
import { cache } from 'react';
import { authUserSchema, successEnvelope, type AuthUser } from '@/contracts';
import { ApiError } from '@/lib/api/client';
import { serverApiFetch } from '@/lib/api/server';
import { ACCESS_COOKIE, REFRESH_COOKIE, sessionCookieOptions } from './config';

/**
 * Resolves the current account by asking the backend (`/auth/me`) via the shared authed fetch — the
 * frontend never parses the JWT itself. A missing token short-circuits to null (no call); a rejected
 * session (401 after a refresh attempt) also yields null. Wrapped in `cache` so a single render
 * (e.g. header + page) makes at most one call.
 */
export const getCurrentUser = cache(async (): Promise<AuthUser | null> => {
  const store = await cookies();
  if (!store.get(ACCESS_COOKIE)?.value) {
    return null;
  }

  try {
    const { data } = await serverApiFetch('/v1/auth/me', successEnvelope(authUserSchema));
    return data;
  } catch (error) {
    if (error instanceof ApiError) {
      return null;
    }
    throw error;
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

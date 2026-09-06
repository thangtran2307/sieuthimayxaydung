import createMiddleware from 'next-intl/middleware';
import type { NextRequest } from 'next/server';
import { routing } from './i18n/routing';
import { ACCESS_COOKIE, REFRESH_COOKIE, sessionCookieOptions } from './lib/auth/config';
import { computeSessionRefresh } from './lib/auth/refresh';

// Next 16 renamed the `middleware` file convention to `proxy`.
const handleI18n = createMiddleware(routing);

/**
 * Runs on every page request: first silently rotates the session (refreshes the access token before
 * it expires, or clears the cookies when the refresh token is dead), then delegates to next-intl for
 * locale routing (FR-010). Refreshed cookies are written onto the outgoing response.
 */
export default async function proxy(request: NextRequest) {
  const mutation = await computeSessionRefresh(request);

  // Update the incoming request too, so this render already sees the rotated token where possible.
  if (mutation?.type === 'set') {
    request.cookies.set(ACCESS_COOKIE, mutation.access);
    request.cookies.set(REFRESH_COOKIE, mutation.refresh);
  } else if (mutation?.type === 'clear') {
    request.cookies.delete(ACCESS_COOKIE);
    request.cookies.delete(REFRESH_COOKIE);
  }

  const response = handleI18n(request);

  if (mutation?.type === 'set') {
    response.cookies.set(ACCESS_COOKIE, mutation.access, sessionCookieOptions());
    response.cookies.set(REFRESH_COOKIE, mutation.refresh, sessionCookieOptions());
  } else if (mutation?.type === 'clear') {
    response.cookies.delete(ACCESS_COOKIE);
    response.cookies.delete(REFRESH_COOKIE);
  }

  return response;
}

export const config = {
  // Skip API routes, Next internals, and static files. `[.]` matches a literal dot without a
  // backslash escape (Next requires a statically-analyzable string literal here).
  matcher: ['/((?!api|_next|_vercel|.*[.].*).*)'],
};

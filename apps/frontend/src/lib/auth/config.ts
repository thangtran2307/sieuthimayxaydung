/**
 * Shared session/auth constants and helpers. Deliberately free of `next/headers` and Node-only APIs
 * so it can be imported from both Server Actions (Node) and the proxy middleware (Edge).
 */

export const ACCESS_COOKIE = 'access_token';
export const REFRESH_COOKIE = 'refresh_token';

/** How long the cookie CONTAINER lives. The access token value itself expires sooner (JWT `exp`);
 *  the middleware rotates it before then, so the container is kept long-lived (matches refresh TTL). */
export const SESSION_MAX_AGE = 60 * 60 * 24 * 30; // 30 days

/**
 * Backend base URL. `API_URL` (server-only) wins when set — e.g. an internal address behind the
 * reverse proxy — falling back to the public URL used by browser reads.
 */
export const API_BASE_URL =
  process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5127/api';

const isProduction = process.env.NODE_ENV === 'production';

/** httpOnly session cookie options (Secure only in production so dev over http still works). */
export function sessionCookieOptions(maxAge: number = SESSION_MAX_AGE) {
  return {
    httpOnly: true,
    secure: isProduction,
    sameSite: 'lax' as const,
    path: '/',
    maxAge,
  };
}

/** Extracts the access + refresh token values from a backend response's Set-Cookie headers. */
export function readSessionTokens(setCookies: readonly string[]): { access: string; refresh: string } | null {
  const access = extractCookieValue(setCookies, ACCESS_COOKIE);
  const refresh = extractCookieValue(setCookies, REFRESH_COOKIE);
  return access && refresh ? { access, refresh } : null;
}

function extractCookieValue(setCookies: readonly string[], name: string): string | null {
  const prefix = `${name}=`;
  for (const cookie of setCookies) {
    if (cookie.startsWith(prefix)) {
      const end = cookie.indexOf(';');
      return end === -1 ? cookie.slice(prefix.length) : cookie.slice(prefix.length, end);
    }
  }
  return null;
}

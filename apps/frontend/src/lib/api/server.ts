import 'server-only';
import { cookies } from 'next/headers';
import type { z } from 'zod';
import {
  ACCESS_COOKIE,
  API_BASE_URL,
  REFRESH_COOKIE,
  readSessionTokens,
  sessionCookieOptions,
} from '@/lib/auth/config';
import { parseApiResponse } from './client';

type CookieStore = Awaited<ReturnType<typeof cookies>>;

/**
 * Server-side fetch for AUTHENTICATED backend calls. The access token is read from the httpOnly
 * cookie and forwarded to the backend as an `access_token` cookie (its primary auth mechanism; the
 * browser never calls the backend directly). On a 401 it refreshes once via the refresh token and
 * retries — covering the gap when the middleware's boundary refresh didn't reach this render.
 * Validates the envelope and throws {@link ApiError}.
 */
export async function serverApiFetch<T>(
  path: string,
  schema: z.ZodType<T>,
  init?: RequestInit,
): Promise<T> {
  const store = await cookies();
  let token = store.get(ACCESS_COOKIE)?.value;

  let response = await callBackend(path, init, token);

  if (response.status === 401) {
    const refreshed = await tryRefresh(store);
    if (refreshed) {
      token = refreshed;
      response = await callBackend(path, init, token);
    }
  }

  return parseApiResponse(response, path, schema);
}

function callBackend(path: string, init: RequestInit | undefined, token: string | undefined) {
  return fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      ...init?.headers,
      ...(token ? { cookie: `${ACCESS_COOKIE}=${token}` } : {}),
    },
    cache: 'no-store',
  });
}

/** Exchanges the refresh token for a new pair; persists it best-effort and returns the access token. */
async function tryRefresh(store: CookieStore): Promise<string | null> {
  const refresh = store.get(REFRESH_COOKIE)?.value;
  if (!refresh) {
    return null;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/v1/auth/refresh`, {
      method: 'POST',
      headers: { cookie: `${REFRESH_COOKIE}=${refresh}` },
      cache: 'no-store',
    });
    if (!response.ok) {
      return null;
    }

    const tokens = readSessionTokens(response.headers.getSetCookie());
    if (!tokens) {
      return null;
    }

    try {
      store.set(ACCESS_COOKIE, tokens.access, sessionCookieOptions());
      store.set(REFRESH_COOKIE, tokens.refresh, sessionCookieOptions());
    } catch {
      // Read-only context (RSC render): can't persist cookies here — use the token in-memory for the
      // retry; the middleware persists on the next request.
    }

    return tokens.access;
  } catch {
    return null;
  }
}

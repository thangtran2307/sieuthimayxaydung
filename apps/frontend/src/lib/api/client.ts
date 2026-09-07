import { errorEnvelopeSchema, type ErrorCode } from '@/contracts';
import type { z } from 'zod';

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5127/api';

/** Typed error thrown when the API returns a non-2xx response. */
export class ApiError extends Error {
  constructor(
    public readonly code: ErrorCode | string,
    message: string,
    public readonly status: number,
    public readonly details?: unknown,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

/**
 * Fetches from the backend and validates the response against a shared contract schema (the Zod
 * schemas in `src/contracts` mirror the backend OpenAPI). Isomorphic: works in Server Components and
 * in the browser. Cookies are sent automatically in the browser via `credentials: 'include'`; for
 * authenticated server rendering, pass the cookie header through `init.headers`.
 */
export async function apiFetch<T>(
  path: string,
  schema: z.ZodType<T>,
  init?: RequestInit,
): Promise<T> {
  const res = await fetch(`${API_URL}${path}`, {
    ...init,
    credentials: 'include',
    cache: 'no-store',
  });

  return parseApiResponse(res, path, schema);
}

/**
 * Shared response handling for every backend call (used by {@link apiFetch} and the server-side
 * authed fetch): validates a success body against the contract schema, or parses the error envelope
 * into a typed {@link ApiError}. Callers differ only in transport (public/isomorphic vs authed
 * server-only), not in how responses are interpreted.
 */
export async function parseApiResponse<T>(
  response: Response,
  path: string,
  schema: z.ZodType<T>,
): Promise<T> {
  const json: unknown = await response.json().catch(() => null);

  if (!response.ok) {
    const parsed = errorEnvelopeSchema.safeParse(json);
    if (parsed.success) {
      throw new ApiError(
        parsed.data.error.code,
        parsed.data.error.message,
        response.status,
        parsed.data.error.details,
      );
    }
    throw new ApiError('INTERNAL_ERROR', `Request to ${path} failed`, response.status, json);
  }

  return schema.parse(json);
}

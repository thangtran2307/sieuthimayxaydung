import { errorEnvelopeSchema, type ErrorCode } from '@/contracts';
import type { z } from 'zod';

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:3001/api';

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
 * Fetches from the backend and validates the response against a shared contract schema
 * (Zod schemas in src/contracts mirror the backend OpenAPI). During SSR, forwards the cookies so the
 * authenticated session is preserved.
 */
export async function apiFetch<T>(
  path: string,
  schema: z.ZodType<T>,
  init?: RequestInit,
): Promise<T> {
  const headers = new Headers(init?.headers);

  if (typeof window === 'undefined') {
    const { cookies } = await import('next/headers');
    const cookieHeader = (await cookies()).toString();
    if (cookieHeader) {
      headers.set('cookie', cookieHeader);
    }
  }

  const res = await fetch(`${API_URL}${path}`, {
    ...init,
    headers,
    credentials: 'include',
    cache: 'no-store',
  });

  const json: unknown = await res.json().catch(() => null);

  if (!res.ok) {
    const parsed = errorEnvelopeSchema.safeParse(json);
    if (parsed.success) {
      throw new ApiError(
        parsed.data.error.code,
        parsed.data.error.message,
        res.status,
        parsed.data.error.details,
      );
    }
    throw new ApiError('INTERNAL_ERROR', `Request to ${path} failed`, res.status);
  }

  return schema.parse(json);
}

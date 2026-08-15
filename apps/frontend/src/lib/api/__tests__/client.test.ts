import { afterEach, describe, expect, it, vi } from 'vitest';
import { z } from 'zod';
import { ApiError, apiFetch } from '@/lib/api/client';

function stubFetch(status: number, body: unknown) {
  vi.stubGlobal(
    'fetch',
    vi.fn().mockResolvedValue({
      ok: status >= 200 && status < 300,
      status,
      json: () => Promise.resolve(body),
    } as Response),
  );
}

describe('apiFetch', () => {
  afterEach(() => vi.unstubAllGlobals());

  it('parses a successful response against the schema', async () => {
    stubFetch(200, { value: 42 });
    const result = await apiFetch('/x', z.object({ value: z.number() }));
    expect(result).toEqual({ value: 42 });
  });

  it('throws ApiError carrying the envelope code and status on error responses', async () => {
    stubFetch(404, { error: { code: 'LISTING_NOT_FOUND', message: 'nope' } });
    await expect(apiFetch('/x', z.unknown())).rejects.toMatchObject({
      code: 'LISTING_NOT_FOUND',
      status: 404,
    });
  });

  it('throws a generic ApiError when the error body is not an envelope', async () => {
    stubFetch(500, 'oops');
    const error = await apiFetch('/x', z.unknown()).catch((e: unknown) => e);
    expect(error).toBeInstanceOf(ApiError);
    expect((error as ApiError).code).toBe('INTERNAL_ERROR');
  });
});

import { afterEach, describe, expect, it, vi } from 'vitest';
import { logger } from '@/lib/logger';

describe('logger', () => {
  afterEach(() => vi.restoreAllMocks());

  it('writes errors to console.error including the message', () => {
    const spy = vi.spyOn(console, 'error').mockImplementation(() => {});
    logger.error('boom', new Error('kaboom'), { slug: 'x' });
    expect(spy).toHaveBeenCalledTimes(1);
    expect(String(spy.mock.calls[0]?.[0])).toContain('boom');
  });

  it('writes info to console.info', () => {
    const spy = vi.spyOn(console, 'info').mockImplementation(() => {});
    logger.info('hello');
    expect(spy).toHaveBeenCalledTimes(1);
  });
});

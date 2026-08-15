import { describe, expect, it } from 'vitest';
import { searchParamsSchema } from '@/contracts/search';

describe('searchParamsSchema', () => {
  it('applies defaults for an empty query', () => {
    const result = searchParamsSchema.parse({});
    expect(result.sort).toBe('recent');
    expect(result.page).toBe(1);
    expect(result.q).toBeUndefined();
  });

  it('coerces numeric fields from strings', () => {
    const result = searchParamsSchema.parse({ priceMin: '1000', priceMax: '5000', page: '3' });
    expect(result.priceMin).toBe(1000);
    expect(result.priceMax).toBe(5000);
    expect(result.page).toBe(3);
  });

  it('falls back to "recent" for an invalid sort', () => {
    expect(searchParamsSchema.parse({ sort: 'bogus' }).sort).toBe('recent');
  });

  it('clamps an invalid or out-of-range page to 1', () => {
    expect(searchParamsSchema.parse({ page: '0' }).page).toBe(1);
    expect(searchParamsSchema.parse({ page: 'abc' }).page).toBe(1);
  });

  it('drops an invalid condition rather than throwing', () => {
    expect(searchParamsSchema.parse({ condition: 'BROKEN' }).condition).toBeUndefined();
    expect(searchParamsSchema.parse({ condition: 'USED' }).condition).toBe('USED');
  });
});

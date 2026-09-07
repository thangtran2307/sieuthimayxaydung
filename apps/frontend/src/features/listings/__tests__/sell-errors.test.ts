import { describe, expect, it } from 'vitest';
import { sellErrorKey } from '@/features/listings/sell-errors';

describe('sellErrorKey', () => {
  it('maps known backend codes to specific message keys', () => {
    expect(sellErrorKey('CATEGORY_NOT_FOUND')).toBe('errors.categoryNotFound');
    expect(sellErrorKey('INVALID_SUBCATEGORY')).toBe('errors.invalidSubcategory');
    expect(sellErrorKey('UNAUTHENTICATED')).toBe('errors.unauthenticated');
    expect(sellErrorKey('VALIDATION_FAILED')).toBe('errors.validation');
  });

  it('falls back to the generic key for unknown codes', () => {
    expect(sellErrorKey('BOOM')).toBe('errors.generic');
  });
});

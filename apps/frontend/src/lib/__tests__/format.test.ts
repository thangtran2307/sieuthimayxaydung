import { describe, expect, it } from 'vitest';
import { formatDate, formatPrice } from '@/lib/format';

describe('formatPrice', () => {
  it('returns the contact label when price is contact-only', () => {
    expect(formatPrice(1000, true, 'vi', 'Liên hệ')).toBe('Liên hệ');
  });

  it('returns the contact label when the amount is null', () => {
    expect(formatPrice(null, false, 'en', 'Contact for price')).toBe('Contact for price');
  });

  it('formats a VND amount as currency', () => {
    const result = formatPrice(1_000_000, false, 'vi', 'Liên hệ');
    expect(result).not.toBe('Liên hệ');
    expect(result).toMatch(/\d/);
    expect(result).toContain('₫');
  });
});

describe('formatDate', () => {
  it('formats an ISO timestamp into a locale date', () => {
    expect(formatDate('2026-01-15T00:00:00Z', 'en')).toMatch(/2026/);
  });
});

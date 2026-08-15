/** Locale-aware formatting helpers for money and dates. */

const intlLocale = (locale: string): string => (locale === 'vi' ? 'vi-VN' : 'en-US');

/** Formats a whole-VND amount, or returns `contactLabel` for "contact for price" listings. */
export function formatPrice(
  amount: number | null,
  priceContact: boolean,
  locale: string,
  contactLabel: string,
): string {
  if (priceContact || amount === null) {
    return contactLabel;
  }
  return new Intl.NumberFormat(intlLocale(locale), {
    style: 'currency',
    currency: 'VND',
    maximumFractionDigits: 0,
  }).format(amount);
}

/** Formats an ISO timestamp as a locale-aware date. */
export function formatDate(iso: string, locale: string): string {
  return new Intl.DateTimeFormat(intlLocale(locale), { dateStyle: 'medium' }).format(new Date(iso));
}

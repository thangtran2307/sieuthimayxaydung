import { defineRouting } from 'next-intl/routing';

/** VI is the primary market; EN is the secondary language (spec FR-010). */
export const routing = defineRouting({
  locales: ['vi', 'en'],
  defaultLocale: 'vi',
});

export type AppLocale = (typeof routing.locales)[number];

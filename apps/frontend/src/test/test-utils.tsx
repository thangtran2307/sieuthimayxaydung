import { render, type RenderResult } from '@testing-library/react';
import { NextIntlClientProvider } from 'next-intl';
import type { ReactElement } from 'react';
import en from '@/messages/en.json';

/** Renders a component inside the i18n provider (English messages) for component tests. */
export function renderWithIntl(ui: ReactElement, locale = 'en'): RenderResult {
  return render(
    <NextIntlClientProvider locale={locale} messages={en}>
      {ui}
    </NextIntlClientProvider>,
  );
}

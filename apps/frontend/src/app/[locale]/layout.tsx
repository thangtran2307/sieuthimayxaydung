import type { Metadata } from 'next';
import { NextIntlClientProvider } from 'next-intl';
import { getMessages } from 'next-intl/server';
import { notFound } from 'next/navigation';
import type { ReactNode } from 'react';
import { Header } from '@/components/header';
import { routing, type AppLocale } from '@/i18n/routing';
import '../globals.css';

export const metadata: Metadata = {
  title: 'MayXayDung — Construction Machinery Marketplace',
  description: 'Buy and sell construction machinery, parts, and equipment in Vietnam.',
};

export default async function LocaleLayout({
  children,
  params,
}: Readonly<{
  children: ReactNode;
  params: Promise<{ locale: string }>;
}>) {
  const { locale } = await params;
  if (!routing.locales.includes(locale as AppLocale)) {
    notFound();
  }
  const messages = await getMessages();

  return (
    <html lang={locale}>
      <body className="font-sans antialiased">
        <NextIntlClientProvider messages={messages}>
          <Header />
          <main className="container py-8">{children}</main>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}

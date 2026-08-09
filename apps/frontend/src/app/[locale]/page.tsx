import { getTranslations, setRequestLocale } from 'next-intl/server';

// Foundational placeholder homepage. The full search-first homepage is delivered in
// User Story 1 (task T032).
export default async function HomePage({ params }: { params: Promise<{ locale: string }> }) {
  const { locale } = await params;
  setRequestLocale(locale);
  const t = await getTranslations('home');

  return (
    <section className="mx-auto max-w-2xl space-y-4 text-center">
      <h1 className="font-display text-4xl font-bold text-brand">{t('title')}</h1>
      <p className="text-lg text-slate-600">{t('subtitle')}</p>
      <p className="rounded-md bg-slate-50 p-4 text-sm text-slate-500">{t('status')}</p>
    </section>
  );
}

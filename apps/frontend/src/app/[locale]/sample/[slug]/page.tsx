import { SampleForm } from '@/features/sample/sample-form';
import { getSampleById } from '@/lib/api/sample';
import { getTranslations } from 'next-intl/server';
import { Suspense } from 'react';

export default async function SampleDetailPage({
  params,
}: Readonly<{
  params: Promise<{ locale: string; slug: string }>;
}>) {
  const { slug, locale } = await params;
  const sample = await getSampleById(slug);
  const t = await getTranslations('sample');

  if (sample === null) {
    return <p>Sample not found</p>;
  }

  return (
    <div>
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <p className="text-slate-600">{t('description')}</p>
      <p>Slug: {slug}</p>

      <Suspense fallback={<p>Loading sample details...</p>}>
        <SampleForm sample={sample} isCreate={false} locale={locale} />
      </Suspense>
    </div>
  );
}

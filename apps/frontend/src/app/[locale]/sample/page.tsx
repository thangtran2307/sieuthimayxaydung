import { Sample } from '@/contracts/sample';
import { getAllSamples } from '@/lib/api/sample';
import { getTranslations } from 'next-intl/server';
import Link from 'next/dist/client/link';
import { Suspense } from 'react';

export default async function SamplePage() {
  const samples = await getAllSamples();
  const t = await getTranslations('sample');

  return (
    <div className="space-y-5">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <p className="text-slate-600">{t('description')}</p>
      <Suspense fallback={<p>Loading samples...</p>}>
        <SampleList samples={samples} />
      </Suspense>
    </div>
  );
}

async function SampleList({ samples }: Readonly<{ samples: Sample[] }>) {
  return (
    <ul className="space-y-2">
      {samples.map((sample) => (
        <Link
          key={sample.id}
          href={`/sample/${sample.id}`}
          className="block rounded-lg border border-slate-200 p-4 hover:bg-slate-50"
        >
          <li className="rounded-lg border border-slate-200 p-4">
            <p className="text-sm text-slate-500">ID: {sample.id}</p>
            <p className="text-sm text-slate-500">Field One: {sample.fieldOne}</p>
            <p className="text-sm text-slate-500">Field Two: {sample.fieldTwo}</p>
          </li>
        </Link>
      ))}
    </ul>
  );
}

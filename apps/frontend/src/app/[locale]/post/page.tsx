import { getTranslations } from 'next-intl/server';
import { redirect } from 'next/navigation';
import { CreateListingForm } from '@/features/listings/create-listing-form';
import { getCategories } from '@/lib/api/categories';
import { getCurrentUser } from '@/lib/auth/session';
import { logger } from '@/lib/logger';

// Auth-gated + live categories — render per request.
export const dynamic = 'force-dynamic';

/** Create-listing page (US2). Requires an authenticated seller (FR-013). */
export default async function PostListingPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  if (!(await getCurrentUser())) {
    redirect(`/${locale}/login`);
  }

  const t = await getTranslations('sell');
  const categories = await getCategories().catch((error) => {
    logger.error('Post listing: failed to load categories', error);
    return [];
  });

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div className="space-y-1">
        <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
        <p className="text-sm text-slate-500">{t('subtitle')}</p>
      </div>
      <CreateListingForm categories={categories} />
    </div>
  );
}

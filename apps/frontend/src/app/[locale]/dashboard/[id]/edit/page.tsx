import { getTranslations } from 'next-intl/server';
import { notFound, redirect } from 'next/navigation';
import { EditListingForm } from '@/features/listings/edit-listing-form';
import { getCategories } from '@/lib/api/categories';
import { getSellerListing } from '@/lib/api/seller';
import { getCurrentUser } from '@/lib/auth/session';
import { logger } from '@/lib/logger';

// Authed + live data — render per request.
export const dynamic = 'force-dynamic';

/** Edit one of the current seller's listings (US2, FR-017). */
export default async function EditListingPage({
  params,
}: Readonly<{ params: Promise<{ locale: string; id: string }> }>) {
  const { locale, id } = await params;
  if (!(await getCurrentUser())) {
    redirect(`/${locale}/login`);
  }

  const [t, listing, categories] = await Promise.all([
    getTranslations('sell'),
    getSellerListing(id),
    getCategories().catch((error) => {
      logger.error('Edit listing: failed to load categories', error);
      return [];
    }),
  ]);

  if (!listing) {
    notFound();
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('editTitle')}</h1>
      <EditListingForm listingId={id} listing={listing} categories={categories} />
    </div>
  );
}

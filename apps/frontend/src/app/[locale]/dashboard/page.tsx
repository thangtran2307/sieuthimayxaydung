import { Package } from 'lucide-react';
import { getTranslations } from 'next-intl/server';
import { redirect } from 'next/navigation';
import type { ReactNode } from 'react';
import { EmptyState } from '@/components/empty-state';
import { buttonVariants } from '@/components/ui/button';
import type { SellerListing } from '@/contracts';
import { ListingRow } from '@/features/dashboard/listing-row';
import { Link } from '@/i18n/navigation';
import { getMyListings } from '@/lib/api/seller';
import { getCurrentUser } from '@/lib/auth/session';
import { logger } from '@/lib/logger';

// Authed + live seller data — render per request (and refresh cleanly on navigate back).
export const dynamic = 'force-dynamic';

/** Seller dashboard (US2): the seller's own listings with status, views, and management actions. */
export default async function DashboardPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  if (!(await getCurrentUser())) {
    redirect(`/${locale}/login`);
  }

  const t = await getTranslations('dashboard');
  const listings = await getMyListings().catch((error) => {
    logger.error('Dashboard: failed to load seller listings', error);
    return null;
  });

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
        <Link href="/post" className={buttonVariants({ variant: 'primary', size: 'md' })}>
          {t('newListing')}
        </Link>
      </div>

      {renderListings(listings, t)}
    </div>
  );
}

/** Chooses the dashboard body: load-error state, empty state, or the list of rows. */
function renderListings(
  listings: SellerListing[] | null,
  t: Awaited<ReturnType<typeof getTranslations<'dashboard'>>>,
): ReactNode {
  if (listings === null) {
    return (
      <EmptyState
        icon={<Package className="h-10 w-10" aria-hidden />}
        title={t('errorTitle')}
        description={t('errorHint')}
      />
    );
  }

  if (listings.length === 0) {
    return (
      <EmptyState
        icon={<Package className="h-10 w-10" aria-hidden />}
        title={t('emptyTitle')}
        description={t('emptyHint')}
      />
    );
  }

  return (
    <div className="space-y-3">
      {listings.map((listing) => (
        <ListingRow key={listing.id} listing={listing} />
      ))}
    </div>
  );
}

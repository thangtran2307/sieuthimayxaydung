import { Package } from 'lucide-react';
import { getTranslations } from 'next-intl/server';
import { Suspense } from 'react';
import { EmptyState } from '@/components/empty-state';
import { CategoryGrid, CategoryGridSkeleton } from '@/features/catalog/category-grid';
import { ListingCard } from '@/features/listings/listing-card';
import { ListingGridSkeleton } from '@/features/listings/listing-skeletons';
import { SearchBar } from '@/features/search/search-bar';
import { getCategories, searchListings } from '@/lib/api/catalog';
import { logger } from '@/lib/logger';

// Live marketplace data — render per request (never serve build-time-empty static HTML).
export const dynamic = 'force-dynamic';

/**
 * Search-first homepage (US1). The hero + search render instantly; the category grid and latest
 * listings each stream in via their own Suspense boundary.
 */
export default async function HomePage() {
  const t = await getTranslations('home');

  return (
    <div className="space-y-12">
      <section className="rounded-xl bg-brand px-6 py-12 text-center text-brand-fg">
        <h1 className="font-display text-3xl font-bold sm:text-4xl">{t('heroTitle')}</h1>
        <p className="mx-auto mt-3 max-w-2xl text-brand-fg/80">{t('heroSubtitle')}</p>
        <div className="mx-auto mt-6 max-w-2xl">
          <SearchBar />
        </div>
      </section>

      <section className="space-y-4">
        <h2 className="font-display text-xl font-semibold text-slate-900">
          {t('browseCategories')}
        </h2>
        <Suspense fallback={<CategoryGridSkeleton />}>
          <HomeCategories />
        </Suspense>
      </section>

      <section className="space-y-4">
        <h2 className="font-display text-xl font-semibold text-slate-900">{t('latest')}</h2>
        <Suspense fallback={<ListingGridSkeleton count={8} />}>
          <HomeLatest />
        </Suspense>
      </section>
    </div>
  );
}

async function HomeCategories() {
  const categories = await getCategories().catch((error) => {
    logger.error('Homepage: failed to load categories', error);
    return [];
  });
  return <CategoryGrid categories={categories} />;
}

async function HomeLatest() {
  const t = await getTranslations('home');
  const latest = await searchListings({ sort: 'recent', pageSize: 8 })
    .then((result) => result.items)
    .catch((error) => {
      logger.error('Homepage: failed to load latest listings', error);
      return [];
    });

  if (latest.length === 0) {
    return (
      <EmptyState
        icon={<Package className="h-10 w-10" aria-hidden />}
        title={t('noListings')}
        description={t('noListingsHint')}
      />
    );
  }

  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
      {latest.map((listing) => (
        <ListingCard key={listing.id} listing={listing} />
      ))}
    </div>
  );
}

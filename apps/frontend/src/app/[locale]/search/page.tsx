import { AlertTriangle } from 'lucide-react';
import { getTranslations } from 'next-intl/server';
import { Suspense } from 'react';
import { EmptyState } from '@/components/empty-state';
import { searchParamsSchema, type SearchParams } from '@/contracts';
import { ListingGrid } from '@/features/listings/listing-grid';
import { Pagination } from '@/features/search/pagination';
import { SearchFilters } from '@/features/search/search-filters';
import { SearchFiltersSkeleton, SearchResultsSkeleton } from '@/features/search/search-skeletons';
import { getCategories, searchListings } from '@/lib/api/catalog';
import { logger } from '@/lib/logger';

const PAGE_SIZE = 20;

// Depends on live results + URL query — always render per request.
export const dynamic = 'force-dynamic';

/**
 * Search results (US1). The heading renders instantly; the filter sidebar and the results column
 * each stream in their own Suspense boundary. The results boundary is keyed on the filters so the
 * skeleton reappears whenever the query changes.
 */
export default async function SearchPage({
  searchParams,
}: Readonly<{
  searchParams: Promise<Record<string, string | string[] | undefined>>;
}>) {
  const t = await getTranslations('search');
  const parsed = searchParamsSchema.parse(await searchParams);

  return (
    <div className="space-y-5">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <div className="grid gap-6 lg:grid-cols-[260px_1fr]">
        <aside>
          <Suspense fallback={<SearchFiltersSkeleton />}>
            <SearchSidebar params={parsed} />
          </Suspense>
        </aside>
        <div className="space-y-4">
          <Suspense key={JSON.stringify(parsed)} fallback={<SearchResultsSkeleton />}>
            <SearchResults params={parsed} />
          </Suspense>
        </div>
      </div>
    </div>
  );
}

async function SearchSidebar({ params }: Readonly<{ params: SearchParams }>) {
  const categories = await getCategories().catch((error) => {
    logger.error('Search: failed to load categories', error);
    return [];
  });
  return <SearchFilters categories={categories} params={params} />;
}

async function SearchResults({ params }: Readonly<{ params: SearchParams }>) {
  const t = await getTranslations('search');
  const result = await searchListings({ ...params, pageSize: PAGE_SIZE }).catch((error) => {
    logger.error('Search: listing query failed', error, { params });
    return null;
  });

  if (!result) {
    return (
      <EmptyState
        icon={<AlertTriangle className="h-10 w-10" aria-hidden />}
        title={t('errorTitle')}
        description={t('errorHint')}
      />
    );
  }

  return (
    <>
      <p className="text-sm text-slate-500">{t('resultsCount', { count: result.meta.total })}</p>
      <ListingGrid listings={result.items} />
      <Pagination page={result.meta.page} totalPages={result.meta.totalPages} />
    </>
  );
}

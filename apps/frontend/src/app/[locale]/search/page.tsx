import { AlertTriangle } from 'lucide-react';
import { getTranslations, setRequestLocale } from 'next-intl/server';
import { EmptyState } from '@/components/empty-state';
import { searchParamsSchema } from '@/contracts';
import { ListingGrid } from '@/features/listings/listing-grid';
import { Pagination } from '@/features/search/pagination';
import { SearchFilters } from '@/features/search/search-filters';
import { getCategories, searchListings } from '@/lib/api/catalog';

const PAGE_SIZE = 20;

// Depends on live results + URL query — always render per request.
export const dynamic = 'force-dynamic';

/** Search results: filter sidebar + results grid + pagination (US1). */
export default async function SearchPage({
  params,
  searchParams,
}: Readonly<{
  params: Promise<{ locale: string }>;
  searchParams: Promise<Record<string, string | string[] | undefined>>;
}>) {
  const { locale } = await params;
  setRequestLocale(locale);
  const t = await getTranslations('search');

  const parsed = searchParamsSchema.parse(await searchParams);

  const [categories, result] = await Promise.all([
    getCategories().catch(() => []),
    searchListings({ ...parsed, pageSize: PAGE_SIZE }).catch(() => null),
  ]);

  return (
    <div className="space-y-5">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <div className="grid gap-6 lg:grid-cols-[260px_1fr]">
        <aside>
          <SearchFilters categories={categories} params={parsed} />
        </aside>
        <div className="space-y-4">
          {result ? (
            <>
              <p className="text-sm text-slate-500">
                {t('resultsCount', { count: result.meta.total })}
              </p>
              <ListingGrid listings={result.items} />
              <Pagination page={result.meta.page} totalPages={result.meta.totalPages} />
            </>
          ) : (
            <EmptyState
              icon={<AlertTriangle className="h-10 w-10" aria-hidden />}
              title={t('errorTitle')}
              description={t('errorHint')}
            />
          )}
        </div>
      </div>
    </div>
  );
}

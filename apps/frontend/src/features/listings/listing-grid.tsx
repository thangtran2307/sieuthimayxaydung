import { SearchX } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { EmptyState } from '@/components/empty-state';
import type { ListingSummary } from '@/contracts';
import { ListingCard } from './listing-card';

/** Responsive grid of listing cards, with a built-in empty state. */
export function ListingGrid({ listings }: Readonly<{ listings: ListingSummary[] }>) {
  const t = useTranslations('search');

  if (listings.length === 0) {
    return (
      <EmptyState
        icon={<SearchX className="h-10 w-10" aria-hidden />}
        title={t('noResults')}
        description={t('noResultsHint')}
      />
    );
  }

  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
      {listings.map((listing) => (
        <ListingCard key={listing.id} listing={listing} />
      ))}
    </div>
  );
}

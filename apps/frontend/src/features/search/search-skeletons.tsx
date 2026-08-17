import { Skeleton } from '@/components/ui/skeleton';
import { ListingGridSkeleton } from '@/features/listings/listing-skeletons';

/** Placeholder for the filter sidebar. */
export function SearchFiltersSkeleton() {
  return (
    <div className="space-y-4 rounded-lg border border-slate-200 bg-white p-4">
      {Array.from({ length: 5 }, (_, index) => `filter-skeleton-${index}`).map((key) => (
        <div key={key} className="space-y-1">
          <Skeleton className="h-3 w-20" />
          <Skeleton className="h-10 w-full" />
        </div>
      ))}
    </div>
  );
}

/** Placeholder for the results column (count line + grid). */
export function SearchResultsSkeleton() {
  return (
    <div className="space-y-4">
      <Skeleton className="h-4 w-24" />
      <ListingGridSkeleton count={8} />
    </div>
  );
}

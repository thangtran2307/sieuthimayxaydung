import { Skeleton } from '@/components/ui/skeleton';

/** Placeholder matching a single ListingCard. */
export function ListingCardSkeleton() {
  return (
    <div className="flex flex-col overflow-hidden rounded-lg border border-slate-200 bg-white">
      <Skeleton className="aspect-[4/3] rounded-none" />
      <div className="flex flex-col gap-2 p-3">
        <Skeleton className="h-4 w-full" />
        <Skeleton className="h-5 w-24" />
        <Skeleton className="h-3 w-20" />
      </div>
    </div>
  );
}

/** Placeholder grid shown while listings load. */
export function ListingGridSkeleton({ count = 8 }: Readonly<{ count?: number }>) {
  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
      {Array.from({ length: count }, (_, index) => `listing-skeleton-${index}`).map((key) => (
        <ListingCardSkeleton key={key} />
      ))}
    </div>
  );
}

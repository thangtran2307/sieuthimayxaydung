import { Skeleton } from '@/components/ui/skeleton';

/** Route-level skeleton shown while a listing detail streams in. */
export default function ListingDetailLoading() {
  return (
    <div className="grid gap-8 lg:grid-cols-[1fr_360px]">
      <div className="space-y-6">
        <Skeleton className="aspect-[4/3] w-full rounded-lg" />
        <div className="space-y-2">
          <Skeleton className="h-5 w-40" />
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-4 w-5/6" />
          <Skeleton className="h-4 w-2/3" />
        </div>
      </div>
      <aside className="space-y-4">
        <div className="space-y-3 rounded-lg border border-slate-200 bg-white p-4">
          <Skeleton className="h-5 w-24" />
          <Skeleton className="h-6 w-3/4" />
          <Skeleton className="h-7 w-1/2" />
          <Skeleton className="h-4 w-1/3" />
        </div>
        <Skeleton className="h-44 w-full rounded-lg" />
        <Skeleton className="h-24 w-full rounded-lg" />
      </aside>
    </div>
  );
}

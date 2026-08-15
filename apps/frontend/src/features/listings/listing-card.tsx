import { MapPin, Package } from 'lucide-react';
import { useLocale, useTranslations } from 'next-intl';
import Image from 'next/image';
import { Badge } from '@/components/ui/badge';
import type { ListingSummary } from '@/contracts';
import { Link } from '@/i18n/navigation';
import { formatPrice } from '@/lib/format';

/** Compact listing card for grids (server component). */
export function ListingCard({ listing }: Readonly<{ listing: ListingSummary }>) {
  const t = useTranslations('listing');
  const locale = useLocale();
  const price = formatPrice(listing.priceAmount, listing.priceContact, locale, t('priceContact'));

  return (
    <Link
      href={`/listing/${listing.slug}`}
      className="group flex flex-col overflow-hidden rounded-lg border border-slate-200 bg-white transition-shadow hover:shadow-md focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand"
    >
      <div className="relative aspect-[4/3] bg-slate-100">
        {listing.thumbnailUrl ? (
          <Image
            src={listing.thumbnailUrl}
            alt={listing.title}
            fill
            sizes="(max-width: 640px) 50vw, (max-width: 1024px) 33vw, 25vw"
            className="object-cover"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-slate-300">
            <Package className="h-10 w-10" aria-hidden />
          </div>
        )}
        {listing.boosted ? (
          <Badge variant="boost" className="absolute left-2 top-2 shadow-sm">
            {t('boosted')}
          </Badge>
        ) : null}
      </div>
      <div className="flex flex-1 flex-col gap-1 p-3">
        <h3 className="line-clamp-2 text-sm font-medium text-slate-900 group-hover:text-brand">
          {listing.title}
        </h3>
        <p className="font-display text-lg font-bold text-accent">{price}</p>
        <p className="mt-auto flex items-center gap-1 pt-1 text-xs text-slate-500">
          <MapPin className="h-3.5 w-3.5" aria-hidden />
          {listing.locationProvince}
        </p>
      </div>
    </Link>
  );
}

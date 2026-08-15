import { BadgeCheck, Eye, MapPin } from 'lucide-react';
import type { Metadata } from 'next';
import { getLocale, getTranslations, setRequestLocale } from 'next-intl/server';
import { cache } from 'react';
import { Badge } from '@/components/ui/badge';
import { buttonVariants } from '@/components/ui/button';
import { EmptyState } from '@/components/empty-state';
import type { ListingDetail } from '@/contracts';
import { ContactSeller } from '@/features/contact/contact-seller';
import { ListingGallery } from '@/features/listings/listing-gallery';
import { ReportListing } from '@/features/report/report-listing';
import { Link } from '@/i18n/navigation';
import { getListingBySlug } from '@/lib/api/catalog';
import { formatDate, formatPrice } from '@/lib/format';

/** Request-scoped cache so metadata + the page share one fetch (one view increment). */
const loadListing = cache(async (slug: string): Promise<ListingDetail | null> => {
  try {
    return await getListingBySlug(slug);
  } catch {
    return null;
  }
});

export async function generateMetadata({
  params,
}: Readonly<{
  params: Promise<{ slug: string }>;
}>): Promise<Metadata> {
  const { slug } = await params;
  const listing = await loadListing(slug);
  if (!listing) {
    return { title: 'MayXayDung' };
  }
  return {
    title: `${listing.title} — MayXayDung`,
    description: listing.description.slice(0, 160),
    openGraph: {
      title: listing.title,
      images: listing.photos[0]?.url ? [listing.photos[0].url] : [],
    },
  };
}

export default async function ListingDetailPage({
  params,
}: Readonly<{
  params: Promise<{ locale: string; slug: string }>;
}>) {
  const { locale, slug } = await params;
  setRequestLocale(locale);
  const t = await getTranslations('listing');
  const listing = await loadListing(slug);

  if (!listing) {
    return (
      <EmptyState
        title={t('notAvailable')}
        description={t('notAvailableHint')}
        action={
          <Link href="/search" className={buttonVariants({ variant: 'primary', size: 'md' })}>
            {t('backToSearch')}
          </Link>
        }
      />
    );
  }

  const activeLocale = await getLocale();
  const price = formatPrice(
    listing.priceAmount,
    listing.priceContact,
    activeLocale,
    t('priceContact'),
  );
  const specs = Object.entries(listing.specs).filter(([, value]) => value !== null && value !== '');

  return (
    <div className="grid gap-8 lg:grid-cols-[1fr_360px]">
      <div className="space-y-6">
        <ListingGallery photos={listing.photos} title={listing.title} />

        {listing.description ? (
          <section className="space-y-2">
            <h2 className="font-display text-lg font-semibold text-slate-900">
              {t('description')}
            </h2>
            <p className="whitespace-pre-line text-sm leading-relaxed text-slate-700">
              {listing.description}
            </p>
          </section>
        ) : null}

        {specs.length > 0 ? (
          <section className="space-y-2">
            <h2 className="font-display text-lg font-semibold text-slate-900">
              {t('specifications')}
            </h2>
            <dl className="grid grid-cols-1 gap-x-6 gap-y-2 sm:grid-cols-2">
              {specs.map(([key, value]) => (
                <div
                  key={key}
                  className="flex justify-between border-b border-slate-100 py-1.5 text-sm"
                >
                  <dt className="text-slate-500">{humanize(key)}</dt>
                  <dd className="font-medium text-slate-800">{String(value)}</dd>
                </div>
              ))}
            </dl>
          </section>
        ) : null}
      </div>

      <aside className="space-y-4">
        <div className="space-y-3 rounded-lg border border-slate-200 bg-white p-4">
          <div className="flex flex-wrap items-center gap-2">
            <Badge variant={listing.condition === 'NEW' ? 'success' : 'neutral'}>
              {listing.condition === 'NEW' ? t('conditionNew') : t('conditionUsed')}
            </Badge>
            {listing.boosted ? <Badge variant="boost">{t('boosted')}</Badge> : null}
          </div>
          <h1 className="font-display text-xl font-bold text-slate-900">{listing.title}</h1>
          <p className="font-display text-2xl font-bold text-accent">{price}</p>
          <p className="flex items-center gap-1.5 text-sm text-slate-500">
            <MapPin className="h-4 w-4" aria-hidden />
            {listing.locationProvince}
          </p>
          <p className="flex items-center gap-1.5 text-xs text-slate-400">
            <Eye className="h-3.5 w-3.5" aria-hidden />
            {t('views', { count: listing.viewCount })}
          </p>
        </div>

        <ContactSeller listingId={listing.id} />

        <div className="space-y-1 rounded-lg border border-slate-200 bg-white p-4">
          <h2 className="font-display text-sm font-semibold uppercase tracking-wide text-slate-500">
            {t('sellerInfo')}
          </h2>
          <p className="flex items-center gap-1.5 font-medium text-slate-900">
            {listing.seller.displayName}
            {listing.seller.verified ? (
              <BadgeCheck className="h-4 w-4 text-brand" aria-label={t('verified')} />
            ) : null}
          </p>
          {listing.seller.locationProvince ? (
            <p className="text-sm text-slate-500">{listing.seller.locationProvince}</p>
          ) : null}
          <p className="text-xs text-slate-400">
            {t('memberSince', { date: formatDate(listing.seller.joinedAt, activeLocale) })}
          </p>
        </div>

        <div className="px-1">
          <ReportListing listingId={listing.id} />
        </div>
      </aside>
    </div>
  );
}

/** Turns a spec key like "loadCapacity" into "Load capacity". */
function humanize(key: string): string {
  const spaced = key.replace(/([a-z])([A-Z])/g, '$1 $2').replace(/[_-]+/g, ' ');
  return spaced.charAt(0).toUpperCase() + spaced.slice(1);
}

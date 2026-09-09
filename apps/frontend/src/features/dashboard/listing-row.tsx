'use client';

import { CheckCircle2, Eye, Loader2, Pencil, Trash2 } from 'lucide-react';
import { useLocale, useTranslations } from 'next-intl';
import { useState, useTransition } from 'react';
import { deleteListing, markListingSold } from '@/actions/listing';
import { Badge } from '@/components/ui/badge';
import { Button, buttonVariants } from '@/components/ui/button';
import type { ListingStatus, SellerListing } from '@/contracts';
import { Link, useRouter } from '@/i18n/navigation';
import { formatDate, formatPrice } from '@/lib/format';

/** Maps a listing status to a badge variant. */
const STATUS_VARIANT: Record<ListingStatus, 'neutral' | 'success' | 'brand'> = {
  PENDING: 'brand',
  ACTIVE: 'success',
  REJECTED: 'neutral',
  SOLD: 'neutral',
  EXPIRED: 'neutral',
  REMOVED: 'neutral',
};

/** A single row in the seller dashboard with mark-sold and delete actions. */
export function ListingRow({ listing }: Readonly<{ listing: SellerListing }>) {
  const t = useTranslations('dashboard');
  const locale = useLocale();
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [pending, startAction] = useTransition();

  const canMarkSold = listing.status === 'ACTIVE' || listing.status === 'PENDING';

  const run = (action: () => Promise<{ ok: boolean; message?: string }>) => {
    startAction(async () => {
      setError(null);
      const result = await action();
      if (result.ok) {
        router.refresh();
        return;
      }
      setError(t('actionError'));
    });
  };

  const onMarkSold = () => run(() => markListingSold(listing.id));
  const onDelete = () => {
    if (globalThis.confirm(t('confirmDelete'))) {
      run(() => deleteListing(listing.id));
    }
  };

  return (
    <div className="flex flex-col gap-3 rounded-lg border border-slate-200 bg-white p-4 sm:flex-row sm:items-center">
      <div className="min-w-0 flex-1 space-y-1">
        <div className="flex items-center gap-2">
          <Badge variant={STATUS_VARIANT[listing.status]}>{t(`status.${listing.status}`)}</Badge>
          <p className="truncate font-medium text-slate-900">{listing.title}</p>
        </div>
        <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-slate-500">
          <span className="font-medium text-slate-700">
            {formatPrice(listing.priceAmount, listing.priceContact, locale, t('priceContact'))}
          </span>
          <span className="flex items-center gap-1">
            <Eye className="h-3.5 w-3.5" aria-hidden />
            {t('views', { count: listing.viewCount })}
          </span>
          <span>{formatDate(listing.createdAt, locale)}</span>
        </div>
        {error ? <p className="text-sm text-red-600">{error}</p> : null}
      </div>

      <div className="flex shrink-0 items-center gap-2">
        <Link
          href={`/dashboard/${listing.id}/edit`}
          className={buttonVariants({ variant: 'outline', size: 'sm' })}
          aria-label={t('edit')}
        >
          <Pencil className="h-4 w-4" aria-hidden />
          {t('edit')}
        </Link>
        {canMarkSold ? (
          <Button variant="outline" size="sm" onClick={onMarkSold} disabled={pending}>
            {pending ? (
              <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
            ) : (
              <CheckCircle2 className="h-4 w-4" aria-hidden />
            )}
            {t('markSold')}
          </Button>
        ) : null}
        <Button
          variant="ghost"
          size="sm"
          onClick={onDelete}
          disabled={pending}
          aria-label={t('delete')}
          className="text-red-600 hover:bg-red-50"
        >
          <Trash2 className="h-4 w-4" aria-hidden />
        </Button>
      </div>
    </div>
  );
}

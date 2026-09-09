'use client';

import { Check, Loader2, Trash2, X } from 'lucide-react';
import { useLocale, useTranslations } from 'next-intl';
import { useState, useTransition } from 'react';
import { bulkModerate, moderateListing } from '@/actions/moderation';
import { Button } from '@/components/ui/button';
import type { ModerationAction, ModerationQueueItem } from '@/contracts';
import { useRouter } from '@/i18n/navigation';
import { formatDate, formatPrice } from '@/lib/format';

/** Admin review queue: per-row and bulk approve/reject/remove for pending listings (US3). */
export function ModerationQueue({ items }: Readonly<{ items: ModerationQueueItem[] }>) {
  const t = useTranslations('moderation');
  const locale = useLocale();
  const router = useRouter();
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [message, setMessage] = useState<string | null>(null);
  const [pending, startAction] = useTransition();

  const allSelected = items.length > 0 && selected.size === items.length;

  const toggle = (id: string) =>
    setSelected((current) => {
      const next = new Set(current);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });

  const toggleAll = () =>
    setSelected(allSelected ? new Set() : new Set(items.map((item) => item.id)));

  const runSingle = (id: string, action: ModerationAction) =>
    startAction(async () => {
      setMessage(null);
      const result = await moderateListing(id, action);
      if (result.ok) {
        router.refresh();
      } else {
        setMessage(t('actionError'));
      }
    });

  const runBulk = (action: ModerationAction) =>
    startAction(async () => {
      setMessage(null);
      const result = await bulkModerate([...selected], action);
      if (result.ok) {
        setMessage(t('bulkResult', { succeeded: result.result.succeeded, skipped: result.result.skipped }));
        setSelected(new Set());
        router.refresh();
      } else {
        setMessage(t('actionError'));
      }
    });

  if (items.length === 0) {
    return <p className="text-sm text-slate-500">{t('queueEmpty')}</p>;
  }

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between gap-4 rounded-lg border border-slate-200 bg-slate-50 p-3">
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input type="checkbox" checked={allSelected} onChange={toggleAll} className="h-4 w-4 rounded border-slate-300" />
          {t('selectAll')}
        </label>
        {selected.size > 0 ? (
          <div className="flex items-center gap-2">
            <span className="text-sm text-slate-500">{t('selectedCount', { count: selected.size })}</span>
            <Button size="sm" onClick={() => runBulk('APPROVED')} disabled={pending}>
              {t('approve')}
            </Button>
            <Button size="sm" variant="outline" onClick={() => runBulk('REJECTED')} disabled={pending}>
              {t('reject')}
            </Button>
            <Button size="sm" variant="ghost" className="text-red-600" onClick={() => runBulk('REMOVED')} disabled={pending}>
              {t('remove')}
            </Button>
          </div>
        ) : null}
      </div>

      {message ? <p className="text-sm text-slate-600">{message}</p> : null}

      {items.map((item) => (
        <div key={item.id} className="flex flex-col gap-3 rounded-lg border border-slate-200 bg-white p-4 sm:flex-row sm:items-center">
          <input
            type="checkbox"
            checked={selected.has(item.id)}
            onChange={() => toggle(item.id)}
            aria-label={t('selectListing', { title: item.title })}
            className="h-4 w-4 rounded border-slate-300"
          />
          <div className="min-w-0 flex-1 space-y-1">
            <p className="truncate font-medium text-slate-900">{item.title}</p>
            <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-slate-500">
              <span>{item.sellerName}</span>
              <span className="font-medium text-slate-700">
                {formatPrice(item.priceAmount, item.priceContact, locale, t('priceContact'))}
              </span>
              <span>{item.locationProvince}</span>
              <span>{formatDate(item.createdAt, locale)}</span>
            </div>
          </div>
          <div className="flex shrink-0 items-center gap-2">
            <Button size="sm" onClick={() => runSingle(item.id, 'APPROVED')} disabled={pending}>
              {pending ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : <Check className="h-4 w-4" aria-hidden />}
              {t('approve')}
            </Button>
            <Button size="sm" variant="outline" onClick={() => runSingle(item.id, 'REJECTED')} disabled={pending}>
              <X className="h-4 w-4" aria-hidden />
              {t('reject')}
            </Button>
            <Button size="sm" variant="ghost" className="text-red-600" onClick={() => runSingle(item.id, 'REMOVED')} disabled={pending} aria-label={t('remove')}>
              <Trash2 className="h-4 w-4" aria-hidden />
            </Button>
          </div>
        </div>
      ))}
    </div>
  );
}

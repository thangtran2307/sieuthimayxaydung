'use client';

import { Check, Loader2, Trash2 } from 'lucide-react';
import { useLocale, useTranslations } from 'next-intl';
import { useState, useTransition } from 'react';
import { resolveReport } from '@/actions/moderation';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import type { ModerationReport, ReportResolution } from '@/contracts';
import { Link, useRouter } from '@/i18n/navigation';
import { formatDate } from '@/lib/format';

/** Admin report queue: resolve each open report by removing the listing or clearing the report (US3). */
export function ReportList({ reports }: Readonly<{ reports: ModerationReport[] }>) {
  const t = useTranslations('moderation');
  const locale = useLocale();
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [pending, startAction] = useTransition();

  const resolve = (id: string, resolution: ReportResolution) => {
    if (resolution === 'REMOVE' && !globalThis.confirm(t('confirmRemove'))) {
      return;
    }
    startAction(async () => {
      setError(null);
      const result = await resolveReport(id, resolution);
      if (result.ok) {
        router.refresh();
      } else {
        setError(t('actionError'));
      }
    });
  };

  if (reports.length === 0) {
    return <p className="text-sm text-slate-500">{t('reportsEmpty')}</p>;
  }

  return (
    <div className="space-y-3">
      {error ? <p className="text-sm text-red-600">{error}</p> : null}
      {reports.map((report) => (
        <div key={report.id} className="flex flex-col gap-3 rounded-lg border border-slate-200 bg-white p-4 sm:flex-row sm:items-start">
          <div className="min-w-0 flex-1 space-y-1.5">
            <div className="flex items-center gap-2">
              <Badge variant="neutral">{t(`reasons.${report.reason}`)}</Badge>
              <Link href={`/listing/${report.listingSlug}`} className="truncate font-medium text-brand hover:text-accent">
                {report.listingTitle}
              </Link>
            </div>
            {report.details ? <p className="text-sm text-slate-700">{report.details}</p> : null}
            <div className="flex flex-wrap items-center gap-x-4 text-xs text-slate-400">
              {report.reporterContact ? <span>{report.reporterContact}</span> : null}
              <span>{formatDate(report.createdAt, locale)}</span>
            </div>
          </div>
          <div className="flex shrink-0 items-center gap-2">
            <Button size="sm" variant="ghost" className="text-red-600" onClick={() => resolve(report.id, 'REMOVE')} disabled={pending}>
              {pending ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : <Trash2 className="h-4 w-4" aria-hidden />}
              {t('removeListing')}
            </Button>
            <Button size="sm" variant="outline" onClick={() => resolve(report.id, 'CLEAR')} disabled={pending}>
              <Check className="h-4 w-4" aria-hidden />
              {t('clear')}
            </Button>
          </div>
        </div>
      ))}
    </div>
  );
}

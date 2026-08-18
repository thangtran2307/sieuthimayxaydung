'use client';

import { Check, Flag, Loader2 } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useState, useTransition, type SyntheticEvent } from 'react';
import { Button } from '@/components/ui/button';
import { Select } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import type { ReportReason } from '@/contracts';
import { createReport } from '@/actions/report';
import { formString } from '@/lib/utils';

const REASONS: ReportReason[] = ['FRAUD', 'INCORRECT_INFO', 'SPAM', 'PROHIBITED', 'OTHER'];

/** Lets any visitor flag a listing for admin review (FR-009). */
export function ReportListing({ listingId }: Readonly<{ listingId: string }>) {
  const t = useTranslations('report');
  const [open, setOpen] = useState(false);
  const [done, setDone] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [submitting, startSubmit] = useTransition();

  const submit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    startSubmit(async () => {
      setError(null);
      try {
        await createReport(listingId, {
          reason: form.get('reason') as ReportReason,
          details: formString(form, 'details') || undefined,
        });
        setDone(true);
      } catch {
        setError(t('error'));
      }
    });
  };

  if (done) {
    return (
      <p className="flex items-center gap-1.5 text-sm text-emerald-700">
        <Check className="h-4 w-4" aria-hidden />
        {t('submitted')}
      </p>
    );
  }

  if (!open) {
    return (
      <button
        type="button"
        onClick={() => setOpen(true)}
        className="flex items-center gap-1.5 text-sm text-slate-500 transition-colors hover:text-red-600"
      >
        <Flag className="h-4 w-4" aria-hidden />
        {t('report')}
      </button>
    );
  }

  return (
    <form
      onSubmit={(event) => void submit(event)}
      className="space-y-2 rounded-lg border border-slate-200 bg-white p-4"
    >
      <p className="font-medium text-slate-800">{t('reportListing')}</p>
      <Select name="reason" defaultValue="FRAUD" aria-label={t('reason')}>
        {REASONS.map((reason) => (
          <option key={reason} value={reason}>
            {t(`reasons.${reason}`)}
          </option>
        ))}
      </Select>
      <Textarea name="details" placeholder={t('details')} aria-label={t('details')} />
      <div className="flex gap-2">
        <Button type="submit" size="sm" disabled={submitting}>
          {submitting ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : null}
          {t('submit')}
        </Button>
        <Button type="button" size="sm" variant="ghost" onClick={() => setOpen(false)}>
          {t('cancel')}
        </Button>
      </div>
      {error ? <p className="text-sm text-red-600">{error}</p> : null}
    </form>
  );
}

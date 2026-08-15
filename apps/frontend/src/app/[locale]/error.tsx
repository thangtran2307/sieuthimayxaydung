'use client';

import { AlertTriangle } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useEffect } from 'react';
import { EmptyState } from '@/components/empty-state';
import { Button } from '@/components/ui/button';
import { logger } from '@/lib/logger';

/** Segment error boundary — shown when a page in this locale fails to render. */
export default function LocaleError({
  error,
  reset,
}: Readonly<{ error: Error & { digest?: string }; reset: () => void }>) {
  const t = useTranslations('error');

  useEffect(() => {
    // Server errors are already logged via instrumentation's onRequestError; log the digest here
    // to correlate, plus any client-side render error.
    logger.error('Render error boundary', error, { digest: error.digest });
  }, [error]);

  return (
    <EmptyState
      icon={<AlertTriangle className="h-10 w-10" aria-hidden />}
      title={t('title')}
      description={t('description')}
      action={<Button onClick={reset}>{t('retry')}</Button>}
    />
  );
}

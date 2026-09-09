import { getTranslations } from 'next-intl/server';
import { ModerationNav } from '@/features/moderation/moderation-nav';
import { ReportList } from '@/features/moderation/report-list';
import { getReports } from '@/lib/api/moderation';
import { requireAdmin } from '@/lib/auth/admin';
import { logger } from '@/lib/logger';

// Admin-only + live reports — render per request.
export const dynamic = 'force-dynamic';

/** Admin queue of open reports (US3). */
export default async function ModerationReportsPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  await requireAdmin(locale);

  const t = await getTranslations('moderation');
  const reports = await getReports().catch((error) => {
    logger.error('Moderation: failed to load reports', error);
    return [];
  });

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <ModerationNav active="reports" />
      <ReportList reports={reports} />
    </div>
  );
}

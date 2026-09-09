import { getTranslations } from 'next-intl/server';
import { ModerationNav } from '@/features/moderation/moderation-nav';
import { ModerationQueue } from '@/features/moderation/moderation-queue';
import { getModerationQueue } from '@/lib/api/moderation';
import { requireAdmin } from '@/lib/auth/admin';
import { logger } from '@/lib/logger';

// Admin-only + live queue — render per request.
export const dynamic = 'force-dynamic';

/** Admin moderation queue of pending listings (US3). */
export default async function ModerationPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  await requireAdmin(locale);

  const t = await getTranslations('moderation');
  const items = await getModerationQueue().catch((error) => {
    logger.error('Moderation: failed to load the queue', error);
    return [];
  });

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('title')}</h1>
      <ModerationNav active="queue" />
      <ModerationQueue items={items} />
    </div>
  );
}

import { getTranslations } from 'next-intl/server';
import { Link } from '@/i18n/navigation';

/** Tab links between the moderation queue and the reports view. */
export async function ModerationNav({ active }: Readonly<{ active: 'queue' | 'reports' }>) {
  const t = await getTranslations('moderation');
  const linkClass = (tab: 'queue' | 'reports') =>
    `text-sm font-medium ${active === tab ? 'text-brand' : 'text-slate-500 hover:text-brand'}`;

  return (
    <nav className="flex items-center gap-4 border-b border-slate-200 pb-3">
      <Link href="/moderation" className={linkClass('queue')}>
        {t('queueTab')}
      </Link>
      <Link href="/moderation/reports" className={linkClass('reports')}>
        {t('reportsTab')}
      </Link>
    </nav>
  );
}

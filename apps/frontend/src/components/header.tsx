import { useTranslations } from 'next-intl';
import { Link } from '@/i18n/navigation';
import { LanguageSwitcher } from './language-switcher';

/** Site header with brand, primary actions, and the language toggle. */
export function Header() {
  const t = useTranslations('header');
  const tApp = useTranslations('app');

  return (
    <header className="border-b border-slate-200 bg-white">
      <div className="container flex h-16 items-center justify-between gap-4">
        <Link href="/" className="font-display text-xl font-bold text-brand">
          {tApp('name')}
        </Link>
        <nav className="flex items-center gap-4">
          <Link href="/post" className="text-sm font-medium text-brand hover:text-accent">
            {t('postListing')}
          </Link>
          <Link href="/login" className="text-sm font-medium text-brand hover:text-accent">
            {t('login')}
          </Link>
          <LanguageSwitcher />
        </nav>
      </div>
    </header>
  );
}

import { getTranslations } from 'next-intl/server';
import { UserMenu } from '@/features/auth/user-menu';
import { Link } from '@/i18n/navigation';
import { getCurrentUser } from '@/lib/auth/session';
import { LanguageSwitcher } from './language-switcher';

/** Site header with brand, session-aware actions, and the language toggle. */
export async function Header() {
  const t = await getTranslations('header');
  const tApp = await getTranslations('app');
  const user = await getCurrentUser();

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
          {user ? (
            <UserMenu name={user.displayName} />
          ) : (
            <>
              <Link href="/login" className="text-sm font-medium text-brand hover:text-accent">
                {t('login')}
              </Link>
              <Link href="/register" className="text-sm font-medium text-brand hover:text-accent">
                {t('register')}
              </Link>
            </>
          )}
          <LanguageSwitcher />
        </nav>
      </div>
    </header>
  );
}

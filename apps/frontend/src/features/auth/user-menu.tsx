'use client';

import { Loader2, LogOut } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useTransition } from 'react';
import { logout } from '@/actions/auth';
import { useRouter } from '@/i18n/navigation';

/** Signed-in header control: shows the account name and a logout button. */
export function UserMenu({ name }: Readonly<{ name: string }>) {
  const t = useTranslations('header');
  const router = useRouter();
  const [pending, startLogout] = useTransition();

  const onLogout = () => {
    startLogout(async () => {
      await logout();
      router.refresh();
      router.push('/');
    });
  };

  return (
    <div className="flex items-center gap-3">
      <span className="max-w-[12rem] truncate text-sm text-slate-600" title={name}>
        {name}
      </span>
      <button
        type="button"
        onClick={onLogout}
        disabled={pending}
        className="inline-flex items-center gap-1.5 text-sm font-medium text-brand hover:text-accent disabled:opacity-50"
      >
        {pending ? (
          <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
        ) : (
          <LogOut className="h-4 w-4" aria-hidden />
        )}
        {t('logout')}
      </button>
    </div>
  );
}

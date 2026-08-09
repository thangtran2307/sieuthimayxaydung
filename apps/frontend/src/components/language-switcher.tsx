'use client';

import { useLocale } from 'next-intl';
import { useTransition } from 'react';
import { usePathname, useRouter } from '@/i18n/navigation';
import { routing } from '@/i18n/routing';
import { cn } from '@/lib/utils';

/** Toggles the active locale; next-intl persists the choice via cookie (FR-010). */
export function LanguageSwitcher() {
  const locale = useLocale();
  const pathname = usePathname();
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  const switchTo = (next: string) => {
    if (next === locale) return;
    startTransition(() => {
      router.replace(pathname, { locale: next });
    });
  };

  return (
    <div className="flex items-center gap-1 text-sm" aria-label="Language">
      {routing.locales.map((loc) => (
        <button
          key={loc}
          type="button"
          disabled={isPending}
          onClick={() => switchTo(loc)}
          className={cn(
            'rounded px-2 py-1 uppercase transition-colors',
            loc === locale ? 'bg-brand text-brand-fg' : 'text-brand hover:bg-slate-100',
          )}
        >
          {loc}
        </button>
      ))}
    </div>
  );
}

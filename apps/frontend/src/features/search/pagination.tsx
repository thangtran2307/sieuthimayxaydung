'use client';

import { ChevronLeft, ChevronRight } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useSearchParams } from 'next/navigation';
import { buttonVariants } from '@/components/ui/button';
import { Link, usePathname } from '@/i18n/navigation';
import { cn } from '@/lib/utils';

/** Prev/next pagination that preserves the current filters in the URL. */
export function Pagination({ page, totalPages }: Readonly<{ page: number; totalPages: number }>) {
  const t = useTranslations('search');
  const pathname = usePathname();
  const searchParams = useSearchParams();

  if (totalPages <= 1) {
    return null;
  }

  const hrefForPage = (target: number) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set('page', String(target));
    return `${pathname}?${params.toString()}`;
  };

  const disabled = 'pointer-events-none opacity-40';

  return (
    <nav className="flex items-center justify-center gap-3 pt-2" aria-label={t('pagination')}>
      <Link
        href={hrefForPage(page - 1)}
        className={cn(buttonVariants({ variant: 'outline', size: 'sm' }), page <= 1 && disabled)}
        aria-disabled={page <= 1}
      >
        <ChevronLeft className="h-4 w-4" aria-hidden />
        {t('previous')}
      </Link>
      <span className="text-sm text-slate-600">{t('pageOf', { page, totalPages })}</span>
      <Link
        href={hrefForPage(page + 1)}
        className={cn(
          buttonVariants({ variant: 'outline', size: 'sm' }),
          page >= totalPages && disabled,
        )}
        aria-disabled={page >= totalPages}
      >
        {t('next')}
        <ChevronRight className="h-4 w-4" aria-hidden />
      </Link>
    </nav>
  );
}

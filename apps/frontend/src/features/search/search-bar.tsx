'use client';

import { Search } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useState, type FormEvent } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { useRouter } from '@/i18n/navigation';

/** Keyword search box (hero + reusable) that navigates to the search results page. */
export function SearchBar({ initialQuery = '' }: Readonly<{ initialQuery?: string }>) {
  const t = useTranslations('home');
  const router = useRouter();
  const [query, setQuery] = useState(initialQuery);

  const onSubmit = (event: FormEvent) => {
    event.preventDefault();
    const trimmed = query.trim();
    router.push(trimmed ? `/search?q=${encodeURIComponent(trimmed)}` : '/search');
  };

  return (
    <form onSubmit={onSubmit} className="flex w-full gap-2">
      <Input
        value={query}
        onChange={(event) => setQuery(event.target.value)}
        placeholder={t('searchPlaceholder')}
        aria-label={t('searchPlaceholder')}
        className="h-12 flex-1 text-base"
      />
      <Button type="submit" size="lg" variant="accent">
        <Search className="h-5 w-5" aria-hidden />
        <span className="hidden sm:inline">{t('searchButton')}</span>
      </Button>
    </form>
  );
}

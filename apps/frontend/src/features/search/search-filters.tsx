'use client';

import { useLocale, useTranslations } from 'next-intl';
import { type SyntheticEvent } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import type { Category, SearchParams } from '@/contracts';
import { useRouter } from '@/i18n/navigation';
import { formString } from '@/lib/utils';

const FIELDS = ['q', 'category', 'condition', 'province', 'priceMin', 'priceMax', 'sort'] as const;

/** Filter sidebar for the search page. Uncontrolled form → builds the query and navigates. */
export function SearchFilters({
  categories,
  params,
}: Readonly<{
  categories: Category[];
  params: SearchParams;
}>) {
  const t = useTranslations('search');
  const locale = useLocale();
  const router = useRouter();
  const topLevel = categories.filter((category) => category.parentId === null);

  const onSubmit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    const query = new URLSearchParams();
    for (const field of FIELDS) {
      const value = formString(form, field).trim();
      if (value) {
        query.set(field, value);
      }
    }
    const qs = query.toString();
    router.push(qs ? `/search?${qs}` : '/search');
  };

  return (
    <form onSubmit={onSubmit} className="space-y-4 rounded-lg border border-slate-200 bg-white p-4">
      <Field label={t('keyword')}>
        <Input name="q" defaultValue={params.q ?? ''} placeholder={t('keyword')} />
      </Field>

      <Field label={t('category')}>
        <Select name="category" defaultValue={params.category ?? ''}>
          <option value="">{t('allCategories')}</option>
          {topLevel.map((category) => (
            <option key={category.id} value={category.slug}>
              {locale === 'vi' ? category.labelVi : category.labelEn}
            </option>
          ))}
        </Select>
      </Field>

      <Field label={t('condition')}>
        <Select name="condition" defaultValue={params.condition ?? ''}>
          <option value="">{t('allConditions')}</option>
          <option value="NEW">{t('conditionNew')}</option>
          <option value="USED">{t('conditionUsed')}</option>
        </Select>
      </Field>

      <Field label={t('province')}>
        <Input name="province" defaultValue={params.province ?? ''} placeholder={t('province')} />
      </Field>

      <Field label={t('price')}>
        <div className="flex items-center gap-2">
          <Input
            name="priceMin"
            type="number"
            min={0}
            defaultValue={params.priceMin ?? ''}
            placeholder={t('priceMin')}
          />
          <span className="text-slate-400">–</span>
          <Input
            name="priceMax"
            type="number"
            min={0}
            defaultValue={params.priceMax ?? ''}
            placeholder={t('priceMax')}
          />
        </div>
      </Field>

      <Field label={t('sort')}>
        <Select name="sort" defaultValue={params.sort}>
          <option value="recent">{t('sortRecent')}</option>
          <option value="price_asc">{t('sortPriceAsc')}</option>
          <option value="price_desc">{t('sortPriceDesc')}</option>
          <option value="relevance">{t('sortRelevance')}</option>
        </Select>
      </Field>

      <div className="flex flex-col gap-2 pt-2">
        <Button type="submit" className="w-full">
          {t('apply')}
        </Button>
        <Button
          type="button"
          variant="ghost"
          className="w-full"
          onClick={() => router.push('/search')}
        >
          {t('clear')}
        </Button>
      </div>
    </form>
  );
}

function Field({ label, children }: Readonly<{ label: string; children: React.ReactNode }>) {
  return (
    <label className="block space-y-1">
      <span className="text-xs font-semibold uppercase tracking-wide text-slate-500">{label}</span>
      {children}
    </label>
  );
}

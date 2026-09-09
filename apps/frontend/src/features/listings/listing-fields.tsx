'use client';

import { useLocale, useTranslations } from 'next-intl';
import type { ReactNode } from 'react';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import type { Category, Condition, ListingSpecsInput } from '@/contracts';
import { formString } from '@/lib/utils';

const numberOrUndefined = (value: string) => (value.trim() ? Number(value) : undefined);
const stringOrUndefined = (value: string) => value.trim() || undefined;

/** The fields shared by create and edit — everything except photos, submit, and success handling. */
export interface ListingCoreInput {
  categoryId: string;
  subcategoryId: string | null;
  title: string;
  condition: Condition;
  priceContact: boolean;
  priceAmount: number | null;
  locationProvince: string;
  description: string;
  specs: ListingSpecsInput;
}

/** Values used to pre-fill the fields when editing (omitted for a fresh create). */
export interface ListingFieldsInitial {
  title: string;
  condition: Condition;
  priceAmount: number | null;
  locationProvince: string;
  description: string;
  specs: {
    year: number | null;
    brand: string | null;
    model: string | null;
    hours: number | null;
    origin: string | null;
    capacity: string | null;
  };
}

/** Reads the shared fields from the form, combining the DOM inputs with the controlled selects. */
export function readCoreListingInput(
  form: FormData,
  controlled: Readonly<{ categoryId: string; subcategoryId: string; priceContact: boolean }>,
): ListingCoreInput {
  return {
    categoryId: controlled.categoryId,
    subcategoryId: controlled.subcategoryId || null,
    title: formString(form, 'title'),
    condition: formString(form, 'condition') === 'NEW' ? 'NEW' : 'USED',
    priceContact: controlled.priceContact,
    priceAmount: controlled.priceContact
      ? null
      : numberOrUndefined(formString(form, 'priceAmount')) ?? null,
    locationProvince: formString(form, 'locationProvince'),
    description: formString(form, 'description'),
    specs: {
      year: numberOrUndefined(formString(form, 'year')),
      brand: stringOrUndefined(formString(form, 'brand')),
      model: stringOrUndefined(formString(form, 'model')),
      hours: numberOrUndefined(formString(form, 'hours')),
      origin: stringOrUndefined(formString(form, 'origin')),
      capacity: stringOrUndefined(formString(form, 'capacity')),
    },
  };
}

interface ListingCoreFieldsProps {
  categories: Category[];
  initial?: ListingFieldsInitial;
  categoryId: string;
  onCategoryChange: (id: string) => void;
  subcategoryId: string;
  onSubcategoryChange: (id: string) => void;
  priceContact: boolean;
  onPriceContactChange: (value: boolean) => void;
}

/**
 * The listing fields common to create and edit (controlled selects for category/subcategory and the
 * price-contact toggle live in the parent; text inputs are uncontrolled and read via
 * {@link readCoreListingInput}). Prefilled from {@link initial} when editing.
 */
export function ListingCoreFields({
  categories,
  initial,
  categoryId,
  onCategoryChange,
  subcategoryId,
  onSubcategoryChange,
  priceContact,
  onPriceContactChange,
}: Readonly<ListingCoreFieldsProps>) {
  const t = useTranslations('sell');
  const locale = useLocale();
  const label = (category: Category) => (locale === 'vi' ? category.labelVi : category.labelEn);

  const topLevel = categories.filter((category) => category.parentId === null);
  const subcategories = categories.filter((category) => category.parentId === categoryId);
  const specs = initial?.specs;

  return (
    <>
      <Field label={t('listingTitle')} htmlFor="title">
        <Input id="title" name="title" required maxLength={200} defaultValue={initial?.title ?? ''} />
      </Field>

      <div className="grid gap-4 sm:grid-cols-2">
        <Field label={t('category')} htmlFor="categoryId">
          <Select
            id="categoryId"
            name="categoryId"
            required
            value={categoryId}
            onChange={(event) => {
              onCategoryChange(event.target.value);
              onSubcategoryChange('');
            }}
          >
            <option value="" disabled>
              {t('selectCategory')}
            </option>
            {topLevel.map((category) => (
              <option key={category.id} value={category.id}>
                {label(category)}
              </option>
            ))}
          </Select>
        </Field>

        <Field label={t('subcategory')} htmlFor="subcategoryId">
          <Select
            id="subcategoryId"
            name="subcategoryId"
            value={subcategoryId}
            disabled={subcategories.length === 0}
            onChange={(event) => onSubcategoryChange(event.target.value)}
          >
            <option value="">{t('noSubcategory')}</option>
            {subcategories.map((category) => (
              <option key={category.id} value={category.id}>
                {label(category)}
              </option>
            ))}
          </Select>
        </Field>

        <Field label={t('condition')} htmlFor="condition">
          <Select id="condition" name="condition" defaultValue={initial?.condition ?? 'USED'} required>
            <option value="USED">{t('conditionUsed')}</option>
            <option value="NEW">{t('conditionNew')}</option>
          </Select>
        </Field>

        <Field label={t('location')} htmlFor="locationProvince">
          <Input
            id="locationProvince"
            name="locationProvince"
            required
            maxLength={120}
            defaultValue={initial?.locationProvince ?? ''}
          />
        </Field>
      </div>

      <div className="space-y-2">
        <Field label={t('price')} htmlFor="priceAmount">
          <Input
            id="priceAmount"
            name="priceAmount"
            type="number"
            min={1}
            required={!priceContact}
            disabled={priceContact}
            defaultValue={initial?.priceAmount ?? ''}
          />
        </Field>
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input
            type="checkbox"
            checked={priceContact}
            onChange={(event) => onPriceContactChange(event.target.checked)}
            className="h-4 w-4 rounded border-slate-300"
          />
          {t('priceContact')}
        </label>
      </div>

      <Field label={t('description')} htmlFor="description">
        <Textarea
          id="description"
          name="description"
          required
          maxLength={5000}
          className="min-h-32"
          defaultValue={initial?.description ?? ''}
        />
      </Field>

      <fieldset className="space-y-4 rounded-lg border border-slate-200 p-4">
        <legend className="px-1 text-sm font-semibold text-slate-700">{t('specsHeading')}</legend>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label={t('year')} htmlFor="year">
            <Input id="year" name="year" type="number" min={1900} max={2100} defaultValue={specs?.year ?? ''} />
          </Field>
          <Field label={t('brand')} htmlFor="brand">
            <Input id="brand" name="brand" maxLength={120} defaultValue={specs?.brand ?? ''} />
          </Field>
          <Field label={t('model')} htmlFor="model">
            <Input id="model" name="model" maxLength={120} defaultValue={specs?.model ?? ''} />
          </Field>
          <Field label={t('hours')} htmlFor="hours">
            <Input id="hours" name="hours" type="number" min={0} defaultValue={specs?.hours ?? ''} />
          </Field>
          <Field label={t('origin')} htmlFor="origin">
            <Input id="origin" name="origin" maxLength={120} defaultValue={specs?.origin ?? ''} />
          </Field>
          <Field label={t('capacity')} htmlFor="capacity">
            <Input id="capacity" name="capacity" maxLength={120} defaultValue={specs?.capacity ?? ''} />
          </Field>
        </div>
      </fieldset>
    </>
  );
}

/** Label + control wrapper for a form field. */
function Field({
  label,
  htmlFor,
  children,
}: Readonly<{ label: string; htmlFor: string; children: ReactNode }>) {
  return (
    <div className="space-y-1.5">
      <label htmlFor={htmlFor} className="text-sm font-medium text-slate-700">
        {label}
      </label>
      {children}
    </div>
  );
}

'use client';

import { Check, Loader2, Plus, X } from 'lucide-react';
import { useLocale, useTranslations } from 'next-intl';
import { useState, useTransition, type ReactNode, type SyntheticEvent } from 'react';
import { createListing } from '@/actions/listing';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { createListingSchema, type Category, type CreateListingInput } from '@/contracts';
import { Link } from '@/i18n/navigation';
import { formString } from '@/lib/utils';
import { sellErrorKey } from './sell-errors';

const numberOrUndefined = (value: string) => (value.trim() ? Number(value) : undefined);
const stringOrUndefined = (value: string) => value.trim() || undefined;

/** Guided listing-creation form (FR-014). Photos are provided as URLs (upload flow deferred). */
export function CreateListingForm({ categories }: Readonly<{ categories: Category[] }>) {
  const t = useTranslations('sell');
  const locale = useLocale();
  const label = (category: Category) => (locale === 'vi' ? category.labelVi : category.labelEn);
  const [categoryId, setCategoryId] = useState('');
  const [subcategoryId, setSubcategoryId] = useState('');
  const [priceContact, setPriceContact] = useState(false);
  const [photos, setPhotos] = useState<string[]>(['']);
  const [error, setError] = useState<string | null>(null);
  const [created, setCreated] = useState<{ slug: string; status: string } | null>(null);
  const [pending, startSubmit] = useTransition();

  const topLevel = categories.filter((category) => category.parentId === null);
  const subcategories = categories.filter((category) => category.parentId === categoryId);

  const updatePhoto = (index: number, value: string) =>
    setPhotos((current) => current.map((url, i) => (i === index ? value : url)));
  const addPhoto = () => setPhotos((current) => [...current, '']);
  const removePhoto = (index: number) =>
    setPhotos((current) => current.filter((_, i) => i !== index));

  const resetForm = () => {
    setCreated(null);
    setCategoryId('');
    setSubcategoryId('');
    setPriceContact(false);
    setPhotos(['']);
    setError(null);
  };

  const onSubmit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;
    if (!form.checkValidity()) {
      form.reportValidity();
      return;
    }

    const data = new FormData(form);
    const input: CreateListingInput = {
      categoryId,
      subcategoryId: subcategoryId || null,
      title: formString(data, 'title'),
      condition: formString(data, 'condition') === 'NEW' ? 'NEW' : 'USED',
      priceContact,
      priceAmount: priceContact ? null : numberOrUndefined(formString(data, 'priceAmount')) ?? null,
      locationProvince: formString(data, 'locationProvince'),
      description: formString(data, 'description'),
      specs: {
        year: numberOrUndefined(formString(data, 'year')),
        brand: stringOrUndefined(formString(data, 'brand')),
        model: stringOrUndefined(formString(data, 'model')),
        hours: numberOrUndefined(formString(data, 'hours')),
        origin: stringOrUndefined(formString(data, 'origin')),
        capacity: stringOrUndefined(formString(data, 'capacity')),
      },
      photos: photos
        .map((url) => url.trim())
        .filter(Boolean)
        .map((url, index) => ({ url, sortOrder: index, width: null, height: null })),
    };

    const check = createListingSchema.safeParse(input);
    if (!check.success) {
      setError(t('errors.validation'));
      return;
    }

    startSubmit(async () => {
      setError(null);
      const result = await createListing(check.data);
      if (result.ok) {
        setCreated({ slug: result.slug, status: result.status });
        return;
      }
      setError(t(sellErrorKey(result.code)));
    });
  };

  if (created) {
    return (
      <div className="space-y-4 rounded-lg border border-emerald-200 bg-emerald-50 p-6">
        <div className="flex items-start gap-2 text-emerald-800">
          <Check className="mt-0.5 h-5 w-5 shrink-0" aria-hidden />
          <div>
            <p className="font-display text-lg font-semibold">{t('success.title')}</p>
            <p className="text-sm text-emerald-700">{t('success.hint')}</p>
          </div>
        </div>
        <div className="flex gap-3">
          <Button type="button" onClick={resetForm}>
            {t('success.postAnother')}
          </Button>
          <Link href="/" className="inline-flex h-10 items-center px-4 text-sm font-medium text-brand hover:text-accent">
            {t('success.home')}
          </Link>
        </div>
      </div>
    );
  }

  return (
    <form onSubmit={onSubmit} className="space-y-5" noValidate>
      <Field label={t('listingTitle')} htmlFor="title">
        <Input id="title" name="title" required maxLength={200} />
      </Field>

      <div className="grid gap-4 sm:grid-cols-2">
        <Field label={t('category')} htmlFor="categoryId">
          <Select
            id="categoryId"
            name="categoryId"
            required
            value={categoryId}
            onChange={(event) => {
              setCategoryId(event.target.value);
              setSubcategoryId('');
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
            onChange={(event) => setSubcategoryId(event.target.value)}
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
          <Select id="condition" name="condition" defaultValue="USED" required>
            <option value="USED">{t('conditionUsed')}</option>
            <option value="NEW">{t('conditionNew')}</option>
          </Select>
        </Field>

        <Field label={t('location')} htmlFor="locationProvince">
          <Input id="locationProvince" name="locationProvince" required maxLength={120} />
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
          />
        </Field>
        <label className="flex items-center gap-2 text-sm text-slate-700">
          <input
            type="checkbox"
            checked={priceContact}
            onChange={(event) => setPriceContact(event.target.checked)}
            className="h-4 w-4 rounded border-slate-300"
          />
          {t('priceContact')}
        </label>
      </div>

      <Field label={t('description')} htmlFor="description">
        <Textarea id="description" name="description" required maxLength={5000} className="min-h-32" />
      </Field>

      <fieldset className="space-y-4 rounded-lg border border-slate-200 p-4">
        <legend className="px-1 text-sm font-semibold text-slate-700">{t('specsHeading')}</legend>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label={t('year')} htmlFor="year">
            <Input id="year" name="year" type="number" min={1900} max={2100} />
          </Field>
          <Field label={t('brand')} htmlFor="brand">
            <Input id="brand" name="brand" maxLength={120} />
          </Field>
          <Field label={t('model')} htmlFor="model">
            <Input id="model" name="model" maxLength={120} />
          </Field>
          <Field label={t('hours')} htmlFor="hours">
            <Input id="hours" name="hours" type="number" min={0} />
          </Field>
          <Field label={t('origin')} htmlFor="origin">
            <Input id="origin" name="origin" maxLength={120} />
          </Field>
          <Field label={t('capacity')} htmlFor="capacity">
            <Input id="capacity" name="capacity" maxLength={120} />
          </Field>
        </div>
      </fieldset>

      <div className="space-y-2">
        <p className="text-sm font-medium text-slate-700">{t('photos')}</p>
        {photos.map((url, index) => (
          <div key={index} className="flex gap-2">
            <Input
              name={`photo-${index}`}
              type="url"
              inputMode="url"
              placeholder={t('photoUrl')}
              value={url}
              onChange={(event) => updatePhoto(index, event.target.value)}
              required={index === 0}
            />
            {photos.length > 1 ? (
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={() => removePhoto(index)}
                aria-label={t('removePhoto')}
              >
                <X className="h-4 w-4" aria-hidden />
              </Button>
            ) : null}
          </div>
        ))}
        <Button type="button" variant="outline" size="sm" onClick={addPhoto}>
          <Plus className="h-4 w-4" aria-hidden />
          {t('addPhoto')}
        </Button>
      </div>

      {error ? <p className="text-sm text-red-600">{error}</p> : null}

      <Button type="submit" disabled={pending}>
        {pending ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : null}
        {t('submit')}
      </Button>
    </form>
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

'use client';

import { Check, Loader2, Plus, X } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useState, useTransition, type SyntheticEvent } from 'react';
import { createListing } from '@/actions/listing';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { createListingSchema, type Category, type CreateListingInput } from '@/contracts';
import { Link } from '@/i18n/navigation';
import { ListingCoreFields, readCoreListingInput } from './listing-fields';
import { sellErrorKey } from './sell-errors';

/** Guided listing-creation form (FR-014). Photos are provided as URLs (upload flow deferred). */
export function CreateListingForm({ categories }: Readonly<{ categories: Category[] }>) {
  const t = useTranslations('sell');
  const [categoryId, setCategoryId] = useState('');
  const [subcategoryId, setSubcategoryId] = useState('');
  const [priceContact, setPriceContact] = useState(false);
  const [photos, setPhotos] = useState<string[]>(['']);
  const [error, setError] = useState<string | null>(null);
  const [created, setCreated] = useState<{ slug: string; status: string } | null>(null);
  const [pending, startSubmit] = useTransition();

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
      ...readCoreListingInput(data, { categoryId, subcategoryId, priceContact }),
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
      <ListingCoreFields
        categories={categories}
        categoryId={categoryId}
        onCategoryChange={setCategoryId}
        subcategoryId={subcategoryId}
        onSubcategoryChange={setSubcategoryId}
        priceContact={priceContact}
        onPriceContactChange={setPriceContact}
      />

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

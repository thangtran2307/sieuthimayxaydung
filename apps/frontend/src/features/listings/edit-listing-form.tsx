'use client';

import { Loader2 } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useState, useTransition, type SyntheticEvent } from 'react';
import { updateListing } from '@/actions/listing';
import { Button } from '@/components/ui/button';
import {
  updateListingSchema,
  type Category,
  type SellerListingDetail,
  type UpdateListingInput,
} from '@/contracts';
import { useRouter } from '@/i18n/navigation';
import { ListingCoreFields, readCoreListingInput } from './listing-fields';
import { sellErrorKey } from './sell-errors';

/** Edit form for an existing listing (FR-017). Prefilled; photos are not editable in this pass. */
export function EditListingForm({
  listingId,
  listing,
  categories,
}: Readonly<{ listingId: string; listing: SellerListingDetail; categories: Category[] }>) {
  const t = useTranslations('sell');
  const router = useRouter();

  const [categoryId, setCategoryId] = useState(listing.categoryId);
  const [subcategoryId, setSubcategoryId] = useState(listing.subcategoryId ?? '');
  const [priceContact, setPriceContact] = useState(listing.priceContact);
  const [error, setError] = useState<string | null>(null);
  const [pending, startSubmit] = useTransition();

  const onSubmit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = event.currentTarget;
    if (!form.checkValidity()) {
      form.reportValidity();
      return;
    }

    const data = new FormData(form);
    const input: UpdateListingInput = readCoreListingInput(data, {
      categoryId,
      subcategoryId,
      priceContact,
    });

    const check = updateListingSchema.safeParse(input);
    if (!check.success) {
      setError(t('errors.validation'));
      return;
    }

    startSubmit(async () => {
      setError(null);
      const result = await updateListing(listingId, check.data);
      if (result.ok) {
        router.refresh();
        router.push('/dashboard');
        return;
      }
      setError(t(sellErrorKey(result.code)));
    });
  };

  return (
    <form onSubmit={onSubmit} className="space-y-5" noValidate>
      <ListingCoreFields
        categories={categories}
        initial={{
          title: listing.title,
          condition: listing.condition,
          priceAmount: listing.priceAmount,
          locationProvince: listing.locationProvince,
          description: listing.description,
          specs: listing.specs,
        }}
        categoryId={categoryId}
        onCategoryChange={setCategoryId}
        subcategoryId={subcategoryId}
        onSubcategoryChange={setSubcategoryId}
        priceContact={priceContact}
        onPriceContactChange={setPriceContact}
      />

      {error ? <p className="text-sm text-red-600">{error}</p> : null}

      <Button type="submit" disabled={pending}>
        {pending ? <Loader2 className="h-4 w-4 animate-spin" aria-hidden /> : null}
        {t('saveChanges')}
      </Button>
    </form>
  );
}

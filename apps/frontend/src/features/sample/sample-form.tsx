'use client';

import { updateSample } from '@/actions/sample';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Sample } from '@/contracts/sample';
import { formString } from '@/lib/utils';
import { SyntheticEvent, useTransition } from 'react';

export function SampleForm({
  sample,
  isCreate,
  locale,
}: Readonly<{ sample: Sample; isCreate: boolean; locale: string }>) {
  const [submitting, startSubmitting] = useTransition();

  const submitForm = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    startSubmitting(async () => {
      // Handle form submission logic here
      console.log('Form submitted', formString(form, 'fieldOne'), formString(form, 'fieldTwo'));
      try {
        await updateSample(sample.id, {
          fieldOne: formString(form, 'fieldOne'),
          fieldTwo: formString(form, 'fieldTwo'),
        }, locale);
      } catch (error) {
        console.error('Error submitting form:', error);
      }
    });
  };

  return (
    <form className="space-y-4" onSubmit={submitForm}>
      <div>
        <label htmlFor="fieldOne" className="block text-sm font-medium text-gray-700">
          Field One
        </label>
        <Input
          type="text"
          name="fieldOne"
          id="fieldOne"
          defaultValue={isCreate ? '' : sample.fieldOne}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
        />
      </div>
      <div>
        <label htmlFor="fieldTwo" className="block text-sm font-medium text-gray-700">
          Field Two
        </label>
        <Input
          type="text"
          name="fieldTwo"
          id="fieldTwo"
          defaultValue={isCreate ? '' : sample.fieldTwo}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
        />
      </div>
      <Button
        disabled={submitting}
        type="submit"
        className="inline-flex justify-center rounded-md border border-transparent bg-indigo-600 py-2 px-4 text-sm font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
      >
        {isCreate ? 'Create Sample' : 'Update Sample'}
      </Button>
    </form>
  );
}

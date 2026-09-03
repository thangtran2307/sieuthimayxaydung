'use server';

import { successEnvelope } from '@/contracts/common/envelope';
import { CreateUpdateSample, Sample, sampleSchema } from '@/contracts/sample';
import { ApiError, apiFetch } from '@/lib/api/client';
import { redirect } from 'next/dist/client/components/navigation';
import { revalidatePath } from 'next/dist/server/web/spec-extension/revalidate';

export async function updateSample(id: string, body: CreateUpdateSample, locale: string): Promise<Sample> {
  const { data } = await apiFetch(`/v1/samples/${id}`, successEnvelope(sampleSchema), {
    method: 'PUT',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(body),
  });

  revalidatePath(`/${locale}/sample`);
  redirect(`/${locale}/sample`);
}

import { type Sample, sampleSchema } from '@/contracts/sample';
import { ApiError, apiFetch } from './client';
import { successEnvelope } from '@/contracts';
import z from 'zod';

export async function getAllSamples(): Promise<Sample[]> {
  const { data } = await apiFetch('/v1/samples', successEnvelope(z.array(sampleSchema)));
  return data;
}

export async function getSampleById(id: string): Promise<Sample | null> {
  try {
    const { data } = await apiFetch(`/v1/samples/${id}`, successEnvelope(sampleSchema));
    return data;
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      return null;
    }
    throw error;
  }
}

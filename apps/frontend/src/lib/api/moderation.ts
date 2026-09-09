import 'server-only';
import { z } from 'zod';
import {
  moderationQueueItemSchema,
  moderationReportSchema,
  successEnvelope,
  type ModerationQueueItem,
  type ModerationReport,
} from '@/contracts';
import { serverApiFetch } from './server';

/** The pending-listing review queue (admin only). */
export async function getModerationQueue(): Promise<ModerationQueueItem[]> {
  const { data } = await serverApiFetch(
    '/v1/moderation/queue',
    successEnvelope(z.array(moderationQueueItemSchema)),
  );
  return data;
}

/** Open reports awaiting resolution (admin only). */
export async function getReports(): Promise<ModerationReport[]> {
  const { data } = await serverApiFetch(
    '/v1/moderation/reports',
    successEnvelope(z.array(moderationReportSchema)),
  );
  return data;
}

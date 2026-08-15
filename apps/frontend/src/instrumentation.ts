import type { Instrumentation } from 'next';
import { logger } from '@/lib/logger';

export function register(): void {
  // Reserved for one-time server startup wiring (tracing, metrics, …).
}

/**
 * Next.js server-error hook — invoked for every uncaught error during server rendering, route
 * handlers, and server actions. Logs the error with request + route context so failures are
 * diagnosable instead of silent.
 */
export const onRequestError: Instrumentation.onRequestError = (error, request, context) => {
  logger.error('Unhandled server error', error, {
    path: request.path,
    method: request.method,
    routerKind: context.routerKind,
    routePath: context.routePath,
    routeType: context.routeType,
    renderSource: context.renderSource,
  });
};

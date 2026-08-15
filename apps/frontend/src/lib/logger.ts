/**
 * Minimal structured logger for server-side code (Server Components, route handlers,
 * instrumentation). Human-readable in development, single-line JSON in production so logs are
 * greppable/aggregatable — mirroring the backend's Serilog setup. Zero dependencies.
 */

type Fields = Record<string, unknown>;
type Level = 'info' | 'warn' | 'error';

const isProduction = process.env.NODE_ENV === 'production';

/** Turns an unknown thrown value into loggable fields (incl. ApiError's code/status/details). */
function serializeError(error: unknown): Record<string, unknown> | undefined {
  if (error === null || error === undefined) {
    return undefined;
  }
  if (error instanceof Error) {
    const extra = error as { code?: unknown; status?: unknown; details?: unknown };
    return {
      name: error.name,
      message: error.message,
      stack: error.stack,
      ...(extra.code !== undefined ? { code: extra.code } : {}),
      ...(extra.status !== undefined ? { status: extra.status } : {}),
      ...(extra.details !== undefined ? { details: extra.details } : {}),
    };
  }
  return { message: String(error) };
}

function emit(level: Level, message: string, fields?: Fields): void {
  const consoleFn = level === 'error' ? console.error : level === 'warn' ? console.warn : console.info;

  if (isProduction) {
    consoleFn(JSON.stringify({ time: new Date().toISOString(), level, message, ...fields }));
    return;
  }

  const prefix = `[${new Date().toISOString()}] ${level.toUpperCase()} ${message}`;
  if (fields && Object.keys(fields).length > 0) {
    consoleFn(prefix, fields);
  } else {
    consoleFn(prefix);
  }
}

export const logger = {
  info: (message: string, fields?: Fields) => emit('info', message, fields),
  warn: (message: string, fields?: Fields) => emit('warn', message, fields),
  error: (message: string, error?: unknown, fields?: Fields) =>
    emit('error', message, { ...fields, error: serializeError(error) }),
};

import createMiddleware from 'next-intl/middleware';
import { routing } from './i18n/routing';

// Persists the chosen locale in a cookie and handles locale-prefixed routing (FR-010).
// Next 16 renamed the `middleware` file convention to `proxy`.
export default createMiddleware(routing);

export const config = {
  // Skip API routes, Next internals, and static files. `[.]` matches a literal dot without a
  // backslash escape (Next requires a statically-analyzable string literal here).
  matcher: ['/((?!api|_next|_vercel|.*[.].*).*)'],
};

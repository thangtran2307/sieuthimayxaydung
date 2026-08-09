import type { ReactNode } from 'react';

// Root layout is a passthrough; <html>/<body> live in the [locale] layout so the
// document language matches the active locale.
export default function RootLayout({ children }: { children: ReactNode }) {
  return children;
}

import type { HTMLAttributes } from 'react';
import { cn } from '@/lib/utils';

/** Pulsing placeholder block used in loading skeletons and Suspense fallbacks. */
export function Skeleton({ className, ...props }: Readonly<HTMLAttributes<HTMLDivElement>>) {
  return (
    <div
      className={cn('animate-pulse rounded-md bg-slate-200', className)}
      aria-hidden
      {...props}
    />
  );
}

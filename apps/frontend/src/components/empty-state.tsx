import type { ReactNode } from 'react';
import { cn } from '@/lib/utils';

/** Centered empty/placeholder block used for no-results and error states. */
export function EmptyState({
  icon,
  title,
  description,
  action,
  className,
}: Readonly<{
  icon?: ReactNode;
  title: string;
  description?: string;
  action?: ReactNode;
  className?: string;
}>) {
  return (
    <div
      className={cn(
        'flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed border-slate-300 bg-slate-50 px-6 py-16 text-center',
        className,
      )}
    >
      {icon ? <div className="text-slate-400">{icon}</div> : null}
      <h3 className="font-display text-lg font-semibold text-slate-800">{title}</h3>
      {description ? <p className="max-w-md text-sm text-slate-500">{description}</p> : null}
      {action}
    </div>
  );
}

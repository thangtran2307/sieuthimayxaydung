import { Cog, Construction, Mountain, Package, Truck, Wrench, type LucideIcon } from 'lucide-react';
import { useLocale } from 'next-intl';
import type { Category } from '@/contracts';
import { Link } from '@/i18n/navigation';

/** Maps the seeded category icon keys to Lucide icons (falls back to a generic box). */
const ICONS: Record<string, LucideIcon> = {
  truck: Truck,
  forklift: Truck,
  construction: Construction,
  cog: Cog,
  mountain: Mountain,
  wrench: Wrench,
};

/** Grid of top-level categories linking into filtered search. */
export function CategoryGrid({ categories }: Readonly<{ categories: Category[] }>) {
  const locale = useLocale();
  const topLevel = categories.filter((category) => category.parentId === null);

  return (
    <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-6">
      {topLevel.map((category) => {
        const Icon = ICONS[category.icon ?? ''] ?? Package;
        const label = locale === 'vi' ? category.labelVi : category.labelEn;
        return (
          <Link
            key={category.id}
            href={`/search?category=${category.slug}`}
            className="flex flex-col items-center gap-2 rounded-lg border border-slate-200 bg-white p-4 text-center transition-colors hover:border-brand hover:bg-slate-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand"
          >
            <Icon className="h-7 w-7 text-brand" aria-hidden />
            <span className="text-sm font-medium text-slate-700">{label}</span>
          </Link>
        );
      })}
    </div>
  );
}

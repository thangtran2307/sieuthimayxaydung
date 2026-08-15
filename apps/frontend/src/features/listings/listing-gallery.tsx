'use client';

import { Package } from 'lucide-react';
import Image from 'next/image';
import { useState } from 'react';
import type { ListingPhoto } from '@/contracts';
import { cn } from '@/lib/utils';

/** Listing image gallery: a main image plus selectable thumbnails. */
export function ListingGallery({
  photos,
  title,
}: Readonly<{ photos: ListingPhoto[]; title: string }>) {
  const [active, setActive] = useState(0);

  if (photos.length === 0) {
    return (
      <div className="flex aspect-[4/3] items-center justify-center rounded-lg bg-slate-100 text-slate-300">
        <Package className="h-16 w-16" aria-hidden />
      </div>
    );
  }

  const main = photos[Math.min(active, photos.length - 1)];

  return (
    <div className="space-y-3">
      <div className="relative aspect-[4/3] overflow-hidden rounded-lg bg-slate-100">
        <Image
          src={main.url}
          alt={title}
          fill
          sizes="(max-width: 1024px) 100vw, 60vw"
          className="object-cover"
          priority
        />
      </div>
      {photos.length > 1 ? (
        <div className="grid grid-cols-5 gap-2">
          {photos.map((photo, index) => (
            <button
              key={photo.url}
              type="button"
              onClick={() => setActive(index)}
              className={cn(
                'relative aspect-square overflow-hidden rounded-md bg-slate-100 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand',
                index === active ? 'ring-2 ring-brand' : 'opacity-80 hover:opacity-100',
              )}
              aria-label={`${title} ${index + 1}`}
            >
              <Image src={photo.url} alt="" fill sizes="120px" className="object-cover" />
            </button>
          ))}
        </div>
      ) : null}
    </div>
  );
}

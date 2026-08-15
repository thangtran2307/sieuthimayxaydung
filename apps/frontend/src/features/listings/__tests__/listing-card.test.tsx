import { screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { describe, expect, it, vi } from 'vitest';
import type { ListingSummary } from '@/contracts';
import { ListingCard } from '@/features/listings/listing-card';
import { renderWithIntl } from '@/test/test-utils';

vi.mock('next/image', () => ({
  default: (props: { src: string; alt: string }) => <img src={props.src} alt={props.alt} />,
}));

vi.mock('@/i18n/navigation', () => ({
  Link: ({ href, children }: { href: string; children: ReactNode }) => (
    <a href={String(href)}>{children}</a>
  ),
}));

const baseListing: ListingSummary = {
  id: '1',
  slug: 'excavator-abc',
  title: 'Komatsu PC200',
  priceAmount: 850_000_000,
  priceContact: false,
  currency: 'VND',
  condition: 'USED',
  locationProvince: 'Hà Nội',
  thumbnailUrl: 'https://example.com/a.jpg',
  boosted: true,
  categorySlug: 'excavator',
  createdAt: '2026-01-01T00:00:00Z',
};

describe('ListingCard', () => {
  it('renders title, location, and the featured badge, linking to the detail page', () => {
    renderWithIntl(<ListingCard listing={baseListing} />);
    expect(screen.getByText('Komatsu PC200')).toBeInTheDocument();
    expect(screen.getByText('Hà Nội')).toBeInTheDocument();
    expect(screen.getByText('Featured')).toBeInTheDocument();
    expect(screen.getByRole('link')).toHaveAttribute('href', '/listing/excavator-abc');
  });

  it('shows the contact-for-price label when the listing has no amount', () => {
    renderWithIntl(
      <ListingCard
        listing={{ ...baseListing, priceContact: true, priceAmount: null, boosted: false }}
      />,
    );
    expect(screen.getByText('Contact for price')).toBeInTheDocument();
    expect(screen.queryByText('Featured')).not.toBeInTheDocument();
  });
});

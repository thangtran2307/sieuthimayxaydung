import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { Badge } from '@/components/ui/badge';

describe('Badge', () => {
  it('renders its content with the boost styling', () => {
    render(<Badge variant="boost">Featured</Badge>);
    const badge = screen.getByText('Featured');
    expect(badge).toBeInTheDocument();
    expect(badge.className).toContain('bg-boost');
  });
});

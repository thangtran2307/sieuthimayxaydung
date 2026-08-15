'use client';

import { Check, Loader2, MessageSquare, Phone } from 'lucide-react';
import { useTranslations } from 'next-intl';
import { useState, type FormEvent, type ReactNode } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { createInquiry } from '@/lib/api/inquiries';

/** Contact panel: reveal the seller's phone or send a message. No account required (FR-008). */
export function ContactSeller({ listingId }: Readonly<{ listingId: string }>) {
  const t = useTranslations('contact');
  const [phone, setPhone] = useState<string | null>(null);
  const [revealing, setRevealing] = useState(false);
  const [sending, setSending] = useState(false);
  const [sent, setSent] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const revealPhone = async () => {
    setRevealing(true);
    setError(null);
    try {
      const result = await createInquiry(listingId, { type: 'PHONE_REVEAL' });
      setPhone(result.sellerPhone ?? '');
    } catch {
      setError(t('error'));
    } finally {
      setRevealing(false);
    }
  };

  const sendMessage = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const form = new FormData(event.currentTarget);
    setSending(true);
    setError(null);
    try {
      await createInquiry(listingId, {
        type: 'MESSAGE',
        buyerName: String(form.get('buyerName') ?? ''),
        buyerPhone: String(form.get('buyerPhone') ?? '') || undefined,
        buyerEmail: String(form.get('buyerEmail') ?? '') || undefined,
        message: String(form.get('message') ?? ''),
      });
      setSent(true);
    } catch {
      setError(t('error'));
    } finally {
      setSending(false);
    }
  };

  let phoneAction: ReactNode;
  if (phone === null) {
    phoneAction = (
      <Button className="w-full" onClick={() => void revealPhone()} disabled={revealing}>
        {revealing ? (
          <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
        ) : (
          <Phone className="h-4 w-4" aria-hidden />
        )}
        {t('revealPhone')}
      </Button>
    );
  } else if (phone) {
    phoneAction = (
      <a
        href={`tel:${phone}`}
        className="flex h-10 w-full items-center justify-center gap-2 rounded-md bg-brand font-medium text-brand-fg hover:bg-brand/90"
      >
        <Phone className="h-4 w-4" aria-hidden />
        {phone}
      </a>
    );
  } else {
    phoneAction = <p className="text-sm text-slate-500">{t('noPhone')}</p>;
  }

  return (
    <div className="space-y-4 rounded-lg border border-slate-200 bg-white p-4">
      <h2 className="font-display text-lg font-semibold text-slate-900">{t('title')}</h2>

      {phoneAction}

      <div className="border-t border-slate-100 pt-4">
        {sent ? (
          <div className="flex items-start gap-2 rounded-md bg-emerald-50 p-3 text-sm text-emerald-800">
            <Check className="mt-0.5 h-4 w-4 shrink-0" aria-hidden />
            <div>
              <p className="font-medium">{t('sent')}</p>
              <p className="text-emerald-700">{t('sentHint')}</p>
            </div>
          </div>
        ) : (
          <form onSubmit={(event) => void sendMessage(event)} className="space-y-2">
            <Input
              name="buyerName"
              required
              placeholder={t('yourName')}
              aria-label={t('yourName')}
            />
            <Input name="buyerPhone" placeholder={t('yourPhone')} aria-label={t('yourPhone')} />
            <Input
              name="buyerEmail"
              type="email"
              placeholder={t('yourEmail')}
              aria-label={t('yourEmail')}
            />
            <Textarea
              name="message"
              required
              placeholder={t('message')}
              aria-label={t('message')}
            />
            <Button type="submit" variant="accent" className="w-full" disabled={sending}>
              {sending ? (
                <Loader2 className="h-4 w-4 animate-spin" aria-hidden />
              ) : (
                <MessageSquare className="h-4 w-4" aria-hidden />
              )}
              {t('send')}
            </Button>
          </form>
        )}
      </div>

      {error ? <p className="text-sm text-red-600">{error}</p> : null}
    </div>
  );
}

import { getTranslations } from 'next-intl/server';
import { redirect } from 'next/navigation';
import { RegisterForm } from '@/features/auth/register-form';
import { Link } from '@/i18n/navigation';
import { getCurrentUser } from '@/lib/auth/session';

/** Seller registration page. Already-authenticated visitors are sent home. */
export default async function RegisterPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  if (await getCurrentUser()) {
    redirect(`/${locale}`);
  }

  const t = await getTranslations('auth');

  return (
    <div className="mx-auto max-w-md space-y-6">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('register.title')}</h1>
      <RegisterForm />
      <p className="text-sm text-slate-600">
        {t('register.haveAccount')}{' '}
        <Link href="/login" className="font-medium text-brand hover:text-accent">
          {t('register.loginLink')}
        </Link>
      </p>
    </div>
  );
}

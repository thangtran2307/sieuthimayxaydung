import { getTranslations } from 'next-intl/server';
import { redirect } from 'next/navigation';
import { LoginForm } from '@/features/auth/login-form';
import { Link } from '@/i18n/navigation';
import { getCurrentUser } from '@/lib/auth/session';

/** Sign-in page. Already-authenticated visitors are sent home. */
export default async function LoginPage({
  params,
}: Readonly<{ params: Promise<{ locale: string }> }>) {
  const { locale } = await params;
  if (await getCurrentUser()) {
    redirect(`/${locale}`);
  }

  const t = await getTranslations('auth');

  return (
    <div className="mx-auto max-w-md space-y-6">
      <h1 className="font-display text-2xl font-bold text-slate-900">{t('login.title')}</h1>
      <LoginForm />
      <p className="text-sm text-slate-600">
        {t('login.noAccount')}{' '}
        <Link href="/register" className="font-medium text-brand hover:text-accent">
          {t('login.registerLink')}
        </Link>
      </p>
    </div>
  );
}

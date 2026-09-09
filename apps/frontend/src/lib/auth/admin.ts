import 'server-only';
import { notFound, redirect } from 'next/navigation';
import { getCurrentUser } from './session';

/**
 * Guards an admin-only page: sends anonymous visitors to login, and returns 404 for signed-in
 * non-admins (so the moderation area's existence isn't revealed). Returns the admin user otherwise.
 */
export async function requireAdmin(locale: string) {
  const user = await getCurrentUser();
  if (!user) {
    redirect(`/${locale}/login`);
  }
  if (user.role !== 'ADMIN') {
    notFound();
  }
  return user;
}

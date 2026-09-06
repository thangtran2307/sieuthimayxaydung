/** Maps a backend error code to a translation key under the `auth.errors` namespace. */
export function authErrorKey(code: string): string {
  switch (code) {
    case 'INVALID_CREDENTIALS':
      return 'errors.invalidCredentials';
    case 'EMAIL_IN_USE':
      return 'errors.emailInUse';
    case 'VALIDATION_FAILED':
      return 'errors.validation';
    default:
      return 'errors.generic';
  }
}

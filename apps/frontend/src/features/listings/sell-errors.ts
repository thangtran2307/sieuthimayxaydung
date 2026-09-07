/** Maps a backend error code to a translation key under the `sell.errors` namespace. */
export function sellErrorKey(code: string): string {
  switch (code) {
    case 'CATEGORY_NOT_FOUND':
      return 'errors.categoryNotFound';
    case 'INVALID_SUBCATEGORY':
      return 'errors.invalidSubcategory';
    case 'UNAUTHENTICATED':
      return 'errors.unauthenticated';
    case 'VALIDATION_FAILED':
      return 'errors.validation';
    default:
      return 'errors.generic';
  }
}

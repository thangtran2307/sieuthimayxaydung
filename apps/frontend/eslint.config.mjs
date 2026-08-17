import next from 'eslint-config-next/core-web-vitals';

/** Flat ESLint config (Next 16 removed `next lint`; we run the ESLint CLI directly). */
const eslintConfig = [
  ...next,
  {
    // Test doubles legitimately use plain <a>/<img>; those Next rules don't apply to tests.
    files: ['**/__tests__/**', '**/*.test.{ts,tsx}', 'src/test/**'],
    rules: {
      '@next/next/no-html-link-for-pages': 'off',
      '@next/next/no-img-element': 'off',
    },
  },
];

export default eslintConfig;

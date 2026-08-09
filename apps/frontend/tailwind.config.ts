import type { Config } from 'tailwindcss';
import animate from 'tailwindcss-animate';

/**
 * Industrial & trustworthy palette from the design prototype:
 * steel-blue brand + safety-orange accent + boost amber.
 */
const config: Config = {
  darkMode: ['class'],
  content: ['./src/**/*.{ts,tsx}'],
  theme: {
    container: {
      center: true,
      padding: '1rem',
      screens: { '2xl': '1280px' },
    },
    extend: {
      colors: {
        brand: {
          DEFAULT: '#1e3a5f',
          fg: '#ffffff',
        },
        accent: {
          DEFAULT: '#ea580c',
          fg: '#ffffff',
        },
        boost: {
          DEFAULT: '#f59e0b',
        },
      },
      fontFamily: {
        display: ['Archivo', 'system-ui', 'sans-serif'],
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [animate],
};

export default config;

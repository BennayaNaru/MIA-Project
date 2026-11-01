import forms from '@tailwindcss/forms'


export default {
  content: ["./index.html", "./src/**/*.{js,jsx,ts,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: '#173B57',
        'primary-container': '#23496B',
        secondary: '#007A6E',
        'secondary-container': '#86F2E2',
        surface: '#F8FAFC',
        'surface-container-lowest': '#FFFFFF',
        'surface-container-low': '#F1F5F9',
        'surface-container': '#E2E8F0',
        outline: '#64748B',
        'outline-variant': '#E2E8F0',
        'on-surface': '#0F172A',
        'on-surface-variant': '#475569',
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
        display: ['Plus Jakarta Sans', 'sans-serif'],
      },
    },
  },
  plugins: [forms],
}
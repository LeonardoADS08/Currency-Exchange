/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          100: '#e6f0ff',
          500: '#2196F3',
          600: '#1976D2',
        }
      }
    }
  },
  plugins: [],
}


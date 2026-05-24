import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // כל בקשת fetch שתתחיל ב- api/ תועבר אוטומטית לשרת ה- .NET
      '/api': {
        target: 'http://localhost:5270', // 👈 שים לב: שנה את ה-5143 לפורט האמיתי של ה-.NET שלך!
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');

  // Aspire injects service discovery variables while running the AppHost. When the
  // frontend is started on its own, fall back to VITE_API_URL or the Web launch profile.
  const target =
    process.env['services__webapi__https__0'] ||
    process.env['services__webapi__http__0'] ||
    env.VITE_API_URL ||
    'http://localhost:5295';

  const proxyOptions = { target, secure: false, changeOrigin: true };

  return {
    plugins: [react(), tailwindcss()],
    server: {
      port: Number.parseInt(env.PORT ?? '5173', 10),
      proxy: {
        '/api': proxyOptions,
        '/openapi': proxyOptions,
        '/scalar': proxyOptions,
      },
    },
    build: {
      outDir: 'build',
    },
  };
});

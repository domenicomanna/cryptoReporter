import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import svgr from 'vite-plugin-svgr';
import checker from 'vite-plugin-checker';
import eslintPlugin from '@nabla/vite-plugin-eslint';

export default defineConfig({
  base: '/',
  plugins: [react(), svgr(), checker({ typescript: true }), eslintPlugin()],
  build: {
    outDir: './build',
  },
  server: {
    port: 5173,
  },
});

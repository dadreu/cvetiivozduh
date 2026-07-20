import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import prerender from 'vite-plugin-prerender'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    prerender({
      staticDir: path.join(__dirname, 'dist'),
      routes: ['/'],
    })
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    proxy: {
      '/api': 'http://localhost:5000',
      '/uploads': 'http://localhost:5000'
    }
  }
})

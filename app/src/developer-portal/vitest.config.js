/// <reference types="vitest" />
import { defineConfig } from 'vite';

export default defineConfig({
    test: {
        alias: {
            '~/': new URL('./', import.meta.url).pathname
        }
    }
});

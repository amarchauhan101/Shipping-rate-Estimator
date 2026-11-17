import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // The browser calls /api/... and Vite forwards it to the .NET backend
    proxy: {
      "/api": "http://localhost:5080",
    },
  },
});

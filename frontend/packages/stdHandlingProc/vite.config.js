import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import { quasar, transformAssetUrls } from "@quasar/vite-plugin";
import legacy from "@vitejs/plugin-legacy";
import { legacyQiankun } from "vite-plugin-legacy-qiankun";

import { name as packageName } from "./package.json";

export default ({ mode }) => {
  process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };
  const { VUE_APP_ASSETS_URL } = process.env;

  return defineConfig({
    base: VUE_APP_ASSETS_URL,
    plugins: [
      vue({
        template: { transformAssetUrls },
      }),
      quasar({
        sassVariables: "src/quasar-variables.sass",
      }),
      legacy(),
      legacyQiankun({ name: packageName, devSandbox: true }),
    ],
    server: {
      port: 5007,
      origin: "http://localhost:5007",
    },
    build: {
      outDir: "output",
      rollupOptions: {
        output: {
          assetFileNames: (assetInfo) => {
            let extType = assetInfo.name.split(".").at(1);
            if (/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)) {
              extType = "img";
            }
            return `assets/${extType}/[name]-[hash][extname]`;
          },

          chunkFileNames: () => {
            return `assets/js/[name]-[hash].js`;
          },

          entryFileNames: () => {
            return `assets/js/[name]-[hash].js`;
          },
        },
      },
    },
  });
};

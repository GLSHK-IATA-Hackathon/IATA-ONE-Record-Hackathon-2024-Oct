import { createI18n } from "vue-i18n";

const messages = {
  "en-us": {
    footer: "footer",
  },
  "zh-hk": {
    footer: "頁尾",
  },
};

export const i18n = createI18n({
  locale: "en-us",
  fallbackLocale: "en-us",
  messages,
  legacy: false,
});

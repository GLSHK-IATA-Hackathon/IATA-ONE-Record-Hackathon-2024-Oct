import { createI18n } from "vue-i18n";

const messages = {
  "en-us": {
    header: "header",
    logout: "logout",
  },
  "zh-hk": {
    header: "頁首",
    logout: "登出",
  },
};

export const i18n = createI18n({
  locale: "en-us",
  fallbackLocale: "en-us",
  messages,
  legacy: false,
});

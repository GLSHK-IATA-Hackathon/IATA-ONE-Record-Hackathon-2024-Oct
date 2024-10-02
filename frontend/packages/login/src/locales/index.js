import { createI18n } from "vue-i18n";

const messages = {
  "en-us": {
    login: "login",
  },
  "zh-hk": {
    login: "登入",
  },
};

export const i18n = createI18n({
  locale: "en-us",
  fallbackLocale: "en-us",
  messages,
  legacy: false,
});

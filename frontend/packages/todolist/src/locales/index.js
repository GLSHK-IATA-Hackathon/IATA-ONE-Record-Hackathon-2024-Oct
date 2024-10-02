import { createI18n } from "vue-i18n";

const messages = {
  "en-us": {
    add: "add",
    delete: "delete",
  },
  "zh-hk": {
    add: "新增",
    delete: "刪除",
  },
};

export const i18n = createI18n({
  locale: "en-us",
  fallbackLocale: "en-us",
  messages,
  legacy: false,
});

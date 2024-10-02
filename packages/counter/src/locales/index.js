import { createI18n } from "vue-i18n";

const messages = {
  "en-us": {
    count: "count",
    increase: "increase",
    decrease: "decrease",
  },
  "zh-hk": {
    count: "次數",
    increase: "增加",
    decrease: "減少",
  },
};

export const i18n = createI18n({
  locale: "en-us",
  fallbackLocale: "en-us",
  messages,
  legacy: false,
});

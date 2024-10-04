import { createWebHistory, createRouter } from "vue-router";

import { i18n } from "../locales";
import { getLanguageFromUrl } from "../../../../libs/utils/router";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

router.beforeEach((to, from, next) => {
  const lang = getLanguageFromUrl();
  if (!availableLocales.includes(lang)) {
    console.error(
      `invalid lang in url, redirect to fallback locale:${fallbackLocale.value}`
    );
    // next(`${fallbackLocale.value}`);
  } else {
    i18n.global.locale.value = lang;
  }

  next();
});

export default router;

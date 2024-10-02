import { createWebHistory, createRouter } from "vue-router";

import Counter from "../components/Counter.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "counter",
      params: { lang: fallbackLocale.value },
    },
  },
  {
    path: "/:lang",
    redirect: {
      name: "counter",
      params: { lang: fallbackLocale.value },
    },
    children: [
      {
        path: "counter",
        name: "counter",
        component: Counter,
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

router.beforeEach((to, from, next) => {
  const {
    params: { lang },
  } = to;

  // adjust locale
  if (!availableLocales.includes(lang)) {
    console.error(
      `invalid lang in url, redirect to fallback locale:${fallbackLocale.value}`
    );
    next(`${fallbackLocale.value}/counter`);
  } else {
    i18n.global.locale.value = lang;
  }

  next();
});

export default router;

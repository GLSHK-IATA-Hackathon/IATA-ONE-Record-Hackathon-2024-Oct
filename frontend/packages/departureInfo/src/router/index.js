import { createWebHistory, createRouter } from "vue-router";

import DepartureInfo from "../components/DepartureInfo.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "departureInfo",
      params: { lang: fallbackLocale.value },
    },
  },
  {
    path: "/:lang",
    redirect: {
      name: "departureInfo",
      params: { lang: fallbackLocale.value },
    },
    children: [
      {
        path: "departureInfo",
        name: "departureInfo",
        component: DepartureInfo,
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
    next(`${fallbackLocale.value}/departureInfo`);
  } else {
    i18n.global.locale.value = lang;
  }

  next();
});

export default router;

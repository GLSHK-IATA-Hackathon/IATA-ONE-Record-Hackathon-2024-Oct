import { createWebHistory, createRouter } from "vue-router";

import AdminSetting from "../components/AdminSetting.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "adminSetting",
      params: { lang: fallbackLocale.value },
    },
  },
  {
    path: "/:lang",
    redirect: {
      name: "adminSetting",
      params: { lang: fallbackLocale.value },
    },
    children: [
      {
        path: "adminSetting",
        name: "adminSetting",
        component: AdminSetting,
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
    next(`${fallbackLocale.value}/admin`);
  } else {
    i18n.global.locale.value = lang;
  }

  next();
});

export default router;

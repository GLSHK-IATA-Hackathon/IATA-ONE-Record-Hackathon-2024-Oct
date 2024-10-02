import { createWebHistory, createRouter } from "vue-router";

import Todolist from "../components/Todolist.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "todolist",
      params: { lang: fallbackLocale.value },
    },
  },
  {
    path: "/wtf",
    component: Todolist,
  },
  {
    path: "/:lang",
    redirect: {
      name: "todolist",
      params: { lang: fallbackLocale.value },
    },
    children: [
      {
        path: "todolist",
        name: "todolist",
        component: Todolist,
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
    next(`${fallbackLocale.value}/todolist`);
  } else {
    i18n.global.locale.value = lang;
  }

  next();
});

export default router;

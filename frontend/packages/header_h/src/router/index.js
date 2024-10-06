import { createWebHistory, createRouter } from "vue-router";

import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});


export default router;

import { createWebHistory, createRouter } from "vue-router";

import Login from "../components/Login.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "login"
    },
  },
  {
    path: "/login",
    name: "login",
    component: Login,
  },
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

export default router;

import { createWebHistory, createRouter } from "vue-router";

import AdminSetting from "@/components/AdminSetting.vue";
import { i18n } from "@/locales";
import DefaultLayout from '../layouts/DefaultLayout.vue'

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "adminSetting",
    },
  },
  {
    path: "/adminSetting",
    name: "adminSetting",
    component: AdminSetting,
    /*
    meta: {
      layout: DefaultLayout
    }
      */
  },
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

export default router;

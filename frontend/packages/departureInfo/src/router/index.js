import { createWebHistory, createRouter } from "vue-router";
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import DepartureInfo from "@/components/DepartureInfo.vue";
import { i18n } from "@/locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "departureInfo",
    },
  },
  {
    path: "/departureInfo",
    name: "departureInfo",
    component: DepartureInfo,
    meta: {
      layout: DefaultLayout
    }
  },
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

export default router;

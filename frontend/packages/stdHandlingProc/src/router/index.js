import { createWebHistory, createRouter } from "vue-router";
import DefaultLayout from '../layouts/DefaultLayout.vue'
import StdHandlingProc from "../components/stdHandlingProc.vue";
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "stdHandlingProc",
    },
  },
  {
    path: "/stdHandlingProc",
    name: "stdHandlingProc",
    component: StdHandlingProc,
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

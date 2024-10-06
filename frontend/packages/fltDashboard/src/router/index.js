import { createWebHistory, createRouter } from "vue-router";
import DefaultLayout from '../layouts/DefaultLayout.vue'
import FltDashboard from "../components/FltDashboard.vue";
import { i18n } from "../locales";


const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "fltDashboard",
    },
  },
  {
    path: "/fltDashboard",
    name: "fltDashboard",
    component: FltDashboard,
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

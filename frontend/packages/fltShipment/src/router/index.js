import { createWebHistory, createRouter } from "vue-router";
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import FltShipment from "@/components/FltShipment.vue";
import ArrivalPage from '@/views/ArrivalPage.vue'
import ShipmentJourneyPage from '@/views/ShipmentJourneyPage.vue'
import { i18n } from "../locales";

const {
  global: { availableLocales, fallbackLocale },
} = i18n;

const routes = [
  {
    path: "/",
    redirect: {
      name: "fltShipment",
    },
  },
  {
    path: "/fltShipment",
    name: "fltShipment",
    component: FltShipment,
    /*
    meta: {
      layout: DefaultLayout,
    }
    */
  },
  {
    path: '/arrival/:flightNo',
    name: 'Arrival',
    component: ArrivalPage,
    meta: {
      layout: DefaultLayout,
    }
  },
  {
    path: '/shipmentJourney',
    name: 'ShipmentJourney',
    component: ShipmentJourneyPage,
    meta: {
      layout: DefaultLayout,
    }
  }
];

const router = createRouter({
  history: createWebHistory("/"),
  routes,
});

export default router;

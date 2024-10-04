import { createApp } from "vue";
import {
  registerMicroApps,
  loadMicroApp,
  start,
  setDefaultMountApp,
} from "qiankun";

import "./style.css";
import App from "./App.vue";

let isHeaderAndFooterDisplay = false;
let headerApp = null;
let footerApp = null;

registerMicroApps([
  /*
  {
    name: "counter",
    entry: import.meta.env.VITE_COUNTER_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/counter");
    },
  },
  */
  {
    name: "login",
    entry: import.meta.env.VITE_LOGIN_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/login");
    },
  },
  /*
  {
    name: "todolist",
    entry: import.meta.env.VITE_TODOLIST_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/todolist");
    },
  },
  */
  {
    name: "headerAndFooter",
    entry: "",
    container: "",
    activeRule: () => {
      // memo: header and footer will display by loadMicroApp
      // so return false is OK in here
      checkHeaderAndFooterNeedToDisplay();

      return false;
    },
  },
  {
    name: "adminSetting",
    entry: import.meta.env.VITE_ADMINSETTING_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/admin");
    },
  },
  {
    name: "stdHandlingProc",
    entry: import.meta.env.VITE_STD_HANDLING_PROC_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/stdhandleproc");
    },
  },
  {
    name: "fltShipment",
    entry: import.meta.env.VITE_FLT_SHIPMENT_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/fltShipment");
    },
  },
  {
    name: "fltDashboard",
    entry: import.meta.env.VITE_FLT_DASHBOARD_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/fltDashboard");
    },
  },
  {
    name: "departureInfo",
    entry: import.meta.env.VITE_DEPARTURE_INFO_URL,
    container: "#main",
    activeRule: (location) => {
      return location.pathname.includes("/departureInfo");
    },
  },
]);

start({
  singular: false,
  prefetch: false,
  sandbox: { experimentalStyleIsolation: true },
});

setDefaultMountApp("/en-us/login");

function checkHeaderAndFooterNeedToDisplay() {
  const lastPathName = location.pathname
    .split("/")
    .filter((path) => !!path)
    .at(-1);

  const hiddenList = ["login"];

  const isPathnameIncludeError = location.pathname.split("/").includes("error");

  const isNeedToDisplay =
    lastPathName &&
    !hiddenList.includes(lastPathName) &&
    !isPathnameIncludeError;
  // console.log('checking header & footer', isNeedToDisplay)
  if (isNeedToDisplay) {
    setTimeout(() => {
      loadHeaderAndFooter();
    }, 200);
  } else {
    hideHeader();
  }
}

function hideHeader() {
  headerApp?.unmount();
  footerApp?.unmount();
}

function loadHeaderAndFooter() {
  if (isHeaderAndFooterDisplay) {
    console.log("header & footer already display");
  } else {
    isHeaderAndFooterDisplay = true;
    const options = {
      sandbox: { experimentalStyleIsolation: true },
    };
    headerApp = loadMicroApp(
      {
        name: "header_h",
        entry: import.meta.env.VITE_HEADER_URL,
        container: "#header",
      },
      options
    );
    // known issue: https://github.com/lishaobos/vite-plugin-legacy-qiankun/issues/19
    setTimeout(() => {
      footerApp = loadMicroApp(
        {
          name: "footer",
          entry: import.meta.env.VITE_FOOTER_URL,
          container: "#footer",
        },
        options
      );
    }, 200);
  }
}

createApp(App).mount("#main-frame__container");

import { createApp } from "vue";
import { createLifecyle, getMicroApp } from "vite-plugin-legacy-qiankun";

import { name as packageName } from "../package.json";
import App from "./App.vue";
import router from "./router";
import { i18n } from "./locales";

const app = createApp(App);

app.use(router);
app.use(i18n);

const microApp = getMicroApp(packageName);

const render = (props) => {
  const { container } = props;
  app.mount(container ? container.querySelector("#app") : "#app");
};

if (microApp.__POWERED_BY_QIANKUN__) {
  createLifecyle(packageName, {
    mount(props) {
      console.log("mount", packageName);
      render(props);
    },
    bootstrap() {
      console.log("bootstrap", packageName);
    },
    unmount() {
      console.log("unmount", packageName);
    },
  });
} else {
  render({});
}

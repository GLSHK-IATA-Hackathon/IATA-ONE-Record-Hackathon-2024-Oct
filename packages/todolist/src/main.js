import { createApp } from "vue";

import App from "./App.vue";
import router from "./router";
import { i18n } from "./locales";

const app = createApp(App);

app.use(router);
app.use(i18n);

async function render(props) {
  const { container } = props;
  app.mount(container ? container.querySelector("#app") : "#app");
}

if (!window.__POWERED_BY_QIANKUN__) {
  render({});
}

export async function bootstrap() {
  console.log("[vue] vue app bootstraped");
}
export async function mount(props) {
  console.log("[vue] props from main framework", props);
  render(props);
}
export async function unmount() {
  instance.$destroy();
  instance.$el.innerHTML = "";
  instance = null;
}

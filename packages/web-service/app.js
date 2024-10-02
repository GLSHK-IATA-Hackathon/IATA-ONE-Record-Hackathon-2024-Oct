const compression = require("compression");
const express = require("express");
const cors = require("cors");
const { execSync } = require("child_process");

const routes = require("./routes");
// const copyWebsettings = require('./utils/copyWebsettings')

const app = express();
const port = process.env.NODE_PORT || 9000;

// replace env and copy dist
const projects = [
  "login",
  "main-frame",
  "counter",
  "header",
  "footer",
  "todolist",
];

for (const project of projects) {
  execSync(`cd ../${project} && yarn replaceEnv && yarn copyDistFolder`, {
    stdio: "inherit",
  });
}

// Compress all HTTP responses
app.use(compression());

app.use(
  cors({
    origin: "*",
  })
);
app.use(routes);
app.listen(port, () => {
  console.log(`[worker] Application is running on port ${port}...`);
});

module.exports = app;

const express = require("express");
const path = require("path");
const router = express.Router();

const healthRouter = require("./health");

const options = {
  maxAge: "30 days",
  setHeaders: (res, path) => {
    if (express.static.mime.lookup(path) === "text/html") {
      res.setHeader("Cache-Control", "no-store");
    }
  },
};
const handler = express.static("./dist/", options);

router.use(handler);
router.get("/", handler);

router.get("/healthcheck", healthRouter);
router.get("*", (req, res) => {
  res.sendFile(path.join(__dirname, "../dist", "index.html"), {
    headers: { "Cache-Control": "no-store" },
  });
});
module.exports = router;

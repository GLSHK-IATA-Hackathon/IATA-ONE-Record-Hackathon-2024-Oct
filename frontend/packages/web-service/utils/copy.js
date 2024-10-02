const fs = require("fs");
const fsPromises = fs.promises;
const path = require("path");
const minimist = require("minimist");

const argv = minimist(process.argv.slice(2));
const root = path.join(__dirname, "../../..");
const ORIGIN_BUNDLE_FILE_PATH = path.join(root, argv.originalBundlePath);
const TARGET_BUNDLE_FILE_PATH = path.join(root, argv.targetBundlePath);

function copyBundleFile() {
  return new Promise((resolve, reject) => {
    fsPromises
      .cp(ORIGIN_BUNDLE_FILE_PATH, TARGET_BUNDLE_FILE_PATH, { recursive: true })
      .then(() => resolve())
      .catch((err) => reject(err));
  });
}

async function main() {
  console.log(
    `start copy from ${ORIGIN_BUNDLE_FILE_PATH} to ${TARGET_BUNDLE_FILE_PATH} web-service`
  );
  return new Promise((resolve, reject) => {
    copyBundleFile()
      .then(() => {
        console.log("copy finished");
        resolve();
      })
      .catch((err) => console.error("copy failed", err));
  });
}

main();

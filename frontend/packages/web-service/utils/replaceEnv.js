const fs = require("fs");
const fsPromises = fs.promises;
const path = require("path");
const readline = require("readline");
const minimist = require("minimist");

const argv = minimist(process.argv.slice(2));
const root = path.join(__dirname, "../../../");
const ORIGIN_BUNDLE_FILE_PATH = path.join(root, argv.originalBundlePath);
const TARGET_BUNDLE_FILE_PATH = path.join(root, argv.targetBundlePath);
const ENV_PATH = path.join(root, argv.envPath);

const getAllFiles = function (dirPath, arrayOfFiles) {
  files = fs.readdirSync(dirPath);
  arrayOfFiles = arrayOfFiles || [];
  files.forEach(function (file) {
    if (fs.statSync(dirPath + "/" + file).isDirectory()) {
      arrayOfFiles = getAllFiles(dirPath + "/" + file, arrayOfFiles);
    } else {
      arrayOfFiles.push(path.join(dirPath, "/", file));
    }
  });

  return arrayOfFiles;
};

async function readEnv(_path) {
  const result = {};
  const fileStream = fs.createReadStream(path.resolve(_path));

  const rl = readline.createInterface({
    input: fileStream,
    crlfDelay: Infinity,
  });

  for await (const line of rl) {
    const [key, value] = line.split("=").map((str) => str.trim());
    if (!key || !value) continue;

    if (!result.hasOwnProperty(key)) {
      result[key] = value;
    }
  }

  return result;
}

async function copyBundleFile() {
  return new Promise((resolve, reject) => {
    fsPromises
      .cp(ORIGIN_BUNDLE_FILE_PATH, TARGET_BUNDLE_FILE_PATH, { recursive: true })
      .then(() => resolve())
      .catch((err) => reject(err));
  });
}

function getNeedReplacedFile() {
  return new Promise((resolve) => {
    const files = getAllFiles(TARGET_BUNDLE_FILE_PATH).filter((file) => {
      const targetExtensions = [".js", ".css", ".html"];

      return targetExtensions.some(
        (acceptedExtname) => path.extname(file) === acceptedExtname
      );
    });
    resolve(files);
  });
}

function getEnvs(mode) {
  const readOriginEnvPromise = readEnv(path.join(ENV_PATH, "/.env"));
  const readTargetEnvPromise = readEnv(path.join(ENV_PATH, `/.env.${mode}`));

  return Promise.all([readOriginEnvPromise, readTargetEnvPromise]).then(
    ([originEnv, targetEnv]) => {
      return {
        originEnv,
        targetEnv,
      };
    }
  );
}

function replace(file, originEnv, targetEnv) {
  return new Promise((resolve, reject) => {
    let result = "";
    fs.createReadStream(file)
      .setEncoding("UTF8")
      .on("error", (error) => reject(error))
      .on("data", (data) => {
        for (const key in originEnv) {
          const regex = new RegExp(`\/?${originEnv[key]}\/?`, "g");
          let replacedString = targetEnv[key];
          if (key === "VUE_APP_ASSETS_URL") {
            const subPathname = file.match(/classic|ez4|lib/g);
            replacedString = subPathname
              ? `${replacedString}${subPathname}/`
              : replacedString;
          }
          // console.log('replacedString', replacedString)
          data = data.replace(regex, replacedString);
        }
        result += data;
      })
      .on("end", () => {
        fsPromises
          .writeFile(file, result)
          .then(() => resolve())
          .catch((error) => reject(error));
      });
  });
}

function main() {
  const mode = process.env.NODE_ENV || "d0";
  console.log(`[Job]: Replace bundle file with .env.${mode} - start`);

  return new Promise((resolve, reject) => {
    copyBundleFile()
      .then(() => {
        return Promise.all([getNeedReplacedFile(), getEnvs(mode)]).then(
          ([files, { originEnv, targetEnv }]) => {
            return {
              files,
              originEnv,
              targetEnv,
            };
          }
        );
      })
      .then(({ files, originEnv, targetEnv }) => {
        // console.log(files)
        // console.log(originEnv)
        // console.log(targetEnv)
        const replacePromiseJobs = files.map((file) =>
          replace(file, originEnv, targetEnv)
        );

        return Promise.all(replacePromiseJobs).catch((error) => {
          throw error;
        });
      })
      .then(() => {
        console.log(`[Job]: Replace bundle file with .env.${mode} - finish`);
        resolve();
      })
      .catch((err) => {
        console.error(
          `[Job]: Replace bundle file with .env.${mode} - interruption with error`
        );
        reject(err);
      });
  });
}

main();

const fs = require('fs')
const fsPromises = fs.promises
const path = require('path')
const readline = require('readline')

const ORIGIN_BUNDLE_FILE_PATH = path.join(__dirname, '../../output')
const TARGET_BUNDLE_FILE_PATH = path.join(__dirname, '../../dist')

const getAllFiles = function (dirPath, arrayOfFiles) {
  files = fs.readdirSync(dirPath)
  arrayOfFiles = arrayOfFiles || []
  files.forEach(function (file) {
    if (fs.statSync(dirPath + '/' + file).isDirectory()) {
      arrayOfFiles = getAllFiles(dirPath + '/' + file, arrayOfFiles)
    } else {
      arrayOfFiles.push(path.join(dirPath, '/', file))
    }
  })

  return arrayOfFiles
}

async function readEnv(_path) {
  const result = {}
  const fileStream = fs.createReadStream(path.resolve(__dirname, _path))

  const rl = readline.createInterface({
    input: fileStream,
    crlfDelay: Infinity,
  })

  for await (const line of rl) {
    const [key, value] = line.split('=').map((str) => str.trim())
    if (!key || !value) continue

    if (!result.hasOwnProperty(key)) {
      result[key] = value
    }
  }

  return result
}

function copyBundleFile() {
  return new Promise((resolve, reject) => {
    fsPromises
      .cp(ORIGIN_BUNDLE_FILE_PATH, TARGET_BUNDLE_FILE_PATH, { recursive: true })
      .then(() => resolve())
      .catch((err) => reject(err))
  })
}

function getNeedReplacedFile() {
  return new Promise((resolve) => {
    const files = getAllFiles(TARGET_BUNDLE_FILE_PATH).filter((file) => {
      const targetExtensions = ['.js', '.css', '.html']

      return targetExtensions.some(
        (acceptedExtname) => path.extname(file) === acceptedExtname
      )
    })
    resolve(files)
  })
}

function getEnvs(mode) {
  const readOriginEnvPromise = readEnv(path.join(__dirname, '../../.env'))
  const readTargetEnvPromise = readEnv(
    path.join(__dirname, `../../.env.${mode}`)
  )

  return Promise.all([readOriginEnvPromise, readTargetEnvPromise]).then(
    ([originEnv, targetEnv]) => {
      return {
        originEnv,
        targetEnv,
      }
    }
  )
}

async function replace(file, originEnv, targetEnv) {
  try {
    const data = await fsPromises.readFile(file, 'utf8')
    let result = data

    for (const key in originEnv) {
      const regex = new RegExp(`\/?${originEnv[key]}\/?`, 'g')
      result = result.replace(regex, targetEnv[key])
    }

    await fsPromises.writeFile(file, result)
    return Promise.resolve()
  } catch (error) {
    return Promise.reject(error)
  }
}

async function main(mode) {
  console.log(`[Job]: Replace bundle file with .env.${mode} - start`)

  return new Promise((resolve, reject) => {
    copyBundleFile()
      .then(() => {
        return Promise.all([getNeedReplacedFile(), getEnvs(mode)]).then(
          ([files, { originEnv, targetEnv }]) => {
            return {
              files,
              originEnv,
              targetEnv,
            }
          }
        )
      })
      .then(({ files, originEnv, targetEnv }) => {
        // console.log(files)
        // console.log(originEnv)
        // console.log(targetEnv)
        const replacePromiseJobs = files.map((file) =>
          replace(file, originEnv, targetEnv)
        )

        return Promise.all(replacePromiseJobs).catch((error) => {
          throw error
        })
      })
      .then(() => {
        console.log(`[Job]: Replace bundle file with .env.${mode} - finish`)
        resolve()
      })
      .catch((err) => {
        console.error(
          `[Job]: Replace bundle file with .env.${mode} - interruption with error`
        )
        reject(err)
      })
  })
}

module.exports = main

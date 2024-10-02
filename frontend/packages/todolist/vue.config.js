const { defineConfig } = require("@vue/cli-service");
const { name: packageName } = require("./package.json");

module.exports = defineConfig({
  transpileDependencies: true,
  publicPath: process.env.VUE_APP_ASSETS_URL,
  outputDir: "output",
  devServer: {
    port: 5005,
    headers: {
      "Access-Control-Allow-Origin": "*",
    },
  },
  configureWebpack: {
    output: {
      library: `${packageName}-[name]`,
      libraryTarget: "umd",
      chunkLoadingGlobal: `webpackJsonp_${packageName}`,
    },
  },
});

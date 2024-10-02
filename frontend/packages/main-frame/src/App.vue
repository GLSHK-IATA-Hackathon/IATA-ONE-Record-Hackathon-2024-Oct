<script setup></script>

<template>
  <header id="header"></header>
  <main id="main"></main>
  <footer id="footer"></footer>
</template>

<style scope lang="scss">
#main {
  flex-grow: 1;
  :deep() {
    > div {
      height: 100%;
    }
  }
}
</style>

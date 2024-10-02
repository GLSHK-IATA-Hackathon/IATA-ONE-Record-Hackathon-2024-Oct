<script setup>
import { ref, watch } from "vue";
import { getLanguageFromUrl } from "../../../../libs/utils/router";

const lang = ref(getLanguageFromUrl());

watch(lang, (newLang) => {
  const nextHref = location.pathname
    .split("/")
    .filter((i) => !!i)
    .map((i, index) => (index === 0 ? newLang : i))
    .join("/");

  location.href = `/${nextHref}`;
});
function logout() {
  const lang = getLanguageFromUrl();
  location.href = `/${lang}/login`;
}

function toSubApp(subApp) {
  const lang = getLanguageFromUrl();
  location.href = `/${lang}${subApp}`;
}
</script>

<template>
  <div class="header">
    <span class="header__title text"> Ezy Cargo - {{ $t("header") }} </span>
    <div class="header__menu">
      <div @click="toSubApp('/counter')">Counter</div>
      <div @click="toSubApp('/todolist')">Todolist</div>
    </div>
    <select v-model="lang" class="header__lang-selector" @onchange="changeLang">
      <option>en-us</option>
      <option>zh-hk</option>
    </select>
    <button class="header__logout-btn" @click="logout">
      {{ $t("logout") }}
    </button>
  </div>
</template>

<style>
.text {
  color: blue;
}
</style>
<style scoped lang="scss">
.header {
  height: 60px;
  padding: 0 16px;
  width: auto;
  display: flex;
  align-items: center;
  border-bottom: 2px solid black;

  &__title {
    margin-right: 32px;
  }

  &__menu {
    display: flex;
    gap: 12px;
    font-weight: bold;
    flex-grow: 1;

    & > * {
      cursor: pointer;
      color: #40c4ff;

      &:hover {
        text-decoration: underline;
      }
    }
  }
  &__lang-selector {
    margin-right: 16px;
  }

  &__logout-btn {
    margin-left: auto;
  }
}
</style>

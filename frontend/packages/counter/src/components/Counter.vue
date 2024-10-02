<script setup>
import { ref } from "vue";

const count = ref(0);
const apiUrl = import.meta.env.VITE_APP_API_BASE_URL;

function increase() {
  count.value++;
}

function decrease() {
  count.value--;
}
</script>

<template>
  <div class="counter">
    <div class="counter__content">{{ $t("count") }}: {{ count }}</div>
    <div class="counter__button-section">
      <button @click="increase">{{ $t("increase") }}</button>
      <button @click="decrease">{{ $t("decrease") }}</button>
    </div>
    <div class="text">api url: {{ apiUrl }}</div>
  </div>
</template>

<style>
.text {
  color: red !important;
}
</style>

<style scoped lang="scss">
.counter {
  width: 400px;
  margin: auto;
  margin-top: 16px;

  &__content {
    margin-bottom: 16px;
  }

  &__button-section {
    display: flex;
    gap: 8px;
  }
}
</style>

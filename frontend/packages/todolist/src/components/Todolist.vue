<script setup>
import { ref } from "vue";
import { v4 as uuidv4 } from "uuid";

const DEFAULT_LIST = [
  {
    value: "feed cat",
    id: uuidv4(),
  },
];

const todolist = ref([...DEFAULT_LIST]);
const inputValue = ref("");
const apiUrl = process.env.VUE_APP_API_BASE_URL;

function addItem() {
  if (inputValue.value === "") return;

  const item = {
    value: inputValue.value,
    id: uuidv4(),
  };

  todolist.value = [...todolist.value, item];
  inputValue.value = "";
}

function deleteItem(id) {
  todolist.value = todolist.value.filter((item) => item.id !== id);
}
</script>

<template>
  <div class="todo">
    <div>api url: {{ apiUrl }}</div>
    <br />
    <div class="todo__input-wrapper">
      <input v-model="inputValue" @keypress.enter="addItem" />
      <button @click="addItem">{{ $t("add") }}</button>
    </div>
    <div class="todo__list">
      <div v-for="todo in todolist" :key="todo.id" class="todo__item">
        <span>{{ todo.value }}</span>
        <button @click="() => deleteItem(todo.id)">{{ $t("delete") }}</button>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.todo {
  width: 400px;
  display: flex;
  flex-direction: column;
  margin: 16px auto 0;

  &__input-wrapper {
    margin-bottom: 24px;
    align-self: center;
  }

  &__list {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  &__item {
    display: flex;
    justify-content: space-between;
  }
}
</style>

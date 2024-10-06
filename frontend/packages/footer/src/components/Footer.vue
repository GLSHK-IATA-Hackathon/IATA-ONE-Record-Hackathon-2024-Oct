<script setup></script>

<template>
  <div class="footer">
    <span> 2024 GLS HK. All rights reserved.</span>
  </div>
</template>

<style scoped lang="scss">
.footer {
  height: 60px;
  padding: 0 16px;
  width: auto;
  display: flex;
  align-items: center;
  border-top: 1px solid black;
  gap: 32px;

  &__menu {
    display: flex;
    gap: 12px;
    font-weight: bold;

    & > * {
      cursor: pointer;
      color: #40c4ff;

      &:hover {
        text-decoration: underline;
      }
    }
  }

  &__logout-btn {
    margin-left: auto;
  }
}
</style>

<script setup></script>

<template>
  <component :is="$route.meta.layout || 'div'">
    <router-view />
  </component>
</template>

<template>
  <span
    class="gls-custom-icon"
    :class="className"
    :style="{
      width: `${width}px`,
      height: `${height}px`,
      transform: `rotate(${rotate}deg)`,
    }"
    v-html="svgContent"
  ></span>
</template>

<script setup>
import { computed } from 'vue'
import { iconMap } from '@/constants/iconMap'

const props = defineProps({
  name: {
    type: String,
    required: true,
  },
  width: {
    type: [Number, String],
    default: 16,
  },
  height: {
    type: [Number, String],
    default: 14,
  },
  className: {
    type: String,
    default: '',
  },
  color: {
    type: String,
    default: '',
  },
  // Add a new prop for fill color
  fillColor: {
    type: String,
    default: '',
  },
  rotate: {
    type: [Number, String],
    default: 0,
  },
})

const svgContent = computed(() => {
  let svg = iconMap[props.name] || ''
  if (props.color) {
    svg = svg.replace(/stroke="[^"]+"/g, `stroke="${props.color}"`)
  }
  if (props.fillColor) {
    svg = svg.replace(/fill="[^"]+"/g, `fill="${props.fillColor}"`)
  }
  return svg
})
</script>

<style scoped>
.gls-custom-icon {
  display: inline-block;
  line-height: 0;
}
.gls-custom-icon :deep(svg) {
  width: 100%;
  height: 100%;
}
</style>

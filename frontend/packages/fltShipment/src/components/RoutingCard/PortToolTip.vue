<script setup>
import { ref, nextTick } from 'vue'

import { hasValue } from '@/utils/misc'

const intervalId = ref(null)
const isVisible = ref(false)
const positionClass = ref('tooltip-right')
const tooltipRef = ref(null)

async function updatePosition(event) {
  if (hasValue(intervalId.value)) {
    clearInterval(intervalId.value)
    intervalId.value = null
  }
  isVisible.value = true

  await nextTick(() => {
    const triggerRect = event.target.getBoundingClientRect()
    const tooltipRect = tooltipRef.value.getBoundingClientRect()
    const spaceRight = window.innerWidth - triggerRect.right

    if (spaceRight > tooltipRect.width) {
      positionClass.value = 'tooltip-right'
    } else {
      positionClass.value = 'tooltip-left'
    }
  })
}

function hideTooltip() {
  intervalId.value = setInterval(() => (isVisible.value = false), 100)
}
</script>

<template>
  <div
    class="tooltip-container"
    @mouseenter="updatePosition"
    @mouseleave="hideTooltip"
  >
    <!-- text -->
    <span class="tooltip-text">
      <slot />
    </span>

    <!-- tooltip -->

    <Transition name="fade" mode="out-in">
      <span
        v-if="isVisible"
        :class="['tooltip', positionClass]"
        ref="tooltipRef"
      >
        <slot name="tooltip" />
      </span>
    </Transition>
  </div>
</template>

<style scoped lang="scss">
.tooltip-container {
  position: relative;
  display: inline-block;
}

.tooltip-text {
  cursor: pointer;
}

.tooltip {
  position: absolute;
  z-index: 1;
  color: #fff;
  text-align: left;
  white-space: nowrap;
}

.tooltip-left {
  top: 50%;
  right: 100%;
  transform: translateY(-50%);
  margin-right: 12px;
}

.tooltip-right {
  top: 50%;
  left: 100%;
  transform: translateY(-50%);
  margin-left: 12px;
}
</style>

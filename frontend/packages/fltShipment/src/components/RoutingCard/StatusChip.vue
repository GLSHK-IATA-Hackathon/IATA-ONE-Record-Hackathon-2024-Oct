<script setup>
import { computed } from 'vue'
import { StatusEnum } from '@/enums/StatusEnum'
import { getStatusInfo } from '@/utils/shipment'

const props = defineProps({
  status: {
    validator(value) {
      return Object.values(StatusEnum).includes(value)
    },
  },
  statusDetail: { type: Object },
  latestMilestone: { type: Object },
  disableTooltip: { type: Boolean, default: false },
})

const statusInfo = computed(() =>
  getStatusInfo(props.status, props.latestMilestone)
)
</script>

<template>
  <div
    class="u-px-8px u-py-4px u-rounded-4px u-text-center u-text-12px u-fw-600"
    :style="statusInfo.dynamicStyle"
    :class="{ 'u-cursor-pointer!': !disableTooltip }"
  >
    <span>{{ statusInfo.label }}</span>

    <q-tooltip
      v-if="statusInfo.isTooltipInfoCompleted && !disableTooltip"
      class="u-w-310px"
      transition-show="scale"
      transition-hide="scale"
      anchor="bottom left"
      self="top left"
      :offset="[0, 10]"
    >
      <div class="u-text-14px">{{ statusInfo.tooltipInfo.eventLabel }}</div>
      <div class="u-text-12px">{{ statusInfo.tooltipInfo.timeLabel }}</div>
    </q-tooltip>
  </div>
</template>

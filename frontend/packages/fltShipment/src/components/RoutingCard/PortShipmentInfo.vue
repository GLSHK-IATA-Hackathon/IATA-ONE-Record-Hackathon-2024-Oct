<script setup>
import { computed } from 'vue'

import { hasValue } from '@/utils/misc'
import { eventMappingObject } from '@/constants/mappingObject'
import { getPiecesLabel, getWeightLabel } from '@/utils/shipment'
import GlsCustomIcon from '../GlsCustomIcon.vue'

const props = defineProps({
  portDetail: { type: Object, default: {} },
  weightCode: { type: String },
})

const isDataReady = computed(
  () =>
    hasValue(props.portDetail.event) &&
    hasValue(props.portDetail.pieces) &&
    hasValue(props.portDetail.weight)
)

const eventLabel = computed(() => eventMappingObject[props.portDetail.event])

const piecesLabel = computed(() => getPiecesLabel(props.portDetail.pieces))

const weightLabel = computed(() =>
  getWeightLabel(props.portDetail.weight, props.weightCode)
)
</script>

<template>
  <div
    v-if="isDataReady"
    class="u-flex u-items-center u-c-EzyBooking-dark-gray-1"
  >
    <GlsCustomIcon
      :name="portDetail?.isComplete ? 'Check' : 'Remove'"
      :width="12"
      :height="12"
      color="#189068"
      class="u-mr-4px"
    />
    <span class="u-mr-8px u-c-EzyBooking-green-5">{{ eventLabel }}</span>
    <span class="u-mr-4px">{{ piecesLabel }}</span>
    <q-separator
      vertical
      size="1px"
      color="EzyBooking-dark-gray-1"
      class="u-my-4px! u-mr-4px!"
    />
    <span>{{ weightLabel }}</span>
  </div>
</template>

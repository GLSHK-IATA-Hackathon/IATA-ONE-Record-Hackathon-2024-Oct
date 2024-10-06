<script setup>
import { computed } from 'vue'
import dayjs from 'dayjs'
import { symOutlinedError } from '@quasar/extras/material-symbols-outlined'

import { PortStatusEnum } from '@/enums/StatusEnum'
import { hasValue } from '@/utils/misc'
import { getPiecesLabel, getWeightLabel } from '@/utils/shipment'
import {
  eventMappingObject,
  discrepancyDDCodeMappingObject,
} from '@/constants/mappingObject'
import PortToolTip from '@/components/RoutingCard/PortToolTip.vue'

const props = defineProps({
  status: { type: String },
  isFirstPort: { type: Boolean, default: false },
  isLastPort: { type: Boolean, default: false },
  tooltipInfo: { type: Object, default: {} },
  weightCode: { type: String },
  disableTooltip: { type: Boolean, default: false },
})

const emits = defineEmits(['onDetailPageClick'])

const iconName = computed(() => {
  if (props.status === PortStatusEnum.COMPLETED) return 'check'
  else if (props.status === PortStatusEnum.IN_PROGRESS) return 'remove'
  else if (props.status === PortStatusEnum.ERROR) return 'priority_high'
})

const title = computed(() => {
  const label = eventMappingObject[props.tooltipInfo.event]

  return label
})

const occurrenceCount = computed(() => props.tooltipInfo.occurrenceCount)

const eventTime = computed(() => {
  return dayjs
    .unix(props.tooltipInfo.eventTime.seconds)
    .format('DD MMM YY HH:MM')
})

const contents = computed(() => {
  const { pieces, weight, osiLine1, osiLine2, ddCode } = props.tooltipInfo
  const piecesLabel = getPiecesLabel(pieces)
  const weightLabel = getWeightLabel(weight, props.weightCode)

  const line1 =
    hasValue(pieces) && hasValue(weight)
      ? `${piecesLabel} | ${weightLabel} occurred discrepancy`
      : ''

  const reason = [discrepancyDDCodeMappingObject[ddCode], osiLine1]
    .filter(hasValue)
    .join(' - ')
  const line2 = hasValue(reason) ? `Reason: ${reason}` : ''

  const [protectedFlight, departureTime] = osiLine2?.split('/') || []
  const convertedDepartureTime = hasValue(departureTime)
    ? dayjs(departureTime).format('DD MMM YY HH:ss')
    : ''
  const line3 = hasValue(protectedFlight)
    ? `Protected Flight: ${protectedFlight}`
    : ''
  const line4 = hasValue(convertedDepartureTime)
    ? `Departure Time: ${convertedDepartureTime}`
    : ''

  const result = [line1, line2, line3, line4].filter(hasValue)

  return result
})

const dynamicClass = computed(() => {
  let result =
    'u-absolute u-h-20px u-w-20px u-rounded-full u-mt--28px  u-flex u-items-center u-justify-center'

  if (props.isLastPort) result += ' u-right-0 u-mr-20px'
  else if (!props.isFirstPort) result += ' u-translate-x--50%'
  else if (props.isFirstPort) result += ' u-ml-20px'

  switch (props.status) {
    case PortStatusEnum.COMPLETED:
    case PortStatusEnum.IN_PROGRESS:
      result += ' u-bg-#359471'
      break
    case PortStatusEnum.ERROR:
      result += ' u-bg-EzyBooking-red'
      break
    case PortStatusEnum.NOT_STARTED:
    default:
      result += ' u-bg-EzyBooking-disable'
      break
  }

  return result
})
</script>

<template>
  <div :class="dynamicClass">
    <template v-if="hasValue(tooltipInfo) && !disableTooltip">
      <PortToolTip>
        <template #default>
          <q-icon :name="iconName" color="white" size="12px" />
        </template>

        <template #tooltip>
          <div
            class="u-w-320px u-pa-12px! u-rounded-4px u-bg-EzyBooking-red-4! u-c-black!"
          >
            <!-- title -->
            <div class="u-flex u-items-center u-mb-8px u-bg-#fee7e7">
              <q-icon
                class="u-mr-4px"
                :name="symOutlinedError"
                color="EzyBooking-red"
              />

              <span
                class="u-c-EzyBooking-red u-text-16px u-leading-18px u-fw-600"
              >
                {{
                  occurrenceCount > 1 ? `${title} (${occurrenceCount})` : title
                }}
              </span>

              <q-space />
              <LinkButton
                v-if="occurrenceCount > 1"
                class="u-mr--8px"
                label="View All"
                fontSize="14px"
                quasarIconRight="chevron_right"
                :fontWeight="600"
                iconSize="22px"
                @click="emits('onDetailPageClick')"
              />
            </div>

            <!-- contents -->
            <div class="u-flex">
              <div>
                <div
                  v-for="(content, index) in contents"
                  :key="index"
                  class="u-pl-20px u-text-12px u-leading-snug u-c-EzyBooking-dark-gray-1 u-leading-17px"
                >
                  {{ content }}
                </div>
              </div>
              <q-space />
              <span
                class="u-c-EzyBooking-dark-gray-1 u-text-12px u-leading-17px"
              >
                {{ eventTime }}
              </span>
            </div>
          </div>
        </template>
      </PortToolTip>
    </template>
    <template v-else>
      <q-icon :name="iconName" color="white" size="12px" />
    </template>
  </div>
</template>

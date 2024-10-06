<script setup>
import { computed } from 'vue'
import { PortBookingStatusEnum } from '@/enums/StatusEnum'
import GlsCustomIcon from './GlsCustomIcon.vue'

const props = defineProps({
  bookingInfo: { type: Object },
})

const statusLabel = computed(() => {
  switch (props.bookingInfo.status) {
    case PortBookingStatusEnum.CONFIRMED:
      return 'Confirmed'
    case PortBookingStatusEnum.NOT_CONFIRM:
      return 'Not Confirm'
    case PortBookingStatusEnum.BOOKING_RECEIVED:
      return 'Booking Received'
  }
})

const statusColor = computed(() => {
  switch (props.bookingInfo.status) {
    case PortBookingStatusEnum.CONFIRMED:
    case PortBookingStatusEnum.BOOKING_RECEIVED:
      return 'u-c-EzyBooking-green'
    case PortBookingStatusEnum.NOT_CONFIRM:
      return 'u-c-#ECB300'
  }
})

const statusIcon = computed(() => {
  switch (statusLabel.value) {
    case 'Confirmed':
      return {
        name: 'Check_circle',
        color: '#2DA659',
      }
    case 'Not Confirm':
      return {
        name: 'Alert_circle',
        color: '#ECB300',
      }
    default:
      return {
        name: '',
        color: '',
      }
  }
})
</script>

<template>
  <div
    class="u-rounded-md u-border-#d5d9d9 u-border-style-solid u-border-1px u-text-12px u-w-230px u-pa-8px"
  >
    <div
      class="u-rounded-md u-flex u-items-center u-mb-4px"
      :class="statusColor"
    >
      <span class="u-fw-600 u-flex u-items-center u-justify-center u-gap-2">
        <GlsCustomIcon
          :name="statusIcon.name"
          :width="16"
          :height="16"
          :color="statusIcon.color"
        />
        {{ statusLabel }}
      </span>
    </div>

    <div class="u-flex u-justify-between u-items-center u-mb-4px">
      <div class="u-flex u-justify-between u-items-center u-gap-4px">
        <GlsCustomIcon
          name="Flight_45degree"
          :width="12"
          :height="12"
          class="u-flex u-items-center u-justify-center u-m-auto"
          color="#556565"
        />
        <span>{{ bookingInfo.flightNo }}</span>
        <span>|</span>
        <span>{{ bookingInfo.eventTime }}</span>
      </div>
      <div class="u-flex u-justify-between u-items-center u-gap-4px">
        <span class="u-text-9px u-c-EzyBooking-dark-gray-1 u-fw-600">
          {{ bookingInfo.deliverType }}
        </span>
        <span>{{ bookingInfo.deliverTime }}</span>
      </div>
    </div>

    <div class="u-flex u-justify-between u-items-center">
      <div class="u-flex u-justify-between u-items-center u-gap-4px">
        <GlsCustomIcon
          name="Piece"
          :width="12"
          :height="12"
          class="u-flex u-items-center u-justify-center u-m-auto"
          fill-color="#556565"
        />
        <span>{{ bookingInfo.pieces }} pcs</span>
        <span>|</span>
        <span>{{ bookingInfo.weight }} kg</span>
      </div>
      <div class="u-flex u-justify-between u-items-center u-gap-4px">
        <span class="u-text-9px u-c-EzyBooking-dark-gray-1 u-fw-600">
          {{ bookingInfo.arriverType }}
        </span>
        <span>{{ bookingInfo.arriverTime }}</span>
      </div>
    </div>
  </div>
</template>

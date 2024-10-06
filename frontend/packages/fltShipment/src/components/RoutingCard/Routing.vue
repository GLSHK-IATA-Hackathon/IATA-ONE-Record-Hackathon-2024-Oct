<script setup>
import StatusCard from '@/components/StatusCard.vue'
import PortStatus from '@/components/RoutingCard/PortStatus.vue'
import PortShipmentInfo from '@/components/RoutingCard/PortShipmentInfo.vue'
import { hasValue } from '@/utils/misc'
import { StatusEnum, PortStatusEnum } from '@/enums/StatusEnum'
import GlsCustomIcon from '../GlsCustomIcon.vue'

const props = defineProps({
  shipmentDetail: { type: Object },
  shipment: { type: Object },
  isDetailViewMode: { type: Boolean, default: false },
})

const emits = defineEmits(['onDetailPageClick'])

function getPortTooltipInfo(portIndex) {
  const targetShipmentInfo = props.shipmentDetail.shipmentInfo[portIndex]
  const hasTooltipInfo = hasValue(targetShipmentInfo?.tooltipInfo)

  return hasTooltipInfo ? targetShipmentInfo.tooltipInfo : {}
}

function getPortStatus(portIndex) {
  const targetShipmentInfo = props.shipmentDetail.shipmentInfo[portIndex]

  const hasNextPort = portIndex === props.shipmentDetail.routings.length
  const isNextPortCompleted = hasNextPort
    ? false
    : getPortStatus(portIndex + 1) === PortStatusEnum.COMPLETED
  const isCompleted = isNextPortCompleted
    ? true
    : targetShipmentInfo?.firstLine?.pieces >= props.shipment.qdPieces &&
      targetShipmentInfo?.firstLine?.weight >= props.shipment.qdWeight &&
      targetShipmentInfo?.secondLine?.pieces >= props.shipment.qdPieces &&
      targetShipmentInfo?.secondLine?.weight >= props.shipment.qdWeight

  const isInProgress =
    targetShipmentInfo?.firstLine?.pieces > 0 ||
    targetShipmentInfo?.firstLine?.weight > 0 ||
    targetShipmentInfo?.secondLine?.pieces > 0 ||
    targetShipmentInfo?.secondLine?.weight > 0

  const currentPort = props.shipmentDetail.routings[portIndex]

  const isError =
    props.status === StatusEnum.DELIVERED
      ? false
      : props.shipmentDetail.groupedEventData?.['DIS']?.some(
          (i) => i.mdPort1 === currentPort
        )

  if (isError) return PortStatusEnum.ERROR
  else if (isCompleted) return PortStatusEnum.COMPLETED
  else if (isInProgress) return PortStatusEnum.IN_PROGRESS
  else return PortStatusEnum.NOT_STARTED
}

function getLineBackground(index) {
  const nextPortStatus = getPortStatus(index + 1)
  switch (nextPortStatus) {
    case PortStatusEnum.COMPLETED:
    case PortStatusEnum.IN_PROGRESS:
    case PortStatusEnum.ERROR:
      return 'u-bg-#359471'
    case PortStatusEnum.NOT_STARTED:
    default:
      return 'u-bg-#DDE0E0'
  }
}
</script>

<template>
  <div v-if="shipmentDetail.isFetching">
    <div class="u-flex u-items-center u-w-full u-mt-16px">
      <q-skeleton type="circle" size="20px" />
      <q-skeleton class="u-flex-1" height="4px" square />
      <q-skeleton type="circle" size="20px" />
    </div>

    <div class="u-flex u-items-center u-justify-between u-my-16px">
      <div class="u-flex u-flex-col u-items-start u-gap-8px">
        <q-skeleton width="60px" height="22px" />
        <q-skeleton width="200px" height="12px" />
        <q-skeleton width="200px" height="12px" />
      </div>
      <div class="u-flex u-flex-col u-items-end u-gap-8px">
        <q-skeleton width="60px" height="22px" />
        <q-skeleton width="200px" height="12px" />
        <q-skeleton width="200px" height="12px" />
      </div>
    </div>
  </div>
  <div
    v-else
    class="u-py-16px u-grid u-text-14px"
    :class="`u-grid-cols-${shipmentDetail.lines}`"
  >
    <!-- detail page info type2  -->
    <template v-if="shipmentDetail.infoType === 1 && isDetailViewMode">
      <div
        v-for="(bookingItems, index) in shipmentDetail.bookingInfo"
        :key="index"
        class="u-justify-self-center u-items-end u-self-end u-grid u-gap-8px"
      >
        <div v-for="(item, index) in bookingItems" :key="index">
          <StatusCard :booking-info="item" />
        </div>
        <div
          class="status-card-divider u-w-1px u-h-18px u-bg-EzyBooking-dark-gray-3 u-m-auto"
        ></div>
        <GlsCustomIcon
          name="Flight"
          :width="16"
          :height="14"
          class="u-flex u-items-center u-justify-center u-m-auto u-mb-4"
        />
      </div>
    </template>

    <!-- line -->
    <div
      v-for="(_, index) in shipmentDetail.lines"
      :key="index"
      class="u-h-4px u-relative u-mb-16px"
      :class="[
        'u-h-4px u-relative u-mb-16px',
        getLineBackground(index),
        {
          'u-ml-40px': index === 0,
          'u-mr-40px': index === shipmentDetail.lines - 1,
        },
      ]"
    ></div>

    <!-- info type1 -->
    <div
      v-for="(_, index) in shipmentDetail.lines"
      :key="index"
      class="u-relative u-flex u-justify-between"
    >
      <template v-if="index === 0">
        <!-- origin port -->
        <PortStatus
          :status="getPortStatus(index)"
          is-first-port
          :tooltip-info="getPortTooltipInfo(index)"
          :weight-code="shipmentDetail.weightCode"
          @on-detail-page-click="emits('onDetailPageClick')"
          :disable-tooltip="isDetailViewMode"
        />
        <div>
          <div class="u-c-EzyBooking-green u-text-16px u-fw600 u-pl-17px">
            {{ shipmentDetail.routings[index] }}
          </div>
          <template v-if="shipmentDetail.infoType === 1">
            <PortShipmentInfo
              :port-detail="shipmentDetail.shipmentInfo[0]?.firstLine"
              :weight-code="shipmentDetail.weightCode"
            />
            <PortShipmentInfo
              :port-detail="shipmentDetail.shipmentInfo[0]?.secondLine"
              :weight-code="shipmentDetail.weightCode"
            />
          </template>
        </div>
      </template>
      <template v-else>
        <!-- middle port -->
        <PortStatus
          :status="getPortStatus(index)"
          :tooltip-info="getPortTooltipInfo(index)"
          :weight-code="shipmentDetail.weightCode"
          @on-detail-page-click="emits('onDetailPageClick')"
          :disable-tooltip="isDetailViewMode"
        />
        <template v-if="true">
          <div class="u-translate-x--50%">
            <div class="u-text-center u-c-EzyBooking-green u-text-16px u-fw600">
              {{ shipmentDetail.routings[index] }}
            </div>
            <template v-if="shipmentDetail.infoType === 1">
              <PortShipmentInfo
                :tooltip-info="getPortTooltipInfo(index)"
                :port-detail="shipmentDetail.shipmentInfo[index]?.firstLine"
                :weight-code="shipmentDetail.weightCode"
              />
              <PortShipmentInfo
                :tooltip-info="getPortTooltipInfo(index)"
                :port-detail="shipmentDetail.shipmentInfo[index]?.secondLine"
                :weight-code="shipmentDetail.weightCode"
              />
            </template>
          </div>
        </template>
      </template>

      <template v-if="index === shipmentDetail.lines - 1">
        <!-- destination port -->
        <PortStatus
          :status="getPortStatus(index + 1)"
          :tooltip-info="getPortTooltipInfo(index + 1)"
          is-last-port
          :weight-code="shipmentDetail.weightCode"
          @on-detail-page-click="emits('onDetailPageClick')"
          :disable-tooltip="isDetailViewMode"
        />
        <div class="u-flex u-items-end u-flex-col">
          <div
            class="u-text-right u-c-EzyBooking-green u-text-16px u-fw600 u-pr-20px"
          >
            {{ shipmentDetail.routings[index + 1] }}
          </div>
          <template v-if="shipmentDetail.infoType === 1">
            <PortShipmentInfo
              :port-detail="shipmentDetail.shipmentInfo[index + 1]?.firstLine"
              :weight-code="shipmentDetail.weightCode"
            />
            <PortShipmentInfo
              :port-detail="shipmentDetail.shipmentInfo[index + 1]?.secondLine"
              :weight-code="shipmentDetail.weightCode"
            />
          </template>
        </div>
      </template>
    </div>

    <!-- info type2 -->
    <template v-if="shipmentDetail.infoType === 2">
      <div
        v-for="(bookingItems, index) in shipmentDetail.bookingInfo"
        :key="index"
        class="u-justify-self-center u-grid u-gap-8px"
      >
        <div v-for="(item, index) in bookingItems" :key="index">
          <StatusCard :booking-info="item" />
        </div>
      </div>
    </template>
  </div>
</template>

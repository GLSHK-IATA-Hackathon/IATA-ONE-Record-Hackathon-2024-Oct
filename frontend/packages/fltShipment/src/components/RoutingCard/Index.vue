<script setup>
import { useRouter } from 'vue-router'
import {
  symOutlinedPackage2,
  symOutlinedWeight,
} from '@quasar/extras/material-symbols-outlined'

import StatusChip from '@/components/RoutingCard/StatusChip.vue'
import Routing from '@/components/RoutingCard/Routing.vue'
import BookmarkButton from '@/components/BookmarkButton.vue'
import { DetailTypeEnum } from '@/enums/CommonEnum'
import { useShipmentStore } from '@/stores/shipment'
import GlsCustomIcon from '../GlsCustomIcon.vue'

const shipmentStore = useShipmentStore()
const { toggleBookmark } = shipmentStore

const router = useRouter()

const props = defineProps({
  shipment: { type: Object, required: true },
  shipmentDetail: { type: Object },
  searchResultType: { type: String },
})

function toDetailPage() {
  router.push({
    name: 'detailPage',
    params: {
      searchResultType: props.searchResultType,
      awbNumber: props.shipmentDetail.awbNumber,
      detailType: DetailTypeEnum.OVERVIEW,
    },
  })
}
</script>

<template>
  <div
    class="u-border-1 u-rounded-4px u-border-solid u-border-EzyBooking-light-green-2 u-px-16px u-py-8px"
  >
    <!-- header (basic information) -->
    <div class="u-flex u-items-center u-h-40px u-c-EzyBooking-black-2">
      <template v-if="shipmentDetail.isFetching">
        <div class="u-flex u-items-center u-w-full">
          <q-skeleton class="u-mr-20px" width="160px" height="20px" />
          <q-skeleton class="u-mr-24px" width="60px" height="20px" />
          <q-skeleton class="u-mr-24px" width="60px" height="20px" />
          <q-skeleton class="u-mr-24px" width="60px" height="20px" />
          <q-skeleton class="u-mr-24px" width="60px" height="20px" />
          <q-space />
          <q-skeleton width="90px" height="20px" />
        </div>
      </template>
      <template v-else>
        <!-- awb number -->
        <div class="u-fw-700 u-text-20px u-mr-4px">
          {{ shipmentDetail.awbNumber }}
        </div>

        <!-- isBookmarked -->
        <BookmarkButton
          class="u-mr-20px"
          :is-bookmarked="shipmentDetail.isBookmarked"
          @on-bookmark-click="
            () =>
              toggleBookmark(
                shipmentDetail.awbNumber,
                shipmentDetail.destination
              )
          "
        />

        <!-- status -->
        <StatusChip
          class="u-mr-24px"
          :status="shipmentDetail.status"
          :latest-milestone="shipmentDetail.latestMilestone"
        />

        <!-- origin & destination -->
        <div class="u-text-14px u-mr-18px u-flex u-items-center">
          <span>{{ props.shipmentDetail.origin }}</span>
          <GlsCustomIcon
            class=""
            name="Chevron_right"
            :width="16"
            :height="16"
            color="#556565"
          />
          <span>{{ props.shipmentDetail.destination }}</span>
        </div>

        <!-- pieces -->
        <div class="u-text-14px u-mr-4 u-flex u-items-center">
          <GlsCustomIcon
            class="u-mr-6px"
            name="Piece"
            fill-color="#B2BABA"
            :width="16"
            :height="16"
          />
          {{ shipmentDetail.piecesLabel }}
        </div>

        <!-- weight -->
        <div class="u-text-14px u-mr-4 u-flex u-items-center">
          <GlsCustomIcon
            class="u-mr-6px"
            name="Weight"
            fill-color="#B2BABA"
            :width="16"
            :height="16"
          />
          {{ shipmentDetail.weightLabel }}
        </div>
        <q-space />

        <!-- view more -->
        <LinkButton
          label="View More"
          fontSize="16px"
          quasarIconRight="chevron_right"
          :fontWeight="600"
          iconSize="22px"
          @click="toDetailPage"
        />
      </template>
    </div>

    <q-separator class="u-my-8px!" />

    <!-- routing -->
    <Routing
      :shipment-detail="shipmentDetail"
      :shipment="shipment"
      @on-detail-page-click="toDetailPage"
    />
  </div>
</template>

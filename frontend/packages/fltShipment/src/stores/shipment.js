import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { v4 as uuidv4 } from 'uuid'
import { Notify } from 'quasar'
import { roundErrorOutline } from '@quasar/extras/material-icons-round'
import dayjs from 'dayjs'

import trackAndTraceResponse from '@/data/trackAndTraceResponse'
import tntApi from '@/apis/tntApi'
import {
  hasValue,
  sleep,
  validateAwbNumberFormat,
  validateAwbSuffix,
} from '@/utils/misc'
import { getComputedShipmentDetail } from '@/utils/shipment'

export const useShipmentStore = defineStore(
  'shipment',
  () => {
    const debugConfig = ref({
      isForceShowBookingInfo: false,
      isSkeleton: false,
    })
    /**
     * [
     *  {
     *    label: String, // '160-12345678'
     *    isEditing: Boolean,
     *    id: uuid,
     *  }
     * ]
     */
    const searchList = ref([])

    const bookmarkList = ref([])

    /**
     * structure
     *
     * shipmentDetailForBookmarkList: {
     *    awb: {
     *      data: Object, // same as fetchShipmentDetail API response
     *      isFetching: Boolean,
     *    }
     * }
     */
    const shipmentDetailForBookmarkList = ref({})

    const shipmentDetailForSearchList = ref({})

    const searchHistory = ref([])

    const lastUpdateInfoForBookmarkList = ref({
      lastUpdatedTime: null,
      isFetching: computed(() =>
        Object.values(shipmentDetailForBookmarkList.value).some(
          (item) => item.isFetching
        )
      ),
    })

    const lastUpdateInfoForSearchList = ref({
      lastUpdatedTime: null,
      isFetching: computed(() =>
        Object.values(shipmentDetailForSearchList.value).some(
          (item) => item.isFetching
        )
      ),
    })

    const isSearchListCapped = computed(() => searchList.value.length >= 10)

    const computedShipmentDetailForBookmarkList = computed(() =>
      getComputedShipmentDetail(
        shipmentDetailForBookmarkList.value,
        bookmarkList.value,
        debugConfig.value
      )
    )

    const listOfNoRecordBookmark = computed(() => {
      return Object.keys(shipmentDetailForBookmarkList.value).reduce(
        (acc, cur) => {
          if (!computedShipmentDetailForBookmarkList.value.hasOwnProperty(cur))
            acc.push(cur)
          return acc
        },
        []
      )
    })

    const computedShipmentDetailForSearchList = computed(() =>
      getComputedShipmentDetail(
        shipmentDetailForSearchList.value,
        bookmarkList.value,
        debugConfig.value
      )
    )

    const listOfNoRecordShipment = computed(() => {
      return Object.keys(shipmentDetailForSearchList.value).reduce(
        (acc, cur) => {
          if (!computedShipmentDetailForSearchList.value.hasOwnProperty(cur))
            acc.push(cur)
          return acc
        },
        []
      )
    })

    function addAwbToSearchList(awbNumber) {
      if (isSearchListCapped.value) return

      const [_awbPrefix, awbSuffix] = awbNumber.split('-')
      const isAwbNumberFormatValid = validateAwbNumberFormat(awbNumber)
      const isAwbSuffixValid = validateAwbSuffix(awbSuffix)

      if (searchList.value.some((item) => item.awbNumber === awbNumber)) {
        Notify.create({
          message: `${awbNumber} is already in search list`,
          icon: roundErrorOutline,
          iconColor: 'EzyBooking-red',
          badgeClass: 'hidden',
          position: 'top',
          color: 'EzyBooking-red-4',
          textColor: 'EzyBooking-gray-2',
        })

        return
      } else if (!isAwbNumberFormatValid || !isAwbSuffixValid) {
        Notify.create({
          message: `${awbNumber} is not a valid AWB number`,
          icon: roundErrorOutline,
          iconColor: 'EzyBooking-red',
          badgeClass: 'hidden',
          position: 'top',
          color: 'EzyBooking-red-4',
          textColor: 'EzyBooking-gray-2',
        })
      }

      searchList.value.push({
        awbNumber,
        isEditing: false,
        id: uuidv4(),
      })
    }

    function removeAwbFromSearchList(id) {
      searchList.value = searchList.value.filter((item) => item.id !== id)
    }

    async function setSearchItemToEditing(awbNumber) {
      const targetSearchItem = searchList.value.find(
        (item) => item.awbNumber === awbNumber
      )
      targetSearchItem.isEditing = true
    }

    function onEditingBubbleEnter(item) {
      const { awbNumber, id } = item
      const [_awbPrefix, awbSuffix] = awbNumber.split('-')
      const isAwbNumberFormatValid = validateAwbNumberFormat(awbNumber)
      const isAwbSuffixValid = validateAwbSuffix(awbSuffix)

      if (!hasValue(awbNumber)) {
        removeAwbFromSearchList(id)
        return
      } else if (
        searchList.value
          .filter((item) => item.id !== id)
          .some((item) => item.awbNumber === awbNumber)
      ) {
        Notify.create({
          message: `${awbNumber} is already in search list`,
          icon: roundErrorOutline,
          iconColor: 'EzyBooking-red',
          badgeClass: 'hidden',
          position: 'top',
          color: 'EzyBooking-red-4',
          textColor: 'EzyBooking-gray-2',
        })
        removeAwbFromSearchList(id)
        return
      } else if (!isAwbNumberFormatValid || !isAwbSuffixValid) {
        Notify.create({
          message: `${awbNumber} is not a valid AWB number`,
          icon: roundErrorOutline,
          iconColor: 'EzyBooking-red',
          badgeClass: 'hidden',
          position: 'top',
          color: 'EzyBooking-red-4',
          textColor: 'EzyBooking-gray-2',
        })
      }

      item.isEditing = false
    }

    function popSearchList() {
      searchList.value.pop()
    }

    function clearSearchList() {
      searchList.value = []
    }

    // call this function for bookmarkList, searchList, detailPage
    async function fetchShipment(awbPrefix, awbSuffix) {
      try {
        const params = {
          awbPrefix,
          awbSuffix,
        }
        const response = await tntApi.fetchShipmentDetail(params)

        return response
      } catch (error) {
        console.error(error)
      }
    }

    async function fetchBookmarkList() {
      try {
        const {
          data: { awbs },
        } = await tntApi.fetchBookmarkList()
        bookmarkList.value = awbs
      } catch (error) {
        console.error(error)
      }
    }

    const getRandomNum = (min, max) => {
      const range = max - min - 1
      const rand = Math.random()

      return min + Math.floor(rand * range)
    }

    async function fetchBookmarkShipmentList() {
      try {
        // 1. fetch bookmark list
        await fetchBookmarkList()

        const latestTenBookmarkAwbNo = bookmarkList.value.slice(-10)
        if (hasValue(latestTenBookmarkAwbNo)) {
          lastUpdateInfoForBookmarkList.value.lastUpdatedTime =
            dayjs().valueOf()
          shipmentDetailForBookmarkList.value = {}
          latestTenBookmarkAwbNo.forEach(async (awbNo) => {
            shipmentDetailForBookmarkList.value[awbNo] = {
              data: {},
              isFetching: true,
            }

            const [awbPrefix, awbSuffix] = awbNo.split('-')
            try {
              const response = await fetchShipment(awbPrefix, awbSuffix)
              // const randomNum = getRandomNum(1000, 3000)
              // await sleep(randomNum)
              shipmentDetailForBookmarkList.value[awbNo].data = response.data
            } catch (error) {
              console.error(error)
            } finally {
              shipmentDetailForBookmarkList.value[awbNo].isFetching = false
            }
          })
        } else {
          console.log('no bookmark list')
        }
      } catch (error) {
        console.error(error)
      }
    }

    async function fetchShipmentListWithSearchBar() {
      const awbNumbers = searchList.value
        .map((item) => item.awbNumber)
        .filter((awbNumber) => validateAwbNumberFormat(awbNumber))

      shipmentDetailForSearchList.value = {}
      lastUpdateInfoForSearchList.value.lastUpdatedTime = dayjs().valueOf()
      awbNumbers.forEach(async (awbNo) => {
        shipmentDetailForSearchList.value[awbNo] = {
          data: {},
          isFetching: true,
        }

        const [awbPrefix, awbSuffix] = awbNo.split('-')
        try {
          const response = await fetchShipment(awbPrefix, awbSuffix)
          shipmentDetailForSearchList.value[awbNo].data = response.data
        } catch (error) {
          console.error(error)
        } finally {
          shipmentDetailForSearchList.value[awbNo].isFetching = false
        }
      })
    }

    async function refreshBookmarkShipmentList() {
      lastUpdateInfoForBookmarkList.value.lastUpdatedTime = dayjs().valueOf()
      Object.entries(shipmentDetailForBookmarkList.value).forEach(
        async (item) => {
          const [awbNumber, shipmentDetail] = item
          try {
            shipmentDetail.isFetching = true
            const [awbPrefix, awbSuffix] = awbNumber.split('-')
            const response = await fetchShipment(awbPrefix, awbSuffix)
            shipmentDetail.data = response.data
          } catch (error) {
            console.error(error)
          } finally {
            shipmentDetail.isFetching = false
          }
        }
      )
    }

    async function refreshShipmentListForSearchBar() {
      lastUpdateInfoForSearchList.value.lastUpdatedTime = dayjs().valueOf()
      Object.entries(shipmentDetailForSearchList.value).forEach(
        async (item) => {
          const [awbNumber, shipmentDetail] = item
          try {
            shipmentDetail.isFetching = true
            const [awbPrefix, awbSuffix] = awbNumber.split('-')
            const response = await fetchShipment(awbPrefix, awbSuffix)
            shipmentDetail.data = response.data
          } catch (error) {
            console.error(error)
          } finally {
            shipmentDetail.isFetching = false
          }
        }
      )
    }

    async function toggleBookmark(awbNumber, destination) {
      try {
        const [awbPrefix, awbSuffix] = awbNumber.split('-')
        const params = { awbPrefix, awbSuffix, port: destination }
        await tntApi.toggleBookmark(params)

        if (bookmarkList.value.includes(awbNumber)) {
          bookmarkList.value = bookmarkList.value.filter(
            (bookmarkedAwbNumber) => bookmarkedAwbNumber !== awbNumber
          )
        } else {
          bookmarkList.value.push(awbNumber)
        }
      } catch (error) {
        console.error(error)
      }
    }

    async function fetchSearchHistory() {
      try {
        const {
          data: { recentSearch },
        } = await tntApi.fetchSearchHistory()
        searchHistory.value = recentSearch
      } catch (error) {
        console.error(error)
      }
    }

    async function addSearchHistory() {
      const awbNumbers = searchList.value
        .map((item) => item.awbNumber)
        .filter((awbNumber) => validateAwbNumberFormat(awbNumber))

      if (!hasValue(awbNumbers)) return

      try {
        const params = { awbs: awbNumbers }
        await tntApi.addSearchHistory(params)
      } catch (error) {
        console.error(error)
      }
    }

    function cancelFetchingStatusAfterRestore() {
      Object.values(shipmentDetailForBookmarkList.value).forEach(
        (item) => (item.isFetching = false)
      )
      Object.values(shipmentDetailForSearchList.value).forEach(
        (item) => (item.isFetching = false)
      )
    }

    return {
      // data
      debugConfig,
      bookmarkList,
      searchList,
      isSearchListCapped,
      shipmentDetailForBookmarkList,
      shipmentDetailForSearchList,
      computedShipmentDetailForBookmarkList,
      computedShipmentDetailForSearchList,
      listOfNoRecordBookmark,
      listOfNoRecordShipment,
      searchHistory,
      lastUpdateInfoForBookmarkList,
      lastUpdateInfoForSearchList,

      // method
      fetchBookmarkShipmentList,
      fetchShipment,
      toggleBookmark,
      addAwbToSearchList,
      removeAwbFromSearchList,
      clearSearchList,
      popSearchList,
      setSearchItemToEditing,
      onEditingBubbleEnter,
      fetchShipmentListWithSearchBar,
      fetchSearchHistory,
      addSearchHistory,
      refreshBookmarkShipmentList,
      refreshShipmentListForSearchBar,
      cancelFetchingStatusAfterRestore,
    }
  },
  {
    persist: {
      paths: [
        'bookmarkList',
        'searchList',
        'shipmentDetailForBookmarkList',
        'shipmentDetailForSearchList',
        'lastUpdateInfoForBookmarkList.lastUpdatedTime',
        'lastUpdateInfoForSearchList.lastUpdatedTime',
      ],
      afterRestore: (ctx) => {
        ctx.store.cancelFetchingStatusAfterRestore()
      },
    },
  }
)

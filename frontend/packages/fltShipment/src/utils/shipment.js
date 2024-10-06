import * as R from 'ramda'
import dayjs from 'dayjs'

import { StatusEnum, PortBookingStatusEnum } from '@/enums/StatusEnum'
import { hasValue } from './misc'
import { weightCodeMappingObject } from '@/constants/mappingObject'
import { eventMappingObject } from '@/constants/mappingObject'

export function getGroupedEventData(shipmentDetail) {
  if (!hasValue(shipmentDetail.freightStatusDetails)) return {}

  return shipmentDetail.freightStatusDetails.reduce((arr, cur) => {
    const { statusCode } = cur

    if (arr.hasOwnProperty(statusCode)) {
      arr[statusCode].push(cur)
    } else {
      arr[statusCode] = [cur]
    }

    return arr
  }, {})
}

export function getRoutings(groupedEventData) {
  const bookingData = groupedEventData?.['BKD']

  const portsList =
    bookingData?.reduce((acc, cur) => {
      const { mdPort1, mdPort2 } = cur

      const icCurrentPortExist = acc.some((port) => {
        const [oldPort1, oldPort2] = port
        return oldPort1 === mdPort1 && oldPort2 === mdPort2
      })

      if (!icCurrentPortExist) {
        acc.push([mdPort1, mdPort2])
      }

      return acc
    }, []) ?? []

  const routings = [...new Set(portsList?.flat())]

  return routings
}

export function getLatestMilestone(shipmentDetail) {
  return shipmentDetail?.freightStatusDetails?.at(-1)
}

export function getStatusCodes(groupedEventData) {
  return Object.keys(groupedEventData)
}

export function checkIsDelivered(shipmentDetail, groupedEventData) {
  const { qdPieces, qdWeight } = shipmentDetail
  const summaryOfDLV = groupedEventData?.['DLV']?.reduce(
    (acc, cur) => {
      const { qdPieces, qdWeight } = cur
      acc.piece += qdPieces
      acc.weight += qdWeight

      return acc
    },
    { piece: 0, weight: 0 }
  )

  const summaryOfDDL = groupedEventData?.['DDL']?.reduce(
    (acc, cur) => {
      const { qdPieces, qdWeight } = cur
      acc.piece += qdPieces
      acc.weight += qdWeight

      return acc
    },
    { piece: 0, weight: 0 }
  )

  if (
    (summaryOfDLV?.piece === qdPieces && summaryOfDLV?.weight === qdWeight) ||
    (summaryOfDDL?.piece === qdPieces && summaryOfDDL?.weight === qdWeight)
  ) {
    return true
  } else {
    return false
  }
}

export function checkIsDelivering(shipmentDetail, groupedEventData, routings) {
  const { qdPieces, qdWeight } = shipmentDetail
  // only for last port, maybe rename it?
  const summaryOfArr = groupedEventData?.['ARR']
    ?.filter((i) => {
      return i.mdPort1 === routings?.at(-1)
    })
    ?.reduce(
      (acc, cur) => {
        const { qdPieces, qdWeight } = cur
        acc.piece += qdPieces
        acc.weight += qdWeight
        return acc
      },
      { piece: 0, weight: 0 }
    )

  if (summaryOfArr?.piece === qdPieces && summaryOfArr?.weight === qdWeight) {
    return true
  } else {
    return false
  }
}

export function checkIsInTransit(groupedEventData, routings) {
  return groupedEventData?.['DEP']?.some((i) => i.mdPort1 === routings?.at(0))
}

export function checkIsCargoAcceptance(groupedEventData, routings) {
  const isFirstPortHasFOHEvent = groupedEventData?.['FOH']?.some(
    (i) => i.mdPort1 === routings?.at(0)
  )
  const isFirstPortHasRCSEvent = groupedEventData?.['RCS']?.some(
    (i) => i.mdPort1 === routings?.at(0)
  )

  return isFirstPortHasFOHEvent || isFirstPortHasRCSEvent
}

export function getStatus(
  isDelivered,
  isDelivering,
  isInTransit,
  isCargoAcceptance
) {
  if (isDelivered) return StatusEnum.DELIVERED
  else if (isDelivering) return StatusEnum.DELIVERING
  else if (isInTransit) return StatusEnum.IN_TRANSIT
  else if (isCargoAcceptance) return StatusEnum.CARGO_ACCEPTANCE
  else return StatusEnum.BOOKING_RECEIVED
}

export function getPieces(shipmentDetail) {
  return shipmentDetail.qdPieces
}

export function getWeight(shipmentDetail) {
  return shipmentDetail.qdWeight
}

export function getWeightCode(shipmentDetail) {
  return shipmentDetail.qdWeightCode
}

export function getPiecesLabel(pieces) {
  return pieces > 1 ? `${pieces} pcs` : `${pieces} pc`
}

export function getWeightLabel(weight, weightCode) {
  const weightUnitLabel = weightCodeMappingObject[weightCode]

  return `${weight} ${weightUnitLabel}`
}

export function getPoints(routings) {
  return routings.length
}

export function getLines(points) {
  return points - 1
}

export function getShipmentInfo(
  statusCodes,
  groupedEventData,
  routings,
  totalPieces,
  totalWeight
) {
  let result = []

  if (statusCodes.includes('RCS')) {
    const eventsWithFOH = groupedEventData['RCS']
    const pieces = eventsWithFOH[0].qdPieces
    const weight = eventsWithFOH[0].qdWeight
    result[0] = {
      firstLine: {
        event: 'RCS',
        pieces,
        weight,
      },
    }
  } else if (statusCodes.includes('FOH')) {
    const eventsWithFOH = groupedEventData['FOH']
    const pieces = eventsWithFOH[0].qdPieces
    const weight = eventsWithFOH[0].qdWeight
    result[0] = {
      firstLine: {
        event: 'FOH',
        pieces,
        weight,
      },
    }
  }

  if (statusCodes.includes('DLV')) {
    const pieces = groupedEventData['DLV'].reduce((acc, cur) => {
      acc += cur.qdPieces
      return acc
    }, 0)
    const weight = groupedEventData['DLV'].reduce((acc, cur) => {
      acc += cur.qdWeight
      return acc
    }, 0)
    result[routings.length - 1] = {
      secondLine: {
        event: 'DLV',
        pieces,
        weight,
      },
    }
  }

  if (statusCodes.includes('DDL')) {
    const pieces = groupedEventData['DDL'].reduce((acc, cur) => {
      acc += cur.qdPieces
      return acc
    }, 0)
    const weight = groupedEventData['DDL'].reduce((acc, cur) => {
      acc += cur.qdWeight
      return acc
    }, 0)
    result[routings.length - 1] = {
      secondLine: {
        event: 'DDL',
        pieces,
        weight,
      },
    }
  }

  if (statusCodes.includes('DEP')) {
    const eventsWithDEP = groupedEventData['DEP']
    eventsWithDEP.forEach((event) => {
      const { mdPort1, qdPieces, qdWeight } = event
      if (mdPort1 === routings?.[0]) {
        result[0].secondLine = {
          event: 'DEP',
          pieces: qdPieces,
          weight: qdWeight,
        }
      } else if (routings.includes(mdPort1)) {
        const indexOfRoutings = routings.findIndex(
          (routing) => routing === mdPort1
        )

        const previousItem = result[indexOfRoutings]
        if (hasValue(previousItem)) {
          result[indexOfRoutings].secondLine = {
            event: 'DEP',
            pieces: previousItem.secondLine.pieces + qdPieces,
            weight: previousItem.secondLine.weight + qdWeight,
          }
        } else {
          result[indexOfRoutings] = {
            secondLine: {
              event: 'DEP',
              pieces: qdPieces,
              weight: qdWeight,
            },
          }
        }
      }
    })
  }

  if (statusCodes.includes('ARR')) {
    const eventsWithARR = groupedEventData['ARR']
    eventsWithARR.forEach((event) => {
      const { mdPort1, qdPieces, qdWeight } = event
      if (mdPort1 === routings?.[0]) {
        result[0].firstLine = {
          event: 'ARR',
          pieces: qdPieces,
          weight: qdWeight,
        }
      } else if (routings.includes(mdPort1)) {
        const indexOfRoutings = routings.findIndex(
          (routing) => routing === mdPort1
        )

        const previousItem = result[indexOfRoutings]
        if (hasValue(previousItem)) {
          const nextPieces = R.is(Number, previousItem?.firstLine?.pieces)
            ? previousItem?.firstLine?.pieces + qdPieces
            : qdPieces
          const nextWeight = R.is(Number, previousItem?.firstLine?.weight)
            ? previousItem?.firstLine?.weight + qdWeight
            : qdWeight
          result[indexOfRoutings].firstLine = {
            event: 'ARR',
            pieces: nextPieces,
            weight: nextWeight,
          }
        } else {
          result[indexOfRoutings] = {
            firstLine: {
              event: 'ARR',
              pieces: qdPieces,
              weight: qdWeight,
            },
          }
        }
      }
    })
  }

  if (statusCodes.includes('DIS')) {
    const eventsWithDIS = groupedEventData['DIS']
    eventsWithDIS.reverse().forEach((event) => {
      const {
        mdPort1,
        qdPieces,
        qdWeight,
        mdDate,
        osiLine1,
        osiLine2,
        ddCode,
      } = event

      const indexOfRoutings = routings.findIndex(
        (routing) => routing === mdPort1
      )

      const previousItem = result[indexOfRoutings]

      if (hasValue(previousItem)) {
        result[indexOfRoutings].tooltipInfo = {
          event: 'DIS',
          pieces: qdPieces,
          weight: qdWeight,
          eventTime: mdDate,
          osiLine1,
          osiLine2,
          ddCode,
          occurrenceCount: hasValue(previousItem.tooltipInfo?.occurrenceCount)
            ? previousItem.tooltipInfo.occurrenceCount + 1
            : 1,
        }
      } else {
        result[indexOfRoutings] = {
          tooltipInfo: {
            event: 'DIS',
            pieces: qdPieces,
            weight: qdWeight,
            eventTime: mdDate,
            osiLine1,
            osiLine2,
            ddCode,
            occurrenceCount: 1,
          },
        }
      }
    })
  }

  const checkIsComplete = (lineInfo, totalPieces, totalWeight) =>
    lineInfo.pieces >= totalPieces && lineInfo.weight >= totalWeight

  result = result.map((item) => {
    if (item.hasOwnProperty('firstLine')) {
      item.firstLine.isComplete = checkIsComplete(
        item.firstLine,
        totalPieces,
        totalWeight
      )
    }

    if (item.hasOwnProperty('secondLine')) {
      item.secondLine.isComplete = checkIsComplete(
        item.secondLine,
        totalPieces,
        totalWeight
      )
    }
    return item
  })

  return result
}

export function getInfoType(shipmentInfo) {
  // todo: use Enum
  return hasValue(shipmentInfo) ? 1 : 2
}

export function getBookingInfo(routings, groupedEventData, awbPrefix) {
  return routings?.slice(0, routings?.length - 1).map((routing) => {
    const matchedBookingEvents = groupedEventData?.['BKD'].filter((event) => {
      const { mdPort1 } = event
      return mdPort1 === routing
    })
    const result = matchedBookingEvents.map((event) => {
      const {
        mdCarrierCode,
        mdFlightNum,
        mdDate,
        dtTime,
        dtType,
        atTime,
        atType,
        qdPieces,
        qdWeight,
      } = event

      const status = getPortBookingStatus(routing, awbPrefix, groupedEventData)

      return {
        status,
        flightNo: `${mdCarrierCode}${mdFlightNum}`,
        eventTime: dayjs.unix(mdDate.seconds).format('DD MMM'),
        deliverTime: dayjs.unix(dtTime.seconds).format('DD MMM HH:mm'),
        deliverType: `${dtType}TD`,
        arriverTime: dayjs.unix(atTime.seconds).format('DD MMM HH:mm'),
        arriverType: `${atType}TA`,
        pieces: qdPieces,
        weight: qdWeight,
      }
    })

    return result
  })
}

function getPortBookingStatus(port, awbPrefix, groupedEventData) {
  if (awbPrefix === '160') {
    const OSIEvents = groupedEventData['OSI']
    const isConfirmed = checkConfirmed(port, OSIEvents)

    return isConfirmed
      ? PortBookingStatusEnum.CONFIRMED
      : PortBookingStatusEnum.NOT_CONFIRM
  } else {
    return PortBookingStatusEnum.BOOKING_RECEIVED
  }
}

function checkConfirmed(port, OSIEvents = []) {
  return OSIEvents.some((event) => {
    const { osiLine1 } = event
    const pattern = new RegExp(`${port} TO .* Confirmed`)
    const isConfirmed = pattern.test(osiLine1)

    return isConfirmed
  })
}

export function checkIsBookmarked(awbNumber, bookmarkList) {
  return bookmarkList.includes(awbNumber)
}

export function getComputedShipmentDetail(
  shipmentDetail,
  bookmarkList,
  debugConfig
) {
  return Object.fromEntries(
    Object.entries(shipmentDetail)
      .map((shipment) => {
        const [awbNumber, data] = shipment
        const { isFetching, data: shipmentDetail } = data

        const originalData = shipmentDetail
        const groupedEventData = getGroupedEventData(shipmentDetail)
        const routings = getRoutings(groupedEventData)
        const latestMilestone = getLatestMilestone(shipmentDetail)
        const statusCode = getStatusCodes(groupedEventData)
        const isDelivered = checkIsDelivered(shipmentDetail, groupedEventData)
        const isDelivering = checkIsDelivering(
          shipmentDetail,
          groupedEventData,
          routings
        )
        const isInTransit = checkIsInTransit(groupedEventData, routings)
        const isCargoAcceptance = checkIsCargoAcceptance(
          groupedEventData,
          routings
        )
        const status = getStatus(
          isDelivered,
          isDelivering,
          isInTransit,
          isCargoAcceptance
        )
        const pieces = getPieces(shipmentDetail)
        const weight = getWeight(shipmentDetail)
        const weightCode = getWeightCode(shipmentDetail)
        const piecesLabel = getPiecesLabel(pieces)
        const weightLabel = getWeightLabel(weight, weightCode)
        const points = getPoints(routings)
        const lines = getLines(points)
        const shipmentInfo = getShipmentInfo(
          statusCode,
          groupedEventData,
          routings,
          pieces,
          weight
        )
        const infoType = debugConfig.isForceShowBookingInfo
          ? 2
          : getInfoType(shipmentInfo)
        const [awbPrefix] = awbNumber.split('-')
        const bookingInfo = getBookingInfo(
          routings,
          groupedEventData,
          awbPrefix
        )
        const isBookmarked = checkIsBookmarked(awbNumber, bookmarkList)
        const origin = shipmentDetail.origin
        const destination = shipmentDetail.destination

        const computedData = {
          originalData,
          isFetching: debugConfig.isSkeleton || isFetching,
          awbNumber,
          groupedEventData,
          routings,
          latestMilestone,
          statusCode,
          isDelivered,
          isDelivering,
          isInTransit,
          isCargoAcceptance,
          status,
          pieces,
          weight,
          weightCode,
          piecesLabel,
          weightLabel,
          shipmentDetail,
          points,
          lines,
          shipmentInfo,
          infoType,
          bookingInfo,
          isBookmarked,
          origin,
          destination,
        }

        return [awbNumber, computedData]
      })
      .filter((shipment) => {
        const [_awbNumber, data] = shipment

        return (
          data.isFetching ||
          !!data.bookingInfo?.length ||
          !!data.shipmentInfo?.length
        )
      })
  )
}

export function getStatusInfo(status, latestMilestone) {
  const tooltipInfo = {
    eventLabel: '',
    timeLabel: '',
  }

  if (latestMilestone) {
    const { statusCode, mdDate } = latestMilestone
    tooltipInfo.eventLabel = eventMappingObject[statusCode] || ''
    const time = mdDate?.seconds
      ? dayjs.unix(mdDate.seconds).format('DD MMM YYYY HH:mm')
      : ''
    tooltipInfo.timeLabel = `Last Updated Time: ${time}`
  }

  const isTooltipInfoCompleted =
    hasValue(tooltipInfo.eventLabel) && hasValue(tooltipInfo.timeLabel)

  const dynamicStyle = (() => {
    switch (status) {
      case StatusEnum.DELIVERED:
        return { backgroundColor: '#E8F4F0', color: '#189068' }
      case StatusEnum.DELIVERING:
      case StatusEnum.IN_TRANSIT:
        return { backgroundColor: '#E0F6FC', color: '#007C92' }
      case StatusEnum.CARGO_ACCEPTANCE:
      case StatusEnum.BOOKING_RECEIVED:
        return { backgroundColor: '#EEF0F0', color: '#556565' }
      default:
        return {}
    }
  })()

  const label = (() => {
    switch (status) {
      case StatusEnum.DELIVERED:
        return 'Delivered'
      case StatusEnum.DELIVERING:
        return 'Delivering'
      case StatusEnum.IN_TRANSIT:
        return 'In Transit'
      case StatusEnum.CARGO_ACCEPTANCE:
        return 'Cargo Acceptance'
      case StatusEnum.BOOKING_RECEIVED:
        return 'Booking Received'
      default:
        return ''
    }
  })()

  return {
    tooltipInfo,
    isTooltipInfoCompleted,
    dynamicStyle,
    label,
  }
}

// for entire AWB
export const StatusEnum = Object.freeze({
  DELIVERED: 'delivered',
  DELIVERING: 'delivering',
  IN_TRANSIT: 'inTransit',
  CARGO_ACCEPTANCE: 'cargoAcceptance',
  BOOKING_RECEIVED: 'bookingReceived',
})

// for port
export const PortStatusEnum = Object.freeze({
  COMPLETED: 'completed',
  IN_PROGRESS: 'inProgress',
  ERROR: 'error',
  NOT_STARTED: 'notStarted',
})

export const PortBookingStatusEnum = Object({
  CONFIRMED: 'confirmed',
  NOT_CONFIRM: 'notConfirm',
  BOOKING_RECEIVED: 'bookingReceived',
})

// api res for front end
export const ApiResStatusEnum = Object.freeze({
  NO_BOOKMARK_FOUND: 100,
  NO_SHIPMENT_FOUND: 101,
  AWB_NOT_FOUND: 102,
})

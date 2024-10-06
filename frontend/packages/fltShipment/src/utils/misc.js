import { nextTick, unref } from 'vue'
import Decimal from 'decimal.js'
import * as R from 'ramda'

export async function scrollToSpecificTag(id) {
  await nextTick()

  const el = document.querySelector(id)

  el.style = 'scroll-margin-top: 108px'

  el.scrollIntoView({ behavior: 'smooth', block: 'start' })

  el.style = null

  el.focus()
}

export function isNumeric(value) {
  return /^-?\d*\.?\d+$/.test(value)
}

export function isPositiveNumber(value) {
  const result = new Decimal(Number(value))
  return result.gt(0) && !result.isNaN()
}

export function isArray(obj) {
  if (typeof Array.isArray === 'function') return Array.isArray(obj)
  else return Object.prototype.toString.call(obj) === '[object Array]'
}

export function awbLastDigitChecker(awbSuffix) {
  if (awbSuffix?.length === 8) {
    return awbSuffix.substring(0, 7) + (awbSuffix.substring(0, 7) % 7)
  } else {
    return awbSuffix
  }
}

export function validateAwbNumberFormat(awbNumber) {
  const pattern = /\d{3}-\d{8}/

  return pattern.test(awbNumber)
}

export function validateAwbSuffix(awbSuffix) {
  if (awbSuffix?.length !== 8) return false

  const mainDigits = awbSuffix.substring(0, 7)

  const checksumDigit = mainDigits % 7
  const expectedSuffix = `${mainDigits}${checksumDigit}`

  return awbSuffix === expectedSuffix
}

export function specificRoundingMethod(value) {
  if (Number(value) === NaN || value < 0) return 0
  Decimal.rounding = Decimal.ROUND_HALF_EVEN
  const roundedValue = new Decimal(value).round().toNumber()
  return roundedValue < value
    ? new Decimal(roundedValue).add(0.5).toNumber()
    : roundedValue
}

export function shuffle(array) {
  const copiedArray = [...array]
  let currentIndex = copiedArray.length

  while (currentIndex != 0) {
    let randomIndex = Math.floor(Math.random() * currentIndex)
    currentIndex--
    ;[copiedArray[currentIndex], copiedArray[randomIndex]] = [
      copiedArray[randomIndex],
      copiedArray[currentIndex],
    ]
  }

  return copiedArray
}

export function isContentOverflowed(vueElRef) {
  let el = unref(vueElRef)
  if (!el) return
  const range = document.createRange()
  range.setStart(el, 0)
  range.setEnd(el, el.childNodes.length)
  const rangeWidth = range.getBoundingClientRect().width
  const getStyle = (el, key) => {
    if (!el || !key) return
    return getComputedStyle(el)?.[key]
  }
  const padding =
    (parseInt(getStyle(el, 'paddingLeft'), 10) || 0) +
    (parseInt(getStyle(el, 'paddingRight'), 10) || 0)
  const scrollWidth = rangeWidth + padding
  const clientWidth = el.getBoundingClientRect().width
  return clientWidth < scrollWidth
}

export const hasValue = R.complement(R.either(R.isNil, R.isEmpty))

export const sleep = (timer) =>
  new Promise((resolve) => setTimeout(() => resolve(), timer))

// ref: https://github.com/quasarframework/quasar/issues/1958#issuecomment-1050679652
export const calcTextWidth = (input, text) => {
  const span = document.createElement('span')
  span.textContent = text
  span.classList = input.classList
  span.style = input.style
  Object.assign(span.style, {
    maxWidth: 'none',
    width: 'auto',
    whiteSpace: 'nowrap',
    position: 'absolute',
    opacity: '0',
    visibility: 'hidden',
  })
  input.after(span)
  const width = span.getBoundingClientRect().width
  span.remove()
  return width
}

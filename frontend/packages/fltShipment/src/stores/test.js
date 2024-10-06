import { defineStore } from 'pinia'
import { reactive } from 'vue'

export const useTestStore = defineStore('test', () => {
  const criteria = reactive({
    dateRangeType: {
      label: 'Date Range(test)',
      value: '',
    },
    dateRangeFrom: {
      label: 'From(test)',
      value: '',
    },
    dateRangeTo: {
      label: 'To(test)',
      value: '',
    },
  })

  return {
    criteria,
  }
})

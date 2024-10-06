import { reactive, computed } from 'vue'
import { defineStore } from 'pinia'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'
import { useNestBStore } from './nestB'

export const useNestAStore = defineStore('nestA', () => {
  // const nestBStore = useNestBStore()

  const nestAState = reactive({
    name: null,
  })

  const rules = computed(() => {
    const rules = {
      name: {
        required: helpers.withMessage(
          () => `Invalid name - nestA (required)`,
          required
        ),
        length: helpers.withMessage(
          () => `Invalid name - nestA (length)`,
          (value) => {
            return value?.length === 3
          }
        ),
      },
    }

    return rules
  })

  const nestAVuelidate = useVuelidate(rules, nestAState, { $scope: false })

  return {
    nestAState,
    nestAVuelidate,
  }
})

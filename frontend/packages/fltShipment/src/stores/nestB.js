import { reactive, computed } from 'vue'
import { defineStore } from 'pinia'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'

export const useNestBStore = defineStore('nestB', () => {
  const nestAState = reactive({
    name: null,
  })

  const rules = computed(() => {
    const rules = {
      name: {
        required: helpers.withMessage(
          () => `Invalid name - nestB (required)`,
          required
        ),
        length: helpers.withMessage(
          () => `Invalid name - nestB (length)`,
          (value) => {
            return value?.length === 3
          }
        ),
      },
    }

    return rules
  })

  const nestBVuelidate = useVuelidate(rules, nestAState, {
    $lazy: false,
    $scope: false,
  })

  return {
    nestAState,
    nestBVuelidate,
  }
})

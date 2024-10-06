<script setup>
import { roundStar } from '@quasar/extras/material-icons-round'

const props = defineProps({
  isBookmarked: { type: Boolean, required: true, default: false },
})

const emits = defineEmits(['onBookmarkClick'])
</script>

<template>
  <q-icon
    @click="emits('onBookmarkClick')"
    class="u-cursor-pointer!"
    :name="roundStar"
    :color="
      isBookmarked ? 'EzyBooking-alert-yellow' : 'EzyBooking-light-gray-2'
    "
    size="sm"
  />
</template>

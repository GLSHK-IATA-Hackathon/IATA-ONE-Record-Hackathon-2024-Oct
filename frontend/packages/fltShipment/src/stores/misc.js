import { ref } from 'vue'
import { defineStore } from 'pinia'
import { Dialog } from 'quasar'

import { i18n } from '@/locales'
import GlsDialog from '@/components/GlsDialog.vue'

export const useMiscStore = defineStore('misc', () => {
  const glsDialogState = ref(false)

  function redirectToLoginPage() {
    const ezyCargoBaseUrl = import.meta.env.VITE_EZYCARGO_BASE_URL
    const locale = i18n.global.locale.value
    const loginPageUrl = `${ezyCargoBaseUrl}/${locale}/ezycargo/`
    location.replace(loginPageUrl)
  }

  function glsDialogCreateHandler(glsDialogProps) {
    if (!glsDialogState.value) {
      glsDialogState.value = true
      Dialog.create({
        component: GlsDialog,
        componentProps: {
          ...glsDialogProps,
          dialogState: glsDialogState.value,
        },
      })
        .onOk(() => (glsDialogState.value = false))
        .onCancel(() => (glsDialogState.value = false))
        .onDismiss(() => (glsDialogState.value = false))
    }
  }

  return {
    redirectToLoginPage,
    glsDialogCreateHandler,
  }
})

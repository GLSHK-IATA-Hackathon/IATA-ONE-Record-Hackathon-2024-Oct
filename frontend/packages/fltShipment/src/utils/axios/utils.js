import * as R from 'ramda'

import { useMiscStore } from '@/stores/misc'
import { apiErrorMessage } from '@/constants/messages'
import { hasValue } from '@/utils/misc'

export function responseHandler(response) {
  if (!hasValue(response?.data)) return

  if (R.is(String, response?.data)) {
    try {
      response.data = JSON.parse(response?.data)
    } catch (error) {
      console.error(
        'Response is not in JSON format, Response : ' + response?.data
      )
      response.data = {}
    }
  }

  return response
}

export function responseErrorHandler(error) {
  const miscStore = useMiscStore()
  const { redirectToLoginPage, glsDialogCreateHandler } = miscStore

  if (error.response) {
    switch (error.response.status) {
      case 401:
        glsDialogCreateHandler({
          isPersistent: true,
          isCancelBtnDisplayed: false,
          title: apiErrorMessage.type.SESSION_EXPIRED,
          description: apiErrorMessage.description.SESSION_EXPIRED,
          okBtnLabel: apiErrorMessage.action.OK,
          afterOk: () => {
            redirectToLoginPage()
          },
        })
      case 403:
      case 500:
      default:
        throw error
    }
  }
  if (!window.navigator.onLine) {
  }

  return Promise.reject(error)
}

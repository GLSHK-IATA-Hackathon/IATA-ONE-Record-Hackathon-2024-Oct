import { defineStore } from 'pinia'
import dayjs from 'dayjs'
import { reactive } from 'vue'

import authorizationApi from '@/apis/authorizationApi'
import { useMiscStore } from '@/stores/misc'
import { apiErrorMessage } from '@/constants/messages'

export const useUserStore = defineStore('user', () => {
  const userProfile = reactive({
    companyID: '',
    userID: '',
    RARCode: '',
    DefaultWgtCode: '',
    DefaultLengthCode: '',
    DefaultVolumeCode: '',
    CurrencyCode: '',
    CompanyName: '',
    UserCity: '',
    UserOprPort: '',
  })

  const miscStore = useMiscStore()
  const { glsDialogCreateHandler } = miscStore

  function getToken() {
    return $cookies.get('ApiToken')
  }

  // only for local development
  async function signIn() {
    const userID = import.meta.env.VITE_USER_ID
    const companyID = import.meta.env.VITE_COMPANY_ID

    try {
      const { data } = await authorizationApi.signIn({
        client_id: `${userID}@${companyID}`,
        client_secret: import.meta.env.VITE_CLIENT_SECRET,
      })
      const tokenPair = {
        ApiToken: data.access_token,
        RefreshToken: data.refresh_token,
        ApiTokenExpiryTime: dayjs().add(data.expires_in, 's').valueOf(),
        userID,
        companyID,
      }

      for (const key in tokenPair) {
        $cookies.set(key, tokenPair[key])
      }
    } catch (error) {
      console.log(error)
      glsDialogCreateHandler({
        isCancelBtnDisplayed: false,
        title: apiErrorMessage.type.UNAVAILABLE_SERVICE,
        description: apiErrorMessage.description.GENERAL,
        okBtnLabel: apiErrorMessage.action.OK,
      })
    }
  }

  function getUserAndCompanyID() {
    if (!userProfile.companyID)
      userProfile.companyID = $cookies.get('companyID').toUpperCase()
    if (!userProfile.userID)
      userProfile.userID = $cookies.get('userID').toUpperCase()
  }

  return {
    getToken,
    signIn,
    getUserAndCompanyID,
  }
})

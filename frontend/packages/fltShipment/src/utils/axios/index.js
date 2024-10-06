import axios from 'axios'

import { useUserStore } from '@/stores/user'
import { responseHandler, responseErrorHandler } from '@/utils/axios/utils'

export const apiHelper_auth = axios.create({
  baseURL: import.meta.env.VITE_AUTH_API_BASE_URL,
})

export const apiHelper_ezyDataMart = axios.create({
  baseURL: import.meta.env.VITE_EZYDATAMART_API_BASE_URL,
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

export const apiHelper_tnt = axios.create({
  baseURL: import.meta.env.VITE_TNT_API_BASE_URL,
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

apiHelper_tnt.interceptors.request.use(
  function (config) {
    const userStore = useUserStore()
    const { getToken } = userStore

    const token = getToken()
    config.headers.Authorization = `Bearer ${token}`

    return config
  },
  function (error) {
    return Promise.reject(error)
  }
)

apiHelper_tnt.interceptors.response.use(
  function (response) {
    return responseHandler(response)
  },
  function (error) {
    responseErrorHandler(error)
  }
)

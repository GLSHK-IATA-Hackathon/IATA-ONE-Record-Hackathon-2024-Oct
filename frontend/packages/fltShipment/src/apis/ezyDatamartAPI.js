import { apiHelper_ezyDataMart } from '@/utils/axios'

export default {
  search(params) {
    return apiHelper_ezyDataMart.post('/GetFreightStatus', params)
  },
}

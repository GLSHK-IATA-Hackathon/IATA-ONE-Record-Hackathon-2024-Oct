import { apiHelper_tnt } from '@/utils/axios'

export default {
  fetchBookmarkList() {
    return apiHelper_tnt.get('/Bookmark')
  },

  fetchShipmentDetail(params) {
    return apiHelper_tnt.get('/TrackAndTrace/Search', { params })
  },

  toggleBookmark(params) {
    return apiHelper_tnt.post('/Bookmark', params)
  },

  fetchSearchHistory() {
    return apiHelper_tnt.get('/SearchHistory/recent')
  },

  addSearchHistory(params) {
    return apiHelper_tnt.post('/SearchHistory/recent', params)
  },
}

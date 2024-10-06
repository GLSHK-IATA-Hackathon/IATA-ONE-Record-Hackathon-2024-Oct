import { apiHelper_auth } from '@/utils/axios'

export default {
  signIn({ client_id, client_secret }) {
    return apiHelper_auth.post(
      '/token',
      {
        client_id,
        client_secret,
        grant_type: 'client_credentials',
      },
      {
        headers: { 'content-type': 'application/x-www-form-urlencoded' },
      }
    )
  },
}

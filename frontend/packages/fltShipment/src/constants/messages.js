import { i18n } from '@/locales'
import { isArray } from '@/utils/misc'
import { ApiResStatusEnum } from '@/enums/StatusEnum'

export const apiErrorMessage = {
  get type() {
    return {
      NULL_CURRENCY: i18n.global.t('errorMessage.type.nullDefaultCurrency'),
      UNAUTHORIZED_ACCESS: i18n.global.t(
        'errorMessage.type.unauthorizedAccess'
      ),
      SESSION_EXPIRED: i18n.global.t('errorMessage.type.sessionExpired'),
      UNAVAILABLE_SERVICE: i18n.global.t(
        'errorMessage.type.unavailableService'
      ),
      AWB_NOT_FOUND: 'AWB not found',
      NO_RECORD_FOUND: 'No record found',
    }
  },
  get description() {
    return {
      GENERAL: i18n.global.t('errorMessage.general'),
      RE_LOGIN: i18n.global.t('errorMessage.reLogin'),
      SESSION_EXPIRED: i18n.global.t('errorMessage.sessionExpired'),
      REDIRECTED_TO_LOGIN: i18n.global.t('errorMessage.redirectedToLogin'),
      ACCESS_DENIED: i18n.global.t('errorMessage.accessDenied'),
      CURRENCY: i18n.global.t('errorMessage.currency'),
    }
  },
  get action() {
    return {
      OK: i18n.global.t('common.ok'),
      CANCEL: i18n.global.t('common.cancel'),
    }
  },
}

export const LANDING_PAGE_MESSAGES = {
  get SEARCH_API_RESPONSE() {
    return {
      [ApiResStatusEnum.NO_SHIPMENT_FOUND]: {
        iconName: 'Awb_not_found',
        title: i18n.global.t('views.landingPage.searchApiResponse.title1'),
        description: i18n.global.t(
          'views.landingPage.searchApiResponse.description1'
        ),
      },
      [ApiResStatusEnum.NO_BOOKMARK_FOUND]: {
        iconName: 'No_record_found',
        title: i18n.global.t('views.landingPage.searchApiResponse.title2'),
        description: i18n.global.t(
          'views.landingPage.searchApiResponse.description2'
        ),
      },
      [ApiResStatusEnum.AWB_NOT_FOUND]: {
        iconName: 'Error_icon',
        title: function (_awbNumber) {
          const awbNumber = isArray(_awbNumber)
            ? _awbNumber.join(', ')
            : _awbNumber

          return i18n.global.t('views.landingPage.searchApiResponse.title3', {
            awbNumber,
          })
        },
        description: function (_awbNumber) {
          const awbNumber = isArray(_awbNumber)
            ? _awbNumber.join(', ')
            : _awbNumber

          return i18n.global.t(
            'views.landingPage.searchApiResponse.description3',
            {
              awbNumber,
            }
          )
        },
      },
    }
  },
}

<script setup>
import { useDialogPluginComponent } from 'quasar'

import { ErrorEnum } from '@/enums/ErrorEnum'

const props = defineProps({
  dialogState: { required: false, type: Boolean, default: false },
  isCloseBtnDisplayed: { required: false, type: Boolean, default: false },
  isPersistent: { required: false, type: Boolean, default: true },
  title: { required: false, type: String, default: ErrorEnum.NOTICE },
  description: { required: false, type: String, default: '' },
  isErrorType: { required: false, type: Boolean, default: false },
  isOkBtnDisplayed: { required: false, type: Boolean, default: true },
  okBtnLabel: { required: false, type: String, default: 'OK' },
  isCancelBtnDisplayed: { required: false, type: Boolean, default: true },
  cancelBtnLabel: { required: false, type: String, default: 'Cancel' },
  afterOk: { required: false, type: Function, default: () => {} },
  afterCancel: { required: false, type: Function, default: () => {} },
})

const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } =
  useDialogPluginComponent()

const emits = defineEmits(['after-dialog-state-changes', 'ok', 'cancel'])

function onCancelClick() {
  emits('cancel')
  onDialogCancel(props.afterCancel())
}

function onOKClick() {
  emits('ok')
  onDialogOK(props.afterOk())
}
</script>

<template>
  <q-dialog
    :model-value="dialogState"
    :persistent="isPersistent"
    v-bind="$attrs"
    ref="dialogRef"
    @hide="onDialogHide"
  >
    <q-card class="dialog-card inner">
      <div
        :class="{ 'dialog-card__wrapper gls-scrollbarY': isCloseBtnDisplayed }"
      >
        <div class="u-right-16px u-top-16px u-pos-absolute">
          <q-btn
            name="cancel button"
            v-if="isCloseBtnDisplayed"
            color="EzyBooking-light-gray-4"
            flat
            round
            dense
            icon="close"
            v-close-popup
          />
        </div>
        <q-card-section
          name="title"
          class="dialog-card__header justify-start row"
        >
          <div v-if="isErrorType" class="row items-center">
            <GlsIcon name="ErrorSymbol" class="u-mr-16px" />
            <span class="dialog-card__title--error">
              {{ title }}
            </span>
          </div>
          <span v-else class="dialog-card__title">
            {{ title }}
          </span>
        </q-card-section>
        <q-card-section
          name="description"
          class="dialog-card__content row justify-start no-wrap"
        >
          <span v-if="!!description" class="dialog-card__description row">
            {{ description }}
          </span>
          <slot name="description"> </slot>
        </q-card-section>
        <q-card-actions
          name="footer"
          v-if="isOkBtnDisplayed || isCancelBtnDisplayed"
          class="dialog-card__footer"
          align="right"
        >
          <q-btn
            v-show="isCancelBtnDisplayed"
            outline
            class="dialog-card__footer-cancel"
            :no-caps="true"
            v-close-popup
            @click="onCancelClick()"
          >
            <span> {{ cancelBtnLabel }}</span>
          </q-btn>
          <q-btn
            v-show="isOkBtnDisplayed"
            flat
            class="dialog-card__footer-ok"
            :no-caps="true"
            v-close-popup
            @click="onOKClick()"
          >
            <span> {{ okBtnLabel }}</span>
          </q-btn>
        </q-card-actions>
      </div>
    </q-card>
  </q-dialog>
</template>
<style lang="scss" scoped>
.dialog-card {
  position: relative;
  padding: 40px 0;
  background-color: rgba(255, 255, 255, 1);

  &__wrapper {
    overflow-y: auto;
    max-height: 60vh;
    scrollbar-width: thin;
  }

  &__header {
    margin: 0 32px 16px;
  }

  &__title {
    font-weight: 600;
    font-size: 24px;
    line-height: 30px;

    &--error {
      font-weight: 600;
      font-size: 24px;
      line-height: 30px;
      color: #e03426;
    }
  }

  &__description {
    padding: 0 32px;
    font-weight: 600;
    font-size: 16px;
    line-height: 20px;
    white-space: pre-line;
  }

  &__footer {
    margin: 40px 32px 0;
  }

  &__footer-ok,
  &__footer-cancel {
    height: 50px;
    min-width: 144px;
    font-weight: 600;
    font-size: 16px;
    line-height: 18px;
  }

  &__footer-ok {
    color: #f8fafc;
    background-color: $EzyBooking-bright-green;
  }

  &__footer-cancel {
    color: $EzyBooking-green;
    background-color: #ffffff;
    border-radius: 1px solid $EzyBooking-green;
    margin-right: 16px;
  }

  :deep(.q-card__section--vert) {
    padding: 0;
  }
}
.q-dialog__inner--minimized > div {
  max-width: 100%;
  width: 757px;
}

::-webkit-scrollbar {
  /* make scrollbar transparent */
  width: 0px;
  background: transparent;
}
</style>

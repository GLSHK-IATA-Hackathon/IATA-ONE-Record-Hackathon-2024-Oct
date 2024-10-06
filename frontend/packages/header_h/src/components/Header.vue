<template>
  <q-layout>
    <q-header elevated class="text-black" style="background: #FFFFFF" height-hint="61.59">
      <q-toolbar class="q-py-sm q-px-md">
        <q-img src="@/assets/EzyHandle.svg" class="q-mr-sm"style="width: 105px; height: 32px;"></q-img>


        <div v-if="$q.screen.gt.sm" class="GL__toolbar-link q-ml-xs q-gutter-md text-body2 text-weight-bold row items-center no-wrap">
          <router-link to="/adminSetting" class="q-ml-auto q-mr-md text-black">
            Admin Setting
          </router-link>
          <router-link to="/stdHandlingProc" class="q-ml-auto q-mr-md text-black">
            Standard Handling Procedure
          </router-link>          
          <router-link to="/fltShipment" class="q-ml-auto q-mr-md text-black">
            Flight Shipment
          </router-link>
          <router-link to="/fltDashboard" class="q-ml-auto q-mr-md text-black">
            Flight Dashboard
          </router-link>
          <router-link to="/departureInfo" class="q-ml-auto q-mr-md text-black">
            Departure Info
          </router-link>
        
        </div>

        <q-space />
        <div v-if="$q.screen.gt.sm" class="GL__toolbar-link q-ml-xs q-gutter-md text-body2 text-weight-bold row items-center no-wrap">
          <q-avatar color="primary" text-color="white">I</q-avatar>
          <span class="text-black">IST RAMP</span>
        </div>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
import { ref } from 'vue'
import { fabGithub } from '@quasar/extras/fontawesome-v6'

const stringOptions = [
  'quasarframework/quasar',
  'quasarframework/quasar-awesome'
]

export default {
  name: 'MyLayout',

  setup () {
    const text = ref('')
    const options = ref(null)
    const filteredOptions = ref([])
    const search = ref(null) // $refs.search

    function filter (val, update) {
      if (options.value === null) {
        // load data
        setTimeout(() => {
          options.value = stringOptions
          search.value.filter('')
        }, 2000)
        update()
        return
      }

      if (val === '') {
        update(() => {
          filteredOptions.value = options.value.map(op => ({ label: op }))
        })
        return
      }

      update(() => {
        filteredOptions.value = [
          {
            label: val,
            type: 'In this repository'
          },
          {
            label: val,
            type: 'All GitHub'
          },
          ...options.value
            .filter(op => op.toLowerCase().includes(val.toLowerCase()))
            .map(op => ({ label: op }))
        ]
      })
    }

    return {
      fabGithub,

      text,
      options,
      filteredOptions,
      search,

      filter
    }
  }
}
</script>

<style lang="sass">
$blue-grey-6 : #607d8b !default
$light-blue-9 : #0277bd !default

.GL
  &__select-GL__menu-link
    .default-type
      visibility: hidden

    &:hover
      background: #0366d6
      color: white
      .q-item__section--side
        color: white
      .default-type
        visibility: visible

  &__toolbar-link
    a
      color: white
      text-decoration: none
      &:hover
        opacity: 0.7

  &__menu-link:hover
    background: #0366d6
    color: white

  &__menu-link-signed-in,
  &__menu-link-status
    &:hover
      & > div
        background: white !important

  &__menu-link-status
    color: $blue-grey-6
    &:hover
      color: $light-blue-9

  &__toolbar-select.q-field--focused
    width: 450px !important
    .q-field__append
      display: none
</style>

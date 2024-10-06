<script setup>
import { ref } from "vue";


const email = ref("");
const password = ref("");
const apiUrl = import.meta.env.VITE_LOGIN_API_BASE_URL;

function login() {
  location.href = `/fltShipment`;
}
</script>

<template>
  <div class="window-height window-width row justify-center items-center">
    <div class="column">
      <div class="row"></div>
      <div class="row">
        <h6 class="text-h6 q-ma-none"></h6>
      </div>
      <div class="row">
        <q-card square bordered class="q-pa-lg shadow-1">
          <q-card-section>
            <q-form class="q-gutter-md">
              <q-input
                square
                filled
                clearable
                v-model="email"
                type="email"
                label="email"
              />
              <q-input
                square
                filled
                clearable
                v-model="password"
                type="password"
                label="password"
              />
            </q-form>
          </q-card-section>
          <q-card-actions class="q-px-md">
            <q-btn
              @click="login"
              unelevated
              color="secondary"
              size="lg"
              class="full-width"
              :label="$t('login')"
            />
          </q-card-actions>
          <q-card-section class="text-center q-pa-none">
            <p class="text-grey-6">Not reigistered? Created an Account</p>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </div>
</template>

<style scoped>
.q-card {
  width: 360px;
}
</style>

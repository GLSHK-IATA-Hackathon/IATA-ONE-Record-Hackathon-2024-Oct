<template>
    <div class="q-pa-md row items-start q-gutter-md">
      <q-card class="my-card">
        <q-card-section>
          Flight Shipment
        </q-card-section>
      </q-card>

      <q-card class="my-card">
        <q-card-section>
          Departure
          25
          3 completed
        </q-card-section>
      </q-card>

      <q-card class="my-card">
        <q-card-section>
          Arrival
          12
          3 completed
        </q-card-section>
      </q-card>

      <q-card class="my-card">
        <q-card-section>
          Delay
          2
        </q-card-section>
      </q-card>

      <q-card class="my-card">
        <q-card-section>
          Awareness
          3
        </q-card-section>
      </q-card>

      <q-table
        flat bordered
        title="Latest Shipment"
        dense
        :rows="rows"
        :columns="columns"
        row-key="name"
      />
  
      
    </div>
  </template>


<script>
  const columns = [

  ]

  const rows = [

  ]

  export default {
    setup () {
      return {
        columns,
        rows
      }
    }
  }
</script>
<template>
  <div class="q-pa-md row items-start q-gutter-md">
    
    
    <q-card class="my-card">
      <q-card-section>
        Standard handling procedure
      </q-card-section>
    </q-card>

    <q-table
      flat bordered
      title="Handling procedure"
      dense
      :rows="rows"
      :columns="columns"
      row-key="name"
    />

  </div>
</template>

<script>

  const columns = [
    { name: 'airline', required: true, label: 'Airline', field: row => row.airline, format: val => `${val}`, sortable: true},
    { name: 'airport', align: 'center', label: 'Airport', field: 'airport', sortable: true },
    { name: 'shipmentType', label: 'Shipment Type', field: 'shipmentType'},
    { name: 'transitTime', label: 'Transit time (m/n)', field: 'transitTime'},
    { name: 'action', label: 'Action', field: 'action'}
  ]

  const rows = [
    {
      airline: 'GLS',
      airport: 'HKG',
      shipmentType: 'Transit',
      transitTime: '</=90',
      action: 'TARMAC'
    },
    {
      airline: 'GLS',
      airport: 'HKG',
      shipmentType: 'Transit',
      transitTime: '90-180',
      action: 'Fast transit'
    },
    {
      airline: 'GLS',
      airport: 'HKG',
      shipmentType: 'Transit',
      transitTime: '>180',
      action: 'Normal transit'
    }

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
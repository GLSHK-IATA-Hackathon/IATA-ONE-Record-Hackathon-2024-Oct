<template>
  <div class="q-pa-md row items-start q-gutter-md">
    <q-card class="my-card">
      <q-card-section>
        Flight Dashboard
      </q-card-section>
    </q-card>

    <q-card class="my-card">
      <q-card-section>
        Departure (12)
        <q-table
          flat bordered
          title="Test"
          dense
          :rows="rows"
          :columns="columns"
          row-key="name"
        />        
      </q-card-section>
    </q-card>

    <q-card class="my-card">
      <q-card-section>
        Arrival (12)
      </q-card-section>
    </q-card>

    
  </div>
</template>


<script>
const columns = [
  {
    name: 'flightNo',
    required: true,
    label: 'Flight No.',
    align: 'left',
    field: row => row.flightNo,
    format: val => `${val}`,
    sortable: true
  },
  { name: 'type', align: 'center', label: 'Type', field: 'type', sortable: true },
  { name: 'ori', label: 'ORI', field: 'ori', sortable: true },
  { name: 'des', label: 'DES', field: 'des' },
  { name: 'stdOrSta', label: 'STD/STA', field: 'stdOrSta' },
  { name: 'atdOrEta', label: 'ATD/ETA', field: 'atdOrEta' },
  { name: 'status', label: 'Status', field: 'status' }
]

const rows = [
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Processing'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Delay'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Processing'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Processing'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Completed'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    stdOrSta: '23:00/07:00',
    atdOrEta: '22:50/06:35',
    status: 'Delay'
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
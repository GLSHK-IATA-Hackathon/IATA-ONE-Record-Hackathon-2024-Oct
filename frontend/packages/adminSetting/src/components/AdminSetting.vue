<template>
  <div class="q-pa-md row items-start q-gutter-md">

    
    
    <q-card class="my-card">
      <q-card-section>
        Admin Setting
      </q-card-section>
    </q-card>

    <q-table
      flat bordered
      title="ENCORE Offsets"
      dense
      :rows="rows"
      :columns="columns"
      row-key="name"
    />
  </div>
</template>

<script>
const columns = [
  {
    name: 'airline',
    required: true,
    label: 'Airline',
    align: 'left',
    field: row => row.airline,
    format: val => `${val}`,
    sortable: true
  },
  { name: 'airport', align: 'center', label: 'Airport', field: 'airport', sortable: true },
  { name: 'export', label: 'Export', field: 'export', sortable: true },
  { name: 'import', label: 'Import', field: 'import' },
  { name: 'transit', label: 'Transit', field: 'transit' },
  { name: 'productGroup', label: 'Product Group', field: 'productGroup' },
  { name: 'dow', label: 'DOW', field: 'dow' },
  { name: 'flight', label: 'Flight', field: 'flight' },
  { name: 'aircraftType', label: 'Aircraft Type', field: 'aircraftType' },
  { name: 'fohBeforeFwb', label: 'FOH before FWB', field: 'fohBeforeFwb' },
  { name: 'uws', label: 'UWS', field: 'uws' },
  { name: 'lir', label: 'LIR', field: 'lir' },
  { name: 'fow', label: 'FOW', field: 'fow' },
  { name: 'als', label: 'ALS', field: 'als' },
  { name: 'ale', label: 'ALE', field: 'ale' },
  { name: 'ofb', label: 'OFB', field: 'ofb' },
  { name: 'ffm', label: 'FFM', field: 'ffm' },
  { name: 'wdo', label: 'WDO', field: 'wdo' },
  { name: 'aus', label: 'AUS', field: 'aus' },
  { name: 'aue', label: 'AUE', field: 'aue' },
  { name: 'fiw', label: 'FIW', field: 'fiw' },
  { name: 'efreightEnabled', label: 'EFreight Enabled', field: 'efreightEnabled' },
  { name: 'status', label: 'Status', field: 'status' },
  { name: 'confirmationDate', label: 'Confirmation Date', field: 'confirmationDate' },
  //{ name: 'calcium', label: 'Calcium (%)', field: 'calcium', sortable: true, sort: (a, b) => parseInt(a, 10) - parseInt(b, 10) },
  //{ name: 'iron', label: 'Iron (%)', field: 'iron', sortable: true, sort: (a, b) => parseInt(a, 10) - parseInt(b, 10) }
]

const rows = [
  {
    airline: 'GLS',
    airport: 'HKG',
    export: 1,
    import: 1,
    transit: 0,
    productGroup: 'GC',
    dow: 1234567,
    flight: 'ANY',
    aircraftType: 'RFS',
    fohBeforeFwb: 0,
    uws: 155,
    lir: 150,
    fow: 150,
    als: 60,
    ale: 60,
    ofb: '-',
    ffm: 180,
    wdo: 180,
    aus: 185,
    aue: 2880,
    fiw: 60,
    efreightEnabled: 'NOT',
    status: 'CONFIRMED',
    confirmationDate: "12-FEB-2016"
  },
  {
    airline: 'GLS',
    airport: 'HKG',
    export: 1,
    import: 1,
    transit: 0,
    productGroup: 'PER',
    dow: 1234567,
    flight: 'ANY',
    aircraftType: 'RFS',
    fohBeforeFwb: 0,
    uws: 155,
    lir: 150,
    fow: 150,
    als: 60,
    ale: 45,
    ofb: '-',
    ffm: 180,
    wdo: 180,
    aus: 185,
    aue: 2880,
    fiw: 45,
    efreightEnabled: 'NOT',
    status: 'CONFIRMED',
    confirmationDate: "12-FEB-2016"
  },
  {
    airline: 'GLS',
    airport: 'IST',
    export: 1,
    import: 1,
    transit: 0,
    productGroup: 'GC',
    dow: 1234567,
    flight: 'ANY',
    aircraftType: 'RFS',
    fohBeforeFwb: 0,
    uws: 155,
    lir: 150,
    fow: 150,
    als: 60,
    ale: 45,
    ofb: '-',
    ffm: 180,
    wdo: 180,
    aus: 185,
    aue: 2880,
    fiw: 60,
    efreightEnabled: 'NOT',
    status: 'CONFIRMED',
    confirmationDate: "12-FEB-2016"
  },
  {
    airline: 'GLS',
    airport: 'HKG',
    export: 1,
    import: 1,
    transit: 0,
    productGroup: 'PER',
    dow: 1234567,
    flight: 'ANY',
    aircraftType: 'RFS',
    fohBeforeFwb: 0,
    uws: 155,
    lir: 150,
    fow: 150,
    als: 60,
    ale: 45,
    ofb: '-',
    ffm: 180,
    wdo: 180,
    aus: 185,
    aue: 2880,
    fiw: 45,
    efreightEnabled: 'NOT',
    status: 'CONFIRMED',
    confirmationDate: "12-FEB-2016"
  },
]

/*
const rows = [
  {
    name: 'Frozen Yogurt',
    calories: 159,
    fat: 6.0,
    carbs: 24,
    protein: 4.0,
    sodium: 87,
    calcium: '14%',
    iron: '1%'
  },
  {
    name: 'Ice cream sandwich',
    calories: 237,
    fat: 9.0,
    carbs: 37,
    protein: 4.3,
    sodium: 129,
    calcium: '8%',
    iron: '1%'
  },
  {
    name: 'Eclair',
    calories: 262,
    fat: 16.0,
    carbs: 23,
    protein: 6.0,
    sodium: 337,
    calcium: '6%',
    iron: '7%'
  },
  {
    name: 'Cupcake',
    calories: 305,
    fat: 3.7,
    carbs: 67,
    protein: 4.3,
    sodium: 413,
    calcium: '3%',
    iron: '8%'
  },
  {
    name: 'Gingerbread',
    calories: 356,
    fat: 16.0,
    carbs: 49,
    protein: 3.9,
    sodium: 327,
    calcium: '7%',
    iron: '16%'
  },
  {
    name: 'Jelly bean',
    calories: 375,
    fat: 0.0,
    carbs: 94,
    protein: 0.0,
    sodium: 50,
    calcium: '0%',
    iron: '0%'
  },
  {
    name: 'Lollipop',
    calories: 392,
    fat: 0.2,
    carbs: 98,
    protein: 0,
    sodium: 38,
    calcium: '0%',
    iron: '2%'
  },
  {
    name: 'Honeycomb',
    calories: 408,
    fat: 3.2,
    carbs: 87,
    protein: 6.5,
    sodium: 562,
    calcium: '0%',
    iron: '45%'
  },
  {
    name: 'Donut',
    calories: 452,
    fat: 25.0,
    carbs: 51,
    protein: 4.9,
    sodium: 326,
    calcium: '2%',
    iron: '22%'
  },
  {
    name: 'KitKat',
    calories: 518,
    fat: 26.0,
    carbs: 65,
    protein: 7,
    sodium: 54,
    calcium: '12%',
    iron: '6%'
  }
]
*/

export default {
  setup () {
    return {
      columns,
      rows
    }
  }
}
</script>
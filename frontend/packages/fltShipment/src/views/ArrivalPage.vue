<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const flightNo = route.params.flightNo;
const origin = route.query.ori;
const destination = route.query.des;
const stdOrAtd = route.query.stdOrAtd;
const staOrEta = route.query.staOrEta;


const flight = ref(null);

const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));
const handleClick = async () => {
  await axios.post('http://localhost:3000/log', { message: 'Waiting Request' });
  await delay(5000);
  await axios.post('http://localhost:3000/log', { message: 'Received milestone update request (FOW)' });
  await axios.post('http://localhost:3000/log', { message: 'Calling 1R request' });
  await axios.post('http://localhost:3000/log', { message: 'milestone created (FOW)' });
};

// Transit 
const transitColumns = [
  { name: 'awbNo', required: true, label: 'AWB No.', align: 'left', field: 'awbNo' },
  { name: 'ori', label: 'ORI', align: 'left', field: 'ori' },
  { name: 'des', label: 'DES', align: 'left', field: 'des' },
  { name: 'pcWtVol', label: 'PC/WT/VOL', align: 'left', field: 'pcWtVol'},
  { name: 'uldQty', label: 'ULD(Quantity)', align: 'left'},
  { name: 'shc', label: 'SHC', align: 'left', field: 'shc'},

]

const transitRows = [
  {
    awbNo: '000-11111111',
    ori: 'HKG',
    des: 'LHR',
    transit: 'CX002/02Oct\r\nIST-LHR',
    pcWtVol: '1pc/1600kg/10MC',
    uld: 'A2',
    qty: '1',
    shc: 'HEA'
  },
  {
    awbNo: '000-22222222',
    ori: 'HKG',
    des: 'MXP',
    transit: 'CX003/02Oct\r\nIST-MXP',
    pcWtVol: '1pc/2000kg/12MC',
    uld: 'H2',
    qty: '1',
    shc: 'COL'
  },
]

// Import
const importColumns = [
  { name: 'awbNo', required: true, label: 'AWB No.', align: 'left', field: 'awbNo' },
  { name: 'ori', label: 'ORI', align: 'left', field: 'awbNo' },
  { name: 'des', label: 'DES', align: 'left', field: 'awbNo' },
  { name: 'transit', label: 'Transit', align: 'left', field: 'awbNo' },
  { name: 'pcWtVol', label: 'PC/WT/VOL', align: 'left', field: 'awbNo'},
  { name: 'uldQty', label: 'ULD(Quantity)', align: 'left', field: 'awbNo'},
  { name: 'shc', label: 'SHC', align: 'left', field: 'awbNo'},

]

const importRows = [
  { 
    awbNo: '000-11111111',
    ori: 'HKG',
    des: 'LHR',
    transit: 'CX002/02Oct\r\nIST-LHR',
    pcWtVol: '1pc/1600kg/10MC',
    uld: 'A2',
    qty: '1',
    shc: 'HEA'
  },
  {
    awbNo: '000-22222222',
    ori: 'HKG',
    des: 'MXP',
    transit: 'CX003/02Oct\r\nIST-MXP',
    pcWtVol: '1pc/2000kg/12MC',
    uld: 'H2',
    qty: '1',
    shc: 'COL'
  },
]



const goToRoutMapPage = (evt, row) => {
  console.log(row);
  //router.push({ name: 'RouteMapPage', params: { awbNo: row.awbNo } });
}
</script>

<template>

  <div class="u-mb-32px title-container">
    <div class="u-text-32px u-fw600 u-c-EzyBooking-green">Arrival</div>
    <p class="u-text-20px u-fw600 u-c-EzyBooking-light-gray-3">
      {{ flightNo }}/01OCT &nbsp;&nbsp;&nbsp;&nbsp; {{ origin }} to {{ destination }} &nbsp;&nbsp;&nbsp;&nbsp; STD / ATD: {{ stdOrAtd }} &nbsp;&nbsp;&nbsp;&nbsp; STA / ETA: {{ staOrEta }}
    </p>  
  </div>

  <hr>

  <q-table
    flat bordered
    title="Transit"
    :rows="transitRows"
    :columns="transitColumns"
    row-key="flightNo"        
    :rows-per-page-options="[0]"
    @row-click="goToRoutMapPage"
  >
    <template v-slot:body-cell-uldQty="props">      
      <q-td :props="props">
        {{ props.row.uld }} ( {{ props.row.qty }} )
      </q-td>
    </template>

  </q-table>  

  <br>

  <q-table
    flat bordered
    title="Import"
    :rows="importRows"
    :columns="importColumns"
    row-key="flightNo"        
    :rows-per-page-options="[0]"
    @row-click="goToRoutMapPage"
  >
  </q-table>  
  
  






  <button @click="handleClick">Click Me</button>


</template>
<script setup>
console.log("Init Flight Shipment");
import { ref } from 'vue'
import { useRouter } from 'vue-router';

const router = useRouter()
const tab = ref('departure')

// Departure DATA 
const depColumns = [
  {
    name: 'flightNo',
    required: true,
    label: 'Flight No.',
    align: 'left',
    field: row => row.flightNo,
    format: val => `${val}`,
    //format: val => `<q-badge rounded color="orange" label="${val}" /> `,
    sortable: true
  },
  { name: 'type', align: 'center', label: 'Type', field: 'type', sortable: true },
  { name: 'ori', label: 'ORI', field: 'ori', sortable: true },
  { name: 'des', label: 'DES', field: 'des' },
  { name: 'stdOrSta', label: 'STD/STA', field: 'stdOrSta' },
  { name: 'etdOrEta', label: 'ETD/ETA', field: 'etdOrEta' },
  { name: 'atdOrAta', label: 'ATD/ATA', field: 'atdOrAta' },
  { name: 'status', label: 'Status', field: 'status' }
]

const depRows = [
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',

    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Delayed'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Delayed'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Delayed'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    

    std: '23:00',
    sta: '07:00',
    etd: '22:50',
    eta: '06:35',
    atd: '22:50',
    ata: '06:35',
    status: 'Normal'
  }
]

//Arrival DATA
const arrColumns = [
{
    name: 'flightNo',
    required: true,
    label: 'Flight No.',
    align: 'left',
    field: row => row.flightNo,
    format: val => `${val}`,
    //format: val => `<q-badge rounded color="orange" label="${val}" /> `,
    sortable: true
  },
  { name: 'type', align: 'center', label: 'Type', field: 'type', sortable: true },
  { name: 'ori', label: 'ORI', field: 'ori', sortable: true },
  { name: 'des', label: 'DES', field: 'des' },
  { name: 'stdOrSta', label: 'STD/STA', field: 'stdOrSta' },
  { name: 'etdOrEta', label: 'ETD/ETA', field: 'etdOrEta' },
  { name: 'atdOrAta', label: 'ATD/ATA', field: 'atdOrAta' },
  { name: 'status', label: 'Status', field: 'status' }
]

const arrRows = [
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Normal'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Delay'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Awareness'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Processing'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Processing'
  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Completed'

  },
  {
    flightNo: 'CX501',
    type: 'ARR',
    ori: 'HKG',
    des: 'LHR',
    
    
    
    status: 'Delay'
  }
]


const goToArrivalPage = (evt, row) => {
  console.log('goToArrivalPage', row)
  router.push(
    { 
      name: 'Arrival', 
      params: 
      { 
        flightNo: row.flightNo
      },
      query:
      {
        ori: row.ori,
        des: row.des,
        stdOrAtd: row.std + ' / ' + row.atd,
        staOrEta: row.sta + ' / ' + row.eta
      } 
    }
  );
}

function badgeColor (value) {
  switch (value) {
    case "Normal":
      return 'red'
    case 'Delayed':
      return 'red'
    case 'Awareness':
      return 'awareness'
    case 'Completed':
      return 'completed'
    default:
      return 'black'
  }
}

function badgeTextColor (value) {
  switch (value) {
    case 'Normal':
      return 'red'
    case 'Delayed':
      return 'red'
    case 'Awareness':
      return 'awareness'
    case 'Completed':
      return 'completed'
    default:
      return 'black'
  }
}

</script>

<template>
  <div>
    
    <div class="u-mb-32px title-container">
      <div class="u-text-32px u-fw600 u-c-EzyBooking-green">Flight Shipment</div>
      <div class="title-icon">
        <!-- Inline SVG -->
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16" fill="none">
          <path d="M12.6667 2.66675H3.33333C2.59695 2.66675 2 3.2637 2 4.00008V13.3334C2 14.0698 2.59695 14.6667 3.33333 14.6667H12.6667C13.403 14.6667 14 14.0698 14 13.3334V4.00008C14 3.2637 13.403 2.66675 12.6667 2.66675Z" stroke="#353C3C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M2 6.66675H14" stroke="#353C3C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M10.6667 1.33325V3.99992" stroke="#353C3C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M5.33334 1.33325V3.99992" stroke="#353C3C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        Today
      </div>
    </div>

    <q-card flat>
      <q-tabs
        v-model="tab"
        active-color="EzyBooking-green"
        indicator-color="EzyBooking-green"
        align="left"
        narrow-indicator
      >
        <q-tab name="departure" label="Departure" />
        <q-tab name="arrival" label="Arrival" />

      </q-tabs>    

      <q-tab-panels v-model="tab" animated>
        <q-tab-panel name="departure">
         
          <q-table
            flat bordered
            title=""
            :rows="depRows"
            :columns="depColumns"
            row-key="flightNo"        
            :rows-per-page-options="[0]"
            @row-click="goToArrivalPage"
          >
            <template v-slot:body-cell-flightNo="props">
              <q-td :props="props">
                {{ props.row.flightNo }}
              </q-td>
            </template>

            <template v-slot:body-cell-stdOrSta="props">      
              <q-td :props="props">
                {{ props.row.std }} / {{ props.row.sta }} 
              </q-td>
            </template>

            <template v-slot:body-cell-etdOrEta="props">      
              <q-td :props="props">
                {{ props.row.etd }} / {{ props.row.eta }} 
              </q-td>
            </template>

            <template v-slot:body-cell-atdOrAta="props">      
              <q-td :props="props">
                {{ props.row.atd }} / {{ props.row.ata }} 
              </q-td>
            </template>

            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-badge 
                  class="my-badge" 
                  :rounded="true"                   
                  :color="badgeColor(props.row.status)" 
                  :text-color="badgeTextColor(props.row.status)" 
                  :label="props.row.status" />
              </q-td>
            </template>
          </q-table>
        </q-tab-panel>

        <q-tab-panel name="arrival">
          
          <q-table
            flat bordered
            title=""          
            :rows="arrRows"
            :columns="arrColumns"
            row-key="name"
            :rows-per-page-options="[0]"
          >
            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-badge 
                  class="my-badge" 
                  :rounded="true" 
                  
                  :color="badgeColor(props.row.status)" 
                  :text-color="badgeTextColor(props.row.status)" 
                  :label="props.row.status" />
              </q-td>
            </template>
          </q-table>
        </q-tab-panel>

      
      </q-tab-panels>
    </q-card>


  </div>

</template>

<style scoped>
  

  .title-container {
    display: flex;
    align-items: center;
    gap: 39px; /* Adjust the gap as needed */
  } 

  .title-icon {
    display: flex;
    align-items: center;
    gap: 4px; /* Adjust the gap as needed */
  }
</style>


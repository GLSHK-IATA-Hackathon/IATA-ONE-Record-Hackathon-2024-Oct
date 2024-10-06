import { ref, reactive } from 'vue'
import { defineStore } from 'pinia'

import { DetailTypeEnum } from '@/enums/CommonEnum'

export const useDeviceStore = defineStore('device', () => {
  const tabs = reactive({
    [DetailTypeEnum.OVERVIEW]: {
      name: DetailTypeEnum.OVERVIEW,
      label: 'Overview',
      display: true,
    },
    [DetailTypeEnum.ULTRATRACK]: {
      name: DetailTypeEnum.ULTRATRACK,
      label: 'Ultratrack',
      display: true,
    },
    [DetailTypeEnum.TEMP_VOLTAGE]: {
      name: DetailTypeEnum.TEMP_VOLTAGE,
      label: 'Temp & Voltage',
      display: true,
    },
  })

  const sensorReadingDevices = ref([])

  const ultraTrackGridData = reactive({
    rows: [],
    columns: [],
  })

  const tempAndVoltageGridData = reactive({
    rows: [],
    columns: [],
  })

  const lastAvailableSensorReading = reactive({
    Temperature: {
      isActive: false,
      data: '',
      time: '',
    },
    Humidity: {
      isActive: false,
      data: '',
      time: '',
    },
    Light: {
      isActive: false,
      data: '',
      time: '',
    },
    Shock: {
      isActive: false,
      data: '',
      time: '',
    },
    PresetTemperature: {
      isActive: false,
      data: '',
      time: '',
    },
    InsideTemperatureHigh: {
      isActive: false,
      data: '',
      time: '',
    },
    InsideTemperatureLow: {
      isActive: false,
      data: '',
      time: '',
    },
    BatteryLevel: {
      isActive: false,
      data: '',
      time: '',
    },
  })

  function updateTabStatus(tabName, isDisplayed) {
    tabs[tabName].display = isDisplayed
  }

  function updateUltraTrackGridData(propertyName, updateValue) {
    if (!ultraTrackGridData.hasOwnProperty(propertyName)) return

    ultraTrackGridData[propertyName] = updateValue
  }

  function updateTempAndVoltageGridData(propertyName, updateValue) {
    if (!tempAndVoltageGridData.hasOwnProperty(propertyName)) return

    tempAndVoltageGridData[propertyName] = updateValue
  }

  function updateLastAvailableSensorReading(
    sensorName,
    propertyName,
    updateValue
  ) {
    if (
      !lastAvailableSensorReading.hasOwnProperty(sensorName) ||
      !lastAvailableSensorReading[sensorName].hasOwnProperty(propertyName)
    )
      return
    lastAvailableSensorReading[sensorName][propertyName] = updateValue
  }

  function addSensorReadingDevices(deviceProps) {
    sensorReadingDevices.value.push(deviceProps)
  }

  function initSensorReadingDevices() {
    sensorReadingDevices.value = []
  }

  function initUltraTrackGrid() {
    ultraTrackGridData.value = []
  }

  function initTempAndVoltageGridData() {
    tempAndVoltageGridData.value = []
  }

  function initLastAvailableSensorReading() {
    Object.values(lastAvailableSensorReading).forEach((value) => {
      value.isActive = false
      value.data = ''
      value.time = ''
    })
  }

  return {
    // data
    tabs,
    ultraTrackGridData,
    tempAndVoltageGridData,
    lastAvailableSensorReading,
    sensorReadingDevices,

    // method
    updateUltraTrackGridData,
    updateTempAndVoltageGridData,
    updateLastAvailableSensorReading,
    updateTabStatus,
    addSensorReadingDevices,
    initSensorReadingDevices,
    initUltraTrackGrid,
    initTempAndVoltageGridData,
    initLastAvailableSensorReading,
  }
})

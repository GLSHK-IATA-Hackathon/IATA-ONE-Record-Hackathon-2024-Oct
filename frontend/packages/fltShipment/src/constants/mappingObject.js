import { UnitCodeEnum } from '@/enums/CommonEnum'

export const eventMappingObject = {
  DIS: 'Discrepancy',
  AWD: 'Document Delivered to Forwarder',
  AWR: 'Documents Received',
  NFD: 'Freight Ready for Pick Up',
  ARR: 'Arrived',
  CCD: 'Custom Cleared',
  TRM: 'Freight to be Transferred to Airline',
  MAN: 'Manifest Received',
  DLV: 'Delivered',
  DDL: 'Delivered',
  RCF: 'Freight accepted at Airport',
  RCS: 'Airline Received',
  RCT: 'Freight Received from Airline',
  TFD: 'Freight Transferred to Airline',
  PRE: 'Prepared for Loading',
  CRC: 'Reported to the Customs',
  TGC: 'Transferred to Customs Control',
  DEP: 'Departed',
  FOH: 'Freight on Hand',
  DOC: 'Documents Received',
  OCI: 'Other Information',
}

export const discrepancyDDCodeMappingObject = {
  FDCA: 'Found cargo',
  OFLD: 'Offload',
  MSAW: 'Missing AWB',
  SSPD: 'Short shipped',
  OVCD: 'Over carried',
}

export const weightCodeMappingObject = {
  [UnitCodeEnum.K]: 'kg',
  [UnitCodeEnum.L]: 'lb',
}

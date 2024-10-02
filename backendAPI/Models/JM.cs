using static WebAPITemplate.Models.JMCommon;

namespace WebAPITemplate.Models
{
    public static class JMCommon
    {

        public enum WayBillType
        {
            DIRECT = 1,
            MASTER = 2,
            HOUSE = 3
        }
        public static WayBillType? GetWayBillTypeEnum(string input)
        {
            if (Enum.TryParse(input, out WayBillType retEnum))
            {
                return retEnum;
            }
            return null;
        }
        public static string GetWayBillTypeEnumName(WayBillType input)
        {
            return Enum.GetName(typeof(WayBillType), input);
        }
        public enum EventTimeType
        {
            ACTUAL = 1,
            ESTIMATED = 2,
            EXPECTED = 3,
            PLANNED = 4,
            REQUESTED = 5
        }
        public static EventTimeType? GetEventTimeTypeEnum(string input)
        {
            if (Enum.TryParse(input, out EventTimeType retEnum))
            {
                return retEnum;
            }
            return null;
        }
        public static string GetEventTimeTypeEnumName(EventTimeType input)
        {
            return Enum.GetName(typeof(EventTimeType), input);
        }
        public enum DefaultMileStoneType
        {
            UWS = 1,
            LIR = 2,
            FOW = 3,
            ALS = 4,
            ALE = 5,
            OFB = 6,
            WUP = 7,
            FFM = 8,
            WDO = 9,
            ONB = 10,
            AUS = 11,
            AUE = 12,
            FIW = 13,
        }
        public static DefaultMileStoneType? GetDefaultMileStoneTypeEnum(string input)
        {
            if (Enum.TryParse(input, out DefaultMileStoneType retEnum))
            {
                return retEnum;
            }
            return null;
        }
        public static string GetDefaultMileStoneTypeEnumName(DefaultMileStoneType input)
        {
            return Enum.GetName(typeof(DefaultMileStoneType), input);
        }
        public static Dictionary<DefaultMileStoneType, string> DefaultMileStoneDict = new Dictionary<DefaultMileStoneType, string>
        {
            {DefaultMileStoneType.UWS, "Finish Palletizing"},
            {DefaultMileStoneType.LIR, "Load Plan Ready"},
            {DefaultMileStoneType.FOW, "Freight out of Warehouse"},
            {DefaultMileStoneType.ALS, "Aircraft Loading Started"},
            {DefaultMileStoneType.ALE, "Aircraft Loading Ended "},
            {DefaultMileStoneType.OFB, "Off Blocks​"},
            {DefaultMileStoneType.WUP, "Wheels Up​"},
            {DefaultMileStoneType.FFM, "Updated Flight Manifest"},
            {DefaultMileStoneType.WDO, "Wheels Down​"},
            {DefaultMileStoneType.ONB, "On Blocks​"},
            {DefaultMileStoneType.AUS, "Aircraft Unloading Started​"},
            {DefaultMileStoneType.AUE, "Aircraft Unoading Ended"},
            {DefaultMileStoneType.FIW, "Freight in to Warehouse"},
        };
        public static string GetDefaultMileStoneDesc(string type)
        {
            string retVal = "";
            if (GetDefaultMileStoneTypeEnum(type).HasValue)
            {
                retVal = GetDefaultMileStoneDesc(GetDefaultMileStoneTypeEnum(type).Value);
            }
            return retVal;
        }
        public static string GetDefaultMileStoneDesc(DefaultMileStoneType type)
        {
            string retVal = "";
            if (DefaultMileStoneDict.TryGetValue(type, out string desc))
                retVal = desc;
            return retVal;
        }
    }
    public class JMWayBill
    {
        public string id { get; set; } = "";
        public const string type = "Waybill";
        public JMCommon.WayBillType waybillType { get; set; } = JMCommon.WayBillType.MASTER;
        public string waybillPrefix { get; set; } = "";
        public string waybillNumber { get; set; } = "";
    }

    public class JMCodeListElement
    {


    }


    public class JMInsertWaybillObj
    {
        public string id { get; set; } = "";
        public string waybillPrefix { get; set; } = "";
        public string waybillNumber { get; set; } = "";
        public WayBillType waybillType { get; set; } = WayBillType.MASTER;
        public string arrivalLocation { get; set; } = "";   
        public string departureLocation { get; set; } = "";
        public JMInsertShipmentObj shipment { get; set; } = new JMInsertShipmentObj();
    }
    public class JMInsertShipmentObj
    {
        public string id { get; set; } = "";
        public string goodsDescription { get; set; } = "";
        public Dictionary<string, List<JMInsertLEDetail>> stationEventList { get; set; } = new Dictionary<string, List<JMInsertLEDetail>>();
    }
    public class JMInsertLEDetail
    {
        public EventTimeType eventTimeType { get; set; } = EventTimeType.ACTUAL;
        public DateTime eventDate { get; set; } = DateTime.Now;
        public string mileStoneCode { get; set; } = "";
        private string _mileStoneDesc = "";
        public string mileStoneDesc
        {
            get
            {
                return _mileStoneDesc;
            }
            set
            {
                this._mileStoneDesc = value;
                if (string.IsNullOrEmpty(this._mileStoneDesc))
                {
                    this._mileStoneDesc = GetDefaultMileStoneDesc(this.mileStoneCode);
                }
            }
        }
        private string _eventName = "";
        public string eventName
        {
            get
            {
                return _eventName;
            }
            set
            {
                this._eventName = value;
                if (string.IsNullOrEmpty(this._eventName))
                {
                    this._eventName = GetDefaultMileStoneDesc(this.mileStoneCode);
                }
            }
        }
        public JMInsertLEDetail()
        {

        }
        public JMInsertLEDetail(EventTimeType eventTimeType, string mileStoneCode)
        {
            this.eventTimeType = eventTimeType;
            this.mileStoneCode = mileStoneCode;
        }
        public JMInsertLEDetail(EventTimeType eventTimeType, string mileStoneCode, DateTime eventDate, string mileStoneDesc, string eventName)
        {
            this.eventTimeType = eventTimeType;
            this.eventDate = eventDate;
            this.mileStoneCode = mileStoneCode;
            this.mileStoneDesc = mileStoneDesc;
            this.eventName = eventName;
        }
    }
}


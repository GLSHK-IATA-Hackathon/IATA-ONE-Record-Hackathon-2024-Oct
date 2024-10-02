using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net;
using System.Text.Json;
using WebAPITemplate.Attributes;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;
using WebAPITemplate.Swagger;
using static WebAPITemplate.Models.JMCommon;

namespace WebAPITemplate.Controllers
{

    [AllowAnonymous]
    [Route("api/")]
    [ApiController]
    public class JMController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INEOneAPIService _NEOneAPIService;
        private string acessToken = "";
        private string logisticsObjectServerPath = "";
        private bool createDEP = false;
        private bool createMAN = false;

        public JMController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            createDEP = _config.GetSection("AppSettings:CreateDEP").Get<bool>();
            createMAN = _config.GetSection("AppSettings:CreateMAN").Get<bool>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;
        }

        [Route("test")]
        [TokenAuthorize]
        [HttpPost]
        public string test()
        {
         Dictionary<string, List<JMInsertLEDetail>> stationEventList = new Dictionary<string, List<JMInsertLEDetail>>();
            stationEventList.Add("LAX", new List<JMInsertLEDetail> { new JMInsertLEDetail(EventTimeType.ESTIMATED, "WDO"), new JMInsertLEDetail(EventTimeType.ACTUAL, "WDO") });
            stationEventList.Add("NRT", new List<JMInsertLEDetail> { new JMInsertLEDetail(EventTimeType.ESTIMATED, "FOW"), new JMInsertLEDetail(EventTimeType.PLANNED, "WDUWSO") });
            return JsonSerializer.Serialize(stationEventList);
        }

        [Route("createWaybill")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateWaybill(JMInsertWaybillObj input)
        {


            string responseBody;
            string createStr = $@"
{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
{(string.IsNullOrEmpty(input.id) ? "" : $@"""@id"": ""http://localhost:8080/logistics-objects/{input.id}"",")}
   ""@type"":""cargo:Waybill"",
   ""cargo:waybillType"":{{
      ""@id"":""cargo:{JMCommon.GetWayBillTypeEnumName(input.waybillType)}""
   }},
   ""cargo:waybillPrefix"":""{input.waybillPrefix}"",
   ""cargo:waybillNumber"":""{input.waybillNumber}"",   

{(string.IsNullOrEmpty(input.arrivalLocation) ? "" : $@"
    ""cargo:arrivalLocation"":{{
      ""@type"":""cargo:Location"",
      ""cargo:locationCodes"":[
         {{
            ""@type"":""cargo:CodeListElement"",
            ""cargo:code"":""{input.arrivalLocation}""
         }}
      ],
      ""cargo:locationType"":""AIRPORT""
   }},
")}
{(string.IsNullOrEmpty(input.departureLocation) ? "" : $@"
    ""cargo:departureLocation"":{{
      ""@type"":""cargo:Location"",
      ""cargo:locationCodes"":[
         {{
            ""@type"":""cargo:CodeListElement"",
            ""cargo:code"":""{input.departureLocation}""
         }}
      ],
      ""cargo:locationType"":""AIRPORT""
   }},
")}
";
            if (input.shipment != default)
            {
                createStr += $@"
""cargo:shipment"":{{
{(string.IsNullOrEmpty(input.shipment.id) ? "" : $@"""@id"": ""http://localhost:8080/logistics-objects/{input.shipment.id}"",")}
{(string.IsNullOrEmpty(input.shipment.goodsDescription) ? "" : $@"""cargo:goodsDescription"": ""{input.shipment.goodsDescription}"",")}
";
                if (input.shipment.stationEventList.Count > 0)
                {
                    createStr += $@"
""cargo:events"":[
";
                    var insertStrList = new List<string>();
                    foreach (var item in input.shipment.stationEventList)
                    {
                        var station = item.Key;
                        foreach (var eventObj in item.Value)
                        {
                            insertStrList.Add($@"
{{
    ""@type"": ""cargo:LogisticsEvent"",
    ""cargo:creationDate"": {{
        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
        ""@value"": ""{DateTime.Now.ToString("yyyy/MM/ddTHH:mm:ss.000Z")}""
    }},
    ""cargo:eventDate"": {{
        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
        ""@value"": ""{eventObj.eventDate.ToString("yyyy/MM/ddTHH:mm:ss.000Z")}""
    }},
    ""cargo:eventCode"": {{
        ""@type"": ""cargo:CodeListElement"",
        ""cargo:code"": ""{eventObj.mileStoneCode}"",
        ""cargo:codeListName"": ""{eventObj.mileStoneDesc}""
    }},
    ""cargo:eventName"": ""{eventObj.eventName}"",
    ""cargo:eventTimeType"": {{
        ""@id"": ""cargo:{JMCommon.GetEventTimeTypeEnumName(eventObj.eventTimeType)}"",
        ""@type"": ""cargo:EventTimeType""
    }}
}}
");

                        }
                    }
                    createStr += string.Join(",", insertStrList);
                    createStr += $@"
],
";
                }
                createStr += $@"
      ""@type"":""cargo:Shipment""
}}
";
            }



            createStr += $@"
}}
";


            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {
                return new Response() { status = 1, message = "" };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }


        }
    }



}

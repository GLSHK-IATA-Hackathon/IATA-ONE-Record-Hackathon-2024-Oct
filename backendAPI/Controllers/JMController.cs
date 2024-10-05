using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly bool isDebugmode;
        //private string debugGetTokenBaseUrl = "";
        //private string debugGetTokenUsername = "";
        //private string debugGetTokenPassword = "";

        public JMController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            createDEP = _config.GetSection("AppSettings:CreateDEP").Get<bool>();
            createMAN = _config.GetSection("AppSettings:CreateMAN").Get<bool>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;


            isDebugmode = _config.GetSection("AppSettings:IsDebugMode").Get<bool>();
            //this.debugGetTokenBaseUrl = _config.GetSection("APIConfig:NEOneAPI:debugGetTokenBaseUrl").Get<string>();
            //this.debugGetTokenUsername = _config.GetSection("APIConfig:NEOneAPI:debugGetTokenUsername").Get<string>();
            //this.debugGetTokenPassword = _config.GetSection("APIConfig:NEOneAPI:debugGetTokenPassword").Get<string>();
            //if(!string.IsNullOrEmpty(debugGetTokenBaseUrl) && !string.IsNullOrEmpty(debugGetTokenUsername) && !string.IsNullOrEmpty(debugGetTokenPassword))
            //{
            //    HttpResponseMessage response = await _NEOneAPIService.DebugModeGetToken(debugGetTokenBaseUrl, debugGetTokenUsername, debugGetTokenPassword);

            //}
        }


        [Route("createWaybill")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateWaybill(JMInsertObj input)
        {
            if (isDebugmode)//&& !string.IsNullOrEmpty(debugGetTokenBaseUrl) && !string.IsNullOrEmpty(debugGetTokenUsername) && !string.IsNullOrEmpty(debugGetTokenPassword)
            {
                var tokenResponse = await GenToken();
                if (!string.IsNullOrEmpty(tokenResponse.message))
                {
                    var token_json_u = (JObject)JsonConvert.DeserializeObject(tokenResponse.message);
                    string token_json_result = token_json_u["access_token"].Value<string>();
                    if (!string.IsNullOrEmpty(token_json_result))
                    {
                        acessToken = token_json_result;
                    }
                }

                //HttpResponseMessage getTokenResponse = await _NEOneAPIService.DebugModeGetToken(this.debugGetTokenBaseUrl, this.debugGetTokenUsername, this.debugGetTokenPassword);
                //if (getTokenResponse.IsSuccessStatusCode)
                //{
                //    var tokenResponse = await getTokenResponse.Content.ReadAsStringAsync();
                //    var token_json_u = (JObject)JsonConvert.DeserializeObject(tokenResponse);
                //    string token_json_result = token_json_u["access_token"].Value<string>();
                //    if (!string.IsNullOrEmpty(token_json_result))
                //    {
                //        acessToken = token_json_result;
                //    }
                //}
            }

            input.stationDict = new Dictionary<string, string>();
            input.ghaDict = new Dictionary<string, string>();

            var stationList = new List<string>();
            stationList.Add(input.arrivalLocation);
            stationList.Add(input.departureLocation);
            stationList.AddRange(input.shipment.stationEventList.Keys);
            stationList = stationList.Distinct().ToList();
            foreach (var station in stationList)
            {
                var stationResponse = await CreateLocation(station);
                input.stationDict.Add(station, stationResponse.message);
                var ghaResponse = await CreateGHA(station);
                input.ghaDict.Add(station, ghaResponse.message);
            }
            var glsAirlineResponse = await CreateGLSAirline();
            var glsAgentResponse = await CreateGLSAgent();
            input.glsAirlineLink = glsAirlineResponse.message;
            input.glsAgentLink = glsAgentResponse.message;

            var waybillChar = "";
            switch (input.waybill.waybillType)
            {
                case WayBillType.MASTER:
                    waybillChar = "m";
                    break;
                case WayBillType.HOUSE:
                    waybillChar = "h";
                    break;
                case WayBillType.DIRECT:
                    waybillChar = "d";
                    break;
            }

            input.waybillID = $"gls{waybillChar}awb_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
            input.bookingID = $"glsbooking_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
            input.shipmentID = $"glsshipment_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
            var chkIndex = 0;
            while (true && chkIndex < 100)
            {
                HttpResponseMessage chkResponse = await _NEOneAPIService.GetLogisticsObject(input.waybillID, acessToken);
                if (chkResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
                else
                {
                    chkIndex++;
                    input.waybillID = $"gls{waybillChar}awb_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                    input.bookingID = $"glsbooking_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                    input.shipmentID = $"glsshipment_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                }
            }
            var chkAircraftResponse = await CreateAircraft(input.flightNo);
            input.aircraftLink = chkAircraftResponse.message;


            var chkTransportMovementResponse = await CreateTransportMovement(input);
            input.transportMovementLink = chkTransportMovementResponse.message;
            var chkBookingResponse = await CreateBooking(input);
            input.bookingLink = chkBookingResponse.message;
            var chkShippmentResponse = await CreateShippment(input);
            input.shipmentLink = chkShippmentResponse.message;



            string responseBody;
            string createStr = $@"
{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
   ""@id"": ""{logisticsObjectServerPath}{input.waybillID}"",
   ""cargo:waybillType"":{{
      ""@id"":""cargo:{JMCommon.GetWayBillTypeEnumName(input.waybill.waybillType)}""
   }},
   ""cargo:waybillPrefix"":""{input.waybill.waybillPrefix}"",
   ""cargo:waybillNumber"":""{input.waybill.waybillNumber}"",   


{(string.IsNullOrEmpty(input.arrivalLocation) ? "" : $@"
    ""cargo:arrivalLocation"":{{
     ""@id"":""{input.stationDict[input.arrivalLocation]}""
   }},
")}
{(string.IsNullOrEmpty(input.departureLocation) ? "" : $@"
    ""cargo:departureLocation"":{{
     ""@id"":""{input.stationDict[input.departureLocation]}""
   }},
")}
   ""cargo:referredBookingOption"":{{
      ""@id"":""{input.bookingLink}""
   }},
   ""cargo:shipment"":{{
      ""@id"":""{input.shipmentLink}""
   }},
";
            if (input.ghaDict.Count > 0)
            {
                var insertStrList = new List<string>();
                foreach (var gha in input.ghaDict)
                {
                    insertStrList.Add($@"{{""@id"":""{gha.Value}""}}");
                }
                insertStrList.Add($@"{{""@id"":""{input.glsAirlineLink}""}}");
                insertStrList.Add($@"{{""@id"":""{input.glsAgentLink}""}}");
                createStr += $@"""cargo:involvedParties"": [{string.Join(',', insertStrList)}],";
            }
            createStr += @$"""@type"":""cargo:Waybill""";




            createStr += $@"
}}
";


            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {
                return new Response() { status = 1, message = $@"Successfully create for {logisticsObjectServerPath}{input.waybillID}" };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }


        }
        [Route("createLocation")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateLocation(string stationCode)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glsairport_{stationCode}";
            string createStr = $@"{{
                ""@context"": {{
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                }},
                ""@id"": ""{retPath}"",
                ""@type"": ""cargo:Location"",
                ""cargo:locationType"": ""Airport"",
                 ""cargo:locationCodes"": [{{
                    ""@type"": ""cargo:CodeListElement"",
                        ""cargo:code"": ""{stationCode}"",
                        ""cargo:codeListName"": ""IATA Airline Coding Directory and Location Identifiers (ACD) - Airport Codes"",
                        ""cargo:codeListVersion"": ""2024 Edition for GLS""
                }}]
    
            }}";

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }


        [Route("createGHA")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateGHA(string stationCode)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glsgha_{stationCode}";
            string createStr = $@"{{
                ""@context"": {{
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                }},
                ""@id"": ""{retPath}"",
               ""@type"": ""cargo:Company"",
                ""cargo:name"": ""{stationCode} Handling Services""
            }}";

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }
        [Route("createGLSAirline")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateGLSAirline()
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glsairline_GLS";
            string createStr = $@"{{
                ""@context"": {{
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                }},
                ""@id"": ""{retPath}"",
                ""@type"": ""cargo:Carrier"",
                ""cargo:shortName"": ""GLS Airlines""
            }}";

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }
        [Route("createGLSAgent")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateGLSAgent()
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glsagent_GLS";
            string createStr = $@"{{
                ""@context"": {{
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                }},
                ""@id"": ""{retPath}"",
                 ""@type"": [
                    ""cargo:LogisticsObject"",
                    ""cargo:Company""
                ],
                ""cargo:name"": ""GLS Logísticos A.G.""
            }}";

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }
        [Route("createAircraft")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateAircraft(string aircraftCode)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glsaircraft_{aircraftCode}";
            string createStr = $@"{{
                ""@context"": {{
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                }},
                ""@id"": ""{retPath}"",
                ""@type"": [
                    ""cargo:LogisticsObject"",
                    ""cargo:TransportMeans""
                ],
                ""cargo:operatedTransportMovement"": {{
                    ""@id"": ""{logisticsObjectServerPath}glstransportmovement_{aircraftCode}""
                }},
                ""cargo:vehicleRegistration"": ""D-ABBB""
            }}";

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }
        [Route("createTransportMovement")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateTransportMovement(JMInsertObj input)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}glstransportmovement_{input.flightNo}";
            string createStr = $@"{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
   ""@id"": ""{retPath}"",
   ""@type"":""cargo:TransportMovement"",
   ""cargo:arrivalLocation"":{{
      ""@id"":""{input.stationDict[input.arrivalLocation]}""
   }},
   ""cargo:departureLocation"":{{
      ""@id"":""{input.stationDict[input.departureLocation]}""
   }},
   ""cargo:executionStatus"":{{
      ""@id"":""https://onerecord.iata.org/ns/cargo#ACTIVE""
   }},
   ""cargo:loadingActions"":[
      {{
         ""@id"":""http://localhost:8080/logistics-objects/load113-81004864on1R4000""
      }},
      {{
         ""@id"":""http://localhost:8080/logistics-objects/unload113-81004864on1R4000""
      }}
   ],
   ""cargo:modeCode"":{{
      ""@id"":""https://onerecord.iata.org/ns/coreCodeLists#ModeCode_AIR_TRANSPORT""
   }},
   ""cargo:modeQualifier"":{{
      ""@id"":""https://onerecord.iata.org/ns/cargo#MAIN_CARRIAGE""
   }},
   ""cargo:movementTime"":[
      {{
         ""@type"":""cargo:MovementTime"",
         ""cargo:direction"":{{
            ""@id"":""https://onerecord.iata.org/ns/cargo#OUTBOUND""
         }},
         ""cargo:movementMilestone"":{{
            ""@id"":""https://onerecord.iata.org/ns/coreCodeLists#MovementIndicator_EB""
         }},
         ""cargo:movementTimeType"":{{
            ""@id"":""https://onerecord.iata.org/ns/cargo#ESTIMATED""
         }},
         ""cargo:movementTimestamp"":{{
            ""@type"":""http://www.w3.org/2001/XMLSchema#dateTime"",
            ""@value"":""{input.outBoundDate.JMDTToStr(2)}""
         }}
      }},
      {{
         ""@type"":""cargo:MovementTime"",
         ""cargo:direction"":{{
            ""@id"":""https://onerecord.iata.org/ns/cargo#INBOUND""
         }},
         ""cargo:movementMilestone"":{{
            ""@id"":""https://onerecord.iata.org/ns/coreCodeLists#MovementIndicator_EB""
         }},
         ""cargo:movementTimeType"":{{
            ""@id"":""https://onerecord.iata.org/ns/cargo#ESTIMATED""
         }},
         ""cargo:movementTimestamp"":{{
            ""@type"":""http://www.w3.org/2001/XMLSchema#dateTime"",
            ""@value"":""{input.inBoundDate.JMDTToStr(2)}""
         }}
      }}
   ],
   ""cargo:operatingTransportMeans"":{{
      ""@id"":""{logisticsObjectServerPath}glsaircraft_{input.flightNo}""
   }},
   ""cargo:servedServices"":[
      {{
         ""@id"":""{logisticsObjectServerPath}glsbooking_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}""
      }},
      {{
         ""@id"":""http://localhost:8080/logistics-objects/Booking_113-11003087""
      }}
   ],
   ""cargo:transportIdentifier"":""{input.flightNo}""
}}";


            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }
        [Route("createBooking")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateBooking(JMInsertObj input)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}{input.bookingID}";
            string createStr = $@"{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
   ""@id"":""{retPath}"",
   ""@type"":[
      ""cargo:LogisticsObject"",
      ""cargo:Booking""
   ],
   ""cargo:activitySequences"":[
      {{
         ""@type"":""cargo:ActivitySequence"",
         ""cargo:activity"":{{
            ""@id"":""{logisticsObjectServerPath}glstransportmovement_{input.flightNo}""
         }}
      }}
   ],
   ""cargo:issuedForWaybill"":{{
      ""@id"":""{logisticsObjectServerPath}{input.waybillID}""
   }}
}}";


            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }

        [Route("createShippment")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> CreateShippment(JMInsertObj input)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}{input.shipmentID}";
            string createStr = $@"
{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
    ""@id"": ""{retPath}"",
    ""cargo:goodsDescription"": ""{(string.IsNullOrEmpty(input.shipment.goodsDescription) ? "Goods Description" : $@"{input.shipment.goodsDescription}")}"",
    ""cargo:waybill"": {{
        ""@id"": ""{logisticsObjectServerPath}{input.waybillID}""
    }},
    ""cargo:pieces"": [
        {{
            ""@id"": ""http://localhost:8080/logistics-objects/Piece95116595""
        }},
        {{
            ""@id"": ""http://localhost:8080/logistics-objects/Piece239443240""
        }},
        {{
            ""@id"": ""http://localhost:8080/logistics-objects/Piece383769885""
        }},
        {{
            ""@id"": ""http://localhost:8080/logistics-objects/Piece811751391""
        }}
    ],
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
        ""@value"": ""{DateTime.Now.JMDTToStr()}""
    }},
    ""cargo:eventDate"": {{
        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
        ""@value"": ""{eventObj.eventDate.JMDTToStr()}""
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
    }},
    ""cargo:eventLocation"": {{""@id"":""{input.stationDict[station]}""}}
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
        ""@type"":[
            ""cargo:LogisticsObject"",
            ""cargo:Shipment""
        ]
}}
";






            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                return new Response() { status = 1, message = retPath };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = retPath };
            }

        }

        [Route("genToken")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> GenToken()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8989/realms/neone/protocol/openid-connect/token");
            request.Headers.Add("Authorization", "Basic bmVvbmUtY2xpZW50Omx4N1RoUzVhWWdnZHNNbTQyQlAzd01yVnFLbTlXcE5Z");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("grant_type", "client_credentials"));
            collection.Add(new("client_id", "neone-client"));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return new Response() { status = response.IsSuccessStatusCode ? 1 : 0, message = await response.Content.ReadAsStringAsync() };

        }
    }



    public static class JMGeneralClass
    {
        private static string GetDTFormat(int getType)
        {
            string format = "";
            switch (getType)
            {
                case 1:
                    format = "yyyy/MM/ddTHH:mm:ss.000Z";
                    break;
                case 2:
                    format = "yyyy-MM-ddTHH:mm:ss+00:00";
                    break;
            }
            return format;
        }
        public static string JMDTToStr(this DateTime dt, int getType = 1)
        {
            string format = GetDTFormat(getType);
            return string.IsNullOrEmpty(format) ? "" : dt.ToString(format);
        }
    }
}

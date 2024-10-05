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
        private bool isDebugmode = true;
        private bool eventIfAppendInObj = true;

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
            eventIfAppendInObj = _config.GetSection("AppSettings:eventIfAppendInObj").Get<bool>();
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
            //stationList.AddRange(input.waybillList.Select(x => x.arrivalLocation));
            //stationList.AddRange(input.waybillList.Select(x => x.departureLocation));
            stationList.Add(input.waybill.arrivalLocation);
            stationList.Add(input.waybill.departureLocation);
            stationList.Add(input.flightArrivalLocation);
            stationList.Add(input.flightDepartureLocation);
            //stationList.AddRange(input.waybillList.Select(x => x.shipment).SelectMany(x => x.stationEventList.Keys));
            stationList.AddRange(input.waybill.shipment.stationEventList.Keys);
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


            var chkIndex = 0;
            #region Create ID
            var waybillChar = "";
            //foreach (var waybill in input.waybillList)
            //{
            //    switch (waybill.waybillType)
            //    {
            //        case WayBillType.MASTER:
            //            waybillChar = "m";
            //            break;
            //        case WayBillType.HOUSE:
            //            waybillChar = "h";
            //            break;
            //        case WayBillType.DIRECT:
            //            waybillChar = "d";
            //            break;
            //    }

            //    waybill.waybillID = $"gls{waybillChar}awb_{waybill.waybillPrefix}-{waybill.waybillNumber}";
            //    waybill.bookingID = $"glsbooking_{waybill.waybillPrefix}-{waybill.waybillNumber}";
            //    waybill.shipmentID = $"glsshipment_{waybill.waybillPrefix}-{waybill.waybillNumber}";
            //    chkIndex = 0;
            //    while (true && chkIndex < 100)
            //    {
            //        HttpResponseMessage chkResponse = await _NEOneAPIService.GetLogisticsObject(waybill.waybillID, acessToken);
            //        if (chkResponse.StatusCode == HttpStatusCode.NotFound)
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            chkIndex++;
            //            waybill.waybillID = $"gls{waybillChar}awb_{waybill.waybillPrefix}-{waybill.waybillNumber}_{chkIndex}";
            //            waybill.bookingID = $"glsbooking_{waybill.waybillPrefix}-{waybill.waybillNumber}_{chkIndex}";
            //            waybill.shipmentID = $"glsshipment_{waybill.waybillPrefix}-{waybill.waybillNumber}_{chkIndex}";
            //        }
            //    }
            //}
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

            input.waybill.waybillID = $"gls{waybillChar}awb_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
            input.waybill.bookingID = $"glsbooking_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
            input.waybill.shipmentID = $"glsshipment_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}";
                chkIndex = 0;
                while (true && chkIndex < 100)
                {
                    HttpResponseMessage chkResponse = await _NEOneAPIService.GetLogisticsObject(input.waybill.waybillID, acessToken);
                    if (chkResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        break;
                    }
                    else
                    {
                        chkIndex++;
                    input.waybill.waybillID = $"gls{waybillChar}awb_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                    input.waybill.bookingID = $"glsbooking_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                    input.waybill.shipmentID = $"glsshipment_{input.waybill.waybillPrefix}-{input.waybill.waybillNumber}_{chkIndex}";
                    }
                }
            

            input.aircraftID = $"glsaircraft_{input.flightNo}";
            chkIndex = 0;
            while (true && chkIndex < 100)
            {
                HttpResponseMessage chkResponse = await _NEOneAPIService.GetLogisticsObject(input.aircraftID, acessToken);
                if (chkResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
                else
                {
                    chkIndex++;
                    input.aircraftID = $"glsaircraft_{input.flightNo}_{chkIndex}";
                }
            }
            input.transportMovementID = $"glstransportmovement_{input.flightNo}";
            chkIndex = 0;
            while (true && chkIndex < 100)
            {
                HttpResponseMessage chkResponse = await _NEOneAPIService.GetLogisticsObject(input.transportMovementID, acessToken);
                if (chkResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
                else
                {
                    chkIndex++;
                    input.transportMovementID = $"glstransportmovement_{input.flightNo}_{chkIndex}";
                }
            }


            #endregion Create ID


            var chkAircraftResponse = await CreateAircraft(input);
            input.aircraftLink = chkAircraftResponse.message;
            var chkTransportMovementResponse = await CreateTransportMovement(input);
            input.transportMovementLink = chkTransportMovementResponse.message;



            var chkBookingResponse = await CreateBooking(input);
            input.waybill.bookingLink = chkBookingResponse.message;
            var chkShippmentResponse = await CreateShippment(input);
            input.waybill.shipmentLink = chkShippmentResponse.message;



            string responseBody;
            var retPath = $"{logisticsObjectServerPath}{input.waybill.waybillID}";
            string createStr = $@"
{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#""
   }},
   ""@id"": ""{retPath}"",
   ""cargo:waybillType"":{{
      ""@id"":""cargo:{JMCommon.GetWayBillTypeEnumName(input.waybill.waybillType)}""
   }},
   ""cargo:waybillPrefix"":""{input.waybill.waybillPrefix}"",
   ""cargo:waybillNumber"":""{input.waybill.waybillNumber}"",   


{(string.IsNullOrEmpty(input.waybill.arrivalLocation) ? "" : $@"
    ""cargo:arrivalLocation"":{{
     ""@id"":""{input.stationDict[input.waybill.arrivalLocation]}""
   }},
")}
{(string.IsNullOrEmpty(input.waybill.departureLocation) ? "" : $@"
    ""cargo:departureLocation"":{{
     ""@id"":""{input.stationDict[input.waybill.departureLocation]}""
   }},
")}
   ""cargo:referredBookingOption"":{{
      ""@id"":""{input.waybill.bookingLink}""
   }},
   ""cargo:shipment"":{{
      ""@id"":""{input.waybill.shipmentLink}""
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
                input.waybill.waybillLink = $"{retPath}";
                return new Response() { status = 1, message = $@"Successfully create for {logisticsObjectServerPath}{input.waybill.waybillID}" };
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
                ""cargo:name"": ""GLS Logistics A.G.""
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
        public async Task<Response> CreateAircraft(JMInsertObj input)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}{input.aircraftID}";
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
                    ""@id"": ""{logisticsObjectServerPath}{input.transportMovementID}""
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
            var retPath = $"{logisticsObjectServerPath}{input.transportMovementID}";

            //var bookingLinkList = input.waybillList.Select(x => $@"{logisticsObjectServerPath}{x.bookingID}").ToList();
            var bookingLink =  $@"{logisticsObjectServerPath}{input.waybill.bookingID}";

            string createStr = $@"{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#"",
      ""@vocab"": ""https://onerecord.iata.org/ns/cargo#""
   }},
   ""@id"": ""{retPath}"",
   ""@type"": [
        ""LogisticsObject"",
        ""TransportMovement"",
        ""LogisticsActivity""
    ],
   ""arrivalLocation"":{{
      ""@id"":""{input.stationDict[input.flightArrivalLocation]}""
   }},
   ""departureLocation"":{{
      ""@id"":""{input.stationDict[input.flightDepartureLocation]}""
   }},
   ""executionStatus"":{{
      ""@id"":""ACTIVE""
   }},
   ""loadingActions"":[
      {{
         ""@id"":""http://localhost:8080/logistics-objects/load113-81004864on1R4000""
      }},
      {{
         ""@id"":""http://localhost:8080/logistics-objects/unload113-81004864on1R4000""
      }}
   ],
   ""modeCode"":{{
      ""@id"":""https://onerecord.iata.org/ns/coreCodeLists#ModeCode_AIR_TRANSPORT""
   }},
   ""modeQualifier"":""MAIN_CARRIAGE"",
   ""movementTimes"":[
      {{
         ""@type"":""movementTime"",
         ""direction"":""OUTBOUND"",
         ""movementMilestone"":""EB"",
         ""movementTimeType"":""ESTIMATED"",
         ""movementTimestamp"":""{input.outBoundDate.JMDTToStr(2)}""
      }},
      {{
         ""@type"":""movementTime"",
         ""direction"":""INBOUND"",
         ""movementMilestone"":""EB"",
         ""movementTimeType"":""ESTIMATED"",
         ""movementTimestamp"":""{input.inBoundDate.JMDTToStr(2)}""
      }}
   ],
   ""operatingTransportMeans"":{{
      ""@id"":""{logisticsObjectServerPath}{input.aircraftID}""
   }},
   ""servedServices"":[
      {{bookingLink}}
   ],
   ""transportIdentifier"":""{input.flightNo}""
}}";
            //{ { { (string.Join("},{", bookingLinkList))} } }



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
            var retPath = $"{logisticsObjectServerPath}{input.waybill.bookingID}";
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
            ""@id"":""{logisticsObjectServerPath}{input.transportMovementID}""
         }}
      }}
   ],
   ""cargo:issuedForWaybill"":{{
      ""@id"":""{logisticsObjectServerPath}{input.waybill.waybillID}""
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

        [Route("createEvents")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<bool> CreateEvents(JMInsertObj input)
        {
            var retVar = false;
            int eventNum = 1;
            if (input.waybill.shipment.stationEventList.Count > 0)
            {
                foreach (var station in input.waybill.shipment.stationEventList)
                {
                    foreach (var eventDetail in station.Value)
                    {
                        if (eventDetail != null)
                        {
                            eventDetail.eventID = $"{input.waybill.shipmentID}_Event{eventNum}";

                            var retPath = $"{logisticsObjectServerPath}{input.waybill.shipmentID}/logistics-events/{eventDetail.eventID}";
                            //""@id"": ""{retPath}"",
                            string createStr = $@"
                                {{
                                    ""@context"": {{
                                        ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                                    }},
                                    ""@type"": ""cargo:LogisticsEvent"",
                                    ""cargo:creationDate"": {{
                                        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                                        ""@value"": ""{DateTime.Now.JMDTToStr(2)}""
                                    }},
                                    ""cargo:eventDate"": {{
                                        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                                        ""@value"": ""{eventDetail.eventDate.JMDTToStr(2)}""
                                    }},
                                    ""cargo:eventCode"": {{
                                        ""@type"": ""cargo:CodeListElement"",
                                        ""cargo:code"": ""{eventDetail.mileStoneCode}"",
                                        ""cargo:codeListName"": ""{eventDetail.mileStoneDesc}""
                                    }},
                                    ""cargo:eventName"": ""{eventDetail.eventName}"",
                                    ""cargo:eventTimeType"": {{
                                        ""@id"": ""cargo:{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}"",
                                        ""@type"": ""cargo:EventTimeType""
                                    }},
                                    ""cargo:eventLocation"": {{""@id"":""{input.stationDict[station.Key]}""}}
                                }}
                            ";
                            /*
                             

                            {{
                                        ""@type"": ""cargo:CodeListElement"",
                                        ""cargo:code"": ""{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}"",
                                        ""cargo:codeListName"": ""{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}""
                                    }}
                            */

                            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObjectEvent(input.waybill.shipmentID, createStr, acessToken);

                            if (response.IsSuccessStatusCode)
                            {
                                eventDetail.eventLink = retPath;
                                retVar = true;
                            }

                            eventNum++;
                        }
                    }
                }
            }
            return retVar;
        }
        [Route("getEventsCreationStr")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<string> GetEventsCreationStr(JMInsertObj input)
        {
            var retVal = "";
            int eventNum = 1;
            var insertStrList = new List<string>();
            if (input.waybill.shipment.stationEventList.Count > 0)
            {
                foreach (var station in input.waybill.shipment.stationEventList)
                {
                    foreach (var eventDetail in station.Value)
                    {
                        if (eventDetail != null)
                        {
                            var retPath = $"{logisticsObjectServerPath}{input.waybill.shipmentID}/logistics-events/{eventDetail.eventID}";
                            //""@id"": ""{retPath}"",
                            insertStrList.Add($@"
                                {{
                                    ""@type"": ""cargo:LogisticsEvent"",
                                    ""cargo:creationDate"": {{
                                        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                                        ""@value"": ""{DateTime.Now.JMDTToStr(2)}""
                                    }},
                                    ""cargo:eventDate"": {{
                                        ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                                        ""@value"": ""{eventDetail.eventDate.JMDTToStr(2)}""
                                    }},
                                    ""cargo:eventCode"": {{
                                        ""@type"": ""cargo:CodeListElement"",
                                        ""cargo:code"": ""{eventDetail.mileStoneCode}"",
                                        ""cargo:codeListName"": ""{eventDetail.mileStoneDesc}""
                                    }},
                                    ""cargo:eventName"": ""{eventDetail.eventName}"",
                                    ""eventTimeType"": ""{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}"",
                                    ""cargo:eventLocation"": {{""@id"":""{input.stationDict[station.Key]}""}}
                                }}
                            ");
                            /*
                             

                            {{
                                        ""@type"": ""cargo:CodeListElement"",
                                        ""cargo:code"": ""{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}"",
                                        ""cargo:codeListName"": ""{JMCommon.GetEventTimeTypeEnumName(eventDetail.eventTimeType)}""
                                    }}
                            */

                        }
                    }
                }
            }
            retVal = string.Join(',', insertStrList);
            return retVal;
        }

        [Route("appendEvent")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<Response> AppendEvent(JMInsertObj input)
        {
            string responseBody;
            var retPath = $"{logisticsObjectServerPath}{input.waybill.shipmentID}";
            var insertStrList = new List<string>();
            var eventLinkList = input.waybill.shipment.stationEventList.SelectMany(x => x.Value).Select(x => x.eventLink).ToList();

            foreach (var eventLink in eventLinkList)
            {
                insertStrList.Add($@"
                    {{
                        ""@type"": ""api:OperationObject"",
                        ""api:hasDatatype"": ""https://onerecord.iata.org/ns/cargo#LogisticsEvent"",
                        ""api:hasValue"": ""{eventLink}""
                    }}
                ");
            }

            string createStr = $@"{{
    ""@context"": {{
        ""cargo"": ""https://onerecord.iata.org/ns/cargo#"",
        ""api"": ""https://onerecord.iata.org/ns/api#""
    }},
    ""@type"": ""api:Change"",
    ""api:hasLogisticsObject"": {{
        ""@id"": ""{retPath}""
    }},
    ""api:hasDescription"": ""append event"",
    ""api:hasOperation"": [{{
            ""@type"": ""api:Operation"",
            ""api:op"": {{
                ""@id"": ""api:ADD""
            }},
            ""api:s"": ""{retPath}"",
            ""api:p"": ""https://onerecord.iata.org/ns/cargo#events"",
            ""api:o"": [
                {string.Join(',', insertStrList)}
            ]
        }}
    ],
    ""api:hasRevision"": {{
        ""@type"": ""http://www.w3.org/2001/XMLSchema#positiveInteger"",
        ""@value"": ""1""
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
            var retPath = $"{logisticsObjectServerPath}{input.waybill.shipmentID}";
            string createStr = $@"
{{
   ""@context"":{{
      ""cargo"":""https://onerecord.iata.org/ns/cargo#"",
      ""@vocab"": ""https://onerecord.iata.org/ns/cargo#""
   }},
    ""@id"": ""{retPath}"",
    ""cargo:goodsDescription"": ""{(string.IsNullOrEmpty(input.waybill.shipment.goodsDescription) ? "Goods Description" : $@"{input.waybill.shipment.goodsDescription}")}"",
    {(input.waybill.shipment.shcList != null && input.waybill.shipment.shcList.Count > 0 ? $@"""cargo:specialHandlingCodes"": [{string.Join(',', input.waybill.shipment.shcList.Select(x => $@"{{""@type"": ""CodeListElement"",""code"": ""{x}""}}"))}]," : "")}
    ""cargo:waybill"": {{
        ""@id"": ""{logisticsObjectServerPath}{input.waybill.waybillID}""
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

            if (eventIfAppendInObj && input.waybill.shipment.stationEventList.Count > 0)
            {
                createStr += $@"""cargo:events"":[";
                var insertStrList = new List<string>();
                foreach (var item in input.waybill.shipment.stationEventList)
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
					            ""eventTimeType"": ""{JMCommon.GetEventTimeTypeEnumName(eventObj.eventTimeType)}"",
					            ""cargo:eventLocation"": {{""@id"":""{input.stationDict[station]}""}}
				            }}
			            ");

                    }
                }
                createStr += string.Join(",", insertStrList);
                createStr += $@"],";
            }

            createStr += $@"
    ""@type"":[
        ""cargo:LogisticsObject"",
        ""cargo:Shipment""
    ]
}}
";
            //(eventIfAppendInObj ? ($@"""@cargo:events"": [{await GetEventsCreationStr(input)}],") : "")
            //""cargo: events"":[{ { { string.Join("},{", input.shipment.stationEventList.SelectMany(x => x.Value).Select(x => x.eventLink))} } }],

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createStr, acessToken);

            if (response.IsSuccessStatusCode)
            {

                var retObj = new Response() { status = 1, message = retPath };
                if (!eventIfAppendInObj)
                {
                    var ifEventCreated = await CreateEvents(input);
                    await AppendEvent(input);
                }
                return retObj;
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

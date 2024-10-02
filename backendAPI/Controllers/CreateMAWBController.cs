using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using WebAPITemplate.Attributes;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;
using WebAPITemplate.Swagger;

namespace WebAPITemplate.Controllers
{

    [AllowAnonymous]
    [Route("api/")]
    [ApiController]
    public class CreateMAWBController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INEOneAPIService _NEOneAPIService;
        private string acessToken = "";
        private string logisticsObjectServerPath = "";
        private bool createDEP = false;
        private bool createMAN = false;

        public CreateMAWBController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            createDEP = _config.GetSection("AppSettings:CreateDEP").Get<bool>();
            createMAN = _config.GetSection("AppSettings:CreateMAN").Get<bool>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;
        }

        [Route("createstatus/{awbprefix}/{awbsuffix}/milestone/timestamp")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<IActionResult> CreateStatus(string awbprefix, string awbsuffix, string milestone, string timestamp)
        {
            Response createStatusResult = await CreateStatusAsync("mawb" + awbprefix + "-" + awbsuffix, milestone, timestamp);
            return base.Ok();
        }

        [Route("createdep/{awbprefix}/{awbsuffix}")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<IActionResult> CreateDEP(string awbprefix, string awbsuffix)
        {
            Response createDEPResult = await CreateDEPAsync("mawb" + awbprefix + "-" + awbsuffix);
            return base.Ok();
        }

        [Route("createman/{awbprefix}/{awbsuffix}")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<IActionResult> CreateMAN(string awbprefix, string awbsuffix)
        {
            Response createDEPResult = await CreateMANAsync("mawb" + awbprefix + "-" + awbsuffix);
            return base.Ok();
        }

        [Route("createmawb")]
        [SwaggerExampleJsonRequest("createmawb")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<IActionResult> Post(CreateMAWBRequest data)
        {
            //HttpResponseMessage response;

            List<string> houseLOIds = new List<string>();

            foreach (var item in data.houses)
            {
                Response createShipmentResult = await CreateShipmentAsync(item.pieces, item.waybillNumber, item.goodsDesc);

                //return base.Ok(new CreateMAWBResponse() { result = createShipmentResult.message });
                if (createShipmentResult.status > 0)
                {

                    Response createHouseResult = await CreateHouseAsync(item.waybillNumber, "mawb"+ data.awbPrefix+"-" +data.awbSuffix);
                    if (createHouseResult.status > 0)
                    {
                        houseLOIds.Add("hawb"+ item.waybillNumber);
                    }
                    else
                    {
                        return base.BadRequest(new ErrorResponse() { errorCode = 10001, errorMessage = createHouseResult.message });

                    }

                }
                else
                {
                    return base.BadRequest(new ErrorResponse() { errorCode = 10002, errorMessage = createShipmentResult.message });
                }

            }

            //Create MAWB LO
            Response createMAWBResult = await CreateMAWBAsync("mawb"+ data.awbPrefix+"-"+ data.awbSuffix, data.awbPrefix, data.awbSuffix, houseLOIds);
            if (createMAWBResult.status > 0)
            {
                if (createDEP)
                {
                    Response createDEPResult = await CreateDEPAsync("mawb" + data.awbPrefix + "-" + data.awbSuffix);

                }

                if (createMAN)
                {
                    Response createMANResult = await CreateMANAsync("mawb" + data.awbPrefix + "-" + data.awbSuffix);

                }

                return base.Ok(new CreateMAWBResponse() { result = createMAWBResult.message });
            }
            else
            {
                return base.BadRequest(new ErrorResponse() { errorCode = 10003, errorMessage = createMAWBResult.message });

            }

                
        }


        protected async Task<Response> CreateShipmentAsync(List<string> pieces, string houseWaybillNumber, string goodsDesc)
        {

            string responseBody;
            string createShipmentString = @"{
                ""@id"": ""http://localhost:8080/logistics-objects/shipmentSZX20000267"",
                ""@type"": [
                    ""LogisticsObject"", ""Shipment""
                ],
                ""pieces"": [
                    {
                        ""@id"": ""http://localhost:8080/logistics-objects/Piece111111""
                    }
                ],
                ""goodsDescription"": ""TOOTHFISH MEAT RESTRICTED"",
                ""waybill"": {
                        ""@id"": ""http://localhost:8080/logistics-objects/hawbSZX20000267""
                    },
                ""@context"": {
                    ""@vocab"": ""https://onerecord.iata.org/ns/cargo#""
                }
            

             }";

            string toBeReplace = "{\r\n                        \"@id\": \"http://localhost:8080/logistics-objects/Piece111111\"\r\n                    }";
            string toReplace = "{\r\n                        \"@id\": \"http://localhost:8080/logistics-objects/Piece111111\"\r\n                    }";
            if (pieces.Count > 0)
            {
                for (int i = 0; i<pieces.Count;i++)
                {
                    if (i < 1)
                    {
                        toReplace = toReplace.Replace("http://localhost:8080/logistics-objects/Piece111111", "http://localhost:8080/logistics-objects/" + pieces[i].ToString());

                    }
                    else
                    {
                        toReplace = toReplace + ",\r\n                " + toBeReplace.Replace("http://localhost:8080/logistics-objects/Piece111111", "http://localhost:8080/logistics-objects/" + pieces[i].ToString());

                    }
                }
            }
            createShipmentString = createShipmentString.Replace(toBeReplace, toReplace);
            createShipmentString = createShipmentString.Replace("TOOTHFISH MEAT RESTRICTED", goodsDesc);
            createShipmentString = createShipmentString.Replace("SZX20000267", houseWaybillNumber);
            createShipmentString = createShipmentString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createShipmentString, acessToken);

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



        protected async Task<Response> CreateHouseAsync(string houseWaybillNumber, string mawb)
        {

            string responseBody;
            string createHouseString = @"{
                ""@id"": ""http://localhost:8080/logistics-objects/hawbSZX20000267"",
                ""@type"": [
                    ""LogisticsObject"", ""Waybill""
                ],
                ""masterWaybill"": {
                        ""@id"": ""http://localhost:8080/logistics-objects/mawb160-12002676""
                    },
                ""shipment"": {
                    ""@id"": ""http://localhost:8080/logistics-objects/shipmentSZX20000267""
                },
                ""waybillNumber"": ""SZX20000267"",
                ""waybillType"": {
                    ""@id"": ""https://onerecord.iata.org/ns/cargo#HOUSE""
                },
                ""@context"": {
                    ""@vocab"": ""https://onerecord.iata.org/ns/cargo#""
                }
             }";

            createHouseString = createHouseString.Replace("SZX20000267", houseWaybillNumber);
            createHouseString = createHouseString.Replace("mawb160-12002676", mawb);
            createHouseString = createHouseString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createHouseString, acessToken);

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


        protected async Task<Response> CreateMAWBAsync(string MAWBLOID, string AWBPrefix, string AWBSuffix, List<string> HouseLOIDs)
        {

            string responseBody;
            string createMAWBString = @"{
                ""@graph"": [
                        {
                            ""@id"": ""http://localhost:8080/logistics-objects/mawb1234"",
                            ""@type"": [
                                ""LogisticsObject"",
                                ""Waybill""
                            ],
                            ""arrivalLocation"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/airportGVA""
                            },
                            ""departureLocation"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/airportSZX""
                            },
                            ""houseWaybills"": [
                            {
                                ""@id"": ""http://localhost:8080/logistics-objects/hawbSZXAAAAAA""
                            }
                            ],
                            ""involvedParties"": [
                                {
                                    ""@id"": ""neone:8e6b362c-dabc-451e-8d61-10e7a76531c2""
                                },
                                {
                                    ""@id"": ""neone:ab557f31-886a-4149-9def-56543917eeaf""
                                },
                                {
                                    ""@id"": ""neone:0ca2276f-bab9-4a51-bf5e-4e5b680def66""
                                },
                                {
                                    ""@id"": ""neone:8be12f7e-8b15-4c22-b243-1bca9f6e3bf9""
                                }
                            ],
                            ""waybillNumber"": ""11111122"",
                            ""waybillPrefix"": ""160"",
                            ""waybillType"": {
                                ""@id"": ""https://onerecord.iata.org/ns/cargo#MASTER""
                            }
                        },
                        {
                            ""@id"": ""neone:0ca2276f-bab9-4a51-bf5e-4e5b680def66"",
                            ""@type"": ""Party"",
                            ""partyDetails"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/ICCS""
                            },
                            ""partyRole"": {
                                ""@id"": ""https://onerecord.iata.org/ns/coreCodeLists#ParticipantIdentifier_GHA""
                            }
                        },
                        {
                            ""@id"": ""neone:8be12f7e-8b15-4c22-b243-1bca9f6e3bf9"",
                            ""@type"": ""Party"",
                            ""partyDetails"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/GVA_Handling_Services""
                            },
                            ""partyRole"": {
                                ""@id"": ""https://onerecord.iata.org/ns/coreCodeLists#ParticipantIdentifier_GHA""
                            }
                        },
                        {
                            ""@id"": ""neone:8e6b362c-dabc-451e-8d61-10e7a76531c2"",
                            ""@type"": ""Party"",
                            ""partyDetails"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/CATHAY_CARGO""
                            },
                            ""partyRole"": {
                                ""@id"": ""https://onerecord.iata.org/ns/coreCodeLists#ParticipantIdentifier_AIR""
                            }
                        },
                        {
                            ""@id"": ""neone:ab557f31-886a-4149-9def-56543917eeaf"",
                            ""@type"": ""Party"",
                            ""partyDetails"": {
                                ""@id"": ""http://localhost:8080/logistics-objects/1561304566""
                            },
                            ""partyRole"": {
                                ""@id"": ""https://onerecord.iata.org/ns/coreCodeLists#ParticipantIdentifier_AGT""
                            }
                        }
                    ],
                    ""@context"": {
                        ""@vocab"": ""https://onerecord.iata.org/ns/cargo#""
                    }

             }";
            createMAWBString = createMAWBString.Replace("\"waybillNumber\": \"11111122\"", "\"waybillNumber\": \""+ AWBSuffix + "\"");
            createMAWBString = createMAWBString.Replace("\"waybillPrefix\": \"160\"", "\"waybillPrefix\": \"" + AWBPrefix + "\"");
            createMAWBString = createMAWBString.Replace("mawb1234", MAWBLOID);

            string toBeReplace = "{\r\n                                \"@id\": \"http://localhost:8080/logistics-objects/hawbSZXAAAAAA\"\r\n                            }";
            string toReplace = "{\r\n                                \"@id\": \"http://localhost:8080/logistics-objects/hawbSZXAAAAAA\"\r\n                            }";
            if (HouseLOIDs.Count > 0)
            {
                for (int i = 0; i < HouseLOIDs.Count; i++)
                {
                    if (i < 1)
                    {
                        toReplace = toReplace.Replace("http://localhost:8080/logistics-objects/hawbSZXAAAAAA", "http://localhost:8080/logistics-objects/" + HouseLOIDs[i].ToString());

                    }
                    else
                    {
                        toReplace = toReplace + ",\r\n                " + toBeReplace.Replace("http://localhost:8080/logistics-objects/hawbSZXAAAAAA", "http://localhost:8080/logistics-objects/" + HouseLOIDs[i].ToString());

                    }
                }
            }
            createMAWBString = createMAWBString.Replace(toBeReplace, toReplace);
            createMAWBString = createMAWBString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/


            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObject(createMAWBString, acessToken);

            if (response.IsSuccessStatusCode)
            {
                return new Response() { status = 1, message = MAWBLOID };
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }

        }


        protected async Task<Response> CreateDEPAsync(string mawb)
        {

            string responseBody;
            string createDEPString = @"{
                ""@context"": {
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                },
                ""@type"": ""cargo:LogisticsEvent"",
                ""cargo:creationDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T10:38:01.000Z""
                },
                ""cargo:eventDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T10:38:01.000Z""
                },
                ""cargo:eventCode"": {
                    ""@type"": ""cargo:CodeListElement"",
                    ""cargo:code"": ""DEP"",
                    ""cargo:codeListName"": ""Departure""
                },
                ""cargo:eventName"": ""Consignment departed on a specific flight"",
                ""cargo:eventTimeType"": {
                    ""@id"": ""cargo:ACTUAL"",
                    ""@type"": ""cargo:EventTimeType""
                },
                ""cargo:partialEventIndicator"": false,
                ""cargo:eventFor"": {
                    ""@type"": ""cargo:Shipment"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                },
                ""cargo:recordingOrganization"": {
                    ""@type"": ""cargo:Company"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                }
            }";

            createDEPString = createDEPString.Replace("mawb160-22223333", mawb);
            createDEPString = createDEPString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObjectEvent(mawb, createDEPString, acessToken);

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


        protected async Task<Response> CreateMANAsync(string mawb)
        {

            string responseBody;
            string createDEPString = @"{
                ""@context"": {
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                },
                ""@type"": ""cargo:LogisticsEvent"",
                ""cargo:creationDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T11:45:01.000Z""
                },
                ""cargo:eventDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T11:45:01.000Z""
                },
                ""cargo:eventCode"": {
                    ""@type"": ""cargo:CodeListElement"",
                    ""cargo:code"": ""MAN"",
                    ""cargo:codeListName"": ""Manifested""
                },
                ""cargo:eventName"": ""Consignment manifested on a specific flight"",
                ""cargo:eventTimeType"": {
                    ""@id"": ""cargo:ACTUAL"",
                    ""@type"": ""cargo:EventTimeType""
                },
                ""cargo:partialEventIndicator"": false,
                ""cargo:eventFor"": {
                    ""@type"": ""cargo:Shipment"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                },
                ""cargo:recordingOrganization"": {
                    ""@type"": ""cargo:Company"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                }
            }";

            createDEPString = createDEPString.Replace("mawb160-22223333", mawb);
            createDEPString = createDEPString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObjectEvent(mawb, createDEPString, acessToken);

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

        protected async Task<Response> CreateStatusAsync(string mawb, string milestone, string timestamp)
        {

            string responseBody;
            string createStatusString = @"{
                ""@context"": {
                    ""cargo"": ""https://onerecord.iata.org/ns/cargo#""
                },
                ""@type"": ""cargo:LogisticsEvent"",
                ""cargo:creationDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T11:45:00.000Z""
                },
                ""cargo:eventDate"": {
                    ""@type"": ""http://www.w3.org/2001/XMLSchema#dateTime"",
                    ""@value"": ""2024-03-16T11:45:00.000Z""
                },
                ""cargo:eventCode"": {
                    ""@type"": ""cargo:CodeListElement"",
                    ""cargo:code"": ""XXXX"",
                    ""cargo:codeListName"": ""Manifested""
                },
                ""cargo:eventName"": ""Consignment manifested on a specific flight"",
                ""cargo:eventTimeType"": {
                    ""@id"": ""cargo:ACTUAL"",
                    ""@type"": ""cargo:EventTimeType""
                },
                ""cargo:partialEventIndicator"": false,
                ""cargo:eventFor"": {
                    ""@type"": ""cargo:Shipment"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                },
                ""cargo:recordingOrganization"": {
                    ""@type"": ""cargo:Company"",
                    ""@id"": ""http://localhost:8080/logistics-objects/mawb160-22223333""
                }
            }";

            string resultCodeListName = "";
            string resultEventName = "";

            switch (milestone)
            {
                case "FOH":
                    resultCodeListName = "Freight On Hand";
                    resultEventName = "Freight On Hand";
                    break;
                case "RCS":
                    resultCodeListName = "Airline Received";
                    resultEventName = "Airline Received";
                    break;
                case "MAN":
                    resultCodeListName = "Manifested";
                    resultEventName = "Consignment manifested on a specific flight";
                    break;
                case "DEP":
                    resultCodeListName = "Departed";
                    resultEventName = "Departed";
                    break;
                case "ARR":
                    resultCodeListName = "Arrived";
                    resultEventName = "Arrived";
                    break;
                case "RCF":
                    resultCodeListName = "Freight Accepted at Airport";
                    resultEventName = "Freight Accepted at Airport";
                    break;
                default:
                    // code block
                    break;
            }

            createStatusString = createStatusString.Replace("Manifested", resultCodeListName);
            createStatusString = createStatusString.Replace("Consignment manifested on a specific flight", resultEventName);

            createStatusString = createStatusString.Replace("2024-03-16T11:45", timestamp);
            createStatusString = createStatusString.Replace("XXXX", milestone);
            createStatusString = createStatusString.Replace("mawb160-22223333", mawb);
            createStatusString = createStatusString.Replace("http://localhost:8080/logistics-objects/", logisticsObjectServerPath);

            /*
            try
            {
                var fullPathFileName = Path.GetFullPath("Sample/CreateShipment.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    string json = r.ReadToEnd();

                    object[] args = new object[] { "1" };
                    requestBody = string.Format(json, args);
                    requestBody = requestBody;
                }
            }
            catch (Exception e)
            {
                throw e;
            }*/

            HttpResponseMessage response = await _NEOneAPIService.PostLogisticsObjectEvent(mawb, createStatusString, acessToken);

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

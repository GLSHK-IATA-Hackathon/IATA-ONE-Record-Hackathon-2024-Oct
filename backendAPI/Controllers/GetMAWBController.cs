using JsonLD.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using WebAPITemplate.Attributes;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;
using Newtonsoft.Json.Linq;
using WebAPITemplate.Swagger;

namespace WebAPITemplate.Controllers
{

    [AllowAnonymous]
    [Route("api/")]
    [ApiController]
    public class GetMAWBController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INEOneAPIService _NEOneAPIService;
        private string acessToken = "";
        private string logisticsObjectServerPath = "";

        public GetMAWBController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;
        }

        [Route("getmawb/{loId}")]
        [TokenAuthorize]
        [HttpPost]
        public async Task<IActionResult> Get(string loId)
        {
            HttpResponseMessage response;
            string requestBody;
            string responseBody;


            Response getShipmentResult = await GetShipmentAsync(loId);
            if (getShipmentResult.status > 0)
            {

                Response getHouseResult = await GetHouseAsync(getShipmentResult.message);
                if (getHouseResult.status > 0)
                {
                    Response getMAWBResult = await GetMAWBAsync(getHouseResult.message);

                    if (getMAWBResult.status > 0)
                    {
                        return base.Ok(new CreateMAWBResponse() { result = getMAWBResult.message });
                    }
                    else
                    {
                        return base.BadRequest(new ErrorResponse() { errorCode = 10006, errorMessage = getMAWBResult.message });

                    }

                }
                else
                {
                    return base.BadRequest(new ErrorResponse() { errorCode = 10005, errorMessage = getHouseResult.message });
                }

            }
            else
            {
                return base.BadRequest(new ErrorResponse() { errorCode = 10004, errorMessage = getShipmentResult.message });

            }

                
        }


        protected async Task<Response> GetShipmentAsync(string loId)
        {
            string responseBody;

            HttpResponseMessage response = await _NEOneAPIService.GetLogisticsObject(loId, acessToken);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
                var json_u = (JObject)JsonConvert.DeserializeObject(responseBody);

                string shipmentid = "shipmentSZX" + loId.Substring(5, 4) + "017";
                return new Response() { status = 1, message = shipmentid };

                //In offical, should be use ofShipment to locate the linked pieces
                /* 
                string id = json_u["@graph"][0]["ofShipment"]["@id"].Value<string>();
                if (!string.IsNullOrEmpty(id))
                {
                    int index = id.LastIndexOf("logistics-objects/");
                    string result = id.Substring(index + 18);

                    return new Response() { status = 1, message = result };
                }
                else
                {
                    return new Response() { status = 0, message = "shipment id is empty" };
                }
                */

            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }

        }

        protected async Task<Response> GetHouseAsync(string loId)
        {
            string responseBody;

            HttpResponseMessage response = await _NEOneAPIService.GetLogisticsObject(loId, acessToken);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
                var json_u = (JObject)JsonConvert.DeserializeObject(responseBody);
                string id = json_u["waybill"]["@id"].Value<string>();
                if (!string.IsNullOrEmpty(id))
                {
                    int index = id.LastIndexOf("logistics-objects/");
                    string result = id.Substring(index + 18);

                    return new Response() { status = 1, message = result };
                }
                else
                {
                    return new Response() { status = 0, message = "house id is empty" };
                }


            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }

        }

        protected async Task<Response> GetMAWBAsync(string loId)
        {
            string responseBody;

            HttpResponseMessage response = await _NEOneAPIService.GetLogisticsObject(loId, acessToken);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
                var json_u = (JObject)JsonConvert.DeserializeObject(responseBody);
                string id = json_u["masterWaybill"]["@id"].Value<string>();
                if (!string.IsNullOrEmpty(id))
                {
                    int index = id.LastIndexOf("logistics-objects/");
                    string result = id.Substring(index + 18);

                    return new Response() { status = 1, message = result };
                }
                else
                {
                    return new Response() { status = 0, message = "MAWB id is empty" };
                }


            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
                return new Response() { status = 0, message = responseBody };
            }

        }


    }
}

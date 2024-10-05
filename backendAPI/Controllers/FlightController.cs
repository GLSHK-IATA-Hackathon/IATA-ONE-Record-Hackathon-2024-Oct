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
using log4net;

namespace WebAPITemplate.Controllers
{

    [AllowAnonymous]
    [Route("api/")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INEOneAPIService _NEOneAPIService;
        private string acessToken = "";
        private string logisticsObjectServerPath = "";

        public FlightController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;
        }

        [Route("flight/{flightNumber}")]
        [TokenAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get(string flightNumber)
        {
            HttpResponseMessage response;

            if (true)
            {
                string result = "";
                var fullPathFileName = Path.GetFullPath("Swagger/SwaggerExample/flightresponse.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    result = r.ReadToEnd();
                }

                return Ok(result);
            }
            else
            {
                Response getLogisticsEventsResult = await GetLogisticsEventsAsync(flightNumber);
                if (getLogisticsEventsResult.status > 0)
                {

                    return Ok(getLogisticsEventsResult);
                }
                else
                {
                    return base.BadRequest(new ErrorResponse() { errorCode = 10004, errorMessage = getLogisticsEventsResult.message });

                }
            }
            
        }


        protected async Task<Response> GetLogisticsEventsAsync(string loId)
        {
            string responseBody;

            HttpResponseMessage response = await _NEOneAPIService.GetLogisticsEvents(loId, acessToken);

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

    }


    public class FlightResposne
    {
        
    }

    
    

}

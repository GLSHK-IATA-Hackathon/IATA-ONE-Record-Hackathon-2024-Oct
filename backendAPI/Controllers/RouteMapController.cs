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
    public class RouteMapController : ControllerBase
    {
        private readonly IConfiguration _config;
        private INEOneAPIService _NEOneAPIService;
        private string acessToken = "";
        private string logisticsObjectServerPath = "";

        public RouteMapController(IConfiguration config, INEOneAPIService __NEOneAPIService)
        {
            _config = config;
            acessToken = _config.GetSection("APIConfig:NEOneAPI:TestingAccessToken").Get<string>();

            logisticsObjectServerPath = _config.GetSection("AppSettings:LogisticsObjectServerPath").Get<string>();
            _NEOneAPIService = __NEOneAPIService;
        }

        [Route("routemap/{loId}")]
        [TokenAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get(string loId)
        {
            HttpResponseMessage response;

            if (true)
            {
                string result = "";
                var fullPathFileName = Path.GetFullPath("Swagger/SwaggerExample/routemapresponse.json");
                using (StreamReader r = new StreamReader(fullPathFileName))
                {
                    result = r.ReadToEnd();
                }

                return Ok(result);
            }
            else
            {
                Response getLogisticsEventsResult = await GetLogisticsEventsAsync(loId);
                if (getLogisticsEventsResult.status > 0)
                {

                    var json_u = (JObject)JsonConvert.DeserializeObject(getLogisticsEventsResult.message);


                    //var embeddedLO = JsonLdProcessor.Frame(json_u, json_u["@context"], opts);

                    //JObject compactedDocument = JsonLdProcessor.Compact(embeddedLO, json_u["@context"], opts);


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
                return new Response() { status = 1, message = responseBody };

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


    public class RouteMapResposne
    {
        
    }

    
    

}

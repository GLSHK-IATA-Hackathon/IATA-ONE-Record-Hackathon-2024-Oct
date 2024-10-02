using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;
using WebAPITemplate.Attributes;
using WebAPITemplate.Models;
using WebAPITemplate.Models.RestClient;

namespace WebAPITemplate.Controllers
{

    [AllowAnonymous]
    [Route("api/")]
    [ApiController]
    public class DGCheckingController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ApiRestClient _gLSRestClient;

        protected bool isLoadCSV = false;
        protected string CSVName = "";
        protected List<DGItem> dgCSV = new List<DGItem>();

        public DGCheckingController(IConfiguration config, IWebHostEnvironment env, ApiRestClient gLSRestClient)
        {
            _config = config;
            _env = env;
            _gLSRestClient = gLSRestClient;

            isLoadCSV = _config.GetSection("AppSettings:IsLoadCSV").Get<bool>();            
            CSVName = _config.GetSection("AppSettings:CSVPath").Get<string>() ?? "";
            dgCSV = loadDBCSV();            
        }

        [Route("dgfile")]
        [TokenAuthorize]
        [HttpGet]
        public IActionResult Get()
        {
            List<DGItem> values = new List<DGItem>();
            string filename = Path.Combine(Directory.GetCurrentDirectory() + CSVName);
            var result = new
            {
                filename = filename,
                isExist = System.IO.File.Exists(filename)
            };
            return base.Ok(result);
        }

        [Route("dgchecking")]
        [TokenAuthorize]
        [HttpPost]
        public IActionResult Post(List<DGCheckingRequest> data)
        {
            List<DGPrediction> result = new List<DGPrediction>();
            if (isLoadCSV)
            {
                foreach (var d in data)
                {
                    result.Add(new DGPrediction()
                    {
                        id = d.id,
                        instance = getDGInstanceString(d),
                        prediction = checkIsDG_Testing(d.description)
                    });
                }
            }
            else
            {
                result = checkIsDG_AWS(data);
            }
            return base.Ok(new DGCheckingResponse() { predictions = result });
        }

        [Route("msiffsets")]
        [TokenAuthorize]
        [HttpPost]
        public IActionResult msiffsets(DGAIRequest data)
        {
            DGCheckingResponse response = new DGCheckingResponse();
            List<DGPrediction> list = new List<DGPrediction>();
            if (data != null && data.instances != null && data.instances.Count > 0)
            {
                foreach (var d in data.instances)
                {
                    list.Add(new DGPrediction() { id = d.id, instance = d.instance, prediction = checkIsDG_Testing(d.instance) });
                }
            }
            response.predictions = list;
            return base.Ok(response);
        }




        protected List<DGPrediction> checkIsDG_AWS (List<DGCheckingRequest> data)
        {
            List<DGPrediction> result = new List<DGPrediction>();
            if (data != null && data.Count > 0)
            {
                List<DGAIInstance> instances = data.Select(a => new DGAIInstance() { id = a.id, instance = getDGInstanceString(a) }).ToList();
                string apiResult = checkDGAIAPI(JsonSerializer.Serialize(new DGAIRequest() { instances = instances}));
                if (!string.IsNullOrWhiteSpace(apiResult))
                {
                    var apiObj = Newtonsoft.Json.JsonConvert.DeserializeObject<DGCheckingResponse>(apiResult);
                    if (apiObj != null && apiObj.predictions != null)
                    {
                        result = apiObj.predictions;
                    }
                }
            }
            return result;
        }

        protected string checkDGAIAPI(object postData)
        {
            var restRequest = new RestRequest($"/api/msiffsets", Method.Post)
              .AddHeader("Content-Type", "application/json")
              .AddJsonBody(postData)
            ;

            //code for execution
            var restResponse = _gLSRestClient.Execute(restRequest);

            return restResponse.Content;
        }

        protected string checkIsDG_Testing (string desc)
        {
            string predict = "";
            var result = dgCSV.Where(a => desc.ToLower().Contains(a.Name.ToLower())).FirstOrDefault();
            if (result != null)
            {
                predict = result.Remark;
            }
            return predict;
        }

        protected List<DGItem> loadDBCSV()
        {
            List<DGItem> values = new List<DGItem> ();
            string filename = Path.Combine(Directory.GetCurrentDirectory() + CSVName);
            if (System.IO.File.Exists(filename))
            {
                values = System.IO.File.ReadAllLines(filename)
                                               .Skip(1)
                                               .Select(v => DGItem.FromCsv(v))
                                               .ToList();
            }
            return values;
        }

        protected string getDGInstanceString (DGCheckingRequest i)
        {
            return (i.origin ?? "")
                + "," + (i.destination ?? "")
                + "," + (i.shipperName ?? "")
                + "," + (i.consigneeName ?? "")
                + "," + (i.SPH ?? "")
                + "," + (i.description ?? "");
        }
    }
}

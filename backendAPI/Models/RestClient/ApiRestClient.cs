using Newtonsoft.Json;
using System.Xml.Serialization;

namespace WebAPITemplate.Models.RestClient
{
    public partial class ApiRestClient : GLSHK.APIClient.GLSRestClient
    {
        private string _ApiName = "AIAPI";

        public ApiRestClient(IConfiguration config, string APIName = "")
        {
            if (!string.IsNullOrWhiteSpace(APIName))
            {
                _ApiName = APIName;
            }
            Init(config, _ApiName);
        }
    }

    public class RequestBodyContent
    {
        [JsonProperty("content")]
        [XmlElement("content")]
        public string content { get; set; }
    }
}

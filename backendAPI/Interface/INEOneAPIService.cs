using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPITemplate.Interface
{

    public interface INEOneAPIService
    {

        //Task<HttpResponseMessage> DebugModeGetToken(string debugGetTokenBaseUrl, string debugGetTokenUsername, string debugGetTokenPassword);
        Task<HttpResponseMessage> PostLogisticsObject(string body = "", string bearToken = "");
        Task<HttpResponseMessage> PostLogisticsObjectEvent(string mawb, string body = "", string bearToken = "");
        Task<HttpResponseMessage> GetLogisticsObject(string loId, string bearToken = "");
    }
}

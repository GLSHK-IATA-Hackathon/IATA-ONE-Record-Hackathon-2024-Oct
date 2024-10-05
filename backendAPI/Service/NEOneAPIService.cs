using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebAPITemplate.Domain.Configs;
using WebAPITemplate.Interface;

namespace WebAPITemplate.Service
{

    public class NEOneAPIService : INEOneAPIService
    {
        private readonly NEOneAPIConfig _apiConfig;
        private readonly HttpClient _httpClient;
        private readonly string logisticsObjectPath;
        private readonly string logisticsObjectEventPath;

        public NEOneAPIService(IOptions<NEOneAPIConfig> config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiConfig = config.Value;
            logisticsObjectPath = config.Value.LogisticsObjectPath;
            logisticsObjectEventPath = config.Value.LogisticsObjectPath;
            logisticsObjectPath = "logistics-objects";
            logisticsObjectEventPath = "logistics-objects/{0}/logistics-events";
        }


        //public async Task<HttpResponseMessage> DebugModeGetToken(string debugGetTokenBaseUrl, string debugGetTokenUsername, string debugGetTokenPassword)
        //{
        //    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        //    }

        //    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {debugGetTokenPassword}");

        //    var collection = new List<KeyValuePair<string, string>>();
        //    collection.Add(new("grant_type", "client_credentials"));
        //    collection.Add(new("client_id", "neone-client"));
        //    var content = new FormUrlEncodedContent(collection);

        //    return await _httpClient.PostAsync(debugGetTokenBaseUrl, content);

        //}


        public async Task<HttpResponseMessage> PatchLogisticsObject(string body = "", string bearToken = "")
        {

            HttpContent contentPost = new StringContent(body, Encoding.UTF8, "application/ld+json");

            if (!string.IsNullOrEmpty(bearToken))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + bearToken);
            }


            return await _httpClient.PatchAsync(logisticsObjectPath, contentPost);

        }
        public async Task<HttpResponseMessage> PostLogisticsObject(string body = "", string bearToken = "")
        {

            HttpContent contentPost = new StringContent(body, Encoding.UTF8, "application/ld+json");

            if (!string.IsNullOrEmpty(bearToken))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + bearToken);
            }


            return await _httpClient.PostAsync(logisticsObjectPath, contentPost);

        }

        public async Task<HttpResponseMessage> PostLogisticsObjectEvent(string mawb, string body = "", string bearToken = "")
        {

            HttpContent contentPost = new StringContent(body, Encoding.UTF8, "application/ld+json");

            if (!string.IsNullOrEmpty(bearToken))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + bearToken);
            }

            string finalPath = string.Format(logisticsObjectEventPath, mawb);
            return await _httpClient.PostAsync(finalPath, contentPost);

        }

        public async Task<HttpResponseMessage> GetLogisticsObject(string loId, string bearToken = "")
        {

            if (!string.IsNullOrEmpty(bearToken))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + bearToken);
            }


            return await _httpClient.GetAsync(logisticsObjectPath + "/" + loId);

        }


    }

}

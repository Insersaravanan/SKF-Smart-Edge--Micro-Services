using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EMaintanenceMicroService.Utils
{
    public class HttpHelper
    {
        public HttpHelper(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public static async Task<dynamic> GetReqstAsync(string Url)
        {
            try
            {
                using (var httpClientHandler = new HttpClientHandler())
                {
                    // NB: You should make this more robust by actually checking the certificate:
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    //######## The below code is used to send request with out Authentication ##########
                    //using (var httpClient = new HttpClient(httpClientHandler))
                    //{
                    //    httpClient.BaseAddress = new Uri("https://18.138.91.203:7777");
                    //    //httpClient.BaseAddress = new Uri("https://my-json-server.typicode.com");
                    //    httpClient.DefaultRequestHeaders.Accept.Clear();

                    //    HttpResponseMessage response = await httpClient.GetAsync(Url);
                    //    response.EnsureSuccessStatusCode();
                    //    string resultContentString = await response.Content.ReadAsStringAsync();
                    //    return resultContentString;
                    //}

                    //######## The below code is used to send request with Credentials ###########
                    using (HttpClient httpClient = new HttpClient(httpClientHandler))
                    {
                        Dictionary<string, string> tokenDetails = null;
                        //var messageDetails = new Message { Id = 4, Message1 = des };
                        httpClient.BaseAddress = new Uri("https://18.138.91.203:7777");
                        var login = new Dictionary<string, string>
                           {
                               {"grant_type", "password"},
                               {"username", "apiadmin"},
                               {"password", "@dminap!"},
                           };
                        var response = httpClient.PostAsync("/api/token", new FormUrlEncodedContent(login)).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
                            if (tokenDetails != null && tokenDetails.Any())
                            {
                                var tokenNo = tokenDetails.FirstOrDefault().Value;
                                using (HttpClient httpClient1 = new HttpClient(httpClientHandler))
                                {
                                    httpClient1.BaseAddress = new Uri("https://18.138.91.203:7777");
                                    httpClient1.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenNo);
                                    await httpClient.GetAsync(Url)
                                        .ContinueWith((getTask) => getTask.Result.EnsureSuccessStatusCode());

                                    HttpResponseMessage response1 = await httpClient1.GetAsync(Url);
                                    response1.EnsureSuccessStatusCode();
                                    return await response1.Content.ReadAsStringAsync();
                                }
                            }
                        }
                        return "Response not found...";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
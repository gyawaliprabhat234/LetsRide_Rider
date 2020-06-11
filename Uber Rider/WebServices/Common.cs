using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Uber_Rider.DataModels;
using Uber_Rider.Helpers;

namespace Uber_Rider.WebServices
{
    public static class Common
    {
        public static async Task<ResponseData> WebPostServiceAuthorize(string UrlPath, string parameter)
        {
            ResponseData response = new ResponseData();
            try
            {
                AppData userInfo = new AppData();
              
                var client = new HttpClient();
                HttpResponseMessage result;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userInfo.GetToken);

                if (string.IsNullOrEmpty(parameter))
                {
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, null).ConfigureAwait(false);

                }
                else
                {
                    var content = new StringContent(parameter);
                    HttpContent cont = content;
                    cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, cont).ConfigureAwait(false);
                }

                if (result.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<ResponseData>(await result.Content.ReadAsStringAsync());
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Problem to establish connection with server";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

                
            }
            return response;
        }
        public static async Task<ResponseData> WebPostService(string UrlPath, string parameter, bool IsAuthorize)
        {
            try
            {
                AppData userInfo = new AppData();
                ResponseData response = new ResponseData();
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                var client = new HttpClient();
                HttpResponseMessage result;
                if (IsAuthorize)
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userInfo.GetToken);

                if (string.IsNullOrEmpty(parameter))
                {
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, null).ConfigureAwait(false);

                }
                else
                {
                    var content = new StringContent(parameter);
                    HttpContent cont = content;
                    cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    result = await client.PostAsync(LetsRideCredentials.WebUrl + UrlPath, cont).ConfigureAwait(false);
                }

                if (result.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<ResponseData>(await result.Content.ReadAsStringAsync());
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Problem to establish connection with server";
                }
                return response;
            }
            catch (Exception ex)
            {
                return new ResponseData() { IsSuccess = false, Message = "Error! " + ex.Message };

            }
        }
    }
}
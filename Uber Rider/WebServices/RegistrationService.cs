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

namespace Uber_Rider.WebServices
{
   public class RegistrationService
   {

        public async Task<ResponseData> RegisterCustomer(CustomerInfo customerInfo)
        {
            ResponseData response = new ResponseData();
            var client = new HttpClient();
            var content = new StringContent(
            JsonConvert.SerializeObject(customerInfo));
            HttpContent cont = content;
            cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(LetsRideCredentials.WebUrl + "api/Login/CustomerRegistration", cont).ConfigureAwait(false);
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

        public async Task<ResponseData> CheckOTPCode(CustomerInfo customer)
        {
            ResponseData response = new ResponseData();
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(customer));
            HttpContent cont = content;
            cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync(LetsRideCredentials.WebUrl + "api/Login/CheckOtpCode", cont).ConfigureAwait(false);
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
   }
}
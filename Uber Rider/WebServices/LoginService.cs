using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
   public class LoginService
    {
        public async Task<bool> IsLogggedIn()
        {
            AppData userInfo = new AppData();
            CustomerInfo customerInfo = userInfo.GetCurrentUser;
            if(customerInfo.CustomerId == new Guid() || string.IsNullOrEmpty(userInfo.GetToken))
            {
                return false;
            }
            ResponseData response = new ResponseData();
            var client = new HttpClient();
            
            var content = new StringContent(JsonConvert.SerializeObject(customerInfo));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userInfo.GetToken);
            HttpContent cont = content;
            
            cont.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

              var result = await client.PostAsync(LetsRideCredentials.WebUrl + "api/Login/IsLoggedIn", cont).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                response = JsonConvert.DeserializeObject<ResponseData>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Problem to establish connection with server";
            }
            return response.IsSuccess;
        }

        public async Task<ResponseData> ResendVerificationCode(CustomerInfo customer)
        {
            string cust = JsonConvert.SerializeObject(customer);
            return await Common.WebPostService("api/Login/ResendCustomerVerificationCode", cust, false);
        }

    }
}
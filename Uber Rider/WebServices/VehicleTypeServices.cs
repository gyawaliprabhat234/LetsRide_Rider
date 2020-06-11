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
  public  class VehicleTypeServices
    {
        public async Task<ResponseData> GetActiveVehicleType()
        {
            ResponseData response = await   Common.WebPostServiceAuthorize("api/Customer/GetActiveVehicleType", null);
            return response;
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
using Uber_Rider.ReferenceModels;

namespace Uber_Rider.WebServices
{
    public class TripServices
    {

        public async Task<ResponseData> SaveRideRequest(Rides ride)
        {
            if (string.IsNullOrEmpty(ride.Action))
                ride.Action = "A";

            string rideString = JsonConvert.SerializeObject(ride);
            return await Common.WebPostServiceAuthorize("api/Trip/SaveRideRequest", rideString);

        }


    }
}
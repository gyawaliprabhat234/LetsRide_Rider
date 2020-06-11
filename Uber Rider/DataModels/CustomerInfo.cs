using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Uber_Rider.DataModels
{
    public class CustomerInfo
    {
        public string Action { get; set; }
      
        public string CustomerName { get; set; }
        public Nullable<decimal> CustomerMobileNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsVerified { get; set; }
        public string CustomerDeviceInfo { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
        public CustomerVerification Verification { get; set; }
        public string CustomerPassword { get; set; }
        public CustomerDeviceInfo DeviceInfo { get; set; }
        public string CustomerIdString { get; set; }

        public System.Guid CustomerId
        {
            get; set;
        }


    }
}
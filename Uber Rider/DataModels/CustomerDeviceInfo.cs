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
   public class CustomerDeviceInfo
    {
        public System.Guid CustomerId { get; set; }
        public string DeviceNumber { get; set; }
        public Nullable<decimal> DeviceSetPhoneNumber { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceVersion { get; set; }
        public Nullable<int> DeviceType { get; set; }
        public Nullable<System.DateTime> DateDeleted { get; set; }
    }
}
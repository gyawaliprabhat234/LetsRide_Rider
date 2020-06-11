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
using Xamarin.Essentials;

namespace Uber_Rider.DataModels
{
   public class DeviceInformation
    {
        public string DeviceModel { get; set; }
        public string DeviceMobileNumber { get; set; }
        public string DeviceId { get; set; }
        public string VersionNo { get; set; }
        public string  DeviceName { get; set; }
        public int DeviceType { get; set; }

    }
}
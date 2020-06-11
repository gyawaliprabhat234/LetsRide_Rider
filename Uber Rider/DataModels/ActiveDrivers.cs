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
   public class ActiveDrivers
    {
        public string Action { get; set; }
        public Guid DriverId { get; set; }
        public Guid RideId { get; set; }
        public bool IsOnline { get; set; }
        public bool IsBusy { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal VehicleTypeId { get; set; }
        public double DistanceFromPickup { get; set; }
    }
}
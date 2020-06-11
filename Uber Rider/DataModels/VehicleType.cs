using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Uber_Rider.DataModels
{
    public class VehicleType
    {
        public int TypeId { get; set; }

        public int CategoryId { get; set; }
        public string VehicleName { get; set; }
        public string VehicleDescription { get; set; }
        public string VehicleImage { get; set; }
        public double BaseFare { get; set; }
        public double KmFare { get; set; }
        public double BaseKm { get; set; }
        public string CategoryName { get; set; }
        public Bitmap Image { get; set; }
        public byte AvailableSeats { get; set; }
    }
}
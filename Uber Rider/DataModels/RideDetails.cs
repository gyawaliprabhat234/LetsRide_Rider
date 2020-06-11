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
    public class RideDetails
    {
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string CustomerId { get; set; }
        public string RiderName { get; set; }
        public string RiderPhone { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public double DestinationLat { get; set; }
        public double DestinationLng { get; set; }
        public string RideId { get; set; }
        public decimal Distance { get; set; }
        public decimal EstimatedArrivalTime { get; set; }
        public decimal TotalCost { get; set; }
    }
}
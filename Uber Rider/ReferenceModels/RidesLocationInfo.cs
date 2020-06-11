using System;
using System.Collections.Generic;
using System.Text;

namespace Uber_Rider.ReferenceModels
{
   public class RidesLocationInfo
   {
        public string Action { get; set; }
        public Guid LocationId { get; set; }
        public decimal PickupLongitude { get; set; }
        public decimal PickupLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }
        public decimal DestinationLatitude { get; set; }
        public string PickupLocationName { get; set; }
        public string PickupDestinationName { get; set; }
        public decimal TotalDistance { get; set; }
        public Nullable<decimal> DriverLatitude { get; set; }
        public Nullable<decimal> DriverLongitude { get; set; }
       

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Uber_Rider.ReferenceModels
{
    public class RidesInfo
    {
        public string Action { get; set; }
        public Guid RideId { get; set; }
        public System.DateTime RideBookingTime { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> EstimatedArrivalTime { get; set; }
        public Guid LocationId { get; set; }
        public string RideReview { get; set; }
        public Nullable<byte> RideRating { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public  RidesLocationInfo LocationInfo { get; set; }

    }
}

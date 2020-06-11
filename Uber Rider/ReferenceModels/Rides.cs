using System;
using System.Collections.Generic;
using System.Text;

namespace Uber_Rider.ReferenceModels
{
   public class Rides
    {
        public string  Action { get; set; }
        public Guid RideId { get; set; }
        public byte RideTypeId { get; set; }
        public Nullable<Guid> DriverId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public int VehicleTypeId { get; set; }
        public  RideType RideType { get; set; }
        public RidesInfo RidesInfo { get; set; }

       


    }
}

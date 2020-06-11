using System;

namespace Uber_Rider.DataModels
{
    public class CustomerVerification
    {
        public Nullable<System.Guid> CustomerId { get; set; }
        public decimal OTP { get; set; }
        public byte Attempt { get; set; }
    }
}
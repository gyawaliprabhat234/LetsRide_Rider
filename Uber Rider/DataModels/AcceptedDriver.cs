using System;
namespace Uber_Rider.DataModels
{
    public class AcceptedDriver
    {
       public string ID { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public double Latitude { get; set; }
        public double  Longitude { get; set; }

        public string Status { get; set; }
      //  public  MyProperty { get; set; }
    }
}

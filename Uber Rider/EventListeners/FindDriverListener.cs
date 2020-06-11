using System;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils;
using Firebase.Database;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Uber_Rider.DataModels;
using Uber_Rider.Helpers;
using Uber_Rider.ReferenceModels;

namespace Uber_Rider.EventListeners
{
    public class FindDriverListener 
    {

        //Events
        HubConnection hubConnection;

        public class DriverFoundEventArgs : EventArgs
        {
            public List<ActiveDrivers> Drivers { get; set; }
            public Rides Ride { get; set; }
        }

        public event EventHandler<DriverFoundEventArgs> DriversFound;
        public event EventHandler DriverNotFound;

        //Ride Details
        Rides rides;
        //Available Drivers
        List<ActiveDrivers> availableDrivers = new List<ActiveDrivers>();

        public FindDriverListener(Rides _rides, HubConnection connection)
        {
            this.rides = _rides;
            this.hubConnection = connection;
        }

        public void OnCancelled(DatabaseError error)
        {

        }
     
        public void OnDataChange(DataSnapshot snapshot)
        {
            //if(snapshot.Value != null)
            //{
            //    var child = snapshot.Children.ToEnumerable<DataSnapshot>();
            //    availableDrivers.Clear();

            //    foreach(DataSnapshot data in child)
            //    {
            //        if(data.Child("ride_id").Value != null && data.Child("type_id").Value != null)
            //        {
            //            if (data.Child("ride_id").Value.ToString() == "waiting" && data.Child("type_id").Value.ToString() == rides.VehicleTypeId.ToString())
            //            {
            //                //Get Driver Location;
            //                double latitude = double.Parse(data.Child("location").Child("latitude").Value.ToString());
            //                double longitude = double.Parse(data.Child("location").Child("longitude").Value.ToString());
            //                LatLng driverLocation = new LatLng(latitude, longitude);
            //                AvailableDriver driver = new AvailableDriver();

                            //Compute Distance Between Pickup Location and Driver Location
                         //   driver.DistanceFromPickup = SphericalUtil.ComputeDistanceBetween(mPickupLocation, driverLocation);
                         //   driver.ID = data.Key;
                       //     availableDrivers.Add(driver);

            //            }
            //        }
            //    }

            //    if(availableDrivers.Count > 0)
            //    {
                    
            //    }
            //    else
            //    {
                    
            //    }
            //}
            //else
            //{
            //    DriverNotFound.Invoke(this, new EventArgs());
            //}
        }

        public void Create(List<ActiveDrivers> activeDrivers)
        {
            availableDrivers = activeDrivers;
            if (availableDrivers.Count > 0) {

                availableDrivers.ForEach(x => {

                    LatLng driverLocation = new LatLng(Convert.ToDouble(x.Latitude), Convert.ToDouble(x.Longitude));
                    LatLng pickUpLocation = new LatLng(Convert.ToDouble(rides.RidesInfo.LocationInfo.PickupLatitude), Convert.ToDouble(rides.RidesInfo.LocationInfo.PickupLongitude));
                    x.DistanceFromPickup = SphericalUtil.ComputeDistanceBetween(pickUpLocation, driverLocation);

                });

                availableDrivers = availableDrivers.OrderBy(o => o.DistanceFromPickup).ToList();
                DriversFound?.Invoke(this, new DriverFoundEventArgs { Drivers = availableDrivers, Ride = rides });
            }
            else
            {
                DriverNotFound.Invoke(this, new EventArgs());
            }
           
           
        }
    }
}

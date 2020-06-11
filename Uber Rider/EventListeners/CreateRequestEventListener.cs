using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Xamarin.Essentials;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using Uber_Rider.DataModels;
using Uber_Rider.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Uber_Rider.ReferenceModels;
using Uber_Rider.WebServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uber_Rider.EventListeners
{
    public class CreateRequestEventListener
    {
        //  NewTripDetails newTrip;
        Rides rides;
        FirebaseDatabase database;
        DatabaseReference newTripRef;
        DatabaseReference notifyDriverRef;
        HubConnection hubConnection;


        //NotifyDriver
        List<ActiveDrivers> mAvailableDrivers;
        ActiveDrivers selectedDriver;

        //Timer
        System.Timers.Timer RequestTimer = new System.Timers.Timer();
        int TimerCounter = 0;

        //Flags
        bool isDriverAccepted;
        bool isDriverRejected;

        //Events
        public class DriverAcceptedEventArgs : EventArgs
        {
            public AcceptedDriver acceptedDriver { get; set; }

        }
        public class SendNotificationEventArgs : EventArgs { 
        public string driverID { get; set; }
            public string rideDetails { get; set; }

        }
        public class TripUpdatesEventArgs : EventArgs
        {
            public LatLng DriverLocation { get; set; }
            public string Status { get; set; }
            public double Fares { get; set; }
        }

        public event EventHandler<DriverAcceptedEventArgs> DriverAccepted;
        public event EventHandler NoDriverAcceptedRequest;
        public event EventHandler<TripUpdatesEventArgs> TripUpdates;
        public  event EventHandler<SendNotificationEventArgs> SendNotification;
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDriverResponse(AcceptedDriver driverInfor)
        {

            if(!string.IsNullOrEmpty(driverInfor.fullname) && !isDriverAccepted)
            {
                isDriverAccepted = true;
                DriverAccepted.Invoke(this, new DriverAcceptedEventArgs { acceptedDriver = driverInfor });
                LatLng driverLocationLatLng = new LatLng(driverInfor.Latitude, driverInfor.Longitude);
                TripUpdates.Invoke(this, new TripUpdatesEventArgs 
                { 
                    DriverLocation = driverLocationLatLng,
                    Status = "accepted", 
                    Fares = Convert.ToDouble(rides.RidesInfo.TotalCost)
                });
            }
            else if (!string.IsNullOrEmpty(driverInfor.Status) && isDriverAccepted)
            {
                LatLng driverLocationLatLng = new LatLng(driverInfor.Latitude, driverInfor.Longitude);
                TripUpdates.Invoke(this, new TripUpdatesEventArgs
                {
                    DriverLocation = driverLocationLatLng,
                    Status = driverInfor.Status,
                    Fares = Convert.ToDouble(rides.RidesInfo.TotalCost)
                });
            }
            else
            {
                isDriverRejected = true;
            }
            //if(snapshot.Value != null)
            //{
            //    if(snapshot.Child("driver_id").Value.ToString() != "waiting")
            //    {
            //        string status = "";
            //        double fares = 0;

            //        if (!isDriverAccepted)
            //        {
            //            AcceptedDriver acceptedDriver = new AcceptedDriver();
            //            acceptedDriver.ID = snapshot.Child("driver_id").Value.ToString();
            //            acceptedDriver.fullname = snapshot.Child("driver_name").Value.ToString();
            //            acceptedDriver.phone = snapshot.Child("driver_phone").Value.ToString();
            //            isDriverAccepted = true;
            //            DriverAccepted.Invoke(this, new DriverAcceptedEventArgs { acceptedDriver = acceptedDriver });
            //        }

            //        //Gets Status
            //        if(snapshot.Child("status").Value != null)
            //        {
            //            status = snapshot.Child("status").Value.ToString();
            //        }

            //        //Get Fares
            //        if(snapshot.Child("fares").Value != null)
            //        {
            //            fares = double.Parse(snapshot.Child("fares").Value.ToString());
            //        }

            //        if (isDriverAccepted)
            //        {
            //            //Get Driver Location Updates
            //            double driverLatitude = double.Parse(snapshot.Child("driver_location").Child("latitude").Value.ToString());
            //            double driverLongitude = double.Parse(snapshot.Child("driver_location").Child("longitude").Value.ToString());
            //            LatLng driverLocationLatLng = new LatLng(driverLatitude, driverLongitude);
            //            TripUpdates.Invoke(this, new TripUpdatesEventArgs { DriverLocation = driverLocationLatLng , Status = status, Fares = fares});
            //        }
            //    }
            //}
        }
        public CreateRequestEventListener(Rides mNewTrip, HubConnection connection)
        {
            this.hubConnection = connection;
            rides = mNewTrip;


            //  database = AppDataHelper.GetDatabase();
            RequestTimer.Interval = 1000;
            RequestTimer.Elapsed += RequestTimer_Elapsed;
        }

        public async Task<ResponseData> CreateRequest()
        {
            return (await new TripServices().SaveRideRequest(rides));
        }
        public void CancelRequest()
        {
            if (selectedDriver != null)
            {
                //  DatabaseReference cancelDriverRef = database.GetReference("driversAvailable/" + selectedDriver.ID + "/ride_id");
                // cancelDriverRef.SetValue("cancelled");
            }
            //   newTripRef.RemoveEventListener(this);
            //  newTripRef.RemoveValue();
        }
        public void CancelRequestOnTimeout()
        {
            // newTripRef.RemoveEventListener(this);
            //  newTripRef.RemoveValue();
        }
        public RideDetails RideDetails()
        {
            CustomerInfo customer = new AppData().GetCurrentUser;
            return new RideDetails()
            {
                PickupAddress = rides.RidesInfo.LocationInfo.PickupLocationName,
                DestinationAddress = rides.RidesInfo.LocationInfo.PickupDestinationName,
                CustomerId = customer.CustomerId.ToString(),
                RiderName = customer.CustomerName,
                RiderPhone = customer.CustomerMobileNumber.ToString().Split('.')[0],
                PickupLat = Convert.ToDouble(rides.RidesInfo.LocationInfo.PickupLatitude),
                PickupLng = Convert.ToDouble(rides.RidesInfo.LocationInfo.PickupLongitude),
                DestinationLat = Convert.ToDouble(rides.RidesInfo.LocationInfo.DestinationLatitude),
                DestinationLng = Convert.ToDouble(rides.RidesInfo.LocationInfo.DestinationLongitude),
                RideId = rides.RideId.ToString(),
                Distance = rides.RidesInfo.LocationInfo.TotalDistance,
                EstimatedArrivalTime = Convert.ToDecimal(rides.RidesInfo.EstimatedArrivalTime),
                TotalCost = Convert.ToDecimal(rides.RidesInfo.TotalCost)
            };
        }
        public void NotifyDriver(List<ActiveDrivers> availableDrivers, Rides rideInfo)
        {
            rides = rideInfo;
            mAvailableDrivers = availableDrivers;
            if (mAvailableDrivers.Count >= 1 && mAvailableDrivers != null)
            {
                selectedDriver = mAvailableDrivers[0];


                //  notifyDriverRef = database.GetReference("driversAvailable/" + selectedDriver.ID + "/ride_id");
                // notifyDriverRef.SetValue(newTrip.RideID);
              
                string driverID = selectedDriver.DriverId.ToString();
                string rideDetails = JsonConvert.SerializeObject(RideDetails());
               SendNotification.Invoke(this, new SendNotificationEventArgs { driverID = driverID, rideDetails = rideDetails });
             //   hubConnection.InvokeAsync("NotifyDriver", driverID ,rideDetails );

                if (mAvailableDrivers.Count > 1)
                {
                    mAvailableDrivers.RemoveAt(0);
                }

                else if (mAvailableDrivers.Count == 1)
                {
                    //No more available drivers in our list
                    mAvailableDrivers = null;
                }

                RequestTimer.Enabled = true;
            }

            else
            {
                //No driver accepted
                RequestTimer.Enabled = false;
                NoDriverAcceptedRequest?.Invoke(this, new EventArgs());
            }
        }
        void RequestTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerCounter++;
            if (TimerCounter == 20 || isDriverRejected)
            {
                isDriverRejected = false;
                if (!isDriverAccepted)
                {
                    TimerCounter = 0;
                    //DatabaseReference cancelDriverRef = database.GetReference("driversAvailable/" + selectedDriver.ID + "/ride_id");
                    //cancelDriverRef.SetValue("timeout");
                    if (mAvailableDrivers != null)
                    {
                        NotifyDriver(mAvailableDrivers, rides);
                    }
                    else
                    {
                        RequestTimer.Enabled = false;
                        NoDriverAcceptedRequest?.Invoke(this, new EventArgs());
                    }
                }
            }
        }
        public void EndTrip()
        {
            // newTripRef.RemoveEventListener(this);
            //  newTripRef = null;
        }


    }
}
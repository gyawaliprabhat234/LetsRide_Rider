using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Firebase;
using Firebase.Database;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Gms.Location;
using Uber_Rider.Helpers;
using Android.Content;
using Android.Graphics;
using Android.Support.Design.Widget;
using Uber_Rider.EventListeners;
using Uber_Rider.Fragments;
using Uber_Rider.DataModels;
using System;
using Android.Media;
using Google.Places;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using System.Threading.Tasks;
using Uber_Rider.ReferenceModels;
using LetsRide;

namespace Uber_Rider
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        HubConnection hubConnection;
        //Firebase
        //   UserProfileEventListener profileEventListener = new UserProfileEventListener();
        CreateRequestEventListener requestListener;
        FindDriverListener findDriverListener;

        //Get logged user info and request

        //Views
        Android.Support.V7.Widget.Toolbar mainToolbar;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;

        //TextViews
        TextView pickupLocationText;
        TextView destinationText;
        TextView driverNameText;
        TextView tripStatusText;


        //Buttons
        Button favouritePlacesButton;
        Button locationSetButton;
        Button requestDriverButton;
        RadioButton pickupRadio;
        RadioButton destinationRadio;
        ImageButton callDriverButton;
        ImageButton cancelTripButton;
        RelativeLayout myLocation;


        //Imageview
        ImageView centerMarker;


        //Layouts
        RelativeLayout layoutPickUp;
        RelativeLayout layoutDestination;

        //Bottomsheets
        BottomSheetBehavior tripDetailsBottonsheetBehavior;
        BottomSheetBehavior driverAssignedBottomSheetBehavior;

        GoogleMap mainMap;

        readonly string[] permissionGroupLocation = { Android.Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.AccessCoarseLocation };
        const int requestLocationId = 0;

        LocationRequest mLocationRequest;
        FusedLocationProviderClient locationClient;
        Android.Locations.Location mLastLocation;
        LocationCallbackHelper mLocationCallback;

        static int UPDATE_INTERVAL = 5; //5 SECONDS
        static int FASTEST_INTERVAL = 5;
        static int DISPLACEMENT = 3; //meters

        //Helpers
        MapFunctionHelper mapHelper;

        //TripDetails
        LatLng pickupLocationLatlng;
        LatLng destinationLatLng;
        string pickupAddress;
        string destinationAddress;
        string totalDistance;
        string totalDuration;
        string setText = "";
        int activeDriver;
        int sendingRequest;
        string driverPhone;
        //Flags
        int addressRequest = 1;
        bool IsConnected = false;
        // 1 = Set Address as Pickup Location
        // 2 = Set Address as Destination Location

        // Set address from place search and Ignore Calling FindAddressFromCordinate Method when CameraIdle Event is Fired
        bool takeAddressFromSearch;
        Random random = new Random();

        //Fragments
        RequestDriver requestDriverFragment;
        View reqView;

        //DataModels
        //  NewTripDetails newTripDetails;
        Rides tripDetails;
        List<ActiveDrivers> activeDrivers;

        //selected vehicle
        VehicleType selectedVehicleType;

        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitializeConnection();
            ConnectControl();
            // StartConnectionToHub();
            string selectedVehicle = Intent.GetStringExtra("vehicleType") ?? string.Empty;
            selectedVehicleType = JsonConvert.DeserializeObject<VehicleType>(selectedVehicle);
            selectedVehicleType.Image = Common.GetImageBitmapFromUrl(LetsRideCredentials.WebUrl + selectedVehicleType.VehicleImage);
            SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
            CheckLocationPermission();
            CreateLocationRequest();
            GetMyLocation();
            StartLocationUpdates();
            //profileEventListener.Create();
            InitilizePlaces();

        }

        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Are you sure want to go back?");
            alert.SetMessage("You may lose your current processed data.");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                alert.Dispose();
                base.OnBackPressed();
            });
            alert.SetNegativeButton("No", (sendAlert, args) =>
            {
                alert.Dispose();
            });
            alert.Show();
           
        }

        async void InitializeConnection()
        {
            try
            {

                hubConnection = new HubConnectionBuilder().WithUrl(LetsRideCredentials.HubUrl
                        , options => options.AccessTokenProvider = () => Task.FromResult(new AppData().GetToken)
                     ).WithAutomaticReconnect().Build();
                await ConnectAsync();
                hubConnection.Closed += HubConnection_Closed;
                hubConnection.On<bool, string>("OnConnected", (status, message) =>
                {
                    //CloseProgressDialogue();
                    if (status)
                    {
                        //goOnlineButton.Enabled = true;
                        //availablityStatus = true;
                        //goOnlineButton.Text = "Go offline";
                        //goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.uberroundbutton_green);
                    }

                    else
                    {
                        //availablityStatus = false;
                        //goOnlineButton.Enabled = true;
                        //goOnlineButton.Text = "Go Online";
                        ////isConnectionSuccessful = false;
                        //Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                        //alert1.SetTitle("GO ONLINE");
                        //alert1.SetMessage("Request Failed, " + message + ". Try Again");
                        //alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                        //{
                        //    alert1.Dispose();
                        //});

                        //alert1.Show();
                    }

                });
                hubConnection.On<string, string, Nullable<double>, Nullable<double>>("ReceiveResponse", (driverName, driverPhone, latitude, longitude) =>
               {
                   if (string.IsNullOrEmpty(driverName))
                   {
                       requestListener.OnDriverResponse(new AcceptedDriver() );
                   }
                   else
                   {
                       this.driverPhone = driverPhone;
                       requestListener.OnDriverResponse(new AcceptedDriver()
                       {
                           fullname = driverName,
                           phone = driverPhone,
                           Latitude = Convert.ToDouble(latitude),
                           Longitude = Convert.ToDouble(longitude)
                       });
                   }
               });

                hubConnection.On<string>("ErrorInformation", (message) =>
                {
                    if (message == "notonline")
                    {
                        requestListener.OnDriverResponse(null);
                    }
                    else
                    {
                        Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert1.SetTitle("Request Failed");
                        alert1.SetMessage(message);
                        alert1.SetPositiveButton("Continue", (senderAlert, args) =>
                        {
                            alert1.Dispose();
                        });
                        alert1.Show();
                    }
                });

                hubConnection.On<string>("TripInformation", (info) =>
                {
                    if (!string.IsNullOrEmpty(info))
                    {
                        AcceptedDriver driverInfo = JsonConvert.DeserializeObject<AcceptedDriver>(info);
                        requestListener.OnDriverResponse(driverInfo);
                    }

                });


            }
            catch (Exception ex)
            {
                // CloseProgressDialogue();
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error in connection");
                alert.SetMessage("Request Failed, " + ex.Message + ". Try Again");
                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    alert.Dispose();
                });
                alert.Show();
            }

        }

        private async Task HubConnection_Closed(Exception exception)
        {
            IsConnected = false;

            await Task.Delay(random.Next(1, 5) * 1000);
            try
            {
                await ConnectAsync();

            }
            catch (Exception ex)
            {
                ExceptionDialogue("Error", ex.Message);

            }


        }

        void ExceptionDialogue(string title, string message)
        {
            Android.Support.V7.App.AlertDialog.Builder alert1 = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert1.SetTitle(title);
            alert1.SetMessage(message);
            alert1.SetPositiveButton("Continue", (senderAlert, args) =>
            {
                alert1.Dispose();
            });
            alert1.Show();
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
                IsConnected = true;
            }
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;
            try
            {
                await hubConnection.DisposeAsync();
                IsConnected = false;
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }

        //void StartConnectionToHub()
        //{
        //    hubConnection = new HubConnectionBuilder().WithUrl(LetsRideCredentials.HubUrl
        //           , options => options.AccessTokenProvider = () => Task.FromResult(new AppData().GetToken)
        //        ).WithAutomaticReconnect().Build();
        //    hubConnection.StartAsync();

        //    hubConnection.Closed += HubConnection_Closed;


        //}CancelRequest

        void ConnectControl()
        {
            //DrawerLayout
            drawerLayout = (Android.Support.V4.Widget.DrawerLayout)FindViewById(Resource.Id.drawerLayout);

            //ToolBar
            mainToolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.mainToolbar);
            SetSupportActionBar(mainToolbar);
            SupportActionBar.Title = "";
            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_action);
            actionBar.SetDisplayHomeAsUpEnabled(true);

            //TextView 
            pickupLocationText = (TextView)FindViewById(Resource.Id.pickupLocationText);
            destinationText = (TextView)FindViewById(Resource.Id.destinationText);
            tripStatusText = (TextView)FindViewById(Resource.Id.tripStatusText);
            driverNameText = (TextView)FindViewById(Resource.Id.driverNameText);

            //Buttons
            favouritePlacesButton = (Button)FindViewById(Resource.Id.favouritePlacesButton);
            locationSetButton = (Button)FindViewById(Resource.Id.locationsSetButton);
            requestDriverButton = (Button)FindViewById(Resource.Id.requestDriverButton);
            pickupRadio = (RadioButton)FindViewById(Resource.Id.pickupRadio);
            

            destinationRadio = (RadioButton)FindViewById(Resource.Id.DestinationRadio);
            favouritePlacesButton.Visibility = ViewStates.Gone;
            callDriverButton = (ImageButton)FindViewById(Resource.Id.callDriverButton);
            cancelTripButton = (ImageButton)FindViewById(Resource.Id.callDriverButton);
            myLocation = (RelativeLayout)FindViewById(Resource.Id.mylocation);

            myLocation.Click += MyLocation_Click;
            favouritePlacesButton.Click += FavouritePlacesButton_Click;
            locationSetButton.Click += LocationSetButton_Click;
            requestDriverButton.Click += RequestDriverButton_Click;
            pickupRadio.Click += PickupRadio_Click;
            destinationRadio.Click += DestinationRadio_Click;

            callDriverButton.Click += CallDriverButton_Click;
            //Layouts
            layoutPickUp = (RelativeLayout)FindViewById(Resource.Id.layoutPickUp);
            layoutDestination = (RelativeLayout)FindViewById(Resource.Id.layoutDestination);

            layoutPickUp.Click += LayoutPickUp_Click;
            layoutDestination.Click += LayoutDestination_Click;

            //Imageview
            centerMarker = (ImageView)FindViewById(Resource.Id.centerMarker);

            //Bottomsheet
            FrameLayout tripDetailsView = (FrameLayout)FindViewById(Resource.Id.tripdetails_bottomsheet);
            FrameLayout rideInfoSheet = (FrameLayout)FindViewById(Resource.Id.bottom_sheet_trip);

            tripDetailsBottonsheetBehavior = BottomSheetBehavior.From(tripDetailsView);
            driverAssignedBottomSheetBehavior = BottomSheetBehavior.From(rideInfoSheet);

        }

        private void MyLocation_Click(object sender, EventArgs e)
        {
            GetMyLocation();
        }

        private void CallDriverButton_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("tel:" + driverPhone);
            Intent intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        void InitilizePlaces()
        {
            string mapkey = Resources.GetString(Resource.String.mapkey);
            if (!PlacesApi.IsInitialized)
            {
                PlacesApi.Initialize(this, mapkey);
            }
        }

        #region CLICK EVENT HANDLERS

        private void RequestDriverButton_Click(object sender, System.EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder confirmRequestDiaglog = new Android.Support.V7.App.AlertDialog.Builder(this);
            confirmRequestDiaglog.SetTitle("Are You sure Want to Request Driver?");
            confirmRequestDiaglog.SetMessage("Your PickUp Address :" + pickupAddress + "\n Destination Address :" + destinationAddress);
            confirmRequestDiaglog.SetPositiveButton("Yes", (confirm, args) =>
            {

                RequestDriver();
            });

            confirmRequestDiaglog.SetNegativeButton("Cancel", (deleteAlert, args) =>
            {

                confirmRequestDiaglog.Dispose();
            });

            confirmRequestDiaglog.Show();
        }
        async void RequestDriver()
        {

            try
            {
                if (!IsConnected)
                    await ConnectAsync();

                if (tripDetails == null || (tripDetails != null && tripDetails.Action != "E"))
                {
                    tripDetails = new Rides();
                    RidesLocationInfo locationInfo = new RidesLocationInfo();
                    RidesInfo ridesInfo = new RidesInfo();
                    #region location Information 
                    locationInfo.PickupDestinationName = destinationAddress;
                    locationInfo.PickupLocationName = pickupAddress;
                    locationInfo.DestinationLatitude = Convert.ToDecimal(destinationLatLng.Latitude);
                    locationInfo.DestinationLongitude = Convert.ToDecimal(destinationLatLng.Longitude);
                    locationInfo.TotalDistance = Convert.ToDecimal(mapHelper.distance);
                    locationInfo.PickupLatitude = Convert.ToDecimal(pickupLocationLatlng.Latitude);
                    locationInfo.PickupLongitude = Convert.ToDecimal(pickupLocationLatlng.Longitude);
                    #endregion
                    #region Rides Information
                    ridesInfo.EstimatedArrivalTime = Convert.ToDecimal(mapHelper.duration);
                    ridesInfo.TotalCost = Convert.ToDecimal(mapHelper.EstimateFares(selectedVehicleType));
                    ridesInfo.RideBookingTime = DateTime.Now;
                    #endregion
                    #region tripInformation 
                    tripDetails.CustomerId = new AppData().GetCurrentUser.CustomerId;
                    tripDetails.VehicleTypeId = selectedVehicleType.TypeId;
                    tripDetails.RideTypeId = 1;
                    tripDetails.RidesInfo = ridesInfo;
                    tripDetails.RidesInfo.LocationInfo = locationInfo;
                    #endregion
                }
                requestListener = new CreateRequestEventListener(tripDetails, hubConnection);
                requestListener.NoDriverAcceptedRequest += RequestListener_NoDriverAcceptedRequest;
                requestListener.DriverAccepted += RequestListener_DriverAccepted;
                requestListener.TripUpdates += RequestListener_TripUpdates;
                requestListener.SendNotification += RequestListener_SendNotification;

                ResponseData response = await requestListener.CreateRequest();
                if (response.IsSuccess == false)
                {
                    requestListener.CancelRequest();
                    requestListener = null;
                    requestDriverFragment.Dismiss();
                    requestDriverFragment = null;
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Message");
                    alert.SetMessage("Request not Completed. " + response.Message);
                    alert.Show();
                }
                else
                {
                    tripDetails = FetchInformation(response);
                    findDriverListener = new FindDriverListener(tripDetails, hubConnection);
                    findDriverListener.DriversFound += FindDriverListener_DriversFound;
                    findDriverListener.DriverNotFound += FindDriverListener_DriverNotFound;
                    requestDriverFragment = new RequestDriver(Convert.ToDouble(tripDetails.RidesInfo.TotalCost), selectedVehicleType.Image);
                    requestDriverFragment.Cancelable = false;
                    var trans = SupportFragmentManager.BeginTransaction();
                    requestDriverFragment.Show(trans, "Request");
                    requestDriverFragment.CancelRequest += RequestDriverFragment_CancelRequest;
                    // reqView = requestDriverFragment.View;
                    findDriverListener.Create(activeDrivers);
                }
            }
            catch (Exception ex)
            {
                ExceptionDialogue("Error", "Error in connection please try again");
            }
        }

        async void RequestListener_SendNotification(object sender, CreateRequestEventListener.SendNotificationEventArgs e)
        {
            try
            {
                if (!IsConnected)
                    await ConnectAsync();
                sendingRequest++;
                //  TextView status = (TextView)FindViewById(Resource.Id.statusText);
                //     status.Text= "Found " + activeDriver + " Drivers.\n Sending request " +
                //    sendingRequest + " out of " + activeDriver + " Drivers";
                await hubConnection.InvokeAsync("PushRequest", e.driverID, e.rideDetails);

            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", "Error in Connection please try again");
            }
        }

        public Rides FetchInformation(ResponseData response)
        {
            List<ActiveDrivers> availableDrivers = JsonConvert.DeserializeObject<List<ActiveDrivers>>(response.CallBack);
            activeDrivers = new List<ActiveDrivers>();
            activeDrivers = availableDrivers;
            Rides rides = JsonConvert.DeserializeObject<Rides>(response.RecordsInString);
            //Guid r = rides.RideId;
            //Guid d = rides.RidesInfo.LocationInfo.LocationId;
            rides.Action = "E";
            return rides;

        }

        void RequestListener_TripUpdates(object sender, CreateRequestEventListener.TripUpdatesEventArgs e)
        {
            if (e.Status == "accepted")
            {
                tripStatusText.Text = "Coming";
                mapHelper.UpdateDriverLocationToPickUp(pickupLocationLatlng, e.DriverLocation);
            }

            else if (e.Status == "arrived")
            {
                tripStatusText.Text = "Driver Arrived";
                mapHelper.UpdateDriverArrived();
                MediaPlayer player = MediaPlayer.Create(this, Resource.Raw.alert);
                player.Start();
            }

            else if (e.Status == "ontrip")
            {
                tripStatusText.Text = "On Trip";
                mapHelper.UpdateLocationToDestination(e.DriverLocation, destinationLatLng);
            }

            else if (e.Status == "ended")
            {
             //   requestListener.EndTrip();
                requestListener = null;
                TripLocationUnset();

                driverAssignedBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

                MakePaymentFragment makePaymentFragment = new MakePaymentFragment(e.Fares, selectedVehicleType.Image);
                makePaymentFragment.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                makePaymentFragment.Show(trans, "payment");
                makePaymentFragment.PaymentCompleted += (i, p) =>
                {
                    makePaymentFragment.Dismiss();
                };
            }
        }

        void RequestListener_DriverAccepted(object sender, CreateRequestEventListener.DriverAcceptedEventArgs e)
        {
            if (requestDriverFragment != null)
            {
                requestDriverFragment.Dismiss();
                requestDriverFragment = null;
            }

            driverNameText.Text = e.acceptedDriver.fullname;
            tripStatusText.Text = "Coming";

            tripDetailsBottonsheetBehavior.State = BottomSheetBehavior.StateHidden;
            driverAssignedBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
        }

       async void RequestListener_NoDriverAcceptedRequest(object sender, EventArgs e)
        {
            try
            {
                RunOnUiThread(() =>
                   {
                       if (requestDriverFragment != null && requestListener != null)
                       {
                           requestListener.CancelRequestOnTimeout();
                           requestListener = null;
                           requestDriverFragment.Dismiss();
                           requestDriverFragment = null;

                           Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                           alert.SetTitle("Message");
                           alert.SetMessage("Available Drivers Couldn't Accept Your Ride Request, Try again in a few mnoment");
                           alert.Show();
                       }
                   });
                if (!IsConnected)
                    await ConnectAsync();
                await hubConnection.InvokeAsync("CancelRequest");
            }
            catch (Exception ex)
            {

                ExceptionDialogue("Error", ex.Message);
            }
        }
        void FindDriverListener_DriverNotFound(object sender, EventArgs e)
        {
            if (requestDriverFragment != null && requestListener != null)
            {
                requestListener.CancelRequest();
                requestListener = null;
                requestDriverFragment.Dismiss();
                requestDriverFragment = null;

                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Message");
                alert.SetMessage("No available driver found, try again in a few moments");
                alert.Show();
            }
        }

        void FindDriverListener_DriversFound(object sender, FindDriverListener.DriverFoundEventArgs e)
        {
            if (requestListener != null)
            {
                //  setText = "Found " + e.Drivers.Count + " Drivers.";
                activeDriver = e.Drivers.Count;
                sendingRequest = 0;
                requestListener.NotifyDriver(e.Drivers, e.Ride);
            }
        }

        async void RequestDriverFragment_CancelRequest(object sender, EventArgs e)
        {
            try
            {
                //User cancels request before driver accepts it
                if (requestDriverFragment != null && requestListener != null)
                {
                    // requestListener.CancelRequest();
                    requestListener = null;
                    requestDriverFragment.Dismiss();
                    requestDriverFragment = null;

                }

                if (!IsConnected)
                    await ConnectAsync();
                await hubConnection.InvokeAsync("CancelRequest");

            }
            catch (Exception ex)
            {
                ExceptionDialogue("Error", ex.Message);
            }
        }

        async void LocationSetButton_Click(object sender, System.EventArgs e)
        {
            myLocation.Visibility = ViewStates.Gone;
            locationSetButton.Text = "Please wait...";
            locationSetButton.Enabled = false;

            string json;
            json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatlng, destinationLatLng);

            if (!string.IsNullOrEmpty(json))
            {
                TextView txtFare = (TextView)FindViewById(Resource.Id.tripEstimateFareText);
                TextView txtTime = (TextView)FindViewById(Resource.Id.newTripTimeText);
                //    ImageView vehicleImageRequestDriver = (ImageView)FindViewById(Resource.Id.imageReqDriver);
                ImageView vehicleImageTripView = (ImageView)FindViewById(Resource.Id.imageTripView);
             //   int id = (int)typeof(Resource.Drawable).GetField(selectedVehicleType.VehicleImage).GetValue(null);
                //if (id != 0)
                //{

                    //    vehicleImageRequestDriver.SetImageResource(id);
               vehicleImageTripView.SetImageBitmap(selectedVehicleType.Image);
                //}
                // vehicleImageTripView.SetImageResource(Resource.Drawable.scooter);
                mapHelper.DrawTripOnMap(json);


                // Set Estimate Fares and Time
                txtFare.Text = "NPR." + mapHelper.EstimateFares(selectedVehicleType).ToString();
                txtTime.Text = mapHelper.distanceString + " | " + mapHelper.durationstring;
                requestDriverButton.Text = "Request " + selectedVehicleType.VehicleName;

                //Display BottomSheet
                tripDetailsBottonsheetBehavior.State = BottomSheetBehavior.StateExpanded;

                //DisableViews
                TripDrawnOnMap();
            }

            locationSetButton.Text = "Done";
            locationSetButton.Enabled = true;

        }

        void FavouritePlacesButton_Click(object sender, System.EventArgs e)
        {

        }
        void PickupRadio_Click(object sender, System.EventArgs e)
        {
            addressRequest = 1;
            pickupRadio.Checked = true;
            destinationRadio.Checked = false;
            takeAddressFromSearch = false;
            centerMarker.SetColorFilter(Color.DarkGreen);
        }

        void DestinationRadio_Click(object sender, System.EventArgs e)
        {
            addressRequest = 2;
            destinationRadio.Checked = true;
            pickupRadio.Checked = false;
            takeAddressFromSearch = false;
            centerMarker.SetColorFilter(Color.Red);

        }


        void LayoutPickUp_Click(object sender, System.EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);

            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("NP")
                .Build(this);

            StartActivityForResult(intent, 1);
        }

        void LayoutDestination_Click(object sender, System.EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>();
            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);

            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("NP")
                .Build(this);

            StartActivityForResult(intent, 2);

        }

        #endregion

        #region MAP AND LOCATION SERVICES

        public void OnMapReady(GoogleMap googleMap)
        {

            // Enable Mapstyle

            // var mapStyle = MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.silvermapstyle);
            // googleMap.SetMapStyle(mapStyle);

            mainMap = googleMap;
            mainMap.CameraIdle += MainMap_CameraIdle;
            string mapkey = Resources.GetString(Resource.String.mapkey);
            mapHelper = new MapFunctionHelper(mapkey, mainMap);


        }

        async void MainMap_CameraIdle(object sender, System.EventArgs e)
        {
            if (!takeAddressFromSearch)
            {
                if (addressRequest == 1)
                {
                    pickupLocationLatlng = mainMap.CameraPosition.Target;
                    pickupAddress = await mapHelper.FindCordinateAddress(pickupLocationLatlng);
                    pickupLocationText.Text = pickupAddress;
                }
                else if (addressRequest == 2)
                {
                    destinationLatLng = mainMap.CameraPosition.Target;
                    destinationAddress = await mapHelper.FindCordinateAddress(destinationLatLng);
                    destinationText.Text = destinationAddress;
                    TripLocationsSet();
                }
            }

        }

        bool CheckLocationPermission()
        {
            bool permissionGranted = false;

            if (ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, requestLocationId);
            }
            else
            {
                permissionGranted = true;
            }

            return permissionGranted;
        }

        void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL);
            mLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = LocationServices.GetFusedLocationProviderClient(this);
            mLocationCallback = new LocationCallbackHelper();
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;

        }

        void MLocationCallback_MyLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;
            LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 12));
        }

        void StartLocationUpdates()
        {
            if (CheckLocationPermission())
            {
                locationClient.RequestLocationUpdates(mLocationRequest, mLocationCallback, null);
            }
        }

        void StopLocationUpdates()
        {
            if (locationClient != null && mLocationCallback != null)
            {
                locationClient.RemoveLocationUpdates(mLocationCallback);
            }
        }

        async void GetMyLocation()
        {
            if (!CheckLocationPermission())
            {
                return;
            }

            mLastLocation = await locationClient.GetLastLocationAsync();
            if (mLastLocation != null)
            {
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 17));
            }
        }


        #endregion

        #region TRIP CONFIGURATIONS
        void TripLocationsSet()
        {
            favouritePlacesButton.Visibility = ViewStates.Invisible;
            locationSetButton.Visibility = ViewStates.Visible;
        }

        void TripLocationUnset()
        {
            mainMap.Clear();
            tripDetails = null;
            layoutPickUp.Clickable = true;
            layoutDestination.Clickable = true;
            pickupRadio.Enabled = true;
            destinationRadio.Enabled = true;
            takeAddressFromSearch = false;
            centerMarker.Visibility = ViewStates.Visible;
          //  favouritePlacesButton.Visibility = ViewStates.Visible;
            locationSetButton.Visibility = ViewStates.Invisible;
            tripDetailsBottonsheetBehavior.State = BottomSheetBehavior.StateHidden;
            GetMyLocation();
        }


        void TripDrawnOnMap()
        {
            layoutDestination.Clickable = false;
            layoutPickUp.Clickable = false;
            pickupRadio.Enabled = false;
            destinationRadio.Enabled = false;
            takeAddressFromSearch = true;
            centerMarker.Visibility = ViewStates.Invisible;
        }


        #endregion

        #region OVERRIDE METHODS


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults.Length < 1)
            {
                return;
            }
            if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
            {
                StartLocationUpdates();
            }
            else
            {

            }
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    takeAddressFromSearch = true;
                    pickupRadio.Checked = false;
                    destinationRadio.Checked = false;

                    var place = Autocomplete.GetPlaceFromIntent(data);
                    pickupLocationText.Text = place.Name.ToString();
                    pickupAddress = place.Name.ToString();
                    pickupLocationLatlng = place.LatLng;
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    centerMarker.SetColorFilter(Color.DarkGreen);
                }
            }

            if (requestCode == 2)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    takeAddressFromSearch = true;
                    pickupRadio.Checked = false;
                    destinationRadio.Checked = false;

                    var place = Autocomplete.GetPlaceFromIntent(data);
                    destinationText.Text = place.Name.ToString();
                    destinationAddress = place.Name.ToString();
                    destinationLatLng = place.LatLng;
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    centerMarker.SetColorFilter(Color.Red);
                    TripLocationsSet();
                }
            }
        }

        #endregion
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);


            }
        }

        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
    }
}
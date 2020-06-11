using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Uber_Rider.DataModels;
using Uber_Rider.Adapters;
using Newtonsoft.Json;
using Uber_Rider.WebServices;
using LetsRide;

namespace Uber_Rider.Activities
{
    [Activity(Label = "@string/app_name")]
    public class VehicleTypeActivity : Activity
    {
        RecyclerView recyclerView;
        List<VehicleType> vehicletypeList;
        List<VehicleCategory> vehicleCategories;
        VehicleAdapter adapter;
        VehicleType selectedVehicleType = new VehicleType();
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        protected  override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_vehicletype);
            recyclerView = (RecyclerView)FindViewById(Resource.Id.myRecyclerView);
            GetVehicleTypeData();
           
        }
        private void SetUpRecyclerView()
        {
            recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(recyclerView.Context));
            adapter = new VehicleAdapter(vehicletypeList);
            adapter.ItemClick += Adapter_ItemClick;
            recyclerView.SetAdapter(adapter);
        }

        private void Adapter_ItemClick(object sender, VehicleAdapterClickEventArgs e)
        {
            selectedVehicleType = vehicletypeList[e.Position];
            Android.Support.V7.App.AlertDialog.Builder 
                selectVehicle = new Android.Support.V7.App.AlertDialog.Builder(this);
            selectVehicle.SetTitle("You Have Selected " + vehicletypeList[e.Position].VehicleName);
            selectVehicle.SetMessage("Are you sure want to go with " + 
                vehicletypeList[e.Position].VehicleName + "?");
            selectVehicle.SetPositiveButton("Continue", (deleteAlert, args) =>
            {
                var activity = new Intent(this, typeof(MainActivity));
                // activity.PutExtra("VehicleImage", selectedVehicleType.Image);
                selectedVehicleType.Image = null;
                string selectedType = JsonConvert.SerializeObject(selectedVehicleType);
                activity.PutExtra("vehicleType", selectedType);
                StartActivity(activity);
                this.Finish();
            });
            selectVehicle.SetNegativeButton("Cancel", (deleteAlert, args) =>
            {
                selectedVehicleType = new VehicleType();
                selectVehicle.Dispose();
            });

            selectVehicle.Show();

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

        private async void GetVehicleTypeData()
        {
            ShowProgressDialogue();
            ResponseData response = await new VehicleTypeServices().GetActiveVehicleType();
            vehicletypeList = JsonConvert.DeserializeObject<List<VehicleType>>(response.RecordsInString);
            vehicleCategories = JsonConvert.DeserializeObject<List<VehicleCategory>>(response.CallBack);
            vehicletypeList.ForEach(x => 
            {
                x.CategoryName = vehicleCategories.Find(y => y.CategoryId == x.CategoryId).CategoryName;
                x.Image = DataModels.Common.GetImageBitmapFromUrl(LetsRideCredentials.WebUrl + x.VehicleImage);
            });
            CloseProgressDialogue(); 
            SetUpRecyclerView();

        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using LetsRide;

namespace Uber_Rider.Fragments
{
    public class RequestDriver : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler CancelRequest;

        double mfares;
        Button cancelRequestButton;
        TextView faresText;
     // TextView statusText;
        Bitmap image;
        ImageView  imageReqDriver;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.request_driver, container, false);
            cancelRequestButton = (Button)view.FindViewById(Resource.Id.cancelrequestButton);
            cancelRequestButton.Click += CancelRequestButton_Click;
            faresText = (TextView)view.FindViewById(Resource.Id.faresText);
            imageReqDriver = (ImageView)view.FindViewById(Resource.Id.imageReqDriver);
          //  statusText = (TextView)view.FindViewById(Resource.Id.statusText);
            imageReqDriver.SetImageBitmap(image);
            faresText.Text = "NPR." + mfares.ToString();
          //  statusText.Text = "Requesting";
            return view;
        }

        public RequestDriver(double fares , Bitmap vehicleImage)
        {
            mfares = fares;
            image = vehicleImage;
            
        }

        void CancelRequestButton_Click(object sender, EventArgs e)
        {
            CancelRequest?.Invoke(this, new EventArgs());
        }

        //public static void SetText(string text)
        //{
        //    statusText.Text = text;
        //}
    }
}
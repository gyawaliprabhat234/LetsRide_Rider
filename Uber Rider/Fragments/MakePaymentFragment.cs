
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
    public class MakePaymentFragment : Android.Support.V4.App.DialogFragment
    {
        double mfares;
        TextView totalFaresText;
        Button makePaymentButton;
        ImageView imageReqDriver;
        Bitmap image;

        public event EventHandler PaymentCompleted;

        public MakePaymentFragment(double fares, Bitmap vehicleImage)
        {
            mfares = fares;
            image = vehicleImage;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
         View view =  inflater.Inflate(Resource.Layout.makepayment, container, false);
            totalFaresText = (TextView)view.FindViewById(Resource.Id.totalfaresText);
            makePaymentButton = (Button)view.FindViewById(Resource.Id.makePaymentButton);
            imageReqDriver = (ImageView)view.FindViewById(Resource.Id.imageReqDriver);
            imageReqDriver.SetImageBitmap(image);
            totalFaresText.Text = "NPR. " + mfares.ToString();
            makePaymentButton.Click += MakePaymentButton_Click;

            return view;
        }

        void MakePaymentButton_Click(object sender, EventArgs e)
        {
            PaymentCompleted.Invoke(this, new EventArgs());
        }

    }
}

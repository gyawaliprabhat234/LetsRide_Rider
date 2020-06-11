
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Uber_Rider.Helpers;
using Uber_Rider.WebServices;

namespace Uber_Rider.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true,  ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);           
            // Create your application here
        }

        protected async override void OnResume()
        {
            base.OnResume();

            //  FirebaseUser currentUser = AppDataHelper.GetCurrentUser();
            bool isLoggedIn = await new LoginService().IsLogggedIn();

            if (isLoggedIn)
            {
                StartActivity(typeof(VehicleTypeActivity));
            }
            else
            {
                StartActivity(typeof(OtpLoginActivity));
            }
        }
    }
}

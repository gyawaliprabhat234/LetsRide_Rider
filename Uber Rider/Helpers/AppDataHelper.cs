using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

namespace Uber_Rider.Helpers
{
   public static class AppDataHelper
    {
       static  ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);

        public static FirebaseDatabase GetDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                    .SetApplicationId("uber-clone-8af91")
                    .SetApiKey("AIzaSyCXC1uk2DWYP10GJYU2r4vlN-U43Vu5F_g")
                    .SetDatabaseUrl("https://uber-clone-8af91.firebaseio.com")
                    .SetStorageBucket("uber-clone-8af91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }


        public static FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                     .SetApplicationId("uber-clone-8af91")
                    .SetApiKey("AIzaSyCXC1uk2DWYP10GJYU2r4vlN-U43Vu5F_g")
                    .SetDatabaseUrl("https://uber-clone-8af91.firebaseio.com")
                    .SetStorageBucket("uber-clone-8af91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

            return mAuth;
        }

        public static FirebaseUser GetCurrentUser()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseAuth mAuth;
            FirebaseUser mUser;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                    .SetApplicationId("uber-clone-8af91")
                    .SetApiKey("AIzaSyCXC1uk2DWYP10GJYU2r4vlN-U43Vu5F_g")
                    .SetDatabaseUrl("https://uber-clone-8af91.firebaseio.com")
                    .SetStorageBucket("uber-clone-8af91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
                mUser = mAuth.CurrentUser;
            }

            return mUser;
        }

        public static string GetFullName()
        {
            string fullname = preferences.GetString("FullName", "");
            return fullname;
        }

        public static string GetEmail()
        {
            string email = preferences.GetString("Email", "");
            return email;
        }

        public static string GetPhone()
        {
            string phone = preferences.GetString("PhoneNumber", "");
            return phone;
        }
    }
}
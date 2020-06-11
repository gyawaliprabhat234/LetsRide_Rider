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
using Firebase.Database;
using Uber_Rider.Helpers;

namespace Uber_Rider.EventListeners
{
    public class UserProfileEventListener : Java.Lang.Object, IValueEventListener
    {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                string fullname, email, phone = "";
                fullname = (snapshot.Child("FullName") != null) ? snapshot.Child("FullName").Value.ToString() : "";
                email = (snapshot.Child("Email") != null) ? snapshot.Child("Email").Value.ToString() : "";
                if(snapshot.Child("PhoneNumber") != null)
                {
                    phone = snapshot.Child("PhoneNumber").Value.ToString();
                }

                editor.PutString("FullName", fullname);
                editor.PutString("Email", email);
                editor.PutString("PhoneNumber", phone);
                editor.Apply();
            }
        }

        public void Create()
        {
            editor = preferences.Edit();
            FirebaseDatabase database = AppDataHelper.GetDatabase();
            string userId = AppDataHelper.GetCurrentUser().PhoneNumber;
            DatabaseReference profileReference = database.GetReference("users/" + userId);
            profileReference.AddValueEventListener(this);
        }
    }
}
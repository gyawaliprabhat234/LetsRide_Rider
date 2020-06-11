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
using Newtonsoft.Json;
using Uber_Rider.DataModels;

namespace Uber_Rider.Helpers
{
  public  class AppData
  {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences(LetsRideCredentials.SessionName, FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        public void SetUserInformation(CustomerInfo user , string token)
        {
            editor = preferences.Edit();
            editor.PutString("userinfo", JsonConvert.SerializeObject(user));
            editor.PutString("token", token);
            editor.Apply();
        }

        public CustomerInfo GetCurrentUser
        {
            get
            {
                CustomerInfo userInfo = new CustomerInfo();
                string user = preferences.GetString("userinfo", null);
                if (!string.IsNullOrEmpty(user))
                {
                    userInfo = JsonConvert.DeserializeObject<CustomerInfo>(user);
                    userInfo.CustomerId = Guid.Parse(userInfo.CustomerIdString);
                }
                return userInfo;
            }
        }

        public string GetToken
        {
            get
            {
                return preferences.GetString("token", null);
            }
        }

    }
}
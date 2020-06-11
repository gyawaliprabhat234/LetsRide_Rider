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
using GoogleGson;
using Java.Util;
using Uber_Rider.DataModels;
using Uber_Rider.Helpers;

namespace Uber_Rider.EventListeners
{
    public class CustomerEventListener : Java.Lang.Object, IValueEventListener
    {
        List<CustomerInfo> customerList = new List<CustomerInfo>();

        public event EventHandler<CustomerDataEventArgs> customerRetrived;

        public class CustomerDataEventArgs : EventArgs
        {
            public List<CustomerInfo> Customers { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                customerList.Clear();
                foreach (DataSnapshot data in child)
                {

                    CustomerInfo customer = new CustomerInfo();
                    customer = Common.Cast<CustomerInfo>(data);
                    customerList.Add(customer);

                }
                customerRetrived.Invoke(this, new CustomerDataEventArgs { Customers = customerList });
            }
        }
        public void InitializeDatabase()
        {
          DatabaseReference reference =   AppDataHelper.GetDatabase().GetReference("RiderInfo");
            reference.AddValueEventListener(this);
        }
    }
}
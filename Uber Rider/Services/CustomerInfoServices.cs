using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Uber_Rider.DataModels;

namespace Uber_Rider.Services
{
    public class CustomerInfoServices
    {
        public HashMap AddCustomer(CustomerInfo customer)
        {
            HashMap map = new HashMap();
            HashMap deviceMap = new HashMap();
            PropertyInfo[] properties = customer.GetType().GetProperties();
            //foreach (PropertyInfo pi in properties)
            //{
            //    if (pi.Name == "InfoDevice")
            //    {
                    
            //        PropertyInfo[] infoDevice = customer.InfoDevice.GetType().GetProperties();
            //        foreach(PropertyInfo p in infoDevice)
            //        {
            //            deviceMap.Put(p.Name, p.GetValue(customer.InfoDevice, null).ToString());
            //        }
            //        map.Put(pi.Name, deviceMap);

            //    }
            //    else

            //        map.Put(pi.Name, pi.GetValue(customer, null).ToString());


            //}

            //
            return map;
        }

        
    }

    
}
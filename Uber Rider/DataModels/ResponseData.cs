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

namespace Uber_Rider.DataModels
{
    public class ResponseData
    {
        public bool IsSuccess { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public object Records { get; set; }
        public string RecordsInString { get; set; }
        public string CallBack { get; set; }
        public bool IsToken { get; set; }
        public Guid Id { get; set; }
        public decimal Code { get; set; }

    }
}
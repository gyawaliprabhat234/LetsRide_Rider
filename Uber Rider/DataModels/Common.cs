using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using GoogleGson;
using Java.IO;
using Java.Util;
using Newtonsoft.Json;

namespace Uber_Rider.DataModels
{
    public static  class Common
    {
        public static readonly string firebase = "https://uber-clone-8af91.firebaseio.com";
        public static bool EmailIsValid(string email)
        {
            // string expression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            string expression = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
             + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
             + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            if (Regex.IsMatch(email, expression))
            {
                if (Regex.Replace(email, expression, string.Empty).Length == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
        //public static string EncodeToBase64(Bitmap image, Bitmap.CompressFormat compressFormat, int quality)
        //{
        //    System.IO.MemoryStream byteArrayOS = new System.IO.MemoryStream();
        //    image.Compress(compressFormat, quality, byteArrayOS);
        //    return Base64.encodeToString(byteArrayOS.toByteArray(), Base64.DEFAULT);
        //}

        //public static Bitmap decodeBase64(String input)
        //{
        //    byte[] decodedBytes = Base64.Deco(input, 0);
        //    return BitmapFactory.decodeByteArray(decodedBytes, 0, decodedBytes.length);
        //}
        public static T Cast<T>(DataSnapshot obj) where T : class
        {
            HashMap map = obj.Value.JavaCast<HashMap>();
            Gson gson = new GsonBuilder().Create();
            string jsonObj = gson.ToJson(map);

            return JsonConvert.DeserializeObject<T>(jsonObj) as T;
        }
        public static Java.Lang.Object ToJavaObject<TObject>(this TObject value)
        {
            if (Equals(value, default(TObject)) && !typeof(TObject).IsValueType)
                return null;

            var holder = new JavaHolder(value);

            return holder;
        }

    }
    public class JavaHolder : Java.Lang.Object
    {
        public readonly object Instance;

        public JavaHolder(object instance)
        {
            Instance = instance;
        }
    }
}

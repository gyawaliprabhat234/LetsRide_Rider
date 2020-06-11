using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Java.Util.Concurrent;
using LetsRide;
using Newtonsoft.Json;
using Uber_Rider.DataModels;
using Uber_Rider.EventListeners;
using Uber_Rider.Helpers;
using Uber_Rider.Services;
using Uber_Rider.WebServices;

namespace Uber_Rider.Activities
{

    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class OtpLoginActivity : Activity
    {
        readonly string[] permissionGroupLocation = { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation, Android.Manifest.Permission.ReadPhoneState };
        private FirebaseAuth mAuth;
        private FirebaseUser mCurrentUser;
        PhoneAuthCallbacks phoneAuthCallbacks = new PhoneAuthCallbacks();
        CustomerInfo customerInfo;
        FirebaseDatabase database;
        TextInputLayout phoneText;
        TextInputLayout fullNameText;
        TextInputLayout emailText;
        Button registerButton;
        CoordinatorLayout rootView;
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;
        CustomerEventListener customerEventListener;
        List<CustomerInfo> customerList = new List<CustomerInfo>();

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.otp_login);
            ConnectControls();
          //  InitializeFirebase();
            CheckPermission();
          //  RetriveData();
            // Create your application here
        }

        private void ConnectControls()
        {
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            registerButton.Click += RegisterButton_Click;

        }
        private void RetriveData()
        {
            customerEventListener = new CustomerEventListener();
            customerEventListener.InitializeDatabase();
            customerEventListener.customerRetrived += CustomerEventListener_customerRetrived;
        }

        private void CustomerEventListener_customerRetrived(object sender, CustomerEventListener.CustomerDataEventArgs e)
        {
            customerList = e.Customers;
        }
        bool CheckPermission()
        {
            bool permissionGranted = false;

            if (ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadPhoneState) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted
                )
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, 0);
            }
            else
            {
              
                permissionGranted = true;
            }

            return permissionGranted;
        }
        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            if (!CheckPermission())
            {
                Snackbar.Make(rootView, "Please allow the permissions", Snackbar.LengthShort).Show();
                return;
            }
            if(Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Snackbar.Make(rootView, "Internet connection is required", Snackbar.LengthShort).Show();
                return;
            }

            SetCustomerInfo();
            if (customerInfo.CustomerMobileNumber.ToString().Length != 10)
            {
                Snackbar.Make(rootView, "Please provide a valid Phone Number", Snackbar.LengthShort).Show();
                return;
            }
            if(customerInfo.CustomerName.Length < 6)
            {
                Snackbar.Make(rootView, "Please provide a valid name", Snackbar.LengthShort).Show();
                return;
            }
            if(customerInfo.CustomerEmail.Length != 0)
            {
                if (!DataModels.Common.EmailIsValid(customerInfo.CustomerEmail))
                {
                    Snackbar.Make(rootView, "Please provide a valid Email Address", Snackbar.LengthShort).Show();
                    return;
                }
            }
            ShowProgressDialogue();
            ResponseData response = await new RegistrationService().RegisterCustomer(customerInfo);
            if (response.IsSuccess)
            {
                CloseProgressDialogue();
                CustomerInfo userInfo = JsonConvert.DeserializeObject<CustomerInfo>(response.RecordsInString);
                if (Guid.TryParse(userInfo.CustomerIdString, out Guid result))
                    userInfo.CustomerId = result;
                else
                {
                    Snackbar.Make(rootView, "Operation Failed in conversion", Snackbar.LengthShort).Show();
                    return;
                }
                if (response.IsValid)
                    Login_Successful(userInfo, response.Token);
                else
                 Customer_CodeSent(userInfo);
            }
            else
            {
                CloseProgressDialogue();
                Snackbar.Make(rootView, "Operation Failed " + response.Message, Snackbar.LengthShort).Show();

            }
        }

        public  void SaveCustomerInfoToDatabase()
        {
            // Java.Lang.Object o = Common.ToJavaObject<CustomerInfo>(customerInfo);

          //  DatabaseReference userReference = database.GetReference("users/" + customerInfo.PhoneNumber);
         //   userReference.SetValue(new CustomerInfoServices().AddCustomer(customerInfo));
        }
        private void SetCustomerInfo()
        {
            Android.Telephony.TelephonyManager mTelephonyMgr;
            mTelephonyMgr = (Android.Telephony.TelephonyManager)GetSystemService(TelephonyService);
            CustomerDeviceInfo deviceInfo = new CustomerDeviceInfo()
            {
                DeviceSetPhoneNumber = Convert.ToDecimal(mTelephonyMgr.Line1Number),
                DeviceNumber = mTelephonyMgr.DeviceId, 
                DeviceModel = Xamarin.Essentials.DeviceInfo.Model,
                DeviceName = Xamarin.Essentials.DeviceInfo.Name,
                DeviceVersion = Xamarin.Essentials.DeviceInfo.VersionString,
                DeviceType = Convert.ToInt16(Xamarin.Essentials.DeviceInfo.DeviceType)

            };
            customerInfo = new CustomerInfo()
            {
                CustomerMobileNumber = Convert.ToDecimal(phoneText.EditText.Text),
                CustomerName = fullNameText.EditText.Text,
                CustomerEmail = emailText.EditText.Text,
                DeviceInfo = deviceInfo
            };
        }

        private void PhoneAuthCallbacks_VerificationFailed(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Login Failed " + phoneAuthCallbacks.ErrorMessage, Snackbar.LengthShort).Show();  
        }

        private void Customer_CodeSent(CustomerInfo info)
        {
            //string token = JsonConvert.SerializeObject(phoneAuthCallbacks._token);
                string customer = JsonConvert.SerializeObject(info);
                var activity = new Intent(this, typeof(OtpCheckingActivity));
                activity.PutExtra("customer", customer);
                this.Finish();
                StartActivity(activity);
               
              //  CloseProgressDialogue();
        }
        private void Login_Successful(CustomerInfo user, string token)
        {
            new AppData().SetUserInformation(user, token);
            var activity = new Intent(this, typeof(VehicleTypeActivity));
            this.Finish();
            StartActivity(activity);
          

        }

        private void PhoneAuthCallbacks_VerificationComplete(object sender, EventArgs e)
        {
            SignInWithPhoneAuthCredential();
        }

        private void SignInWithPhoneAuthCredential()
        {
            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;
            
            mAuth.SignInWithCredential(phoneAuthCallbacks._credential)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);
           
        }
        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Login Failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            SaveCustomerInfoToDatabase();
            CloseProgressDialogue();
            this.Finish();
            StartActivity(typeof(VehicleTypeActivity));
        }

        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
        void InitializeFirebase()
        {
            var app = FirebaseApp.InitializeApp(this);

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                     .SetApplicationId("uber-clone-8af91")
                    .SetApiKey("AIzaSyCXC1uk2DWYP10GJYU2r4vlN-U43Vu5F_g")
                    .SetDatabaseUrl("https://uber-clone-8af91.firebaseio.com")
                    .SetStorageBucket("uber-clone-8af91.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
                mAuth = FirebaseAuth.Instance;
            }


        }



    }

}
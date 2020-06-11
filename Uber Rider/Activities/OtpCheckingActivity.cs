using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
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
    [Activity(Label = "@string/app_name")]
    public class OtpCheckingActivity : Activity
    {
        private FirebaseAuth mAuth;
        TextInputLayout otpcodeText;
        TextView resendHelperText;
        Button checkOtpButton;
        Button resendText;
        CoordinatorLayout rootView;
        PhoneAuthProvider.ForceResendingToken forceResendingToken;
        PhoneAuthCallbacks phoneAuthCallbacks = new PhoneAuthCallbacks();
        CustomerInfo customerInfo;
        FirebaseDatabase database;
        string VerificationId;
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.otp_check);
            otpcodeText = (TextInputLayout)FindViewById(Resource.Id.otpcodeText);
            checkOtpButton = (Button)FindViewById(Resource.Id.checkOtpButton);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            resendHelperText = (TextView)FindViewById(Resource.Id.resendHelperText);
            resendText = (Button)FindViewById(Resource.Id.resendText);
            ResendCodeDelay();
            resendText.Click += ResendText_Click;   
            checkOtpButton.Click += CheckOtpButton_Click;
            string customer = Intent.GetStringExtra("customer") ?? string.Empty;
            customerInfo = JsonConvert.DeserializeObject<CustomerInfo>(customer);
            customerInfo.CustomerId = Guid.Parse(customerInfo.CustomerIdString);
          
        }

        private async void ResendText_Click(object sender, EventArgs e)
        {
            ShowProgressDialogue();
            Guid d = customerInfo.CustomerId;
            ResponseData response = await new LoginService().ResendVerificationCode(customerInfo);
            CloseProgressDialogue();
            if (response.IsSuccess)
            {
                ResendCodeDelay();
                Snackbar.Make(rootView, "Successfully code sent...", Snackbar.LengthShort).Show();
            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }

        }

        private void PhoneAuthCallbacks_VerificationFailed(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Failed to send code please try again" + phoneAuthCallbacks.ErrorMessage, Snackbar.LengthShort).Show();
        }

        private void PhoneAuthCallbacks_CodeSent(object sender, EventArgs e)
        {

           
            CloseProgressDialogue();
        //    Snackbar.Make(rootView, "Verification code successfully sent to "+customerInfo.PhoneNumber, Snackbar.LengthShort).Show();
         //   resendText.Visibility = ViewStates.Gone;
            ResendCodeDelay();
        }

        private void PhoneAuthCallbacks_VerificationComplete(object sender, EventArgs e)
        {
           
            SignInWithPhoneAuthCredential(phoneAuthCallbacks._credential);
        }
       
        public async void ResendCodeDelay()
        {
            resendText.Visibility = ViewStates.Gone;
            resendHelperText.Visibility = ViewStates.Visible;
            for (int i = 29; i >1 ; i--)
            {
                resendHelperText.Text = "Please wait " + i.ToString() + " seconds to resend code";
                await Task.Delay(1000);
            }
            resendHelperText.Text = "Now wait is finished. You can resend code again";
            await Task.Delay(1000);
            resendText.Visibility = ViewStates.Visible;
            resendHelperText.Visibility = ViewStates.Gone;


        }

        private async void CheckOtpButton_Click(object sender, EventArgs e)
        {
            
            if(otpcodeText.EditText.Text.Length != 6)
            {
                Snackbar.Make(rootView, "Code is not valid", Snackbar.LengthShort).Show();
                return;
            }
            if (Decimal.TryParse(otpcodeText.EditText.Text.Trim(), out decimal result))
            {
                customerInfo.Verification = new CustomerVerification();
                customerInfo.Verification.OTP = result;
            }
            else
            {
                Snackbar.Make(rootView, "Code is not valid", Snackbar.LengthShort).Show();
                return;
            }

            ShowProgressDialogue();

            ResponseData response = await new RegistrationService().CheckOTPCode(customerInfo);
            CloseProgressDialogue();
            if (response.IsSuccess && response.IsValid)
            {

                CustomerInfo userInfo = JsonConvert.DeserializeObject<CustomerInfo>(response.RecordsInString);
                Login_Successful(userInfo, response.Token);

            }
            else
            {
                Snackbar.Make(rootView, response.Message, Snackbar.LengthShort).Show();
            }
        }

        private void Login_Successful(CustomerInfo user, string token)
        {
            new AppData().SetUserInformation(user, token);
            var activity = new Intent(this, typeof(VehicleTypeActivity));
            this.Finish();
            StartActivity(activity);
           
        }

        private void SignInWithPhoneAuthCredential(PhoneAuthCredential credential)
        {
            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            mAuth.SignInWithCredential(credential)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);
          

        }
        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Verification falied", Snackbar.LengthShort).Show();
        }
        public void SaveCustomerInfoToDatabase()
        {
           
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
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }


        }
    }
}
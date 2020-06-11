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
using Uber_Rider.EventListeners;

namespace Uber_Rider.Helpers
{
    public  class PhoneAuthCallbacks : PhoneAuthProvider.OnVerificationStateChangedCallbacks
    {
        public PhoneAuthCredential _credential;
        public bool IsVerificationSucceed;
        public bool IsCodeSent;
        public string ErrorMessage;
        public string VerificationId;
        public  PhoneAuthProvider.ForceResendingToken _token;
        public event EventHandler VerificationComplete;
        public event EventHandler  VerificationFailed ;
        public event EventHandler CodeSent;


        public override void OnVerificationCompleted(PhoneAuthCredential credential)
        {
            this._credential = credential;
            IsVerificationSucceed = true;
            VerificationComplete?.Invoke(this, new EventArgs());
            // This callback will be invoked in two situations:
            // 1 - Instant verification. In some cases the phone number can be instantly
            // verified without needing to send or enter a verification code.
            // 2 - Auto-retrieval. On some devices Google Play services can automatically
            // detect the incoming verification SMS and perform verification without
            // user action.
        }
        public override void OnVerificationFailed(FirebaseException exception)
        {
            IsVerificationSucceed = false;
            ErrorMessage = exception.Message;
            VerificationFailed?.Invoke(this, new EventArgs());
            // This callback is invoked in an invalid request for verification is made,
            // for instance if the the phone number format is not valid.
        }
        public override void OnCodeSent(string verificationId, PhoneAuthProvider.ForceResendingToken forceResendingToken)
        {
            
            // The SMS verification code has been sent to the provided phone number, we
            // now need to ask the user to enter the code and then construct a credential
            // by combining the code with a verification ID.
            base.OnCodeSent(verificationId, forceResendingToken);
            IsCodeSent = true;
            VerificationId = verificationId;
            this._token = forceResendingToken;
            CodeSent?.Invoke(this, new EventArgs());
        }
   
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Trail_Tracker.Helpers;

namespace Trail_Tracker
{
    public class SignUpDialog : DialogFragment
    {
        private Button SignUpButton;
        private string url = null;
        private TextView Username, Pass, PassCheck, Email;

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SignUpDialog, container, false);

            url = Context.Resources.GetString(Resource.String.url);
            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.SignUpDialog, null);

            SignUpButton = view.FindViewById<Button>(Resource.Id.btnConfirmSignUp);
            SignUpButton.Click += SignUpAttempt;

            Email = view.FindViewById<EditText>(Resource.Id.emailtext);
            Username = view.FindViewById<EditText>(Resource.Id.usernametext);
            Pass = view.FindViewById<EditText>(Resource.Id.passwordtext);
            PassCheck = view.FindViewById<EditText>(Resource.Id.repeatpasswordtext);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        public void SignUpAttempt(object sender, System.EventArgs e)
        {
            //retrieves data from dialog box
            Android.Views.View view = this.View;
            string email = Email.Text;
            string username = Username.Text;
            string password = Pass.Text;
            string passCheck = Pass.Text;

            bool email_valid = (email != null && email != "");
            bool uname_valid = (username != null && username != "");
            bool pass_valid = (password != null && password != "");
            bool all_valid = (email_valid && uname_valid && pass_valid);

            //create an alert box to show data that was entered (For testing)
            if (all_valid && password == passCheck)
            {
                User user = new User(username, password, email);
                DataAccess da = new DataAccess();

                if (da.Add_User(user))
                {
                    Toast.MakeText(this.Context, "Account Created!", ToastLength.Long).Show();
                    Intent mapIntent = new Intent(this.Activity, typeof(MapActivity));
                    mapIntent.PutExtra("User", username + "," + email);
                    this.Activity.StartActivity(mapIntent);
                    Dismiss();
                }
                else
                    Toast.MakeText(this.Context, "Error Creating Account! Try again", ToastLength.Long).Show();
            }
            else
            {
                InvalidInput();
            }
        }

        private void InvalidInput()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Signup Failed");
            alert.SetTitle("Cannot leave any field blank");
            alert.SetPositiveButton("Retry", (senderAlert, args) => { });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Dismiss();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}
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
using System.Data;

namespace Trail_Tracker
{
    public class LoginDialog : DialogFragment
    {
        Button btnLogin;
        TextView Username, Password;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.LoginDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.LoginDialog, null);

            btnLogin = view.FindViewById<Button>(Resource.Id.buttonlogin);
            btnLogin.Click += BtnLogin_Click;

            Username = view.FindViewById<EditText>(Resource.Id.signinusername);
            Password = view.FindViewById<EditText>(Resource.Id.signinpassword);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (Username.Text != "" && Password.Text != "")
            {
                DataAccess da = new DataAccess();
                DataTable dt = new DataTable();

                dt = da.Login(Username.Text, Password.Text);

                if(dt.Rows.Count == 1)
                {
                    User user = new User(dt.Rows[0].ItemArray);
                    Toast.MakeText(this.Context, "Login Successful!", ToastLength.Long).Show();
                    Intent mapIntent = new Intent(this.Activity, typeof(MapActivity));
                    mapIntent.PutExtra("User", user.m_username + "," + user.m_email);
                    this.Activity.StartActivity(mapIntent);
                }
            }
            
        }
    }
}
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

namespace Trail_Tracker
{
    [Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/TrailTrackerIcon")]
    //[Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        Button btnLogin, btnSignUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            btnLogin = FindViewById<Button>(Resource.Id.btnSignIn);
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);

            btnLogin.Click += BtnLogin_Click;
            btnSignUp.Click += BtnSignUp_Click;
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SignUpDialog dialogFragment = new SignUpDialog();
            dialogFragment.Show(transaction, "SignUp_Dialog");
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
        }
    }
}
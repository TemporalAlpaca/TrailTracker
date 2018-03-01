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
    public class CheckPasswordDialog : DialogFragment
    {
        private int m_userID;
        private string m_username, m_email;
        Button btnCheckPasswordDialogConfirm, btnCheckPasswordDialogCancel;
        EditText txtboxCheckPassword;

        public CheckPasswordDialog(int userID, string username, string email)
        {
            m_userID = userID;
            m_username = username;
            m_email = email;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.CheckPasswordDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.CheckPasswordDialog, null);

            btnCheckPasswordDialogConfirm = view.FindViewById<Button>(Resource.Id.btnCheckPasswordDialogConfirm);
            btnCheckPasswordDialogConfirm.Click += BtnCheckPasswordDialogConfirm_Click;

            btnCheckPasswordDialogCancel = view.FindViewById<Button>(Resource.Id.btnCheckPasswordDialogCancel);
            btnCheckPasswordDialogCancel.Click += BtnCheckPasswordDialogCancel_Click;

            txtboxCheckPassword = view.FindViewById<EditText>(Resource.Id.txtboxCheckPassword);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnCheckPasswordDialogCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BtnCheckPasswordDialogConfirm_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();

            if (txtboxCheckPassword.Text != "")
            {
                if (da.Check_Pass(m_userID, txtboxCheckPassword.Text))
                {
                    User user = new User(m_userID, m_username, txtboxCheckPassword.Text, m_email);
                    Toast.MakeText(this.Context, "Password accepted!", ToastLength.Long).Show();

                    Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    SettingsDialog dialogFragment = new SettingsDialog(user);

                    dialogFragment.Show(transaction, "Settings_Dialog");
                    Dismiss();
                }
                else
                {
                    Toast.MakeText(this.Context, "Incorrect password!", ToastLength.Long).Show();
                }
            }
            else
                Toast.MakeText(this.Context, "Please enter a password.", ToastLength.Long).Show();

        }
    }
}
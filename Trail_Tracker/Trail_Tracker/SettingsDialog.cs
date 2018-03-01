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
    public class SettingsDialog : DialogFragment
    {
        private User m_user;
        Button btnUpdateUsername, btnUpdateEmail, btnUpdatePassword, btnSettingsDialogCancel;
        EditText txtboxUpdateUsername, txtboxUpdateEmail, txtboxUpdatePassword;
        DataAccess da;

        public SettingsDialog(User user)
        {
            m_user = user;
            da = new DataAccess();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.SettingsDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.SettingsDialog, null);

            btnUpdateUsername = view.FindViewById<Button>(Resource.Id.btnUpdateUsername);
            btnUpdateEmail = view.FindViewById<Button>(Resource.Id.btnUpdateEmail);
            btnUpdatePassword = view.FindViewById<Button>(Resource.Id.btnUpdatePassword);
            btnSettingsDialogCancel = view.FindViewById<Button>(Resource.Id.btnSettingsDialogCancel);

            btnUpdateUsername.Click += BtnUpdateUsername_Click;
            btnUpdateEmail.Click += BtnUpdateEmail_Click;
            btnUpdatePassword.Click += BtnUpdatePassword_Click;
            btnSettingsDialogCancel.Click += BtnSettingsDialogCancel_Click;

            txtboxUpdateUsername = view.FindViewById<EditText>(Resource.Id.txtboxUpdateUsername);
            txtboxUpdateEmail = view.FindViewById<EditText>(Resource.Id.txtboxUpdateEmail);
            txtboxUpdatePassword = view.FindViewById<EditText>(Resource.Id.txtboxUpdatePassword);

            txtboxUpdateUsername.Text = m_user.m_username;
            txtboxUpdateEmail.Text = m_user.m_email;

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnSettingsDialogCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void BtnUpdatePassword_Click(object sender, EventArgs e)
        {
            if (txtboxUpdatePassword.Text.Trim() != "")
            {
                if (da.Update_Password(m_user, txtboxUpdatePassword.Text))
                {
                    m_user.m_password = txtboxUpdatePassword.Text;
                    Toast.MakeText(this.Context, "Password updated!", ToastLength.Long).Show();
                }
                else
                    Toast.MakeText(this.Context, "Password update failed!", ToastLength.Long).Show();
            }
        }

        private void BtnUpdateEmail_Click(object sender, EventArgs e)
        {
            if (txtboxUpdateEmail.Text.Trim() != "")
            {
                if (da.Update_Email(m_user, txtboxUpdateEmail.Text))
                {
                    m_user.m_email = txtboxUpdateEmail.Text;
                    Toast.MakeText(this.Context, "Email updated!", ToastLength.Long).Show();
                }
                else
                    Toast.MakeText(this.Context, "Email update failed!", ToastLength.Long).Show();
            }
        }

        private void BtnUpdateUsername_Click(object sender, EventArgs e)
        {
            if (txtboxUpdateUsername.Text.Trim() != "")
            {
                if (da.Update_Username(m_user, txtboxUpdateUsername.Text))
                {
                    m_user.m_username = txtboxUpdateUsername.Text;
                    Toast.MakeText(this.Context, "Username updated!", ToastLength.Long).Show();
                }
                else
                    Toast.MakeText(this.Context, "Username update failed!", ToastLength.Long).Show();
            }
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((MapActivity)activity).UpdateUserInfo(m_user);
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}
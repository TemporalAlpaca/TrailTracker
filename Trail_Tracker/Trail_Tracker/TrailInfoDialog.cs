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
using Android;

namespace Trail_Tracker
{
    public class TrailInfoDialog : DialogFragment
    {
        private string m_trailname, m_length, m_username;
        private TextView txtTrailName, txtTrailLength, txtTrailUsername;

        public TrailInfoDialog(string trailname, string length, string username)
        {
            m_trailname = trailname;
            m_length = length;
            m_username = username;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.TrailInfoDialog, container, false);

            txtTrailName = view.FindViewById<TextView>(Resource.Id.txtTrailName);
            txtTrailUsername = view.FindViewById<TextView>(Resource.Id.txtTrailUsername);
            txtTrailLength = view.FindViewById<TextView>(Resource.Id.txtTrailLength);

            txtTrailName.Text = m_trailname;
            txtTrailUsername.Text = "Uploaded by: " + m_username;
            txtTrailLength.Text = "Length: " + m_length + " miles";

            return view;
        }
    }
}
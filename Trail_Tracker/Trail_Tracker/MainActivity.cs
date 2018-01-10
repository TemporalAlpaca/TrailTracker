using Android.App;
using Android.Widget;
using Android.OS;
using Trail_Tracker;
using Android.Locations;
using Android.Content;
using Android.Util;
using Android.Runtime;
using System;

namespace TrailTracker
{
    [Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        Button btnStartTracking;
        Button btnStopTracking;
        TextView txtLatitude;
        TextView txtLongitude;
        LocationManager locMgr;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            //Creates instance of Start Tracking button
            btnStartTracking = FindViewById<Button>(Resource.Id.btnStartTracking);
            btnStartTracking.Click += BtnStartTracking_Click;

            //Creates instance of Sign in button
            btnStopTracking = FindViewById<Button>(Resource.Id.btnStopTracking);
            btnStopTracking.Click += BtnStopTracking_Click;

            txtLatitude = FindViewById<TextView>(Resource.Id.txtLatitude);
            txtLongitude = FindViewById<TextView>(Resource.Id.txtLongitude);
        }

        private void BtnStopTracking_Click(object sender, System.EventArgs e)
        {
         
        }

        private void BtnStartTracking_Click(object sender, System.EventArgs e)
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            string Provider = LocationManager.GpsProvider;

            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            txtLatitude.Text = loc.Latitude.ToString();
            txtLongitude.Text = loc.Longitude.ToString();
            if (locMgr.IsProviderEnabled(Provider))
            {
                //look at recording movements
                //locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 1000, 100, this);
                //locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
            }
            else
            {
                Log.Info("GPS Start Error", Provider + " is not available. Does the device have location services enabled?");
            }
        }

        public void OnLocationChanged(Location location)
        {
            txtLatitude.Text = "Latitude: " + location.Latitude;
            txtLongitude.Text = "Longitude: " + location.Longitude;
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

        protected override void OnPause()
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);
        }
    }
}


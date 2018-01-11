using Android.App;
using Android.Widget;
using Android.OS;
using Trail_Tracker;
using Android.Locations;
using Android.Content;
using Android.Util;
using Android.Runtime;
using System;
using System.Collections.Generic;

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

        Tuple<double, double> startCoord;
        Tuple<double, double> endCoord;

        List<Tuple<double, double>> path = new List<Tuple<double, double>>();

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
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            string Provider = LocationManager.GpsProvider;

            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            txtLatitude.Text = loc.Latitude.ToString();
            txtLongitude.Text = loc.Longitude.ToString();

            endCoord = new Tuple<double, double>(loc.Latitude, loc.Longitude);
            locMgr.RemoveUpdates(this);

            string displayCoords = "Start Position:\nLatitude: " + startCoord.Item1.ToString();
            displayCoords += "\nLongitude: ";
            displayCoords += startCoord.Item2.ToString();
            displayCoords += "\n";

            foreach (Tuple<double, double> coord in path)
            {
                displayCoords += "\nLatitude: ";
                displayCoords += coord.Item1.ToString();
                displayCoords += "\nLongitude: ";
                displayCoords += coord.Item2.ToString();
                displayCoords += "\n";
            }
            displayCoords += "\nEnd Position:";
            displayCoords += "\nLatitude: ";
            displayCoords += endCoord.Item1.ToString();
            displayCoords += "\nLongitude: ";
            displayCoords += endCoord.Item2.ToString();

            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("GPS Coordinates");
            alert.SetMessage(displayCoords);

            alert.SetPositiveButton("Confirm", (senderAlert, args) => {
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            });

            Dialog dialog = alert.Create();
            dialog.Show();
            //Toast.MakeText(this, displayCoords, ToastLength.Long).Show();
        }

        private void BtnStartTracking_Click(object sender, System.EventArgs e)
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            string Provider = LocationManager.GpsProvider;

            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            txtLatitude.Text = "Latitude: " + loc.Latitude.ToString();
            txtLongitude.Text = "Longitude: " + loc.Longitude.ToString();

            startCoord = new Tuple<double, double>(loc.Latitude, loc.Longitude);

            try
            {
                locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
                if (path != null)
                    path.Clear();
            }
            catch(Exception)
            {}

        }

        protected override void OnResume()
        {
        }

        public void OnLocationChanged(Location location)
        {
            txtLatitude.Text = "Latitude: " + location.Latitude;
            txtLongitude.Text = "Longitude: " + location.Longitude;

            path.Add(new Tuple<double, double>(location.Latitude, location.Longitude));
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


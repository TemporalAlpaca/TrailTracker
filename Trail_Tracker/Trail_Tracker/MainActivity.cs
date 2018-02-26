using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Content;
using Android.Util;
using Android.Runtime;
using System;
using System.Collections.Generic;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Trail_Tracker.Helpers;

namespace Trail_Tracker
{
    //[Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/trailtrackericon")]
    [Activity(Label = "TrackingActivity")]
    public class MainActivity : BaseActivity, ILocationListener
    {
        Button btnTracking;
  
        TextView txtLatitude;
        TextView txtLongitude;
        TextView txtDistance;
        LocationManager locMgr;

        Location startCoord;
        Location endCoord;
        Location prevCoord;

        List<Location> path = new List<Location>();
        float distance = 0;
        string m_username;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Tracking);

            //Creates instance of Start Tracking button
            btnTracking = FindViewById<Button>(Resource.Id.btnTracking);
            btnTracking.Click += BtnStartTracking_Click;

            txtLatitude = FindViewById<TextView>(Resource.Id.txtLatitude);
            txtLongitude = FindViewById<TextView>(Resource.Id.txtLongitude);
            txtDistance = FindViewById<TextView>(Resource.Id.txtDistance);

            m_username = this.Intent.GetStringExtra("User");
        }

        private void BtnStopTracking_Click(object sender, System.EventArgs e)
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;

            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            //Final update for onscreen lat and long
            txtLatitude.Text = loc.Latitude.ToString();
            txtLongitude.Text = loc.Longitude.ToString();

            endCoord = new Location(loc);
            
            //stop tracking location
            locMgr.RemoveUpdates(this);

            //create dialog box to name trail
            InsertTrail(startCoord.Latitude.ToString() + "," + startCoord.Longitude.ToString(),
                endCoord.Latitude.ToString() + "," + endCoord.Longitude.ToString(), path);
        }

        private void InsertTrail(string start, string end, List<Location> path)
        {
            //Handle translating path values
            string coordinates = "";

            foreach (Location coord in path)
            {
                if (coordinates != "")
                    coordinates += ";";

                coordinates += coord.Latitude.ToString();
                coordinates += ",";
                coordinates += coord.Longitude.ToString();
            }

            //Create new dialog for trail submission
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();

            //length = CalcDistance();

            TrailSubmitDialog dialogFragment = new TrailSubmitDialog(distance, startCoord.Latitude.ToString() + "," + startCoord.Longitude.ToString(),
                endCoord.Latitude.ToString() + "," + endCoord.Longitude.ToString(), coordinates, m_username);

            dialogFragment.Show(transaction, "TrailSubmit_Dialog");
        }

        private void BtnStartTracking_Click(object sender, System.EventArgs e)
        {
            distance = 0;
            prevCoord = null;
            //check permissions before tracking
            string permission = Manifest.Permission.AccessFineLocation;
            if (this.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                string Provider = LocationManager.GpsProvider;
                Location loc;
                try
                {
                    //get start location
                    loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);
                    txtLatitude.Text = "Latitude: " + loc.Latitude.ToString();
                    txtLongitude.Text = "Longitude: " + loc.Longitude.ToString();

                    startCoord = new Location(loc);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }

                try
                {
                    //Poll every 2000ms, when distance has changed more than 1 meter
                    locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
                    if (path != null)
                        path.Clear();
                }
                catch (Exception)
                { }
            }
            else
            {
                string[] request_permissions = new string[1];
                request_permissions[0] = Manifest.Permission.AccessFineLocation;
                ActivityCompat.RequestPermissions(this, request_permissions, 0);
            }

            //Change button functionality
            btnTracking.Text = "Stop Tracking";
            btnTracking.Click -= BtnStartTracking_Click;
            btnTracking.Click += BtnStopTracking_Click;

        }

        public void OnLocationChanged(Location location)
        {
            txtLatitude.Text = "Latitude: " + location.Latitude;
            txtLongitude.Text = "Longitude: " + location.Longitude;

            path.Add(new Location(location));

            if(prevCoord != null)
            {
                distance += prevCoord.DistanceTo(location);

                //convert from meters to miles
                distance = distance / (float)1609.344;
                if (distance > 0.01)
                    txtDistance.Text = distance.ToString().Substring(0, 4) + " miles";
                else
                    txtDistance.Text = "0 miles";
            }
            prevCoord = location;
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

        protected float CalcDistance()
        {
            float distance = 0;
            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    //DistanceTo returns value in meters
                    distance += path[i].DistanceTo(path[i + 1]);
                }
                distance += path[path.Count - 1].DistanceTo(endCoord);

                //convert meters to miles
                return distance / (float)1609.344;
            }
            else
                return 0;
        }
    }
}


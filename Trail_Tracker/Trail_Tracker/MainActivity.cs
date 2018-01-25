﻿using Android.App;
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
    [Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/trailtrackericon")]
    //[Activity(Label = "TrailTracker")]
    public class MainActivity : BaseActivity, ILocationListener
    {
        Button btnStartTracking;
        Button btnStopTracking;
        Button btnTestMap;
  
        TextView txtLatitude;
        TextView txtLongitude;
        LocationManager locMgr;

        Location startCoord;
        Location endCoord;

        List<Location> path = new List<Location>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            InitializeToolbar();

            //Creates instance of Start Tracking button
            btnStartTracking = FindViewById<Button>(Resource.Id.btnStartTracking);
            btnStartTracking.Click += BtnStartTracking_Click;

            //Creates instance of Sign in button
            btnStopTracking = FindViewById<Button>(Resource.Id.btnStopTracking);
            btnStopTracking.Click += BtnStopTracking_Click;

            btnTestMap = FindViewById<Button>(Resource.Id.btnTestMap);
            btnTestMap.Click += BtnTestMap_Click;

            txtLatitude = FindViewById<TextView>(Resource.Id.txtLatitude);
            txtLongitude = FindViewById<TextView>(Resource.Id.txtLongitude);
        }

        private void BtnTestMap_Click(object sender, EventArgs e)
        {
            this.StartActivity(typeof(MapActivity));
        }

        private void BtnStopTracking_Click(object sender, System.EventArgs e)
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            float length = 0;

            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            txtLatitude.Text = loc.Latitude.ToString();
            txtLongitude.Text = loc.Longitude.ToString();

            endCoord = new Location(loc);
            
            locMgr.RemoveUpdates(this);

            string displayCoords = "Start Position:\nLatitude: " + startCoord.Latitude.ToString();
            displayCoords += "\nLongitude: ";
            displayCoords += startCoord.Longitude.ToString();
            displayCoords += "\n";

            foreach (Location coord in path)
            {
                displayCoords += "\nLatitude: ";
                displayCoords += coord.Latitude.ToString();
                displayCoords += "\nLongitude: ";
                displayCoords += coord.Longitude.ToString();
                displayCoords += "\n";
            }
            displayCoords += "\nEnd Position:";
            displayCoords += "\nLatitude: ";
            displayCoords += endCoord.Latitude.ToString();
            displayCoords += "\nLongitude: ";
            displayCoords += endCoord.Longitude.ToString();

            length = CalcDistance();
            displayCoords += "\n\nTotal Distance: " + length.ToString() + " miles";

            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("GPS Coordinates");
            alert.SetMessage(displayCoords);

            alert.SetPositiveButton("Confirm", (senderAlert, args) => {
                InsertTrail("Sample", length, startCoord.Latitude.ToString() + "," + startCoord.Longitude.ToString(),
                    endCoord.Latitude.ToString() + "," + endCoord.Longitude.ToString(), path, "Caleb");
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void InsertTrail(string name, float length, string start, string end, List<Location> path, string username)
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

            DataAccess dataAccess = new DataAccess();
            dataAccess.Insert_Trail(name, length, start, end, coordinates, username);
        }

        private void BtnStartTracking_Click(object sender, System.EventArgs e)
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (this.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                string Provider = LocationManager.GpsProvider;
                Location loc;
                try
                {
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
        }

        public void OnLocationChanged(Location location)
        {
            txtLatitude.Text = "Latitude: " + location.Latitude;
            txtLongitude.Text = "Longitude: " + location.Longitude;

            path.Add(new Location(location));
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


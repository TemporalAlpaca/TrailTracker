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
using Android.Support.V4.App;
using Android.Gms.Location;
using Android.Gms;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android;
using Android.Content.PM;
using Trail_Tracker.Helpers;
using System.Data;

namespace Trail_Tracker
{
    //[Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/icon")]
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        MapFragment _mapFragment;
        GoogleMap _map;

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            SetCamera();
            LoadTrails();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.Map);
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }

            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeTerrain)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);


                Android.App.FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);
        }

        private void SetCamera()
        {
            LatLng location = GetLatLng();
            if (location != null)
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(15);
                builder.Bearing(155);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                if (_map != null)
                {
                    _map.MoveCamera(cameraUpdate);
                }
            }
        }

        private void LoadTrails()
        {
            DataAccess da = new DataAccess();
            DataTable dt = da.Search_Trail("Sample", 0, "", "");

            foreach(DataRow row in dt.Rows)
            {
                if (_map != null)
                {
                    //Separate latitude and longitude
                    string[] startCoord = row.ItemArray[3].ToString().Split(',');
                    double startLat = -1;
                    double startLng = -1;
                    try
                    {
                        startLat = double.Parse(startCoord[0]);
                        startLng = double.Parse(startCoord[1]);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Failed to parse start coordinates for a trail: MapActivity line 105");
                        Console.WriteLine(ex.ToString());
                    }

                    if (startLat != -1 && startLng != -1)
                    {
                        LatLngBounds bounds = _map.Projection.VisibleRegion.LatLngBounds;
                        LatLng trailhead = new LatLng(startLat, startLng);

                        //Load trails if they are in bounds of the zoom level
                        if (bounds.Contains(trailhead))
                        {
                            MarkerOptions markerOpt1 = new MarkerOptions();
                            markerOpt1.SetPosition(trailhead);
                            //Set title to the trail's name
                            markerOpt1.SetTitle("Name: " + row.ItemArray[1].ToString() + 
                                " Length: " + row.ItemArray[2].ToString().Substring(0,4) + " miles");
                            markerOpt1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                            _map.AddMarker(markerOpt1);
                        }
                    }
                }
            }
        }

        private LatLng GetLatLng()
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (this.CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                LocationManager locMgr = GetSystemService(Context.LocationService) as LocationManager;
                string Provider = LocationManager.GpsProvider;
                Location loc;

                try
                {
                    loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

                    return new LatLng(loc.Latitude, loc.Longitude);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    return null;
                }
            }
            else
                return null;
        }
    }
}
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
using Android.Graphics;

namespace Trail_Tracker
{
    [Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/TrailTrackerIcon")]
    //[Activity(Label = "")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        MapFragment _mapFragment;
        GoogleMap _map;
        Button btnAddTrail;

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            SetCamera();
            LoadTrails();

            _map.CameraChange += _map_CameraMove;
        }

        private void _map_CameraMove(object sender, EventArgs e)
        {
            LoadTrails();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                CheckLocationPermissions();
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

            btnAddTrail = FindViewById<Button>(Resource.Id.btnAddTrail);
            btnAddTrail.Click += BtnAddTrail_Click;
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
            LocationManager locMgr = GetSystemService(Context.LocationService) as LocationManager;
            Location loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            string startlat = loc.Latitude.ToString().Substring(0, 4);
            string startLong = loc.Longitude.ToString().Substring(0, 4);

            DataAccess da = new DataAccess();
            DataTable dt;
            
            if(da != null)
            { 
                dt = da.Search_Trail("", 0, startlat, startLong, "");

                if(dt != null)
                { 
                foreach (DataRow row in dt.Rows)
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
                            catch (Exception ex)
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
                                    LoadMarkers(bounds, trailhead, row);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadMarkers(LatLngBounds bounds, LatLng trailhead, DataRow row)
        {
            MarkerOptions markerOpt1 = new MarkerOptions();
            markerOpt1.SetPosition(trailhead);
            //Set title to the trail's name
            markerOpt1.SetTitle("Name: " + row.ItemArray[1].ToString() +
                " Length: " + row.ItemArray[2].ToString().Substring(0, 4) + " miles");
            markerOpt1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            _map.AddMarker(markerOpt1);

            LoadTrailPath(row.ItemArray[3].ToString(), row.ItemArray[4].ToString(), row.ItemArray[5].ToString());
        }

        private void LoadTrailPath(string start, string end, string path)
        {
            string[] pathCoords = path.Split(';');
            string[] latlng = start.Split(',');

            double latitude = 0;
            double longitude = 0;

            PolylineOptions lineOptions = new PolylineOptions();

            latitude = double.Parse(latlng[0]);
            longitude = double.Parse(latlng[1]);
            lineOptions.Add(new LatLng(latitude, longitude));

            //Random rand = new Random();
            //Color randomColor = Color.Argb(rand.Next(256), rand.Next(256), rand.Next(256), rand.Next(256));
            //lineOptions.InvokeColor(randomColor);
            
            foreach(string coord in pathCoords)
            {
                try
                {
                    latlng = coord.Split(',');

                    if (latlng.Length > 0)
                    {
                        latitude = double.Parse(latlng[0]);
                        longitude = double.Parse(latlng[1]);

                        lineOptions.Add(new LatLng(latitude, longitude));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error translating path values: LoadTrailPath");
                    Console.WriteLine(ex.ToString());
                }
            }

            latlng = end.Split(',');
            latitude = double.Parse(latlng[0]);
            longitude = double.Parse(latlng[1]);
            lineOptions.Add(new LatLng(latitude, longitude));

            if (pathCoords.Length > 0)
                _map.AddPolyline(lineOptions);
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


        private void BtnAddTrail_Click(object sender, EventArgs e)
        {
            try
            {
                //Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                //TrailSubmitDialog dialogFragment = new TrailSubmitDialog();
                //dialogFragment.Show(transaction, "TrailSubmit_Dialog");

                this.StartActivity(typeof(MainActivity));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void CheckLocationPermissions()
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (this.CheckSelfPermission(permission) != (int)Permission.Granted)
            {            
                string[] request_permissions = new string[1];
                request_permissions[0] = Manifest.Permission.AccessFineLocation;
                ActivityCompat.RequestPermissions(this, request_permissions, 0);

            }
        }

    }
}
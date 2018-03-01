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
    //[Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/TrailTrackerIcon")]
    [Activity(Label = "TrailTracker")]
    public class MapActivity : Activity, IOnMapReadyCallback, IDialogInterfaceOnDismissListener, GoogleMap.IOnMarkerClickListener
    {
        MapFragment _mapFragment;
        GoogleMap _map;
        Button btnAddTrail, btnLiked, btnSearch, btnFriends, btnSettings;
        LocationManager locMgr;
        Location loc;
        LatLng latlng;
        List<Tuple<MarkerOptions, Trail>> TrailList;
        List<Polyline> PathList;
        private string m_username, m_email;
        private int m_userID;

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.InfoWindowClick += _map_InfoWindowClick;
            _map.SetOnMarkerClickListener(this);
            _map.MapClick += _map_MapClick;
            SetCamera();
            TrailList.Clear();
            LoadTrails();
            _map.CameraChange += _map_CameraMove;
        }

        private void _map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            foreach(Polyline path in PathList)
            {
                path.Color = Color.Black;
            }
        }

        private void _map_InfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            string[] dialogInfo = e.Marker.Title.ToString().Split(',');
            Trail trail = null;

            if (dialogInfo.Length > 2)
            {
                for(int i = 0; i < TrailList.Count; ++i)
                {
                    if (e.Marker.Title == TrailList[i].Item1.Title)
                        trail = TrailList[i].Item2;
                }

                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                TrailInfoDialog dialogFragment = new TrailInfoDialog(trail, m_username, m_userID);
                dialogFragment.Show(transaction, "TrailInfo_Dialog");
            }
        }

        private void _map_CameraMove(object sender, EventArgs e)
        {
            //TrailList.Clear();
            //PathList.Clear();
            LoadTrails();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActionBar.Hide();
            try
            {
                var userInfo = this.Intent.GetStringExtra("User").Split(',');
                m_username = userInfo[0];
                m_email = userInfo[1];

                TrailList = new List<Tuple<MarkerOptions, Trail>>();
                PathList = new List<Polyline>();
                CheckLocationPermissions();
                SetContentView(Resource.Layout.Map);
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                loc = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

                DataAccess da = new DataAccess();
                DataTable dt = new DataTable();

                dt = da.Find_User(m_username);

                if (dt.Rows.Count == 1)
                {
                    m_userID = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
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

            btnSearch = FindViewById<Button>(Resource.Id.btnSearchTrail);
            btnSearch.Click += BtnSearch_Click;

            btnLiked = FindViewById<Button>(Resource.Id.btnLikedTrails);
            btnLiked.Click += BtnLiked_Click;

            btnFriends = FindViewById<Button>(Resource.Id.btnFriends);
            btnFriends.Click += BtnFriends_Click;

            btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
            btnSettings.Click += BtnSettings_Click;
        }

        private void SetCamera()
        {
            latlng = GetLatLng();
            if (latlng != null)
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(latlng);
                builder.Zoom(15);
                builder.Bearing(0);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                if (_map != null)
                {
                    _map.MoveCamera(cameraUpdate);
                }
            }
        }

        public void LoadTrails()
        {
            latlng = _map.CameraPosition.Target;
            if (latlng.Latitude != 0 && latlng.Longitude != 0)
            {
                string startlat = latlng.Latitude.ToString().Substring(0, 4);
                string startlong = latlng.Longitude.ToString().Substring(0, 4);

                DataAccess da = new DataAccess();
                DataTable dt;

                if (da != null)
                {
                    dt = da.Load_Trail(startlat, startlong);

                    if (dt != null)
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
        }

        private void LoadMarkers(LatLngBounds bounds, LatLng trailhead, DataRow row)
        {
            bool exists = false;

            for(int i = 0; i < TrailList.Count; ++i)
            {
                if (TrailList[i].Item1.Position.Latitude == trailhead.Latitude &&
                  TrailList[i].Item1.Position.Longitude == trailhead.Longitude)
                    exists = true;
            }
            if (!exists)
            {
                MarkerOptions markerOpt1 = new MarkerOptions();
                float length = (float)(0.00);

                try
                {
                    length = float.Parse(row.ItemArray[2].ToString());
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error parsing trail length LoadMarkers()");
                }

                markerOpt1.SetPosition(trailhead);
                //Set title to the trail's name
                markerOpt1.SetTitle(row.ItemArray[1].ToString() +
                    "," + length.ToString("F2") + "," + row.ItemArray[6].ToString());
                markerOpt1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

                _map.AddMarker(markerOpt1);

                //Add trail and trail ID to a list
                try
                {
                    TrailList.Add(new Tuple<MarkerOptions, Trail>(markerOpt1, new Trail(row.ItemArray)));
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing trail ID LoadMarkers");
                }

                LoadTrailPath(row.ItemArray[3].ToString(), row.ItemArray[4].ToString(), row.ItemArray[5].ToString());
            }
        }

        private void LoadTrailPath(string start, string end, string path)
        {
            string[] pathCoords = path.Split(';');
            string[] latlng = start.Split(',');

            double latitude = 0;
            double longitude = 0;

            PolylineOptions lineOptions = new PolylineOptions();
            lineOptions.Clickable(true);

            latitude = double.Parse(latlng[0]);
            longitude = double.Parse(latlng[1]);
            lineOptions.Add(new LatLng(latitude, longitude));
            
            //lineOptions.InvokeColor(colorList[currentColor]);
            
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
                PathList.Add(_map.AddPolyline(lineOptions));
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


        private void BtnFriends_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            FriendsDialog dialogFragment = new FriendsDialog(m_userID);

            dialogFragment.Show(transaction, "Friends_Dialog");
        }

        private void BtnAddTrail_Click(object sender, EventArgs e)
        {
            try
            {
                Intent trackingIntent = new Intent(this, typeof(MainActivity));
                trackingIntent.PutExtra("User", m_username);
                StartActivity(trackingIntent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            //Create new dialog for trail search
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            TrailSearchDialog dialogFragment = new TrailSearchDialog();

            dialogFragment.Show(transaction, "TrailSubmit_Dialog");
        }

        private void BtnLiked_Click(object sender, EventArgs e)
        {
            //Create new dialog for trail search
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            LikedTrailsDialog dialogFragment = new LikedTrailsDialog(m_userID);

            dialogFragment.Show(transaction, "LikedTrails_Dialog");
        }


        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            CheckPasswordDialog dialogFragment = new CheckPasswordDialog(m_userID, m_username, m_email);

            dialogFragment.Show(transaction, "Settings_Dialog");
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

        public void OnDismiss(IDialogInterface dialog)
        {
            TrailList.Clear();
            PathList.Clear();
            _map.Clear();
            LoadTrails();
        }

        public void GetCoord(string coord)
        {
            //Use this function to set new target.
            //Called after a user selects a trail from the search list.
            if (coord != null && coord.Trim() != "")
            {
                string[] startcoords = coord.Split(',');
                try
                {
                    latlng.Latitude = float.Parse(startcoords[0]);
                    latlng.Longitude = float.Parse(startcoords[1]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing return values from Search Dialog");
                }

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(latlng);
                builder.Zoom(15);
                builder.Bearing(0);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                if (_map != null)
                {
                    _map.MoveCamera(cameraUpdate);
                }
            }
        }

        public void UpdateUserInfo(User user)
        {
            m_username = user.m_username;
            m_email = user.m_email;
        }

        public bool OnMarkerClick(Marker marker)
        {
            foreach (Polyline path in PathList)
            {
                path.Color = Color.Black;
            }

            for (int i = 0; i < PathList.Count; ++i)
            {
                if (marker.Position.Latitude == PathList[i].Points[0].Latitude && 
                    marker.Position.Longitude == PathList[i].Points[0].Longitude)
                    PathList[i].Color = Color.Blue;
            }
            return false;
        }
    }
}
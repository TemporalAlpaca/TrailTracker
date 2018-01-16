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

namespace Trail_Tracker
{
    //[Activity(Label = "TrailTracker", MainLauncher = true, Icon = "@drawable/icon")]
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView (Resource.Layout.Map);

        }
    }
}
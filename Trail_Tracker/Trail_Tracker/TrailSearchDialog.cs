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
using System.Data;

namespace Trail_Tracker
{
    public class TrailSearchDialog : DialogFragment
    {
        Button btnSearch, btnCancel;
        EditText txtboxTrailSearch;
        ListView lvTrailSearchDialog;
        List<Trail> Trails;
        public string coord;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.TrailSearchDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.TrailSearchDialog, null);

            btnSearch = view.FindViewById<Button>(Resource.Id.btnTrailSearchDialogConfirm);
            btnSearch.Click += BtnSearch_Click;

            btnCancel = view.FindViewById<Button>(Resource.Id.btnTrailSearchDialogCancel);
            btnCancel.Click += BtnCancel_Click;

            txtboxTrailSearch = view.FindViewById<EditText>(Resource.Id.txtboxTrailSearch);
            lvTrailSearchDialog = view.FindViewById<ListView>(Resource.Id.lvTrailSearchDialog);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string trailname = "";
            string username = "";
            Trails = new List<Trail>();

            trailname = txtboxTrailSearch.Text;

            if(trailname.Trim() != "" || username.Trim() != "")
            {
                DataAccess da = new DataAccess();
                DataTable dt = new DataTable();

                dt = da.Search_Trail(trailname, username);

                for(int i = 0; i < dt.Rows.Count; ++i)
                {
                    //1 is trail name, 6 is username
                    Trail trail = new Trail(dt.Rows[i].ItemArray);
                    Trails.Add(trail);
                }
                if (Trails.Count != 0)
                {
                    lvTrailSearchDialog.Adapter = new SearchListAdapter(this.Activity, Trails);
                    lvTrailSearchDialog.ItemClick += LvTrailSearchDialog_ItemClick;
                }
                else
                {
                    List<Trail> list = new List<Trail>();
                    list.Add(new Trail("No results found :("));
                    lvTrailSearchDialog.Adapter = new SearchListAdapter(this.Activity, list);
                }
            }
            
        }

        private void LvTrailSearchDialog_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            coord = Trails[e.Position].m_start;
            Dismiss();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((MapActivity)activity).GetCoord(coord);
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}
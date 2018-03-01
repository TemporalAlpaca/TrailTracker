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
    public class LikedTrailsDialog : DialogFragment
    {
        int m_userID;
        string m_username;
        Button btnCancel;
        ListView lvLikedTrails;
        TextView txtLikedTrailsDialogHeader;
        List<Trail> Trails;
        public string coord;

        public LikedTrailsDialog(int userID)
        {
            m_userID = userID;
            m_username = "";
        }

        public LikedTrailsDialog(int userID, string username)
        {
            m_userID = userID;
            m_username = username;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.LikedTrailsDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.LikedTrailsDialog, null);

            txtLikedTrailsDialogHeader = view.FindViewById<TextView>(Resource.Id.txtLikedTrailsDialogHeader);

            lvLikedTrails = view.FindViewById<ListView>(Resource.Id.lvLikedTrails);
            FillFavorites();

            btnCancel = view.FindViewById<Button>(Resource.Id.btnLikedTrailsDialogCancel);
            btnCancel.Click += BtnCancel_Click;

            if (m_username != "")
                txtLikedTrailsDialogHeader.Text = m_username + "'s liked trails:";

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void FillFavorites()
        {
            DataAccess da = new DataAccess();
            DataTable dt = new DataTable();
            Trails = new List<Trail>();

            dt = da.Get_Liked_Trails(m_userID);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                //1 is trail name, 6 is username
                Trail trail = new Trail(dt.Rows[i].ItemArray);
                Trails.Add(trail);
            }
            if (Trails.Count != 0)
            {
                lvLikedTrails.Adapter = new SearchListAdapter(this.Activity, Trails);
                lvLikedTrails.ItemClick += LvLikedTrails_ItemClick;
            }
            else
            {
                txtLikedTrailsDialogHeader.Text = "No liked trails :(";
            }
        }

        private void LvLikedTrails_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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
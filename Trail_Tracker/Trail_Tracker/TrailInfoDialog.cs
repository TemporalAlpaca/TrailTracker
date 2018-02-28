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
using Android;
using Android.Graphics;
using Trail_Tracker.Helpers;
using System.Data;

namespace Trail_Tracker
{
    public class TrailInfoDialog : DialogFragment
    {
        private string m_trailname, m_length, m_username, m_current_user;
        private TextView txtTrailName, txtTrailLength, txtTrailUsername, txtTrailLikes, txtTrailDislikes;
        private Button btnReport, btnLike, btnDislike;
        private int m_trailID, m_likes, m_dislikes, m_userID;
        private bool m_rated = false;

        public TrailInfoDialog(Trail trail, string current_user, int userID)
        {
            m_trailname = trail.m_trailname;
            m_length = trail.m_length.ToString("F2");
            m_username = trail.m_username;
            m_trailID = trail.m_id;
            m_likes = trail.m_likes;
            m_dislikes = trail.m_dislikes;
            m_current_user = current_user;
            m_userID = userID;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.TrailInfoDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.TrailInfoDialog, null);

            txtTrailName = view.FindViewById<TextView>(Resource.Id.txtTrailName);
            txtTrailUsername = view.FindViewById<TextView>(Resource.Id.txtTrailUsername);
            txtTrailLength = view.FindViewById<TextView>(Resource.Id.txtTrailLength);
            txtTrailLikes = view.FindViewById<TextView>(Resource.Id.txtTrailLikes);
            txtTrailDislikes = view.FindViewById<TextView>(Resource.Id.txtTrailDislikes);

            txtTrailName.Text = m_trailname;
            txtTrailUsername.Text = "Uploaded by: " + m_username;
            txtTrailLength.Text = "Length: " + m_length + " miles";
            txtTrailLikes.Text = m_likes.ToString();
            txtTrailDislikes.Text = m_dislikes.ToString();

            btnDislike = view.FindViewById<Button>(Resource.Id.btnTrailInfoDislike);
            btnDislike.Background.SetColorFilter(Color.Red, PorterDuff.Mode.Multiply);
            btnDislike.Click += BtnDislike_Click;

            btnLike = view.FindViewById<Button>(Resource.Id.btnTrailInfoLike);
            btnLike.Background.SetColorFilter(Color.Green, PorterDuff.Mode.Multiply);
            btnLike.Click += BtnLike_Click;

            btnReport = view.FindViewById<Button>(Resource.Id.btnTrailInfoReport);
            btnReport.Background.SetColorFilter(Color.DarkRed, PorterDuff.Mode.Multiply);
            btnReport.Click += BtnReport_Click;

            DataAccess da = new DataAccess();

            if(da.Check_Rate(m_trailID, m_userID).Rows.Count != 0)
            {
                btnLike.Enabled = false;
                btnDislike.Enabled = false;
            }

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
        }

        private void BtnLike_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            da.Rate_Trail(m_trailID, 1, m_userID);
            btnDislike.Enabled = false;
            btnLike.Enabled = false;

            int likes = int.Parse(txtTrailLikes.Text);
            txtTrailLikes.Text = (++likes).ToString();
        }

        private void BtnDislike_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            da.Rate_Trail(m_trailID, 0, m_userID);
            btnLike.Enabled = false;
            btnDislike.Enabled = false;

            int dislikes = int.Parse(txtTrailDislikes.Text);
            txtTrailDislikes.Text = (++dislikes).ToString();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity activity = this.Activity;
            ((MapActivity)activity).LoadTrails();
            ((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}
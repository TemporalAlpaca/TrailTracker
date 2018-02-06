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
using Trail_Tracker.Helpers;

namespace Trail_Tracker
{
    public class TrailSubmitDialog : DialogFragment
    {
        private float m_length;
        private string m_start, m_end, m_path, m_username, m_trailname;

        private TextView txtTrailSubmitDialogLength;
        private EditText txtboxTrailName;
        private Button btnTrailSubmitDialogCancel, btnTrailSubmitDialogConfirm;

        public TrailSubmitDialog(float length, string start, string end, string path, string username)
        {
            m_length = length;
            m_start = start;
            m_end = end;
            m_path = path;
            m_username = username;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.TrailSubmitDialog, container, false);

            txtTrailSubmitDialogLength = view.FindViewById<TextView>(Resource.Id.txtTrailSubmitDialogLength);
            txtTrailSubmitDialogLength.Text = "Length: " + m_length.ToString() + " miles";

            txtboxTrailName = view.FindViewById<EditText>(Resource.Id.txtboxTrailName);

            btnTrailSubmitDialogCancel = view.FindViewById<Button>(Resource.Id.btnTrailSubmitDialogCancel);

            btnTrailSubmitDialogConfirm = view.FindViewById<Button>(Resource.Id.btnTrailSubmitDialogConfirm);
            btnTrailSubmitDialogConfirm.Click += BtnTrailSubmitDialogConfirm_Click;

            return view;
        }

        private void BtnTrailSubmitDialogConfirm_Click(object sender, EventArgs e)
        {
            //Confirm that trail name is not spaces or empty
            if (txtboxTrailName.Text.Trim() != "")
            {
                if (m_length != 0)
                {
                    DataAccess da = new DataAccess();
                    da.Insert_Trail(txtboxTrailName.Text, m_length, m_start, m_end, m_path, "Caleb");
                }
                else
                {
                    Toast lenToast = Toast.MakeText(Context, "Trail has no length. Please map a longer trail", ToastLength.Long);
                    lenToast.Show();
                }
            }
            else
            {
                Toast nameToast = Toast.MakeText(Context, "You must enter a trail name.", ToastLength.Long);
                nameToast.Show();
            }
        }
    }
}
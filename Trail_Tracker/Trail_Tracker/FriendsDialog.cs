﻿using System;
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
    public class FriendsDialog : DialogFragment
    {
        int m_userID;
        ListView lvFriends;
        TextView txtFriends;
        List<User> Users;
        Button btnAddFriends;

        public FriendsDialog(int userID)
        {
            m_userID = userID;
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
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.FriendsDialog, null);

            btnAddFriends = view.FindViewById<Button>(Resource.Id.btnAddFriends);
            btnAddFriends.Click += BtnAddFriends_Click;

            txtFriends = view.FindViewById<TextView>(Resource.Id.txtFriendsDialogHeader);
            lvFriends = view.FindViewById<ListView>(Resource.Id.lvFriends);
            FillFriends();

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            builder.SetView(view);
            return builder.Create();
        }

        private void BtnAddFriends_Click(object sender, EventArgs e)
        {
            //Create new dialog for user search
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            UserSearchDialog dialogFragment = new UserSearchDialog(m_userID);

            dialogFragment.Show(transaction, "UserSearch_Dialog");
            Dismiss();
        }

        private void FillFriends()
        {
            DataAccess da = new DataAccess();
            DataTable dt = new DataTable();
            Users = new List<User>();

            dt = da.Get_Friends(m_userID);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                //1 is trail name, 6 is username
                User user = new User(dt.Rows[i].ItemArray);
                Users.Add(user);
            }
            if (Users.Count != 0)
            {
                lvFriends.Adapter = new UserListAdapter(this.Activity, Users);
                lvFriends.ItemClick += LvLikedTrails_ItemClick;
            }
            else
            {
                txtFriends.Text = "You don't have any friends :(";
            }
        }

        private void LvLikedTrails_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            LikedTrailsDialog dialogFragment = new LikedTrailsDialog(int.Parse(Users[e.Position].m_id), Users[e.Position].m_username);
            dialogFragment.Show(transaction, "LikedTrails_Dialog");
            Dismiss();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            //base.OnDismiss(dialog);
            //Activity activity = this.Activity;
            //((MapActivity)activity).GetCoord(coord);
            //((IDialogInterfaceOnDismissListener)activity).OnDismiss(dialog);
        }
    }
}
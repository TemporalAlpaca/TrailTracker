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
    public class UserSearchDialog : DialogFragment
    {
        Button btnSearch, btnCancel;
        EditText txtboxUserSearch;
        ListView lvUserSearchDialog;
        List<User> Users;
        public string coord;
        int m_userID;

        public UserSearchDialog(int userID)
        {
            m_userID = userID;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.UserSearchDialog, container, false);

            return view;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.UserSearchDialog, null);

            btnSearch = view.FindViewById<Button>(Resource.Id.btnUserSearchDialogConfirm);
            btnSearch.Click += BtnSearch_Click;

            btnCancel = view.FindViewById<Button>(Resource.Id.btnUserSearchDialogCancel);
            btnCancel.Click += BtnCancel_Click;

            txtboxUserSearch = view.FindViewById<EditText>(Resource.Id.txtboxUserSearch);
            lvUserSearchDialog = view.FindViewById<ListView>(Resource.Id.lvUserSearchDialog);

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
            string username = "";
            Users = new List<User>();

            username = txtboxUserSearch.Text;

            if (username.Trim() != "")
            {
                DataAccess da = new DataAccess();
                DataTable dt = new DataTable();

                dt = da.Find_User(username);

                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    //1 is trail name, 6 is username
                    User user = new User(dt.Rows[i].ItemArray);
                    Users.Add(user);
                }
                if (Users.Count != 0)
                {
                    lvUserSearchDialog.Adapter = new UserListAdapter(this.Activity, Users);
                    lvUserSearchDialog.ItemClick += LvUserSearchDialog_ItemClick;
                }
                else
                    Toast.MakeText(this.Context, "User not found!", ToastLength.Long).Show();

            }

        }

        private void LvUserSearchDialog_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Follow user " + Users[e.Position].m_username + " ?");

            alert.SetPositiveButton("Confirm", (senderAlert, args) => {
                DataAccess da = new DataAccess();
                if(da.Add_Friend(m_userID, int.Parse(Users[e.Position].m_id)))
                    Toast.MakeText(this.Context, "Follow successful!", ToastLength.Long).Show();
                else
                    Toast.MakeText(this.Context, "Follow failed!", ToastLength.Long).Show();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Dismiss();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}
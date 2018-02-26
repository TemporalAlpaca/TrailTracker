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

namespace Trail_Tracker.Helpers
{
    class UserListAdapter : BaseAdapter<List<User>>
    {
        List<User> m_results;
        Activity m_context;

        public UserListAdapter(Activity context, List<User> results) : base()
        {
            m_context = context;
            m_results = results;
        }

        public override List<User> this[int position]
        {
            get
            {
                return m_results;
            }
        }

        public override int Count
        {
            get
            {
                return m_results.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is supplied
            if (view == null) // otherwise create a new one
                view = m_context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            // set username
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = m_results[position].m_username;
            // return the view, populated with data, for display
            return view;
        }
    }
}
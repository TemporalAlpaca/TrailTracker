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
    public class User
    {
        public string m_username, m_password, m_email;

        public User(string username, string password, string email)
        {
            m_username = username;
            m_password = password;
            m_email = email;
        }
    }
}
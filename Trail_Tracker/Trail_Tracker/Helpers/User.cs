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
        public string m_id, m_username, m_password, m_email;
        private const int USERCOLUMNS = 4;

        public User(string username, string password, string email)
        {
            m_username = username;
            m_password = password;
            m_email = email;
        }

        public User(object[] databaseArray)
        {
            if(databaseArray.Length == USERCOLUMNS)
            {
                m_id = databaseArray[0].ToString();
                m_username = databaseArray[1].ToString();
                m_password = databaseArray[2].ToString();
                m_email = databaseArray[3].ToString();
            }
            else if(databaseArray.Length == 2)
            {
                m_id = databaseArray[0].ToString();
                m_username = databaseArray[1].ToString();
            }
        }
    }
}
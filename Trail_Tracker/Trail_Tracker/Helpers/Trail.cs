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
    public class Trail
    {
        public string m_trailname, m_start, m_end, m_path, m_username;
        public float m_length;
        public int m_id, m_likes, m_dislikes;

        private const int COL_COUNT = 9;

        public Trail(string trailname)
        {
            m_trailname = trailname;
        }

        public Trail(object[] databaseArray)
        {
            if (databaseArray.Length == COL_COUNT)
            {
                m_trailname = databaseArray[1].ToString();
                m_start = databaseArray[3].ToString();
                m_end = databaseArray[4].ToString();
                m_path = databaseArray[5].ToString();
                m_username = databaseArray[6].ToString();

                m_length = float.Parse(databaseArray[2].ToString());

                m_id = int.Parse(databaseArray[0].ToString());
                m_likes = int.Parse(databaseArray[7].ToString());
                m_dislikes = int.Parse(databaseArray[8].ToString());
            }
            else
                Console.WriteLine("Invalid database parameters in Trail Constructor.");
        }
    }
}
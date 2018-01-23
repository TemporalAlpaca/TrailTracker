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
using System.Data.SqlClient;
using System.Data;

namespace Trail_Tracker.Helpers
{
    class DataAccess
    {
        private string connection = "NICETRY";

        public bool Insert_Trail(string name, float length, string start, string end, string path, string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("INS_TRAIL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@NAME", SqlDbType.VarChar).Value = name;
                        cmd.Parameters.Add("@LENGTH", SqlDbType.Float).Value = length;
                        cmd.Parameters.Add("@START", SqlDbType.Text).Value = start;
                        cmd.Parameters.Add("@END", SqlDbType.Text).Value = end;
                        cmd.Parameters.Add("@PATH", SqlDbType.Text).Value = path;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public bool Search_Trail(string name, float length, string start, string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SEARCH_TRAIL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@NAME", SqlDbType.VarChar).Value = name;
                        cmd.Parameters.Add("@LENGTH", SqlDbType.Float).Value = length;
                        cmd.Parameters.Add("@START", SqlDbType.Text).Value = start;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;

                        con.Open();
                        //cmd.ExecuteNonQuery();
                        DataSet ds = new DataSet("SearchResults");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
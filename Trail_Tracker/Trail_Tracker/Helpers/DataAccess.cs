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

        public DataTable Search_Trail(string trailname, string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("SEARCH_TRAIL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@NAME", SqlDbType.VarChar).Value = trailname;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;

                        con.Open();
                        DataSet ds = new DataSet("SearchResults");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public DataTable Load_Trail(string startLat, string startLong)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("LOAD_TRAIL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@STARTLAT", SqlDbType.Text).Value = startLat;
                        cmd.Parameters.Add("@STARTLONG", SqlDbType.Text).Value = startLong;

                        con.Open();
                        DataSet ds = new DataSet("LoadResults");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //1 = like
        //0 = dislike
        public bool Rate_Trail(int trailID, int rating, int userID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("RATE_TRAIL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@TRAILID", SqlDbType.Int).Value = trailID;
                        cmd.Parameters.Add("@RATING", SqlDbType.Int).Value = rating;
                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Add_User(User user)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("ADD_USER", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user.m_username;
                        cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar).Value = user.m_password;
                        cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = user.m_email;

                        con.Open();
                        cmd.ExecuteNonQuery();
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

        public DataTable Login(string username, string password)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("LOGIN", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
                        cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar).Value = password;

                        con.Open();
                        DataSet ds = new DataSet("UserData");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public DataTable Find_User(string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("FIND_USER", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;

                        con.Open();
                        DataSet ds = new DataSet("UserData");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //1 = like
        //0 = dislike
        public DataTable Check_Rate(int trailID, int userID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("CHECK_RATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;
                        cmd.Parameters.Add("@TRAILID", SqlDbType.Int).Value = trailID;

                        con.Open();
                        DataSet ds = new DataSet("LikeData");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public DataTable Get_Liked_Trails(int userID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("GET_LIKED_TRAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;

                        con.Open();
                        DataSet ds = new DataSet("LikedTrails");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public DataTable Get_Friends(int userID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("GET_FRIENDS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;

                        con.Open();
                        DataSet ds = new DataSet("Friends");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool Add_Friend(int userID, int friendID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("ADD_FRIEND", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userID;
                        cmd.Parameters.Add("@FRIENDID", SqlDbType.Int).Value = friendID;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
using System.Data.SqlClient;
using System;
using System.Data;
using log4net;
using System.Reflection;

namespace MyDb
{
    public class SqlServer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string[] get_crud_values (string wbs, SqlConnection cnn)
        {
            string sql = @"select ft_c|| '' ft_C, ft_r|| '' ft_r, ft_u|| '' ft_u, ft_D || '' ft_d from TRACKER_GROUPS_SECURITY t where ft_prj = 'SMPL' and ft_wbs = '" + wbs + "'";
           string[] crud = new string[4];
           var CRUD = new object[4];
           DataTable dt = sql2DT(sql, cnn);
           if (dt != null && dt.Rows.Count > 0) { CRUD = dt.Rows[0].ItemArray; } else { return crud; }
            crud[0] = (string)(CRUD[0] + ""); crud[1] = (string)(CRUD[1] + ""); crud[2] = (string)(CRUD[2] + ""); crud[3] = (string)(CRUD[3] + "");
            return crud;
        }

        public static void get_crud(ref bool c, ref bool r, ref bool u, ref bool d, string wbs, string user_group, SqlConnection cnn)
        {
            //If nothing set -> AD can CRUD all, everyone else can just Read (select)
            c = false; r = true; u = false; d = false;
            if (user_group == "AD") {c=true; r=true; u=true;}
            string[] crud = get_crud_values(wbs, cnn); 
            c = check_sec((string)(crud[0] + ""), user_group); r = check_sec((string)(crud[1] + ""), user_group,true);
            //u = (string)(crud[2] + ""); d = (string)(crud[3] + "");
            u = check_sec((string)(crud[2] + ""), user_group); d = check_sec((string)(crud[3] + ""), user_group);
        }

        private static bool check_sec(string Disciplines, string user_group, bool r = false)
        {
            if (Disciplines == "" && user_group == "AD") { return r; }
            if (user_group == "AD" && Disciplines.Contains("AD")) { return true; }
            else {return Disciplines.Contains(user_group);}
        }

        public static int sqlexecute(string sql, SqlConnection cnn)
        {
            SqlCommand objComm = new SqlCommand(sql, cnn);
            int i = -1;
            try
            {
                i = objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
            }
            return i;
        }

        public static object sql2Value(string sql, SqlConnection cnn)
        {
            //OracleDataAdapter da = new OracleDataAdapter(sql, cnn);
            //DataTable dt = new DataTable();
            //try
            //{
            //    da.Fill(dt);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
            //}
            //// Dim x = BitConverter.ToString(dt.Rows(0).Item(0)).Replace("-", "")
            //return dt.Rows[0].ItemArray[0];
            SqlCommand objComm = new SqlCommand(sql, cnn);
            try { return objComm.ExecuteScalar(); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return null; }
        }

        public static DataTable sql2DT(string sql,  System.Data.Common.DbConnection Cnn)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, (SqlConnection)Cnn);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
            }
            return dt;
        }

        public static DataSet sql2DataSet(string sql, System.Data.Common.DbConnection Cnn)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, (SqlConnection)Cnn);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
            }
            return ds;
        }
    }
}

using System.Data.SqlClient;
using System.Data.Common;

using System;
using System.Data;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using System.Data.Odbc;
using System.Reflection;
using log4net;
using System.Diagnostics;
using System.Collections.Generic;

namespace MyDb
{
    public static class Common
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public enum DataBaseType
        {
            OLEDB = 0,
            ORACLE = 1,
            ODBC = 2,
            SQLSERVER = 3,
            SQLServerCompact = 4,
            POSTGRESS = 5,
            SQLite = 6,
            TXTFile = 99,
            XMLFileDataSet = 100,
            XMLFileDataTable = 101
        }

        public static string GUID2String(dynamic bytes)
        {

            string sysGUID = "";
            // check for a null value.
            if (bytes != null)
            {
                foreach (byte b in bytes)
                {
                    sysGUID += b.ToString("X2"); // "X" was missing :o)
                }
            }
            return sysGUID;
        }

        public static DbDataAdapter getNewDataAdapater(string sql, DbConnection cnn)
        {
            if (cnn is OleDbConnection) { return new OleDbDataAdapter(sql, (OleDbConnection)cnn); }
            else if (cnn is OracleConnection) { return new OracleDataAdapter(sql, (OracleConnection)cnn); }
            else if (cnn is OdbcConnection) { return new OdbcDataAdapter(sql, (OdbcConnection)cnn); }
            //else if (cnn is SQLiteConnection) { return new SQLiteDataAdapter(sql, (SQLiteConnection)cnn); }
            else if (cnn is SqlConnection) { return new SqlDataAdapter(sql, (SqlConnection)cnn); }
            else { return null; }
        }

        public static void getConnectionInfo(DbConnection cnnIn, int cnnID, string Prj_No,
            out string cnnString, out DataBaseType CnnType, out DbConnection cnnOut)
        {
            string sql = "select cnn_string, cnn_type from TRACKER_CONNECTIONS c where c.cnn_id = {0} and Prj_No = '{1}'";
            sql = string.Format(sql, cnnID, Prj_No);
            DataRow dr = null;
            cnnString = ""; CnnType =  DataBaseType.ORACLE; cnnOut = null;
            try {
                dr = sql2DT(sql, cnnIn).Rows[0];
                if (dr != null)
                {
                    CnnType = (DataBaseType)Enum.Parse(typeof(DataBaseType), dr["cnn_type"] + "");// (DataBaseType) dr["cnn_type"];
                    cnnString = dr["cnn_String"] + "";
                    cnnOut = cnnType2DbConnection(CnnType, cnnString);
                }
            } catch (Exception ex)
            { Console.WriteLine(ex.Message); _log.Fatal(sql, ex); }

        }

        public static void getConnectionInfo(DbConnection cnnIn, int cnnID, string Prj_No,
          out string cnnString, out DataBaseType CnnType, out OracleConnection cnnOut)
        {
            string sql = "select cnn_string, cnn_type from TRACKER_CONNECTIONS c where c.cnn_id = {0} and Prj_No = '{1}'";
            sql = string.Format(sql, cnnID, Prj_No);
            DataRow dr = null;
            cnnString = ""; CnnType = DataBaseType.ORACLE; cnnOut = null;
            try
            {
                dr = sql2DT(sql, cnnIn).Rows[0];
                if (dr != null)
                {
                    CnnType = (DataBaseType)Enum.Parse(typeof(DataBaseType), dr["cnn_type"] + "");// (DataBaseType) dr["cnn_type"];
                    cnnString = dr["cnn_String"] + "";
                    cnnOut = new OracleConnection(cnnString);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); _log.Fatal(sql, ex); }

        }

        public static DbCommandBuilder getNewCommandBuilder(DbDataAdapter da)
        {
            if (da is OleDbDataAdapter) { return new OleDbCommandBuilder((OleDbDataAdapter)da); }
            else if (da is OracleDataAdapter) { return new OracleCommandBuilder((OracleDataAdapter)da); }
            else if (da is OdbcDataAdapter) { return new OdbcCommandBuilder((OdbcDataAdapter)da); }
            //else if (da is SQLiteDataAdapter) { return new SQLiteCommandBuilder((SQLiteDataAdapter)da); }
            else if (da is SqlDataAdapter) { return new SqlCommandBuilder((SqlDataAdapter)da); }
            else { return null; }
        }

        public static DbCommand getNewDbCommand(string sql, DbConnection cnn)
        {
            if (cnn is OleDbConnection) { return new OleDbCommand(sql, (OleDbConnection)cnn); }
            else if (cnn is OracleConnection) { return new OracleCommand(sql, (OracleConnection)cnn); }
            else if (cnn is OdbcConnection) { return new OdbcCommand(sql, (OdbcConnection)cnn); }
            //else if (cnn is SQLiteConnection) { return new SQLiteCommand(sql, (SQLiteConnection)cnn); }
            else if (cnn is SqlConnection) { return new SqlCommand(sql, (SqlConnection)cnn); }
            else { return null; }
        }

        public static string[] get_crud_values (string wbs, DbConnection cnn)
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

        public static int sqlexecute(string sql, DbConnection cnn)
        {
            DbCommand objComm = getNewDbCommand(sql, cnn);
            int i = -1;
            try
            {
                i = objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
                _log.Fatal(sql, ex);
            }
            return i;
        }

        public static object sql2Value(string sql, DbConnection cnn)
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
            DbCommand objComm = getNewDbCommand(sql, cnn);
            try { return objComm.ExecuteScalar(); }
            catch (Exception ex) { Console.WriteLine(ex.Message); _log.Fatal(sql, ex); return null; }
        }

        public static DataTable sql2DT(string sql,  
            System.Data.Common.DbConnection Cnn)
        {
            DbDataAdapter da = getNewDataAdapater(sql,Cnn);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
                if (System.Diagnostics.Debugger.IsAttached)
                { System.Windows.Forms.Clipboard.SetText(sql); }
                _log.Fatal(sql, ex);
            }
            return dt;
        }

        public static DbConnection cnnType2DbConnection(DataBaseType cnnType, string cnnString)
        {
            switch (cnnType)
            {
                case DataBaseType.OLEDB:
                    return new OleDbConnection(cnnString);
                    break;
                case DataBaseType.ORACLE:
                    return new OracleConnection(cnnString); //break;
                case DataBaseType.ODBC:
                    break;
                case DataBaseType.SQLSERVER:
                    return new SqlConnection(cnnString);  break;
                case DataBaseType.SQLServerCompact:
                    break;
                case DataBaseType.POSTGRESS:
                    break;
                case DataBaseType.SQLite:
                    break;
                case DataBaseType.TXTFile:
                    break;
                case DataBaseType.XMLFileDataSet:
                    break;
                case DataBaseType.XMLFileDataTable:
                    break;
                default:
                    break;
            }
            return null;
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
                _log.Fatal(sql, ex);
            }
            return ds;
        }

        #region dataArrays

        public static List<string> getArray_String_FromDT(string columnName, DataColumn dc, System.Data.DataTable dt)
        {
            List<string> L = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (dr.IsNull(columnName)) { L.Add(""); }
                    else
                    {
                        //L.Add((dr.Field<string>(columnName)).Replace("'", "''"));
                        L.Add((dr.Field<string>(columnName)).Trim() + "");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fatal Error in getArray_String_FromDT;" + ex.Message);
                    return null;
                }
            }
            //int maxLen = L.Select(l => l.Length).Max();
            return L;
        }

        public static List<Decimal?> getArray_Decimal_FromDT(string columnName, DataColumn dc, System.Data.DataTable dt)
        {
            List<Decimal?> L = new List<Decimal?>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (dr.IsNull(columnName))
                    { L.Add(null); }
                    else
                    {
                        L.Add(Decimal.Parse("" + dr[columnName]));
                    }
                }
                catch (Exception ex)
                { Debug.Print(ex.Message); }
            }
            return L;
        }
        public static List<Decimal?> getArray_Boolean_FromDT(string columnName, DataColumn dc, System.Data.DataTable dt)
        {
            List<Decimal?> L = new List<Decimal?>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (dr.IsNull(columnName))
                    { L.Add(null); }
                    else
                    {
                        bool Value = (bool)dr[columnName]; if (Value) { L.Add(1); } else { L.Add(0); }
                    }
                }
                catch (Exception ex)
                { Debug.Print(ex.Message); }
            }
            return L;
        }
        public static List<DateTime?> getArray_Date_FromDT(string columnName, DataColumn dc, System.Data.DataTable dt)
        {
            List<DateTime?> L = new List<DateTime?>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    //L.Add(dr.Field<DateTime>(columnName));
                    if (dr.IsNull(columnName))
                    { L.Add(null); }//DateTime.Parse("")); }
                    else
                    {
                        L.Add((dr.Field<DateTime>(columnName)).Date);
                    }
                }
                catch (Exception ex)
                { Debug.Print(ex.Message); }
            }
            return L;
        }
        #endregion


        public enum qAttrType : byte
        {
            iDateTime = 4,
            iNumber = 1,
            iText = 2,
            iBoolean = 3
        }

        public static qAttrType dotNetType_to_BasicType(TypeCode tyCode)
        {
            switch (tyCode)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return qAttrType.iNumber;
                case TypeCode.Boolean: return qAttrType.iBoolean;
                case TypeCode.DateTime: return qAttrType.iDateTime;

                case TypeCode.Char:
                case TypeCode.String:
                    return qAttrType.iText;
                case TypeCode.Object: return qAttrType.iText;
                default: System.Windows.Forms.MessageBox.Show("Type not Mapped : '" + tyCode.GetType().FullName + "'"); return qAttrType.iText;
            }
        }


    }
}

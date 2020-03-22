using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace MyDb
{
    public static class Oracle
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string[] get_crud_values (string wbs, string prj_no, OracleConnection cnn )
        {
            string sql = @"select ft_c|| '' ft_C, ft_r|| '' ft_r, ft_u|| '' ft_u, ft_D || '' ft_d from TRACKER_GROUPS_SECURITY t where ft_prj = '{0}' and ft_wbs = '{1}'";
            sql = string.Format(sql, prj_no, wbs);
           string[] crud = new string[4];
           var CRUD = new object[4];
           DataTable dt = sql2DT(sql, cnn);
           if (dt != null && dt.Rows.Count > 0) { CRUD = dt.Rows[0].ItemArray; } else { return crud; }
            crud[0] = (string)(CRUD[0] + ""); crud[1] = (string)(CRUD[1] + ""); crud[2] = (string)(CRUD[2] + ""); crud[3] = (string)(CRUD[3] + "");
            return crud;
        }

        public static void get_crud(ref bool c, ref bool r, ref bool u, ref bool d, string wbs, string user_group, string prj_no, OracleConnection cnn)
        {
            //If nothing set -> AD can CRUD all, everyone else can just Read (select)
            c = false; r = true; u = false; d = false;
            if (user_group == "AD") {c=true; r=true; u=true;}
            string[] crud = get_crud_values(wbs, prj_no, cnn); 
            c = check_sec((string)(crud[0] + ""), user_group);
            r = check_sec((string)(crud[1] + ""), user_group,true);
            //u = (string)(crud[2] + ""); d = (string)(crud[3] + "");
            u = check_sec((string)(crud[2] + ""), user_group);
            d = check_sec((string)(crud[3] + ""), user_group);
        }
        // If you want to implement "*" only
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("%", ".*") + "$";
        }

        private static bool check_sec(string Disciplines, string user_group, bool r = false)
        {
            if (Disciplines == "" && user_group == "AD") { return r; }
            if (user_group == "AD" && Disciplines.Contains("AD")) { return true; }
            else {
                if (Disciplines.Contains("%"))
                {
                    foreach (string discipline in Disciplines.Split(','))
                    {
                        Boolean complex = Regex.IsMatch(user_group, WildCardToRegular(discipline));
                        if (complex) { return true; }
                    }
                    return false;
                }
                else
                {
                    return Disciplines.Contains(user_group);
                }
            }
        }

        public static int sqlexecute(string sql, OracleConnection cnn, out string errorMessage, bool showErrorMsg = true)
        {
            if(cnn.State != ConnectionState.Open) { try { cnn.Open(); } catch (Exception ex) { errorMessage = ex.Message; return -1; }}
            OracleCommand objComm = new OracleCommand(sql, cnn);
            int i = -1;errorMessage = String.Empty;
            try
            {
                i = objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (showErrorMsg) { MessageBox.Show(ex.Message + "\t" + ex.StackTrace + "\n SQL was copied to the clipboard."); }
                Clipboard.SetText(sql); errorMessage = ex.Message;
                _log.Fatal(sql, ex); Console.WriteLine(ex.Message);
            }
              return i;
        }

        public static object sql2Value(string sql, OracleConnection cnn)
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
            OracleCommand objComm = new OracleCommand(sql, cnn);
            try { return objComm.ExecuteScalar(); }
            catch (Exception ex) { Console.WriteLine(ex.Message); _log.Fatal(sql, ex); return null; }
        }

        public static DataTable sql2DT(string sql,  System.Data.Common.DbConnection Cnn)
        {
            OracleDataAdapter da = new OracleDataAdapter(sql, (OracleConnection)Cnn);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\t" + ex.StackTrace);
                _log.Error(sql, ex);
            }
            return dt;
        }

        public static DataSet sql2DataSet(string sql, System.Data.Common.DbConnection Cnn)
        {
            OracleDataAdapter da = new OracleDataAdapter(sql, (OracleConnection)Cnn);
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
        public static int Dt2Oracle(System.Data.DataTable dt, string OracleTableName,
                OracleConnection cnn, bool truncatetable = true)
        {
            OracleCommand cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            cmd.ArrayBindCount = dt.Rows.Count;
            cmd.CommandText =
           @"
insert into {0}
  (--Columns list
	{1}
  )
values (
    {2})";
            string msg, sCols, sValues; sCols = ""; sValues = "";
            // add parameters to collection
            foreach (DataColumn dc in dt.Columns)
            {
                var oP = SetOracleParam(dc, dt);
                cmd.Parameters.Add(oP);
                if (dc.ColumnName.ToUpper() == "zzAttachments".ToUpper())
                {
                    Console.WriteLine(oP.ToString());
                }
                sCols = sCols + getColName(dc.ColumnName, true) + ","; sValues = sValues + ":" + getColName(dc.ColumnName) + ",";
            }
            sCols = sCols.Substring(0, sCols.Length - 1); sValues = sValues.Substring(0, sValues.Length - 1);
            cmd.CommandText = string.Format(cmd.CommandText, OracleTableName, sCols, sValues);
            try
            {
                if (truncatetable) { sqlexecute("truncate table " + OracleTableName, cnn, out msg); }
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                { sqlexecute("Commit", cnn, out msg); }
                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Importing Data: " + ex.Message);//ex.Message.Dump("Error Importing Data");
            }
            return 0;
        }

        public static string getColName(string colName, bool showWarning = false)
        {
            string cName = colName;
            return cName;
        }

        public static string OracleType(MyDb.Common.qAttrType t)
        {
            string s = "";
            switch (t)
            {
                case MyDb.Common.qAttrType.iText: return "varchar2";
                case MyDb.Common.qAttrType.iNumber: return "number";
                case MyDb.Common.qAttrType.iBoolean: return "number(1)";
                case MyDb.Common.qAttrType.iDateTime: return "DATE";
            }
            return "varchar2";
        }

        public static OracleParameter SetOracleParam(DataColumn dc, System.Data.DataTable dt)
        {

            OracleParameter p = null; var oracletype = MyDb.Common.dotNetType_to_BasicType(Type.GetTypeCode(dc.DataType));
            switch (oracletype)
            {
                case MyDb.Common.qAttrType.iText:
                    return new OracleParameter(":" + getColName(dc.ColumnName), OracleDbType.Varchar2,
                    MyDb.Common.getArray_String_FromDT(dc.ColumnName, dc, dt).ToArray(), ParameterDirection.Input);
                case MyDb.Common.qAttrType.iNumber:
                    return new OracleParameter(":" + getColName(dc.ColumnName), OracleDbType.Decimal,
                    MyDb.Common.getArray_Decimal_FromDT(dc.ColumnName, dc, dt).ToArray(), ParameterDirection.Input);
                case MyDb.Common.qAttrType.iDateTime:
                    return new OracleParameter(":" + getColName(dc.ColumnName), OracleDbType.Date,
                    MyDb.Common.getArray_Date_FromDT(dc.ColumnName, dc, dt).ToArray(), ParameterDirection.Input);
                case MyDb.Common.qAttrType.iBoolean:
                    return new OracleParameter(":" + getColName(dc.ColumnName), OracleDbType.Decimal,
                    MyDb.Common.getArray_Boolean_FromDT(dc.ColumnName, dc, dt).ToArray(), ParameterDirection.Input);
                default:
                    return new OracleParameter(":" + getColName(dc.ColumnName), OracleDbType.Varchar2,
                        MyDb.Common.getArray_String_FromDT(dc.ColumnName, dc, dt).ToArray(), ParameterDirection.Input);
            }
        }
    }
}

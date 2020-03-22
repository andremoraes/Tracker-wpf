using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyDb
{
    public static class MsAccess
    {

        public static string DataTable2CreateTableSql(DataTable dt, string tn, bool convertColName)
        {
            string sql = "";
            if (dt != null) 
            {
                
            }
            return sql;
        }
        /*
            public static int Dt2MsAccess(DataTable dt, string TableName, string fn)
        {
            if (dt == null) { return -1; }
            String accessConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+ fn + ";";
            if (!(File.Exists(fn)))
            {
                ADOX.Catalog catalog = new ADOX.Catalog();
                catalog.Create(accessConnectionString);
            }
            OleDbConnection cnn = new OleDbConnection(accessConnectionString);
            try
            {
                cnn.Open();
                if (cnn.State == ConnectionState.Open)
                {
                    DataColumn dc, dpk;
                    string ColumnName, sqlEnd = null;
                    string tn = (TableName+"").Trim();
                    if (tn == "")
                    {
                        tn = dt.TableName;
                    }
                    string sql = "CREATE TABLE " + tn + "] (";
                    if (dt.PrimaryKey.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.PrimaryKey[0].ColumnName))
                        {
                            sqlEnd = ",CONSTRAINT pk_" + tn + " PRIMARY KEY (" + dt.PrimaryKey[0].ColumnName + ")";
                            dpk = dt.PrimaryKey[0];
                        }
                    }
                    else
                    {
                        sql = "Create table [" + tn + "] ([pk] NUMBER, ";
                        sqlEnd = ",CONSTRAINT pk_" + tn + " PRIMARY KEY (PK)";
                    }
                        foreach (DataColumn column in dt.Columns)
                    {
                        String columnName = column.ColumnName;
                        String dataTypeName = column.DataType.Name;
                        String sqlDataTypeName = getSqlDataTypeName(dataTypeName);
                        columnsCommandText += "[" + columnName + "] " + sqlDataTypeName + ",";
                    }
                    columnsCommandText = columnsCommandText.Remove(columnsCommandText.Length - 1);
                    columnsCommandText += ")";

                    
                    OleDbCommand command = new OleDbCommand(sql, cnn);
                    try { command.ExecuteNonQuery(); }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        */
    }
}

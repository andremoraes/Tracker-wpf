using ExcelDataReader;
using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
//using _ExcelDr = ExcelDataReader.IExcelDataReader;

namespace MyDb
{
    public class Db_Excel
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static System.Data.DataTable Excel2DataTable(string FileName)
        {
            System.Data.DataTable dt = null;
            try
            {
                using (var stream = File.Open(FileName, FileMode.Open, FileAccess.Read))
                {
                    dt = new System.Data.DataTable();
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex); Console.WriteLine(ex.Message);
            }
            return dt;
        }

        public static System.Data.DataSet Excel2DataSet(string FileName, bool useHeaderRow = true)
        {
            System.Data.DataSet ds = null;

            ExcelDataSetConfiguration erc = new ExcelDataSetConfiguration();
            erc.ConfigureDataTable = (tableReder) => new ExcelDataTableConfiguration()
            {
                EmptyColumnNamePrefix = "Column",
                UseHeaderRow = useHeaderRow//,                        ReadHeaderRow = (rowReader) => { rowReader.Read(); }
            };

            //using (var stream = File.Open(fn, FileMode.Open, FileAccess.Read))
            //{
            //    ExcelDataSetConfiguration erc = new ExcelDataSetConfiguration();
            //    erc.ConfigureDataTable = (tableReder) => new ExcelDataTableConfiguration()
            //    {
            //        EmptyColumnNamePrefix = "Column",
            //        UseHeaderRow = true//,                        ReadHeaderRow = (rowReader) => { rowReader.Read(); }
            //    };

            //    using (var xlsReader = ExcelReaderFactory.CreateReader(stream))
            //    {

            try
            {
                using (var stream = File.Open(FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var xlsReader = ExcelReaderFactory.CreateReader(stream))

                        //ds = new System.Data.DataSet();
                        ds = xlsReader.AsDataSet(erc);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex); System.Windows.Forms.MessageBox.Show(ex.Message); Console.WriteLine(ex.Message);
            }
            return ds;
        }


        public static System.Array setData(System.Collections.Generic.IEnumerable<System.Data.DataRow> Data, bool expHeader = true)
        {
            int rStart = 1;
            if (!expHeader) { rStart = 0; }
            var data = new object[Data.Count() + rStart, Data.First().Table.Columns.Count];
            int iR = 0;
            if (expHeader)
            {
                for (int iC = 0; iC < Data.First().Table.Columns.Count; iC++)
                {
                    data[iR, iC] = Data.First().Table.Columns[iC].Caption;
                }
                iR++;
            }
            foreach (DataRow dr in Data)
            {
                for (int iC = 0; iC < Data.First().Table.Columns.Count; iC++)
                {
                    data[iR, iC] = dr.Field<dynamic>(iC);
                }
                iR++;
            }
            return data;
        }

        public  static System.Array Transform_DT_2_Data(DataTable dt)
        {
            try
            {
                var data = new object[dt.Rows.Count, dt.Columns.Count];
                int iR = 0;
                foreach (DataRow dr in dt.Rows) //assuming that empty columns will remain empty AND columns WITH formulas will have formulas
                {
                    for (int iC = 0; iC < dt.Columns.Count; iC++)
                    {
                        string cn = dt.Columns[iC].ColumnName;
                        data[iR, iC] = dr[iC];
                    }
                    iR++;
                }
                return data;
            }
            catch (Exception ex)
            { _log.Fatal(ex); Console.WriteLine(ex.Message); return null; }
        }

        public static bool ExpData(System.Array Data, Excel.Range rDestStart)
        {
            try
            {
                Excel.Worksheet ws = rDestStart.Worksheet;
                Excel.Range rEnd = (Excel.Range)ws.Cells[Data.GetLength(0) + rDestStart.Row - 1, Data.GetLength(1) + rDestStart.Column - 1];
                Excel.Range rg = ws.Range[rDestStart, rEnd];
                rg.Value = Data;
                return true;
            }
            catch (Exception ex)
            { _log.Fatal(ex); Console.WriteLine(ex.Message); return false; }
        }

        public static System.Data.DataTable ExcelActiveSheet2DataTable(string columnsRange, string columnToGetLastRow,
                                            int rowStart, DataTable _dt, int pkCol,
                                            bool ActiveSheet = true, string sheetName = "", List<int> iCols = null)
        {
            Excel.Application xApp = null;
            try { xApp = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application"); }
            catch (Exception ex)
            { _log.Fatal("Error opening Excel",ex); Console.WriteLine(ex.Message); return null; }
            System.Data.DataTable dt = _dt.Clone();
            try
            {
                //xApp.Visible = true;
                Excel.Workbook wb = xApp.ActiveWorkbook; Excel.Worksheet ws = null;
                if (sheetName != "") { try { ws = wb.Worksheets[sheetName]; } catch { ws = wb.ActiveSheet as Excel.Worksheet; } }
                else
                {
                  ws = wb.ActiveSheet as Excel.Worksheet;
                }
                int Tot = ws.Range[columnToGetLastRow].SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
                string address = columnsRange.Substring(0, 1) + rowStart + ":" + columnsRange.Substring(columnsRange.IndexOf(':') + 1) + Tot;
                Excel.Range rg = ws.Range[address];
                var iMapTable = Set_iMapTable(_dt, iCols);
                List<ArrayList> al = RangeToList(rg, iMapTable, pkCol);
               // wb.Close(false); xApp.Quit();
                ArrayListToDataTable(al, dt);
            }
            catch (Exception ex)
            {
                _log.Fatal("Error ExcelSheetDataTable", ex); Console.WriteLine(ex.Message);
            }
            return dt;
        }

        private static qAttrType dotNetType_to_BasicType(object testObject)
        {
            TypeCode tyCode = Type.GetTypeCode(testObject.GetType());
            return dotNetType_to_BasicType(tyCode);
        }

        private static qAttrType dotNetType_to_BasicType(TypeCode tyCode)
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

        public enum qAttrType : byte
        {
            iDateTime = 4,
            iNumber = 1,
            iText = 2,
            iBoolean = 3
        }

        public static void ArrayListToDataTable(List<ArrayList> al, DataTable dt)
        {
            string str = "";
            try
            {
                foreach (var dRow in al)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        str = dRow[i]+""; DataColumn dc = dt.Columns[i];
                        var TyCode = Type.GetTypeCode(dc.DataType);
                        var a = dotNetType_to_BasicType(TyCode);
                        switch (a )
                        {
                            case  qAttrType.iDateTime:
                                if (str != "")
                                {
                                   try { dRow[i]  = DateTime.Parse(dRow[i] + ""); }
                                    catch (Exception ex)
                                    { Console.WriteLine(ex.Message + " value = " + str); }
                                }
                                break;
                            case qAttrType.iText:

                                try {dr[i] =str; }
                                catch (Exception ex)
                                { Console.WriteLine(ex.Message + " value = " + str); }
                                break;
                            case qAttrType.iNumber:
                                if (str != "")
                                {
                                   try{ dr[i] = Decimal.Parse(str); }
                                    catch (Exception ex)
                                    { Console.WriteLine(ex.Message + " value = " + str); }
                                }
                                break;
                            case qAttrType.iBoolean:
                                if (str != "")
                                {
                                    try { dr[i] = bool.Parse(str) ? 1 : 0; } catch (Exception ex)
                                    { Console.WriteLine(ex.Message + " value = " + str); }
                                }
                                break;
                            default:
                                
                                Console.WriteLine(dc.ColumnName + " : " + dc.DataType.Name + " : basic Type=" + Enum.GetName(typeof(qAttrType),a));
                                try{  dr[i] = dRow[i]; }
                                catch (Exception ex)
                                { Console.WriteLine(ex.Message + " value = " + str); }
                                break; 
                        }
                       
                    }
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            { _log.Fatal("Error ArrayListToDataTable", ex); Console.WriteLine(ex.Message + " value = " + str); }
        }



        public static List<ArrayList> RangeToList(Excel.Range inputRng, Dictionary<int, ColumnMap> iMapTable, int pkCol)
        {
            object[,] cellValues = (object[,])inputRng.Value;

            //List<string> lst = cellValues.Cast<object>().ToList().ConvertAll(x => Convert.ToString(x));

            //int rowCount = cellValues.GetLength(0);
            int rowCount = inputRng.Rows.Count;

            int columnCount = iMapTable.Count();//inputRng.Columns.Count;//cellValues.GetUpperBound(1);
            List<ArrayList> aLst = new List<ArrayList>(rowCount);
            var nDimensions = cellValues.Rank;

            for (int r = 1; r <= rowCount; r++)
            {
                var aL = new ArrayList();
                try
                {
                    //Assuming that Column is primary Key and once is empty - reach the end
                    if (pkCol > 0 && cellValues.GetValue(r, pkCol) + "" == "") { break; }
                    for (int c = 1; c <= columnCount; c++)
                    {
                        var x = cellValues.GetValue(r, iMapTable[c].ColumnOrderExcel) + "";
                        aL.Add(x);
                    }
                    if (aL != null) { aLst.Add(aL); } //aLst.Add(aL);
                                                      //List<string> rowData = cellValues.GetValue(.Cast<object>().ToList().ConvertAll(x => Convert.ToString(x));
                                                      //		 object[,] cellValues = (object[,])inputRng.Row.Value;
                } catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
            return aLst;
        } //end RangeToList

        public class ColumnMap
        {
            public int ColumnOrderExcel { get; set; }
            public string ColumName { get; set; }
            public Type type { get; set; }
        }



        public static Dictionary<int, ColumnMap> Set_iMapTable(DataTable dt, List<int> iCols)
        {
            Dictionary<int, ColumnMap> iMapTable = new Dictionary<int, ColumnMap>();
            int i = 1;
            foreach (DataColumn dc in dt.Columns)
            {
                iMapTable.Add(i, new ColumnMap()
                { ColumName = dc.ColumnName, ColumnOrderExcel = iCols is null ? i : iCols [i-1], type = typeof(string) });
                i++;
            }
            return iMapTable;
        }


        public static int DTtoOracle(System.Data.DataTable dt, 
            string OracleTableName, OracleConnection cnn, bool truncatetable = true)
        {
            OracleCommand cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            cmd.ArrayBindCount = dt.Rows.Count;
            cmd.CommandText =
           @"
insert into {0}
  (--strings
	{1}
  )
values (
    {2})";
            string msg, sCols, sValues; sCols = ""; sValues = "";
            // add parameters to collection
            foreach (DataColumn dc in dt.Columns)
            {
                try
                {
                    cmd.Parameters.Add(":" + dc.ColumnName, OracleDbType.Varchar2, getArray_String_FromDT(dc.ColumnName, dt).ToArray(),
                    ParameterDirection.Input);
                    sCols = sCols + dc.ColumnName + ","; sValues = sValues + ":" + dc.ColumnName + ",";
                } catch (Exception ex)
                { Console.WriteLine(ex.Message); _log.Fatal(ex); }
            }
            sCols = sCols.Substring(0, sCols.Length - 1); sValues = sValues.Substring(0, sValues.Length - 1);
            cmd.CommandText = string.Format(cmd.CommandText, OracleTableName, sCols, sValues);
            try
            {
                if (truncatetable) { MyDb.Oracle.sqlexecute("truncate table " + OracleTableName, cnn, out msg); }
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    MyDb.Oracle.sqlexecute("Commit", cnn, out msg);
                }
                return i;
            }
            catch (Exception ex)
            {
                //ex.Message.Dump("Error Importing Data");
                _log.Fatal(ex); Console.WriteLine(ex.Message);
            }
            return 0;
        }
        public static List<string> getArray_String_FromDT(string columnName, DataTable dt)
        {
            List<string> L = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (dr.IsNull(columnName)) { L.Add(""); }
                    else
                    {
                        L.Add((dr.Field<string>(columnName)).Replace("'", "''"));
                    }
                }
                catch (Exception ex)
                {
                    _log.Fatal(ex); Console.WriteLine(ex.Message);
                    //ex.Message.Dump("Fatal Error in getArray_String_FromDT"); return null; 
                }
            }
            return L;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MyDb
{
    public class ExcelFree
    {
        public static DataTable Xls2DataTable(string FileName, int RowStart, string RangeCols, int pk = -1, string SheetName = "")
        {
            var wb = new XLWorkbook(FileName); DataTable dt = null;
            IXLWorksheet ws = null;
            try { if (SheetName != "") { ws = wb.Worksheet(SheetName); } else { ws = wb.Worksheets.Worksheet(1); } } catch (Exception ex) { Console.WriteLine(ex.Message); }
            if (ws != null)
            {
                string RangeAddress = "";
                if (pk>0)
                {

                } else { }
                
            }
            return dt;
        }
    }
}

using log4net;
using Microsoft.VisualBasic.ApplicationServices;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Tracker.Classes;
using System.Data;
using Infragistics.Controls.Menus;
using Infragistics.Windows.DockManager;
using Tracker.Forms;
using System.Data.Common;

namespace Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public DbConnection cnnMain = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                MainWindow.WindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                HwndSource.FromHwnd(MainWindow.WindowHandle)?.AddHook(new HwndSourceHook(HandleMessages));
            };

            //Todo: this.Title = $"Parameter passing example - Process Id: {Process.GetCurrentProcess().Id}";


            string b = (System.Environment.Is64BitProcess == true ? "64" : "32");
            this.Title = "Generic Tracker (" + b + " bits)";

            //Get assembly info
            ApplicationBase Assembly = new ApplicationBase();
            string[] v = Assembly.Info.Version.ToString().Split('.');
            sbVersion.Text = "Version: " + v[0] + "." + v[1] + "." + v[2];

            sbUserName.Text = AppsCommon.Classes.Common.User();
            GetMenuItems();
        }

        void GetMenuItems()
        {
         cnnMain = MyDb.Common.cnnType2DbConnection(My.Application.Csettings.cnnType, My.Application.Csettings.cnnString); string sql = "";
            try { cnnMain.Open(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            if (cnnMain != null && cnnMain.State == ConnectionState.Open)
            {
                if (this.Main_Menu.HasItems) { this.Main_Menu.Items.Clear(); }
DataTable dtP = null; try
                {
    sql = @"select swbs, view_name from TRACKER_tb_VIEWS t where t.prj_no = '" + My.Application.Prj_No + "' and t.view_enabled=1 and view_parent is null order by view_order";
    dtP = MyDb.Oracle.sql2DT(sql, cnnMain);
}
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
                if (dtP != null && dtP.Rows.Count > 0)
                {
    foreach (DataRow drP in dtP.Rows)
    {
        string wbs = (string)(drP["SWBS"] + "");
        XamMenuItem m = new XamMenuItem();
        m.Tag = wbs; m.Header = wbs + " " + (string)drP["VIEW_NAME"];
        //                    popupmenutool.SharedProps.Category = "Reports";
        sql = "select swbs, view_name from TRACKER_tb_VIEWS t where t.prj_no = 'BPGOM' and t.view_enabled=1 and view_parent ='" + wbs + "' order by view_order";
        DataTable dtC = null; try { dtC = MyDb.Oracle.sql2DT(sql, cnnMain); }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        if (dtP != null && dtP.Rows.Count > 0)
        {
            foreach (DataRow drC in dtC.Rows)
            {
                XamMenuItem mm = new XamMenuItem();
                mm.Tag = (string)drC["swbs"]; mm.Header = (string)drC["swbs"] + " " + (string)drC["view_name"];
                mm.Click += new System.EventHandler(this.XamMenuItem_Click);
                m.Items.Add(mm);
            }
        }
        Main_Menu.Items.Add(m);
    }
}
}
        }

        void GetMenuItems_Reports()
        {
            cnnMain = new OracleConnection(My.Application.Csettings.cnnString); string sql = "";
            try { cnnMain.Open(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            if (cnnMain != null && cnnMain.State == ConnectionState.Open)
            {
                if (this.Main_Menu.HasItems) { this.Main_Menu.Items.Clear(); }
                DataTable dtP = null; try
                {
                    sql = @"select swbs, view_name from TRACKER_tb_VIEWS t where t.prj_no = '" + My.Application.Prj_No + "' and t.view_enabled=1 and view_parent is null order by view_order";
                    dtP = MyDb.Oracle.sql2DT(sql,cnnMain);
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
                if (dtP != null && dtP.Rows.Count > 0)
                {
                    foreach (DataRow drP in dtP.Rows)
                    {
                        string wbs = (string)(drP["SWBS"] + "");
                        XamMenuItem m = new XamMenuItem();
                        m.Tag = wbs; m.Header = wbs + " " + (string)drP["VIEW_NAME"];
                        //                    popupmenutool.SharedProps.Category = "Reports";
                        sql = "select swbs, view_name from TRACKER_tb_VIEWS t where t.prj_no = 'BPGOM' and t.view_enabled=1 and view_parent ='" + wbs + "' order by view_order";
                        DataTable dtC = null; try { dtC = MyDb.Oracle.sql2DT(sql, cnnMain); }
                        catch (Exception ex) { Debug.WriteLine(ex.Message); }
                        if (dtP != null && dtP.Rows.Count > 0)
                        {
                            foreach (DataRow drC in dtC.Rows)
                            {
                                XamMenuItem mm = new XamMenuItem();
                                mm.Tag = (string)drC["swbs"]; mm.Header = (string)drC["swbs"] + " " + (string)drC["view_name"];
                                mm.Click += new System.EventHandler(this.XamMenuItem_Click);
                                m.Items.Add(mm);
                            }
                        }
                        Main_Menu.Items.Add(m);
                    }
                }
            }
        }


        private void XamMenuItem_Click(object sender, EventArgs e)
        {
            if (sender.GetType().Name == "XamMenu") { return; }
            
            Infragistics.Controls.Menus.XamMenuItem m = (Infragistics.Controls.Menus.XamMenuItem)sender;
            ContentPane contentPane1 = null;
            switch ((string)m.Tag)
            {
                case "1.1":
                    System.Environment.Exit(0); break;
                case "3.1":
                    ucGanttChart ucGC = new ucGanttChart();
                    contentPane1 = new ContentPane();
                    contentPane1.Header = (string)m.Header;
                    contentPane1.Content = ucGC;//f; //p;
                    Main_TabGroupPane.Items.Add(contentPane1);
                    break;
                case "6.10.8":
                    ucDataGrid frmDG = new ucDataGrid();
                    frmDG._SQL = @"SELECT    
       to_number(year) year,  to_number(month) month, MAX(tdate) max_date,  SUM(hrs) hrs, coa
FROM 

(
SELECT year,to_char(t.tdate,'MM') month , hrs,
CASE WHEN coa LIKE '1___%' THEN 'Safety/Quality (1000)'
     WHEN coa LIKE '2___%' THEN 'Project Services/Management (2000)'
     WHEN coa LIKE '3___%' THEN 'Engineering (3000)'  
     WHEN coa LIKE '4___%' THEN 'Procurement (4000)'
     WHEN coa LIKE '5___%' THEN 'Construction (5000)'        
ELSE coa END coa , tdate

FROM BPGM_TB_GBS_LD t
WHERE year >=2016 AND t.tasknumber LIKE '__P%' AND 
t.coa NOT LIKE '3111%' AND t.coa NOT LIKE '3511%' AND t.coa NOT LIKE '3970%' AND t.coa NOT LIKE '3982%'
)
GROUP BY 
      year, coa, month
      ORDER BY 3 DESC,1,2,4
";
                    contentPane1 = new ContentPane();
                    contentPane1.Header = (string)m.Header;
                    contentPane1.Content = frmDG;//f; //p;
                    Main_TabGroupPane.Items.Add(contentPane1);
                    break;
                default:
                    //     Tracker.Forms.Admin.frmAdmin_Maintain_VL f = new Forms.Admin.frmAdmin_Maintain_VL();
                    //WpfApplication2.Forms.Page1 p = new WpfApplication2.Forms.Page1();
                    ucMaintainVL u = new ucMaintainVL();
                    contentPane1 = new ContentPane();
                    contentPane1.Header = (string)m.Tag + " - Maintain Value List";
                    contentPane1.Content = u;//f; //p;
                    Main_TabGroupPane.Items.Add(contentPane1);
                    break;
            }
        }

        //todo: private Classes.cSettings SaveSettings()
        //{
        //    Classes.cSettings cs = null;
        //    try
        //    {
        //        cs = new Classes.cSettings()
        //        {
        //            SP_Site = txtSiteUrl.Text,
        //            SP_Domain = txtSP_Domain.Text,
        //            SP_RowLimit = (int)num_sp_Row_limit.Value,
        //            SP_List_Title = txtListTitle.Text,
        //            SP_List_Path_In = txtSP_Path_Filter.Text,
        //            Path_Out = txtPathOut.Text,
        //            Get_All_Versions = chkGetAllVersions.Checked,
        //            SP_List_Path = txtSP_List_Path.Text
        //        };
        //    }
        //    catch (Exception ex)
        //    { Console.WriteLine(ex.Message); }
        //    return cs;
        //}

        public static IntPtr WindowHandle { get; private set; }

        internal static void HandleParameter(string[] args)
        {
            if (Application.Current?.MainWindow is MainWindow mainWindow)
            {
                if (args != null && args.Length > 0 && args[0]?.IndexOf("tracker", StringComparison.CurrentCultureIgnoreCase) >= 0 && Uri.IsWellFormedUriString(args[0], UriKind.RelativeOrAbsolute))
                {
                    var url = new Uri(args[0]);
                    var parsedUrl = HttpUtility.ParseQueryString(url.Query);
                    //todo: mainWindow.textBlock.Text = $"Project No: {parsedUrl.Get("prjno")}\r\nwbs: {parsedUrl.Get("wbs")}\r\nid: {parsedUrl.Get("id")}";
                }
                else { }
                    //todo: mainWindow.textBlock.Text = string.Join("\r\n", args);
            }
        }

        private static IntPtr HandleMessages(IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            if (handle != MainWindow.WindowHandle)
                return IntPtr.Zero;

            var data = UnsafeNative.GetMessage(message, lParameter);

            if (data != null)
            {
                if (Application.Current.MainWindow == null)
                    return IntPtr.Zero;

                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;

                UnsafeNative.SetForegroundWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle);

                var args = data.Split(' ');
                HandleParameter(args);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void CboDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UnsafeNative.SendMessage(WindowHandle, "ÄääÖÖ*##**(/4324=ß?? ᾁώ");
        }
    }
}

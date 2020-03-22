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
        public MyDb.Common.DataBaseType cnnType;

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
            cnnMain = MyDb.Common.cnnType2DbConnection(Class_Settings.getCnnType(My.Application.Csettings), My.Application.Csettings.cnnString);
            GetMenuItems();
        }

        public class MenuItem
        {
            public string Key { get; set; }
            public string Sql { get; set; }
            public string SqlUpdate { get; set; }
            public string Name { get; set; }
            public string Descr { get; set; }
            // public Class_Db_Common.DataBaseType cnn_type { get; set; }
            public int cnn_id { get; set; }
            public string FormType { get; set; }
            public bool isMDIList { get; set; }
            public byte TargetType { get; set; }
            public string TableName { get; set; }
            public string Icon { get; set; }
            public string FeatureCode { get; set; }
            public string FeatureUrl { get; set; }


            public MenuItem(string _Key, string Name, DbConnection cnn)
            {
                string sql =
@"select 
     feature_name Name, 
     feature_sql sql, 
     feature_descr descr,  
     cnn_id, 
     feature_icon Icon, 
     feature_ismdilist isMDIList, 
     feature_targettype TargetType, 
     feature_update_sql sqlupdate, 
     feature_FormType FormType, FEATURE_TABLENAME TABLENAME, feature_Code, 
     feature_wbs, FEATURE_URL, feature_long_sql long_sql
from tracker_tb_features
where prj_no = '{0}' and feature_wbs = '{1}'";
                
                sql = string.Format(sql, My.Application.Prj_No, _Key);
                

                
                if (cnn !=null && cnn.State == ConnectionState.Open)
                {
                    DataTable dt = MyDb.Common.sql2DT(sql, cnn);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        Key = _Key; Sql = dr["sql"] + "";
                        if (Sql.Trim() == "")
                        {
                            try { Sql = dr["long_sql"] + ""; }
                            catch (Exception ex)
                            { Console.WriteLine(ex.Message); }
                        }// _log.Fatal(ex); } }
                        Sql = Sql.Replace("%prj_no%", My.Application.Prj_No);
                        SqlUpdate = dr["sqlupdate"] + "";
                        Icon = dr["Icon"] + "";
                        Descr = dr["Descr"] + "";
                        if (SqlUpdate.Trim() == "") { SqlUpdate = Sql; }
                        Name = dr["Name"] + "";
                        FormType = "Tracker.Forms.ucDataGrid"; //dr["FormType"] + "";
                        isMDIList = false;
                        int i = 0, _cnn_id = 1; _cnn_id = int.Parse("0" + dr["cnn_id"]);
                        cnn_id = _cnn_id;
                        i = int.Parse("0" + dr["isMDIList"] + "");
                        isMDIList = i.Equals(1) ? true : false;
                        TargetType = 0; byte b = 0; b = byte.Parse("0" + dr["TargetType"]);
                        if (b > 0) { TargetType = b; }
                        TableName = dr["TableName"] + "";
                        FeatureCode = dr["Feature_Code"] + "";
                        FeatureUrl = dr["FEATURE_URL"] + "";
                    }
                }
            }
        }



        void GetMenuItems()
        {
            string sql = "";
            try { cnnMain.Open(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            if (cnnMain != null && cnnMain.State == ConnectionState.Open)
            {
                if (this.Main_Menu.HasItems) { this.Main_Menu.Items.Clear(); }
DataTable dtP = null; try
                {
    sql = @"
select FEATURE_WBS swbs, FEATURE_NAME view_name 
from tracker_tb_features t 
where t.prj_no = '" + My.Application.Prj_No +
"' and t.FEATURE_ENABLED=1 and FEATURE_PARENT is null order by FEATURE_ORDER";
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
        sql = @"
select FEATURE_WBS swbs, FEATURE_NAME view_name 
from tracker_tb_features t 
where t.prj_no = '" + My.Application.Prj_No + "' and t.FEATURE_ENABLED=1 and FEATURE_PARENT ='" + wbs + "' order by FEATURE_ORDER";
        DataTable dtC = null; try { dtC = MyDb.Oracle.sql2DT(sql, cnnMain); }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        if (dtP != null && dtP.Rows.Count > 0)
        {
            foreach (DataRow drC in dtC.Rows)
            {
                XamMenuItem mm = new XamMenuItem(); mm.Tag = (string)drC["swbs"]; mm.IsEnabled = GetMenuEnabled(mm.Tag+"");
                 mm.Header = (string)drC["swbs"] + " " + (string)drC["view_name"];
                mm.Click += new System.EventHandler(this.XamMenuItem_Click);
                m.Items.Add(mm);
            }
        }
        Main_Menu.Items.Add(m);
    }
}
}
        }

        bool GetMenuEnabled(string _Tag)
        { return true; }
        //Todo add check in security tables to see if user should not have access to open this

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
            try
            {

                //if (sender.GetType().Name == "XamMenu") { return; }
                if (sender is XamMenuItem)
                {
                    Infragistics.Controls.Menus.XamMenuItem m = (Infragistics.Controls.Menus.XamMenuItem)sender;
                    MenuItem mi = null;
                    if (m.Tag + "" != "")
                    {
                        mi = new MenuItem(m.Tag + "", m.Header + "", cnnMain);
                        ContentPane contentPane1 = new ContentPane();
                        dynamic objNewForm = Activator.CreateInstance(Type.GetType(mi.FormType, false, true));
                        //Common Attributes
                        objNewForm.Tag = mi.Key;
                        objNewForm.cnnType = MyDb.Common.DataBaseType.ORACLE;
                        objNewForm.cnnString = My.Application.Csettings.cnnString;
                        objNewForm._SQL = mi.Sql; objNewForm._SQL_Update = mi.SqlUpdate;
                        //objNewForm.Text = mi.Key + " " + mi.Name;
                        contentPane1.Header = (string)m.Header;
                        contentPane1.Content = objNewForm;//f; //p;
                        Main_TabGroupPane.Items.Add(contentPane1);
                        // ((WeifenLuo.WinFormsUI.Docking.DockContent)objNewForm).ToolTipText = _Menu.Descr;
                    }
                }
            } catch ( Exception ex)
            { Console.WriteLine(ex.Message); }
            /*
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
            */
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

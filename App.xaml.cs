using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Tracker.Classes;

namespace Tracker
{

    public static class My
    {
        private static MyApplication defaultInstance = new MyApplication();

        public static MyApplication Application
        {
            get
            {
                return (defaultInstance);
            }
        }
    }

    public class MyApplication
    {
        public Classes.cSettings Csettings = new cSettings();
        public string Prj_No = "OASIS";
        public string Public_Location = "";
        public PrjSettings _PrjSettings = new PrjSettings();

        public string getLogFileName()
        {
            return Public_Location + Prj_No + AppsCommon.Classes.Common.User() + AppsCommon.Classes.Common.Computer_Name() + ".txt";
        }

        public void GetSettings()
        {
            string SettingsFileName = System.Windows.Forms.Application.StartupPath + "\\" + "CommonProjectsSettings.xml";

            if (File.Exists(SettingsFileName))
            {
                Csettings = Classes.Class_Settings.Deserialize(SettingsFileName);
                Public_Location = Csettings.PublicPath + @"Tracker\";
                string cnnString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = '" + System.Windows.Forms.Application.StartupPath + "\\DataBase\\Tracker.accdb'; Persist Security Info = True;";
                Csettings.cnnString = cnnString;
                //txtSiteUrl.Text = csettings.SP_Site; try { num_sp_Row_limit.Value = csettings.SP_RowLimit; } catch { }
                //txtListTitle.Text = csettings.SP_List_Title;
                //txtSP_Path_Filter.Text = csettings.SP_List_Path_In; txtPathOut.Text = csettings.Path_Out;
                //txtSP_Domain.Text = csettings.SP_Domain; txtSP_List_Path.Text = csettings.SP_List_Path;
                //chkGetAllVersions.Checked = csettings.Get_All_Versions;
            }
        }
        public void SetProjectSettings()
        {
            try
            {
                DataRow dr = null;
                string sql = "Select * from tracker_tb_projects where upper(prj_no) = '{0}'";
                sql = string.Format(sql, Prj_No);
                OracleConnection cnn = new OracleConnection(Csettings.cnnString);
                try { cnn.Open(); } catch { }
                if (cnn.State == ConnectionState.Open)
                {
                    try { dr = MyDb.Oracle.sql2DT(sql, cnn).Rows[0]; } catch { }
                    if (dr != null)
                    {
                        _PrjSettings.Prj_Name = dr["Prj_Name"] + "";
                        _PrjSettings.Prj_AD_Mask = dr["prj_admask"] + "";
                        _PrjSettings.Prj_Title_Mask = dr["tracker_prg_title_mask"] + "";
                        _PrjSettings.Prj_icon = dr["prj_icon"] + "";
                        _PrjSettings.Prj_Tracker_Folder = dr["prj_tracker_folder"] + "";
                    }
                }
                cnn.Close();
            }
            catch { }

        }
    }

    public static class Program
    {
        
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            My.Application.GetSettings();

            var proc = Process.GetCurrentProcess();
            var processName = proc.ProcessName.Replace(".vshost", "");
            var runningProcess = Process.GetProcesses()
                .FirstOrDefault(x => (x.ProcessName == processName || x.ProcessName == proc.ProcessName || x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id);


            bool newInstance = true; //false
            if (args.Count() > 0)
            {
                if (args[0].ToLower().StartsWith("prj="))
                {
                  My.Application.Prj_No   = args[0].ToUpper().Replace("PRJ=", "");
                }
                if (args.Count() > 1 && args[1].ToLower().StartsWith("new"))
                {
                    newInstance = true;
                }
            }

            if (runningProcess == null | newInstance)
            {
           
                var window = new MainWindow();
                MainWindow.HandleParameter(args);
                app.Run(window);
                return; // In this case we just proceed on loading the program
            }

            if (args.Length > 0)
                UnsafeNative.SendMessage(runningProcess.MainWindowHandle, string.Join(" ", args));
        }
    }


    public class PrjSettings
    {
        public string Prj_Name { set; get; }
        public string Prj_AD_Mask { set; get; }
        public string Prj_Tracker_Folder { set; get; }
        public string Prj_Title_Mask { set; get; }
        public string Prj_icon { set; get; }
    }

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
      
    }
}
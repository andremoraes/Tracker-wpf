using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppsCommon.Classes;
using System.Data;
using System.Diagnostics;
using log4net;
using System.Reflection;

namespace Tracker.Classes
{
    class Class_User
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string ErrorMsg = "", disc = "";

        /// username = tsUser.Text.ToLower()
        /// AppVersion = tsVersion.Text 
        public static void check_user_login(string username, DbConnection cnn, ComboBox tsDDisciplines, string AppVersion)
        {

            //check if User is Admin User
            decimal cntAdmin = 0;
            string sql = "select  count(*) from TRACKER_TB_VL t where ENABLED =1 and  upper(prj) = '{0}' and lower(sgroup) = 'admin_users' and lower(VL) ='{1}'";
            try
            {
                sql = string.Format(sql, My.Application.Prj_No, username);
                var dAdmin = MyDb.Common.sql2Value(sql, cnn);
                cntAdmin = (decimal)dAdmin;
            }
            catch (Exception ex) { System.IO.File.AppendAllText(My.Application.getLogFileName(), ex.Message); if (ex.InnerException == null) { MessageBox.Show("frmMain_Load check_user Admin/n" + ex.Message); } else { MessageBox.Show(ex.InnerException.Source + "\n" + ex.InnerException.Message); } }
            //check if users has any discipline associated with him
            decimal cntOthers = 0;
            try
            {
                sql = "select count(*) from TRACKER_TB_VL t where ENABLED =1 and  upper(prj) = '{0}' and sgroup = 'Disc-Users' and lower (descr) = '" + username + "'";
                sql = string.Format(sql,  My.Application.Prj_No);
                var dOthers = MyDb.Common.sql2Value(sql, cnn);
                cntOthers = (decimal)dOthers;
            }
            catch (Exception ex) { System.IO.File.AppendAllText(My.Application.getLogFileName(), ex.Message); if (ex.InnerException == null) { MessageBox.Show("frmMain_Load check_user Disc/n" + ex.Message); } else { MessageBox.Show(ex.InnerException.Source + "\n" + ex.InnerException.Message); } }

            if (cntAdmin > 0)
            {
                sql = "select vl Disc, Details qfields from TRACKER_TB_VL t where ENABLED =1 and prj = '{0}' and sgroup = 'Disciplines'";
                sql = string.Format(sql,  My.Application.Prj_No);
                DataTable dtDisc = null; try { dtDisc =  MyDb.Common.sql2DT(sql, cnn); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
                foreach (DataRow dr in dtDisc.Rows)
                {
                    string Disc = (string)(dr["Disc"] + "");
                    var tsiDisc = new ToolStripMenuItem(); tsiDisc.Text = Disc; tsiDisc.Tag = dr["qFields"] + "";
                    tsDDisciplines.Items.Add(tsiDisc); tsDDisciplines.Text = Disc;//"AD";
                    disc = Disc;
                }
                tsDDisciplines.Items.Add("AD");
                tsDDisciplines.Text = "AD"; disc = "AD";
            }
            else
            {
                if (cntOthers > 0)
                {
                    sql = "select vl from TRACKER_TB_VL t where  ENABLED =1 and prj = '{0}' and sgroup = 'Disc-Users' and lower (descr) = '" +username + "'";
                    sql = string.Format(sql,  My.Application.Prj_No);
                    string Disc = (string) MyDb.Common.sql2Value(sql, cnn);
                    tsDDisciplines.Items.Add(Disc);
                    tsDDisciplines.Text = Disc;
                    disc = Disc;
                }
                else { disc = "New-User"; tsDDisciplines.Text = disc; }
            }

            try
            {
                bool check_user_Exist = false;
                sql = "select count(*) from TRACKER_tb_USERS where  usr_prj = '{0}' and lower(USR_NAME) = '{1}'";
                sql = string.Format(sql,  My.Application.Prj_No, username);
                int i = int.Parse("0" +  MyDb.Common.sql2Value(sql, cnn));
                check_user_Exist = i > 0;
                if (check_user_Exist)
                {
                    sql = @"update TRACKER_tb_USERS set USR_NO_ACCESS = USR_NO_ACCESS+1, USR_LAST_ACCESS = sysdate, usr_email = '{0}', usr_prg_version = '{1}', 
                                usr_machine = '{2}', USR_LAST_RUN_PATH = '{3}', USR_NET_VERSION = '{6}',
                                USR_IP_ADDRESS = '{7}', USR_LOGON_SERVER = '{8}', USR_OS_VERSION = '{9}'
                            where  usr_prj = '{4}' and  lower(USR_NAME) = '{5}'";
                    sql = string.Format(sql, Common.getEmail(), AppVersion,
                        Common.Computer_Name(), Application.ExecutablePath,
                         My.Application.Prj_No, username, Common.Get45PlusFromRegistry(),
                        Common.User_IP_Address(), Common.User_Logon_Server(),
                        new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName);
                     MyDb.Common.sqlexecute(sql, cnn);
                }
                else
                {
                    // MyDb.Common..sqlexecute("insert into TRACKER_USERS  (usr_logon, usr_name, usr_prj, usr_no_access, usr_last_access) values ('"
                    // + Class_Common.User_Logon() + "','" + tsUser.Text + "','MPL_TRACKER',1, sysdate)", cnn);
                    sql = @"insert into TRACKER_tb_USERS  (usr_logon, usr_name, usr_prj, usr_no_access,usr_machine,usr_email,usr_prg_version, USR_LAST_RUN_PATH, USR_IP_ADDRESS, USR_LOGON_SERVER, USR_OS_VERSION ) " +
                        "values ('{0}','{1}','{2}',1,'{3}','{4}','{5}','{6}','{7}','{8}','{9}')";
                    sql = string.Format(sql,
                        username, Common.User(), My.Application.Prj_No, Common.Computer_Name(),
                        Common.getEmail(), AppVersion, Application.ExecutablePath,
                        Common.User_IP_Address(), Common.User_Logon_Server(),
                        new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName); 
                     MyDb.Common.sqlexecute(sql, cnn);
                }
                if (cnn is OracleConnection) { MyDb.Common.sqlexecute("commit", cnn); }
            }
            catch (Exception ex) { _log.Fatal(ex); System.IO.File.AppendAllText(My.Application.getLogFileName(), ex.Message); if (ex.InnerException == null) { MessageBox.Show("frmMain_Load check_user_Exist/n" + ex.Message); } else { MessageBox.Show(ex.InnerException.Source + "\n" + ex.InnerException.Message); } }
        }
    }
}

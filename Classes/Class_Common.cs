using log4net;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppsCommon.Classes;

namespace Tracker.Classes
{

    class tCommon
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string CurrentUserDisc(MainWindow f) { return f.CboDiscipline.Text + ""; }
        public static string getOutPutFolder()
        {
            if (Regex.IsMatch(AppsCommon.Classes.Common.Computer_Name(), AppsCommon.Classes.Common.LikeToRegular("USAZRWPCXA%")))
            {
                if (AppsCommon.Classes.Common.isDriveExists("Z:\\"))
                {
                    string sDir = "z:\\out\\"; if (!Directory.Exists(sDir)) { try { Directory.CreateDirectory(sDir); } catch { } }
                    try { _log.Info("z: drive is '" + AppsCommon.Classes.Common.GetDriveUncPath("Z:")); } catch (Exception ex) { _log.Fatal("error to get z: drive unc path", ex); }
                    return sDir;
                }
                else
                {
                    string sDir = My.Application.Public_Location + My.Application.Prj_No + "\\" + Common.User() + "\\";
                    return sDir;
                }
            }

            else
            {
                string sDir = "c:\\out\\"; if (!Directory.Exists(sDir)) { try { Directory.CreateDirectory(sDir); } catch { } }
                return sDir;
            }
        }
        public static void fHelp(string wbs, string Layout)
        {
            try
            {
                cSettings cs = My.Application.Csettings; string Help_Path = cs.HelpPath;
                string wbs_parent = wbs.Substring(0, wbs.LastIndexOf(".")) + ".x";
                if (Strings.Trim(Layout + "") != "")
                {
                    if (System.IO.File.Exists(Help_Path + wbs_parent + "_" + Layout + ".docx"))
                    {
                        Process.Start(Help_Path + wbs_parent + "_" + Layout + ".docx"); return;
                    }
                    if (System.IO.File.Exists(Help_Path + wbs + "_" + Layout + ".docx"))
                    {
                        Process.Start(Help_Path + wbs + "_" + Layout + ".docx"); return;
                    }
                }
                //start help without being layout specific
                if (System.IO.File.Exists(Help_Path + wbs_parent + ".docx"))
                {
                    Process.Start(Help_Path + wbs_parent + ".docx"); return;
                }
                if (System.IO.File.Exists(Help_Path + wbs + ".docx"))
                {
                    Process.Start(Help_Path + wbs + ".docx"); return;
                }



            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }
    }

}

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
using Tracker;
using Tracker.Classes;

namespace AppsCommon.Classes
{
    public class Common
    {



        public static string CurrentUserDisc(MainWindow f) { return f.CboDiscipline.Text + ""; }
        public static string getOutPutFolder()
        {
            if (Regex.IsMatch(AppsCommon.Classes.Common.Computer_Name(), AppsCommon.Classes.Common.LikeToRegular("USAZRWPCXA%")))
            {
                if (AppsCommon.Classes.Common.isDriveExists("Z:\\"))
                {
                    string sDir = "z:\\out\\"; if (!Directory.Exists(sDir)) { try { Directory.CreateDirectory(sDir); } catch { } }
                    try { _Log.Info("z: drive is '" + AppsCommon.Classes.Common.GetDriveUncPath("Z:")); } catch (Exception ex) { _Log.Fatal("error to get z: drive unc path", ex); }
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


        private static ILog _Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
      [MarshalAs(UnmanagedType.LPTStr)] string localName,
      [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
      ref int length);


        public static string GetDriveUncPath(string driveLetter) //can't have the \
        {
            //DriveInfo.GetDrives().Dump();
            var sb = new StringBuilder(512);
            var size = sb.Capacity;
            var error = WNetGetConnection(driveLetter, sb, ref size);
            if (error != 0)
                throw new Win32Exception(error, "WNetGetConnection failed");
            var networkpath = sb.ToString();
            return networkpath;
        }

        public static string Computer_Name()
        {
            try
            {
                if (System.Net.Dns.GetHostEntry("localhost").HostName == "")
                { return System.Environment.GetEnvironmentVariable("COMPUTERNAME"); }
                else { return System.Net.Dns.GetHostEntry("localhost").HostName; }
            }
            catch (Exception ex) { return System.Windows.Forms.SystemInformation.ComputerName; }
        }

        public static bool isDriveExists(string driveLetterWithColonAndSlash)
        {
            return DriveInfo.GetDrives().Any(x => x.Name == driveLetterWithColonAndSlash);
        }
        public static string LikeToRegular(String value) { return "^" + Regex.Escape(value).Replace("_", ".").Replace("%", ".*") + "$"; }


        public static string Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            string s = "";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    s = ".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release"));
                }
                else
                {
                    s = ".NET Framework Version 4.5 or later is not detected.";
                }
            }
            return s;
        }

        // Checking the version using >= will enable forward compatibility.
        public static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 461308)
                return "4.7.1 or later";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            if (releaseKey >= 393295)
            {
                return "4.6";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5";
            }
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }

        public static string User_IP_Address()
        {
            string hostName = System.Net.Dns.GetHostName(); Console.WriteLine(hostName);
            string myIP = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();
            return myIP;
        }
        public static string User_Logon_Server()
        {
            return Environment.GetEnvironmentVariable("LOGONSERVER");
        }
        public static string User_Logon()
        {
            if (System.Security.Principal.WindowsIdentity.GetCurrent().Name == "")
            { return User() + "\\" + Domain(); }
            else { return System.Security.Principal.WindowsIdentity.GetCurrent().Name; }
        }


        public static string runCodeRuntime(System.Reflection.Assembly ass, object[] args)
        {
            string output = "";
            if (ass == null)
            {
                //MessageBox.Show("Compile first!")
            }
            else
            {
                Type scriptType = default(Type);
                //Dim instance As Object
                object rslt = null;
                try
                {
                    //Get the type from the assembly.  This will allow us access to
                    //all the properties and methods.
                    scriptType = ass.GetType("Script");
                    //Set up an array of objects to pass as arguments.
                    // Dim args() As Object = {cg} ', buildBasicSQL(), GridEX, DataAdapter, DataSet, gexFilter, Me, TV} '= Nothing '= {txtArgument.Text}
                    //And call the static function
                    rslt = scriptType.InvokeMember("StaticFunction", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, null, args);
                    //Return value is an object, cast it back to a string and display
                    if ((rslt != null))
                    {
                        try { output = (Convert.ToString(rslt)); }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return output;
        }


        public static string CleanFileName(string fn)
        {
            string fnOut = "", fn_;
            fn_ = System.Text.RegularExpressions.Regex.Replace(fn, @"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.(txt|gif|pdf|doc|docx|xls|xlsx)$", string.Empty);
            fn_ = fn_.Replace(":", "").Replace(@"\", "-").Replace("<", "").Replace(">", "").Replace("|", "-").Replace("*", "").Replace("/", "-").Replace(@"""", "");
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fn_ = fn_.Replace(c, '_');
            }
            fnOut = fn_;
            return fnOut;
        }

        public static string CompileCodeRunTime(string txtCodeSource,
            string featureCalling, string Disc, out DataTable dt,
            ref System.Reflection.Assembly assembly, TextBox txtConsole, string[] refs = null,
           object[] args = null)
        {
            if (txtCodeSource == "") { dt = null; return ""; }
            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersions", "v4.5" } });
            var compileParameters = new CompilerParameters();
            CompilerResults results = default(CompilerResults);
            compileParameters.GenerateInMemory = true;
            compileParameters.TreatWarningsAsErrors = false;
            compileParameters.WarningLevel = 0; //4
                                                //compileParameters.CompilerOptions = "/Imports:System.Security.Cryptography,System.IO,System,System,System.Collections,System.Collections.Generic,System.Data.Common,System.Data,System.Drawing,System.Diagnostics,System.Windows.Forms,System.Linq,clbData,System.Xml.Linq,Microsoft.Office.Interop.Excel,Oracle.ManagedDataAccess.Client";
                                                //compileParameters.CompilerOptions = compileParameters.CompilerOptions + " / optionexplicit + / optionstrict - / optioncompare:text / optimize - / debug + / warnaserror - / nowarn:42016,41999,42017,42018,42019,42032,42036,42020,42021,42022 / optioninfer+ ";

            /*•0 - Turns off emission of all warning messages.
            •1 - Displays severe warning messages.
            •2 - Displays level 1 warnings plus certain, less - severe warnings, such as warnings about hiding class members.
            •3 - Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            •4 - Displays all level 3 warnings plus informational warnings.This is the default warning level at the command line.
            */

            string excel = "",
    excel_15 = @"C:\WINDOWS\assembly\GAC_MSIL\Microsoft.Office.Interop.Excel\15.0.0.0__71e9bce111e9429c\Microsoft.Office.Interop.Excel.dll",
    excel_14 = @"C:\WINDOWS\assembly\GAC_MSIL\Microsoft.Office.Interop.Excel\14.0.0.0__71e9bce111e9429c\Microsoft.Office.Interop.Excel.dll"; ;
            if (File.Exists(excel_15))
            { excel = excel_15; }
            else
            { if (File.Exists(excel_14)) { excel = excel_14; } }

            if (refs == null)
            {
                string[] dRefs = { "System.Data.dll","System.Data.DataSetExtensions.dll",
                    "System.Core.dll", "System.dll", "System.Xml.dll",
                    "System.Windows.Forms.dll", "System.Xml.Linq.dll",
                    "System.DirectoryServices.dll","System.Security.Principal.dll",
                    "System.DirectoryServices.AccountManagement.dll",
                    "Oracle.ManagedDataAccess.dll", excel, "System.Data.Linq.dll",
                    "System.Drawing.dll","ClbData.dll",
                    "BouncyCastle.Crypto.dll","Common.Logging.Core.dll","Common.Logging.dll",
                    "itext.barcodes.dll","itext.forms.dll","itext.io.dll","itext.kernel.dll","itext.layout.dll",
                    "itext.pdfa.dll","itext.sign.dll","itext.styledxmlparser.dll","itext.svg.dll",
                    "itextsharp.dll", "ZetaLongPaths.dll"
                    //,Application.ExecutablePath +@"\Tracker.exe" 
                };


                refs = dRefs;
            }
            if ((refs != null))
            {
                compileParameters.ReferencedAssemblies.AddRange(refs);
                try
                {
                    txtCodeSource = txtCodeSource.Replace("%featureCalling%", featureCalling).Replace("%cnnstring%", "Data Source = usazrwpora03v:1523 / uspicore; User ID = TRACKERDATA; Password = TRACKERDATA").Replace(" %Disc%", Disc);
                    results = provider.CompileAssemblyFromSource(compileParameters, txtCodeSource);
                }
                catch (Exception ex)
                {
                    //Compile errors don't throw exceptions; you've got some deeper problem...
                    MessageBox.Show(ex.Message);
                    dt = null; return "Compile errors don't throw exceptions; you've got some deeper problem...";
                }
                //Output errors to the StatusBar
                if (results.Errors.Count == 0)
                {
                    assembly = results.CompiledAssembly;
                    Type program = assembly.GetType("Tracker.AddOn.Program");


                    MethodInfo main = program.GetMethod("Main");
                    if (main != null)
                    {
                        ParameterInfo[] parameters = main.GetParameters();
                        object classInstance = Activator.CreateInstance(program, null);
                        try
                        {
                            if (parameters.Length == 0)
                            {
                                main.Invoke(classInstance, null);
                            }
                            else
                            {
                                //object[] parametersArray = new object[] { "Hello" };
                                var result = main.Invoke(classInstance, args);

                                foreach (Attribute attr in program.GetCustomAttributes(true))
                                {
                                    //dt = program.GetCustomAttribute(DataTable);
                                }
                                dt = ((dynamic)classInstance).dt;
                                string txt = ((dynamic)classInstance).txtOut;
                                if (txtConsole != null) { try { txtConsole.Text = txt; } catch { } }
                                return "";
                            }
                        }
                        catch (Exception ex)
                        {
                            dt = null; MessageBox.Show(ex.Message);
                            return ex.Message;
                        }
                    }
                    dt = null; return "";
                }
                else
                {
                    string sErrors = "";
                    foreach (CompilerError err in results.Errors)
                    {
                        sErrors = sErrors + (string.Format("Line {0}, Col {1}: {4} {2} - {3}", err.Line, err.Column, err.ErrorNumber, err.ErrorText, err.IsWarning ? "Warning" : "Error")) + Constants.vbCrLf;
                    }
                    MessageBox.Show(sErrors);
                    dt = null; return sErrors;
                }
            }
            dt = null; return "empty";
        }


        public static string User()
        {
            if (System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last() == "")
            { return Environment.GetEnvironmentVariable("username"); }
            else { return System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last(); }
        }

        public static string Domain()
        {

            string myDomain = "";
            try { myDomain = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').First(); }
            //Strings.Left(User.Identity.Name, Strings.InStr(System.Threading.Thread.CurrentPrincipal.Identity.Name, "\\") - 1); }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            if (myDomain == "")
                try { myDomain = Environment.GetEnvironmentVariable("userdomain") + ""; }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            return myDomain;
        }
        //public static string UserType = "";
        public static string UserInitials()
        {
            Regex initials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
            string init = initials.Replace(User(), "$1");
            return init.ToUpper();
        }



        public static string getEmail()
        {
            string eMail = "";
            string DomainName = Domain();
            DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + DomainName);
            DirectorySearcher dirSearcher = new DirectorySearcher(dirEntry);
            //   1. Search the Active Directory for the speied user
            string userLogin = User();
            dirSearcher.Filter = "(&(objectCategory=Person)(objectClass=user) (SAMAccountName=" + userLogin + "))";
            try
            {
                dirSearcher.SearchScope = SearchScope.Subtree;
                SearchResult searchResults = dirSearcher.FindOne();
                if ((searchResults != null))
                {
                    DirectoryEntry dirEntryResults = new DirectoryEntry(searchResults.Path);
                    eMail = (string)searchResults.Properties["mail"][0];
                    dirEntryResults.Close();
                }
                dirEntry.Close();

            }
            catch (Exception ex)
            {
            }
            return eMail;
        }

        /// <summary>
        /// Converts a string to sentence case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SentenceCase(string input)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            return myTI.ToTitleCase(input);
        }
        /// <summary>
        /// e.g.: there are 4 options  : 
        ///     2.x.docx 
        ///     2.1.docx
        ///     2.x_Cost.docx
        ///     2.1_Cost.docx
        /// </summary>
        /// <param name="wbs"></param>
        /// <param name="Layout"></param>



        /// <summary>
        /// Returns an icon representation of an image contained in the specified file.
        /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, xcept this version works.
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        /// <exception cref="System.ArgumentException">filePath does not indicate a valid file.</exception>
        public static Icon ExtractAssociatedIcon(String filePath)
        {
            int index = 0;

            Uri uri = default(Uri);
            if (filePath == null)
            {
                throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
            }
            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException generatedExceptionName)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }
            //if (uri.IsUnc)
            //{
            //  throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", filePath, "filePath"), "filePath");
            //}
            if (uri.IsFile)
            {
                if (!File.Exists(filePath))
                {
                    //IntSecurity.DemandReadFileIO(filePath);
                    throw new FileNotFoundException(filePath);
                }

                StringBuilder iconPath = new StringBuilder(260);
                iconPath.Append(filePath);

                IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                if (handle != IntPtr.Zero)
                {
                    //IntSecurity.ObjectFromWin32Handle.Demand();
                    return Icon.FromHandle(handle);
                }
            }
            return null;
        }




        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>
        /// This class suppresses stack walks for unmanaged code permission. 
        /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
        /// This class is for methods that are safe for anyone to call. 
        /// Callers of these methods are not required to perform a full security review to make sure that the 
        /// usage is secure because the methods are harmless for any caller.
        /// </summary>
        [SuppressUnmanagedCodeSecurity()]
        internal sealed class SafeNativeMethods
        {
            private SafeNativeMethods()
            {
            }
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
            static internal extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
        }



        /// <summary>
        /// to release com objs like excel app 
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}

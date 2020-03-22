using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyDb;

namespace Tracker.Classes
{

    [DataContract]
    public class cSettings
    {
        [DataMember]
        public string cnnString { get; set; } 
        [DataMember]
        public MyDb.Common.DataBaseType cnnType { get; set; }

        [DataMember]
        public string HelpPath { get; set; }
        [DataMember]
        public string LayoutsPath { get; set; }
        [DataMember]
        public string TemplatesPath { get; set; }
        [DataMember]
        public string PublicPath { get; set; }
        [DataMember]
        public string ImagesPath { get; set; }

        [DataMember]
        public int SP_RowLimit { get; set; }

        [DataMember]
        public bool useWbsInMenuItems { get; set; }
    }

   public class Class_Settings
    {

        public static void Serialize(cSettings cs, string xmlFileName)
        {
            string xmlString = "";
            DataContractSerializer serializer = null;
            try
            {
                serializer = new DataContractSerializer(typeof(cSettings));//XmlSerializer(typeof(cSettings)); 
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            if (serializer != null)
                using (var sw = new StringWriter())
                {
                    using (var writer = new System.Xml.XmlTextWriter(sw))
                    {
                        writer.Formatting = System.Xml.Formatting.Indented;
                        serializer.WriteObject(writer, cs);
                        writer.Flush();
                        xmlString = sw.ToString();
                        //x.Serialize(writer, cs);
                    }
                }
            if (xmlString != "")
            {
                File.WriteAllText(xmlFileName, xmlString, Encoding.ASCII);
            }
        }
        public static cSettings Deserialize(string xmlFileName)
        {
            cSettings cs = null;
            // XmlSerializer mySerializer = new XmlSerializer(typeof(cSettings));
            DataContractSerializer serializer = null;
            try
            {
                serializer = new DataContractSerializer(typeof(cSettings));//XmlSerializer(typeof(cSettings)); 
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            if (serializer != null)
            {
                using (FileStream myFileStream = new FileStream(xmlFileName, FileMode.Open))
                {
                    try { cs = (cSettings)serializer.ReadObject(myFileStream); } catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); }
                }
            }
            return cs;
        }
    }
}

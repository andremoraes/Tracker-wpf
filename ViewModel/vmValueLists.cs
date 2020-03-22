using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Infragistics.Windows.DataPresenter;

namespace Tracker.ViewModel
{
    public class vmValueLists : INotifyPropertyChanged
    {
        public Oracle.ManagedDataAccess.Client.OracleConnection cnn = null;
        public OracleDataAdapter da_sGroup = null;
        public DataSet ds = null;
        public XamDataGrid xDataGrid = null;

        public MainWindow xMainWindow = null;
        public string wbs = "";
        private bool canInsert, canSelect, canUpdate, canDelete = false;


        private List<KeyValuePair<string, string>> _comboBoxItems;
        public List<KeyValuePair<string, string>> ComboBoxItems
        {
            get
            {
                return _comboBoxItems;
            }
            set
            {
                _comboBoxItems = value;
                RaisePropertyChanged("ComboBoxItems");
            }
        }


        private KeyValuePair<string, string> m_SelectedCboItem;
        public KeyValuePair<string, string> SelectedCboItem
        {
            get
            {
                return m_SelectedCboItem;
            }
            set
            {
                m_SelectedCboItem = value;
                RaisePropertyChanged("SelectedCboItem");
                comboBox_CurrentChanged(m_SelectedCboItem.Key);
            }
        }

        public vmValueLists()
        {
            cnn = new OracleConnection (My.Application.Csettings.cnnString);
            cnn.Open();
            refresh_comboBox();
        }

        //private CollectionView _comboBoxOptions = null;
        //public CollectionView comboBoxOptions
        //{
        //    get { return _comboBoxOptions; }
        //    set
        //    {
        //        if (_comboBoxOptions != value)
        //        {
        //            _comboBoxOptions = value;
        //            RaisePropertyChanged("comboBoxOptions");
        //        }
        //    }
        //}
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        public void refresh_comboBox()
        {

            string sql = "select  sgroup, count(*) Tot from TRACKER_TB_VL t where t.prj = 'BPGOM' group by sgroup order by 1";
            try
            {
                DataTable dt = MyDb.Oracle.sql2DT(sql, cnn);
                dt.TableName = "sGroup";
                ComboBoxItems = new List<KeyValuePair<string, string>>();//CollectionView(dt.DefaultView);
                foreach (DataRow dr in dt.Rows)
                {
                    ComboBoxItems.Add(new KeyValuePair<string, string>((string)dr["SGROUP"], dr["SGROUP"] + " (" + dr["Tot"] + ")"));
                }

                //ComboBoxItems.CurrentChanged += new EventHandler(comboBox_CurrentChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        void comboBox_CurrentChanged(string curItem)
        {
            //string curItem = (string)sender;
            string sql = @"select * from  tracker_tb_vl where sgroup = '%group%' and prj = 'BPGOM' order by 1,2".Replace("%group%", curItem);
            try
            {
                //ds = New DataSet("sGroup")
                da_sGroup = new OracleDataAdapter(sql, cnn);
                da_sGroup.AcceptChangesDuringUpdate = true;
                OracleCommandBuilder cb = new OracleCommandBuilder(da_sGroup);

                //set_upd_cmd()
                da_sGroup.UpdateCommand = cb.GetUpdateCommand();

                da_sGroup.InsertCommand = cb.GetInsertCommand();
                da_sGroup.DeleteCommand = cb.GetDeleteCommand();



                DataTable dt = MyDb.Oracle.sql2DT(sql, (System.Data.Common.DbConnection)cnn);
                dt.TableName = "sGroup";
                ds = new DataSet("sGroup");
                ds.Tables.Add(dt);
                //Class_Db_Oracle.get_crud(ref canInsert, ref canSelect, ref canUpdate, ref canDelete, wbs,
                //    Class_Common.CurrentUserDisc(xMainWindow), cnn);//.ParentForm), cnn);

                xDataGrid.DataSource = ds.Tables["sGroup"].DefaultView; //ultraGrid1.DataMember = "sGroup";




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}


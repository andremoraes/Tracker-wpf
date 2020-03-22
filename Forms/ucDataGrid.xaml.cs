using Infragistics.Windows.DataPresenter;
using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tracker.Forms
{
    /// <summary>
    /// Interaction logic for ucDataGrid.xaml
    /// </summary>
    public partial class ucDataGrid : UserControl
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string _SQL = null;
        public string _SQL_Update = null;
        public MyDb.Common.DataBaseType cnnType;

        public DbConnection cnn = null;
        public string cnnString = "";
        
        public bool canUpdate = false;
        public bool canDelete = false;
        public bool canInsert = false;
        public bool canSelect = true;

        public ucDataGrid()
        {
            InitializeComponent();
        }

        private void cboLayouts_DropDownOpened(object sender, RoutedEventArgs e)
        {

        }

        private void cboLayouts_DropDownClosed(object sender, RoutedEventArgs e)
        {

        }

        private void cmdExport2Excel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdFilter_Click(object sender, RoutedEventArgs e)
        {
            string sql = _SQL;
            string sqlFilter = dbFilter.makeQuery() + "";
            string sSQl = sqlFilter == "" ? sql : "select * from (" + sql + ") where " + sqlFilter;

            
            DataTable dt = MyDb.Common.sql2DT(sql, cnn);
            mainDataGrid.FieldLayoutSettings.AllowRecordFixing = AllowRecordFixing.TopOrBottom;
            mainDataGrid.FieldLayoutSettings.FilterAction = RecordFilterAction.Hide;
            mainDataGrid.FieldSettings.AllowSummaries = true;
            mainDataGrid.FieldSettings.AllowRecordFiltering = true;
            mainDataGrid.FieldSettings.AllowFixing = AllowFieldFixing.NearOrFar;
            mainDataGrid.FieldSettings.AllowResize = true;
            mainDataGrid.FieldSettings.AllowHiding = AllowFieldHiding.Always;
            this.mainDataGrid.DataSource = dt.DefaultView;
            foreach (var f in mainDataGrid.DefaultFieldLayout.Fields)
            {
                if (f.Settings.FilterOperandUIType == FilterOperandUIType.TextBox)
                { f.Settings.FilterOperatorDefaultValue = Infragistics.Windows.Controls.ComparisonOperator.Contains; }
            }
        }

        private void cmdHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GridMain_RecordAdded(object sender, Infragistics.Windows.DataPresenter.Events.RecordAddedEventArgs e)
        {

        }

        private void GridMain_RecordFilterChanged(object sender, Infragistics.Windows.DataPresenter.Events.RecordFilterChangedEventArgs e)
        {

        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            
        }

        private void cmdFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton ==  MouseButtonState.Pressed)//(e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
            {
                string sqlFilter = dbFilter.makeQuery() + "";
                string sSQl = sqlFilter == "" ? _SQL : "select * from (" + _SQL + ") where " + sqlFilter;
                cmdFilter.ToolTip = sSQl;
                Clipboard.SetText(sSQl);//if (e.Button == MouseButtons.Right) { Clipboard.SetText(sSQl); }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
               // cnn.Close();
            } catch (Exception ex) 
            { Console.WriteLine(ex.Message); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Enum.IsDefined(typeof(MyDb.Common.DataBaseType), cnnType) && cnnString != "")
            {
                cnn = MyDb.Common.cnnType2DbConnection(cnnType, cnnString);
                cnn.Open();
                dbFilter.setFilter(_SQL, mainDataGrid, cnn);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
using Oracle.ManagedDataAccess.Client;


namespace Tracker.Forms
{
    /// <summary>
    /// Interaction logic for ucMaintainVL.xaml
    /// </summary>
    public partial class ucMaintainVL : UserControl
    {
        public ucMaintainVL()
        {
            InitializeComponent();
            try
            {

                // RefreshCombo();

                cBoViewModel.xDataGrid = xDataGrid;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmdRefresh_Click(object sender, RoutedEventArgs e)
        {
            //  RefreshCombo();
            cBoViewModel.refresh_comboBox();
        }

        private void cmdHelp_Click(object sender, RoutedEventArgs e)
        {
            Tracker.Classes.tCommon.fHelp((string)this.Tag, "");
        }

        private void cmdExportData_Click(object sender, RoutedEventArgs e)
        {
            //  Class_UltraGrid.ExportToExcel(ultraGrid1);
        }

        //private void cboVListGroup_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{


        //}

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //try { cboViewModel.cnn.Close(); }
            //catch { }
        }

    }
}

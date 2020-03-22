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
using System.Collections.ObjectModel;
using System.Data.Common;
using Tracker.UserControls.Classes;

namespace Tracker.UserControls.ViewModel
{
  public class vmDbFilter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
        public OracleConnection cnn = null;
        public string cnnString = "";
        public vmDbFilter()
        {
            cnn = new OracleConnection(cnnString);
            cnn.Open();
        }

        private DataView _allData;
        public DataView allData
        {
            get { return _allData; }
            set
            {
                if (value != _allData)
                {
                    _allData = value;
                    //OnPropertyChanged("allData");
                }
            }
        }
    }
}

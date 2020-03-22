using Infragistics.Windows.DataPresenter;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
//using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
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
using Tracker.UserControls.Classes;

namespace Tracker.UserControls
{
    /// <summary>
    /// Interaction logic for ucFilter.xaml
    /// </summary>
    public partial class ucDbFilter : UserControl
    {
        //parameters set during Filter Setup   
        private string _BasicQuery;
        private DbConnection _cnn = null;
        private MyDb.Common.DataBaseType _cnnType = MyDb.Common.DataBaseType.ORACLE;
        XamDataGrid _xDG = null;
        List<cField> Fields = null; //Dictionary<int, cField> Fields = null;
        
        public bool _CaseSensitive ;

        
        //public string tnAttr = "tAttributes";
        //public string tnObjTypes = "tObjTypes"; //not used anymore to make the filter simples - now is just either "and" or "or"
        public string tnSQLSearch = "tSQLSearch";
        public string tnLogOper = "tLogOper";
        public string tnClauses = "tClauses";
        public string PreviousLayoutName = "";

        public DbConnection cnn
        {
            get
            {
                if ((_cnn != null) && _cnn.State != ConnectionState.Open)
                    _cnn.Open();
                return _cnn;
            }
            set
            {
                if ((!object.ReferenceEquals(_cnn, value)))
                {
                    _cnn = value;
                }
            }
        }

        


        ObservableCollection<FilterItem> FcItems { get; set; }
        ObservableCollection<Clauses> oClauses { get; set; }

        
        public string cnnString = "";
        /*
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
        public vmDbFilter()
        {
            cnn = new OracleConnection(cnnString);
            cnn.Open();
        }
        */

     


        public void setFilter(string sql, XamDataGrid xDG, DbConnection cnn)
        {
            _BasicQuery = sql; _cnn = cnn; _xDG = xDG;
            setFields();
            FcItems = new ObservableCollection<FilterItem>();
            lbMain.ItemsSource = FcItems;
            addFilterItem();
            //fi.Field = fiel
            if (FcItems.Count > 0)
            {
                lbMain.ScrollIntoView(FcItems.Last());
                Infragistics.Controls.Editors.XamMultiColumnComboEditor ce = null;
                //lbMain.Items.MoveCurrentToFirst(); 
                int pos = 0;
                lbMain.SelectedIndex = pos;
                //ListBoxItem item = lbMain.ItemContainerGenerator.ContainerFromIndex(pos) as ListBoxItem;
                //try { ce = VisualTree.FindFirstElementInVisualTree<Infragistics.Controls.Editors.XamMultiColumnComboEditor>(lbMain); } catch { }
                if (ce != null && ce.Items.Count > 0) { ce.SelectedIndex = 0; }
            }
            //    new FilterItem()
            // lbMain.Items.Refresh();
        }


        public string makeQuery(int DataRepeaterItem2Exclude = -1)
        {
            string sqlFilter = "";
            try
            {
                if (FcItems.Count > 0)
                {
                    foreach (FilterItem fi in FcItems)
                    {
                        if (fi.Field != null)
                        {
                            string attr_name = fi.Field.FieldName; Strings4Enums.qFieldType attr_type;
                        }
                        //if (
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); };
            return sqlFilter;
        }


        public ucDbFilter()
        {
            InitializeComponent();
            //
            FcItems = new ObservableCollection<FilterItem>
            {
            //new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains}, // AndOr = qLogOper.iOpenParenthesis, FieldName = "FN1", Clause= Clauses.Contains, FieldAlias ="FA1" 
            //new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains, Field = new cField() { FieldAlias  = "Doc No", FieldName = "Doc_No", FieldPosition=0, FieldHasValueList = false,FieldType = Strings4Enums.qFieldType.iText, FieldVisible = true, Tag ="" } },//AndOr = qLogOper.iAnd, FieldName = "FN2", Clause = Clauses.Contains, FieldAlias = "FA2" },
            //new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains, Field = new cField() { FieldAlias  = "Doc xNo", FieldName = "Doc_No", FieldPosition=0, FieldHasValueList = false,FieldType = Strings4Enums.qFieldType.iText, FieldVisible = true, Tag ="" } },//  new FilterItem() { AndOr = qLogOper.iAnd, FieldName = "FN3", Clause = Clauses.Contains, FieldAlias = "FA3" },
            //new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains}, // AndOr = qLogOper.iOpenParenthesis, FieldName = "FN1", Clause= Clauses.Contains, FieldAlias ="FA1" 
             new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains, Field = new cField() { FieldAlias  = "Doc No", FieldName = "Doc_No", FieldPosition=0, FieldHasValueList = false,FieldType = Strings4Enums.qFieldType.iText, FieldVisible = true, Tag ="" } },//AndOr = qLogOper.iAnd, FieldName = "FN2", Clause = Clauses.Contains, FieldAlias = "FA2" },
            //new FilterItem() {  Clauses = Strings4Enums.qClauses.Contains, Field = new cField() { FieldAlias  = "Doc xNo", FieldName = "Doc_No", FieldPosition=0, FieldHasValueList = false,FieldType = Strings4Enums.qFieldType.iText, FieldVisible = true, Tag ="" } }//  new FilterItem() { AndOr = qLogOper.iAnd, FieldName = "FN3", Clause = Clauses.Contains, FieldAlias = "FA3" },
            };
            lbMain.ItemsSource = FcItems;
            lbMain.Items.Refresh();
        }
        /// <summary>
        /// assuming for now that is using default connection always - need to add a possibilty of using a <> connection
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="xDG"></param>

        cField GetFirstVisibleField() { return Fields.Where(f => f.FieldVisible == true).First(); }

   
        private void addFilterItem()
        {
            var fi = new FilterItem();
            fi.Field = GetFirstVisibleField();
            switch (fi.Field.FieldType)
            {
                case Strings4Enums.qFieldType.iDateTime:
                    fi.Clauses = Strings4Enums.qClauses.GreaterThanOrEqualTo; 
                    break;
                case Strings4Enums.qFieldType.iNumber:
                    fi.Clauses = Strings4Enums.qClauses.GreaterThanOrEqualTo;
                    break;
                case Strings4Enums.qFieldType.iText:
                    fi.Clauses = Strings4Enums.qClauses.Contains;
                    break;
                case Strings4Enums.qFieldType.iBoolean:
                    fi.Clauses = Strings4Enums.qClauses.iTrue;
                    break;
                default:
                    break;
            }
            FcItems.Add(fi);//return fi;
            int pos = lbMain.Items.Count - 1;
            if (pos==0) 
            { lbMain.Items.Refresh(); }
            ListBoxItem item = lbMain.ItemContainerGenerator.ContainerFromIndex(pos) as ListBoxItem;
            Infragistics.Controls.Editors.XamMultiColumnComboEditor ce = null;
            if (item != null)
            {
                try
                {
                    ce = VisualTree.FindFirstElementInVisualTree<Infragistics.Controls.Editors.XamMultiColumnComboEditor>(item);
                    ce.SelectedIndex = fi.Field.FieldPosition;
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
            }
        }
        

        private void setFields()
        {
            if (_xDG == null | (_xDG != null && _xDG.DefaultFieldLayout == null) // |
                    //(_xDG != null && _xDG.DefaultFieldLayout != null || _xDG.DefaultFieldLayout.Fields == null) |
                //(_xDG != null && _xDG.DefaultFieldLayout != null || _xDG.DefaultFieldLayout.Fields != null && _xDG.DefaultFieldLayout.Fields.Count >0)
                )
            {
                OracleDataAdapter da = new OracleDataAdapter("select * from (" + _BasicQuery + ") where 1=2", (OracleConnection)_cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                setFieldsFromDT(dt.Columns);
            }
            else
            {
                foreach (Field f in _xDG.DefaultFieldLayout.Fields)
                {
                    Fields.Add(new cField()
                    {
                        FieldAlias = (string)f.Label,
                        // FieldHasValueList = f.Settings.EditorStyle.TargetType == Infragistics.Windows.Editors.XamComboEditor ?
                        FieldHasValueList = false,//_xDG.val.
                        FieldName = (string)f.DataContext,
                        FieldPosition = f.ActualPosition.Column,
                        FieldType = Strings4Enums.dotNetType_to_BasicType(f.DataType),
                        FieldVisible = f.Visibility == System.Windows.Visibility.Visible ? true : false,
                        Tag = f
                    }); 
                }
                Console.WriteLine(_xDG.DataSource.GetType().Name);
            }
        }

        private void setFieldsFromDT(DataColumnCollection dCc)
        {
            Fields = new List<cField>();
            int cPos = 0;
            foreach (DataColumn dc in dCc)
            {
                Fields.Add(new cField()
                {
                    FieldAlias = dc.Caption.Replace("_", " "),
                    FieldHasValueList = false,
                    FieldName = dc.ColumnName,
                    FieldPosition = cPos,
                    FieldType = Strings4Enums.dotNetType_to_BasicType(dc.DataType),
                    FieldVisible = true,
                    Tag = dc
                });
                cPos++;
            }
        }

        private void cboFieldName_DropDownOpening(object sender, CancelEventArgs e)
        {
            Infragistics.Controls.Editors.XamMultiColumnComboEditor ce = sender as Infragistics.Controls.Editors.XamMultiColumnComboEditor;

            //ListBoxItem item = lbMain.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;

            var cnt = ce.Items.Count();
            ce.ItemsSource = Fields;
            ce.DisplayMemberPath = "FieldAlias";
            ce.SelectedValuePath = "FieldName";
            ce.AllowMultipleSelection = false;
            ce.Columns["Tag"].Visibility = Visibility.Hidden;

        }

        public class Clauses
        {
            public Strings4Enums.qFieldType Attr_Type { get; set; }
            public Strings4Enums.qClauses Attr_Clause { get; set; }
            public string Sql { get; set; }
            public string Text { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }


        private void loadClauses(Strings4Enums.qFieldType _Attr_Type, bool CaseSensitive)
        {
            List<Strings4Enums.qClauses> clauses = Strings4Enums.Ft4Clauses(_Attr_Type);
            foreach (Strings4Enums.qClauses c in clauses)
            {
                Clauses C = new Clauses() { Attr_Type = _Attr_Type,  Attr_Clause = c,  Sql = Strings4Enums.sqlClauses(c, CaseSensitive, _Attr_Type), Text = Strings4Enums.sClause(c)  }; 
                oClauses.Add(C); 
            }
        }

        private void oClausesLoad(bool CaseSensitive)
        { 
            try
            {
                if (oClauses is null) { oClauses = new ObservableCollection<Clauses>(); }
                if (oClauses.Count == 0)
                {
                    //cbo.ItemsSource = DS.Tables[tnClauses].AsEnumerable(); cbo.DisplayMemberPath = "Text";
                    loadClauses(Strings4Enums.qFieldType.iBoolean, CaseSensitive); loadClauses(Strings4Enums.qFieldType.iDateTime, CaseSensitive);
                    loadClauses(Strings4Enums.qFieldType.iNumber, CaseSensitive); loadClauses(Strings4Enums.qFieldType.iText, CaseSensitive);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex); }
        }


        private void cboFieldName_SelectionChanged(object sender, Infragistics.Controls.Editors.SelectionChangedEventArgs e)
        {
            if (sender is Infragistics.Controls.Editors.XamMultiColumnComboEditor)
            {
                Infragistics.Controls.Editors.XamMultiColumnComboEditor cb = sender as Infragistics.Controls.Editors.XamMultiColumnComboEditor;

                if (cb.SelectedIndex ==-1 ) { return; }
                FilterItem curItem =  (FilterItem)((ListBoxItem)lbMain.ContainerFromElement(cb)).Content;//FcItems[lbMain.SelectedIndex];
                if (curItem != null)
                {
                    cField prevITem = null; try { prevITem = e.RemovedItems[0] as cField; } catch { }
                    //curItem.Field = e.AddedItems[0] as cField;
                    FcItems_Set_newField_Value(curItem, e.AddedItems[0] as cField);

                    int pos = lbMain.Items.IndexOf(curItem);
                    //dynamic item = lbMain.[pos];
                    ListBoxItem item = lbMain.ItemContainerGenerator.ContainerFromIndex(pos) as ListBoxItem;
                    System.Windows.Controls.Image imgtype = VisualTree.FindFirstElementInVisualTree<System.Windows.Controls.Image>(item);
                    if (prevITem !=null && curItem.Field != null && prevITem.FieldType != curItem.Field.FieldType) 
                    { 
                        imgtype.Tag = Enum.GetName(typeof(Strings4Enums.qFieldType), curItem.Field.FieldType);
                        imgtype.Source = new ImageSourceConverter().ConvertFromString(getImageAttr_Type(curItem.Field.FieldType)) as ImageSource;
                    }
                    ComboBox cboAttrClause = VisualTree.SearchVisualTreeForCombo(item, "cboClause");

                    
                    if (cboAttrClause != null)
                    {
                        Debug.WriteLine(cboAttrClause.Name);
                        oClausesLoad(_CaseSensitive);
                        cboAttrClause.ItemsSource = oClauses.Where (c => c.Attr_Type== curItem.Field.FieldType);
                        cboAttrClause.DisplayMemberPath = "Text"; cboAttrClause.SelectedValuePath = "Attr_Clause";
                        cboAttrClause.SelectedIndex = 0;
                        //curItem.Clauses = ((dynamic)cboAttrClause.SelectedItem).Attr_Clause;
                        FcItems_Set_newClause_Value (curItem, ((dynamic)cboAttrClause.SelectedItem).Attr_Clause);

                    }
                    //FcItems[FcItems.IndexOf(curItem)].Field =  curItem.Field;
                    //         FcItems[pos].Clauses = curItem.Clauses;
                    //Debug.WriteLine(curItem.Field.FieldName);
                    
                }
            }
        }

        void FcItems_Set_newClause_Value(FilterItem curItem, Strings4Enums.qClauses newClause)
        {
            foreach (var fcitem in FcItems)
            {
                if (fcitem == curItem)
                {
                    System.Reflection.PropertyInfo[] Fields = fcitem.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo field in Fields)
                    {
                        var currentField = field.GetValue(fcitem, null);
                        if (field.PropertyType.Name == "qClauses")
                        {
                            field.SetValue(fcitem, newClause);
                            return;
                        }
                    }
                }
            }
        }

        void FcItems_Set_newField_Value(FilterItem curItem, cField newField)
        {
            foreach (var fcitem in FcItems)
            {
                if (fcitem == curItem)
                {
                    System.Reflection.PropertyInfo[] Fields = fcitem.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo field in Fields)
                    {
                        var currentField = field.GetValue(fcitem, null);
                        if (field.PropertyType.Name == "cField") 
                        { 
                             field.SetValue(fcitem, newField);
                                return;
                        }
                    }
                }
            }
        }

        Dictionary<string, Bitmap> AttrTypeImages = new Dictionary<string, Bitmap>();

        private string getImageAttr_Type(Strings4Enums.qFieldType attr_type)
        {
            switch (attr_type)
            {
                case Strings4Enums.qFieldType.iNumber: return "pack://siteoforigin:,,,/Resources/Number.bmp";
                case Strings4Enums.qFieldType.iText: return "pack://siteoforigin:,,,/Resources/Text.bmp";// AttrTypeImages["iText"];//(Bitmap)imageList1.Images["text"];
                case Strings4Enums.qFieldType.iBoolean: return "pack://siteoforigin:,,,/Resources/Boolean.bmp";
                case Strings4Enums.qFieldType.iDateTime: return "pack://siteoforigin:,,,/Resources/Date.bmp";
                default: return null;
            }
        }


   

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //  Button s = (Button)sender;
                FilterItem curItem = (FilterItem)((ListBoxItem)lbMain.ContainerFromElement((Button)sender)).Content;
                //FcItems.Add();
                //{ AndOr = qLogOper.iAnd, FieldName = "FN" + FcItems.Count, Clause = Clauses.Contains, FieldAlias = "FA" + FcItems.Count }
                //var fi = addFilterItem();
                //fi.Field = fiel
                //FcItems.Add(fi);
                addFilterItem();
                if (FcItems.Count > 0)
                {
                    lbMain.ScrollIntoView(FcItems.Last());
                }
                //    new FilterItem()
               // lbMain.Items.Refresh();
            }
            catch { }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilterItem curItem = (FilterItem)((ListBoxItem)lbMain.ContainerFromElement((Button)sender)).Content;
                if (FcItems.Count > 1)
                {
                    FcItems.Remove(curItem);
                    lbMain.Items.Refresh();
                }
            }
            catch { }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cboValue2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void cboValue2_DropDownOpened(object sender, EventArgs e)
        {
            cboValue_DropDownOpened(sender, e);
        }



        private void cboValue1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
        } 
        private void cboValue_DropDownOpened(object sender, EventArgs e)
        {
            if (lbMain.SelectedIndex == -1 ) { return; }
            if (FcItems[lbMain.SelectedIndex].Field is null ) { return; }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            int DataRepeaterItem2Exclude = lbMain.SelectedIndex;
            string sqlWhere = makeQuery(DataRepeaterItem2Exclude);
            string attrName = (string)FcItems[lbMain.SelectedIndex].Field.FieldName;
            ComboBox cbovalue1 = sender as ComboBox;
            if (attrName != null)
            {
                string sql = "select distinct " + attrName + " from (" + _BasicQuery + ") v " + (sqlWhere != "" && optAnd.IsChecked.Value ?
                            " where " + sqlWhere : "" + " order by 1");
                DataTable dt = (MyDb.Common.sql2DT(sql, cnn));
                var ListDist = dt.AsEnumerable().Select(p => p.Field<dynamic>(0)).Distinct();
                cbovalue1.ItemsSource = ListDist;
                cbovalue1.DisplayMemberPath = dt.Columns[0].ColumnName;
                //cbovalue1.Width = Auto;

            }
            Mouse.OverrideCursor = null;
        }
        private void cboValue1_DropDownOpened(object sender, EventArgs e)
        {
            cboValue_DropDownOpened(sender, e);
        }

        private void cboClause_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int cbv1w_when_cbv2_is_not_visible = 298;
            int cbv1w_when_cbv2_is_visible = 130;

            ComboBox cboClause = sender as ComboBox;
            if (cboClause.SelectedIndex ==-1) { return; }
            Clauses C = e.AddedItems[0] as Clauses;

            FilterItem curItem = (FilterItem)((ListBoxItem)lbMain.ContainerFromElement(cboClause)).Content;
            if (curItem != null)
            {
                var prevITem = curItem.Field;
                curItem.Field = e.AddedItems[0] as cField;
                int pos = lbMain.Items.IndexOf(curItem);
                //dynamic item = lbMain.[pos];
                ListBoxItem item = lbMain.ItemContainerGenerator.ContainerFromIndex(pos) as ListBoxItem;
                
                ComboBox cbv1 = VisualTree.SearchVisualTreeForCombo(item, "cboValue1");
                ComboBox cbv2 = VisualTree.SearchVisualTreeForCombo(item, "cboValue2");
                Button bPaste = VisualTree.SearchVisualTreeForButton(item, "btnPaste");
                Label _lbl_2Values_Separator = VisualTree.FindFirstElementInVisualTree<Label>(item);
                //Grid grid = VisualTree.FindFirstElementInVisualTree<Grid>(item);
                switch (C.Attr_Clause)
                {
                    case Strings4Enums.qClauses.Between:
                    case Strings4Enums.qClauses.NotBetween:
                        cbv1.Visibility = Visibility.Visible; cbv1.Width = cbv1w_when_cbv2_is_visible;
                        cbv2.Visibility = Visibility.Visible; _lbl_2Values_Separator.Visibility = Visibility.Visible;
                        bPaste.Visibility = Visibility.Hidden; bPaste.IsEnabled = false;
                        break;
                    case Strings4Enums.qClauses.IsNull:
                    case Strings4Enums.qClauses.IsEmpty:
                    case Strings4Enums.qClauses.NotIsNull:
                        cbv1.Visibility = Visibility.Hidden; cbv2.Visibility = Visibility.Hidden;
                        _lbl_2Values_Separator.Visibility = Visibility.Hidden;
                        bPaste.Visibility = Visibility.Hidden; bPaste.IsEnabled = false;
                        break;
                    case Strings4Enums.qClauses.iIn:
                    case Strings4Enums.qClauses.NotIn:
                        cbv1.Visibility = Visibility.Visible; cbv1.Width = cbv1w_when_cbv2_is_not_visible;
                        cbv2.Visibility = Visibility.Hidden; _lbl_2Values_Separator.Visibility = Visibility.Hidden;
                        bPaste.Visibility = Visibility.Visible; bPaste.IsEnabled = true;
                        break;
                    default:
                        cbv1.Visibility = Visibility.Visible; cbv1.Width = cbv1w_when_cbv2_is_not_visible;
                        cbv2.Visibility = Visibility.Hidden; _lbl_2Values_Separator.Visibility = Visibility.Hidden;
                        bPaste.Visibility = Visibility.Hidden; bPaste.IsEnabled = false; 
                        Grid.SetColumnSpan(cbv1, 3);
                        break;
                }
            }

             
        }






    }

    public class cField
    {
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public bool FieldVisible { get; set; }
        public int FieldPosition { get; set; }
        public bool FieldHasValueList { get; set; }
        public object Tag { get; set; }
        public Strings4Enums.qFieldType FieldType { get; set; }
        public override string ToString()
        {
            return FieldAlias;
        }
    }

    public class FilterItem
    {
       // public Strings4Enums.qLogOper AndOr { get; set; }
        public cField Field { get; set; }
        public Strings4Enums.qClauses Clauses { get; set; }
        public string Value1 { get; set; }  //would be nice to have a control as datecalendar or listbox (with) for some datatypes or sclauses
        public string Value2 { get; set; }  //present only in a few options of sClause e.g Between; 
    }

            
}

/*

private Bitmap getImageAttr_Type_OLD(Strings4Enums.qFieldType attr_type)
{
    if (AttrTypeImages.Count == 0)
    {
        AttrTypeImages.Add("iNumber", global::Tracker.UserControls.Properties.Resources.Number);
        AttrTypeImages.Add("iText", global::Tracker.UserControls.Properties.Resources.Text);
        AttrTypeImages.Add("iBoolean", global::Tracker.UserControls.Properties.Resources.Boolean);
        AttrTypeImages.Add("iDateTime", global::Tracker.UserControls.Properties.Resources.Date);
    }
    switch (attr_type)
    {
        case Strings4Enums.qFieldType.iNumber: return AttrTypeImages["iNumber"];// (Bitmap)imageList1.Images["decimal"];
        case Strings4Enums.qFieldType.iText: return AttrTypeImages["iText"];//(Bitmap)imageList1.Images["text"];
        case Strings4Enums.qFieldType.iBoolean: return AttrTypeImages["iBoolean"];//(Bitmap)imageList1.Images["bool"];
        case Strings4Enums.qFieldType.iDateTime: return AttrTypeImages["iDateTime"];//(Bitmap)imageList1.Images["date"];
        default: return null;
    }
}
*/
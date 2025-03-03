using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using CBSControls;
using DataFactory;
using CDTLib;
using CDTControl;
using DevControls;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using ErrorManager;
using System.Threading;
//using publicCDT;
using DevExpress.XtraGrid.Views.BandedGrid;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormFactory
{

    public class FormDesigner
    {
        // Fields
        public List<BaseEdit> _BaseList;
        private BindingSource _bindingSource;
        private CDTData _data;
        private BaseEdit _firstControl;
        private FormAction _formAction;
        public List<GridControl> _gcDetail;
        private GridControl _gcMain;
        public List<CDTGridLookUpEdit> _glist;
        private List<LookUp_CDTData> _Glist;
        public List<LayoutControlItem> _LayoutList;
        public List<DataSingle> _lstData;
        private List<CDTRepGridLookup> _lstRep;
        private List<CDTRepGridLookup> _rlist;
        private List<RLookUp_CDTData> _Rlist;
        public bool InsertedToDetail;
        private Hashtable RIOldValue;
        private Hashtable GOldValue;
        public XtraTabControl TabDetail;
        public List<fileContener> _lFileContener = new List<fileContener>();
        public List<ImageContener> _lImageContener = new List<ImageContener>();
        private System.Windows.Forms.ImageList imageList1;
        private LayoutControl lcMain;
        // Methods
        public FormDesigner(CDTData data)
        {
            this._lstData = new List<DataSingle>();
            this.RIOldValue = new Hashtable();
            this.GOldValue = new Hashtable();
            this._lstRep = new List<CDTRepGridLookup>();
            this._Glist = new List<LookUp_CDTData>();
            this._glist = new List<CDTGridLookUpEdit>();
            this._Rlist = new List<RLookUp_CDTData>();
            this._rlist = new List<CDTRepGridLookup>();
            this._BaseList = new List<BaseEdit>();
            this._LayoutList = new List<LayoutControlItem>();
            this.TabDetail = new XtraTabControl();
            this.InsertedToDetail = true;
            this._data = data;
            if (this._data.dataType == DataType.Single)
            {
                this._lstData.Add(this._data as DataSingle);
                if (publicCDTData.findCDTData(_data._tableName, _data.Condition, _data.DynCondition) == null)
                {
                    //publicCDTData.AddCDTData(_data);
                }
            }
        }

        public FormDesigner(CDTData data, BindingSource bindingSource)
        {
            this._lstData = new List<DataSingle>();
            this.RIOldValue = new Hashtable();
            this.GOldValue = new Hashtable();
            this._lstRep = new List<CDTRepGridLookup>();
            this._Glist = new List<LookUp_CDTData>();
            this._glist = new List<CDTGridLookUpEdit>();
            this._Rlist = new List<RLookUp_CDTData>();
            this._rlist = new List<CDTRepGridLookup>();
            this._BaseList = new List<BaseEdit>();
            this._LayoutList = new List<LayoutControlItem>();
            this.TabDetail = new XtraTabControl();
            this.InsertedToDetail = true;
            this._data = data;
            this._bindingSource = bindingSource;
            if (this._data.dataType == DataType.Single )//&& this._data.dataType!=DataType.MasterDetail)
            {
                this._lstData.Add(this._data as DataSingle);
                
            }
        }

        private void FormDesigner_Spin1(object sender, SpinEventArgs e)
        {
            (sender as CalcEdit).ShowPopup();
        }

        private void gcMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
               if(_data.dataType==DataType.MasterDetail )
                {

                    if (_data.DrTable["sDelete"] == DBNull.Value || bool.Parse(Config.GetValue("Admin").ToString()))
                    {
                        GridControl gcMain = sender as GridControl;
                        (gcMain.MainView as GridView).DeleteSelectedRows();
                        
                    }
                    else
                    {
                        string s = _data.DrTable["sDelete"].ToString();
                        bool t = false;
                        if (s == "1") t = true;
                        else if (s == "0") t = false;
                        else
                        {
                            string Key = _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString();
                            string con = "(" + _data.PkMaster.FieldName + "=" + _data.quote + Key + _data.quote + " and " + s + ")";
                            DataRow[] ldrCur = _data.DsData.Tables[0].Select(con);
                            if (ldrCur.Length > 0) t = true;
                            else t = false;
                        }
                        if (t)
                        {
                            GridControl gcMain = sender as GridControl;
                            (gcMain.MainView as GridView).DeleteSelectedRows();
                        }
                    }    
                }
                
            }
            else if (e.KeyCode == Keys.F8)
            {
                GridControl gcMain = sender as GridControl;
                if (gcMain.Views.Count > 1)
                {
                    for (int i = 1; i < gcMain.Views.Count; i++)
                    {
                        if (gcMain.IsVisibleView(gcMain.Views[i]))
                        {
                            if ((gcMain.Views[i] as GridView) != null)
                            {
                                (gcMain.Views[i] as GridView).DeleteSelectedRows();
                            }
                            else
                            {
                                (gcMain.Views[i] as AdvBandedGridView).DeleteSelectedRows();
                            }
                        }
                    }
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                if (this.formAction == FormAction.New || this.formAction == FormAction.Edit || this.formAction == FormAction.Copy)
                {
                    GridControl gcMain = sender as GridControl;
                    DataRow drCurrentTable;
                    drCurrentTable = _data.DrTable;
                    string TableName = gcMain.DataMember;
                    if (TableName == string.Empty) return;
                    if (_data.DsData.Tables[1].TableName == TableName) drCurrentTable = _data.DrTable;
                    else
                        foreach (DataRow dr in _data._drTableDt)
                        {
                            if (dr["TableName"].ToString() == TableName)
                            {
                                drCurrentTable = dr;
                                break;
                            }
                        }
                    if ((gcMain.MainView as GridView).SelectedRowsCount > 0)
                    {
                        int[] i = (gcMain.MainView as GridView).GetSelectedRows();
                        for (int j = 0; j < i.Length; j++)
                        {
                            DataRow dr = (gcMain.MainView as GridView).GetDataRow(i[j]);
                            DataRow drDes= _data.Clone1Row(dr);
                            if (e.Modifiers == Keys.Shift)
                            {
                                _data.CDTData_eventClonedtRow(drDes);
                            }
                            
                        }
                    }
                }
            }
            
        }

        public BaseEdit GenCBSControl(DataRow dr)
        {
            BaseEdit tmp;
            string dataMember = dr["FieldName"].ToString();
            int pType = int.Parse(dr["Type"].ToString());
            switch (pType)
            {
                case 0:
                    tmp = new VTextEdit();
                    (tmp as VTextEdit).Properties.MaxLength = 0x20;
                    (tmp as VTextEdit).Properties.CharacterCasing = CharacterCasing.Upper;
                    break;

                case 1:
                case 4:
                case 7:
                    {
                        tmp = this.GenGridLookupEdit(dr, true);
                        //if (tmp == null)
                        //    return null;
                        CDTGridLookUpEdit tmpGrd = tmp as CDTGridLookUpEdit;
                        tmpGrd.Properties.CloseUpKey = new KeyShortcut(Keys.Control | Keys.Down);
                        tmpGrd.Properties.View.OptionsView.ShowAutoFilterRow = true;
                        tmpGrd.Properties.View.OptionsView.ColumnAutoWidth = false;
                        tmpGrd.Properties.View.OptionsView.ShowGroupedColumns = false;
                        break;
                    }
                case 2:
                    tmp = new VTextEdit();
                    (tmp as VTextEdit).Properties.MaxLength = 0xff;
                    if (dataMember.ToLower() == "soseri" || dataMember.ToLower() == "soserie")
                    {
                        (tmp as VTextEdit).Properties.CharacterCasing = CharacterCasing.Upper;
                    }
                    break;

                case 5:
                    tmp = new VSpinEdit();
                    break;

                case 8:
                    tmp = new VCalcEdit();
                    tmp.Tag = dr;
                    (tmp as VCalcEdit).Spin += new SpinEventHandler(this.FormDesigner_Spin1);
                    (tmp as VCalcEdit).KeyUp += new KeyEventHandler(this.VCalEdit_KeyUp);
                    if (dr["EditMask"].ToString() != string.Empty)
                    {

                        (tmp as VCalcEdit).Properties.EditMask = dr["EditMask"].ToString();
                       
                        (tmp as VCalcEdit).Properties.Mask.UseMaskAsDisplayFormat = true;

                    }
                    tmp.EditValueChanged += new EventHandler(tmp_EditValueChanged);
                    break;

                case 9:
                    tmp = new VDateEdit();
                    if (dr["EditMask"].ToString() != string.Empty)
                    {
                        (tmp as VDateEdit).Properties.DisplayFormat.FormatType = FormatType.DateTime;
                        if (dr["EditMask"].ToString() == "n")
                        {
                            (tmp as VDateEdit).Properties.EditMask = "dd/MM/yyyy";
                        }
                        else

                            (tmp as VDateEdit).Properties.EditMask = dr["EditMask"].ToString();
                       
                        (tmp as VDateEdit).Properties.Mask.UseMaskAsDisplayFormat = true;

                    }
                    break;

                case 10:
                    tmp = new VCheckEdit();
                    tmp.Text = dr["Tip"].ToString();
                    tmp.KeyDown += (sender, e) =>
                    {
                        if (e.KeyCode == Keys.Delete)
                        {
                            tmp.EditValue = null;
                        }
                    };
                    break;

                case 11:
                    tmp = new TimeEdit();
                    if (dr["EditMask"].ToString() != string.Empty)
                    {
                        (tmp as TimeEdit).Properties.DisplayFormat.FormatType = FormatType.DateTime;
                        if (dr["EditMask"].ToString() == "n")
                        {
                            (tmp as TimeEdit).Properties.EditMask = "dd/MM/yyyy";
                        }
                        else

                            (tmp as TimeEdit).Properties.EditMask = dr["EditMask"].ToString();
                        (tmp as TimeEdit).Properties.Mask.UseMaskAsDisplayFormat = true;

                    }
                    break;

                case 12:
                    ImageContener IC = new ImageContener(dr);
                    if (Config.GetValue("Language").ToString() == "0")
                    {
                        IC.Properties.NullText = "Click phải chuột chọn nạp h\x00ecnh";
                        (IC as PictureEdit).Properties.SizeMode = PictureSizeMode.Stretch;                    }
                    IC.DataBindings.Add("EditValue", this._bindingSource, dataMember, true, DataSourceUpdateMode.OnValidation);
                    IC.EditValueChanged += IC_EditValueChanged;
                    _lImageContener.Add(IC);
                    tmp = IC as BaseEdit;
                    break;

                case 13:
                    tmp = new MemoEdit();
                    tmp.Height = 200;
                    break;

                case 14:
                    tmp = new VDateEdit(); (tmp as VDateEdit).Properties.DisplayFormat.FormatType = FormatType.DateTime;
                    if (dr["EditMask"].ToString() == string.Empty)
                    {
                        (tmp as VDateEdit).Properties.EditMask = "dd/MM/yyyy HH:mm:ss tt";
                        (tmp as VDateEdit).Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss tt";
                    }
                    else
                    {
                        if (dr["EditMask"].ToString() == "n")
                            (tmp as VDateEdit).Properties.EditMask = "dd/MM/yyyy";
                        else
                        {
                            (tmp as VDateEdit).Properties.EditMask = dr["EditMask"].ToString();
                        }    
                        (tmp as VDateEdit).Properties.DisplayFormat.FormatString = dr["EditMask"].ToString();
                    }
                    tmp.EditValue = DateTime.Now;
                    tmp.Text = tmp.EditValue.ToString();
                    (tmp as VDateEdit).DateTime = DateTime.Now;
                    break;
                case 16:

                    fileContener fC = new fileContener(dr);
                    fC.DataBindings.Add("Text", this._bindingSource, dataMember, false, DataSourceUpdateMode.OnValidation);
                    fC.LoadnewData += new EventHandler(fC_LoadnewData);
                    _lFileContener.Add(fC);
                    tmp = fC as BaseEdit;
                    break;
                default:
                    tmp = null;
                    break;
            }
            if (tmp != null)
                {
                tmp.Name = dr["FieldName"].ToString();
                tmp.ToolTip = dr["Tip"].ToString();
                tmp.ToolTip = " Tên trường: " + dr["fieldName"].ToString();
                if (dr["Formula"] != DBNull.Value)
                    tmp.ToolTip += "\n Công thức: " + dr["Formula"].ToString();
                if (dr["FormulaDetail"] != DBNull.Value)
                    tmp.ToolTip += "\n Giá trị lấy từ trường: " + dr["FormulaDetail"].ToString();
                if(dr["refTable"]!=DBNull.Value)
                    tmp.ToolTip += "\n Tham chiếu đến báng: " + dr["refTable"].ToString();                
                if (dr["DynCriteria"] != DBNull.Value)
                    tmp.ToolTip += "\n Lọc những dòng thỏa điều kiện: " + dr["DynCriteria"].ToString();
                tmp.ToolTip+= "\n " + ((Config.GetValue("Language").ToString() == "0") ? dr["Tip"].ToString() : dr["TipE"].ToString());
                if (pType != 12 && pType != 16)
                {
                    tmp.DataBindings.Add("EditValue", this._bindingSource, dataMember, false, DataSourceUpdateMode.OnValidation);
                }

                if (int.Parse(dr["TabIndex"].ToString()) == -1)
                { tmp.TabIndex = 100; }
                bool admin = bool.Parse(Config.GetValue("Admin").ToString());
                if (!admin)
                {

                    string canEdit1 = dr["Editable1"].ToString();
                    //string canEdit = dr["Editable"].ToString();
                    if (canEdit1 != string.Empty)
                    {
                        tmp.Properties.ReadOnly = !bool.Parse(canEdit1);
                    }

                }

            }

            return tmp;
        }



        private int GetDecimalPlacesFromMask(string editMask)
        {
            // Sử dụng regex để đếm số ký tự # hoặc 0 sau dấu '.'
            var match = Regex.Match(editMask, @"\.(#+|0+)");
            if (match.Success)
            {
                return match.Value.Length - 1; // Trừ đi dấu '.'
            }
            return 0; // Nếu không có phần thập phân
        }
        private void IC_EditValueChanged(object sender, EventArgs e) //Load Image từ File lên giao diện
        {
            ImageContener IC = (sender as ImageContener);
            for (int i = 0; i < Data._fileData.Count; i++)
            {
                FileData fdata = Data._fileData[i];
                if (fdata.drField["sysFieldID"].ToString() == IC.drField["sysFieldID"].ToString())
                {
                    Data._fileData.Remove(fdata);
                    i--;
                }
            }
            Data._fileData.Add(new FileData(IC.drField, IC.EditValue as byte[], true));

        }

        

        void fC_LoadnewData(object sender, EventArgs e)
        {
            fileContener fC = (sender as fileContener);
            for (int i = 0; i < Data._fileData.Count; i++)
            {
                FileData fdata = Data._fileData[i];
                if (fdata.drField["sysFieldID"].ToString() == fC.drField["sysFieldID"].ToString())
                {
                    Data._fileData.Remove(fdata);
                    i--;
                }
            }

            Data._fileData.Add(new FileData(fC.drField, fC.data, true));
        }

        void tmp_EditValueChanged(object sender, EventArgs e)
        {
            if (sender as CalcEdit == null) return;
            CalcEdit tmp = sender as CalcEdit;
            DataRow dr = tmp.Tag as DataRow;


            if (tmp == null || string.IsNullOrEmpty(tmp.Properties.EditMask))
                return;

            // Lấy số lượng chữ số thập phân từ EditMask
            int decimalPlaces = GetDecimalPlacesFromMask(tmp.Properties.EditMask);

            if (tmp.EditValue is decimal currentValue)
            {
                // Làm tròn giá trị
                decimal roundedValue = Math.Round(currentValue, decimalPlaces);
                // Cập nhật giá trị đã làm tròn
                tmp.EditValue = roundedValue;
            }


        }

        public BaseEdit GenControl(DataRow dr)
        {
            BaseEdit tmp;
            string dataMember = dr["FieldName"].ToString();
            int pType = int.Parse(dr["Type"].ToString());
            switch (pType)
            {
                case 0:
                    tmp = new VTextEdit();
                    (tmp as VTextEdit).EnterMoveNextControl = true;
                    (tmp as VTextEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    (tmp as VTextEdit).Properties.MaxLength = 0x20;
                    (tmp as VTextEdit).Properties.CharacterCasing = CharacterCasing.Upper;
                    break;

                case 1:
                case 4:
                case 7:
                    {
                        tmp = this.GenGridLookupEdit(dr, false);
                        CDTGridLookUpEdit tmpGrd = tmp as CDTGridLookUpEdit;
                        tmpGrd.Properties.CloseUpKey = KeyShortcut.Empty;
                        tmpGrd.EnterMoveNextControl = true;
                        tmpGrd.Properties.NullText = string.Empty;
                        tmpGrd.Properties.ImmediatePopup = true;
                        tmpGrd.Properties.AllowNullInput = DefaultBoolean.True;
                        tmpGrd.Properties.View.OptionsView.ShowAutoFilterRow = true;
                        tmpGrd.Properties.View.OptionsView.ColumnAutoWidth = false;
                        tmpGrd.Properties.View.OptionsView.ShowGroupedColumns = false;
                        if ((tmpGrd.DymicCondition != "") && (this.formAction != FormAction.Filter))
                        {
                            this._lstRep.Add((CDTRepGridLookup)tmpGrd.Properties);
                        }
                        break;
                    }
                case 2:
                    tmp = new VTextEdit();
                    (tmp as VTextEdit).EnterMoveNextControl = true;
                    (tmp as VTextEdit).Properties.MaxLength = 0xff;
                    (tmp as VTextEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    break;

                case 5:
                    tmp = new VSpinEdit();
                    (tmp as VSpinEdit).EnterMoveNextControl = true;
                    (tmp as VSpinEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    break;

                case 8:
                    tmp = new VCalcEdit();
                    (tmp as VCalcEdit).EnterMoveNextControl = true;
                    (tmp as VCalcEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    (tmp as VCalcEdit).Spin += new SpinEventHandler(this.FormDesigner_Spin1);
                    (tmp as VCalcEdit).KeyUp += new KeyEventHandler(this.VCalEdit_KeyUp);
                    if (dr["EditMask"].ToString() != string.Empty)
                    {
                        (tmp as VCalcEdit).Properties.EditMask = dr["EditMask"].ToString();
                        (tmp as VCalcEdit).Properties.Mask.UseMaskAsDisplayFormat = true;
                    }
                    break;

                case 9:
                    tmp = new VDateEdit();
                    break;

                case 10:
                    tmp = new VCheckEdit();
                    break;

                case 11:
                    tmp = new TimeEdit();
                    (tmp as TimeEdit).EnterMoveNextControl = true;
                    (tmp as TimeEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    break;

                case 12:
                    tmp = new PictureEdit();
                    if (Config.GetValue("Language").ToString() == "0")
                    {
                        tmp.Properties.NullText = "Click phải chuột chọn nạp h\x00ecnh";
                    }
                    tmp.DataBindings.Add("EditValue", this._bindingSource, dataMember, true, DataSourceUpdateMode.OnValidation);
                    break;

                case 13:
                    tmp = new MemoEdit();
                    break;

                case 14:
                    tmp = new VDateEdit();
                    (tmp as VDateEdit).Properties.EditMask = "dd/MM/yyyy HH:mm:ss";
                    break;
                case 15:
                    tmp = new VTextEdit();
                    (tmp as VTextEdit).EnterMoveNextControl = true;
                    (tmp as VTextEdit).Properties.MaxLength = 0xff;
                    (tmp as VTextEdit).Properties.AllowNullInput = DefaultBoolean.True;
                    break;
                case 16:

                    fileContener fC = new fileContener(dr);
                    fC.DataBindings.Add("Text", this._bindingSource, dataMember, false, DataSourceUpdateMode.OnValidation);
                    fC.LoadnewData += new EventHandler(fC_LoadnewData);
                    _lFileContener.Add(fC);
                    tmp = fC as BaseEdit;
                    break;
                default:
                    tmp = null;
                    break;
            }
            if (tmp != null)
            {
                tmp.Name = dr["FieldName"].ToString();
                tmp.ToolTip = dr["Tip"].ToString();
                if ((pType != 12 && pType != 16) && !((this._formAction == FormAction.Filter) && bool.Parse(dr["IsBetween"].ToString())))
                {
                    tmp.DataBindings.Add("EditValue", this._bindingSource, dataMember, false, DataSourceUpdateMode.OnValidation);
                }
                if (int.Parse(dr["TabIndex"].ToString()) >= 0)
                    tmp.TabIndex = int.Parse(dr["TabIndex"].ToString());
                bool admin = bool.Parse(Config.GetValue("Admin").ToString());

                if (!admin)
                {
                    string canEdit1 = dr["Editable1"].ToString();
                    // string canEdit = dr["Editable"].ToString();
                    if (canEdit1 != string.Empty)
                    {
                        tmp.Properties.ReadOnly = !bool.Parse(canEdit1);
                    }
                }
            }
            return tmp;
        }

        private CDTGridColumn GenGridColumn(DataRow dr, int exColNum, bool checkData)
        {
            CDTGridColumn gcl = new CDTGridColumn();

            gcl.Name = "cl" + dr["FieldName"].ToString();
            gcl.FieldName = dr["FieldName"].ToString();
            if (dr["fieldName"].ToString().ToUpper() == "MAXVALUE")
            {
            }
            string caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
            int formType = int.Parse(this._data.DrTable["Type"].ToString());
            if (!(((formType != 1) && (formType != 4)) || dr["AllowNull"].ToString() == "0"))
            {
                caption = "*" + caption;
            }
            gcl.Caption = caption;
            
            gcl.ToolTip = " Tên trường: " + dr["fieldName"].ToString();
            if (dr["Formula"] != DBNull.Value)
                gcl.ToolTip += "\n Công thức: " + dr["Formula"].ToString();
            if (dr["FormulaDetail"] != DBNull.Value)
                gcl.ToolTip +=  "\n Lấy giá trị từ trường: " + dr["FormulaDetail"].ToString();
            if (dr["refTable"] != DBNull.Value)
                gcl.ToolTip += "\n Tham chiếu đến báng: " + dr["refTable"].ToString();
            if (dr["DynCriteria"] != DBNull.Value)
                gcl.ToolTip += "\n Lọc những dòng thỏa điều kiện: " + dr["DynCriteria"].ToString();
            gcl.ToolTip +="\n " +  ((Config.GetValue("Language").ToString() == "0") ? dr["Tip"].ToString() : dr["TipE"].ToString());
            gcl.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            gcl.AppearanceHeader.Options.UseTextOptions = true;
            if (dr["TabIndex"] == DBNull.Value) 
                dr["TabIndex"] = -1;
            if (int.Parse(dr["TabIndex"].ToString()) != -1)
            {
                gcl.VisibleIndex = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                gcl.IndexVisible = int.Parse(dr["TabIndex"].ToString()) + exColNum;
            }
            else gcl.VisibleIndex = -1;
            gcl.MasterRow = dr;
            gcl.refFilter = dr["DynCriteria"].ToString();
            if (!checkData)
            {
                // gcl.Visible = dr["Visible"].ToString()=="1";
            }
            if (bool.Parse(dr["IsFixCol"].ToString()))
            {
                gcl.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            if (bool.Parse(dr["IsGroupCol"].ToString()))
            {
                gcl.GroupIndex = 0;
            }
            int pType = int.Parse(dr["Type"].ToString());
            if (!(checkData || (pType != 3)))
            {
                gcl.Visible = false;
            }
            else gcl.Visible = true;
            if (pType == 1)
            {
                gcl.Width += 20;
                
            }
            if (pType == 2)
            {
                gcl.Width += 150;
            }
            if (pType == 3)
            {
                gcl.Width -= 40;
            }
            if (pType == 15)
            {
                gcl.Width += 150;
            }
            if (pType == 9)
            {
                gcl.DisplayFormat.FormatType = FormatType.DateTime;
                gcl.DisplayFormat.FormatString = "dd/MM/yyyy";
            }
            if (pType == 14)
            {
                gcl.DisplayFormat.FormatType = FormatType.DateTime;

                RepositoryItemDateEdit dEdit = new RepositoryItemDateEdit();
                gcl.ColumnEdit = dEdit;

                dEdit.EditFormat.FormatType = FormatType.DateTime;
                if (dr["EditMask"].ToString() == string.Empty)
                {
                    gcl.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                    dEdit.EditMask = "dd/MM/yyyy HH:mm:ss";
                    dEdit.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                }
                else
                {
                    gcl.DisplayFormat.FormatString = dr["EditMask"].ToString();
                    dEdit.EditMask = dr["EditMask"].ToString();
                    dEdit.EditFormat.FormatString = dr["EditMask"].ToString();
                }
                gcl.Width += 50;
            }
            if (pType == 8)
            {
                string f;

                if (dr["EditMask"].ToString() == string.Empty)
                    f = "";
                else
                    f = ":" + dr["EditMask"].ToString();
                gcl.DisplayFormat.FormatType = FormatType.Numeric;
                gcl.DisplayFormat.FormatString = dr["EditMask"].ToString();
                gcl.SummaryItem.Assign(new GridSummaryItem(DevExpress.Data.SummaryItemType.Sum, dr["FieldName"].ToString(), "{0" + f + "}"));
                //gcl.Width += 30;
            }
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (dr.Table.Columns.Contains("ColWidth") && dr["ColWidth"] != DBNull.Value)
            {
                gcl.Width = int.Parse(dr["ColWidth"].ToString());
            }
            if (dr["Visible"].ToString() == "0")
            {
                gcl.Visible = false;
            }

            if (!admin)
            {
                string canEdit1 = dr["Editable1"].ToString();
                //string canEdit = dr["Editable"].ToString();
                if (canEdit1 != string.Empty)
                {
                    gcl.OptionsColumn.AllowEdit = bool.Parse(canEdit1);
                }
            }
            return gcl;
        }
        private CDTBandGridColumn GenBandGridColumn(DataRow dr, int exColNum, bool checkData)
        {
            CDTBandGridColumn gcl = new CDTBandGridColumn();
            if (dr["FieldName"].ToString().ToLower() == "makhthue")
            {
            }
            gcl.Name = "cl" + dr["FieldName"].ToString();
            gcl.FieldName = dr["FieldName"].ToString();
            if (dr["fieldName"].ToString().ToUpper() == "MAXVALUE")
            {
            }
            string caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
            int formType = int.Parse(this._data.DrTable["Type"].ToString());
            if (!(((formType != 1) && (formType != 4)) || dr["AllowNull"].ToString() == "0"))
            {
                caption = "*" + caption;
            }
            gcl.Caption = caption;
            gcl.ToolTip = dr["Tip"].ToString();
            gcl.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            gcl.AppearanceHeader.Options.UseTextOptions = true;
            if (int.Parse(dr["TabIndex"].ToString()) != -1)
            {
                gcl.VisibleIndex = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                gcl.IndexVisible = int.Parse(dr["TabIndex"].ToString()) + exColNum;
            }
            else gcl.VisibleIndex = -1;
            gcl.MasterRow = dr;
            gcl.refFilter = dr["DynCriteria"].ToString();
            if (!checkData)
            {
                // gcl.Visible = dr["Visible"].ToString()=="1";
            }
            if (bool.Parse(dr["IsFixCol"].ToString()))
            {
                gcl.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            if (bool.Parse(dr["IsGroupCol"].ToString()))
            {
                gcl.GroupIndex = 0;
            }
            int pType = int.Parse(dr["Type"].ToString());
            if (!(checkData || (pType != 3)))
            {
                gcl.Visible = false;
            }
            else gcl.Visible = true;
            if (pType == 1)
            {
                gcl.Width += 20;
            }
            if (pType == 2)
            {
                gcl.Width += 150;
            }
            if (pType == 3)
            {
                gcl.Width -= 40;
            }
            if (pType == 15)
            {
                gcl.Width += 150;
            }
            if (pType == 9)
            {
                gcl.DisplayFormat.FormatType = FormatType.DateTime;
                gcl.DisplayFormat.FormatString = "dd/MM/yyyy";
            }
            if (pType == 13 || pType == 12)
            {
                gcl.RowCount = 1;

            }
            if (pType == 14)
            {
                gcl.DisplayFormat.FormatType = FormatType.DateTime;

                RepositoryItemDateEdit dEdit = new RepositoryItemDateEdit();
                gcl.ColumnEdit = dEdit;

                dEdit.EditFormat.FormatType = FormatType.DateTime;
                if (dr["EditMask"].ToString() == string.Empty)
                {
                    gcl.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                    dEdit.EditMask = "dd/MM/yyyy HH:mm:ss";
                    dEdit.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                }
                else
                {
                    gcl.DisplayFormat.FormatString = dr["EditMask"].ToString();
                    dEdit.EditMask = dr["EditMask"].ToString();
                    dEdit.EditFormat.FormatString = dr["EditMask"].ToString();
                }
                gcl.Width += 50;
            }
            if (pType == 8)
            {
                string f;

                if (dr["EditMask"].ToString() == string.Empty)
                    f = "";
                else
                    f = ":" + dr["EditMask"].ToString();
                gcl.DisplayFormat.FormatType = FormatType.Numeric;
                gcl.DisplayFormat.FormatString = dr["EditMask"].ToString();
                gcl.SummaryItem.Assign(new GridSummaryItem(DevExpress.Data.SummaryItemType.Sum, dr["FieldName"].ToString(), "{0" + f + "}"));
                //gcl.Width += 30;
            }
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (dr.Table.Columns.Contains("ColWidth") && dr["ColWidth"] != DBNull.Value)
            {
                gcl.Width = int.Parse(dr["ColWidth"].ToString());
            }
            if (dr["Visible"].ToString() == "0")
            {
                gcl.Visible = false;
            }

            if (!admin)
            {
                string canEdit1 = dr["Editable1"].ToString();
                //string canEdit = dr["Editable"].ToString();
                if (canEdit1 != string.Empty)
                {
                    gcl.OptionsColumn.AllowEdit = bool.Parse(canEdit1);
                }
            }

            return gcl;
        }
        public GridControl GenGridControl(DataTable dt, bool isEdit, DockStyle ds)
        {
            GridControl gcMain = new GridControl();
            GridView gvMain = new GridView();
            gcMain.BeginInit();
            gvMain.BeginInit();
            gcMain.Dock = ds;
            gcMain.SendToBack();
            gcMain.MainView = gvMain;
            gcMain.ViewCollection.AddRange(new BaseView[] { gvMain });
            gvMain.OptionsView.ShowFooter = true;
            gvMain.GridControl = gcMain;
            gvMain.Appearance.HideSelectionRow.ForeColor = Color.Blue;
            gvMain.Appearance.HideSelectionRow.Options.UseForeColor = true;
            gvMain.Appearance.FocusedRow.ForeColor = Color.Blue;
            gvMain.Appearance.FocusedRow.Options.UseForeColor = true;
            gvMain.Appearance.FocusedRow.BackColor = Color.Azure;
            gvMain.Appearance.FocusedRow.Options.UseBackColor = true;
            gvMain.Appearance.SelectedRow.Options.UseForeColor = true;
            gvMain.Appearance.SelectedRow.ForeColor = Color.Blue;
            gvMain.Appearance.SelectedRow.Options.UseBackColor = true;
            gvMain.Appearance.SelectedRow.BackColor = Color.AliceBlue;
            gvMain.OptionsDetail.AllowExpandEmptyDetails = true;
            gvMain.AppearancePrint.EvenRow.BackColor = System.Drawing.Color.FloralWhite;
            gvMain.AppearancePrint.EvenRow.BackColor2 = System.Drawing.Color.OldLace;
            gvMain.AppearancePrint.EvenRow.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            gvMain.AppearancePrint.EvenRow.Options.UseBackColor = true;
            gvMain.AppearancePrint.FooterPanel.BackColor = System.Drawing.Color.PowderBlue;
            gvMain.AppearancePrint.FooterPanel.Options.UseBackColor = true;
            gvMain.AppearancePrint.GroupFooter.BackColor = System.Drawing.Color.Azure;
            gvMain.AppearancePrint.GroupFooter.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            gvMain.AppearancePrint.GroupFooter.Options.UseBackColor = true;
            gvMain.AppearancePrint.GroupRow.BackColor = System.Drawing.Color.SkyBlue;
            gvMain.AppearancePrint.GroupRow.BackColor2 = System.Drawing.Color.AntiqueWhite;
            gvMain.AppearancePrint.GroupRow.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            gvMain.AppearancePrint.GroupRow.Options.UseBackColor = true;
            gvMain.AppearancePrint.HeaderPanel.BackColor = System.Drawing.Color.AliceBlue;
            gvMain.AppearancePrint.HeaderPanel.BackColor2 = System.Drawing.Color.AntiqueWhite;
            gvMain.AppearancePrint.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            gvMain.AppearancePrint.HeaderPanel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            gvMain.AppearancePrint.HeaderPanel.Options.UseBackColor = true;
            gvMain.AppearancePrint.HeaderPanel.Options.UseFont = true;
            gvMain.AppearancePrint.OddRow.BackColor = System.Drawing.Color.Azure;
            gvMain.AppearancePrint.OddRow.BackColor2 = System.Drawing.Color.White;
            gvMain.AppearancePrint.OddRow.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            gvMain.AppearancePrint.OddRow.Options.UseBackColor = true;
            gvMain.OptionsPrint.EnableAppearanceEvenRow = true;
            gvMain.OptionsPrint.EnableAppearanceOddRow = true;
            gvMain.OptionsPrint.ExpandAllDetails = true;
            gvMain.OptionsPrint.PrintDetails = true;
            gvMain.OptionsPrint.UsePrintStyles = true;
            //gvMain.GridControl.UseEmbeddedNavigator = true;

            if (Config.GetValue("Language").ToString() == "0")
            {
                gvMain.GroupPanelText = "Bảng nhóm: kéo thả một cột vào đây để nhóm số liệu";
            }
            gvMain.OptionsView.ColumnAutoWidth = false;
            gvMain.OptionsView.EnableAppearanceEvenRow = true;
            gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsBehavior.Editable = false;
            gvMain.OptionsView.ShowAutoFilterRow = true;
            gvMain.OptionsNavigation.EnterMoveNextColumn = true;
            gvMain.IndicatorWidth = 40;
            //gvMain.OptionsView.ShowDetailButtons = false;
            gvMain.OptionsBehavior.AutoExpandAllGroups = true;
            gvMain.OptionsNavigation.AutoFocusNewRow = true;
            // gvMain.BestFitColumns();

            gvMain.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);
            gcMain.KeyUp += new KeyEventHandler(gcMain_KeyUp);
            gvMain.OptionsCustomization.AllowRowSizing = true;
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvMain_CellValueChanged);
            gvMain.CustomColumnDisplayText += GvMain_CustomColumnDisplayText;
            gvMain.OptionsBehavior.Editable = true;
            gvMain.OptionsBehavior.ReadOnly = true;
            if (isEdit)
            {
                gvMain.FocusedRowChanged += new FocusedRowChangedEventHandler(this.gvMain_FocusedRowChanged);
                gcMain.KeyDown += new KeyEventHandler(this.gcMain_KeyDown);
                gvMain.OptionsBehavior.Editable = true;
                gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                gvMain.OptionsView.ShowFooter = true;
                gvMain.OptionsCustomization.AllowRowSizing = true;                
                gvMain.OptionsBehavior.ReadOnly = false;
                // gvMain.RowHeight = 50;
            }

            gvMain.RowCountChanged += new EventHandler(gvMain_RowCountChanged);
            int exColNum = 0;
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            DateTime t1 = DateTime.Now;
            DateTime tx = DateTime.Now;
            string s = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                DataRow dr = dt.Rows[i];
                string viewable = dr["Viewable"].ToString();
                if (dr["FieldName"].ToString() == "MaSP")
                {

                }
                if ((admin || !(viewable != string.Empty)) || bool.Parse(viewable))
                {
                    CDTGridColumn gcl = this.GenGridColumn(dr, exColNum, false);
                    RepositoryItem ri = this.GenRepository(dr);
                    //int type = int.Parse(dr["Type"].ToString());
                    //int[] NoRi = new int[] { 0, 3, 6,15 ,16};
                    //if (ri == null && !NoRi.Contains(type))
                    //    return null;
                    if (ri != null)
                    {
                        gcMain.RepositoryItems.Add(ri);
                        gcl.ColumnEdit = ri;
                        CDTRepGridLookup CDTRi = ri as CDTRepGridLookup;
                        if (CDTRi != null)
                        {
                            CDTRi.MainView = gvMain;
                            CDTRi.MainStruct = dt;
                        }
                    }
                    gvMain.Columns.Add(gcl);
                    if (dr["Visible"].ToString() != "0")
                    {
                        gcl.VisibleIndex = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                        gcl.IndexVisible = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                    }

                    else
                        gcl.VisibleIndex = -1;
                    gcl.ToolTip += "\n" + gcl.VisibleIndex.ToString();
                    gcl.Tag = false;
                    int pType = int.Parse(dr["Type"].ToString());
                    if (pType == 12)
                    {
                       // gvMain.OptionsView.RowAutoHeight = true;
                    }
                    if ((pType == 1) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        CDTGridColumn gcl1 = this.GenGridColumn(dr, exColNum, false);
                        gcl1.GroupIndex = -1;
                        gcl1.isExCol = true;
                        gcl1.Tag = true;
                        RepositoryItem ri1 = this.GenRepository(dr);
                        CDTRepGridLookup CDTRi1 = ri1 as CDTRepGridLookup;
                        if (CDTRi1 != null)
                        {
                            CDTRi1.MainView = gvMain;
                            CDTRi1.MainStruct = dt;
                        }
                        if (ri1 != null)
                        {
                            string caption;
                            string displayMember = dr["DisplayMember"].ToString();
                            ((CDTRepGridLookup)ri1).DisplayMember = displayMember;
                            gcMain.RepositoryItems.Add(ri1);
                            if (Config.GetValue("Language").ToString() == "0")
                            {
                                caption = "Tên " + dr["LabelName"].ToString().ToLower();
                            }
                            else
                            {
                                caption = dr["LabelName2"].ToString() + " name";
                            }
                            int formType = int.Parse(this._data.DrTable["Type"].ToString());
                            if (!(((formType != 1) && (formType != 4)) || (dr["AllowNull"].ToString() == "0")))
                            {
                                caption = "*" + caption;
                            }
                            gcl1.Caption = caption;
                            gcl1.VisibleIndex++;
                            gcl1.Width += 150;
                            gcl1.ColumnEdit = ri1;
                            gcl1.Visible = gcl.Visible;
                            if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString()))
                            {
                                gcl.VisibleIndex = -1;
                                gcl.IndexVisible = -1;
                            }
                            gcl1.ToolTip += "\n" + gcl1.VisibleIndex.ToString();
                        }
                        gvMain.Columns.Add(gcl1);
                        exColNum++;
                    }
                    if (gcl.GroupIndex >= 0)
                    {
                        gvMain.GroupSummary.Add(new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, gcl.FieldName, null, "({0} mục)"));
                    }
                }
                DateTime t2 = DateTime.Now;
                TimeSpan t = t2 - t1;
                s += "\n" + dr["fieldName"].ToString() + " " + t.TotalMilliseconds.ToString();
                t1 = t2;
            }
           // LogFile.AppendToFile("log.txt", s);
            TimeSpan tt = DateTime.Now - tx;
            s = "\n" + " Tong cong " + tt.TotalMilliseconds.ToString();
            //LogFile.AppendToFile("log.txt", s);
            gcMain.EndInit();
            gvMain.EndInit();
            this._gcMain = gcMain;
            return gcMain;
    
        }

        private void GvMain_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if ((e.Column.FieldName.ToLower() == "soserie" || e.Column.FieldName.ToLower() == "soseries" || e.Column.FieldName.ToLower() == "soseri") && e.Value != null)
            {
                // Chuyển đổi giá trị của ô thành chữ hoa
                e.DisplayText = e.Value.ToString().ToUpper();
            }
        }

        public GridControl GenBandGridControl(DataTable dt, DataTable dtband, bool isEdit, DockStyle ds)
        {
            GridControl gcMain = new GridControl();
            AdvBandedGridView gvMain = new AdvBandedGridView();
            gcMain.BeginInit();
            gvMain.BeginInit();
            gcMain.Dock = ds;
            gcMain.SendToBack();
            gcMain.MainView = gvMain;
            gcMain.ViewCollection.AddRange(new BaseView[] { gvMain });
            gvMain.OptionsView.ShowFooter = true;
            gvMain.GridControl = gcMain;
            gvMain.Appearance.HideSelectionRow.ForeColor = Color.Blue;
            gvMain.Appearance.HideSelectionRow.Options.UseForeColor = true;
            gvMain.Appearance.FocusedRow.ForeColor = Color.Blue;
            gvMain.Appearance.FocusedRow.Options.UseForeColor = true;
            gvMain.Appearance.FocusedRow.BackColor = Color.Azure;
            gvMain.Appearance.FocusedRow.Options.UseBackColor = true;
            gvMain.Appearance.SelectedRow.Options.UseForeColor = true;
            gvMain.Appearance.SelectedRow.ForeColor = Color.Blue;
            gvMain.Appearance.SelectedRow.Options.UseBackColor = true;
            gvMain.Appearance.SelectedRow.BackColor = Color.AliceBlue;
            if (Config.GetValue("Language").ToString() == "0")
            {
                gvMain.GroupPanelText = "Bảng nhóm: kéo thả một cột vào đây để nhóm số liệu";
            }
            gvMain.OptionsView.ColumnAutoWidth = false;
            gvMain.OptionsView.EnableAppearanceEvenRow = true;
            gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsBehavior.Editable = false;
            gvMain.OptionsView.ShowAutoFilterRow = true;
            gvMain.OptionsNavigation.EnterMoveNextColumn = true;
            gvMain.IndicatorWidth = 40;
            //gvMain.OptionsView.ShowDetailButtons = false;
            gvMain.OptionsBehavior.AutoExpandAllGroups = true;
            gvMain.OptionsNavigation.AutoFocusNewRow = true;
            // gvMain.BestFitColumns();
            gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            gvMain.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);
            gcMain.KeyUp += new KeyEventHandler(gcMain_KeyUp);
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvMain_CellValueChanged);

            gvMain.BandPanelRowHeight = 1;
            if (isEdit)
            {
                gvMain.FocusedRowChanged += new FocusedRowChangedEventHandler(this.gvMain_FocusedRowChanged);
                gcMain.KeyDown += new KeyEventHandler(this.gcMain_KeyDown);
                gvMain.OptionsBehavior.Editable = true;
                gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                gvMain.OptionsView.ShowFooter = true;
                gvMain.OptionsCustomization.AllowRowSizing = true;
            }

            gvMain.RowCountChanged += new EventHandler(gvMain_RowCountChanged);
            int exColNum = 0;
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            DateTime t1 = DateTime.Now;
            DateTime tx = DateTime.Now;
            string s = "";
            foreach (DataRow dr in dtband.Rows)
            {
                GridBand gb = new GridBand();
                gb.Name = dr["sysBandID"].ToString();
                gb.Caption = dr["Caption"].ToString();
                gb.Width = int.Parse(dr["Width"].ToString());
                gvMain.Bands.Add(gb);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                DataRow dr = dt.Rows[i];
                string viewable = dr["Viewable"].ToString();

                if ((admin || !(viewable != string.Empty)) || bool.Parse(viewable))
                {
                    CDTBandGridColumn gcl = this.GenBandGridColumn(dr, exColNum, false);
                    if (dr["sysBandID"] != DBNull.Value)
                    {
                        gvMain.Bands[dr["sysBandID"].ToString()].Columns.Add(gcl);
                    }
                    RepositoryItem ri = this.GenRepository(dr);

                    if (ri != null)
                    {
                        gcMain.RepositoryItems.Add(ri);
                        gcl.ColumnEdit = ri;
                        CDTRepGridLookup CDTRi = ri as CDTRepGridLookup;
                        if (CDTRi != null)
                        {
                            CDTRi.MainView = gvMain;
                            CDTRi.MainStruct = dt;
                        }
                    }
                    gvMain.Columns.Add(gcl);
                    if (dr["Visible"].ToString() != "0")
                    {
                        gcl.VisibleIndex = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                        gcl.IndexVisible = int.Parse(dr["TabIndex"].ToString()) + exColNum;
                    }
                    else
                        gcl.VisibleIndex = -1;
                    
                    int pType = int.Parse(dr["Type"].ToString());
                    if (pType == 12)
                    {
                       // gvMain.OptionsView.RowAutoHeight = true;
                    }
                    if ((pType == 1) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        CDTBandGridColumn gcl1 = this.GenBandGridColumn(dr, exColNum, false);
                        gcl1.GroupIndex = -1;
                        gcl1.isExCol = true;
                        RepositoryItem ri1 = this.GenRepository(dr);
                        CDTRepGridLookup CDTRi1 = ri1 as CDTRepGridLookup;
                        if (CDTRi1 != null)
                        {
                            CDTRi1.MainView = gvMain;
                            CDTRi1.MainStruct = dt;
                        }
                        if (ri1 != null)
                        {
                            string caption;
                            string displayMember = dr["DisplayMember"].ToString();
                            ((CDTRepGridLookup)ri1).DisplayMember = displayMember;
                            gcMain.RepositoryItems.Add(ri1);
                            if (Config.GetValue("Language").ToString() == "0")
                            {
                                caption = "Tên " + dr["LabelName"].ToString().ToLower();
                            }
                            else
                            {
                                caption = dr["LabelName2"].ToString() + " name";
                            }
                            int formType = int.Parse(this._data.DrTable["Type"].ToString());
                            if (!(((formType != 1) && (formType != 4)) || (dr["AllowNull"].ToString() == "0")))
                            {
                                caption = "*" + caption;
                            }
                            gcl1.Caption = caption;
                            gcl1.VisibleIndex++;
                            gcl1.Width += 150;
                            gcl1.ColumnEdit = ri1;
                            gcl1.Visible = gcl.Visible;
                            if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString())) gcl.VisibleIndex = -1;
                        }
                        gvMain.Columns.Add(gcl1);
                        exColNum++;
                    }
                    if (gcl.GroupIndex >= 0)
                    {
                        gvMain.GroupSummary.Add(new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, gcl.FieldName, null, "({0} mục)"));
                    }
                }
                DateTime t2 = DateTime.Now;
                TimeSpan t = t2 - t1;
                s += "\n" + dr["fieldName"].ToString() + " " + t.TotalMilliseconds.ToString();
                t1 = t2;
            }
           // LogFile.AppendToFile("log.txt", s);
            TimeSpan tt = DateTime.Now - tx;
            s = "\n" + " Tong cong " + tt.TotalMilliseconds.ToString();
           // LogFile.AppendToFile("log.txt", s);
            gcMain.EndInit();
            gvMain.EndInit();
            this._gcMain = gcMain;
            return gcMain;
        }
        void gvMain_RowCountChanged(object sender, EventArgs e)
        {

        }

        void gcMain_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public GridControl GenGridControlDt(DataTable dt, string lstField, bool isEdit, DockStyle ds)
        {
            GridControl gcMain = new GridControl();
            GridView gvMain = new GridView();
            gcMain.BeginInit();
            gvMain.BeginInit();
            gcMain.Dock = ds;
            gcMain.SendToBack();
            gcMain.MainView = gvMain;
            gcMain.ViewCollection.AddRange(new BaseView[] { gvMain });
            gvMain.OptionsView.ShowFooter = true;
            gvMain.GridControl = gcMain;
            if (Config.GetValue("Language").ToString() == "0")
            {
                gvMain.GroupPanelText = "Bảng nhóm: kéo thả một cột vào đây để nhóm số liệu";
            }
            gvMain.OptionsView.ColumnAutoWidth = false;
            gvMain.OptionsView.EnableAppearanceEvenRow = true;
            gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsBehavior.Editable = false;
            gvMain.OptionsView.ShowAutoFilterRow = true;
            gvMain.OptionsNavigation.EnterMoveNextColumn = true;
            gvMain.IndicatorWidth = 40;
            gvMain.OptionsView.ShowDetailButtons = false;
           gvMain.OptionsBehavior.AutoExpandAllGroups = true;
            gvMain.OptionsNavigation.AutoFocusNewRow = true;
            gcMain.KeyUp += new KeyEventHandler(gcMain_KeyUp);
            gvMain.CustomColumnDisplayText += GvMain_CustomColumnDisplayText;
            gvMain.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);

            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvMain_CellValueChanged);
            if (isEdit)
            {
                gvMain.FocusedRowChanged += new FocusedRowChangedEventHandler(this.gvMain_FocusedRowChanged);
                gcMain.KeyDown += new KeyEventHandler(this.gcMain_KeyDown);
                gvMain.OptionsBehavior.Editable = true;
                gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                gvMain.OptionsCustomization.AllowRowSizing = true;
            }
            int exColNum = 0;
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string viewable = dr["Viewable"].ToString();
                if ((admin || !(viewable != string.Empty)) || bool.Parse(viewable))
                {
                    CDTGridColumn gcl = this.GenGridColumn(dr, exColNum, false);

                    RepositoryItem ri = this.GenRepository(dr);

                    if (ri != null)
                    {
                        CDTRepGridLookup CDTRi = ri as CDTRepGridLookup;
                        if (CDTRi != null)
                        {
                            CDTRi.MainView = gvMain;
                            CDTRi.MainStruct = dt;
                        }
                        gcMain.RepositoryItems.Add(ri);
                        gcl.ColumnEdit = ri;
                    }
                    gcl.Visible = gcl.Visible && lstField.ToLower().Contains(gcl.FieldName.ToLower() + ",");
                    gcl.Tag = false;
                    gvMain.Columns.Add(gcl);
                    int pType = int.Parse(dr["Type"].ToString());
                    if (pType == 12)
                    {
                        gvMain.OptionsView.RowAutoHeight = true;
                    }
                    if ((pType == 1) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        CDTGridColumn gcl1 = this.GenGridColumn(dr, exColNum, false);
                        gcl1.GroupIndex = -1;
                        gcl1.Visible = gcl1.Visible && lstField.ToLower().Contains(gcl.FieldName.ToLower() + ",");
                        gcl1.isExCol = true;
                        gcl1.Tag = true;
                        RepositoryItem ri1 = this.GenRepository(dr);
                        CDTRepGridLookup CDTRi1 = ri1 as CDTRepGridLookup;
                        if (CDTRi1 != null)
                        {
                            CDTRi1.MainView = gvMain;
                            CDTRi1.MainStruct = dt;
                        }
                        if (ri1 != null)
                        {
                            string caption;
                            string displayMember = dr["DisplayMember"].ToString();
                            ((CDTRepGridLookup)ri1).DisplayMember = displayMember;
                            gcMain.RepositoryItems.Add(ri1);
                            if (Config.GetValue("Language").ToString() == "0")
                            {
                                caption = "Tên " + dr["LabelName"].ToString().ToLower();
                            }
                            else
                            {
                                caption = dr["LabelName2"].ToString() + " name";
                            }
                            int formType = int.Parse(this._data.DrTable["Type"].ToString());
                            if (!(((formType != 1) && (formType != 4)) || bool.Parse(dr["AllowNull"].ToString())))
                            {
                                caption = "*" + caption;
                            }
                            gcl1.Caption = caption;
                            gcl1.VisibleIndex++;
                            gcl1.Width = gcl.Width*2;
                            gcl1.ColumnEdit = ri1;
                            gcl1.Visible = gcl.Visible;
                            if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString()))
                                gcl.VisibleIndex = -1;
                            
                        }
                        gvMain.Columns.Add(gcl1);
                        exColNum++;
                    }
                    if (gcl.GroupIndex >= 0)
                    {
                        gvMain.GroupSummary.Add(new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, gcl.FieldName, null, "({0} mục)"));
                    }
                }
            }
            gcMain.EndInit();
            gvMain.EndInit();
            return gcMain;
        }
        public GridControl GenBandGridControlDt(DataTable dt, DataTable dtband, string lstField, bool isEdit, DockStyle ds)
        {
            GridControl gcMain = new GridControl();

            AdvBandedGridView gvMain = new AdvBandedGridView();
            gcMain.BeginInit();
            gvMain.BeginInit();
            gcMain.Dock = ds;
            gcMain.SendToBack();
            gcMain.MainView = gvMain;
            gcMain.ViewCollection.AddRange(new BaseView[] { gvMain });
            //gvMain.OptionsView.ShowFooter = true;
            gvMain.GridControl = gcMain;
            if (Config.GetValue("Language").ToString() == "0")
            {
                gvMain.GroupPanelText = "Bảng nhóm: kéo thả một cột vào đây để nhóm số liệu";
            }
            gvMain.OptionsView.ColumnAutoWidth = false;
            gvMain.OptionsView.EnableAppearanceEvenRow = true;
            gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsBehavior.Editable = false;
            // gvMain.OptionsView.ShowAutoFilterRow = true;
            gvMain.OptionsNavigation.EnterMoveNextColumn = true;
            gvMain.IndicatorWidth = 40;
            gvMain.OptionsView.ShowDetailButtons = false;
            gvMain.OptionsBehavior.AutoExpandAllGroups = true;
            gvMain.OptionsNavigation.AutoFocusNewRow = true;
            gcMain.KeyUp += new KeyEventHandler(gcMain_KeyUp);
            gvMain.OptionsDetail.AllowExpandEmptyDetails = true;
            gvMain.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);

            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvMain_CellValueChanged);
            if (isEdit)
            {
                gvMain.FocusedRowChanged += new FocusedRowChangedEventHandler(this.gvMain_FocusedRowChanged);
                gcMain.KeyDown += new KeyEventHandler(this.gcMain_KeyDown);
                gvMain.OptionsBehavior.Editable = true;
                gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                gvMain.OptionsCustomization.AllowRowSizing = true;
                gvMain.RowHeight = 100;
            }
            foreach (DataRow dr in dtband.Rows)
            {
                GridBand gb = new GridBand();
                gb.Name = dr["sysBandID"].ToString();
                gb.Caption = dr["Caption"].ToString();
                gb.Width = int.Parse(dr["Width"].ToString());
                gvMain.Bands.Add(gb);
            }
            int exColNum = 0;
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string viewable = dr["Viewable"].ToString();
                if ((admin || !(viewable != string.Empty)) || bool.Parse(viewable))
                {
                    CDTBandGridColumn gcl = this.GenBandGridColumn(dr, exColNum, false);
                    if (dr["sysBandID"] != DBNull.Value)
                    {
                        //gvMain.Bands[dr["sysBandID"].ToString()].Columns.Add(gcl);
                    }
                    RepositoryItem ri = this.GenRepository(dr);

                    if (ri != null)
                    {
                        CDTRepGridLookup CDTRi = ri as CDTRepGridLookup;
                        if (CDTRi != null)
                        {
                            CDTRi.MainView = gvMain;
                            CDTRi.MainStruct = dt;
                        }
                        gcMain.RepositoryItems.Add(ri);
                        gcl.ColumnEdit = ri;
                    }
                    gcl.Visible = gcl.Visible && lstField.ToLower().Contains(gcl.FieldName.ToLower() + ",");
                   
                    gvMain.Columns.Add(gcl);
                    int pType = int.Parse(dr["Type"].ToString());
                    if (pType == 12)
                    {
                        gvMain.OptionsView.RowAutoHeight = true;
                    }
                    if ((pType == 1) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        CDTBandGridColumn gcl1 = this.GenBandGridColumn(dr, exColNum, false);
                        gcl1.GroupIndex = -1;
                        gcl1.Visible = gcl1.Visible && lstField.ToLower().Contains(gcl.FieldName.ToLower() + ",");
                        gcl1.isExCol = true;
                        RepositoryItem ri1 = this.GenRepository(dr);
                        CDTRepGridLookup CDTRi1 = ri1 as CDTRepGridLookup;
                        if (CDTRi1 != null)
                        {
                            CDTRi1.MainView = gvMain;
                            CDTRi1.MainStruct = dt;
                        }
                        if (ri1 != null)
                        {
                            string caption;
                            string displayMember = dr["DisplayMember"].ToString();
                            ((CDTRepGridLookup)ri1).DisplayMember = displayMember;
                            gcMain.RepositoryItems.Add(ri1);
                            if (Config.GetValue("Language").ToString() == "0")
                            {
                                caption = "Tên " + dr["LabelName"].ToString().ToLower();
                            }
                            else
                            {
                                caption = dr["LabelName2"].ToString() + " name";
                            }
                            int formType = int.Parse(this._data.DrTable["Type"].ToString());
                            if (!(((formType != 1) && (formType != 4)) || bool.Parse(dr["AllowNull"].ToString())))
                            {
                                caption = "*" + caption;
                            }
                            gcl1.Caption = caption;
                            gcl1.VisibleIndex++;
                            gcl1.Width += 200;
                            gcl1.ColumnEdit = ri1;
                            gcl1.Visible = gcl.Visible;
                            if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString()))
                                gcl.VisibleIndex = -1;
                        }
                        gvMain.Columns.Add(gcl1);
                        exColNum++;
                    }
                    if (gcl.GroupIndex >= 0)
                    {
                        gvMain.GroupSummary.Add(new GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, gcl.FieldName, null, "({0} mục)"));
                    }
                }
            }
            gcMain.EndInit();
            gvMain.EndInit();
            return gcMain;
        }
        public CDTGridLookUpEdit GenGridLookupEdit(DataRow drField, bool isCBSControl)
        {
            string condition;
            DataTable dt;
            CDTGridLookUpEdit tmp = isCBSControl ? new CDTGridLookUpEdit() : new CDTGridLookUpEdit();
            string refField = drField["RefField"].ToString();
            string refTable = drField["RefTable"].ToString();
            if (this._formAction != FormAction.Filter)
            {
                condition = drField["refCriteria"].ToString();
            }
            else
            {
                condition = drField["FilterCond"].ToString();
            }
            string Dyncondition = drField["DynCriteria"].ToString();
            bool isMaster = true;
            int n = 0;
            CDTData data = null;
            data = this.GetDataForLookup(refTable, condition, Dyncondition, ref isMaster, ref n);
            //if (data == null) 
            //    return null;
            data.DataIndexList = n;
            FormDesigner fd = new FormDesigner(data);
            string displayMember = drField["DisplayMember"].ToString();
            if (isMaster)
            {
                dt = data.DsStruct.Tables[0];
            }
            else
            {
                dt = data.DsStruct.Tables[1];
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr1 = dt.Rows[i];
                string fieldList = "*";
                if (data.DrTable.Table.Columns.Contains("RefFList") && data.DrTable["RefFList"] != DBNull.Value && data.DrTable["RefFList"].ToString() != string.Empty)
                    fieldList = data.DrTable["RefFList"].ToString();
                if (fieldList != "*")
                {
                    if (!fieldList.Contains(dr1["FieldName"].ToString()))
                        continue;
                }

                CDTGridColumn gcl = fd.GenGridColumn(dr1, 0, false);
                if (dr1["EditMask"].ToString() != string.Empty)
                {
                    gcl.DisplayFormat.FormatType = FormatType.Numeric;
                    gcl.DisplayFormat.FormatString = dr1["EditMask"].ToString();
                }
                gcl.GroupIndex = -1;
                tmp.Properties.View.Columns.Add(gcl);
            }
            BindingSource bs = new BindingSource();
            if (data.DsData == null) data.GetData();
            if (isMaster)
            {
                bs.DataSource = data.DsData.Tables[0];
            }
            else
            {
                bs.DataSource = data.DsData.Tables[1];
            }
            tmp.fieldName = drField["FieldName"].ToString();
            tmp.Data = data;
            if (data.dataType == DataType.Single)
            {
                try { data.DataSoureChanged -= Data_DataSoureChanged; }
                catch
                { }
                data.DataSoureChanged += Data_DataSoureChanged;
            } 
            tmp.Properties.DataSource = bs;
            tmp.Properties.ValueMember = refField;
            tmp.refTable = refTable;
            tmp.Properties.Name = drField["FieldName"].ToString();
            tmp.Properties.ValueMember = refField;
            tmp.DymicCondition = Dyncondition;
            tmp.Properties.View.ViewCaption = condition;
            tmp.Allownull = drField["AllowNull"].ToString() == "1";
            tmp.Properties.Buttons[0].Tag = drField;
            tmp.Properties.TextEditStyle = TextEditStyles.Standard;
            tmp.DataIndex = n;
            this._Glist.Add(new LookUp_CDTData(tmp, n));
            this._glist.Add(tmp);
            this.GOldValue.Add(_glist.Count.ToString(), "");
            tmp.Properties.View.OptionsView.ShowFooter = true;
            if (int.Parse(drField["Type"].ToString()) == 1)
            {
                tmp.Properties.DisplayMember = refField;
            }
            else
            {
                tmp.Properties.DisplayMember = displayMember;
            }
            tmp.Properties.PopupFormMinSize = new Size(600, 100);
            tmp.Properties.View.IndicatorWidth = 40;
            tmp.Properties.View.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);
            tmp.EditValueChanged += new EventHandler(this.GridLookupEdit_EditValueChanged);
            tmp.Validated += new EventHandler(this.GridLookupEdit_Validated);
            tmp.Popup += new EventHandler(this.GridLookupEdit_Popup);
            tmp.KeyDown += new KeyEventHandler(this.GridLookupEdit_KeyDown);
            tmp.Properties.View.KeyDown += new KeyEventHandler(this.View_KeyDown);
            tmp.Properties.View.TopRowChanged += View_TopRowChanged;
            tmp.Properties.View.ColumnFilterChanged += View_ColumnFilterChanged;
            if (this._formAction != FormAction.Filter)
            {
                if (data.dataType == DataType.Single)
                {
                    EditorButton plusBtn = new EditorButton(data, ButtonPredefines.Plus);
                    plusBtn.Shortcut = new KeyShortcut(Keys.F6);
                    plusBtn.ToolTip = "F6 - New";
                    tmp.Properties.Buttons.Add(plusBtn);


                   
                    if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                    {
                        string sInsert = data.DrTable["sInsert"].ToString();

                        bool tInsert = false;
                        if (sInsert == "0") tInsert = false;
                        else if (sInsert == "1" || sInsert == string.Empty) tInsert = true;
                        if (sInsert != "" && !tInsert)
                        {
                            plusBtn.Visible = false;
                        }
                    }
                }
                tmp.Properties.ButtonClick += new ButtonPressedEventHandler(this.Plus_ButtonClick);
                EditorButton refreshData = new EditorButton(data, ButtonPredefines.Ellipsis);
                refreshData.Shortcut = new KeyShortcut(Keys.F3);
                refreshData.ToolTip = "F5 - Refresh";
                tmp.Properties.Buttons.Add(refreshData);
            }
            return tmp;
        }

        private void View_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                // Kiểm tra nếu người dùng đã cuộn đến gần cuối danh sách
                int visibleRowCount = view.RowCount;
                CDTGridLookUpEdit tmp;
                var popupForm = view.GridControl.Parent as DevExpress.XtraEditors.Popup.PopupGridLookUpEditForm;
                tmp = popupForm.OwnerEdit as CDTGridLookUpEdit;
                if (visibleRowCount == 0 && !tmp.Data.FullData)
                {
                    this.RefreshLookup(tmp.Data);
                    setDynFilter(tmp);
                }
            }
            catch { }
        }

        private void View_TopRowChanged(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                int visibleRowCount = view.RowCount;
                int topRowIndex = view.PrevTopRowIndex;

                // Kiểm tra nếu người dùng đã cuộn đến gần cuối danh sách
                CDTGridLookUpEdit tmp;
                if (topRowIndex >= visibleRowCount - 50)
                {
                    if (view.GridControl.Parent != null)
                    {
                        var popupForm = view.GridControl.Parent as DevExpress.XtraEditors.Popup.PopupGridLookUpEditForm;
                        tmp = popupForm.OwnerEdit as CDTGridLookUpEdit;
                        if (!tmp.Data.FullData)
                        {
                            this.RefreshLookup(tmp.Data);
                            setDynFilter(tmp);
                        }
                    }
                    //LoadMoreData();

                }
            }
            catch { }
        }

       

        public LayoutControl GenLayout1(ref GridControl gcMain, bool isCBSControl)
        {
            int i;
            DataRow dr;
            BaseEdit ctrl;
            LayoutControlItem lci;
            DataTable dt = this._data.DsStruct.Tables[0];
            LayoutControl lcMain = new LayoutControl();
            LayoutControlGroup lcgMain = lcMain.Root;
            lcMain.BeginInit();
            lcMain.SuspendLayout();
            lcgMain.BeginInit();
            lcMain.Dock = DockStyle.Fill;
            lcMain.OptionsView.HighlightFocusedItem = true;
            lcgMain.TextVisible = false;
            dt.DefaultView.RowFilter = "Visible = 1";
            if (gcMain != null)
            {
                DataView defaultView = dt.DefaultView;
                defaultView.RowFilter = defaultView.RowFilter + " and IsBottom = 0";
            }
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                DataView view2 = dt.DefaultView;
                view2.RowFilter = view2.RowFilter + " and (Viewable is null or Viewable = 1)";
            }
            for (i = 0; i < dt.DefaultView.Count; i++)
            {
                dr = dt.DefaultView[i].Row;
                ctrl = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                if (ctrl != null)
                {
                    BaseEdit ctrl1;
                    LayoutControlItem lci1;
                    if (this._firstControl == null)
                    {
                        this._firstControl = ctrl;
                    }
                    ctrl.StyleController = lcMain;
                    int pType = int.Parse(dr["Type"].ToString());
                    lci = new LayoutControlItem();
                    string caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                    if (dr["AllowNull"].ToString() == "0")
                    {
                        caption = "*" + caption;
                    }
                    lci.Text = caption;
                    lci.Control = ctrl;
                    if (dr["Visible"].ToString() == "0")
                    {
                        lci.Visibility = LayoutVisibility.Never;
                    }
                    lcMain.Controls.Add(ctrl);
                    lcgMain.AddItem(lci);
                    if (((this._formAction != FormAction.Filter) && (pType == 1)) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        ((CDTGridLookUpEdit)ctrl1).Properties.DisplayMember = dr["DisplayMember"].ToString();
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            caption = "Tên " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            caption = dr["LabelName2"].ToString() + " name";
                        }
                        if (dr["AllowNull"].ToString() == "1")
                        {
                            caption = "*" + caption;
                        }
                        lci1.Text = caption;
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString())) lci.Visibility = LayoutVisibility.OnlyInCustomization;
                        lcMain.Controls.Add(ctrl1);
                        this._BaseList.Add(ctrl1);
                        lcgMain.AddItem(lci1);
                    }
                    if ((this._formAction == FormAction.Filter) && bool.Parse(dr["IsBetween"].ToString()))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        ctrl.Name = ctrl.Name + "1";
                        ctrl.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "1");
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci.Text = "Từ " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci.Text = "From " + dr["LabelName2"].ToString().ToLower();
                        }
                        ctrl1.Name = ctrl1.Name + "2";
                        ctrl1.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "2");
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci1.Text = "Đến " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci1.Text = "To " + dr["LabelName2"].ToString().ToLower();
                        }
                        if (dr["AllowNull"].ToString() == "0")
                        {
                            lci.Text = "*" + lci.Text;
                            lci1.Text = "*" + lci1.Text;
                        }
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        lcMain.Controls.Add(ctrl1);
                        lcgMain.AddItem(lci1);
                    }
                }
            }
            dt.DefaultView.RowFilter = "Visible = 1";
            if (!admin)
            {
                DataView view3 = dt.DefaultView;
                view3.RowFilter = view3.RowFilter + " and (Viewable is null or Viewable = 1)";
            }
            if (gcMain != null)
            {
                lcgMain.DefaultLayoutType = LayoutType.Vertical;
                LayoutControlGroup lcg3 = lcgMain.AddGroup();
                LayoutControlItem lcit = new LayoutControlItem();
                gcMain = this.GenGridControl(this._data.DsStruct.Tables[1], true, DockStyle.None);
                gcMain.Name = this._data.DrTable["TableName"].ToString();
                lcg3.TextVisible = false;
                lcg3.GroupBordersVisible = false;
                lcit.Name = "Detail";
                lcit.TextVisible = false;
                lcit.Control = gcMain;
                lcMain.Controls.Add(gcMain);
                lcg3.AddItem(lcit);
                DataView view4 = dt.DefaultView;
                view4.RowFilter = view4.RowFilter + " and IsBottom = 1";
                if (dt.DefaultView.Count > 0)
                {
                    LayoutControlGroup lcg4 = lcgMain.AddGroup();
                    lcg4.TextVisible = false;
                    lcg4.GroupBordersVisible = false;
                    for (i = 0; i < dt.DefaultView.Count; i++)
                    {
                        dr = dt.DefaultView[i].Row;
                        ctrl = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        if (ctrl != null)
                        {
                            ctrl.StyleController = lcMain;
                            lci = new LayoutControlItem();
                            lci.Text = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                            lci.Control = ctrl;

                            lcMain.Controls.Add(ctrl);
                            lcg4.AddItem(lci);
                        }
                    }
                }
            }
            lcMain.EndInit();
            lcMain.ResumeLayout(false);
            lcgMain.EndInit();
            return lcMain;
        }

        public LayoutControl GenLayout2(ref GridControl gcMain, bool isCBSControl)
        {
            DataRow dr;
            BaseEdit ctrl;
            LayoutControlItem lci;
            DataTable dt = this._data.DsStruct.Tables[0];
            lcMain = new LayoutControl();
            LayoutControlGroup lcgMain = lcMain.Root;
            lcgMain.Name = "Root";
            lcMain.BeginInit();
            lcMain.SuspendLayout();
            lcgMain.BeginInit();
            lcMain.Dock = DockStyle.Fill;
            lcMain.OptionsView.HighlightFocusedItem = true;
            lcgMain.TextVisible = false;
            lcgMain.DefaultLayoutType = LayoutType.Horizontal;
            LayoutControlGroup lcg1 = lcgMain.AddGroup();
            lcg1.TextVisible = false;
            lcg1.GroupBordersVisible = false;
            lcg1.Name = "g1";
            LayoutControlGroup lcg2 = lcgMain.AddGroup();
            lcg2.TextVisible = false;
            lcg2.GroupBordersVisible = false;
            lcg2.Name = "g2";
            lcg1.Size = new Size((lcgMain.Size.Width / 2) + 20, 0);
            if (gcMain != null)
            {
                dt.DefaultView.RowFilter = " IsBottom = 0";
            }
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                if (dt.DefaultView.RowFilter == "")
                {
                    DataView defaultView = dt.DefaultView;
                    defaultView.RowFilter = defaultView.RowFilter + " (Viewable is null or Viewable = 1)";
                }
                else
                {
                    DataView view2 = dt.DefaultView;
                    view2.RowFilter = view2.RowFilter + " and (Viewable is null or Viewable = 1)";
                }
            }
            int i = 0;
            while (i < dt.DefaultView.Count)
            {
                dr = dt.DefaultView[i].Row;
                ctrl = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                if (ctrl != null)
                {
                    BaseEdit ctrl1;
                    LayoutControlItem lci1;
                    if (this._firstControl == null && dr["Visible"].ToString() == "1")
                    {
                        this._firstControl = ctrl;
                    }

                    ctrl.StyleController = lcMain;
                    int pType = int.Parse(dr["Type"].ToString());
                    lci = new LayoutControlItem();
                    string caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                    if (dr["AllowNull"].ToString() == "0")
                    {
                        caption = "*" + caption;
                    }
                    lci.Text = caption;
                    lci.Control = ctrl;
                    lcMain.Controls.Add(ctrl);
                    if (dr["Visible"].ToString() == "0")
                    {
                        lci.Visibility = LayoutVisibility.OnlyInCustomization;
                    }
                    this._LayoutList.Add(lci);
                    this._BaseList.Add(ctrl);
                    lci.Name = dr["FieldName"].ToString();
                    if (i < (dt.DefaultView.Count / 2))
                    {
                        lcg1.AddItem(lci);
                    }
                    else
                    {
                        lcg2.AddItem(lci);
                    }
                    if ((((this._formAction != FormAction.Filter) && (pType == 1)) && (dr["DisplayMember"].ToString() != string.Empty)) && ((dr["Visible"].ToString() == "1") || (dr["Visible"].ToString() == "True")))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        // ((CDTGridLookUpEdit)ctrl1).Properties.DisplayMember = dr["DisplayMember"].ToString();
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            caption = "Tên " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            caption = dr["LabelName2"].ToString() + " name";
                        }
                        if (dr["AllowNull"].ToString() == "0")
                        {
                            caption = "*" + caption;
                        }
                        lci1.Text = caption;
                        ctrl1.Name = ctrl1.Name + "001";
                        lci1.Name = dr["fieldName"].ToString() + "001";
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        lcMain.Controls.Add(ctrl1);
                        this._BaseList.Add(ctrl1);
                        lci.Name = dr["FieldName"].ToString();
                        if (i < (dt.DefaultView.Count / 2))
                        {
                            lcg1.AddItem(lci1);
                        }
                        else
                        {
                            lcg2.AddItem(lci1);
                        }
                    }
                    if (((this._formAction == FormAction.Filter) && bool.Parse(dr["IsBetween"].ToString())) && ((dr["Visible"].ToString() == "1") || (dr["Visible"].ToString() == "True")))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        ctrl.Name = ctrl.Name + "1";
                        ctrl.DataBindings.Clear();
                        ctrl.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "1");
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci.Text = "Từ " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci.Text = "From " + dr["LabelName2"].ToString().ToLower();
                        }
                        ctrl1.Name = ctrl1.Name + "2";
                        ctrl1.DataBindings.Clear();
                        ctrl1.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "2");
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci1.Text = "Đến " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci1.Text = "To " + dr["LabelName2"].ToString().ToLower();
                        }
                        if (dr["AllowNull"].ToString() == "0")
                        {
                            lci.Text = "*" + lci.Text;
                            lci1.Text = "*" + lci1.Text;
                        }
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString())) lci.Visibility = LayoutVisibility.OnlyInCustomization;
                        lcMain.Controls.Add(ctrl1);
                        if (i < (dt.DefaultView.Count / 2))
                        {
                            lcg1.AddItem(lci1);
                        }
                        else
                        {
                            lcg2.AddItem(lci1);
                        }
                    }
                }
                else
                {
                    return null;
                }
                i++;
            }
            if (!admin)
            {
                dt.DefaultView.RowFilter = " (Viewable is null or Viewable = 1)";
            }
            if (gcMain != null)
            {
                lcgMain.DefaultLayoutType = LayoutType.Vertical;
                LayoutControlGroup lcgBt = lcgMain.AddGroup();
                LayoutControlItem lcit = new LayoutControlItem();
                lcgBt.Name = "lcgBt";
                this.TabDetail.SelectedPageChanging += new TabPageChangingEventHandler(this.TabDetail_SelectedPageChanging);
                this.TabDetail.SendToBack();
                this.TabDetail.KeyUp += TabDetail_KeyUp;
                XtraTabPage Tab1 = new XtraTabPage();
                Tab1.Text = "Detail";

                this.TabDetail.TabPages.Add(Tab1);
                lcgBt.TextVisible = false;
                lcgBt.GroupBordersVisible = false;

                lcit.Name = "Detail";
                this.TabDetail.Name = "TabDetail";
                lcit.TextVisible = false;
                gcMain = this.GenGridControl(this._data.DsStruct.Tables[1], true, DockStyle.Fill);
                GridView gvMain = (gcMain.Views[0]) as GridView;
                if (this._data.DrTable.Table.Columns.Contains("RowHeight") && this._data.DrTable["RowHeight"] != DBNull.Value)
                    gvMain.RowHeight = int.Parse(this._data.DrTable["RowHeight"].ToString());
                gcMain.Name = this._data.DrTable["TableName"].ToString();

                lcit.Control = this.TabDetail;
                this.TabDetail.GotFocus += new EventHandler(this.TabDetail_GotFocus);
                Tab1.Controls.Add(gcMain);
                Tab1.GotFocus += new EventHandler(this.Tab1_GotFocus);
                lcMain.Controls.Add(this.TabDetail);
                lcgBt.AddItem(lcit);
                this._gcDetail = new List<GridControl>();
                for (i = 0; i < this.Data._drTableDt.Count; i++)
                {
                    DataRow drTable = this.Data._drTableDt[i];
                    GridControl gcDt = this.GenGridControlDt(this.Data._dsStructDt.Tables[i], this.Data._dtDetail.Rows[i]["lstField"].ToString(), true, DockStyle.Fill);
                    gcDt.Name = drTable["TableName"].ToString();
                    GridView gvDetail = (gcDt.Views[0]) as GridView;
                    if (drTable.Table.Columns.Contains("RowHeight") && drTable["RowHeight"] != DBNull.Value)
                        gvDetail.RowHeight = int.Parse(drTable["RowHeight"].ToString());
                    this._gcDetail.Add(gcDt);
                    GridView gv = gcDt.ViewCollection[0] as GridView;
                    gv.OptionsView.ShowAutoFilterRow = false;
                    gv.OptionsView.ShowGroupPanel = false;
                    //gv.OptionsView.ShowFooter = false;
                    XtraTabPage t = new XtraTabPage();
                    t.GotFocus += new EventHandler(this.Tab1_GotFocus);
                    t.Controls.Add(gcDt);
                    
                    t.Text = this.Data._dtDetail.Rows[i]["DetailName"].ToString();
                    if (this.Data.DsData.Tables[drTable["TableName"].ToString()].Columns.Contains("DTID"))
                    {
                        gcMain.ViewCollection.Add(gv);
                        GridLevelNode gridLevelNode1 = new GridLevelNode();
                        gridLevelNode1.LevelTemplate = gv;
                        gridLevelNode1.RelationName = drTable["TableName"].ToString() + "1";
                        gcMain.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
                        (gcMain.Views[0] as GridView).OptionsDetail.AllowExpandEmptyDetails = true;
                    }
                    else
                    {
                        this.TabDetail.TabPages.Add(t);
                    }
                }

                //lcgMain.DefaultLayoutType = LayoutType.Vertical;
                //LayoutControlGroup lcg3 = lcgMain.AddGroup();
                //LayoutControlItem lcit = new LayoutControlItem();
                //gcMain = this.GenGridControl(this._data.DsStruct.Tables[1], true, DockStyle.None);
                //lcg3.TextVisible = false;
                //lcg3.GroupBordersVisible = false;
                //lcit.Name = "Detail";
                //lcit.TextVisible = false;
                //lcit.Control = gcMain;
                //lcMain.Controls.Add(gcMain);
                //lcg3.AddItem(lcit);
                dt.DefaultView.RowFilter = " IsBottom = 1";
                if (dt.DefaultView.Count > 0)
                {
                    LayoutControlGroup lcg4 = lcgMain.AddGroup();
                    lcg4.TextVisible = false;
                    lcg4.GroupBordersVisible = false;
                    lcg4.Name = "lcg4";
                    lcg4.DefaultLayoutType = LayoutType.Horizontal;
                    LayoutControlGroup lcg5 = lcg4.AddGroup();
                    lcg5.TextVisible = false;
                    lcg5.GroupBordersVisible = false;
                    lcg5.Name = "lcg5";
                    lcg5.DefaultLayoutType = LayoutType.Vertical;
                    LayoutControlGroup lcg6 = lcg4.AddGroup();
                    lcg6.TextVisible = false;
                    lcg6.GroupBordersVisible = false;
                    lcg6.Name = "lcg6";
                    lcg6.DefaultLayoutType = LayoutType.Vertical;
                    for (i = 0; i < dt.DefaultView.Count; i++)
                    {
                        dr = dt.DefaultView[i].Row;
                        ctrl = this.GenCBSControl(dr);
                        if (ctrl != null)
                        {
                            ctrl.StyleController = lcMain;
                            lci = new LayoutControlItem();
                            lci.Text = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                            lci.Control = ctrl;
                            lci.Name = dr["fieldName"].ToString();
                            if (dr["Visible"].ToString() == "0")
                            {
                                lci.Visibility = LayoutVisibility.OnlyInCustomization;
                            }
                            lcMain.Controls.Add(ctrl);
                            this._BaseList.Add(ctrl);
                            this._LayoutList.Add(lci);
                            if (i < (dt.DefaultView.Count / 2))
                            {
                                lcg5.AddItem(lci);
                            }
                            else
                            {
                                lcg6.AddItem(lci);
                            }
                        }
                    }
                }
                dt.DefaultView.RowFilter = string.Empty;
            }
            lcMain.EndInit();
            lcMain.ResumeLayout(false);
            lcgMain.EndInit();
            return lcMain;
        }

        private void TabDetail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6 && TabDetail.TabPages.Count>1)
            {
                string Mess = "Tạo lại dữ liệu chi tiết, dữ liệu chi tiết cũ sẽ bị mất!, Bạn có đồng ý không?";
                if (Config.GetValue("Language").ToString() == "1")
                    Mess = "Created detail again, old detail data was lost. Do you agree?";
                if (MessageBox.Show(Mess, "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    InsertedToDetail = false;
                    InsertDetailFromMTDT();
                }
            }
            
        }

        public LayoutControl GenLayout3(ref GridControl gcMain, bool isCBSControl)
        {
            int i;
            DataRow dr;
            BaseEdit ctrl;
            LayoutControlItem lci;

            DataTable dt = this._data.DsStruct.Tables[0];
            LayoutControl lcMain = new LayoutControl();

            LayoutControlGroup lcgMain = lcMain.Root;
            lcgMain.Name = "Root";
            lcMain.BeginInit();
            lcMain.SuspendLayout();
            lcgMain.BeginInit();
            lcMain.Dock = DockStyle.Fill;
            lcMain.OptionsView.HighlightFocusedItem = true;
            lcMain.BringToFront();
            lcgMain.TextVisible = false;
            lcgMain.DefaultLayoutType = LayoutType.Horizontal;
            LayoutControlGroup lcg1 = lcgMain.AddGroup();
            lcg1.TextVisible = false;
            lcg1.GroupBordersVisible = false;
            lcg1.Name = "g1";
            LayoutControlGroup lcg2 = lcgMain.AddGroup();
            lcg2.TextVisible = false;
            lcg2.GroupBordersVisible = false;
            lcg2.Name = "g2";
            LayoutControlGroup lcg3 = lcgMain.AddGroup();
            lcg3.TextVisible = false;
            lcg3.GroupBordersVisible = false;
            lcg3.Name = "g3";
            lcg1.Size = new Size((lcgMain.Size.Width / 3) + 20, 0);
            if (gcMain != null)
            {

                dt.DefaultView.RowFilter = " IsBottom = 0";
            }
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                if (dt.DefaultView.RowFilter == "")
                {
                    DataView defaultView = dt.DefaultView;
                    defaultView.RowFilter = defaultView.RowFilter + " (Viewable is null or Viewable = 1)";
                }
                else
                {
                    DataView view2 = dt.DefaultView;
                    view2.RowFilter = view2.RowFilter + " and (Viewable is null or Viewable = 1)";
                }
            }
            DataRow[] drCount = dt.Select("Visible <> '0' and IsBottom=0 and Type<>13");
            DataRow[] drCountDisplay = dt.Select("DisplayMember is not null and DisplayMember <>'' and IsBottom=0");
            DataRow[] drMemo = dt.Select("Type=13 and IsBottom=0 and visible=1");
            int ControlCount = drCount.Length + drCountDisplay.Length + drMemo.Length * 3;
            int j = 0;
            for (i = 0; i < dt.DefaultView.Count; i++)
            {
                dr = dt.DefaultView[i].Row;
                ctrl = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                //int[] NoControl = new int[] {  3, 6,15 };
                //int type = int.Parse(dr["Type"].ToString());
                //if (ctrl == null && !NoControl.Contains(type))
                //    return null;
                if (ctrl != null)
                {

                    BaseEdit ctrl1;
                    LayoutControlItem lci1;
                    if (this._firstControl == null && dr["Visible"].ToString() == "1")
                    {
                        this._firstControl = ctrl;
                    }
                    ctrl.StyleController = lcMain;
                    int pType = int.Parse(dr["Type"].ToString());
                    lci = new LayoutControlItem();
                    string caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                    if (dr["AllowNull"].ToString() == "0")
                    {
                        caption = "*" + caption;
                    }
                    lci.Text = caption;
                    lci.Control = ctrl;
                    
                    
                    lcMain.Controls.Add(ctrl);

                    if (dr["Visible"].ToString() == "0")
                    {
                        lci.Visibility = LayoutVisibility.OnlyInCustomization;
                    }

                    if (lci.Visibility == LayoutVisibility.Always)
                    {
                        if (ctrl.ToString().Contains("Memo"))
                            j += 2;
                        else
                            j++;

                    }

                    this._LayoutList.Add(lci);
                    this._BaseList.Add(ctrl);
                    lci.Name = dr["FieldName"].ToString();
                    if (j <= (ControlCount / 3))
                    {
                        lcg1.AddItem(lci);
                    }
                    else if (j <= ((ControlCount * 2) / 3))
                    {
                        lcg2.AddItem(lci);
                    }
                    else
                    {
                        lcg3.AddItem(lci);
                    }
                    if (dr["Visible"].ToString() != "0" && ctrl.ToString().Contains("Memo"))
                    {
                        j += 1;
                    }

                    if ((((this._formAction != FormAction.Filter) && (pType == 1)) && (dr["DisplayMember"].ToString() != string.Empty)) && ((dr["Visible"].ToString() == "1") || (dr["Visible"].ToString() == "True")))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        ((CDTGridLookUpEdit)ctrl1).Properties.DisplayMember = dr["DisplayMember"].ToString();
                        // ((CDTGridLookUpEdit)ctrl1).Properties.Name = dr["DisplayMember"].ToString();
                        ((CDTGridLookUpEdit)ctrl1).Properties.Name = dr["FieldName"].ToString();
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            caption = "Tên " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            caption = dr["LabelName2"].ToString() + " name";
                        }
                        if (dr["AllowNull"].ToString() == "0")
                        {
                            caption = "*" + caption;
                        }
                        lci1.Text = caption;
                        ctrl1.Name = ctrl1.Name + "001";
                        lci1.Name = dr["FieldName"].ToString() + "001";
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        if (dr["RefTable"] != DBNull.Value && !bool.Parse(dr["ShowRef"].ToString())) lci.Visibility = LayoutVisibility.OnlyInCustomization;
                        lcMain.Controls.Add(ctrl1);
                        this._BaseList.Add(ctrl1);
                        j++;
                        if (j <= (ControlCount / 3))
                        {
                            lcg1.AddItem(lci1);
                        }
                        else if (j <= ((ControlCount * 2) / 3))
                        {
                            lcg2.AddItem(lci1);
                        }
                        else
                        {
                            lcg3.AddItem(lci1);
                        }
                    }
                    if (((this._formAction == FormAction.Filter) && bool.Parse(dr["IsBetween"].ToString())) && ((dr["Visible"].ToString() == "1") || (dr["Visible"].ToString() == "True")))
                    {
                        ctrl1 = isCBSControl ? this.GenCBSControl(dr) : this.GenControl(dr);
                        ctrl.Name = ctrl.Name + "1";
                        ctrl.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "1");
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci.Text = "Từ " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci.Text = "From " + dr["LabelName2"].ToString().ToLower();
                        }
                        ctrl1.Name = ctrl1.Name + "2";
                        ctrl1.DataBindings.Add("EditValue", this._bindingSource, dr["FieldName"].ToString() + "2");
                        ctrl1.StyleController = lcMain;
                        lci1 = new LayoutControlItem();
                        if (Config.GetValue("Language").ToString() == "0")
                        {
                            lci1.Text = "Đến " + dr["LabelName"].ToString().ToLower();
                        }
                        else
                        {
                            lci1.Text = "To " + dr["LabelName2"].ToString().ToLower();
                        }
                        if (dr["AllowNull"].ToString() == "0")
                        {
                            lci.Text = "*" + lci.Text;
                            lci1.Text = "*" + lci1.Text;
                        }
                        lci1.Control = ctrl1;
                        lci1.Visibility = lci.Visibility;
                        lcMain.Controls.Add(ctrl1);

                        if (j <= (ControlCount / 3))
                        {
                            lcg1.AddItem(lci);
                        }
                        else if (j <= ((ControlCount * 2) / 3))
                        {
                            lcg2.AddItem(lci);
                        }
                        else
                        {
                            lcg3.AddItem(lci);
                        }
                    }
                }
                
            }
            if (!admin)
            {
                dt.DefaultView.RowFilter = " (Viewable is null or Viewable = 1)";
            }
            if (gcMain != null)
            {

                lcgMain.DefaultLayoutType = LayoutType.Vertical;
                LayoutControlGroup lcgBt = lcgMain.AddGroup();
                LayoutControlItem lcit = new LayoutControlItem();
                lcgBt.Name = "lcgBt";
                this.TabDetail.SelectedPageChanging += new TabPageChangingEventHandler(this.TabDetail_SelectedPageChanging);
                this.TabDetail.SendToBack();
                XtraTabPage Tab1 = new XtraTabPage();
                Tab1.Text = (Config.GetValue("Language").ToString() == "0") ?"Chi tiết" : "Detail";

                this.TabDetail.TabPages.Add(Tab1);
                
                this.TabDetail.KeyUp += TabDetail_KeyUp;
                
                lcgBt.TextVisible = false;
                lcgBt.GroupBordersVisible = false;
                lcit.Name = "Detail";
                this.TabDetail.Name = "TabDetail";
                lcit.TextVisible = false;
                if (_data.DrTable.Table.Columns.Contains("useBand") && bool.Parse(_data.DrTable["useBand"].ToString()))
                {
                    gcMain = this.GenBandGridControl(this._data.DsStruct.Tables[1], _data._dsBand.Tables[_data.DrTable["TableName"].ToString()], true, DockStyle.Fill);
                    GridView gvMain = (gcMain.Views[0]) as GridView;
                    gvMain.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
                    //gvMain.OptionsBehavior.AllowAddRows = true;
                    //gvMain.OptionsBehavior.Editable = true;
                }
                else
                {
                    gcMain = this.GenGridControl(this._data.DsStruct.Tables[1], true, DockStyle.Fill);
                    GridView gvMain = (gcMain.Views[0]) as GridView;
                    if (_data.DrTable.Table.Columns.Contains("RowHeight") && (_data.DrTable["RowHeight"]!=DBNull.Value))
                    {
                        gvMain.RowHeight = int.Parse(_data.DrTable["RowHeight"].ToString());
                    }
                       
                    if (this._data.DrTable.Table.Columns.Contains("RowHeight") && this._data.DrTable["RowHeight"] != DBNull.Value)
                        gvMain.RowHeight = int.Parse(this._data.DrTable["RowHeight"].ToString());
                }

                gcMain.Name = this._data.DrTable["TableName"].ToString();
                lcit.Control = this.TabDetail;
                this.TabDetail.GotFocus += new EventHandler(this.TabDetail_GotFocus);
                Tab1.Controls.Add(gcMain);
                Tab1.GotFocus += new EventHandler(this.Tab1_GotFocus);
                lcMain.Controls.Add(this.TabDetail);
                (gcMain.Views[0] as GridView).OptionsView.ShowDetailButtons = true;

                lcgBt.AddItem(lcit);
                this._gcDetail = new List<GridControl>();
                for (i = 0; i < this.Data._drTableDt.Count; i++)
                {

                    DataRow drTable = this.Data._drTableDt[i];
                    DataRow drDetail = this.Data._dtDetail.Rows[i];
                    GridControl gcDt;
                    if (drTable.Table.Columns.Contains("useband") && bool.Parse(drTable["useBand"].ToString()))
                    {
                        gcDt = this.GenBandGridControlDt(this.Data._dsStructDt.Tables[i], _data._dsBand.Tables[drTable["TableName"].ToString()], this.Data._dtDetail.Rows[i]["lstField"].ToString(), true, DockStyle.Fill);
                    }
                    else
                    {
                        gcDt = this.GenGridControlDt(this.Data._dsStructDt.Tables[i], this.Data._dtDetail.Rows[i]["lstField"].ToString(), true, DockStyle.Fill);

                    }
                    GridView gvDetail = (gcDt.Views[0]) as GridView;
                    gvDetail.ValidateRow += GvDetail_ValidateRow;
                    gvDetail.InitNewRow += GvDetail_InitNewRow;
                    gvDetail.Name = drTable["TableName"].ToString();
                    if (drTable.Table.Columns.Contains("RowHeight") && drTable["RowHeight"] != DBNull.Value)
                        gvDetail.RowHeight = int.Parse(drTable["RowHeight"].ToString());
                    gcDt.Name = drTable["TableName"].ToString();
                    this._gcDetail.Add(gcDt);
                    AdvBandedGridView gb = new AdvBandedGridView();
                    GridView gv = new GridView();
                    if (bool.Parse(drTable["useBand"].ToString()))
                    {
                        gb = gcDt.Views[0] as AdvBandedGridView;
                        gb.OptionsView.ShowAutoFilterRow = false;
                        gb.OptionsView.ShowGroupPanel = false;
                    }
                    else
                    {
                        gv = gcDt.Views[0] as GridView;
                        gv.OptionsView.ShowAutoFilterRow = false;
                        gv.OptionsView.ShowGroupPanel = false;
                    }
                    // gv.OptionsView.ShowFooter = false;
                    XtraTabPage t = new XtraTabPage();

                    t.GotFocus += new EventHandler(this.Tab1_GotFocus);
                    t.Controls.Add(gcDt);

                    t.Text = drDetail["DetailName"].ToString();
                    if (!bool.Parse(drDetail["ChildOf"].ToString()))
                    {
                        GridLevelNode gridLevelNode1 = new GridLevelNode();
                        if (bool.Parse(drTable["useBand"].ToString()))
                        {
                            gcMain.ViewCollection.Add(gb);
                            gridLevelNode1.LevelTemplate = gb;
                        }
                        else
                        {
                            gcMain.ViewCollection.Add(gv);
                            gridLevelNode1.LevelTemplate = gv;
                        }


                        gridLevelNode1.RelationName = drTable["TableName"].ToString() + "1";
                        gcMain.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
                        if (drTable.Table.Columns.Contains("useband") && bool.Parse(drTable["useBand"].ToString()))
                        {
                            gb.OptionsDetail.AllowExpandEmptyDetails = true; gb.ViewCaption = drTable["DienGiai"].ToString();
                        }
                        else
                        {
                            gv.OptionsDetail.AllowExpandEmptyDetails = true; gv.ViewCaption = drTable["DienGiai"].ToString();
                        }

                    }
                    else
                    {
                        if (drDetail["Outtag"] != DBNull.Value && bool.Parse(drDetail["Outtag"].ToString()))
                        {
                            LayoutControlItem lcitdt = new LayoutControlItem();
                            lcitdt.Name = drDetail["DetailName"].ToString();
                            lcitdt.TextVisible = false;
                            lcitdt.Control = gcDt;
                            lcgBt.AddItem(lcitdt);
                            _LayoutList.Add(lcitdt);
                        }
                        else
                        {
                            this.TabDetail.TabPages.Add(t);
                        }

                    }
                }
                dt.DefaultView.RowFilter = " IsBottom = 1";
                if (dt.DefaultView.Count > 0)
                {
                    LayoutControlGroup lcg4 = lcgMain.AddGroup();
                    lcg4.TextVisible = false;
                    lcg4.GroupBordersVisible = false;
                    lcg4.Name = "lcg4";
                    lcg4.DefaultLayoutType = LayoutType.Horizontal;
                    LayoutControlGroup lcg5 = lcg4.AddGroup();
                    lcg5.Name = "lcg5";
                    lcg5.TextVisible = false;
                    lcg5.GroupBordersVisible = false;
                    lcg5.DefaultLayoutType = LayoutType.Vertical;
                    LayoutControlGroup lcg6 = lcg4.AddGroup();
                    lcg6.TextVisible = false;
                    lcg6.GroupBordersVisible = false;
                    lcg6.Name = "lcg6";
                    lcg6.DefaultLayoutType = LayoutType.Vertical;
                    for (i = 0; i < dt.DefaultView.Count; i++)
                    {
                        dr = dt.DefaultView[i].Row;
                        ctrl = this.GenCBSControl(dr);
                        if (ctrl != null)
                        {
                            ctrl.StyleController = lcMain;
                            lci = new LayoutControlItem();
                            lci.Text = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                            lci.Control = ctrl;
                            lci.Name = dr["fieldName"].ToString();
                            if (dr["Visible"].ToString() == "0")
                            {
                                lci.Visibility = LayoutVisibility.OnlyInCustomization;
                            }

                            lcMain.Controls.Add(ctrl);
                            this._BaseList.Add(ctrl);
                            this._LayoutList.Add(lci);
                            if (i < (dt.DefaultView.Count / 2))
                            {
                                lcg5.AddItem(lci);
                            }
                            else
                            {
                                lcg6.AddItem(lci);
                            }
                        }
                    }
                }
                dt.DefaultView.RowFilter = string.Empty;
            }
            lcMain.EndInit();
            lcMain.ResumeLayout(false);
            lcgMain.EndInit();
            return lcMain;
        }

        private void GvDetail_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            GridView gvdt = sender as GridView;
            int i = 0;
            //while(i<gvdt.RowCount)
            //{
            //    DataRow dataRow = gvdt.GetDataRow(i);
            //    if (dataRow == null) break;
            //    MessageBox.Show(dataRow["CT12ID"].ToString() );

            //    i++;
            //}
            //i = 0;
            //while (i < Data.DsData.Tables["CT12"].Rows.Count)
            //{
            //    MessageBox.Show( Data.DsData.Tables["CT12"].Rows[i]["CT12ID"].ToString());
            //    i++;
            //}
        }

        private void GvDetail_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            
        }

        private void TabDetail_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private RepositoryItem GenRepository(DataRow dr)
        {
            RepositoryItem tmp = null;
            int pType = int.Parse(dr["Type"].ToString());
            switch (pType)
            {
                case 1:
                case 4:
                case 7:
                    if (!(dr["refTable"].ToString() == string.Empty))
                    {
                        //tmp = new RepositoryItemTextEdit();
                        //break;
                        tmp = this.GenRIGridLookupEdit(dr);
                       // if (tmp == null) return null;
                        // tmp = new CDTRepGridLookup();
                        CDTRepGridLookup riTmp = tmp as CDTRepGridLookup;

                        riTmp.CloseUpKey = KeyShortcut.Empty;
                        riTmp.AllowNullInput = DefaultBoolean.True;
                        riTmp.NullText = string.Empty;
                        riTmp.View.OptionsView.ShowAutoFilterRow = true;
                        riTmp.View.OptionsView.ColumnAutoWidth = false;
                        if (riTmp.DymicCondition != null)
                        {

                            this.RIOldValue.Add(_lstRep.Count.ToString(), "");
                            this._lstRep.Add(riTmp);
                        }
                    }
                    break;

                case 5:
                    tmp = new RepositoryItemSpinEdit();
                    (tmp as RepositoryItemSpinEdit).AllowNullInput = DefaultBoolean.True;
                    break;

                case 8:
                    tmp = new RepositoryItemCalcEdit();
                    (tmp as RepositoryItemCalcEdit).AllowNullInput = DefaultBoolean.True;
                    (tmp as RepositoryItemCalcEdit).Spin += new SpinEventHandler(this.FormDesigner_Spin1);
                    (tmp as RepositoryItemCalcEdit).KeyUp += new KeyEventHandler(this.VCalEdit_KeyUp);
                    if (dr["EditMask"].ToString() != string.Empty)
                    {
                        (tmp as RepositoryItemCalcEdit).EditMask = dr["EditMask"].ToString();
                        (tmp as RepositoryItemCalcEdit).Mask.UseMaskAsDisplayFormat = true;
                    }
                    tmp.EditValueChanged += new EventHandler(tmp_EditValueChanged);
                    break;

                case 9:
                    tmp = new RDateEdit();
                    (tmp as RDateEdit).EditMask = "dd/MM/yyyy";
                    break;

                case 10:
                    tmp = new RepositoryItemCheckEdit();
                    tmp.KeyDown += (Object sender, KeyEventArgs e) =>
                    {
                        if (e.KeyCode == Keys.Delete)
                        {
                            (sender as CheckEdit).EditValue = null;
                        }
                    };
                    break;

                case 11:
                    tmp = new RepositoryItemTimeEdit();
                    (tmp as RepositoryItemTimeEdit).AllowNullInput = DefaultBoolean.True;
                    break;

                case 12:
                    tmp = new RepositoryItemPictureEdit();
                    tmp.NullText = " ";
                    (tmp as RepositoryItemPictureEdit).SizeMode = PictureSizeMode.Stretch;
                    break;
                case 2:
                    tmp = new RepositoryItemTextEdit();
                    break;
                case 13:
                    tmp = new RepositoryItemMemoEdit();
                    break;

                default:
                    tmp = null;
                    break;
            }
            if (tmp != null)
            {
                tmp.Name = dr["FieldName"].ToString();
            }
            return tmp;
        }

        public CDTRepGridLookup GenRIGridLookupEdit(DataRow drField)
        {
            DataTable dt;
            CDTRepGridLookup tmp = new CDTRepGridLookup();
            string refField = drField["RefField"].ToString();
            string refTable = drField["RefTable"].ToString();
            string condition = drField["refCriteria"].ToString();
            string Dyncondition = drField["DynCriteria"].ToString();
            string displayMember = drField["DisplayMember"].ToString();
            bool isMaster = true;
            int n = 0;
            if (refTable == "DMVT")
            {
            }
            CDTData data = this.GetDataForLookup(refTable, condition, Dyncondition, ref isMaster, ref n);


            data.DataIndexList = n;
            FormDesigner fd = new FormDesigner(data);

            dt =  data.DsStruct.Tables[0]; 

            int k = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                DataRow dr1 = dt.Rows[i];
                if (dr1["TabIndex"].ToString() == "-1" && !(dr1["Type"].ToString() == "7" || dr1["Type"].ToString() == "4" || dr1["Type"].ToString() == "0" || dr1["Type"].ToString() == "3" || dr1["Type"].ToString() == "6"))
                {
                    continue;
                }

                string fieldList = "*";
                if (data.DrTable.Table.Columns.Contains("RefFList") && data.DrTable["RefFList"] != DBNull.Value && data.DrTable["RefFList"].ToString() != string.Empty)
                    fieldList = data.DrTable["RefFList"].ToString();
                if (fieldList != "*")
                {
                    if (!fieldList.Contains(dr1["FieldName"].ToString()))
                        continue;
                }
                k++;
                CDTGridColumn gcl = fd.GenGridColumn(dr1, 0, false);
                if (dr1["EditMask"].ToString() != string.Empty)
                {
                    gcl.DisplayFormat.FormatType = FormatType.Numeric;
                    gcl.DisplayFormat.FormatString = dr1["EditMask"].ToString();
                }
                tmp.View.Columns.Add(gcl);
                //if (k == 8)
                //{
                //    break;
                //}
            }
            BindingSource bs = new BindingSource();
            if (isMaster)
            {
                bs.DataSource = data.DsData.Tables[0];
            }
            else
            {
                bs.DataSource = data.DsData.Tables[1];
            }
            tmp.Data = data;
            if (data.dataType == DataType.Single)
            {
                try { data.DataSoureChanged -= Data_DataSoureChanged; }
                catch
                { }
                data.DataSoureChanged += Data_DataSoureChanged;
            }
            tmp.DataSource = bs;
            tmp.refTable = refTable;
            tmp.ValueMember = refField;
            tmp.DymicCondition = Dyncondition;
            tmp.Name = drField["FieldName"].ToString();
            tmp.Condition = condition;
            tmp.DataIndex = n;
            tmp.NullText = string.Empty;
            this._Rlist.Add(new RLookUp_CDTData(tmp, n));
            this._rlist.Add(tmp);

            if (int.Parse(drField["Type"].ToString()) == 1)
            {
                tmp.DisplayMember = refField;
            }
            else
            {
                tmp.DisplayMember = displayMember;
            }
            tmp.PopupFormMinSize = new Size(600, 100);
            tmp.View.OptionsView.ShowFooter = true;
            tmp.View.IndicatorWidth = 40;
            tmp.ImmediatePopup = true;
            tmp.TextEditStyle = TextEditStyles.Standard;
            tmp.View.OptionsView.ShowAutoFilterRow = true;
            tmp.View.OptionsView.ColumnAutoWidth = false;
            tmp.TextEditStyle = TextEditStyles.Standard;
            //tmp.View.GridControl.UseEmbeddedNavigator = true;
            //tmp.ReadOnly = true;
            tmp.View.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(this.View_CustomDrawRowIndicator);

            if (tmp.refTable.Substring(0, 1) != "w")
            {

                if (data.dataType == DataType.Single)
                {
                    EditorButton plusBtn = new EditorButton(data, ButtonPredefines.Plus);
                    plusBtn.Shortcut = new KeyShortcut(Keys.F2);
                    plusBtn.ToolTip = "F2 - New";
                    tmp.Buttons.Add(plusBtn);
                    foreach (DataColumn col in data.DrTable.Table.Columns)
                    {
                        //MessageBox.Show(col.Caption);
                    }
                    if (data.DrTable.Table.Columns.Contains("sInsert"))
                    {
                        string sInsert = data.DrTable["sInsert"].ToString();
                        bool t = false;
                        if (sInsert == "1") t = true;
                        else if (sInsert == "0") t = false;
                        if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
                        {
                            if (sInsert != "" && !t)
                            {
                                plusBtn.Visible = false;
                            }
                        }
                    }
                }
            }

            tmp.Button_click += new ButtonPressedEventHandler(this.tmp_RIButton_click);
            EditorButton refreshData = new EditorButton(data, ButtonPredefines.Ellipsis);
            refreshData.Shortcut = new KeyShortcut(Keys.F5);
            refreshData.ToolTip = "F5 - Refresh";
            tmp.Buttons.Add(refreshData);

            tmp.Popup += new EventHandler(this.RIGridLookupEdit_Popup);
            
            if (drField["AllowNull"].ToString() == "0")
            {
                tmp.AllowNullInput = DefaultBoolean.False;
            }
            tmp.KeyDown += new KeyEventHandler(this.RiGridLookupEdit_KeyDown);
            tmp.KeyUp += RiGridLookupEdit_KeyUp;

            tmp.DymicCondition = Dyncondition;
            tmp.AllowNullInput = DefaultBoolean.True;
            tmp.CloseUp += new CloseUpEventHandler(RItmp_CloseUp);

            tmp.EditValueChanged += new EventHandler(this.RIGridLookupEdit_EditValueChanged);
            tmp.Validating += new CancelEventHandler(this.RIGridLookupEdit_Validating);
            tmp.View.KeyDown += new KeyEventHandler(this.View_KeyDown);
            tmp.View.ColumnFilterChanged += View_ColumnFilterChanged1;
            tmp.View.TopRowChanged += View_TopRowChanged1;
            return tmp;
        }

        private void Tmp_CloseUp(object sender, CloseUpEventArgs e)
        {
            GridLookUpEdit tmp = (GridLookUpEdit)sender;
            tmp.ErrorText = "";
            CDTRepGridLookup ri = tmp.Tag as CDTRepGridLookup;
            if (ri == null) return;
            int k = _lstRep.IndexOf(ri) + 1;

            if (!ri.Data.FullData)
                this.RefreshLookup(ri.Data);
            BindingSource bs = tmp.Properties.DataSource as BindingSource;
            try
            {
                if (tmp.Tag != null && bs != null)
                {
                    bs.Position = int.Parse((tmp.Tag as CDTRepGridLookup).bsCur.ToString());
                }
                setDynFiter(ri);

            }
            catch { }
        }

        private void View_TopRowChanged1(object sender, EventArgs e)
        {
            try
            {

                
                GridView view = sender as GridView;
                if (view == null) return;

                int visibleRowCount = view.RowCount;
                int topRowIndex = view.PrevTopRowIndex;

                // Kiểm tra nếu người dùng đã cuộn đến gần cuối danh sách
                GridLookUpEdit tmp;
                if (topRowIndex >= visibleRowCount - 50)
                {
                    if (view.GridControl.Parent != null)
                    {
                        var popupForm = view.GridControl.Parent as DevExpress.XtraEditors.Popup.PopupGridLookUpEditForm;
                        tmp = popupForm.OwnerEdit as GridLookUpEdit;
                        CDTRepGridLookup ri = tmp.Tag as CDTRepGridLookup;
                        if (!ri.Data.FullData)
                        {
                            this.RefreshLookup(ri.Data);
                            //setDynFilter(ri);
                            setDynFiter(ri);
                        }
                    }
                    //LoadMoreData();

                }
            }
            catch { }
        }

        private void View_ColumnFilterChanged1(object sender, EventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                // Kiểm tra nếu người dùng đã cuộn đến gần cuối danh sách
                int visibleRowCount = view.RowCount;
                GridLookUpEdit tmp;
                if (view.GridControl.Parent == null) return;
                var popupForm = view.GridControl.Parent as DevExpress.XtraEditors.Popup.PopupGridLookUpEditForm;
                tmp = popupForm.OwnerEdit as GridLookUpEdit;
                CDTRepGridLookup ri = tmp.Tag as CDTRepGridLookup;
                if (visibleRowCount == 0 && !ri.Data.FullData)
                {
                    this.RefreshLookup(ri.Data);
                    setDynFiter(ri);
                    //View_ColumnFilterChanged1(sender, e);
                }
                
            }
            catch { }
        }
    

        private void Data_DataSoureChanged(object sender, EventArgs e)
        {
            CDTData dataLookup = (sender as DataFactory.CDTData);
            RefreshLookup(dataLookup);
        }

        private void RiGridLookupEdit_KeyUp(object sender, KeyEventArgs e)
        {
            GridLookUpEdit tmp = sender as GridLookUpEdit;
            if (((!tmp.IsPopupOpen && (tmp.EditValue != null)) && (tmp.EditValue.ToString() == string.Empty)) && (e.KeyCode == Keys.Back))
            {
                tmp.EditValue = null;
            }
        }

        private TreeListColumn GenTreeListColumn(DataRow dr, int exColNum, bool checkData)
        {
            TreeListColumn tlcl = new TreeListColumn();
            tlcl.Name = "cl" + dr["FieldName"].ToString();
            tlcl.FieldName = dr["FieldName"].ToString();
            tlcl.Caption = (Config.GetValue("Language").ToString() == "0") ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
            tlcl.VisibleIndex = int.Parse(dr["TabIndex"].ToString()) + exColNum;

            if (!checkData)
            {
                tlcl.Visible = dr["Visible"].ToString() == "1";
            }
            return tlcl;
        }

        internal TreeList GenTreeListControl(DataRow drTable, DataTable dt)
        {

            TreeList tlMain = new TreeList();
            int bType = int.Parse(drTable["Type"].ToString());
            tlMain.BeginInit();
            tlMain.Dock = DockStyle.Fill;
            tlMain.KeyFieldName = drTable["Pk"].ToString();
            tlMain.ParentFieldName = drTable["ParentPk"].ToString();
            tlMain.OptionsView.AutoWidth = false;
            tlMain.OptionsView.EnableAppearanceEvenRow = true;
            tlMain.Visible = false;
            tlMain.OptionsBehavior.Editable = false;
            switch (bType)
            {
                case 1:
                case 4:
                    tlMain.OptionsBehavior.Editable = true;
                    break;
            }
            int reCol = 0;
            int deCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                if (Int32.Parse(dr["Type"].ToString()) == 1 && tlMain.ParentFieldName.ToUpper() != dr["FieldName"].ToString().ToUpper())
                    if (bool.Parse(dr["Showref"].ToString()) && dr["DisplayMember"].ToString() != string.Empty)
                        reCol++;
                //if (tlMain.ParentFieldName.ToUpper() == dr["FieldName"].ToString().ToUpper())
                //    deCol++;
            }
            TreeListColumn[] tlcls = new TreeListColumn[(dt.Rows.Count + reCol) - deCol];
            int exColNum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                if (tlMain.ParentFieldName.ToUpper() == dr["FieldName"].ToString().ToUpper())
                {
                    exColNum--;
                }
                else
                {
                    TreeListColumn tlcl = this.GenTreeListColumn(dr, exColNum, false);
                    if (bool.Parse(dr["visible"].ToString())) tlcl.VisibleIndex = -1;
                    RepositoryItem ri = this.GenRepository(dr);
                    if (ri != null)
                    {
                        tlMain.RepositoryItems.Add(ri);
                        tlcl.ColumnEdit = ri;
                    }
                    tlcls[i + exColNum] = tlcl;
                    if ((int.Parse(dr["Type"].ToString()) == 1) && bool.Parse(dr["Showref"].ToString()) && (dr["DisplayMember"].ToString() != string.Empty))
                    {
                        TreeListColumn tlcl1 = this.GenTreeListColumn(dr, exColNum, false);
                        RepositoryItem ri1 = this.GenRepository(dr);
                        if (ri1 != null)
                        {
                            string caption;
                            string displayMember = dr["DisplayMember"].ToString();
                            ((CDTRepGridLookup)ri1).DisplayMember = displayMember;
                            tlMain.RepositoryItems.Add(ri1);
                            if (Config.GetValue("Language").ToString() == "0")
                            {
                                caption = "Tên " + dr["LabelName"].ToString().ToLower();
                            }
                            else
                            {
                                caption = dr["LabelName2"].ToString() + " name";
                            }
                            if (!(dr["AllowNull"].ToString() == "1"))
                            {
                                caption = "*" + caption;
                            }
                            tlcl1.Caption = caption;
                            if (bool.Parse(dr["visible"].ToString()))
                                tlcl1.VisibleIndex = tlcl.VisibleIndex++;
                            else tlcl1.VisibleIndex = -1;
                            tlcl1.Width += 100;
                            tlcl1.ColumnEdit = ri1;
                            tlcl1.Visible = tlcl.Visible;
                        }
                        exColNum++;
                        tlcls[i + exColNum] = tlcl1;
                    }
                }
            }
            tlMain.Columns.AddRange(tlcls);
            tlMain.EndInit();
            return tlMain;
        }

        private CDTData GetDataForLookup(string tableName, string condition, string DynCondition, ref bool isMaster, ref int n)
        {
            DataSingle data = null;
            foreach (DataSingle d in this._lstData)
            {
                n = this._lstData.IndexOf(d);
                if (d.DrTable == null && d._tableName.ToLower()==tableName.ToLower())
                {
                    d.ReGetInfor();
                    if (!data.FullData)
                        data.GetDataForLookup(this._data);
                    if (this._lstData.Exists(c => c._tableName == data._tableName && c.Condition == data.Condition && c.DynCondition == data.DynCondition))
                    {

                    }
                    else
                    {
                        this._lstData.Add(data);
                        n = this._lstData.Count - 1;
                    }
                }
                if (d.DrTable["tableName"].ToString().ToUpper() == tableName.ToUpper() && d.Condition==condition )//Tìm đúng bảng&& d.DynCondition==DynCondition
                {
                    data = d;
                        isMaster = true;
                    break;
                }
                else if(d.DrTableMaster!=null && d.DrTableMaster["tableName"].ToString().ToUpper() == tableName.ToUpper())
                {
                    if (d.dataType == DataType.MasterDetail)
                    {
                        isMaster = true;
                    }
                    else if (d.dataType == DataType.Detail)
                    {
                        isMaster = false;
                    }
                    data = d;
                    break;

                }
                     
                              
            }
            if (data == null)
            {
                DataSingle dataInPulibc = DataFactory.publicCDTData.findCDTData(tableName, condition, DynCondition);
                if (dataInPulibc != null)
                {
                    data = dataInPulibc;
                    if (!data.FullData)
                        data.GetDataForLookup(this._data);
                    if (this._lstData.Exists(c => c._tableName == data._tableName && c.Condition == data.Condition && c.DynCondition == data.DynCondition))
                    {

                    }
                    else
                    {
                        this._lstData.Add(data);
                        n = this._lstData.Count - 1;
                    }
                }
                else
                {
                    string sysPackageID = Config.GetValue("sysPackageID").ToString();
                    data = DataFactory.DataFactory.Create(DataType.Single, tableName, sysPackageID) as DataSingle;
                    if (data == null)
                        return null;
                    data.Condition = condition;
                    data.DynCondition = DynCondition;
                    if (((tableName == "sysTable") || (tableName == "sysField")) || (tableName == "sysDataConfig") || (tableName == "sysPackage"))
                    {
                        data.GetData4Lookup();
                        data.FullData = true;
                    }
                    else
                    {
                        data.GetDataForLookup(this._data);
                    }


                    this._lstData.Add(data);
                    if(data.dataType!=DataType.Report)// && data.dataType != DataType.MasterDetail)
                        DataFactory.publicCDTData.AddCDTData(data);
                    if (data.DsData != null && data.DsData.Tables.Count > 0)
                    {
                        CDTTable.AddTable(data.DsData.Tables[0], data.DrTable, data.FullData);
                    }
                    n = this._lstData.Count - 1;
                }
                return data;
            }
            if (data.DsData == null)
            {
                data.GetData4Lookup();
               // data.getDataSroll(0);
            }
            return data;
        }

        private void GridLookupEdit_EditValueChanged(object sender, EventArgs e)
        {
            try { 
            CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
            tmp.Refresh();
                if (tmp.EditValue == null) return;
            string value = tmp.EditValue.ToString();


            BindingSource bs = tmp.Properties.DataSource as BindingSource;
            if (_data.DrCurrentMaster != null && _data.DrCurrentMaster.RowState != DataRowState.Unchanged)
            {

                if (((this._formAction != FormAction.View) && (this._formAction != FormAction.Delete)) && (tmp.EditValue != null))
                {


                    if (!tmp.Data.FullData)
                    {

                       // this.RefreshLookup(tmp.DataIndex);
                    }
                }
            }
            int index = tmp.Properties.GetIndexByKeyValue(value);
            if ((index >= 0) && (value != string.Empty))
            {
                DataTable dt = bs.DataSource as DataTable;
                DataRow drData;
                DataRowView drDataView = tmp.Properties.GetRowByKeyValue(value) as DataRowView;
                if (drDataView != null)
                {
                    drData = drDataView.Row;
                    if ((drData != null) && (this._formAction == FormAction.Filter))
                    {
                        for (int i = 0; i < drData.Table.Columns.Count; i++)
                        {
                            (this._data as DataReport).reConfig.NewKeyValue("@" + drData.Table.Columns[i].ColumnName, drData[i]);
                        }
                    }
                    else if (drData != null)
                    {
                        if (_data.DrCurrentMaster != null && _data.DrCurrentMaster.RowState != DataRowState.Unchanged)
                        {
                           // this._data.SetValuesFromList(tmp.Properties.Name, value, drData, false);
                            this.RefreshDataForLookup(tmp.Name, false);
                        }
                    }
                }


            }

                //this.setDynFiter(tmp);
            }
            catch (Exception ex)
                { }

            CDTGridLookUpEdit tm1 = sender as CDTGridLookUpEdit;
        }

        private void GridLookupEdit_KeyDown(object sender, KeyEventArgs e)
        {
            CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
            if (!tmp.Allownull)
            {
                
                if ((!tmp.IsPopupOpen && (this._formAction != FormAction.Filter) && ((tmp.EditValue == null) || (tmp.EditValue.ToString() == string.Empty))) && (e.KeyCode == Keys.Return))
                {
                    tmp.ShowPopup();
                    e.Handled = true;
                }
                if (((!tmp.IsPopupOpen && (tmp.EditValue != null)) && (tmp.EditValue.ToString() != string.Empty)) && (e.KeyCode == Keys.Delete))
                {
                    tmp.EditValue = null;
                }
            }
        }

        private void GridLookupEdit_Popup(object sender, EventArgs e)
        {
            CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
            //int n = -1;
            //int i = 0;
            //for (i = 0; i < this._Glist.Count; i++)
            //{
            //    if (this._Glist[i].glk == tmp)
            //    {
            //        n = this._Glist[i].dataIndex;
            //        break;
            //    }
            //}

            this.RefreshLookup(tmp.Data);
            setDynFilter(tmp);
        }

        private void GridLookupEdit_Validated(object sender, EventArgs e)
        {
            CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
            // this.setDynFiter(tmp);
        }

        private void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            CDTRepGridLookup tmp = e.Column.ColumnEdit as CDTRepGridLookup;
            if (tmp != null)
            {
                //this.setRepFilter(tmp);
            }
        }

        private void gvMain_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                GridView gvMain = sender as GridView;
                try
                {
                    gvMain.FocusedColumn = gvMain.VisibleColumns[0];
                }
                catch
                {
                }
            }
        }

        private void Plus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Tag != null)
            {

                CDTGridLookUpEdit tmp = sender as CDTGridLookUpEdit;
                if (e.Button.ToolTip == "F5 - Refresh")
                {
                    int n = -1;
                    int i = 0;
                    for (i = 0; i < this._Glist.Count; i++)
                    {
                        if (this._Glist[i].glk == tmp)
                        {
                            n = this._Glist[i].dataIndex;
                            break;
                        }
                    }
                    this.RefreshLookupAllowFull(tmp.Data);
                    return;
                }
                if (!tmp.Data.FullData)
                {
                    //int n = -1;
                    //int i = 0;
                    //for (i = 0; i < this._Glist.Count; i++)
                    //{
                    //    if (this._Glist[i].glk == tmp)
                    //    {
                    //        n = this._Glist[i].dataIndex;
                    //        break;
                    //    }
                    //}
                    this.RefreshLookup(tmp.Data);
                }
                CDTData d = e.Button.Tag as CDTData;
                bool ok = false;
                BindingSource bs = null;
                FrmSingleDt frm = null;
                FormAction fAction = FormAction.View;
                if (e.Button.ToolTip == "F3 - Edit" && d != null)
                {

                    bs = tmp.Properties.DataSource as BindingSource;
                    //bs.DataSource = d.DsData.Tables[0];  
                    frm = new FrmSingleDt(d, bs, true);

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        ok = true;
                        fAction = FormAction.Edit;
                    }
                }
                else if (e.Button.ToolTip == "F6 - New" && d != null)
                {
                    // BindingSource bs = tmp.Properties.DataSource as BindingSource;

                    bs = new BindingSource();
                    bs.DataSource = d.DsData.Tables[0];
                    frm = new FrmSingleDt(d, bs);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        ok = true;
                        fAction = FormAction.New;
                    }
                }


                if (ok && bs != null)
                {
                    tmp.Properties.DataSource = bs;
                    string tableName;
                    if (tmp.refTable != null)
                    {
                        tableName = tmp.refTable;
                    }
                    else
                    {
                        tableName = tmp.refTable;
                    }
                    List<string> _GlistNametmp = new List<string>();
                    foreach (LookUp_CDTData ldtmp in frm._frmDesigner._Glist)
                    {
                        _GlistNametmp.Add(ldtmp.glk.refTable);
                    }
                    foreach (RLookUp_CDTData rgc in this._Rlist)
                    {

                        CDTRepGridLookup rg = rgc.rglk;
                        if (rg.refTable == "DMKH")
                        {
                        }
                        if ((rg.refTable.ToUpper() == tableName.ToUpper()) && (rg.View.ViewCaption.ToUpper() == d.Condition.ToUpper()))
                        {
                            rg.DataSource = null;
                            bs.DataSource = d.DsData.Tables[0];
                            rg.DataSource = bs;
                        }
                        if (_GlistNametmp.Contains(rg.refTable))
                        {
                            this.RefreshLookupAllowFull(tmp.Data);
                        }
                    }
                    foreach (LookUp_CDTData rgc in this._Glist)
                    {
                        CDTGridLookUpEdit rg = rgc.glk;
                        if ((rg.refTable.ToUpper() == tableName.ToUpper()) && (rg.Condition == d.Condition))
                        {

                            rg.Properties.DataSource = null;
                            rg.Properties.DataSource = bs;
                        }
                        if (_GlistNametmp.Contains(rg.refTable))
                        {
                            this.RefreshLookupAllowFull(tmp.Data);
                        }
                    }
                    if ((tmp.GetType() == typeof(CDTGridLookUpEdit)) || (this._gcMain == null))
                    {
                        int index = bs.Count - 1;
                        if (fAction == FormAction.Edit)
                        {
                            Object value = tmp.EditValue;
                            index = bs.Find(tmp.Properties.ValueMember, value);
                        }
                        tmp.EditValue = (bs.List[index] as DataRowView)[tmp.Properties.ValueMember];
                    }
                    else
                    {
                        object t = (bs.List[bs.Count - 1] as DataRowView)[tmp.Properties.ValueMember];
                        (this._gcMain.MainView as GridView).SetFocusedRowCellValue((this._gcMain.MainView as GridView).FocusedColumn, t);
                        (this._gcMain.MainView as GridView).UpdateCurrentRow();
                        tmp.EditValue = t;
                        this.RIGridLookupEdit_EditValueChanged(tmp, new EventArgs());
                    }

                }
            }
        }

        public void RefreshDataForLookup()
        {
            int i;
            BindingSource bs;
            for (i = 1; i < this._lstData.Count; i++)
            {
                if (!this._lstData[i].FullData)
                {
                    this._lstData[i].GetDataForLookup(this._data);
                }
            }
            for (i = 0; i < this._Glist.Count; i++)
            {
                bs = this._Glist[i].glk.Properties.DataSource as BindingSource;
                bs.DataSource = this._lstData[this._Glist[i].dataIndex].DsData.Tables[0];
            }
            for (i = 0; i < this._Rlist.Count; i++)
            {
                bs = this._Rlist[i].rglk.DataSource as BindingSource;
                try
                {
                    bs.DataSource = this._lstData[this._Rlist[i].dataIndex].DsData.Tables[0];
                }
                catch(Exception ex)
                {

                }
            }
        }

        private void RefreshDataForLookup( string controlFrom, bool isDetail)
        {
            string formulaDetail;
            string[] str;
            int i;
            int n;
            List<string> lstStr = new List<string>();
            if (isDetail)
            {
                foreach (DataRow drField in this._data.DsStruct.Tables[1].Rows)
                {
                    formulaDetail = drField["FormulaDetail"].ToString();
                    if (!(formulaDetail == string.Empty))
                    {
                        str = formulaDetail.Split(".".ToCharArray());
                        if (!(controlFrom.ToUpper() != str[0].ToUpper()) || lstStr.Contains(str[0].ToUpper()))
                        {
                            lstStr.Add(drField["FieldName"].ToString().ToUpper());
                        }
                    }
                }
                foreach (string s in lstStr)
                {
                    i = 0;
                    while (i < this._Rlist.Count)
                    {
                        if (this._Rlist[i].rglk.Name.ToUpper() == s)
                        {
                            n = this._Rlist[i].dataIndex;
                            CDTData dataRef = this._lstData[n];
                            this.RefreshLookup(dataRef);
                        }
                        i++;
                    }
                }
            }
            lstStr.Clear();
            foreach (DataRow drField in this._data.DsStruct.Tables[0].Rows)
            {
                formulaDetail = drField["FormulaDetail"].ToString();
                if (!(formulaDetail == string.Empty))
                {
                    str = formulaDetail.Split(".".ToCharArray());
                    if (!(controlFrom.ToUpper() != str[0].ToUpper()) || lstStr.Contains(str[0].ToUpper()))
                    {
                        lstStr.Add(drField["FieldName"].ToString().ToUpper());
                    }
                }
            }
            foreach (string s in lstStr)
            {
                for (i = 0; i < this._Glist.Count; i++)
                {
                    if (this._Glist[i].glk.Name.ToUpper() == s)
                    {
                        n = this._Glist[i].dataIndex;
                        CDTData dataRef = this._lstData[n];
                        this.RefreshLookup(dataRef);
                    }
                }
            }
        }

        public void RefreshDataLookupForColChanged()
        {
            int i;
            BindingSource bs;
            for (i = 1; i < this._lstData.Count; i++)
            {
                if (!this._lstData[i].FullData)
                {
                    this._lstData[i].GetData();
                }
            }
            for (i = 0; i < this._Glist.Count; i++)
            {
                bs = this._Glist[i].glk.Properties.DataSource as BindingSource;
                bs.DataSource = this._lstData[this._Glist[i].dataIndex].DsData.Tables[0];
            }
            for (i = 0; i < this._Rlist.Count; i++)
            {
                bs = this._Rlist[i].rglk.DataSource as BindingSource;
                bs.DataSource = this._lstData[this._Rlist[i].dataIndex].DsData.Tables[0];
            }
        }

        public void RefreshFormulaDetail()
        {
            for (int i = 0; i < this._Glist.Count; i++)
            {
                CDTGridLookUpEdit tmp = this._Glist[i].glk;
                if (tmp.EditValue != null)
                {
                    string value = tmp.EditValue.ToString();
                    BindingSource bs = tmp.Properties.DataSource as BindingSource;
                    int index = tmp.Properties.GetIndexByKeyValue(value);
                    if ((index >= 0) && !(value == string.Empty))
                    {
                        DataTable dt = bs.DataSource as DataTable;
                        DataRow drData = dt.Rows[index];
                        if (drData != null)
                        {

                            //if (this._data.DrCurrentMaster[(bs.DataSource as DataTable).TableName].ToString() != value)  

                            //    this._data.SetValuesFromList((bs.DataSource as DataTable).TableName, value, drData, true);
                        }
                    }
                }
            }
        }

        public void RefreshLookup(CDTData data1)
        {
            //if (dataIndex >= 0 && dataIndex < this._lstData.Count)
            //{
            DataSingle data = (DataSingle)data1;
            // if (data.FullData) return;
            if ((((data.dataType != DataType.MasterDetail) || !data.DrTableMaster.Table.Columns.Contains("TableName")) || (data.DrTableMaster["TableName"].ToString() != "sysTable")))
            {
                int i;
                BindingSource bs;
                if (!data.FullData && data.dataType == DataType.Single)
                {
                    //if (data.DsData == null)
                    //{
                    //    data.getDataSroll(0);
                    //}
                    //else
                    //{
                    //    data.getDataSroll(data.DsData.Tables[0].Rows.Count);
                    //}    
                    data.GetData4Lookup();
                    data.FullData = true;
                    if (data.DsData != null && data.DsData.Tables.Count > 0)
                    {
                        CDTTable.AddTable(data.DsData.Tables[0], data.DrTable, data.FullData);
                    }
                }
                //else if(data.dataType == DataType.MasterDetail)
                //{ 

                //}
                for (i = 0; i < this._Glist.Count; i++)
                {
                    //if (this._Glist[i].dataIndex == dataIndex)
                    //{
                    bs = this._Glist[i].glk.Properties.DataSource as BindingSource;
                    if (data.DsData == null) return;
                    if ((bs.DataSource as DataTable).TableName == data.DsData.Tables[0].TableName)
                    {
                        bs.DataSource = data.DsData.Tables[0];
                    }
                    //else
                    //{
                    //    bs.DataSource = data.DsData.Tables[1];
                    //}
                    // }
                }
                for (i = 0; i < this._Rlist.Count; i++)
                {
                    //if (this._Rlist[i].dataIndex == dataIndex)
                    //{

                    bs = this._Rlist[i].rglk.DataSource as BindingSource;
                    if (data.DsData == null) return;
                    if (bs != null && (bs.DataSource == null || (bs.DataSource as DataTable).TableName == data.DsData.Tables[0].TableName))
                    {
                        bs.DataSource = data.DsData.Tables[0];
                    }
                    //else
                    //{
                    //    bs.DataSource = data.DsData.Tables[1];
                    //}
                    //}
                }
            }

            //}
        }
        private void RefreshLookupAllowFull(CDTData data)
        {
            //if (dataIndex >= 0)
            //{
               // CDTData data = this._lstData[dataIndex];
                //if ((((data.dataType != DataType.MasterDetail) || !data.DrTableMaster.Table.Columns.Contains("TableName")) || (data.DrTableMaster["TableName"].ToString() != "sysTable")))//(data.dataType != DataType.MasterDetail) ||
                //{
                    int i;
                    BindingSource bs;
                    data.GetData();
            for (i = 0; i < this._Glist.Count; i++)
            {
                //if (this._Glist[i].dataIndex == dataIndex)
                //{
                bs = this._Glist[i].glk.Properties.DataSource as BindingSource;
                if (bs == null) continue;
                if ((bs.DataSource as DataTable).TableName == data.DsData.Tables[0].TableName)
                {
                    try
                    {
                        bs.DataSource = data.DsData.Tables[0];
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        bs.DataSource = data.DsData.Tables[1];
                    }
                    catch { }

                }
                // }
            }
            for (i = 0; i < this._Rlist.Count; i++)
            {
                //if (this._Rlist[i].dataIndex == dataIndex)
                //{
                bs = this._Rlist[i].rglk.DataSource as BindingSource;
                if (bs == null) continue;
                if ((bs.DataSource as DataTable).TableName == data.DsData.Tables[0].TableName)
                {
                    try
                    {
                        bs.DataSource = data.DsData.Tables[0];
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        bs.DataSource = data.DsData.Tables[1];
                    }
                    catch { }

                }
                //}
            }
                //}
           // }
        }
        public void RefreshViewForLookup()
        {
            for (int i = 1; i < this._lstData.Count; i++)
            {
                if ((this._lstData[i].DrTable["CollectType"].ToString() == "-1") && this._lstData[i].FullData)
                {
                    BindingSource bs;
                    this._lstData[i].GetData();
                    for (int j = 0; j < this._Glist.Count; j++)
                    {
                        if (this._Glist[j].dataIndex == i)
                        {
                            bs = this._Glist[j].glk.Properties.DataSource as BindingSource;
                            bs.DataSource = this._lstData[this._Glist[j].dataIndex].DsData.Tables[0];
                        }
                    }
                    for (int k = 0; k < this._Rlist.Count; k++)
                    {
                        if (this._Rlist[k].dataIndex == i)
                        {
                            bs = this._Rlist[k].rglk.DataSource as BindingSource;
                            bs.DataSource = this._lstData[this._Rlist[k].dataIndex].DsData.Tables[0];
                        }
                    }
                }
            }

        }
        public void RefreshGridLookupEdit()
        {
            if (this.formAction == FormAction.New || this.formAction == FormAction.Copy)
            {
                for (int j = 0; j < this._Glist.Count; j++)
                {
                    if (this._Glist[j].glk.EditValue != null && this._Glist[j].glk.EditValue != DBNull.Value)
                        GridLookupEdit_EditValueChanged(this._Glist[j].glk, new EventArgs());

                }
            }
        }
        private void RItmp_CloseUp(object sender, CloseUpEventArgs e)
        {
            GridLookUpEdit tmp = sender as GridLookUpEdit;
            tmp.EditValue = e.Value;
            //RIGridLookupEdit_EditValueChanged(sender, new EventArgs());

            if (tmp.EditValue != null)
                RIGridLookupEdit_Validating(sender, new EventArgs());
            //tmp.Properties.View.ActiveFilterString = "";
            //tmp.Properties.View.OptionsFilter.BeginUpdate();
            //tmp.Properties.View.ApplyFindFilter("");
            //tmp.Properties.View.ActiveFilterEnabled = true;
            //tmp.Properties.View.OptionsFilter.EndUpdate();
            //tmp.Properties.View.RefreshEditor(true);
            //ClearFilter();
            //Đoạn này nếu bỏ filter thì lọc sẽ lâu mà để filter thì sẽ bị ẩn những dòng đã chọn ở trên

            // CDTRepGridLookup ri = tmp.Tag as CDTRepGridLookup;
            // if (ri == null) return;
            //if (ri.isFiltered)
            //{
            //    BindingSource bs = ri.DataSource as BindingSource;
            //    bs.DataSource = ri.Data.DsData.Tables[0];
            //    ri.DataSource = bs;
            //    ri.isFiltered = false;
            //}
        }
        private void RIGridLookupEdit_EditValueChanged(object sender, EventArgs e)
        {if (this._gcMain == null) return;
            if (((this._formAction != FormAction.View) && (this._formAction != FormAction.Delete)) && (this._formAction != FormAction.Filter))
            {
                GridLookUpEdit tmp = sender as GridLookUpEdit;
                RepositoryItemGridLookUpEdit tmp1 = tmp.Properties as RepositoryItemGridLookUpEdit;
                
                GridView gv = this._gcMain.Views[0] as GridView;
                if (tmp.EditValue != null)
                {
                    string value = tmp.EditValue.ToString();
                    if (value != string.Empty)
                    {
                        BindingSource bs = tmp.Properties.DataSource as BindingSource;
                        try
                        {
                            //int index = bs.Position;// 
                            int index = tmp.Properties.GetIndexByKeyValue(value);
                            if (index < 0)
                            {
                                index = bs.Count - 1;
                            }
                            if (tmp.Tag != null)
                            {
                                (tmp.Tag as CDTRepGridLookup).bsCur = index;
                            }
                            DataTable dt = bs.DataSource as DataTable;
                            if (index < 0) return;
                            DataRow drData = dt.Rows[index];
                            DataRowView drDataView = tmp.Properties.GetRowByKeyValue(value) as DataRowView;

                            if (drDataView != null)
                            {
                                drData = drDataView.Row;
                                DataRow drDetail = gv.GetDataRow(gv.FocusedRowHandle);
                                if (this._data.dataType != DataType.MasterDetail)
                                {
                                    //this._data.SetValuesFromList(tmp.Properties.Name, value, drData, false);
                                }
                                this.RefreshDataForLookup(tmp.Properties.Name, this._data.dataType == DataType.MasterDetail);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void RiGridLookupEdit_KeyDown(object sender, KeyEventArgs e)
        {
            GridLookUpEdit tmp = sender as GridLookUpEdit;
            if ((!tmp.IsPopupOpen && ((tmp.EditValue == null) || (tmp.EditValue.ToString() == string.Empty))) && (e.KeyCode == Keys.Return))
            {
                if (tmp.Properties.AllowNullInput == DefaultBoolean.False)
                {
                    tmp.ShowPopup();
                    e.Handled = true;
                }
            }
            if (((!tmp.IsPopupOpen && (tmp.EditValue != null)) && (tmp.EditValue.ToString() != string.Empty)) && (e.KeyCode == Keys.Delete))
            {
                tmp.EditValue = null;
                try
                {
                    
                }
                catch { }
            }
            
        }

        private void RIGridLookupEdit_Popup(object sender, EventArgs e)
        {
            GridLookUpEdit tmp = (GridLookUpEdit)sender;
           // MessageBox.Show(tmp.Properties.View.ActiveFilterString);
            tmp.ErrorText = "";
            CDTRepGridLookup ri = tmp.Tag as CDTRepGridLookup;
            if (ri == null) return;
            int k = _lstRep.IndexOf(ri) + 1;
            // ri.View.ActiveFilter
            //int n = -1;
            //int i = 0;
            //for (i = 0; i < this._Rlist.Count; i++)
            //{
            //    if (this._Rlist[i].rglk.Name == tmp.Properties.Name)
            //    {
            //        n = this._Rlist[i].dataIndex;
            //        break;
            //    }
            //}
            if (!ri.Data.FullData)
                this.RefreshLookup(ri.Data);
            BindingSource bs = tmp.Properties.DataSource as BindingSource;
            try
            {
                if (tmp.Tag != null && bs !=null)
                {
                    bs.Position = int.Parse((tmp.Tag as CDTRepGridLookup).bsCur.ToString());
                }
               setDynFiter(ri);

            }
            catch { }
            

        }

        private void RIGridLookupEdit_Validating(object sender, EventArgs e)
        {
            if (this._gcMain == null) return;
            if (((this._formAction != FormAction.View) && (this._formAction != FormAction.Delete)) && (this._formAction != FormAction.Filter))
            {
                GridLookUpEdit tmp = sender as GridLookUpEdit;
                    if (tmp.EditValue != null)
                {
                    string value = tmp.EditValue.ToString();
                    if (value != string.Empty)
                    {
                        BindingSource bs = tmp.Properties.DataSource as BindingSource;
                        //int index = bs.Position;
                        int index = tmp.Properties.GetIndexByKeyValue(value);
                        if (index < 0)
                        {
                            index = bs.Count - 1;
                        }
                        DataTable dt = bs.DataSource as DataTable;
                        if (index < 0) return;
                        DataRow drData;//= dt.Rows[index];
                        DataRowView drDataView = tmp.Properties.GetRowByKeyValue(value) as DataRowView;

                        if (drDataView != null)
                        {
                            drData = drDataView.Row;
                            GridView gv;
                            CDTRepGridLookup CDTri = tmp.Tag as CDTRepGridLookup;
                            DataTable tbStruct;
                            
                            GridView vMain = this._gcMain.Views[0] as GridView;
                            if (CDTri != null)
                            {
                                gv = CDTri.MainView;
                                tbStruct = CDTri.MainStruct;
                            }
                            else
                            {
                                gv = vMain;
                                tbStruct = this._data.DsStruct.Tables[1];
                            }
                            
                            DataRow drDetail = gv.GetDataRow(gv.FocusedRowHandle);
                            if (drDetail == null)//đang forcus tới detail view
                            {
                                gv = vMain.GetVisibleDetailView(vMain.FocusedRowHandle) as GridView;
                                if(gv!=null)
                                    drDetail = gv.GetDataRow(gv.FocusedRowHandle);
                            }
                            if (gv!=null && gv.Columns[tmp.Properties.Name] != null)
                            {
                                if (this._data.dataType == DataType.MasterDetail)
                                {
                                   // this._data.SetValuesFromListDt(drDetail, tmp.Properties.Name, value, drData, tbStruct);
                                }
                            }
                            else
                            {
                                if (_gcDetail != null)
                                {
                                    foreach (GridControl gc in this._gcDetail)
                                    {
                                        gv = gc.Views[0] as GridView;
                                        drDetail = gv.GetDataRow(gv.FocusedRowHandle);
                                        if (gv.Columns[tmp.Properties.Name] != null)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (this._data.dataType == DataType.MasterDetail)
                                {
                                    this._data.SetValuesFromListDetail(drDetail, tmp.Properties.Name, value, drData);
                                }
                            }
                            if (gv!=null && gv.Columns[tmp.Properties.Name] != null)
                            {
                            }
                        }
                    }
                }
            }
        }
        public void setStaticFilter()
        {
            string refFilter;
            foreach (CDTRepGridLookup Ri in _lstRep)
            {
                refFilter = Ri.DymicCondition;
                if (refFilter == null || refFilter == string.Empty || refFilter.Contains("@")) continue;
                Ri.View.OptionsFilter.BeginUpdate();
                try
                {

                    Ri.View.ActiveFilterString = refFilter;
                }
                catch (Exception e) { }
                Ri.ActiveFilter = refFilter;

                Ri.View.ActiveFilterEnabled = true;
                Ri.View.OptionsFilter.EndUpdate();
                Ri.View.RefreshEditor(true);
            }

        }
        public void ClearFilter()
        {
            foreach (CDTGridLookUpEdit Ri in this._glist)
            {
                if (!Ri.isFiltered) continue;
                Ri.Properties.BeginUpdate();
                BindingSource bs = Ri.Properties.DataSource as BindingSource;
                bs.DataSource = Ri.Data.DsData.Tables[0];
                Ri.Properties.DataSource = bs;
                Ri.Properties.EndUpdate();

            }
            foreach (CDTRepGridLookup Ri in this._rlist)
            {
                if (!Ri.isFiltered) continue;
                Ri.BeginUpdate();
                BindingSource bs = Ri.DataSource as BindingSource;
                bs.DataSource = Ri.Data.DsData.Tables[0];
                Ri.DataSource = bs;
                Ri.EndUpdate();

            }

        }
        private void setDynFilter(CDTGridLookUpEdit Gl)
        {
            string filter = string.Empty;
            if (Gl.DymicCondition == null || Gl.DymicCondition == string.Empty) return;
            filter = Gl.DymicCondition;
            if (_data.DrCurrentMaster == null) return;
            List<string> lvar = Config.GetVariableList(filter.ToUpper());
            var variables = new Dictionary<string, string>();
            foreach (DataColumn dcMater in _data.DrCurrentMaster.Table.Columns)
            {
                
                string fieldName = dcMater.ColumnName;
                if(!lvar.Contains("@" + fieldName.ToUpper())) continue;
                //if (!filter.ToUpper().Contains("@" + fieldName.ToUpper())) continue;
                string value = _data.DrCurrentMaster[fieldName].ToString();

                if (value == null || value == string.Empty)
                {
                    if ((dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                        value = "0";
                    else if (dcMater.DataType == typeof(string))
                        value = "''";
                    else if (dcMater.DataType == typeof(Guid))
                        value = "  NULL ";
                }
                else
                {
                    if (!(dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                        value = "'" + value + "'";
                }
                if (dcMater.DataType == typeof(bool))
                {
                    if (value == "'True'") value = "1";
                    else if (value == "'False'") value = "0";
                    else value = " NULL ";
                }
                variables.Add("@" + fieldName.ToUpper(), value);

                //filter = filter.ToUpper().Replace("@" + fieldName.ToUpper(), value);
            }
            var sortedVariables = variables.OrderByDescending(v => v.Key.Length);
            foreach (var variable in sortedVariables)
            {
                string pattern = $@"\{variable.Key}\b"; // Sử dụng \b để đảm bảo khớp với từ nguyên vẹn
                filter = Regex.Replace(filter.ToUpper(), pattern, variable.Value);
            }
            filter = filter.Replace(" = NULL", " IS NULL");
            if (filter != string.Empty)
            {
                if (!Gl.Data.FullData) Gl.Data.GetData();
                DataTable dttmp = Gl.Data.DsData.Tables[0].Clone();

                DataRow[] drtmp;
                try
                {
                    drtmp = Gl.Data.DsData.Tables[0].Select(filter);
                }
                catch
                {
                    drtmp = Gl.Data.DsData.Tables[0].Select("1=0");
                }

                foreach (DataRow dr in drtmp)
                    dttmp.ImportRow(dr);
                BindingSource bs = Gl.Properties.DataSource as BindingSource;
                bs.DataSource = dttmp;
                Gl.isFiltered = true;


            }
        }
        private void setDynFiter(CDTRepGridLookup Ri)
        {
            string refFilter;
            string strReplaced;
            string filter = string.Empty;
            if (Ri.DymicCondition == null || Ri.DymicCondition == string.Empty) return;
            filter = Ri.DymicCondition;
            if (_data.DrCurrentMaster == null) return;
            List<string> fvar = Config.GetVariableList(filter.ToUpper());
            var variables = new Dictionary<string, string>();
            foreach (DataColumn dcMater in _data.DrCurrentMaster.Table.Columns)
            {
                string fieldName = dcMater.ColumnName;
                
                if (!fvar.Contains("@" + fieldName.ToUpper())) continue;
                //if (!filter.ToUpper().Contains("@" + fieldName.ToUpper())) continue;
                string value = _data.DrCurrentMaster[fieldName].ToString();

                if (value == null || value == string.Empty)
                {
                    if ((dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                        value = "0";
                    else if (dcMater.DataType == typeof(string))
                        value = "''";
                    else if (dcMater.DataType == typeof(Guid))
                        value = " NULL ";
                }
                else
                {
                    if (!(dcMater.DataType == typeof(decimal) || dcMater.DataType == typeof(int)))
                        if (!value.StartsWith("'"))
                            value = "'" + value + "'";
                }
                // filter = filter.ToUpper().Replace("@" + fieldName.ToUpper(), value);
                variables.Add("@" + fieldName.ToUpper(), value);
            }
            if (this._gcMain == null) return;
            DataRow drc = (this._gcMain.DefaultView as GridView).GetFocusedDataRow();
            if (drc == null)
            {
                foreach (DevControls.CDTGridColumn dcDetail in (this._gcMain.DefaultView as GridView).Columns)
                {
                    if (!fvar.Contains("@" + dcDetail.FieldName.ToUpper())) continue;
                    if (variables.ContainsKey("@" + dcDetail.FieldName.ToUpper())) continue;
                    if (!(dcDetail.ColumnType == typeof(decimal) || dcDetail.ColumnType == typeof(int)))                             
                    variables.Add("@" + dcDetail.FieldName.ToUpper(), "''");
                    else variables.Add("@" + dcDetail.FieldName.ToUpper(), "0");
                }
            }
            else
            {
                foreach (DataColumn dcDetail in drc.Table.Columns)
                {
                    string fieldName = dcDetail.ColumnName;
                    if (!fvar.Contains("@" + fieldName.ToUpper())) continue;
                    string value = drc[fieldName].ToString();
                    if (value == null || value == string.Empty) value = "";
                    if (value == null || value == string.Empty)
                    {
                        if ((dcDetail.DataType == typeof(decimal) || dcDetail.DataType == typeof(int)))
                            value = "0";
                        else if (dcDetail.DataType == typeof(string))
                            value = "''";
                        else if (dcDetail.DataType == typeof(Guid))
                            value = " NULL ";
                    }
                    else
                    {
                        if (!(dcDetail.DataType == typeof(decimal) || dcDetail.DataType == typeof(int)))
                            if (!value.StartsWith("('"))//Biểu thị cho kiểu list
                                value = "'" + value + "'";
                    }
                    //filter = filter.ToUpper().Replace("@" + fieldName.ToUpper(), value);
                    if (variables.ContainsKey("@" + fieldName.ToUpper()))
                    {
                        if (variables[("@" + fieldName.ToUpper())].ToString() == " NULL " || variables[("@" + fieldName.ToUpper())].ToString() == "''")
                        {
                            variables["@" + fieldName.ToUpper()] = value;
                        }
                        else continue;
                    }
                    else
                        variables.Add("@" + fieldName.ToUpper(), value);
                }
            }

            //Thay thế giá trị vào biểu thức
            var sortedVariables = variables.OrderByDescending(v => v.Key.Length);
            
            foreach (var variable in sortedVariables)
            {
                string pattern = $@"\{variable.Key}\b"; // Sử dụng \b để đảm bảo khớp với từ nguyên vẹn
                filter = Regex.Replace(filter.ToUpper(), pattern, variable.Value);
            }
            filter = filter.Replace("= NULL", " IS NULL");
            if (filter != string.Empty)
            {
                if (!Ri.Data.FullData) Ri.Data.GetData();
                DataTable dttmp = Ri.Data.DsData.Tables[0].Clone();

                DataRow[] drtmp;
                
                try
                {
                    drtmp = Ri.Data.DsData.Tables[0].Select(filter);
                }
                catch(Exception ex)
                {                    
                    drtmp = Ri.Data.DsData.Tables[0].Select("1=0");
                }
                
                foreach (DataRow dr in drtmp)
                    dttmp.ImportRow(dr);
                BindingSource bs = Ri.DataSource as BindingSource;
                bs.DataSource = dttmp;
                //Ri.DataSource = bs;
                Ri.isFiltered = true;


            }
        }

        private void Tab1_GotFocus(object sender, EventArgs e)
        {
            try
            {
                (sender as XtraTabPage).Controls[0].Focus();
            }
            catch
            {
            }
        }

        private void TabDetail_GotFocus(object sender, EventArgs e)
        {
            try
            {
                (sender as XtraTabControl).TabPages[0].Focus();
            }
            catch
            {
            }
        }

        private void TabDetail_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
        {
            //if ((e.PrevPage != null) && ((e.PrevPage.TabIndex == 0) && !this.InsertedToDetail))
            //{
            //    int ix = e.Page.TabIndex;
            //    if (ix > this._gcDetail.Count) return;
            //    if (ix == 0) return;
            //    ix = ix - 1;
            //    DataTable tbTmp = (this.Data as DataMasterDetail).UpdateDetailFromMTDT(ix);

            //    GridControl gr = this._gcDetail[ix];
            //    GridView gv = gr.MainView as GridView;
            //    foreach (DataRow dr in tbTmp.Rows)
            //    {
            //        gv.AddNewRow();
            //        CurrentRowDt drDt = this.Data._lstCurRowDetail[this.Data._lstCurRowDetail.Count - 1];
            //        foreach (DataColumn col in tbTmp.Columns)
            //        {
            //            drDt.RowDetail[col.ColumnName] = dr[col.ColumnName];
            //        }
            //    }

            //    this.InsertedToDetail = true;
            //}
        }
        public void InsertDetailFromMTDT()
        {
            if (this.TabDetail == null) return;
            int i = this.TabDetail.SelectedTabPageIndex;
            if (i > this._gcDetail.Count) return;
            if (i == 0) return;
            i = i - 1;
            DataTable tbTmp = (this.Data as DataMasterDetail).UpdateDetailFromMTDT(i);
            int count = this.Data._lstCurRowDetail.Count;
            while (count>0)
            {
                if (this.Data._lstCurRowDetail[count - 1].TableName == this.Data._drTableDt[i]["TableName"].ToString())
                {
                    this.Data._lstCurRowDetail.RemoveAt(count - 1);                    
                }
                count--;
            }

            //this.Data._lstCurRowDetail.Clear();

            //for (int i = 0; i < this._gcDetail.Count; i++)
            //{
            //if (i >= 1) continue;
            
            GridControl gr = this._gcDetail[i];
            GridView gv = gr.MainView as GridView;
            while (gv.RowCount > 1)
            {
                gv.DeleteRow(0);
            }
            DataRow drtableDt = this.Data._drTableDt[i];
            if (tbTmp == null) return;

            

            foreach (DataRow dr in tbTmp.Rows)
            {
                gv.AddNewRow();
                CurrentRowDt drDt = this.Data._lstCurRowDetail[this.Data._lstCurRowDetail.Count - 1];
                foreach (DataColumn col in tbTmp.Columns)
                {
                     drDt.RowDetail[col.ColumnName] = dr[col.ColumnName];

                }
                if (drDt.RowDetail.RowState == DataRowState.Detached)
                    drDt.RowDetail.Table.Rows.Add(drDt.RowDetail);
                // drDt.RowDetail.SetAdded();
                //drDt.RowDetail.EndEdit();
            }
            // }
            this.InsertedToDetail = true;
        }
        private void tmp_RIButton_click(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                CDTRepGridLookup tmp = sender as CDTRepGridLookup;
                CDTData d = e.Button.Tag as CDTData;

                if (e.Button.ToolTip == "F5 - Refresh")
                {
                    int n = -1;
                    int i = 0;
                    for (i = 0; i < this._rlist.Count; i++)
                    {
                        if (this._Rlist[i].rglk == tmp)
                        {
                            n = this._Rlist[i].dataIndex;
                            break;
                        }
                        }
                    this.RefreshLookupAllowFull(tmp.Data);
                    return;
                }
                // if (!d.FullData)
                // {
                //int n = -1;
                //int i = 0;
                //for (i = 0; i < this._Rlist.Count; i++)
                //{
                //    if (this._Rlist[i].rglk == tmp)
                //    {
                //        n = this._Rlist[i].dataIndex;
                //        break;
                //    }
                //}
                this.RefreshLookup(tmp.Data);
                // }
                if (d != null)
                {
                    BindingSource bs = tmp.DataSource as BindingSource;
                    FrmSingleDt frm = new FrmSingleDt(d, bs);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string tableName;
                        if (tmp.refTable != null)
                        {
                            tableName = tmp.refTable;
                        }
                        else
                        {
                            tableName = tmp.refTable;
                        }
                        foreach (RLookUp_CDTData rgc in this._Rlist)
                        {
                            CDTRepGridLookup rg = rgc.rglk;
                            if ((rg.refTable.ToUpper() == tableName.ToUpper()) && (rg.View.ViewCaption.ToUpper() == d.Condition.ToUpper()))
                            {
                                rg.DataSource = null;
                                rg.DataSource = bs;
                            }
                        }
                        foreach (LookUp_CDTData rgc in this._Glist)
                        {
                            CDTGridLookUpEdit rg = rgc.glk;
                            if ((rg.refTable.ToUpper() == tableName.ToUpper()) && (rg.Condition == d.Condition))
                            {
                                rg.Properties.DataSource = null;
                                rg.Properties.DataSource = bs;
                            }
                        }

                        if (_gcMain == null) return;
                        object t = (bs.List[bs.Count - 1] as DataRowView)[tmp.ValueMember];
                        (this._gcMain.MainView as GridView).SetFocusedRowCellValue((this._gcMain.MainView as GridView).FocusedColumn, t);
                        (this._gcMain.MainView as GridView).UpdateCurrentRow();
                        tmp.GridLookup.EditValue = t;
                        this.RIGridLookupEdit_EditValueChanged(tmp.GridLookup, new EventArgs());
                    }
                    else
                    {
                        tmp.DataSource = null;
                        tmp.DataSource = bs;
                    }
                }
            }
        }

        private void VCalEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                (sender as CalcEdit).ClosePopup();
            }
        }

        private void View_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && (e.RowHandle >= 0))
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            GridView tmp = sender as GridView;
            if (tmp.OptionsView.ShowAutoFilterRow && (e.KeyCode == Keys.F5))
            {
                tmp.FocusedRowHandle = -999997;
                tmp.FocusedColumn = tmp.VisibleColumns[1];
                tmp.ShowEditor();
            }
        }

        // Properties
        public BindingSource bindingSource
        {
            get
            {
                return this._bindingSource;
            }
            set
            {
                this._bindingSource = value;
            }
        }

        public CDTData Data
        {
            get
            {
                return this._data;
            }
        }

        public BaseEdit FirstControl
        {
            get
            {
                return this._firstControl;
            }
            set
            {
                this._firstControl = value;
            }
        }

        public FormAction formAction
        {
            get
            {
                return this._formAction;
            }
            set
            {
                this._formAction = value;
            }
        }

        public List<CDTRepGridLookup> rlist
        {
            get
            {
                return this._rlist;
            }
            set
            {
                this._rlist = value;
                this.RefreshDataForLookup();
            }
        }

        // Nested Types

        private struct LookUp_CDTData
        {
            public CDTGridLookUpEdit glk;
            public int dataIndex;
            public LookUp_CDTData(CDTGridLookUpEdit g, int i)
            {
                this.glk = g;
                this.dataIndex = i;
            }
        }

        private struct RLookUp_CDTData
        {
            public CDTRepGridLookup rglk;
            public int dataIndex;
            public RLookUp_CDTData(CDTRepGridLookup r, int i)
            {
                this.rglk = r;
                this.dataIndex = i;
            }
        }
    }


}

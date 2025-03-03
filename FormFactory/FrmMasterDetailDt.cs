using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DataFactory;
using CDTLib;
using CDTControl;
using Plugins;
using CusCDTData;
using System.IO;
using System.Runtime.Remoting;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevControls;
using DevExpress.XtraGrid;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using ErrorManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Linq;
using DevExpress.XtraBars;
namespace FormFactory
{
    public partial class FrmMasterDetailDt : CDTForm
    {
        private bool _x;
        public bool refresh = false;
        LayoutControl lcMain;

        public class TransparentPanel : Panel
        {
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                    return cp;
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
            }
        }
        public FrmMasterDetailDt(CDTData data)
        {
            InitializeComponent();
           
            this._data = data;
            InitActionCommand();
            _data.SetDetailValue += new EventHandler(_data_SetDetailValue);
            this._frmDesigner = new FormDesigner(this._data);
            _frmDesigner.formAction = FormAction.New;
            _bindingSource.DataSource = this._data.DsData;
            _bindingSource.CurrentChanged += new EventHandler(_bindingSource_CurrentChanged);
           // this._data.DrCurrentMaster.Table.ColumnChanged += Table_ColumnChanged;
            _frmDesigner.bindingSource = _bindingSource;
            //dataNavigatorMain.DataSource = _frmDesigner.bindingSource;
            bindingNavigator1.BindingSource =_bindingSource;
            bindingNavigator1.ItemClicked += new ToolStripItemClickedEventHandler(bindingNavigator1_ItemClicked);
            bindingNavigator1.MouseUp += BindingNavigator1_MouseUp;
            this._data.AddHis += _data_AddHis;
            dxErrorProviderMain.DataSource = _frmDesigner.bindingSource;
            _bindingSource.AddNew();

            InitializeLayout();
            this.Load += new EventHandler(FrmMasterDetailCt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            AddICDTData(_data);
            //else
            //this.dataNavigatorMain.TextStringFormat = "Mục {0} của {1}";
            //dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
            if (_data._dtDtReport != null)
            {
                glReport.Properties.DataSource = _data._dtDtReport;
                glReport.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(glReport_ButtonClick);
            }
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            
            _data.DataChangedEvent += _data_DataChangedEvent;
        }

        private void _data_AddHis()
        {
            this.pHis.BeginUpdate();
            this.pHis.LinksPersistInfo.Clear();
            this.pHis.ClearLinks();

            foreach (historyCurrent hist in this._data.HistoryCurrents)
            {
                //  this.pMInsetToDetail.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
                // new DevExpress.XtraBars.LinkPersistInfo(this.btSaveGrid),
                BarButtonItem bar=  new DevExpress.XtraBars.BarButtonItem(barManager1, hist.datetime.ToString("dd/MM/yyyy HH:mm:ss") + "- " + hist.tip);
                bar.Tag = hist.jsonData;
                DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();

                // Tạo tiêu đề
                DevExpress.Utils.ToolTipTitleItem titleItem = new DevExpress.Utils.ToolTipTitleItem();
                titleItem.Text = hist.tip;

                // Tạo nội dung
                DevExpress.Utils.ToolTipItem contentItem = new DevExpress.Utils.ToolTipItem();
                contentItem.Text = hist.jsonData;

                // Thêm các mục vào SuperToolTip
                contentItem.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
                superToolTip.Items.Add(titleItem);
                superToolTip.Items.Add(contentItem);
                bar.SuperTip = superToolTip;
                barManager1.Items.Add(bar);
                bar.ItemClick += Bar_ItemClick;
                DevExpress.XtraBars.LinkPersistInfo hisbut = new DevExpress.XtraBars.LinkPersistInfo(bar);
                this.pHis.LinksPersistInfo.Add(hisbut);
                this.pHis.ItemLinks.Add(bar);
                
            }
            this.pHis.EndUpdate();
        }

        private void Bar_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            if (e.Item.Tag!= null)
            {
                string his = e.Item.Tag.ToString();
                ImportDataFromText(his);
            }
        }

        private void _data_DataChangedEvent()
        {
            if (_data.DataChanged)
            {
                this.tsNew.Enabled = false;
                this.tsSave.Enabled = true;
                this.tsSaveNew.Enabled = false;
                this.tsPrint.Enabled = false;
                this.tsComment.Enabled = false;
            }
            else
            {
                this.tsNew.Enabled = true;
                this.tsSave.Enabled = false;
                this.tsSaveNew.Enabled = true;
                string sPrint = _data.DrTableMaster["sPrint"].ToString();
                bool tPrint = false;
                if (sPrint == "0") tPrint = false;
                else if (sPrint == "1" || sPrint == string.Empty) tPrint = true;
                if (sPrint != string.Empty && !tPrint)
                    this.tsPrint.Enabled = false;
                else
                    this.tsPrint.Enabled = true;
                this.tsComment.Enabled = true;

            }
            foreach (ToolStripItem it in this.bindingNavigator1.Items)
            {
                DataRow drAction = it.Tag as DataRow;
                if (it.Tag == null) continue;
                it.Enabled = !_data.DataChanged;
            }
        }
        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //set visible, editable Cho các field Main
            
            foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
            {
                List<string> lFieldEdit = Config.GetFieldList(dr["Editable"].ToString().ToUpper());
                List<string> lFieldVisible = Config.GetFieldList(dr["Visible"].ToString().ToUpper());

                if (dr["Editable"] == DBNull.Value || dr["Editable"].ToString() == "0" || dr["Editable"].ToString() == "1") { }
                else if (lFieldEdit.Contains(e.Column.ColumnName.ToUpper()))
                    setEdit(dr["Editable"].ToString(), dr["FieldName"].ToString());
                if (dr["Visible"] == DBNull.Value || dr["Visible"].ToString() == "0" || dr["Visible"].ToString() == "1") { }
                else if (lFieldVisible.Contains(e.Column.ColumnName.ToUpper()))
                    setVisible(dr["Visible"].ToString(), dr["FieldName"].ToString());
            }
            //Set visible, editable cho các Field Detail
            foreach (DataRow dr in _data.DsStruct.Tables[1].Rows)
            {
                List<string> lFieldEdit = Config.GetFieldList(dr["Editable"].ToString().ToUpper());
                List<string> lFieldVisible = Config.GetFieldList(dr["Visible"].ToString().ToUpper());
                if (dr["Editable"] == DBNull.Value || dr["Editable"].ToString() == "0" || dr["Editable"].ToString() == "1")
                { 
                }
                else if (lFieldEdit.Contains(e.Column.ColumnName.ToUpper()))
                    setEditCol(gvMain, dr["Editable"].ToString(), dr["FieldName"].ToString());
                if (dr["Visible"] == DBNull.Value || dr["Visible"].ToString() == "0" || dr["Visible"].ToString() == "1") { }
                else if (lFieldVisible.Contains(e.Column.ColumnName.ToUpper()))
                    setVisibleCol(gvMain, dr["Visible"].ToString(), dr["FieldName"].ToString());
            }
           // _frmDesigner._gcDetail[0].Enabled = true;
            //Set visible cho các chi tiết
            for (int i = 0; i < _data._drTableDt.Count; i++)
            {
                DataRow drTable = _data._drTableDt[i];
                DataRow drDt = _data._dtDetail.Rows[i];

                if (drDt["ShowCondition"] != DBNull.Value && drDt["ShowCondition"].ToString().Contains(e.Column.ColumnName))
                    foreach (GridControl gr in _frmDesigner._gcDetail)
                    {
                        if (drTable["TableName"].ToString() == gr.Name) setVisibleGridDetail(drDt["ShowCondition"].ToString(), gr);
                    }

            }
            foreach(GridControl gc in _frmDesigner._gcDetail)
            {
                GridView gv = gc.DefaultView as GridView;
                foreach (DataRow dr in _data._dsStructDt.Tables[gv.Name].Rows)
                {
                    if (dr["Editable"] == DBNull.Value || dr["Editable"].ToString() == "0" || dr["Editable"].ToString() == "1")
                    { }
                    else if (dr["Editable"].ToString().Contains(e.Column.ColumnName))
                        setEditCol(gv,dr["Editable"].ToString(), dr["FieldName"].ToString());
                    if (dr["Visible"] == DBNull.Value || dr["Visible"].ToString() == "0" || dr["Visible"].ToString() == "1") { }
                    else if (dr["Visible"].ToString().Contains(e.Column.ColumnName))
                        setVisibleCol(gv, dr["Visible"].ToString(), dr["FieldName"].ToString());
                }
            }
        }
        private void setVisibleGridDetail( string condition, GridControl gr)
        {
            if (_data.DrCurrentMaster == null) return;
            _data.DrCurrentMaster.Table.EndInit();
            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            tableTmp.Rows.Add(drtmp);
            drtmp.EndEdit();
            tableTmp.AcceptChanges();
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            //foreach(GridControl grdt in _frmDesigner._gcDetail)
            //{
            //}
            //if (lstDr.Length > 0)//Hiện
            //{
            //    if (!gcMain.LevelTree.Nodes.Contains(gr.Name + "1"))
            //    {
            //        GridLevelNode gridLevelNode1 = new GridLevelNode();
            //        gridLevelNode1.LevelTemplate = gr.DefaultView;
            //        gridLevelNode1.RelationName = gr.Name + "1";
            //        gcMain.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            //    }
            //}
            //else
            //{
            //    foreach (GridLevelNode node in gcMain.LevelTree.Nodes)
            //    {
            //        if (node.RelationName == gr.Name + "1")
            //        {
            //            gcMain.LevelTree.Nodes.Remove(node);
            //            break;
            //        }
            //    }
            //}
            
         
        }
        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);

        }
        private void InitActionCommand()
        {
            
            if(this._data.tbAction==null ||this._data.tbAction.Rows.Count==0 ) return;
            foreach (DataRow dr in this._data.tbAction.Rows)
            {
                //if (bool.Parse(dr["AutoDo"].ToString())) continue;
                 ToolStripButton toolStripButton=new ToolStripButton(dr["CommandName"].ToString());
                 if (dr["Icon"] != DBNull.Value)
                 {
                     Image im = GetImage(dr["Icon"] as byte[]);
                     if (im != null) toolStripButton.Image = im;                         
                 }
                 toolStripButton.Name = dr["CommandName"].ToString();
                 toolStripButton.Tag = dr;
                 toolStripButton.Click += new EventHandler(toolStripButton_Click);
                 this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButton });
            }
        }

        void toolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = sender as ToolStripButton;
            if (toolStripButton == null) return;
            if (toolStripButton.Tag == null) return;
            if (_data.DataChanged )
            {
                UpdateData();
                RemoveTabReport();
            }
            DataRow dr = toolStripButton.Tag as DataRow;
            if (!_data.DataChanged)
            {

                string Confirm = "";
                if (Config.GetValue("Language").ToString() == "1")
                    Confirm = "Are you sure " + toolStripButton.Text +  "?";
                else
                    Confirm = "Bạn có chắc thực hiện " + toolStripButton.Text + "?";
                if (dr.Table.Columns.Contains("Confirm") && dr["Confirm"] != DBNull.Value && dr["Confirm"].ToString() != string.Empty)
                    Confirm = dr["Confirm"].ToString();
                if (XtraMessageBox.Show(Confirm, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if ((this._data as DataMasterDetail).doAction(dr))
                    {
                        //MessageBox.Show("Hoàn thành!");
                        if (dr["isRefresh"].ToString().ToLower() == "true")
                        {
                            this.refresh = true;
                            _x = false;
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        if (dr.Table.Columns.Contains("Message") && dr["Message"] != DBNull.Value && dr["Message"].ToString() != string.Empty)
                            MessageBox.Show(dr["Message"].ToString());
                    }

                }
            }
        }

        void glReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (glReport.EditValue == null) return;
            //xem báo cáo
         int index=   glReport.Properties.GetIndexByKeyValue(glReport.EditValue);
         DataRow drTable = _data._dtDtReport.Rows[index];

            CDTData datareport=new DataReport(drTable);
            ReportFilter tmp = new ReportFilter(datareport);
           
            tmp.ShowDialog();
            if (tmp.reportPreview != null)
            {
                XtraTabPage Tab1 = new XtraTabPage();
                Tab1.Text = "BC_" + drTable["ReportName"].ToString();
                Tab1.Controls.Add(tmp.reportPreview);
                tmp.reportPreview.Dock = DockStyle.Fill;
                _frmDesigner.TabDetail.TabPages.Add(Tab1);

            }
            
        }


        void _data_SetDetailValue(object sender, EventArgs e)
        {
            List<object> ob = sender as List<object>;

            string FieldName = ob[0].ToString();
            string value = ob[1].ToString();
            GridColumn col;
            if (int.Parse(ob[2].ToString()) == 0)
            {
                col = gvMain.Columns[FieldName];
            }
            else
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)_frmDesigner._gcDetail[int.Parse(ob[2].ToString()) - 1].MainView;
                col = gv.Columns[FieldName];
            }

            CDTRepGridLookup tmp = col.ColumnEdit as CDTRepGridLookup;
            if (tmp == null) return;
            //GetFull data for col
            if (!tmp.Data.FullData)
            {
                tmp.Data.GetData();
                _frmDesigner.RefreshLookup(tmp.Data);

            }
            this._frmDesigner.setStaticFilter();
        }
        
        

        


        public FrmMasterDetailDt(FormDesigner frmDesigner)
        {
            InitializeComponent();
            this._data = frmDesigner.Data;
            InitActionCommand();
            _data.SetDetailValue += new EventHandler(_data_SetDetailValue);
            //frmDesigner.setStaticFilter();
            this._frmDesigner = frmDesigner;
            this._bindingSource = frmDesigner.bindingSource;
            _bindingSource.CurrentChanged += new EventHandler(_bindingSource_CurrentChanged);
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            
            //dataNavigatorMain.DataSource = this._bindingSource;
            dxErrorProviderMain.DataSource = this._bindingSource;
            bindingNavigator1.BindingSource = _bindingSource;
            bindingNavigator1.ItemClicked += new ToolStripItemClickedEventHandler(bindingNavigator1_ItemClicked);
            bindingNavigator1.MouseUp += BindingNavigator1_MouseUp;

            this._data.AddHis += _data_AddHis;
            InitializeLayout();
            this.Load += new EventHandler(FrmMasterDetailCt_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            //dataNavigatorMain.PositionChanged += new EventHandler(dataNavigatorMain_PositionChanged);
            AddICDTData(_data);
            this.WindowState = FormWindowState.Maximized;
            if (_data._dtDtReport != null)
            {
                glReport.Properties.DataSource = _data._dtDtReport;
                glReport.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(glReport_ButtonClick);
            }
            this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
            foreach (ICDTData pl in _lstICDTData)
                pl.AddEvent();
            this._data.DataChangedEvent += _data_DataChangedEvent;
        }

        private void BindingNavigator1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                pHis.ShowPopup(new Point(e.X, e.Y + (sender as BindingNavigator).Top));
            }
        }

        void _bindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (_frmDesigner.formAction != FormAction.Delete && _frmDesigner.formAction != FormAction.View && _frmDesigner.formAction != FormAction.Copy)
            {
                SetCurrentData();
                _frmDesigner.RefreshViewForLookup();
                _frmDesigner.RefreshFormulaDetail();
            }
        }
        void bindingNavigator1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "tsNew":
                    if (_data.DataChanged)
                    {
                        string Mess = "Dữ liệu chưa được lưu, bạn có muốn lưu không?";
                        if (Config.GetValue("Language").ToString() == "1")
                            Mess = "Data not saved. Do you want to save";
                        if (MessageBox.Show(Mess, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Config.NewKeyValue("Operation", "F12-Lưu");
                            UpdateData();
                            RemoveTabReport();

                        }
                        else
                        {
                            CancelUpdate();
                            break;
                        }
                    }
                    _frmDesigner.formAction = FormAction.New;

                    _bindingSource.AddNew();
                    _bindingSource.EndEdit();
                    _frmDesigner.RefreshGridLookupEdit();
                    _x = true;
                    SetCurrentData();
                    _frmDesigner.RefreshFormulaDetail();
                    _frmDesigner.InsertedToDetail = false;
                    _frmDesigner.TabDetail.SelectedTabPage = _frmDesigner.TabDetail.TabPages[0];                         
                    break;
                case "tsSave":
                    Config.NewKeyValue("Operation", "F12-Lưu");                  
                    UpdateData();
                    RemoveTabReport();
                    break;
                case "tsPrint":
                  //  Config.NewKeyValue("Operation", 'F7-Print");

                    if (_data.DrTable["Report"].ToString() == string.Empty)
                        return;
                    else
                    {                        
                        int[] newIndex ={ _bindingSource.Position };
                        BeforePrint bp = new BeforePrint(_data, newIndex);
                        bp.ShowDialog();
                    }
                    break;
                case "tsSaveNew":
                    Config.NewKeyValue("Operation", "F12-Lưu");
                    UpdateData();
                    RemoveTabReport();
                    if (!_x)
                    {
                        //if (_data.DrTable["Report"].ToString() == string.Empty)
                        //    return;
                        //else
                        //{
                        //   // int[] newIndex ={ _bindingSource.Position };
                        //   // BeforePrint bp = new BeforePrint(_data, newIndex);
                        //  //  bp.ShowDialog();
                        //}
                        Config.NewKeyValue("Operation", "New");
                        _frmDesigner.formAction = FormAction.New;
                        _bindingSource.AddNew();
                        _bindingSource.EndEdit();
                        _frmDesigner.RefreshGridLookupEdit();
                        _x = true;
                        SetCurrentData();
                        _frmDesigner.RefreshFormulaDetail();
                        _frmDesigner.InsertedToDetail = false;
                        _frmDesigner.TabDetail.SelectedTabPage = _frmDesigner.TabDetail.TabPages[0];
                    }
                    break;
                case "tsComment":
                    if (_bindingSource.Position < 0) return;
                    fComment fC = new fComment(_data, _bindingSource.Position);
                    fC.ShowDialog();
                    break;
            }
        }


        void dataNavigatorMain_PositionChanged(object sender, EventArgs e)
        {
            if (_frmDesigner.formAction != FormAction.Delete && _frmDesigner.formAction != FormAction.View && _frmDesigner.formAction != FormAction.Copy)
                SetCurrentData();
        }
        public void SetCurrentData()
        {
            DataRowView drv = (_bindingSource.Current as DataRowView);
            if (drv == null) return;
            this._data.DrCurrentMaster = drv.Row;
            DataRow[] drDetails ;
            if (_data.DrCurrentMaster.RowState == DataRowState.Detached && _frmDesigner.formAction == FormAction.Delete) return;
            if (_data.DrCurrentMaster[_data.PkMaster.FieldName] == DBNull.Value) return;
            string ConditionSelect = _data.PkMaster.FieldName + " = " + _data.quote + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + _data.quote;
            drDetails = _data.DsData.Tables[_data.DrTable["TableName"].ToString()].Select(ConditionSelect);
            //for (int i = 0; i < gvMain.DataRowCount; i++)
            //    drDetails.Add(gvMain.GetDataRow(i));
            this._data.LstDrCurrentDetails.Clear();
            this._data.LstDrCurrentDetails.AddRange(drDetails);
            this._data._formulaCaculator.LstDrCurrentDetails = this._data.LstDrCurrentDetails;
            //this._data._formulaCaculator.DataTable1_Rowdeleted(this._data.DsData.Tables[1], new DataRowChangeEventArgs(null, DataRowAction.Nothing));
            _data._lstCurRowDetail.Clear();
            for (int j = 0; j < this._data._drTableDt.Count; j++)
            {
                DataRow _drTableDt = this._data._drTableDt[j];
                DataRow _drDetail = this._data._dtDetail.Rows[j];
                DataRow[] _lstRowDt;
                _lstRowDt = this._data.DsData.Tables[_drTableDt["TableName"].ToString()].Select("MTID=" + _data.quote + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + _data.quote);
                foreach (DataRow crRDt in _lstRowDt) {
                    CurrentRowDt crdt = new CurrentRowDt();
                    crdt.TableName = _drTableDt["TableName"].ToString();
                    crdt.RowDetail = crRDt;
                    if (bool.Parse(_drDetail["ChildOf"].ToString()))
                        crdt.fkKey = crRDt["MTID"];
                    else
                        crdt.fkKey = crRDt["DTID"];
                    _data._lstCurRowDetail.Add(crdt);
                }
            }
            _data._formulaCaculator.LstCurrentRowDt= _data._lstCurRowDetail;
            DataView dvtmp = drv.Row.Table.DefaultView;
           //Lấy dữ liệu file
            this._data.Get_fileData4Record();
            for (int i = 0; i < this._data._fileData.Count; i++)
            {
                FileData fData = this._data._fileData[i];
                foreach (fileContener fC in this._frmDesigner._lFileContener)
                {
                    if (fC.drField["sysFieldID"].ToString() == fData.drField["sysFieldID"].ToString())
                    {
                        fC.data = fData.fData;
                    }
                }

                foreach (ImageContener IC in this._frmDesigner._lImageContener)
                {
                    if (IC.drField["sysFieldID"].ToString() == fData.drField["sysFieldID"].ToString())
                    {
                        IC.data = fData.fData;
                        IC.EditValue = fData.fData;
                        drv[IC.drField["FieldName"].ToString()] = IC.data;
                        drv.EndEdit();
                    }
                }
            }
            //Set Allow edit
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
                {
                    if (this._frmDesigner.formAction == FormAction.View)
                    {
                        setEdit("1=0", dr["FieldName"].ToString());
                        continue;
                    }
                    if (dr["Editable1"] != DBNull.Value) continue;

                    if (dr["Editable"] == DBNull.Value) { setEdit("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Editable"].ToString() == "0") { setEdit("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Editable"].ToString() == "1") { setEdit("1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setEdit(dr["Editable"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }

                    //Set Visible
                    // if (dr["Visible1"] != DBNull.Value) continue;
                    if (dr["Visible"] == DBNull.Value) { setVisible("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "0") { setVisible("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "1") { setVisible("1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setVisible(dr["Visible"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }
                }
                foreach (DataRow dr in _data.DsStruct.Tables[1].Rows)
                {
                    if (this._frmDesigner.formAction == FormAction.View)
                    {
                        setEditCol(gvMain, "1=0", dr["FieldName"].ToString());
                        continue;
                    }
                    if (dr["Editable1"] != DBNull.Value) continue;
                    if (dr["Editable"] == DBNull.Value) { setEditCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Editable"].ToString() == "0") { setEditCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Editable"].ToString() == "1") { setEditCol(gvMain, "1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setEditCol(gvMain, dr["Editable"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }
                    //Set Visible

                    if (dr["Visible"] == DBNull.Value) { setVisibleCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "0") { setVisibleCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "1") { setVisibleCol(gvMain, "1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setVisibleCol(gvMain, dr["Visible"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }
                }
                foreach (GridControl gc in _frmDesigner._gcDetail)
                {
                    GridView gv = gc.DefaultView as GridView;
                    foreach (DataRow dr in _data._dsStructDt.Tables[gv.Name].Rows)
                    {
                        if (this._frmDesigner.formAction == FormAction.View)
                        {
                            setEditCol(gvMain, "1=0", dr["FieldName"].ToString());
                            continue;
                        }
                        if (dr["Editable"] == DBNull.Value) { setEditCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Editable"].ToString() == "0") { setEditCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Editable"].ToString() == "1") { setEditCol(gv, "1=1", dr["FieldName"].ToString()); }
                        else try
                            {
                                setEditCol(gv, dr["Editable"].ToString(), dr["FieldName"].ToString());
                            }
                            catch { }
                        if (dr["Visible"] == DBNull.Value) { setVisibleCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Visible"].ToString() == "0") { setVisibleCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Visible"].ToString() == "1") { setVisibleCol(gv, "1=1", dr["FieldName"].ToString()); }
                        else try
                            {
                                setVisibleCol(gv, dr["Visible"].ToString(), dr["FieldName"].ToString());
                            }
                            catch { }
                    }
                }
            }
            else
            {
                foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
                {
                    if (this._frmDesigner.formAction != FormAction.View)
                    {
                        setEdit("1=1", dr["FieldName"].ToString());
                        continue;
                    }
                    else
                    {
                        setEdit("1=0", dr["FieldName"].ToString());
                        continue;
                    }
                }
                foreach (DataRow dr in _data.DsStruct.Tables[1].Rows)
                {
                    if (this._frmDesigner.formAction == FormAction.View)
                    {
                        setEditCol(gvMain, "1=0", dr["FieldName"].ToString());
                        continue;
                    }
                    else
                    {
                        setEditCol(gvMain, "1=1", dr["FieldName"].ToString());
                        continue;
                    }
                }
                foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
                {

                    if (dr["Visible"] == DBNull.Value) { setVisible("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "0") { setVisible("1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "1") { setVisible("1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setVisible(dr["Visible"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }
                }
                foreach (DataRow dr in _data.DsStruct.Tables[1].Rows)
                {
                    if (dr["Visible"] == DBNull.Value) { setVisibleCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "0") { setVisibleCol(gvMain, "1=0", dr["FieldName"].ToString()); }
                    else if (dr["Visible"].ToString() == "1") { setVisibleCol(gvMain, "1=1", dr["FieldName"].ToString()); }
                    else try
                        {
                            setVisibleCol(gvMain, dr["Visible"].ToString(), dr["FieldName"].ToString());
                        }
                        catch { }
                }
                foreach (GridControl gc in _frmDesigner._gcDetail)
                {
                    GridView gv = gc.DefaultView as GridView;
                    foreach (DataRow dr in _data._dsStructDt.Tables[gv.Name].Rows)
                    {
                        if (dr["Visible"] == DBNull.Value) { setVisibleCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Visible"].ToString() == "0") { setVisibleCol(gv, "1=0", dr["FieldName"].ToString()); }
                        else if (dr["Visible"].ToString() == "1") { setVisibleCol(gv, "1=1", dr["FieldName"].ToString()); }
                        else try
                            {
                                setVisibleCol(gv, dr["Visible"].ToString(), dr["FieldName"].ToString());
                            }
                            catch { }
                    }
                }
            }
            //Hiện thị các tag

            //Hiển thị các Action
            //foreach (DataRow dr in this._data.tbAction.Rows)
            //{
            foreach (ToolStripItem it in this.bindingNavigator1.Items)
            {
                //if (it.Name == dr["CommandName"].ToString())
                //{
                //it.Visible = true;
                if (this._frmDesigner.formAction == FormAction.View)
                {
                    it.Visible = false;
                    continue;
                }
                else
                {
                    it.Visible = true;
                }
                DataRow drAction = it.Tag as DataRow;
                if (it.Tag == null) continue;
                if (drAction["BTID"].ToString() != _data.DrCurrentMaster["TaskID"].ToString())
                {
                    it.Visible = false;
                    continue;
                }
                string condition = drAction["ShowCond"].ToString();
                if (condition == string.Empty) condition = "(1=1)";
                string con="";
                if (this._data.tbUAction == null) break;
                DataRow[] drActionSecu = this._data.tbUAction.Select("ActionID='" + drAction["ID"].ToString() + "'");
                if (drActionSecu.Length == 0) condition += " and (1=0)";
                else if (drActionSecu[0]["CAllow"] == DBNull.Value || drActionSecu[0]["CAllow"].ToString() == string.Empty)
                    condition += " and (1=1)";
                else
                    condition += " and (" + drActionSecu[0]["CAllow"].ToString() + ")";


                if (condition != string.Empty)
                {
                    condition = _data.UpdateSpecialCondition(condition);
                    con = "(" + condition + ") and (" + this._data.PkMaster.FieldName + "=" + _data.quote + this._data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + _data.quote + ")";
                }
                else
                {
                    con = "(" + this._data.PkMaster.FieldName + "=" + _data.quote + this._data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + _data.quote + ")";
                }
                if (this._data.DrCurrentMaster.RowState == DataRowState.Detached || this._data.DrCurrentMaster.RowState == DataRowState.Added) it.Visible = false;
                else
                {
                    //MessageBox.Show(con);
                    DataRow[] lstDr = _data.DrCurrentMaster.Table.Select(con);
                    if (lstDr.Length == 0)
                    {
                        it.Visible = false;
                    }
                    else
                    {
                        it.Visible = true;
                    }
                }
                //}
            }
            // }
            //Hiển thị nút in
            if (drv.Row.Table.Columns.Contains("TaskID") && drv.Row["TaskID"] != DBNull.Value)
            {
                DataRow[] drTaskSecu = this._data.tbUTask.Select("TaskID='" + drv.Row["TaskID"].ToString() + "'");
                if (drTaskSecu.Length > 0)
                {


                    string cprint = drTaskSecu[0]["CPrint"].ToString();
                    if (cprint == string.Empty) cprint = "1=1";
                    string pcon = _data.PkMaster.FieldName + "='" + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + "' and (" + cprint + ")";
                    DataRow[] sPrint = _data.DrCurrentMaster.Table.Select(pcon);
                    if (sPrint.Length > 0)
                        tsPrint.Visible = true;
                    else
                        tsPrint.Visible = false;
                }
            }

        }
        void setVisible(string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;

            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            drtmp.EndEdit(); tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            BaseEdit it = null;


            foreach (BaseLayoutItem l in lcMain.Items)
            {
                LayoutControlItem li = l as LayoutControlItem;
                if (li == null || li.Control==null) continue;
                if (li.Control.Name == fieldName || li.Control.Name == fieldName + "001")
                {
                    if (lstDr.Length > 0)
                    {
                        li.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        li.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;

                    }
                }
            }

        }
        void setEdit(string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;

            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            drtmp.EndEdit();
            //tableTmp.Rows.Add(drtmp);
            tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            BaseEdit it = null;
            foreach (BaseEdit be in _frmDesigner._BaseList)
            {
                if (be.Name == fieldName || be.Name==fieldName+"001")
                {

                    if (lstDr.Length == 0)
                    {
                        be.Properties.ReadOnly = true;
                    }
                    else
                    {
                        be.Properties.ReadOnly = false;
                    }
                }
            }


        }
        void setVisibleCol(GridView gv,string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;
            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray; 

            drtmp.EndEdit();
            tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            GridColumn gcl = null;
            foreach (GridColumn be in gv.Columns)
            {
                if (be.FieldName == fieldName)
                {
                    gcl = be;
                    CDTGridColumn CDTgcl = (gcl as CDTGridColumn);
                    if (lstDr.Length == 0)
                    {
                        CDTgcl.Visible = false;
                    }
                    else
                    {
                        CDTgcl.Visible = true;
                        CDTgcl.VisibleIndex = CDTgcl.IndexVisible;
                    }
                }
            }

        }
        void setEditCol(GridView gv, string condition, string fieldName)
        {
            if (_data.DrCurrentMaster == null) return;
            DataTable tableTmp = _data.DrCurrentMaster.Table.Clone();
            DataRow drtmp = tableTmp.NewRow();
            drtmp.ItemArray = _data.DrCurrentMaster.ItemArray;
            drtmp.EndEdit(); tableTmp.Rows.Add(drtmp);
            tableTmp.AcceptChanges();
            DataRow[] lstDr = tableTmp.Select("(" + condition + ") and " + _data.PkMaster.FieldName + "=" + _data.quote + drtmp[_data.PkMaster.FieldName].ToString() + _data.quote);
            GridColumn gcl = null;
            foreach (GridColumn be in gv.Columns)
            {
                if (be.FieldName == fieldName)
                {
                    gcl = be;
                    if (lstDr.Length == 0)
                    {
                        gcl.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        gcl.OptionsColumn.AllowEdit = true;
                    }
                }
            }
        }
        void FrmMasterDetailCt_Load(object sender, EventArgs e)
        {
            foreach (ICDTData pl in _lstICDTData)
                pl.AddEvent();
            _x = true;
            SetCurrentData();
            if (_frmDesigner.formAction == FormAction.Copy)
            {
                _data.CloneData();
                int ix = _data.DsData.Tables[0].Rows.IndexOf(_data.DrCurrentMaster);

                _bindingSource.Position = ix;
            }
            
            _frmDesigner.RefreshViewForLookup();
            _frmDesigner.RefreshFormulaDetail();

            if (_frmDesigner.formAction == FormAction.New)
            {
                _frmDesigner.InsertedToDetail = false;
                _frmDesigner.TabDetail.SelectedTabPage = _frmDesigner.TabDetail.TabPages[0];
            }
            else
            {
                _frmDesigner.InsertedToDetail = true;
            }
            _frmDesigner.TabDetail.MouseUp += new MouseEventHandler(TabDetail_MouseUp);
            if (_frmDesigner.formAction == FormAction.Approve) this.DialogResult = DialogResult.Cancel;
            btInsertToDetail.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(btInsertToDetail_ItemPress);
            btSaveGrid.ItemPress += new DevExpress.XtraBars.ItemClickEventHandler(btSaveGrid_ItemPress);
            btSaveLayout.ItemPress += btSaveLayout_ItemPress;
            gvMain.CollapseAllDetails();
            gbMain.CollapseAllDetails();
            _data_DataChangedEvent();
        }

        void btSaveLayout_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
               lcMain.SaveLayoutToStream(ms);
               string English = Config.GetValue("Language").ToString() == "1" ? "_E" : "";
                _data.DrTable["FileLayout" + English] = ms.ToArray();
                _data.updateLayoutFile(_data.DrTable);
            }
            catch (Exception ex)
            { }
        }

        void btSaveGrid_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                return;
            }
            try
            {
                SysTable stb = new SysTable();

                foreach (GridColumn col in gvMain.Columns)
                {
                    CDTGridColumn col1 = (col as CDTGridColumn);
                    if (!(bool)(col1.Tag))
                    {
                        stb.UpdateColWidth(col1.MasterRow, col.Width);
                        stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                        stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                    }
                    else
                    {
                        double wi = (double)(col.Width / 2);
                        stb.UpdateColWidth(col1.MasterRow, (int)Math.Round(wi, 0));
                        stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                        stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                    }

                }
                stb.UpdateRowHeight(_data.DrTable["TableName"].ToString(), gvMain.RowHeight);

                foreach (GridLevelNode node in gcMain.LevelTree.Nodes)
                {
                    GridView gv = (node.LevelTemplate as GridView);
                    if (gv == null) continue;
                    stb.UpdateRowHeight(gv.Name,gv.RowHeight);
                    foreach (GridColumn col in gv.Columns)
                    {
                        CDTGridColumn col1 = (col as CDTGridColumn);
                         if (!(bool)(col1.Tag))
                        {
                            stb.UpdateColWidth(col1.MasterRow, col.Width);
                            stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                            stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                        }
                        else
                        {
                            double wi = (double)(col.Width / 2);
                            stb.UpdateColWidth(col1.MasterRow, (int)Math.Round(wi, 0));
                            stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                            stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                        }
                    }
                }



                for (int i = 0; i < this._frmDesigner._gcDetail.Count; i++)
                {
                    string s = _data._dtDetail.Rows[i]["ChildOf"].ToString();
                    if (!bool.Parse(s)) continue;
                    GridControl gr = this._frmDesigner._gcDetail[i];
                    GridView gv = gr.MainView as GridView;

                    foreach (GridColumn col in gv.Columns)
                    {
                        CDTGridColumn col1 = (col as CDTGridColumn);
                        if (!(bool)(col1.Tag))
                        {
                            stb.UpdateColWidth(col1.MasterRow, col.Width);
                            stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                            stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                        }
                        else
                        {
                            double wi = (double)(col.Width / 2);
                            stb.UpdateColWidth(col1.MasterRow, (int)Math.Round(wi, 0));
                            stb.UpdateColIndex(col1.MasterRow, col.VisibleIndex);
                            stb.UpdateColVisible(col1.MasterRow, col.Visible ? 1 : 0);
                        }
                    }
                }

            }
            catch {
                MessageBox.Show("Has Error!");
                return;
            }
            MessageBox.Show("Update complete!");
        }

        void btInsertToDetail_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string Mess = "Tạo lại dữ liệu chi tiết, dữ liệu chi tiết cũ sẽ bị mất!, Bạn có đồng ý không?";
            //if (Config.GetValue("Language").ToString() == "1")
            //    Mess = "Created detail again, old detail data was lost. Do you agree?";
            //if (MessageBox.Show(Mess, "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            _frmDesigner.InsertedToDetail = false;
            _frmDesigner.InsertDetailFromMTDT();
            //}
        }

        private void InitializeLayout()
        {
            this.SetFormCaption();
            this.AddLayoutControl();

            //dataNavigatorMain.SendToBack();
        }
        private void AddLayoutControl()
       {
           
           int fieldCount = _data.DsStruct.Tables[0].Rows.Count;
           string path ;
           string English = Config.GetValue("Language").ToString() == "1" ? "_E" : "";
           if (Config.GetValue("DuongDanLayout") == null)
               path = Application.StartupPath + "\\Layouts\\" + Config.GetValue("Package").ToString() + English + "\\" + _data.DrTable["TableName"].ToString() + ".xml";
           else
               path = Config.GetValue("DuongDanLayout").ToString() + "\\" + Config.GetValue("Package").ToString() + English + "\\" + _data.DrTable["TableName"].ToString() + ".xml";
            lcMain = _frmDesigner.GenLayout3(ref gcMain, true);
            //if (fieldCount > 3)
            //    lcMain = _frmDesigner.GenLayout3(ref gcMain, true);
            //else if (fieldCount > 2)
            //    lcMain = _frmDesigner.GenLayout2(ref gcMain, true);
            //else
            //    lcMain = _frmDesigner.GenLayout1(ref gcMain, true);

            if (_data.DrTable["FileLayout" + English] == DBNull.Value)
            {
                if (System.IO.File.Exists(path))
                {
                    lcMain.RestoreLayoutFromXml(path);
                    //UpLoad Layout to database
                    System.IO.MemoryStream ms=new MemoryStream();
                    lcMain.SaveLayoutToStream(ms);
                    _data.DrTable["FileLayout" + English] = ms.ToArray();
                    _data.updateLayoutFile(_data.DrTable);
                    lcMain.ShowCustomization += lcMain_ShowCustomization;
                }
            }
            else
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(_data.DrTable["FileLayout" + English] as byte[]);
                XmlDocument xmlDoc = new XmlDocument();

                // Tạo XmlReader từ MemoryStream
                using (XmlReader xmlReader = XmlReader.Create(ms))
                {
                    xmlDoc.Load(xmlReader);
                }
                using (MemoryStream modifiedMemoryStream = new MemoryStream())
                {
                    xmlDoc.Save(modifiedMemoryStream);

                    XmlNodeList nodes = xmlDoc.SelectNodes("//property[@name='Visibility'][. = 'OnlyInCustomization']");
                    foreach (XmlNode node in nodes)
                    {
                        node.InnerText = "Always";
                    }
                }
                // Đưa vị trí của MemoryStream về đầu để chuẩn bị cho việc đọc
                using (MemoryStream ms1 = new MemoryStream())
                {
                    xmlDoc.Save(ms1);

                    // Đưa vị trí của MemoryStream về đầu để chuẩn bị cho việc đọc
                    ms1.Position = 0;

                    // Hiển thị nội dung mới trong MemoryStream
                    //using (StreamReader reader = new StreamReader(ms1))
                    //{
                    //    string modifiedXmlContent = reader.ReadToEnd();
                    //    Console.WriteLine("Nội dung XML sau khi thay đổi:");
                    //    Console.WriteLine(modifiedXmlContent);
                    //}
                    try
                    {
                        lcMain.RestoreLayoutFromStream(ms1);

                    }
                    catch
                    {
                        lcMain.RestoreLayoutFromStream(ms);
                    }
                }

            }
           
           gcMain.DataSource = _bindingSource;
           gcMain.DataMember = this._data.DrTable["TableName"].ToString();
            try
            {
                this.Controls.Add(lcMain);
                lcMain.BringToFront();
            }
            catch
            {

            }
            //if (this._frmDesigner.formAction == FormAction.View)
            //{
            //    TransparentPanel ptop = new TransparentPanel();
            //    ptop.BackColor = Color.FromArgb(100, 88, 44, 55);
            //    ptop.Top = 0; ptop.Left = 0;
            //    ptop.TabIndex = 0;
            //    ptop.Width = Screen.PrimaryScreen.Bounds.Width;
            //    ptop.Height = Screen.PrimaryScreen.Bounds.Height;
            //    this.Controls.Add(ptop); ptop.BringToFront();

            //}
            gvMain = gcMain.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
           gvMain.OptionsView.ShowAutoFilterRow = false;
           gvMain.OptionsView.ShowGroupPanel = false;
           //gvMain.OptionsView.ShowFooter = false;
           //gvMain.BestFitColumns();
           //Thêm phần bindingSource cho các Detail
           for (int i = 0; i < _data._drTableDt.Count; i++)
           {
               GridControl gc = _frmDesigner._gcDetail[i];
               int position = _bindingSource.Position;
               gc.DataSource = _bindingSource;
               _bindingSource.Position =  position;
                if (_data.DsData.Tables[_data._drTableDt[i]["TableName"].ToString()].Columns.Contains("DTID"))
                {
                   // gc.DataMember = _data._drTableDt[i]["TableName"].ToString() + "1";
                }
                gc.DefaultView.ViewCaption = _data._drTableDt[i]["DienGiai"].ToString();
                gc.DataMember = _data._drTableDt[i]["TableName"].ToString();
               // (gc.MainView as GridView).AddNewRow();
            }
            //Thêm phần bindingSource cho các Detail
        }

        void lcMain_ShowCustomization(object sender, EventArgs e)
        {
            LayoutControl lcM = (sender as LayoutControl);

            bool admin = bool.Parse(Config.GetValue("Admin").ToString());
            if (!admin)
            {
                lcM.CustomizationForm.Close();
                return;
            }

        }

        



        //private void dataNavigatorMain_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        //{
        //    if (e.Button == dataNavigatorMain.Buttons.EndEdit)
        //    {
        //        Config.NewKeyValue("Operation", "F12-Lưu");
        //        e.Handled = true;
        //        UpdateData();
        //    }
        //    if (e.Button == dataNavigatorMain.Buttons.CancelEdit)
        //    {
        //        Config.NewKeyValue("Operation", "ESC-Hủy");
        //        e.Handled = true;
        //        CancelUpdate();
        //    }
        //}

        public void UpdateData()
        {
            if (!_data.DataChanged)
            {
                LogFile.AppendNewText("CheckErr.txt", "Data not change");
                return;
            }
            _frmDesigner.FirstControl.Focus();
            gvMain.RefreshData();
            _bindingSource.EndEdit();
            if (!_data.DataChanged)
            {
                this.DialogResult = DialogResult.Cancel;
                
            }
            else
            {
                DataAction dataAction = (_frmDesigner.formAction == FormAction.Edit) ? DataAction.Update : DataAction.Insert;
                _data.CheckRules(dataAction);
                //if (dxErrorProviderMain.HasErrors)
                //{
                //    XtraMessageBox.Show("Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!");
                //    return;
                //}
                if (_data.DrCurrentMaster.Table.Columns.Contains("NgayCt"))
                {
                    bool r = false;
                    DataView dv = _data.DrCurrentMaster.Table.DefaultView;


                    dv.RowStateFilter = DataViewRowState.ModifiedOriginal;
                    DataRowView drv = null;
                    if (dv.Count > 0)
                    {
                        drv = dv[0];
                    }

                    try
                    {
                        string Mess = "Số liệu đã bị khóa!";
                        if (Config.GetValue("Language").ToString() == "1")
                            Mess = "Data was locked!";
                        if (DateTime.Parse(_data.DrCurrentMaster["NgayCt"].ToString()) <= DateTime.Parse(Config.GetValue("Khoasolieu").ToString()))
                        {
                            XtraMessageBox.Show(Mess);
                            r = true;
                        }
                        if (!r && Config.Variables.Contains("Khoasolieu1") && DateTime.Parse(_data.DrCurrentMaster["NgayCt"].ToString()) > DateTime.Parse(Config.GetValue("Khoasolieu1").ToString()))
                        {
                            XtraMessageBox.Show(Mess);
                            r = true;
                        }
                        if (drv != null)
                        {
                            if (!r && DateTime.Parse(drv["NgayCt"].ToString()) <= DateTime.Parse(Config.GetValue("Khoasolieu").ToString()))
                            {
                                XtraMessageBox.Show(Mess);
                                r = true;
                            }
                            if (!r && Config.Variables.Contains("Khoasolieu1") && DateTime.Parse(drv["NgayCt"].ToString()) > DateTime.Parse(Config.GetValue("Khoasolieu1").ToString()))
                            {
                                XtraMessageBox.Show(Mess);
                                r = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "/n" + _data.DrCurrentMaster["NgayCt"].ToString() + "/n" + Config.GetValue("Khoasolieu").ToString() + "/n" + Config.GetValue("Khoasolieu1").ToString());
                    }
                
                    dv.RowStateFilter = DataViewRowState.CurrentRows;
                    if (r)
                    {
                        LogFile.AppendNewText("CheckErr.txt", "Khóa số liệu");
                        return;
                    }
                }

                if (_data.DsData.Tables[0].HasErrors)
                {
                    bool isErr = false;
                    foreach (DataColumn col in _data.DrCurrentMaster.Table.Columns)
                    {

                        if (isErr) break;
                        string err = _data.DrCurrentMaster.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                            isErr = true;
                            return;
                        }
                    }

                }

                if (_data.DsData.Tables[1].HasErrors)
                {
                    string Mess = "Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        Mess = "Data is not valid, please check data before saving!";
                    bool isErr = false;

                    foreach (DataColumn col in _data.DsData.Tables[1].Columns)
                    {
                        if (isErr) break;
                        foreach (DataRow drCurrentDT in _data.LstDrCurrentDetails)
                        {
                            if (isErr) break;
                            string err = drCurrentDT.GetColumnError(col.ColumnName);
                            if (err != string.Empty)
                            {
                                MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                                isErr = true;
                                return;
                            }
                        }

                    }
                    //LogFile.AppendNewText("CheckErr.txt", "Lỗi dt");
                    return;
                }
                for(int idx=2;idx<_data.DsData.Tables.Count;idx++)
                {
                    string tableNameCT = _data.DsData.Tables[idx].TableName;
                    if (!_data.DsData.Tables[idx].HasErrors) continue;
                    bool isErr = false;
                    List<CurrentRowDt> ldr = _data._lstCurRowDetail.Where(x => x.TableName == tableNameCT).ToList();
                    foreach (DataColumn col in _data.DsData.Tables[idx].Columns)
                    {
                        if (isErr) break;
                        foreach (CurrentRowDt CRDt in ldr)
                        {
                            DataRow dr = CRDt.RowDetail;
                            if (isErr) break;
                            string err = CRDt.RowDetail.GetColumnError(col.ColumnName);
                            if (err != string.Empty)
                            {
                                MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                                isErr = true;
                                return;
                            }
                        }
                    }

                }
                if (this._data.UpdateData(dataAction))
                {
                    //LogFile.AppendNewText("CheckErr.txt", "Update xong");
                    if (dataAction == DataAction.Insert && _data.DrCurrentMaster.Table.Columns.Contains("taskID") && _data.DrCurrentMaster["taskID"] == DBNull.Value)
                    {
                        (_data as DataMasterDetail).UpdateBeginTask();
                        SetCurrentData();
                    }
                    _x = false;
                    dataAction = DataAction.Update;
                    _frmDesigner.formAction = FormAction.Edit;

                    _frmDesigner.ClearFilter();
                    string Mess = "Số liệu số liệu đã được lưu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        Mess = "Data was saved";
                    this.pHis.LinksPersistInfo.Clear();
                    this.pHis.ClearLinks();
                    this._data.HistoryCurrents.Clear();
                    XtraMessageBox.Show(Mess);
                    bindingNavigator1.Focus();
                    tsNew.Select();
                }
                else if (dxErrorProviderMain.HasErrors)
                {
                    string Mess = "Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        Mess = "Data is not valid, please check data before saving!";

                    foreach (DataColumn col in _data.DsData.Tables[0].Columns)
                    {
                        string err = _data.DrCurrentMaster.GetColumnError(col.ColumnName);
                        if (err != string.Empty)
                        {
                            MessageBox.Show("Cột " + col.ColumnName + " Có lỗi: " + err);
                            break;
                        }
                    }
                    LogFile.AppendNewText("CheckErr.txt", "Lỗi MT");
                }
                else
                {
                    
                    foreach (DataTable dt in _data.DsData.Tables)
                        if (dt.HasErrors)
                        {
                            //string Mess = "Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!";
                            //if (Config.GetValue("Language").ToString() == "1")
                            //    Mess = "Data is not valid, please check data before saving!";
                            //bool isErr = false;
                            //foreach (DataColumn col in dt.Columns)
                            //{
                            //    if (isErr) break;
                               
                            //    foreach (DataRow drCurrentDT in _data.LstDrCurrentDetails)
                            //    {
                            //        if (isErr) break;
                            //        string err = drCurrentDT.GetColumnError(col.ColumnName);
                            //        if (err != string.Empty)
                            //        {
                            //            MessageBox.Show("Cột " + col.ColumnName + " có lỗi: " + err);
                            //            isErr = true;
                            //            return;
                            //        }
                            //    }

                            //}
                            
                            return;
                        }
                }
            }
        }

        private void CancelUpdate()
        {
            _x = false;
            _bindingSource.EndEdit();

            if (!_data.DataChanged || _frmDesigner.formAction == FormAction.Approve)
            {

                this.DialogResult = DialogResult.Cancel;
            }

            else
            {
                string Mess = "Số liệu chưa được lưu, bạn có thật sự muốn quay ra?";
                if (Config.GetValue("Language").ToString() == "1")
                    Mess = "Data is not saved, Do you want to cancel?";
                if (XtraMessageBox.Show(Mess, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _data.CancelUpdate();
                    this._frmDesigner.formAction = FormAction.View;
                    _bindingSource.ResetBindings(false);
                    
                    _bindingSource.DataSource = _data.DsData;
                    _frmDesigner.ClearFilter();
                    this._data.DsData.Tables[0].ColumnChanged += Table_ColumnChanged;
                    foreach (ICDTData pl in _lstICDTData)
                        pl.AddEvent();
                    this.DialogResult = DialogResult.Cancel;
                    this.pHis.LinksPersistInfo.Clear();
                    this.pHis.ClearLinks();
                    this._data.HistoryCurrents.Clear();
                }
            }
        }
        private void FrmMasterDetailDt_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    if (_data.DataChanged)
                    {
                        string Mess = "Dữ liệu chưa được lưu, bạn có muốn lưu không?";
                        if (Config.GetValue("Language").ToString() == "1")
                            Mess = "Data is not saved, Do you want to save?";
                        if (MessageBox.Show(Mess, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Config.NewKeyValue("Operation", "F12-Lưu");
                            UpdateData();
                            RemoveTabReport();

                        }
                        else
                        {
                            CancelUpdate();
                            break;
                        }
                    }
                    Config.NewKeyValue("Operation", "New");

                    _frmDesigner.formAction = FormAction.New;

                    _bindingSource.AddNew();
                    _bindingSource.EndEdit();
                    _frmDesigner.RefreshGridLookupEdit();
                    _x = true;
                    SetCurrentData();
                    _frmDesigner.RefreshFormulaDetail();
                    _frmDesigner.InsertedToDetail = false;
                    _frmDesigner.TabDetail.SelectedTabPage = _frmDesigner.TabDetail.TabPages[0];
                    break;
                case Keys.F12:
                    Config.NewKeyValue("Operation", "F11-Lưu");
                    UpdateData();
                    RemoveTabReport();
                    break;
                case Keys.S:
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        Config.NewKeyValue("Operation", "F11-Lưu");
                        UpdateData();
                        RemoveTabReport();
                    }
                    break;
                case Keys.F7:
                    if (_data.DrTable["Report"].ToString() == string.Empty)
                        return;
                    else
                    {
                        if (tsPrint.Visible)
                        {
                            int[] newIndex = { _bindingSource.Position };
                            BeforePrint bp = new BeforePrint(_data, newIndex);
                            bp.ShowDialog();
                        }
                    }
                    break;
                    
                case Keys.Escape:
                    CancelUpdate();
                    break;
                case Keys.F11:
                    Config.NewKeyValue("Operation", "F12-Lưu");
                    UpdateData();
                    RemoveTabReport();
                    if (!_x)
                    {
                        //if (_data.DrTable["Report"].ToString() == string.Empty)
                        //    return;
                       // else
                       // {
                            //int[] newIndex ={ _bindingSource.Position };
                         ///   BeforePrint bp = new BeforePrint(_data, newIndex);
                         //   bp.ShowDialog();
                       // }
                        Config.NewKeyValue("Operation", "New");

                        _frmDesigner.formAction = FormAction.New;

                        _bindingSource.AddNew();
                        _bindingSource.EndEdit();
                        _x = true;
                        SetCurrentData();
                        _frmDesigner.RefreshFormulaDetail();
                        _frmDesigner.InsertedToDetail = false;
                        _frmDesigner.TabDetail.SelectedTabPage = _frmDesigner.TabDetail.TabPages[0];
                    }
                    break;
                case Keys.Oem3:
                    if (e.Control)
                    {
                        var tabControl = _frmDesigner.TabDetail; 
                        int currentIndex = tabControl.SelectedTabPageIndex;
                        int nextIndex = (currentIndex + 1) % tabControl.TabPages.Count;
                        tabControl.SelectedTabPageIndex = nextIndex;
                        tabControl.Focus();
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void FrmMasterDetailDt_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (_x)
                CancelUpdate();
            _data.Reset();
            _frmDesigner.formAction = FormAction.View;
            _frmDesigner.FirstControl.Focus();
            RemoveTabReport();
        }
        private void RemoveTabReport()
        {
            int i = _frmDesigner.TabDetail.TabPages.Count - 1;
            while (i >= 0)
            {
               XtraTabPage tb = _frmDesigner.TabDetail.TabPages[i];
                if (tb.Text.Substring(0, 3) == "BC_")
                {
                    _frmDesigner.TabDetail.TabPages.Remove(tb);
                   // i--;
                }
                i--;
            }

        }
        private void dataNavigatorMain_Click(object sender, EventArgs e)
        {
           
        }

        private void barLargeButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var tabControl = _frmDesigner.TabDetail;
            int currentIndex = tabControl.SelectedTabPageIndex;
            DataRow drTableIndex = this._data.DrTable;
            if (currentIndex > 0)
            {
                drTableIndex=this.Data._drTableDt[currentIndex-1];
            }
            fImExcel fEx = new fImExcel(drTableIndex["sysTableID"].ToString());
            fEx.ShowDialog();
            if (fEx.dbEx == null)
            {

                return;
            }
            try
            {
                this.ImportDetailFromExcel(fEx.dbEx, fEx.MapStruct);
            }
            catch (Exception ex)
            {
            }
        }
        void TabDetail_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.pMInsetToDetail.ShowPopup(new Point(e.X, e.Y + (sender as DevExpress.XtraTab.XtraTabControl).Top));
            }
            
        }   
        private void ImportDetailFromExcel(DataTable dataTable, DataTable MapStruct)
        {

            GridView gv = gvMain;
            XtraTabControl tabControl = _frmDesigner.TabDetail;
            int currentIndex = tabControl.SelectedTabPageIndex;
            if (currentIndex > 0)
            {
                gv = _frmDesigner._gcDetail[currentIndex - 1].DefaultView as GridView;
            }
            foreach (DataRow drdata in dataTable.Rows)
            {
                gv.AddNewRow();

                DataRow drDetail;
                if (currentIndex == 0) drDetail = this._data.LstDrCurrentDetails[this._data.LstDrCurrentDetails.Count - 1];
                else
                {
                    string tabName = this._data._drTableDt[currentIndex - 1]["TableName"].ToString();
                    List<CurrentRowDt> currentRows = this._data._lstCurRowDetail.Where(x => x.TableName == tabName).ToList();
                    if (currentRows.Count == 0) continue;
                    drDetail = currentRows[currentRows.Count - 1].RowDetail;
                }
                foreach (DataRow drMap in MapStruct.Rows)
                {
                    try
                    {
                        if (drMap["ColName"] == DBNull.Value || drMap["ColName"].ToString().Trim()==string.Empty || drdata[drMap["ColName"].ToString()].ToString().Trim()== string.Empty) 
                            continue;
                        
                        if (drMap["Type"].ToString() == "10")
                        {
                            drDetail[drMap["FieldName"].ToString()] = drdata[drMap["ColName"].ToString()].ToString() == "1" ? true : false;
                        }
                        else
                        {
                            drDetail[drMap["FieldName"].ToString()] = drdata[drMap["ColName"].ToString()];
                        }    
                    }
                    catch { }
                }
                gv.RefreshData();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }



        private void glReport_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btSaveGrid_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            btSaveGrid_ItemPress(sender, e);
        }

        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string currenData="";
            currenData = (_data as DataMasterDetail).GenSQLDataLog();
            SaveFileDialog op = new SaveFileDialog();
            op.ShowDialog();
            if (op.FileName != string.Empty)
            {
                LogFile.AppendNewText(op.FileName, currenData);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.ShowDialog();
            if (op.FileName != string.Empty)
            {
                string readText = File.ReadAllText(op.FileName);
                ImportDataFromText(readText);



            }
        }
        private void ImportDataFromText(string readText)
        {
            Dictionary<string, object> myObj;
            myObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(readText);


            foreach (DataColumn col in this._data.DrCurrentMaster.Table.Columns)
            {
                if (myObj.ContainsKey(col.ColumnName) && col.ColumnName != _data.PkMaster.FieldName && myObj[col.ColumnName] != null)
                {
                    this._data.DrCurrentMaster[col] = myObj[col.ColumnName];
                }

            }
            string Detail = myObj["Detail"].ToString();
            object[] lrow = JsonConvert.DeserializeObject<object[]>(Detail);
            this._data.flagRowdeleteHandTotal = true;
            gvMain.SelectAll();
            gvMain.DeleteSelectedRows();
            this._data.flagRowdeleteHandTotal = false;
            foreach (object row in lrow)
            {
                Dictionary<string, object> myDetailObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(row.ToString());
                gvMain.AddNewRow();
                DataRow drDetail = this._data.LstDrCurrentDetails[this._data.LstDrCurrentDetails.Count - 1];

                foreach (DataColumn col in drDetail.Table.Columns)
                {
                    if (myDetailObj.ContainsKey(col.ColumnName) && col.ColumnName != _data.PkMaster.FieldName && col.ColumnName != _data.DrTable["Pk"].ToString() && myDetailObj[col.ColumnName] != null)
                    {
                        drDetail[col] = myDetailObj[col.ColumnName];
                    }
                    drDetail.EndEdit();
                }
                gvMain.RefreshData();
            }
            string MultiDetail = myObj["MultiDetail"].ToString();
            object[] lTableDt = JsonConvert.DeserializeObject<object[]>(MultiDetail);
            for (int ix = 0; ix < this._data._drTableDt.Count; ix++)
            {
                GridControl grDt = _frmDesigner._gcDetail[ix];
                (grDt.DefaultView as GridView).SelectAll();
                (grDt.DefaultView as GridView).DeleteSelectedRows();
                object OjTable = lTableDt[ix];
                Dictionary<string, object> myDetailtable = JsonConvert.DeserializeObject<Dictionary<string, object>>(OjTable.ToString());
                object[] lrowdt = JsonConvert.DeserializeObject<object[]>(myDetailtable["Rows"].ToString());
                foreach (object row in lrowdt)
                {
                    Dictionary<string, object> myDetailObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(row.ToString());
                    (grDt.DefaultView as GridView).AddNewRow();
                    int lastRow = this._data._lstCurRowDetail.Count - 1;
                    DataRow drDetail = this._data._lstCurRowDetail[lastRow].RowDetail;
                    foreach (DataColumn col in drDetail.Table.Columns)
                    {
                        if (myDetailObj.ContainsKey(col.ColumnName) && col.ColumnName != _data.PkMaster.FieldName && col.ColumnName != _data.DrTable["Pk"].ToString() && myDetailObj[col.ColumnName] != null && col.ColumnName != "MTID")
                        {
                            drDetail[col] = myDetailObj[col.ColumnName];
                        }
                    }
                    drDetail.EndEdit();
                }
                (grDt.DefaultView as GridView).RefreshData();
            }
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tableMau = new DataTable();tableMau.TableName = "Table1";
            gv.Columns.Clear();
            gc.BeginInit();          
            gc.MainView = gv;
            gc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            gv});

            gv.OptionsCustomization.AllowRowSizing = true;
            gv.OptionsPrint.EnableAppearanceEvenRow = true;
            gv.OptionsPrint.EnableAppearanceOddRow = true;
            gv.OptionsPrint.ExpandAllDetails = true;
            gv.OptionsPrint.PrintDetails = true;
            gv.OptionsPrint.UsePrintStyles = true;
            gv.OptionsView.ColumnAutoWidth = false;
            gv.OptionsView.EnableAppearanceEvenRow = true;
            gv.OptionsView.ShowAutoFilterRow = true;
          

            foreach (DataRow drMt in _data.DsStruct.Tables[0].Rows)
            {
                if (int.Parse(drMt["TabIndex"].ToString())>=0)
                {
                    DataColumn col = new DataColumn(drMt["FieldName"].ToString());
                    col.DataType = _data.DsData.Tables[0].Columns[drMt["FieldName"].ToString()].DataType;
                    tableMau.Columns.Add(col);
                    //dr[col] = col.ColumnName;
                    GridColumn gcol = new GridColumn();gcol.FieldName = col.ColumnName;gcol.Caption = col.ColumnName;gcol.Visible = true;
                    gcol.VisibleIndex = gv.Columns.Count ; gcol.Width = 100;
                    gv.Columns.Add(gcol);
                }
            }
            foreach (DataRow drDt in _data.DsStruct.Tables[1].Rows)
            {
                if (int.Parse(drDt["TabIndex"].ToString()) >=0)
                {
                    try
                    {
                        DataColumn col = new DataColumn(drDt["FieldName"].ToString());
                        col.DataType = _data.DsData.Tables[1].Columns[drDt["FieldName"].ToString()].DataType;
                        tableMau.Columns.Add(col);
                        GridColumn gcol = new GridColumn(); gcol.FieldName = col.ColumnName; gcol.Caption = col.ColumnName;  gcol.Visible = true;
                        gcol.VisibleIndex = gv.Columns.Count ; gcol.Width = 100;
                        gv.Columns.Add(gcol);
                        //dr[col] = col.ColumnName;
                    }
                    catch{ }
                }
            }
            
            SaveFileDialog df = new SaveFileDialog();
            df.Filter = "Excel|*.xls";
            gc.EndInit();
            if (df.ShowDialog() == DialogResult.OK)
            {               
                gc.DataSource = tableMau;               
                gv.ExportToXls(df.FileName);
            }
        }

        private void FrmMasterDetailDt_Load(object sender, EventArgs e)
        {

        }
    }
}
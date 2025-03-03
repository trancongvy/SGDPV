using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using CDTLib;
using DataFactory;
using CDTControl;
using Designflow;
using DevControls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using ErrorManager;

namespace FormFactory
{
    internal partial class FrmMasterDetail : CDTForm
    {
        private FrmMasterDetailDt frmMtDtCt;
        private GridControl gcDetail = new GridControl();
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
        private AdvBandedGridView gbDetail = new AdvBandedGridView();
        private bool referhComment = false;

        public FrmMasterDetail(CDTData data)
        {
            InitializeComponent();
            this._data = data;
            SetRight(); 
            this._frmDesigner = new FormDesigner(this._data, _bindingSource);
            InitializeLayout();


            this.Load += new EventHandler(FrmMasterDetail_Load);
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
        }
        
        private void SetRight()
        {

            //if ((_data as DataMasterDetail).isApp)
            //{

            //    if ((_data as DataMasterDetail).DrTableMaster["sysUserID"] != DBNull.Value && (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString() != string.Empty)
            //    {
            //        string adminList = (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString().Trim();
            //        if (adminList == Config.GetValue("sysUserID").ToString().Trim() || Boolean.Parse(Config.GetValue("Admin").ToString()))
            //        {
            //            lciApp.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            lciCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            lciReturn.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            lciUyquyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            return;
            //        }
            //        if ((_data as DataMasterDetail).isUyQuyen)
            //        {
            //            lciApp.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            lciCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            lciReturn.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //        }
            //    }
            //}



            if (_data.DrTableMaster == null) return;
            string sSelect = _data.DrTableMaster["sSelect"].ToString();
            string sInsert = _data.DrTableMaster["sInsert"].ToString();
            string sUpdate = _data.DrTableMaster["sUpdate"].ToString();
            string sDelete = _data.DrTableMaster["sDelete"].ToString();
            string sPrint = _data.DrTableMaster["sPrint"].ToString();

            if (!bool.Parse(Config.GetValue("Admin").ToString()))
            {
                lciDanhso.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            bool tSelect = false;
            if (sSelect == "0") tSelect = false;
            else if (sSelect == "1" || sSelect == string.Empty || sSelect == "True") tSelect = true;
            bool tInsert = false;
            if (sInsert == "0") tInsert = false;
            else if (sInsert == "1" || sInsert == string.Empty || sInsert == "True") tInsert = true;
            bool tUpdate = false;
            if (sUpdate == "0") tUpdate = false;
            else if (sUpdate == "1" || sUpdate == string.Empty || sUpdate == "True") tUpdate = true;
            bool tDelete = false;
            if (sDelete == "0") tDelete = false;
            else if (sDelete == "1" || sDelete == string.Empty || sDelete == "True") tDelete = true;

            bool tPrint = false;
            if (sPrint == "0") tPrint = false;
            else if (sPrint == "1" || sPrint == string.Empty || sPrint == "True" ) tPrint = true;


            if (sSelect != string.Empty && !tSelect)
                lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !tInsert)
            {
                lciNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            if (sUpdate != string.Empty && !tUpdate)
                lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sDelete != string.Empty && !tDelete)
                lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sPrint != string.Empty && !tPrint)
                lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !tInsert)
                lciCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            
        }


        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);
        }
        private void InitializeLayout()
        {
            this.SetFormCaption();
            if (_data.DsData == null)
                _data.GetData();
            if (_data.DrTableMaster == null) return;
            if (_data.DrTable.Table.Columns.Contains("useBand") && bool.Parse(_data.DrTableMaster["useBand"].ToString()))
            {
                string tablename = _data.DrTableMaster["TableName"].ToString();
                gcMain = this._frmDesigner.GenBandGridControl(this._data.DsStruct.Tables[tablename], _data._dsBand.Tables[_data.DrTableMaster["TableName"].ToString()], false, DockStyle.Fill);
                if (gcMain == null) return;
                gbMain = gcMain.ViewCollection[0] as AdvBandedGridView;
                gbMain.OptionsView.ShowDetailButtons = false;
                gbMain.OptionsBehavior.Editable = true;
                gbMain.OptionsBehavior.ReadOnly = true;
            }
            else
            {
                gcMain = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[0], false, DockStyle.Fill);
                if (gcMain == null) return;
                gvMain = gcMain.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
                gvMain.OptionsView.ShowDetailButtons = false;
                gvMain.OptionsBehavior.Editable = true;
                gvMain.OptionsBehavior.ReadOnly = true;
                foreach (GridColumn gc in gvMain.Columns)
                {
                    if(gc.ColumnEdit!=null && (gc.ColumnEdit as CDTRepGridLookup) != null)
                    {
                        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
                            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
                    }
                    
                }
            }
            
            this.Controls.Add(gcMain);

            SplitterControl spc = new SplitterControl();
            spc.Dock = DockStyle.Bottom;
            this.Controls.Add(spc);
            if (_data.DrTable.Table.Columns.Contains("useBand") && bool.Parse(_data.DrTable["useBand"].ToString()))
            {    
                gcDetail = this._frmDesigner.GenBandGridControl(this._data.DsStruct.Tables[1], _data._dsBand.Tables[_data.DrTable["TableName"].ToString()] , false, DockStyle.Bottom);
                gbDetail = gcDetail.ViewCollection[0] as AdvBandedGridView;
                gbDetail.OptionsBehavior.Editable = true;
                gbDetail.OptionsBehavior.ReadOnly = true;
                gbDetail.OptionsView.ShowFooter = false;
                gbDetail.OptionsView.ShowGroupPanel = false;
                gbDetail.OptionsView.ShowDetailButtons = false;
            }
            else
            {
                gcDetail = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[1], false, DockStyle.Bottom);
                gvDetail = gcDetail.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
                gvDetail.OptionsBehavior.Editable = true;
                gvDetail.OptionsBehavior.ReadOnly = true;
                gvDetail.OptionsView.ShowFooter = true;
                gvDetail.OptionsView.ShowGroupPanel = false;
                gvDetail.OptionsView.ShowDetailButtons = false;
                foreach (GridColumn gc in gvDetail.Columns)
                {
                    if (gc.ColumnEdit != null && (gc.ColumnEdit as CDTRepGridLookup) != null)
                    {
                        foreach (EditorButton b in (gc.ColumnEdit as CDTRepGridLookup).Buttons)
                            if (b.Kind == ButtonPredefines.Plus) b.Visible = false;
                    }

                }
            }
            this.Controls.Add(gcDetail);
            gcMain.MouseUp += new MouseEventHandler(gcMain_MouseUp);
            gcDetail.SendToBack();
            layoutControl2.SendToBack();
            gcDetail.Height =  this.Height / 3;
            //Thêm vào phần duyệt
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvMain.Columns)
            {
                if (col.FieldName.ToUpper() == "TASKID")
                {
                    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repTask = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    this.gcMain.RepositoryItems.Add(repTask);

                    repTask.AutoHeight = false;
                    repTask.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    if (_data.tbTask != null)
                    {
                        for (int i=0; i<_data.tbTask.Rows.Count;i++)
                        {
                            DataRow drTask=_data.tbTask.Rows[i];
                            if (drTask["Icon"] != DBNull.Value)
                            {
                                this.imageList2.Images.Add(GetImage(drTask["Icon"] as byte[]));
                                repTask.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(drTask["TaskLabel"].ToString(), drTask["ID"], repTask.Items.Count)});
                            }
                        }
                    }
                    repTask.SmallImages = this.imageList2;

                    col.ColumnEdit = repTask;
                    col.Caption = "";
                }
                //if (col.FieldName.ToUpper() == "APPROVED" || col.FieldName.ToUpper() == "HOANTHANH")
                //{

                    //if (col.FieldName.ToUpper() == "APPROVED")
                    //{
                    //    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox rep = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    //    this.gcMain.RepositoryItems.Add(rep);

                    //    rep.AutoHeight = false;
                    //    rep.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //    rep.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                    //     new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Chưa duyệt", 0, 2),
                    //     new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Đã duyệt", 1, 0)});
                    //    rep.SmallImages = this.imageList1;
                    //    col.ColumnEdit = rep;
                    //    col.Caption = "";

                    //}
                    //if (col.FieldName.ToUpper() == "HOANTHANH")
                    //{
                    //    DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox rep1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    //    this.gcMain.RepositoryItems.Add(rep1);

                    //    rep1.AutoHeight = false;
                    //    rep1.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //    rep1.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
                    //     new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Chưa hoàn thành", false, 2),
                    //     new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Đã hoàn thành", true, 0)});
                    //    rep1.SmallImages = this.imageList1;
                    //    col.ColumnEdit = rep1;
                    //    col.Caption = "Hoàn thành";

                    //}

                   
               // }
                gvMain.GroupFormat = "[#image]{1}";
            }

        }
        void gcMain_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && ModifierKeys == Keys.Control && Boolean.Parse(Config.GetValue("Admin").ToString()))
            {
                this.pMSysControl.ShowPopup(new Point(e.X + this.Left + this.Parent.Left, e.Y + this.Top + this.Parent.Top));
               //int i= Config.Variables.Count;
            }
            else if (e.Button == MouseButtons.Right)
            {
                foreach (BarButtonItem it in barManager2.Items)
                {
                    it.Visibility = BarItemVisibility.Always;
                }
                this.pMAction.ShowPopup(new Point(e.X + this.Left + this.Parent.Left, e.Y + this.Top + this.Parent.Top));
            }
        }

        private void FrmMasterDetail_Load(object sender, EventArgs e)
        {
            //if (_data.DsData == null)
            //    _data.GetData();
            DisplayData();
            InitActionCommand();
            pMAction.Popup += new EventHandler(pMAction_Popup);
            referhComment = true;
            try
            {
                timer1_Tick(timer1, new EventArgs());
            }
            catch {  }
            gvMain.CustomDrawCell += gvMain_CustomDrawCell;
            referhComment = false;
            if (int.Parse(Config.GetValue("SoftType").ToString()) != 1)
            {
                barDesignFlow.Enabled = false;
            }
            else barDesignFlow.Enabled = true;
            //gvMain.ExpandAllGroups();
            
            gvMain.OptionsBehavior.AutoExpandAllGroups = true;
        }
        int nhapnhay = 0;
        void gvMain_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (isDeleting) return;
            DataRowView dr = gvMain.GetRow(e.RowHandle) as DataRowView;
            if (dr == null || !dr.Row.Table.Columns.Contains("HasComment") || dr["HasComment"].ToString() != "1") return;
            e.Appearance.BackColor = Color.AliceBlue;            
            e.Appearance.BackColor2 = Color.AntiqueWhite;
            e.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }



        void pMAction_Popup(object sender, EventArgs e)
        {
            //  ẩn các action ko cần thiết đi
            DataRow drCurrent = null;
            int[] oldIndexList = gvMain.GetSelectedRows();
            if (oldIndexList.Length > 0)
            {
                if (gvMain.SortedColumns.Count > 0 || gvMain.GroupCount > 0)
                {
                    drCurrent = gvMain.GetDataRow(oldIndexList[0]);
                }
            }bool visible = false;
            if (drCurrent != null)
            {

                string approved = drCurrent["Approved"].ToString();

                for (int i = 0; i < oldIndexList.Length; i++)
                {
                    if (approved != gvMain.GetDataRow(oldIndexList[i])["Approved"].ToString())
                    {
                        visible = false;
                        break;
                    }
                    visible = true;
                }
            }
                barManager2.BeginInit();
            foreach (BarButtonItem it in barManager2.Items)
            {
                //if (it.Name == dr["CommandName"].ToString())
                //{}
                
                DataRow drAction = it.Tag as DataRow;
                if (it.Tag == null) continue;
                if (drCurrent == null || drAction["BTID"].ToString() != drCurrent["TaskID"].ToString() ||!visible)
                {
                    it.Visibility = BarItemVisibility.Never;
                    continue;
                }
                string condition = drAction["ShowCond"].ToString();
                if (condition == string.Empty) condition = "(1=1)";
                string con;

                DataRow[] drActionSecu = this._data.tbUAction.Select("ActionID='" + drAction["ID"].ToString() + "'");
                if (drActionSecu.Length == 0) condition += " and (1=0)";
                else if (drActionSecu[0]["CAllow"] == DBNull.Value || drActionSecu[0]["CAllow"].ToString() == string.Empty)
                    condition += " and (1=1)";
                else
                    condition += " and (" + drActionSecu[0]["CAllow"].ToString() + ")";
                if (condition != string.Empty)
                {
                    condition = _data.UpdateSpecialCondition(condition);
                    con = "(" + condition + ") and (" + this._data.PkMaster.FieldName + "='" + drCurrent[_data.PkMaster.FieldName].ToString() + "')";
                }
                else
                {
                    con = "(" + this._data.PkMaster.FieldName + "='" + drCurrent[_data.PkMaster.FieldName].ToString() + "')";
                }
                DataRow[] lstDr = drCurrent.Table.Select(con);
                if (lstDr.Length == 0)
                {
                    it.Visibility = BarItemVisibility.Never;
                }
                else
                {
                    it.Visibility = BarItemVisibility.Always;
                }
                //}
            }
            barManager2.EndInit();

        }
        private void InitActionCommand()
        {
            if (this._data.tbAction == null || this._data.tbAction.Rows.Count == 0) return;
            barManager2.BeginInit();
            foreach (DataRow dr in this._data.tbAction.Rows)
            {
                //if (bool.Parse(dr["AutoDo"].ToString())) continue;
                BarButtonItem barAction=new BarButtonItem();
                if (dr["Icon"] != DBNull.Value)
                {
                    Image im = GetImage(dr["Icon"] as byte[]);
                    if (im != null){
                        imageList3.Images.Add(im);
                        barAction.ImageIndex = imageList3.Images.Count - 1;
                    }
                }
                //barAction.Name = dr["CommandName"].ToString();
                barAction.Caption = dr["CommandName"].ToString();
                barAction.Tag = dr;
                barAction.Id = this._data.tbAction.Rows.IndexOf(dr);
                barAction.ItemClick += new ItemClickEventHandler(barAction_ItemClick);
                barAction.Visibility = BarItemVisibility.Always;
                barManager2.Items.Add(barAction);
                pMAction.LinksPersistInfo.Add(new LinkPersistInfo(barAction));
            }
            barManager2.EndInit();
        }

        void barAction_ItemClick(object sender, ItemClickEventArgs e)
        {
            referhComment = false;
            DataRow dr = e.Item.Tag as DataRow;
            if (this._data.DbData.HasErrors) this._data.DbData.HasErrors = false;
            string Confirm = "";
            if (Config.GetValue("Language").ToString() == "1")
                Confirm = "Are you sure " + e.Item.Caption + "?";
            else
                Confirm = "Bạn có chắc thực hiện " + e.Item.Caption + "?";
            if (dr.Table.Columns.Contains("Confirm") && dr["Confirm"] != DBNull.Value && dr["Confirm"].ToString() != string.Empty)
                Confirm = dr["Confirm"].ToString();
            if (XtraMessageBox.Show(Confirm, "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Config.NewKeyValue("Operation", "Edit");
                _frmDesigner.formAction = FormAction.Approve;
                if (frmMtDtCt == null)
                    frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
                frmMtDtCt.ShowDialog();
                frmMtDtCt.SetCurrentData();


                if (!_data.DataChanged)
                {
                    //frmMtDtCt.UpdateData();

                    if (!(this._data as DataMasterDetail).doAction(dr))
                    {
                        if (dr.Table.Columns.Contains("Message") && dr["Message"] != DBNull.Value && dr["Message"].ToString() != string.Empty)
                            MessageBox.Show(dr["Message"].ToString());
                    }
                }
            }
            _data.GetData();
            _frmDesigner.RefreshDataForLookup();
            DisplayData();
            gvDetail.ClearColumnsFilter();


            referhComment = true;
        }
        private void DisplayData()
        {
            //if (_data.DsData == null || gcMain==null)
            //    return;
            this._bindingSource.DataSource = _data.DsData;
            
            this._bindingSource.CurrentChanged += new EventHandler(bindingSource_CurrentChanged);
            
           // bindingSource_CurrentChanged(_bindingSource, new EventArgs());
            this._bindingSource.DataMember = _data.DsData.Tables[0].TableName;
            this.gcMain.DataSource = _bindingSource;
            gcDetail.DataSource = _bindingSource;
            gcDetail.DataMember = this._data.DrTable["TableName"].ToString();
            //gvMain.BestFitColumns();
            if (!gvMain.IsGroupRow(RowGroup))
            {
                gvMain.ExpandGroupRow(-1);
            }
            else
            {
                gvMain.ExpandGroupRow(RowGroup);
            }    
            
           
           // gvDetail.BestFitColumns();
        }

        void bindingSource_CurrentChanged(object sender, EventArgs e)
        {
            
            if ( isDeleting) return;
            if (_data.DataChanged ==true) return;
            simpleButtonDelete.Enabled = (_bindingSource.Count > 0);
            simpleButtonEdit.Enabled = (_bindingSource.Count > 0);
            simpleButtonCopy.Enabled = (_bindingSource.Count > 0);
            simpleButtonPreview.Enabled = (_bindingSource.Count > 0);
            DataRowView drv = (_bindingSource.Current as DataRowView);
            if (drv == null) return;
            this._data.DrCurrentMaster = drv.Row;
            //SetCurrentData
            DataRow[] drDetails;
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
                foreach (DataRow crRDt in _lstRowDt)
                {
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
            _data._formulaCaculator.LstCurrentRowDt = _data._lstCurRowDetail;
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
            //SetCurrentData
            if (_data.tbUTask == null) return;
            if (!drv.Row.Table.Columns.Contains("TaskID")) return;
            DataRow[] drTaskSecu = this._data.tbUTask.Select("TaskID='" + drv.Row["TaskID"].ToString() + "'");
            if (drTaskSecu.Length == 0)
            {
                //lciNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                if (drTaskSecu[0]["CView"].ToString() == "" || drTaskSecu[0]["CView"].ToString() == "1") drTaskSecu[0]["CView"] = "1=1";
                if (drTaskSecu[0]["CEdit"].ToString() == "" || drTaskSecu[0]["CEdit"].ToString() == "1") drTaskSecu[0]["CEdit"] = "1=1";
                if (drTaskSecu[0]["CDelete"].ToString() == "" || drTaskSecu[0]["CDelete"].ToString() == "1") drTaskSecu[0]["CDelete"] = "1=1";
                if (drTaskSecu[0]["CPrint"].ToString() == "" || drTaskSecu[0]["CPrint"].ToString() == "1") drTaskSecu[0]["CPrint"] = "1=1";
                string scon = _data.PkMaster.FieldName + "='" + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + "' and (" + drTaskSecu[0]["CView"].ToString() + ")";
                scon = _data.UpdateSpecialCondition(scon);
                DataRow[] sSelect = _data.DrCurrentMaster.Table.Select(scon);
                string econ = _data.PkMaster.FieldName + "='" + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + "' and (" + drTaskSecu[0]["CEdit"].ToString() + ")";
                econ = _data.UpdateSpecialCondition(econ);
                DataRow[] sUpdate = _data.DrCurrentMaster.Table.Select(econ);

                string dcon = _data.PkMaster.FieldName + "='" + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + "' and (" + drTaskSecu[0]["CDelete"].ToString() + ")";

                dcon = _data.UpdateSpecialCondition(dcon);
                DataRow[] sDelete = _data.DrCurrentMaster.Table.Select(dcon);

                string pcon = _data.PkMaster.FieldName + "='" + _data.DrCurrentMaster[_data.PkMaster.FieldName].ToString() + "' and (" + drTaskSecu[0]["CPrint"].ToString() + ")";
                 DataRow[] sPrint = _data.DrCurrentMaster.Table.Select(pcon);

                if (sSelect.Length > 0)
                {
                    lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciView.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciView.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                if (sUpdate.Length > 0)
                    lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (sDelete.Length > 0)
                    lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (sPrint.Length > 0)
                    lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                else
                    lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            }
            //if (_bindingSource.Count > 0) _bindingSource.MoveFirst();
            try
            {
                int i = _data.DsData.Tables[0].Rows.IndexOf(_data.DrCurrentMaster);
                i = gvMain.GetRowHandle(i);
                if (!gvMain.IsGroupRow(i)){
                    RowGroup = gvMain.GetParentRowHandle(i);
                }
            }
            catch { }
        }

       
        private void simpleButtonNew_Click(object sender, EventArgs e)
        {

            if (!_data.KiemtraDemo()) return;
            referhComment = false;
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ClearColumnsFilter();
            _frmDesigner.formAction = FormAction.New;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            _bindingSource.AddNew();
            
            _bindingSource.EndEdit();
            _frmDesigner.RefreshGridLookupEdit();
            frmMtDtCt.ShowDialog();  
            if (frmMtDtCt.refresh)
            {
                RefreshData();
                frmMtDtCt.refresh = false;
            }
            bindingSource_CurrentChanged(_bindingSource, e);
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            this.gvMain.OptionsSelection.MultiSelect = false;           
            this.gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsView.ShowGroupPanel = false;
            this.gvMain.SelectRow(_data.DsData.Tables[0].Rows.Count - 1);
            referhComment = true; 
        }

        private void simpleButtonEdit_Click(object sender, EventArgs e)
        {
            referhComment = false;
            if ((_data as DataMasterDetail).DrTableMaster["sysUserID"] != DBNull.Value && (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString() != string.Empty)
            {
                string adminList = (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString().Trim();
                DataRow drCurrent;
                if (adminList != Config.GetValue("sysUserID").ToString().Trim())
                {
                    //int[] oldIndexList = gvMain.GetSelectedRows();
                    //if (oldIndexList.Length > 0)
                    //{

                    //    if (gvMain.SortedColumns.Count > 0 || gvMain.GroupCount > 0)
                    //    {
                    //        drCurrent = gvMain.GetDataRow(oldIndexList[0]);
                    //        if (int.Parse(drCurrent["Approved"].ToString()) > 0)
                    //        {
                    //            MessageBox.Show("Can't delete when Approved ", "Thông báo");
                    //            if (!drCurrent.Table.Columns.Contains("isReturn"))
                    //                return;
                    //            else if (drCurrent["isReturn"]==DBNull.Value || drCurrent["isReturn"].ToString() == "False")
                    //                    return;
                    //        }
                    //    }
                    //}
                }
            }
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ActiveFilterString = "";
            gvMain.ApplyColumnsFilter();
            _frmDesigner.formAction = FormAction.Edit;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
            if (frmMtDtCt.refresh)
            {
                RefreshData();
                frmMtDtCt.refresh = false;
            }
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            referhComment = true;
        }
        bool isDeleting = false;
        private void simpleButtonDelete_Click(object sender, EventArgs e)
        {
            int[] oldIndexList = gvMain.GetSelectedRows();
            if (_bindingSource.Current != null)
            {
                DataRow drCurrent;
                //  if (gvMain.SortedColumns.Count > 0 || gvMain.GroupCount > 0)
                // {
                drCurrent = (_bindingSource.Current as DataRowView).Row;

                if (oldIndexList.Length == 0)
                {

                }
                bool admin = bool.Parse(Config.GetValue("Admin").ToString());
                if (!admin)
                {
                    if (_data.DrTableMaster["sysUserid"] != DBNull.Value && int.Parse(drCurrent["Approved"].ToString()) > 0)
                    {
                        MessageBox.Show("Can't delete when Approved", "Thông báo");
                        return;
                    }
                }
                if (drCurrent.Table.Columns.Contains("NgayCt"))
                {
                    try
                    {
                        if (DateTime.Parse(drCurrent["NgayCt"].ToString()) <= DateTime.Parse(Config.GetValue("Khoasolieu").ToString()))
                        {
                            XtraMessageBox.Show("Số liệu đã bị khóa!");
                            LogFile.AppendToFile("CheckErr.txt", "Ngày chứng từ :" + DateTime.Parse(drCurrent["NgayCt"].ToString()).ToShortDateString());
                            LogFile.AppendToFile("CheckErr.txt", "Ngày Khóa Số liệu :" + DateTime.Parse(Config.GetValue("Khoasolieu").ToString()).ToShortDateString());
                            return;
                        }
                        if (Config.Variables.Contains("Khoasolieu1") && DateTime.Parse(drCurrent["NgayCt"].ToString()) > DateTime.Parse(Config.GetValue("Khoasolieu1").ToString()))
                        {
                            LogFile.AppendToFile("CheckErr.txt", "Ngày Khóa Số liệu 1:" + DateTime.Parse(Config.GetValue("Khoasolieu1").ToString()).ToShortDateString());
                            XtraMessageBox.Show("Số liệu đã bị khóa!");
                            return;
                        }
                    }
                    catch { }
                }
                //  }
            }
            else
            {
                XtraMessageBox.Show("Chưa chọn dòng xóa!");
                return;
            }    
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            try
            {
                if (_bindingSource.Current != null && XtraMessageBox.Show("Vui lòng xác nhận xóa dữ liệu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _frmDesigner.formAction = FormAction.Delete;
                    this._data.LstDrCurrentDetails.Clear();
                    this._data._formulaCaculator.Active = false;
                    isDeleting = true;
                    _bindingSource.RemoveCurrent();
                    if (!_data.UpdateData(DataAction.Delete))
                    {
                        _data.CancelUpdate();
                        DisplayData();
                    }
                    isDeleting = false;
                    //_bindingSource.MoveFirst();
                    bindingSource_CurrentChanged(_bindingSource, new EventArgs());
                    
                    this._data._formulaCaculator.Active = true;
                    _data.Reset();
                }
            }
            catch
            {
                isDeleting = false;
            }
        }

        private void simpleButtonCopy_Click(object sender, EventArgs e)
        {
            if (!_data.KiemtraDemo()) return;
            string s = gvMain.ActiveFilterString;
            gvMain.ClearColumnsFilter();
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Copy;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
        }

        private void simpleButtonSearch_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string Quetsion = "Chọn Có để tìm kiếm thông tin trên bảng chính \n" +
                "Chọn Không để tìm kiếm thông tin trên bảng chi tiết \n";
            if(Config.GetValue("Language").ToString()=="1")
                Quetsion = "Choose Yes to Find in Master data\n" +
                "Choose No to Find in Detail data\n";
            DialogResult dr = XtraMessageBox.Show(Quetsion, "Confirm", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                gvMain.ShowFilterEditor(gvMain.Columns[0]);
                if (gvMain.RowFilter != string.Empty)
                {
                    SqlSearching sSearch = new SqlSearching();
                    string sql = sSearch.GenSqlFromGridFilter(gvMain.RowFilter);
                    string tmp = _data.ConditionMaster;
                    _data.ConditionMaster = sql;
                    _data.Condition = string.Empty;
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvMain.ClearColumnsFilter();
                    XtraMessageBox.Show("Kết quả tìm kiếm: " + gvMain.DataRowCount.ToString() + " mục số liệu");
                    //_data.ConditionMaster = tmp;
                }
            }
            else if (dr == DialogResult.No)
            {
                gvDetail.ShowFilterEditor(gvDetail.Columns[0]);
                if (gvDetail.RowFilter != string.Empty)
                {
                    SqlSearching sSearch = new SqlSearching();
                    string sql = sSearch.GenSqlFromGridFilter(gvDetail.RowFilter);
                    _data.ConditionMaster = string.Empty;
                    _data.Condition = sql;
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvDetail.ClearColumnsFilter();
                    XtraMessageBox.Show("Kết quả tìm kiếm: " + gvMain.DataRowCount.ToString() + " mục số liệu");
                }
            }
            timer1_Tick(timer1, new EventArgs());
        }

        private void simpleButtonPreview_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            if (gvMain.SelectedRowsCount == 0)
                return;
            //if (_data.DrTable["Report"].ToString() == string.Empty)
            //    gcMain.ShowPrintPreview();
            //else
           // {
                int[] oldIndex = gvMain.GetSelectedRows();
                int[] newIndex = oldIndex;
                if (gvMain.SortedColumns.Count > 0 || gvMain.GroupCount>0)
                    for (int i = 0; i < oldIndex.Length; i++)
                        newIndex[i] = _data.DsData.Tables[0].Rows.IndexOf(gvMain.GetDataRow(oldIndex[i]));

                BeforePrint bp = new BeforePrint(_data, newIndex);
                bp.ShowDialog();
           // }
        }
        int RowGroup = -1;
        private void simpleButtonCancel2_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            this.Close();
        }

        private void FrmMasterDetail_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    if (lciNew.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonNew_Click(simpleButtonNew, new EventArgs());
                    break;
                case Keys.F3:
                    if (lciEdit.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonEdit_Click(simpleButtonEdit, new EventArgs());
                    break;
                case Keys.F4:
                   if (lciDelete.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                       simpleButtonDelete_Click(simpleButtonDelete, new EventArgs());                   
                    break;
                case Keys.F5:
                    RefreshData();
                    
                    break;
                case Keys.F6:
                    if (lciSearch.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonSearch_Click(simpleButtonSearch, new EventArgs());
                    break;
                case Keys.F7:
                    if (lciPrint.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonPreview_Click(simpleButtonPreview, new EventArgs());
                    break;
                case Keys.F12:
                    if (lciApp.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButton2_Click(BtApp, new EventArgs());
                    break;
                case Keys.F11:
                    if (lciCancel.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        btCancel_Click(btCancel, new EventArgs());
                    break;
                case Keys.F10:
                    if (lciReturn.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        btReturn_Click(btReturn, new EventArgs());
                    break;
                case Keys.Escape:
                    simpleButtonCancel2_Click(simpleButtonCancel2, new EventArgs());
                    break;
            }
            
        }

        private void RefreshData()
        {
            referhComment = false;
            _data.GetData();
            _frmDesigner.RefreshDataForLookup();
            DisplayData();
            gvDetail.ClearColumnsFilter();
            referhComment = true;
            timer1_Tick(timer1, new EventArgs());
            
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Bạn có chắc duyệt số liệu này không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
                _frmDesigner.formAction = FormAction.Approve;
                if (frmMtDtCt == null)
                    frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
                frmMtDtCt.ShowDialog();
                frmMtDtCt.SetCurrentData();
                if (Approve())
                {
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvDetail.ClearColumnsFilter();
                }
            }
        }

        private bool Approve()
        {

            try
            {

                if ((this._data as DataMasterDetail).Approve())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
           
                Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
                _frmDesigner.formAction = FormAction.Approve;
                if (frmMtDtCt == null)
                    frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
                frmMtDtCt.ShowDialog();
                frmMtDtCt.SetCurrentData();
                if (_data.DrCurrentMaster["Approved"].ToString() == "1")
                {
                    MessageBox.Show("Dữ liệu đã được duyệt!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
                if (XtraMessageBox.Show("Bạn có chắc hủy số liệu này không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if ((this._data as DataMasterDetail).Cancel())
                    {
                        _data.GetData();
                        _frmDesigner.RefreshDataForLookup();
                        DisplayData();
                        gvDetail.ClearColumnsFilter();
                    }
                }
        }

        private void btReturn_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Approve;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
            frmMtDtCt.SetCurrentData();
            if (_data.DrCurrentMaster["Approved"].ToString() != "1")
            {
                MessageBox.Show("Dữ liệu chưa được duyệt!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            DialogResult yesno = XtraMessageBox.Show("Các dữ liệu liên quan trở về sau sẽ gữi nguyên  hoặc bị xóa ! Bạn có muốn giữ nguyên số liệu liên quan này về không? ", "Xác nhận", MessageBoxButtons.YesNoCancel);
            if (yesno == DialogResult.Yes)
            {
                if ((this._data as DataMasterDetail).Return(1))
                {
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvDetail.ClearColumnsFilter();
                }
            }
            if (yesno == DialogResult.No)
            {
                if ((this._data as DataMasterDetail).Return(0))
                {
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvDetail.ClearColumnsFilter();
                }
            }
        }

        private void btShare_Click(object sender, EventArgs e)
        {
            int id = int.Parse(_data.DrTable["sysTableID"].ToString());
            Uyquyen tbUyQuyen = new Uyquyen(id);
            tbUyQuyen.ShowDialog();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            List<DataRow> drDetails = new List<DataRow>();

            for (int i = 0; i < gvDetail.DataRowCount; i++)
                drDetails.Add(gvDetail.GetDataRow(i));

            this._data.LstDrCurrentDetails = drDetails;
            fShowHistoryMtdt  fSHMt = new fShowHistoryMtdt(_data as DataMasterDetail);
            fSHMt.ShowDialog();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (int.Parse(Config.GetValue("SoftType").ToString()) == 1)
            {
                Ev E = new Ev(this._data);
                E.ShowDialog();
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            fShowActionHistory fsAcHis = new fShowActionHistory(this._data as DataMasterDetail);
            fsAcHis.ShowDialog();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRowView drv = (_bindingSource.Current as DataRowView);
            if (drv == null) return;
            this._data.DrCurrentMaster = drv.Row;
            fShowDocWF fs = new fShowDocWF(_data as DataMasterDetail);
            fs.ShowDialog();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fFlowAnalyze fflowanalyze = new fFlowAnalyze();
            fflowanalyze.ShowDialog();
        }

        private void btSaveGrid_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SysTable stb = new SysTable();
                foreach (CDTGridColumn col in gvMain.Columns)
                {
                    if (!col.isExCol)
                    {
                        stb.UpdateColWidth(col.MasterRow, col.Width);
                        stb.UpdateColIndex(col.MasterRow, col.VisibleIndex);
                        stb.UpdateColVisible(col.MasterRow, col.Visible ? 1 : 0);
                    }
                }
                if (this._frmDesigner._gcDetail != null)
                {
                    for (int i = 0; i < this._frmDesigner._gcDetail.Count; i++)
                    {
                        GridControl gr = this._frmDesigner._gcDetail[i];
                        GridView gv = gr.MainView as GridView;

                        foreach (CDTGridColumn col in gv.Columns)
                        {
                            if (!col.isExCol)
                            {
                                stb.UpdateColWidth(col.MasterRow, col.Width);
                                stb.UpdateColIndex(col.MasterRow, col.VisibleIndex);
                                stb.UpdateColVisible(col.MasterRow, col.Visible ? 1 : 0);
                            }
                        }
                    }
                }

            }
            catch { }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            if (_data.tbTask != null)
            {
                foreach (DataRow dr in _data.tbTask.Rows)
                {
                    if (dr["isBegin"].ToString() == "True")
                    {
                        if (_data.DrCurrentMaster["TaskID"] == DBNull.Value)
                        {
                            _data.DrCurrentMaster["TaskID"] = dr["ID"];
                            _data.UpdateData(DataAction.Update);
                            RefreshData();
                        }
                        break;
                    }
                   
                }
            }
             
        }

        private void barButtonItem1_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            if (_bindingSource.Position < 0) return;
            fComment fc = new fComment(_data, _bindingSource.Position);
            fc.ShowDialog();
        }

        private void barButtonItem3_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            List<DataRow> drDetails = new List<DataRow>();

            for (int i = 0; i < gvDetail.DataRowCount; i++)
                drDetails.Add(gvDetail.GetDataRow(i));

            this._data.LstDrCurrentDetails = drDetails;
            fShowHistoryMtdt fSHMt = new fShowHistoryMtdt(_data as DataMasterDetail);
            fSHMt.ShowDialog();
        }

        private void barButtonItem4_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            fShowActionHistory fsAcHis = new fShowActionHistory(this._data as DataMasterDetail);
            fsAcHis.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //return;
            if (!referhComment) return;
            if (_data.DsData==null || _data.DsData.Tables==null || _data.DsData.Tables.Count == 0) return;
            if (_data.DsData.Tables[0].Columns[_data.PkMaster.FieldName].DataType != typeof(Guid)) return;
            if (_data.DsData.Tables[0] == null || !_data.DsData.Tables[0].Columns.Contains("HasComment")) return;
            string where = "";
            int i = 0;
            foreach (DataRow dr in _data.DsData.Tables[0].Rows) 
            {
                if (i < 50)
                {
                    where += "'" + dr[_data.PkMaster.FieldName].ToString() + "',";
                    i++;
                }
            }
            if (where == "") return;
            where = where.Substring(0, where.Length - 1);
            string sql = "select mtid from syscomment where MTID in (" + where + ") group by mtid";
            DataTable tb = _data.dbStruct.GetDataTable(sql);
            if (tb == null) return;
            foreach (DataRow dr in _data.DsData.Tables[0].Rows)
            {
                DataRow[] ldr = tb.Select("MTID='" + dr[_data.PkMaster.FieldName].ToString() + "'");
                if (ldr.Length > 0)
                {
                    dr["HasComment"] = 1;
                    dr.EndEdit();
                    dr.AcceptChanges();
                    _data.DataChanged = false;
                }
            }
            timer1.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

           
           
                //try
                //{
                //    if (nhapnhay < 100)
                //    {
                //        e.Appearance.BackColor = Color.Pink;
                //        e.Appearance.BackColor2 = Color.LightGreen;
                //        nhapnhay++;
                //    }
                //    else if (nhapnhay < 200)
                //    {
                //        e.Appearance.BackColor = Color.LightGreen;
                //        e.Appearance.BackColor2 = Color.Pink;
                //        nhapnhay++;
                //    }
                //    else
                //    {
                //        nhapnhay = 0;
                //    }
                //    e.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                //}
                //catch { }
            
        }

        private void barButtonItem5_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            if (_data.tbTask != null)
            {
                foreach (DataRow dr in _data.tbTask.Rows)
                {
                    if (dr["isBegin"].ToString() == "True")
                    {
                        if (_data.DrCurrentMaster["TaskID"] == DBNull.Value)
                        {
                            _data.DrCurrentMaster["TaskID"] = dr["ID"];
                            _data.UpdateData(DataAction.Update);
                            RefreshData();
                        }
                        break;
                    }

                }
            }
        }

        private void simpleButtonView_Click(object sender, EventArgs e)
        {
            referhComment = false;
            if ((_data as DataMasterDetail).DrTableMaster["sysUserID"] != DBNull.Value && (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString() != string.Empty)
            {
                string adminList = (_data as DataMasterDetail).DrTableMaster["sysUserID"].ToString().Trim();
                DataRow drCurrent;
                if (adminList != Config.GetValue("sysUserID").ToString().Trim())
                {
                    
                }
            }
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ActiveFilterString = "";
            gvMain.ApplyColumnsFilter();
            _frmDesigner.formAction = FormAction.View;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
            if (frmMtDtCt.refresh)
            {
                RefreshData();
                frmMtDtCt.refresh = false;
            }
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            referhComment = true;
        }

        private void simpleButtonDanhso_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có tìm kiếm hết chứng từ cần đánh chưa?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                fDanhSoCT f_DanhSoCT = new fDanhSoCT(this._data as DataMasterDetail, this._frmDesigner);
                f_DanhSoCT.ImgList = this.imageList2;
                f_DanhSoCT.ShowDialog();
                RefreshData();
            }
        }

        private void FrmMasterDetail_Load_1(object sender, EventArgs e)
        {

        }
    }
}
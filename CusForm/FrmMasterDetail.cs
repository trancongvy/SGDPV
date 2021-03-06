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
using CusData;
using CDTControl;

namespace CusForm
{
    internal partial class FrmMasterDetail : CDTForm
    {
        private FrmMasterDetailDt frmMtDtCt;
        private GridControl gcDetail = new GridControl();
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();

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
            if (Boolean.Parse(Config.GetValue("Admin").ToString()))
                return;
            string sSelect = _data.DrTable["sSelect"].ToString();
            string sInsert = _data.DrTable["sInsert"].ToString();
            string sUpdate = _data.DrTable["sUpdate"].ToString();
            string sDelete = _data.DrTable["sDelete"].ToString();
            string sPrint = _data.DrTable["sPrint"].ToString();
            if (sSelect != string.Empty && !Boolean.Parse(sSelect))
                lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !Boolean.Parse(sInsert))
                lciNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sUpdate != string.Empty && !Boolean.Parse(sUpdate))
                lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sDelete != string.Empty && !Boolean.Parse(sDelete))
                lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sPrint != string.Empty && !Boolean.Parse(sPrint))
                lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !Boolean.Parse(sInsert))
                lciCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void InitializeLayout()
        {
            this.SetFormCaption();
            if (_data.DsData == null)
                _data.GetData();
            gcMain = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[0], false, DockStyle.Fill);
            gvMain = gcMain.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            this.Controls.Add(gcMain);

            SplitterControl spc = new SplitterControl();
            spc.Dock = DockStyle.Bottom;
            this.Controls.Add(spc);

            gcDetail = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[1], false, DockStyle.Bottom);
            gvDetail = gcDetail.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gvDetail.OptionsView.ShowFooter = false;
            gvDetail.OptionsView.ShowGroupPanel = false;
            this.Controls.Add(gcDetail);
            gcDetail.SendToBack();
            layoutControl2.SendToBack();
            gcDetail.Height = 0;// this.Height / 2;

        }

        private void FrmMasterDetail_Load(object sender, EventArgs e)
        {
            //if (_data.DsData == null)
            //    _data.GetData();
            DisplayData();
        }
        
        private void DisplayData()
        {
            if (_data.DsData == null)
                return;
            this._bindingSource.DataSource = _data.DsData;
            this._bindingSource.CurrentChanged += new EventHandler(bindingSource_CurrentChanged);
            bindingSource_CurrentChanged(_bindingSource, new EventArgs());
            this._bindingSource.DataMember = _data.DsData.Tables[0].TableName;

            this.gcMain.DataSource = _bindingSource;
            gcDetail.DataSource = _bindingSource;
            gcDetail.DataMember = this._data.DrTable["TableName"].ToString();
            gvMain.BestFitColumns();
            gvDetail.BestFitColumns();
        }

        void bindingSource_CurrentChanged(object sender, EventArgs e)
        {
            simpleButtonDelete.Enabled = (_bindingSource.Count > 0);
            simpleButtonEdit.Enabled = (_bindingSource.Count > 0);
            simpleButtonCopy.Enabled = (_bindingSource.Count > 0);
            simpleButtonPreview.Enabled = (_bindingSource.Count > 0);
        }

        private void simpleButtonNew_Click(object sender, EventArgs e)
        {
            if (!_data.KiemtraDemo()) return;
            
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ClearColumnsFilter();
            _frmDesigner.formAction = FormAction.New;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
           
            frmMtDtCt.ShowDialog();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            this.gvMain.OptionsSelection.MultiSelect = false;           
            this.gvMain.OptionsSelection.MultiSelect = true;
            gvMain.OptionsView.ShowGroupPanel = false;
            this.gvMain.SelectRow(_data.DsData.Tables[0].Rows.Count - 1);
        }

        private void simpleButtonEdit_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Edit;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
        }

        private void simpleButtonDelete_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            if (XtraMessageBox.Show("Vui lòng xác nhận xóa dữ liệu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _frmDesigner.formAction = FormAction.Delete;
                this._data.LstDrCurrentDetails.Clear();
                _bindingSource.RemoveCurrent();
                if (!_data.UpdateData(DataAction.Delete))
                {
                    _data.CancelUpdate();
                    DisplayData();
                }
                _data.Reset();
            }
        }

        private void simpleButtonCopy_Click(object sender, EventArgs e)
        {
            if (!_data.KiemtraDemo()) return;
            
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Copy;
            if (frmMtDtCt == null)
                frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
            frmMtDtCt.ShowDialog();
        }

        private void simpleButtonSearch_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            DialogResult dr = XtraMessageBox.Show("Chọn Có để tìm kiếm thông tin trên bảng chính \n" +
                "Chọn Không để tìm kiếm thông tin trên bảng chi tiết \n", "Xac nhan", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                gvMain.ShowFilterEditor(gvMain.Columns[0]);
                if (gvMain.RowFilter != string.Empty)
                {
                    SqlSearching sSearch = new SqlSearching();
                    string sql = sSearch.GenSqlFromGridFilter(gvMain.RowFilter);
                    _data.ConditionMaster = sql;
                    _data.Condition = string.Empty;
                    _data.GetData();
                    _frmDesigner.RefreshDataForLookup();
                    DisplayData();
                    gvMain.ClearColumnsFilter();
                    XtraMessageBox.Show("Kết quả tìm kiếm: " + gvMain.DataRowCount.ToString() + " mục số liệu");
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
        }

        private void simpleButtonPreview_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            if (gvMain.SelectedRowsCount == 0)
                return;
            if (_data.DrTable["Report"].ToString() == string.Empty)
                gcMain.ShowPrintPreview();
            else
            {
                int[] oldIndex = gvMain.GetSelectedRows();
                int[] newIndex = oldIndex;
                if (gvMain.SortedColumns.Count > 0)
                    for (int i = 0; i < oldIndex.Length; i++)
                        newIndex[i] = _data.DsData.Tables[0].Rows.IndexOf(gvMain.GetDataRow(oldIndex[i]));

                BeforePrint bp = new BeforePrint(_data, newIndex);
                bp.ShowDialog();
            }
        }

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
                    if (lciCopy.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonCopy_Click(simpleButtonCopy, new EventArgs());
                    break;
                case Keys.F6:
                    if (lciSearch.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonSearch_Click(simpleButtonSearch, new EventArgs());
                    break;
                case Keys.F7:
                    if (lciPrint.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonPreview_Click(simpleButtonPreview, new EventArgs());
                    break;

                case Keys.Escape:
                    simpleButtonCancel2_Click(simpleButtonCancel2, new EventArgs());
                    break;
            }
        }

 


    }
}
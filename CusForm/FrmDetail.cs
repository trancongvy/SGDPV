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
    internal partial class FrmDetail : CDTForm
    {
        private BindingSource bsMain = new BindingSource();
        private FrmSingleDt frmSingleDt;
        private GridControl gcDetail = new GridControl();
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
        
        public FrmDetail(CDTData data)
        {
            InitializeComponent();
            this._data = data;
            SetRight();
            this._frmDesigner = new FormDesigner(this._data, _bindingSource);
            InitializeLayout();
            this.Load += new EventHandler(FrmDetail_Load);
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
            string fk = _data.DrTableMaster["Pk"].ToString();

            gcMain = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[1], false, DockStyle.Left);
            gvMain = gcMain.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gcMain.Width = this.Width / 3;

            SplitterControl spc = new SplitterControl();
            spc.Dock = DockStyle.Left;

            int pType = Int32.Parse(_data.DrTable["Type"].ToString());
            gcDetail = this._frmDesigner.GenGridControl(this._data.DsStruct.Tables[0], pType == 4, DockStyle.Fill);
            gvDetail = gcDetail.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            gvDetail.Columns[fk].Visible = false;
            
            this.Controls.Add(gcDetail);
            this.Controls.Add(spc);
            this.Controls.Add(gcMain);

            layoutControl1.SendToBack();
            layoutControl2.SendToBack();
            if (_data.DrTableMaster["ParentPk"].ToString() == string.Empty)
            {
                layoutControlItemTreeList.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemTreeList2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            if (pType == 4)
            {
                layoutControl2.Visible = false;
                this.FormClosing += new FormClosingEventHandler(FrmDetail_FormClosing);
            }
            else
                layoutControl1.Visible = false;
        }

        void FrmDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_data.DataChanged)
                if (XtraMessageBox.Show("Số liệu chưa được lưu, bạn có thật sự muốn quay ra?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;

        }

        private void GetRelativeFunction()
        {
            DataTable dtCNLQ = _data.GetRelativeFunction();
            if (dtCNLQ == null || dtCNLQ.Rows.Count == 0)
            {
                layoutControlItemCNLQ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemCNLQ2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                return;
            }
            lookUpEditCNLQ.Properties.DataSource = dtCNLQ;
            lookUpEditCNLQ.Properties.Columns[0].FieldName = Config.GetValue("Language").ToString() == "0" ? "DienGiai" : "DienGiai2";
            lookUpEditCNLQ.Properties.Columns[0].Caption = string.Empty;
            lookUpEditCNLQ.Properties.DisplayMember = Config.GetValue("Language").ToString() == "0" ? "DienGiai" : "DienGiai2";
            lookUpEditCNLQ2.Properties.DataSource = dtCNLQ;
            lookUpEditCNLQ2.Properties.Columns[0].FieldName = Config.GetValue("Language").ToString() == "0" ? "DienGiai" : "DienGiai2";
            lookUpEditCNLQ2.Properties.Columns[0].Caption = string.Empty;
            lookUpEditCNLQ2.Properties.DisplayMember = Config.GetValue("Language").ToString() == "0" ? "DienGiai" : "DienGiai2";
        }

        void FrmDetail_Load(object sender, EventArgs e)
        {
            //if (_data.DsData == null)
            //    _data.GetData();
            DisplayData();
            GetRelativeFunction();
        }

        private void DisplayData()
        {
            bsMain.DataSource = _data.DsData;
            bsMain.DataMember = _data.DsData.Tables[1].TableName;
            this._bindingSource.DataSource = bsMain;
            this._bindingSource.DataMember = this._data.DrTable["TableName"].ToString();
            this._bindingSource.CurrentChanged += new EventHandler(bindingSource_CurrentChanged);
            bindingSource_CurrentChanged(_bindingSource, new EventArgs());

            this.gcMain.DataSource = bsMain;
            gcDetail.DataSource = _bindingSource;
            gvMain.BestFitColumns();
            //gvDetail.BestFitColumns();
        }

        void bindingSource_CurrentChanged(object sender, EventArgs e)
        {
            simpleButtonDelete.Enabled = (_bindingSource.Count > 0);
            simpleButtonEdit.Enabled = (_bindingSource.Count > 0);
            simpleButtonCopy.Enabled = (_bindingSource.Count > 0);
        }

        private void lookUpEditCNLQ_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
            {
                LookUpEdit lue = sender as LookUpEdit;
                if (lue.ItemIndex < 0)
                    return;
                if (gvDetail.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn một đối tượng trên danh sách để xem thông tin chi tiết!");
                    return;
                }
                string pkName = _data.DrTable["Pk"].ToString();
                string pkValue = gvDetail.GetFocusedRowCellValue(pkName).ToString();
                if (pkValue == string.Empty)
                    return;
                DataTable dtTable = lue.Properties.DataSource as DataTable;
                DataRow dr = dtTable.Rows[lue.ItemIndex];

                CDTData data1 = CusForm.FormFactory.Create(DataType.Single, dr);
                data1.Condition = pkName + " = '" + pkValue + "'";
                FrmSingle frm = new FrmSingle(data1);
                frm.ShowDialog();
            }
        }

        private void checkEditTreeView2_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckTreeList();
        }

        private void checkEditTreeView_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckTreeList();
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            this.Close();
        }

        private void simpleButtonPreview_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            if (_data.DrTable["Report"].ToString() == string.Empty)
                gcDetail.ShowPrintPreview();
            else
            {
                BeforePrint bp = new BeforePrint(_data, new int[] { 0 });
                bp.ShowDialog();
            }
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
                    DisplayData();
                    gvDetail.ClearColumnsFilter();

                    XtraMessageBox.Show("Kết quả tìm kiếm: " + gvMain.DataRowCount.ToString() + " mục số liệu");
                }
            }
        }

        private void simpleButtonUpdate_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            gvDetail.RefreshData();
            if (_data.DsData.Tables[0].HasErrors)
            {
                XtraMessageBox.Show("Số liệu chưa hợp lệ, vui lòng kiểm tra lại trước khi lưu!");
                return;
            }
            if (!_data.UpdateData())
                DisplayData();
            else
            {
                XtraMessageBox.Show("Hoàn tất cập nhật bảng số liệu");
            }
        }

        private void simpleButtonNew_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.New;
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
            if (frmSingleDt.DialogResult == DialogResult.Cancel)
                bsMain.DataSource = _data.DsData;
        }

        private void simpleButtonEdit_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Edit;
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
            if (frmSingleDt.DialogResult == DialogResult.Cancel)
                bsMain.DataSource = _data.DsData;
        }

        private void simpleButtonDelete_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            if (XtraMessageBox.Show("Vui lòng xác nhận xóa dữ liệu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _frmDesigner.formAction = FormAction.Delete;
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
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            _frmDesigner.formAction = FormAction.Copy;
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
        }

        private void FrmDetail_KeyDown(object sender, KeyEventArgs e)
        {
            int pType = Int32.Parse(_data.DrTable["Type"].ToString());
            switch (e.KeyCode)
            {
                case Keys.F2:
                    if (pType == 5 && lciNew.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonNew_Click(simpleButtonNew, new EventArgs());
                    break;
                case Keys.F3:
                    if (pType == 5 && lciEdit.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonEdit_Click(simpleButtonEdit, new EventArgs());
                    break;
                case Keys.F4:
                    if (pType == 5 && lciDelete.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonDelete_Click(simpleButtonDelete, new EventArgs());
                    break;
                case Keys.F5:
                    if (pType == 5 && lciCopy.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
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
                    simpleButtonCancel_Click(simpleButtonCancel, new EventArgs());
                    break;
            }
        }

        private void FrmDetail_Load_1(object sender, EventArgs e)
        {

        }
    }
}
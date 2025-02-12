using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using DataFactory;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using CDTLib;
using CDTControl;
using DevControls;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;

namespace FormFactory
{
    public partial class FrmSingle : CDTForm
    {
        FrmSingleDt frmSingleDt;

        public FrmSingle(CDTData data)
        {
            InitializeComponent();
            this._data = data;
            SetRight();
            this._frmDesigner = new FormDesigner(this._data, _bindingSource);
            InitializeLayout();
            this.Load += new EventHandler(FrmSingle_Load);
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
            bool tSelect = false;
            if (sSelect == "0") tSelect = false;
            else if (sSelect == "1"  || sSelect==string.Empty) tSelect = true;
            bool tInsert = false;
            if (sInsert == "0") tInsert = false;
            else if (sInsert == "1" || sInsert == string.Empty) tInsert = true;
            bool tUpdate = false;
            if (sUpdate == "0") tUpdate = false;
            else if (sUpdate == "1" || sUpdate == string.Empty) tUpdate = true;
            bool tDelete = false;
            if (sDelete == "0") tDelete = false;
            else if (sDelete == "1" || sDelete == string.Empty) tDelete = true;

            bool tPrint = false;
            if (sPrint == "0") tPrint = false;
            else if (sPrint == "1" || sPrint == string.Empty) tPrint = true;

            if (sSelect != string.Empty && !tSelect)
                lciSearch.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !tInsert)
                lciNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sUpdate != string.Empty && !tUpdate)
                lciEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sDelete != string.Empty && !tDelete)
                lciDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sPrint != string.Empty && !tPrint)
                lciPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (sInsert != string.Empty && !tInsert)
                lciCopy.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

        }

        private void FrmSingle_Load(object sender, EventArgs e)
        {
            DisplayData();
            GetRelativeFunction();
        }

        private void InitializeLayout()
        {
            this.SetFormCaption();
            if (_data.DsData == null)
                _data.GetData();
            if (this._data == null || this._data.DrTable == null || this._data.DsStruct == null || this._data.DsStruct.Tables.Count == 0) return;
            bool isEdit = this._data.DrTable["Type"].ToString() == "1";
            if (_data.DrTable.Table.Columns.Contains("useBand") && bool.Parse(_data.DrTable["useBand"].ToString()))
            {
                string tablename = _data._tableName;
                gcMain = _frmDesigner.GenBandGridControl(_data.DsStruct.Tables[tablename],_data._dsBand.Tables[tablename], isEdit, DockStyle.Fill);
            }
            else { gcMain = _frmDesigner.GenGridControl(_data.DsStruct.Tables[0], isEdit, DockStyle.Fill); }
            if (gcMain == null) return;
            gcMain.MouseUp += new MouseEventHandler(gcMain_MouseUp);
            if (_data.DrTable.Table.Columns.Contains("useBand") && bool.Parse(_data.DrTable["useBand"].ToString()))
            {
                gbMain = gcMain.ViewCollection[0] as AdvBandedGridView;
            }
            else
            {
                gvMain = gcMain.ViewCollection[0] as DevExpress.XtraGrid.Views.Grid.GridView;
            }
            this.Controls.Add(gcMain);

            if (_data.DrTable["ParentPk"].ToString() == string.Empty)
            {
                layoutControlItemTreeList.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItemTreeList2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            layoutControl1.SendToBack();
            layoutControl2.SendToBack();
            int pType = Int32.Parse(_data.DrTable["Type"].ToString());
            if (pType == 1)
            {
                layoutControl2.Visible = false;
                this.FormClosing += new FormClosingEventHandler(FrmSingle_FormClosing);
            }
            else
                layoutControl1.Visible = false;
        }

        void gcMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.pMInsetToDetail.ShowPopup(new Point(e.X+this.Left+this.Parent.Left, e.Y +this.Top +this.Parent.Top));
            }
        }

        void FrmSingle_FormClosing(object sender, FormClosingEventArgs e)
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

        private void DisplayData()
        {
            if (_data.DsData != null && gcMain!=null)
            {
                _bindingSource.DataSource = _data.DsData.Tables[0];
                _frmDesigner.bindingSource = _bindingSource;
                _bindingSource.CurrentChanged += new EventHandler(bindingSource_CurrentChanged);
                bindingSource_CurrentChanged(_bindingSource, new EventArgs());
                gcMain.DataSource = _bindingSource;
                tlMain.DataSource = _bindingSource;
                //gvMain.BestFitColumns();
                gvMain.OptionsCustomization.AllowColumnResizing = true;
                gvMain.OptionsView.RowAutoHeight = false;

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvMain.Columns)
                {
                    if (col.ColumnType.ToString().ToLower() == "system.byte[]")
                    {
                        ((DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)(col.ColumnEdit)).SizeMode= DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                        ((DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)(col.ColumnEdit)).CustomHeight = 30;
                    }
                }
                
            }
        }

        private object CType(RepositoryItemPictureEdit repPic, System.ComponentModel.ISupportInitialize iSupportInitialize)
        {
            throw new Exception("The method or operation is not implemented.");
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
                if (gvMain.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn một đối tượng trên danh sách để xem thông tin chi tiết!");
                    return;
                }
                string pkName = _data.DrTable["Pk"].ToString();
                string pkValue = gvMain.GetFocusedRowCellValue(pkName).ToString();
                if (pkValue == string.Empty)
                    return;
                DataTable dtTable = lue.Properties.DataSource as DataTable;
                DataRow dr = dtTable.Rows[lue.ItemIndex];

                CDTData data1 = DataFactory.DataFactory.Create(DataType.Single, dr);
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
            ShowDefaultReport(gcMain, false, true);
            //if (_data.DrTable["Report"].ToString() == string.Empty)
            //{
            //    if (gcMain.Visible)
            //        gcMain.ShowPrintPreview();
            //    else
            //        tlMain.ShowPrintPreview();
            //}
            //else
            //{
            //    BeforePrint bp = new BeforePrint(_data, new int[] { 0 });
            //    bp.ShowDialog();
            //}
        }
        private void ShowDefaultReport(IPrintable e, bool isPrint, bool ExportExcel)
        {
            PrintingSystem p = new PrintingSystem();

            p.ShowMarginsWarning = false;
            //p.PreviewFormEx.KeyUp += new KeyEventHandler(PreviewFormEx_KeyUp);
            PrintableComponentLink i = new PrintableComponentLink(p);
            //i.CreateDocument(p);

            i.Component = e;
            i.Landscape = false;
            PaperSize customPaperSize = new PaperSize("CustomSize", gcMain.Width, 842);
            i.PaperKind = PaperKind.Custom;
            int w = 0;
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gvMain.Columns)
            {
                w += col.Width;
            }
            i.CustomPaperSize = new Size(w, 842);
            //i.RtfReportFooter = "Tổng Cộng";

            i.Margins = new System.Drawing.Printing.Margins(25, 25, 25, 25);
            i.CreateReportHeaderArea += new CreateAreaEventHandler(i_CreateReportHeaderArea);


            gvMain.ColumnPanelRowHeight = 30;
            gvMain.OptionsPrint.EnableAppearanceEvenRow = true;
            //gvMain.OptionsPrint.EnableAppearanceOddRow = true;
            gvMain.OptionsPrint.UsePrintStyles = true;
            gvMain.CustomColumnDisplayText += (sender, ev) =>
            {
                if (ev.Column.ColumnType == typeof(bool))
                {
                    if (ev.Value == null) ev.DisplayText = "";
                    else
                        ev.DisplayText = (bool)ev.Value ? "1" : "0";
                }
            };

            i.CreateDocument();
            var options = new XlsxExportOptions()
            {
                
                TextExportMode = TextExportMode.Value,  // Xuất dữ liệu đúng kiểu
                ExportMode = XlsxExportMode.SingleFile
            };

            if (!ExportExcel)
                i.ShowPreview(this.gcMain.LookAndFeel);
            else
            {
                SaveFileDialog f = new SaveFileDialog();
                f.Filter = "Excel 2003|*.XLS| Excel 2007|*.XLSX";
                f.ShowDialog();
                if (f.FileName != null && f.FileName != string.Empty)
                {
                    if (f.FileName.ToUpper().Contains("XLSX"))
                        i.ExportToXlsx(f.FileName, options);
                    else
                        i.ExportToXls(f.FileName);
                }
                if (System.IO.File.Exists(f.FileName))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(f.FileName)
                    {
                        UseShellExecute = true  // Sử dụng shell để mở file với ứng dụng mặc định
                    });
                }
                else
                {
                    MessageBox.Show("File export không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        void i_CreateReportHeaderArea(object sender, CreateAreaEventArgs e)
        {
            string TenCongTy = Config.GetValue("TenCongTy").ToString();
            string DiaChiCty;
            int y = 0;
            try
            {
                DiaChiCty = Config.GetValue("DiaChiCty").ToString();
            }
            catch
            {
                DiaChiCty = Config.GetValue("DiaChi").ToString();
            }
            int imagewidth = 0;
            
            //tên công ty
            e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
            e.Graph.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            e.Graph.DrawString(TenCongTy, Color.Black, new RectangleF(imagewidth + 1, y, e.Graph.ClientPageSize.Width - 150, 20), BorderSide.None);
            e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic); y += 20;
            e.Graph.DrawString(DiaChiCty, Color.Black, new RectangleF(imagewidth + 1, y, e.Graph.ClientPageSize.Width - 150, 20), BorderSide.None);

            string title =this.Text;
            e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
            e.Graph.Font = new Font("Times New Roman", 16, FontStyle.Bold); y += 30;
            e.Graph.DrawString(title, Color.Black, new RectangleF(imagewidth + 1, y, e.Graph.ClientPageSize.Width - 2, 50), BorderSide.None);

            //else if (condition != "") e.Graph.DrawString(condition, Color.Black, new RectangleF(101, y, e.Graph.ClientPageSize.Width - 150, 30), BorderSide.None);

        }
        private void simpleButtonSearch_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            gvMain.ShowFilterEditor(gvMain.Columns[0]);
            if (gvMain.RowFilter != string.Empty)
            {
                SqlSearching sSearch = new SqlSearching();
                string sql = sSearch.GenSqlFromGridFilter(gvMain.RowFilter);
                _data.Condition = sql;
                _data.GetData();
                _frmDesigner.RefreshDataForLookup();
                DisplayData();
                gvMain.ClearColumnsFilter();
                XtraMessageBox.Show("Kết quả tìm kiếm: " + gvMain.DataRowCount.ToString() + " mục số liệu");
            }
        }

        private void simpleButtonUpdate_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            gvMain.RefreshData();
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
            this.gcMain.Refresh();
            //this.gvMain.EndDataUpdate();
            string s = gvMain.ActiveFilterString;
            if(_data.DsData.Tables[0].PrimaryKey.Length>0)
            {
                DataColumn colKey = _data.DsData.Tables[0].PrimaryKey[0];                
                _data.DsData.Tables[0].PrimaryKey = null;
                colKey.AllowDBNull = true;                
            }
            gvMain.ClearColumnsFilter();
            _frmDesigner.formAction = FormAction.New;
            _bindingSource.AddNew();
            _bindingSource.EndEdit();
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);

            frmSingleDt.ShowDialog();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            this.gvMain.BeginUpdate();
            this.gcMain.DataSource = null;
            this.gcMain.DataSource = _bindingSource.DataSource;
            this.gvMain.EndUpdate();
            this.gvMain.RefreshData();
            DisplayData();
            //this.bindingSource_CurrentChanged
            _data.DataChanged = false;
        }

        private void simpleButtonEdit_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ActiveFilterString = "";
            gvMain.ApplyColumnsFilter();
            _frmDesigner.formAction = FormAction.Edit;
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            DisplayData();
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
            string s = gvMain.ActiveFilterString;
            gvMain.ClearColumnsFilter();
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
            this.gvMain.BeginUpdate();
            this.gcMain.DataSource = null;
            this.gcMain.DataSource = _bindingSource.DataSource;
            DisplayData();
            this.gvMain.EndUpdate();
            this.gvMain.RefreshData();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
        }

        private void FrmSingle_KeyDown(object sender, KeyEventArgs e)
        {
            int pType = Int32.Parse(_data.DrTable["Type"].ToString());
            switch (e.KeyCode)
            {
                case Keys.F2:
                    if (pType == 2 && lciNew.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonNew_Click(simpleButtonNew, new EventArgs());
                    break;
                case Keys.F3:
                    if (pType == 2 && lciEdit.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonEdit_Click(simpleButtonEdit, new EventArgs());
                    break;
                case Keys.F4:
                    if (pType == 2 && lciDelete.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        simpleButtonDelete_Click(simpleButtonDelete, new EventArgs());
                    if (pType == 1)
                    {
                        (this.gcMain.MainView as DevExpress.XtraGrid.Views.Grid.GridView).DeleteSelectedRows();
                    }
                    break;
                case Keys.F5:
                    if (pType == 2 && lciCopy.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
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
                case Keys.F8:
                    MergeCode();
                    break;
                case Keys.F9:
                    ChangeCode();
                    break;
                case Keys.F11:
                    PhanQuyenRecord();
                    break;
                case Keys.F12:
                    if (simpleButtonUpdate.Visible)
                    {
                        simpleButtonUpdate_Click(simpleButtonUpdate, new EventArgs());
                    }
                    break;
                case Keys.S:
                    if (ModifierKeys.HasFlag(Keys.Control) && simpleButtonUpdate.Visible)
                    {
                        simpleButtonUpdate_Click(simpleButtonUpdate, new EventArgs());
                    }
                    break;

            }
        }

        private void MergeCode()
        {
            if (this._data.DsData.Tables[0].Columns.Contains("ws") && this._data.DrTable["sysUserID"] != DBNull.Value && this._data.DrTable["sysUserID"].ToString() != Config.GetValue("sysUserID").ToString()) return;
            if (lciEdit.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
            DataRow OldCode = (_bindingSource.Current as DataRowView).Row;
            FrmSgMergeCode f = new FrmSgMergeCode(_data, _frmDesigner);
            f.ShowDialog();
            if (f.re == null) return;
            this.gvMain.BeginUpdate();
            if (OldCode[_data.DrTable["pk"].ToString()].ToString() == f.re[_data.DrTable["pk"].ToString()].ToString())
            {
                MessageBox.Show("2 mã trùng nhau!");
                return;
            }
            DataRow NewCode = f.re;
            if (XtraMessageBox.Show("Có chắc chắn muốn gộp mã từ " + OldCode[_data.DrTable["pk"].ToString()].ToString() + " sang " + NewCode[_data.DrTable["pk"].ToString()].ToString() + " không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                (_data as DataSingle).ChangeCode(OldCode, NewCode);
                Config.NewKeyValue("Operation", "F4 - Xóa");
                int index = _bindingSource.Find(_data.DrTable["pk"].ToString(), OldCode[_data.DrTable["pk"].ToString()]);
                _bindingSource.Position = index;
                _frmDesigner.formAction = FormAction.Delete;
                _bindingSource.RemoveCurrent();
            }
            if (!_data.UpdateData(DataAction.Delete))
            {
                _data.CancelUpdate();
                DisplayData(); 
            }

            _data.Reset();

            this.gcMain.DataSource = null;
            this.gcMain.DataSource = _bindingSource.DataSource;
            DisplayData();
            this.gvMain.EndUpdate();
            this.gvMain.RefreshData();

        }

        private void ChangeCode()
        {
            if (this._data.DsData.Tables[0].Columns.Contains("ws") && this._data.DrTable["sysUserID"] != DBNull.Value && this._data.DrTable["sysUserID"].ToString() != Config.GetValue("sysUserID").ToString()) return;
            if (lciEdit.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
            Config.NewKeyValue("Operation", "F5-Sao chép");
            _frmDesigner.formAction = FormAction.Copy;

            DataRow OldCode = (_bindingSource.Current as DataRowView).Row.Table.NewRow();
            OldCode.ItemArray=(_bindingSource.Current as DataRowView).Row.ItemArray;
                
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            string s = gvMain.ActiveFilterString;
            gvMain.ActiveFilterString = "";
            gvMain.ApplyColumnsFilter();
          
            
            frmSingleDt.ShowDialog();
            this.gvMain.BeginUpdate();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            if (frmSingleDt.DialogResult != DialogResult.Cancel)
            {
                DataRow NewCode = (_bindingSource.Current as DataRowView).Row;

                if (XtraMessageBox.Show("Có chắc chắn muốn chuyển Mã không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    (_data as DataSingle).ChangeCode(OldCode, NewCode);

                    Config.NewKeyValue("Operation", "F4 - Xóa");
                    int index = _bindingSource.Find(_data.DrTable["pk"].ToString(), OldCode[_data.DrTable["pk"].ToString()]);
                    _bindingSource.Position = index;
                    _frmDesigner.formAction = FormAction.Delete;
                    _bindingSource.RemoveCurrent();
                }

            }
            if (!_data.UpdateData(DataAction.Delete))
            {
                _data.CancelUpdate();
                DisplayData();
            }

            _data.Reset();

            this.gcMain.DataSource = null;
            this.gcMain.DataSource = _bindingSource.DataSource;
            DisplayData();
            this.gvMain.EndUpdate();
            this.gvMain.RefreshData();

        }
        private void PhanQuyenRecord()
        {
            if (this._data.DsData.Tables[0].Columns.Contains("ws") && this._data.DrTable["sysUserID"] != DBNull.Value)
            {
                if (this._data.DrTable["sysUserID"].ToString() == Config.GetValue("sysUserID").ToString())
                {
                    FrmPQ frm = new FrmPQ(this._frmDesigner, this._data);
                    frm.ShowDialog();
                }
            }
        }

        private void btSaveGrid_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SysTable stb = new SysTable();
            foreach (CDTGridColumn col in gvMain.Columns)
            {
                if (!col.isExCol)
                    stb.UpdateColWidth(col.MasterRow, col.Width);
                stb.UpdateColIndex(col.MasterRow, col.VisibleIndex);
                stb.UpdateColVisible(col.MasterRow, col.Visible ? 1 : 0);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this._bindingSource.Current == null) return;
            _data.DrCurrentMaster = (this._bindingSource.Current as DataRowView).Row;
            fShowHistorySgle fSHS = new fShowHistorySgle(_data as DataSingle);
            fSHS.ShowDialog();
        }

        private void FrmSingle_Load_1(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("Operation", (sender as SimpleButton).Text);
            string s = gvMain.ActiveFilterString;
            gvMain.ActiveFilterString = "";
            gvMain.ApplyColumnsFilter();
            _frmDesigner.formAction = FormAction.View;
            if (frmSingleDt == null)
                frmSingleDt = new FrmSingleDt(_frmDesigner);
            frmSingleDt.ShowDialog();
            gvMain.ActiveFilterString = s;
            gvMain.ApplyColumnsFilter();
            //DisplayData();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            _data.GetData();
            _frmDesigner.RefreshDataForLookup();
            DisplayData();
            gvMain.ClearColumnsFilter();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraCharts;

using DataFactory;
using CDTLib;
using System.IO;
using CDTControl;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid;
namespace FormFactory
{
    public partial class ReportPreview:DevExpress.XtraEditors.XtraUserControl
    {
        public bool DataChanged = false;
        private DataRow drDefaultReport;
        private  CDTData _data;
        private BindingSource _bindingSource = new BindingSource();
        public FormDesigner _frmDesigner;
        private GridControl gcMain = new GridControl();
        public ReportPreview(CDTData data)
        {
            InitializeComponent();

            _data = data;

            InitializeLayout();
            (_data as DataReport).GetDataForReport();
            gridControlReport.DataSource = (_data as DataReport).DtReportData;
            GetColumnsInfo();
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            else
                gridViewReport.GroupPanelText = "Bảng nhóm: kéo thả một cột vào đây để nhóm số liệu";
            gridViewReport.BestFitColumns();
            if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 1)
            {
                if ((_data as DataReport).DtReportData.Columns.Contains("TTSX"))
                {
                    gridViewReport.SortInfo.Add(gridViewReport.Columns["TTSX"], DevExpress.Data.ColumnSortOrder.Ascending);
                    gridViewReport.Columns["TTSX"].Visible = false;
                    gridViewReport.Columns[0].Visible = false;
                }


            }
            //if ((_data as DataReport).DtReportData.Columns.Contains("SoCTGoc"))
            //{
            //    gridViewReport.OptionsBehavior.Editable = true;
            //}
            drDefaultReport = (_data as DataReport).GetDefaultReport();
            GetFormReport();
        }

        private void GetFormReport()
        {
            DataTable dtData = (_data as DataReport).GetFormReport();
            if (dtData == null || dtData.Rows.Count < 1)
                lcisysFormReport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            else
            {
                lookUpEditsysFormReport.Properties.DataSource = dtData;
                lookUpEditsysFormReport.Properties.DisplayMember = "ReportName";
                lookUpEditsysFormReport.Properties.ValueMember = "sysFormReportID";
                lookUpEditsysFormReport.EditValue = dtData.Rows[0]["sysFormReportID"].ToString();
                if (Config.GetValue("Language").ToString() == "1")
                {
                    lookUpEditsysFormReport.Properties.DisplayMember = "ReportName2";
                    lookUpEditsysFormReport.Properties.Columns[0].Caption = "Report Form";
                    lookUpEditsysFormReport.Properties.Columns[0].FieldName = "ReportName2";
                }
            }
        }

        private void InitializeLayout()
        {
            //this.SetFormCaption();
            if (_data.DrTable["LinkField"].ToString() == string.Empty)
                layoutControlItemDetail.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (_data.DrTable["ChartField1"].ToString() == string.Empty && _data.DrTable["ChartField2"].ToString() == string.Empty)
                layoutControlItemChart.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            int pType = Int32.Parse(_data.DrTable["RpType"].ToString());
            if (pType == 2)
                layoutControlItemFormReport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        private void GetColumnsInfo()
        {
            DataTable dt = (_data as DataReport).GetColumnsInfo();
            DataTable dtc = (_data as DataReport).GetColumnsCaptionInfo();
            foreach (DevExpress.XtraGrid.Columns.GridColumn gc in gridViewReport.Columns)
            {
                DataRow[] drLst = dtc.Select("ColName='" + gc.FieldName.ToUpper() + "'");
                if (drLst.Length > 0) gc.Caption = Config.GetValue("Language").ToString() == "0" ? drLst[0]["Caption"].ToString() : drLst[0]["CaptionE"].ToString();
                bool found = false;
                foreach (DataRow dr in dt.Rows)
                    if (dr["FieldName"].ToString().ToUpper() == gc.FieldName.ToUpper())
                    {
                        //gc.Caption = Config.GetValue("Language").ToString() == "0" ? dr["LabelName"].ToString() : dr["LabelName2"].ToString();
                        if (Boolean.Parse(dr["IsFixCol"].ToString()))
                            gc.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                        int pType = Int32.Parse(dr["Type"].ToString());
                        if (pType == 9)
                        {
                            gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                            gc.DisplayFormat.FormatString = "dd/MM/yyyy";
                        }
                        if (pType == 14)
                        {
                            gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                            gc.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                        }
                        if (pType == 3 || pType == 4 || pType == 6 || pType == 7)
                            gc.Visible = false;
                        if (pType == 8 && dr["EditMask"].ToString() != string.Empty)
                        {
                            string f = dr["EditMask"].ToString();
                            gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                            gc.DisplayFormat.FormatString = f;
                            gc.SummaryItem.Assign(new DevExpress.XtraGrid.GridSummaryItem(DevExpress.Data.SummaryItemType.Sum, gc.FieldName, "{0:" + f + "}"));
                        }
                        else
                        {
                            if (gc.ColumnType == typeof(System.Decimal) || gc.ColumnType == typeof(System.Double))
                            {
                                string f = "### ### ### ##0.##";
                                gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                                gc.DisplayFormat.FormatString = f;
                                gc.SummaryItem.Assign(new DevExpress.XtraGrid.GridSummaryItem(DevExpress.Data.SummaryItemType.Sum, gc.FieldName, "{0:" + f + "}"));
                            }
                        }
                        found = true;
                        break;
                    }
                if (!found)
                {
                    if (gc.ColumnType == typeof(System.DateTime))
                    {
                        gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                        gc.DisplayFormat.FormatString = "dd/MM/yyyy";
                    }
                    if (gc.ColumnType == typeof(System.Decimal) || gc.ColumnType == typeof(System.Double))
                    {
                        string f = "### ### ### ##0.##";
                        gc.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        gc.DisplayFormat.FormatString = f;
                        gc.SummaryItem.Assign(new DevExpress.XtraGrid.GridSummaryItem(DevExpress.Data.SummaryItemType.Sum, gc.FieldName, "{0:" + f + "}"));
                    }
                }
            }
        }

        private void simpleButtonPreview_Click(object sender, EventArgs e)
        {
            // if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
            //   gridViewReport.ActiveFilterString = "InBaoCao = 1";

            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            string reportFile, title;
            DataRow dr = null;
            if (lcisysFormReport.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                DataTable dtFormReport = lookUpEditsysFormReport.Properties.DataSource as DataTable;
                dr = dtFormReport.Rows[lookUpEditsysFormReport.ItemIndex];
                reportFile = checkEditNgoaiTe.Checked ? dr["ReportFile2"].ToString() : dr["ReportFile"].ToString();
                title = checkEditNgoaiTe.Checked ? dr["ReportName2"].ToString() : dr["ReportName"].ToString();
                if (dr["FileName"] != DBNull.Value)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(dr["FileName"] as byte[]);
                    rptTmp = XtraReport.FromStream(ms, true);
                }
            }
            else
            {
                reportFile = checkEditNgoaiTe.Checked ? _data.DrTable["ReportFile2"].ToString() : _data.DrTable["ReportFile"].ToString();
                title = checkEditNgoaiTe.Checked ? _data.DrTable["ReportName2"].ToString() : _data.DrTable["ReportName"].ToString();
            }

            string path = "";
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            else
            {
                if (!System.IO.File.Exists(path))
                    path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            }
            if (rptTmp == null)
            {
                if (System.IO.File.Exists(path))
                {
                    rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
                    //update vào fileName trên database
                    if (dr != null)
                    {
                        System.IO.Stream stream = System.IO.File.OpenRead(path);
                        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        long numBytes = new FileInfo(path).Length;
                        dr["FileName"] = br.ReadBytes((int)numBytes);
                        _data.UpdateReportFile(dr);
                    }
                    rptTmp.DataSource = gridViewReport.DataSource;
                    if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
                        (rptTmp.DataSource as DataView).RowFilter = "InBaoCao = 1";
                    XRControl xrcTitle = rptTmp.FindControl("title", true);
                    if (xrcTitle != null)
                        xrcTitle.Text = title;

                    SetVariables(rptTmp);
                    rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
                    rptTmp.ShowPreviewDialog();
                }
                else
                {
                    ShowDefaultReport(this.gridControlReport, false);
                }
            }
            else
            {
                rptTmp.DataSource = gridViewReport.DataSource;
                if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
                    (rptTmp.DataSource as DataView).RowFilter = "InBaoCao = 1";
                XRControl xrcTitle = rptTmp.FindControl("title", true);
                if (xrcTitle != null)
                    xrcTitle.Text = title;

                SetVariables(rptTmp);
                rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
                rptTmp.ShowPreviewDialog();
            }
            //gridControlReport.ShowPrintPreview();
            if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
                gridViewReport.ActiveFilterString = "";
        }
        private void ShowDefaultReport(IPrintable e, bool isPrint)
        {
            PrintingSystem p = new PrintingSystem();
            //p.ShowMarginsWarning = false;
            p.PreviewFormEx.KeyUp += new KeyEventHandler(PreviewFormEx_KeyUp);
            PrintableComponentLink i = new PrintableComponentLink(p);
            
            p.PageSettings.PageSettings.Landscape = _data.DrTable["isLand"] == DBNull.Value || bool.Parse(_data.DrTable["isLand"].ToString());
            //p.PageSettings.PageSettings.PaperSize = PaperKind.A4;
            
            //i.RtfReportFooter = "Tổng Cộng";

            p.PageSettings.PageSettings.Margins = new System.Drawing.Printing.Margins(25, 25, 25, 25);
            //i.CreateReportHeaderArea += new CreateAreaEventHandler(i_CreateReportHeaderArea);
            //i.CreateReportFooterArea += new CreateAreaEventHandler(i_CreateReportFooterArea);
            gridViewReport.ColumnPanelRowHeight = 30;
            gridViewReport.OptionsPrint.UsePrintStyles = true;
            i.CreateDocument(p);

            i.ShowPreview(this.gridControlReport.LookAndFeel);

        }

        void HeaderPanel_SizeChanged(object sender, EventArgs e)
        {
        }

        void PreviewFormEx_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) (sender as DevExpress.XtraPrinting.Preview.PrintPreviewFormEx).Close();

        }

        //void i_CreateReportFooterArea(object sender, CreateAreaEventArgs e)
        //{
        //    if (drDefaultReport == null) return;
        //    e.Graph.BackColor = Color.Transparent;
        //    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
        //    e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic);

        //    if (drDefaultReport["Fleft"].ToString() != string.Empty)
        //    {
        //        e.Graph.DrawString(drDefaultReport["Fleft"].ToString(), Color.Black, new RectangleF(0, 40, (e.Graph.ClientPageSize.Width) / 3, 30), BorderSide.None);
        //    }
        //    if (drDefaultReport["FMid"].ToString() != string.Empty)
        //    {
        //        e.Graph.DrawString(drDefaultReport["FMid"].ToString(), Color.Black, new RectangleF((e.Graph.ClientPageSize.Width - 100) / 3, 40, (e.Graph.ClientPageSize.Width) / 3, 30), BorderSide.None);
        //    }

        //    if (drDefaultReport["FRight"].ToString() != string.Empty)
        //    {
        //        e.Graph.DrawString(drDefaultReport["FRight"].ToString(), Color.Black, new RectangleF(2 * (e.Graph.ClientPageSize.Width - 100) / 3, 40, (e.Graph.ClientPageSize.Width) / 3, 30), BorderSide.None);
        //    }
        //}

        //void i_CreateReportHeaderArea(object sender, CreateAreaEventArgs e)
        //{
        //    string TenCongTy = Config.GetValue("TenCongTy").ToString();
        //    string DiaChiCty ;
        //    try
        //    {
        //        DiaChiCty = Config.GetValue("DiaChiCty").ToString();
        //    }
        //    catch
        //    {
        //        DiaChiCty = Config.GetValue("DiaChi").ToString();
        //    }
        //    int imagewidth = 0;
        //    if (drDefaultReport != null && drDefaultReport["Logo"].ToString() != string.Empty)
        //    {
        //        imagewidth = 100;
        //        Image im = GetImage(drDefaultReport["Logo"] as byte[]);
        //        e.Graph.DrawImage(im, new RectangleF(0, 0, 100, 100), BorderSide.None, Color.Transparent);
        //    }
        //    //tên công ty
        //    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
        //    e.Graph.Font = new Font("Times New Roman", 12, FontStyle.Bold);
        //    e.Graph.DrawString(TenCongTy, Color.Black, new RectangleF(imagewidth + 1, 0, e.Graph.ClientPageSize.Width - 150, 20), BorderSide.None);
        //    e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic);
        //    e.Graph.DrawString(DiaChiCty, Color.Black, new RectangleF(imagewidth + 1, 20, e.Graph.ClientPageSize.Width - 150, 20), BorderSide.None);
        //    if (drDefaultReport != null && drDefaultReport["Hleft"].ToString() != string.Empty)
        //    {
        //        e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Near);
        //        e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic);
        //        e.Graph.DrawString(drDefaultReport["Hleft"].ToString(), Color.Black, new RectangleF(imagewidth + 1, 40, (e.Graph.ClientPageSize.Width - 100) / 3, 30), BorderSide.None);
        //    }
        //    if (drDefaultReport != null && drDefaultReport["HMid"].ToString() != string.Empty)
        //    {
        //        e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
        //        e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic);
        //        e.Graph.DrawString(drDefaultReport["HMid"].ToString(), Color.Black, new RectangleF(imagewidth + 1 + (e.Graph.ClientPageSize.Width - 100) / 3, 40, (e.Graph.ClientPageSize.Width - 100) / 3, 30), BorderSide.None);
        //    }
        //    if (drDefaultReport != null && drDefaultReport["HR"].ToString() != string.Empty)
        //    {
        //        e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Far);
        //        e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Italic);
        //        e.Graph.DrawString(drDefaultReport["HR"].ToString(), Color.Black, new RectangleF(imagewidth + 1 + 2 * (e.Graph.ClientPageSize.Width - 100) / 3, 40, (e.Graph.ClientPageSize.Width - 150) / 3, 30), BorderSide.None);
        //    }
        //    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
        //    e.Graph.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        //    e.Graph.DrawString(_data.DrTable["ReportName"].ToString().ToUpper(), Color.Black, new RectangleF(101, 60, e.Graph.ClientPageSize.Width - 150, 30), BorderSide.None);
        //    //tạo dữ liệu theo điều kiện
        //    string tungay = "";
        //    string denngay = "";
        //    string condition = "";


        //    foreach (DataRow dr in _data.DsStruct.Tables[0].Rows)
        //    {
        //        if (dr["FieldName"].ToString().ToUpper() == "NGAYCT" && bool.Parse(dr["IsBetween"].ToString()))
        //            tungay = "Từ ngày: " + DateTime.Parse((_data as DataReport).reConfig.GetValue("@" + dr["FieldName"].ToString() + "1").ToString()).ToString("dd/MM/yyyy") + "   đến ngày: " + DateTime.Parse((_data as DataReport).reConfig.GetValue("@" + dr["FieldName"].ToString() + "2").ToString()).ToString("dd/MM/yyyy");
        //        else if ((_data as DataReport).reConfig.GetValue("@" + dr["FieldName"].ToString()).ToString() != "")
        //        {
        //            condition += "; " + dr["LabelName"].ToString() + ": " + (_data as DataReport).reConfig.GetValue("@" + dr["FieldName"].ToString());
        //        }
        //    }
        //    if (condition.Length > 1) condition = condition.Substring(1, condition.Length - 1);
        //    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
        //    e.Graph.Font = new Font("Times New Roman", 10, FontStyle.Regular);
        //    if (tungay != "")
        //    {
        //        e.Graph.DrawString(tungay + denngay, Color.Black, new RectangleF(101, 90, e.Graph.ClientPageSize.Width - 150, 30), BorderSide.None);
        //        if (condition != "") e.Graph.DrawString(condition, Color.Black, new RectangleF(101, 110, e.Graph.ClientPageSize.Width - 150, 30), BorderSide.None);
        //    }
        //    else if (condition != "") e.Graph.DrawString(condition, Color.Black, new RectangleF(101, 90, e.Graph.ClientPageSize.Width - 150, 30), BorderSide.None);

        //}
        private void SetVariables(DevExpress.XtraReports.UI.XtraReport rptTmp)
        {
            foreach (DictionaryEntry de in Config.Variables)
            {
                string key = de.Key.ToString();
                if (key.Contains("@"))
                    key = key.Remove(0, 1);
                XRControl xrc = rptTmp.FindControl(key, true);
                if (xrc != null)
                {
                    string value = de.Value.ToString();
                    DateTime r;
                    if (DateTime.TryParse(value, out r))
                        value = DateTime.Parse(value).ToString("dd/MM/yyyy");//.ToShortDateString();
                    xrc.Text = value;
                    xrc = null;
                }
            }
            foreach (DictionaryEntry de in (_data as DataReport).reConfig.Variables)
            {
                string key = de.Key.ToString();
                if (key.Contains("@"))
                    key = key.Remove(0, 1);
                XRControl xrc = rptTmp.FindControl(key, true);
                if (xrc != null)
                {
                    string value = de.Value.ToString();
                    DateTime r;
                    if (DateTime.TryParse(value, out r))
                        value = DateTime.Parse(value).ToString("dd/MM/yyyy");//.ToShortDateString();
                    xrc.Text = value;
                    xrc = null;
                }
            }
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void simpleButtonDetail_Click(object sender, EventArgs e)
        {
            string linkField = _data.DrTable["LinkField"].ToString().ToUpper();
            if (linkField == string.Empty)
                return;
            if (gridViewReport.SelectedRowsCount == 0)
                return;
            string linkItem = gridViewReport.GetFocusedRowCellValue(linkField).ToString().Trim();
            DataRow dr = gridViewReport.GetDataRow(gridViewReport.FocusedRowHandle);
            foreach (DataColumn col in dr.Table.Columns)
            {
                Config.NewKeyValue("@" + col.ColumnName, dr[col]);
            }
            if (linkItem == string.Empty)
                return;

            string linkString = linkField + " = '" + linkItem + "'";

            DataReport data = (_data as DataReport).GetDataForDetailReport(linkField, linkItem) as DataReport;
            if (data != null)
            {

                data.PsString = (_data as DataReport).PsString;
                ReportPreview rptPre = new ReportPreview(data);
                data.SaveVariables();
               // rptPre.MdiParent = this.MdiParent;
                rptPre.Show();
                rptPre.Disposed += new EventHandler(rptPre_Disposed);
                //load lại dữ liệu trong trường hợp sửa lại voucher
                //if (rptPre.DataChanged)
                //{
                //    (_data as DataReport).GetDataForReport();
                //    gridControlReport.DataSource = (_data as DataReport).DtReportData;
                //    this.DataChanged = true;
                //}
            }
            else
            {
                string maCT;//= gridViewReport.GetFocusedRowCellValue("MACT").ToString();
                maCT = dr["MaCT"].ToString();
                CDTData data1 = (_data as DataReport).GetDataForVoucher(maCT, linkItem);
                _bindingSource = new BindingSource();
                _bindingSource.DataSource = data1.DsData;
                this._bindingSource.DataMember = data1.DsData.Tables[0].TableName;
                this._frmDesigner = new FormDesigner(data1, _bindingSource);
                _frmDesigner.formAction = FormAction.Edit;
                FrmMasterDetailDt frmMtDtCt = new FrmMasterDetailDt(_frmDesigner);
                if (frmMtDtCt.ShowDialog() == DialogResult.OK)
                {
                    (_data as DataReport).GetDataForReport();
                    gridControlReport.DataSource = (_data as DataReport).DtReportData;
                    DataChanged = true;
                }
            }
        }
        //load lại dữ liệu trong trường hợp sửa lại voucher
        void rptPre_Disposed(object sender, EventArgs e)
        {
            try
            {
                ReportPreview rptPre = (sender as ReportPreview);
                if (rptPre.DataChanged)
                {
                    (_data as DataReport).GetDataForReport();
                    gridControlReport.DataSource = (_data as DataReport).DtReportData;
                    this.DataChanged = true;
                }
            }
            catch
            {
            }
        }

        private void ReportPreview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F6:
                    simpleButtonDesignReport_Click(simpleButtonDesignReport, e);
                    break;
                case Keys.F8:
                    ChartPreview();
                    break;
                case Keys.F7:
                    simpleButtonPreview_Click(simpleButtonPreview, e);
                    break;
                case Keys.F5:
                    simpleButtonDetail_Click(simpleButtonDetail, e);
                    break;
                case Keys.Escape:
                    simpleButtonCancel_Click(simpleButtonCancel, e);
                    break;
            }
        }

        private void simpleButtonDesignReport_Click(object sender, EventArgs e)
        {
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            string reportFile;
            if (lcisysFormReport.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                DataTable dtFormReport = lookUpEditsysFormReport.Properties.DataSource as DataTable;
                DataRow dr = dtFormReport.Rows[lookUpEditsysFormReport.ItemIndex];
                reportFile = checkEditNgoaiTe.Checked ? dr["ReportFile2"].ToString() : dr["ReportFile"].ToString();
            }
            else
                reportFile = checkEditNgoaiTe.Checked ? _data.DrTable["ReportFile2"].ToString() : _data.DrTable["ReportFile"].ToString();
            string path = "";
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            else
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            string pathTmp = "";
            if (Config.GetValue("DuongDanBaoCao") != null)
                pathTmp = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            else
                pathTmp = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + "\\template.repx";

            //string pathTmp = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\template.repx";
            if (System.IO.File.Exists(path))
                rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
            else if (System.IO.File.Exists(pathTmp))
                rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(pathTmp, true);
            else
                rptTmp = new DevExpress.XtraReports.UI.XtraReport();
            if (rptTmp != null)
            {
                rptTmp.DataSource = gridViewReport.DataSource;
                if (Config.GetValue("Language").ToString() == "1")
                    Translate(rptTmp);
                XRDesignFormEx designForm = new XRDesignFormEx();
                designForm.OpenReport(rptTmp);
                if (System.IO.File.Exists(path))
                    designForm.FileName = path;
                designForm.KeyPreview = true;
                designForm.KeyDown += new KeyEventHandler(designForm_KeyDown);
                designForm.Show();
            }
        }

        void designForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.X)
                (sender as XRDesignFormEx).Close();
        }
        private void ChartPreview()
        {
            string chartField1 = _data.DrTable["ChartField1"].ToString().ToUpper();
            string chartField2 = _data.DrTable["ChartField2"].ToString().ToUpper();
            string chartField3 = _data.DrTable["ChartField3"].ToString().ToUpper();
            if (chartField1 == string.Empty && chartField2 == string.Empty)
                return;
            DevExpress.XtraCharts.ChartControl chartMain = new ChartControl();
            chartMain.DataSource = gridViewReport.DataSource;
            chartMain.Series.Clear();
            chartMain.SeriesDataMember = chartField1;
            if (chartField3 == string.Empty)
                chartMain.SeriesTemplate.ArgumentDataMember = chartField1;
            else
                chartMain.SeriesTemplate.ArgumentDataMember = chartField3;
            chartMain.SeriesTemplate.ValueDataMembers.AddRange(new string[] { chartField2 });
            chartMain.SeriesTemplate.View = new StackedBarSeriesView();

            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = this.Text;
            chartMain.Titles.Add(chartTitle);
            chartMain.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch;
            chartMain.ShowPrintPreview();
        }

        private void simpleButtonChart_Click(object sender, EventArgs e)
        {
            ChartPreview();
        }

        private void simpleButtonFormReport_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("sysReportID", _data.DrTable["sysReportID"]);
            CDTData data = (_data as DataReport).GetFormForReport();
            CDTForm frm = FormFactory.Create(FormType.Single, data);
            frm.ShowDialog();
        }

        private void Translate(XRControl xrc)
        {
            if (xrc.HasChildren)
                foreach (XRControl xrChild in xrc.Controls)
                    Translate(xrChild);
            xrc.Text = UIDictionary.Translate(xrc.Text);
        }

        private void simplePrint_Click(object sender, EventArgs e)
        {
            if (Config.GetValue("isDemo").ToString() == "1") return;
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            string reportFile, title;
            if (lcisysFormReport.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                DataTable dtFormReport = lookUpEditsysFormReport.Properties.DataSource as DataTable;
                DataRow dr = dtFormReport.Rows[lookUpEditsysFormReport.ItemIndex];
                reportFile = checkEditNgoaiTe.Checked ? dr["ReportFile2"].ToString() : dr["ReportFile"].ToString();
                title = checkEditNgoaiTe.Checked ? dr["ReportName2"].ToString() : dr["ReportName"].ToString();
                if (dr["FileName"] != DBNull.Value)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(dr["FileName"] as byte[]);
                    rptTmp = XtraReport.FromStream(ms, true);
                }
            }
            else
            {
                reportFile = checkEditNgoaiTe.Checked ? _data.DrTable["ReportFile2"].ToString() : _data.DrTable["ReportFile"].ToString();
                title = checkEditNgoaiTe.Checked ? _data.DrTable["ReportName2"].ToString() : _data.DrTable["ReportName"].ToString();
            }

            string path = "";
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            else
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
            if (rptTmp != null)
            {
                //rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
                //rptTmp.DataSource = gridViewReport.DataSource;
                //if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
                //    (rptTmp.DataSource as DataView).RowFilter = "InBaoCao = 1";
                //XRControl xrcTitle = rptTmp.FindControl("title", true);
                //if (xrcTitle != null)
                //    xrcTitle.Text = title;

                //SetVariables(rptTmp);
                //rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
                ////rptTmp.Print("\\\\vytc\\Canon LBP2900");
                //rptTmp.Print(Config.GetValue("PrinterName").ToString());

                rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
                CDTControl.Printing re = new CDTControl.Printing(gridViewReport.DataSource, rptTmp);
                re.Print();
            }
            else
            {
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + reportFile + ".repx";
                if (System.IO.File.Exists(path))
                {
                    CDTControl.Printing re = new CDTControl.Printing(gridViewReport.DataSource, path);
                    re.Print();
                }
                else
                {
                    if (Config.Variables.Contains("PrinterName"))
                    {
                        string pName = Config.GetValue("PrinterName").ToString();
                        rptTmp.Print(pName);
                    }
                    else
                    {
                        rptTmp.Print();
                    }
                }
            }
            if (Int32.Parse(_data.DrTable["RpType"].ToString()) == 2)
                gridViewReport.ActiveFilterString = "";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "Excel 2003|*.XLS| Excel 2007|*.XLSX";
            f.ShowDialog();
            if (f.FileName != null && f.FileName !="")
            {
                if (f.FileName.ToUpper().Contains("XLSX"))
                    gridControlReport.ExportToXlsx(f.FileName);
                else
                    gridControlReport.ExportToXls(f.FileName);
            }
        }

        private Image GetImage(byte[] b)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(b);
            if (ms == null)
                return null;
            Image im = Image.FromStream(ms);
            return (im);
        }
    }
}

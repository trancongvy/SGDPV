using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DataFactory;
using CDTLib;
using CDTControl;
using System.IO;
namespace FormFactory
{
    public partial class BeforePrint : DevExpress.XtraEditors.XtraForm
    {
        DataRow drMau = null;
        string _reportFile = string.Empty;
        string _title = string.Empty;
        string _Script=string.Empty;
        int[] _arrIndex;
        int record = 0;
        CDTData _data;
        DataTable tbMau=null;
        public BeforePrint(CDTData data, int[] arrIndex)
        {
            InitializeComponent();
            _data = data;
            _reportFile = _data.DrTable["Report"].ToString();
            _title = data.dataType == CDTControl.DataType.MasterDetail ? _data.DrTableMaster["DienGiai"].ToString(): _data.DrTable["DienGiai"].ToString();
            _arrIndex = arrIndex;
            textEditTitle.Text = _title;
            textEditSoCTGoc.Text = "0";
            textEditSoLien.Text = "1";
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            if (_data.dataType == CDTControl.DataType.MasterDetail)
            {
                tbMau = (_data as DataFactory.DataMasterDetail).GetReportFile(_data.DrTable["sysTableID"].ToString());
            }
            


            if (tbMau == null||tbMau.Rows.Count==0)
            {
                layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                DataTable tbMau1 = tbMau.Clone();
                //Lọc lại tbMau theo điều kiện
                if (tbMau.Columns.Contains("ShowCondition"))
                {
                    foreach (DataRow drM in tbMau.Rows)
                    {
                        if (drM["ShowCondition"] == DBNull.Value || drM["ShowCondition"].ToString() == string.Empty)
                        {
                            DataRow dr = tbMau1.NewRow();
                            dr.ItemArray = drM.ItemArray;
                            tbMau1.Rows.Add(dr);
                        }
                        else
                        {
                            string Cond = drM["ShowCondition"].ToString();
                            bool ok = true;
                            foreach (int idx in _arrIndex)
                            {
                                DataRow MTdr = _data.DsData.Tables[0].Rows[idx];
                                string Cond1 = _data.PkMaster.FieldName + "=" + _data.quote + MTdr[_data.PkMaster.FieldName] + _data.quote + "and (" + Cond + ")";
                                DataRow[] lDr = _data.DsData.Tables[0].Select(Cond1);
                                if (lDr.Length == 0) ok = false;
                                else ok = ok && true;
                            }
                            if (ok)
                            {
                                DataRow dr = tbMau1.NewRow();
                                dr.ItemArray = drM.ItemArray;
                                tbMau1.Rows.Add(dr);
                            }
                        }
                    }
                    tbMau = tbMau1;
                }
                gridLookUpEdit1.Properties.DataSource = tbMau;
                drMau = tbMau.Rows[0];
                gridLookUpEdit1.EditValue = tbMau.Rows[0]["RFile"].ToString();
                layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                 
            }
            if (!Boolean.Parse(Config.GetValue("Admin").ToString()))
            {
                this.simpleButtonSuaMau.Visible = false;
            }
            if (Config.GetValue("Language").ToString() == "1")
                DevLocalizer.Translate(this);
            
            if (bool.Parse(Config.GetValue("Admin").ToString())) layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            else layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
       
        private void simpleButtonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButtonChapNhan_Click(object sender, EventArgs e)
        {
            
                PrintOrPreview(1);
            
        }

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
                    try
                    {
                        if (value.Contains("/"))
                            xrc.Text = DateTime.Parse(value).ToShortDateString();
                        else
                            xrc.Text = value;
                    }
                    catch
                    {
                        xrc.Text = value;
                    }
                    xrc = null;
                }
            }
        }
        private void SetVariables(DevExpress.XtraReports.UI.XtraReport rptTmp, int lien)
        {
            string key = "Lien" + lien.ToString();
            XRControl xrc = rptTmp.FindControl("lien", true);
            if (xrc != null)
            {
                try
                {
                    string value = Config.GetValue(key).ToString();
                    xrc.Text = value;
                }
                catch
                {
                    xrc.Text = "";
                }

            }
        }       
        private void simpleButtonSuaMau_Click(object sender, EventArgs e)
        {
            if (!bool.Parse(Config.GetValue("Admin").ToString())) return;

                int index = _arrIndex[0];
            DataTable dtReport = new DataTable();
            try
            {
                if (_Script == string.Empty)
                    dtReport = _data.GetDataForPrint(index);
                else
                {
                    dtReport = _data.GetDataForPrint(index, _Script);
                }
            }
            catch { }
            if (dtReport == null)
                return;
                dtReport = AddRecordToData(dtReport);
            
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            string path="";
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
            else
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
            if (drMau == null || (drMau != null && drMau["FileName"] == DBNull.Value))
            {
               
                string pathTmp;
                if (Config.GetValue("DuongDanBaoCao") != null)
                    pathTmp = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                else
                    pathTmp = Application.StartupPath + "\\" + Config.GetValue("DbName").ToString() + "\\Reports\\template.repx";
                if (System.IO.File.Exists(path))
                    rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
                else if (System.IO.File.Exists(pathTmp))
                    rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(pathTmp, true);
                else
                    rptTmp = new DevExpress.XtraReports.UI.XtraReport();
            }
            else
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(drMau["FileName"] as byte[]);
                rptTmp = XtraReport.FromStream(ms, true);

            }
            if (rptTmp != null)
            {
                rptTmp.DataSource = dtReport;
                if (drMau != null) rptTmp.Tag = drMau;
                XRDesignFormEx designForm = new XRDesignFormEx();
                designForm.OpenReport(rptTmp);
                designForm.Tag = drMau;
                if (System.IO.File.Exists(path))
                    designForm.FileName = path;
                designForm.KeyPreview = true;
                designForm.KeyDown += new KeyEventHandler(designForm_KeyDown);
                designForm.FormClosed += designForm_FormClosed;
                designForm.Show();
            }
        }

        void designForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if (MessageBox.Show("Save and Upload report template?", "Question", MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string path = "";
                if (Config.GetValue("DuongDanBaoCao") != null)
                    path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                else
                    path = Application.StartupPath + "\\Reports\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                try
                {
                    (sender as XRDesignFormEx).SaveReport(path);
                }
                catch
                {
                    path = Application.StartupPath + "\\Reports\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                    (sender as XRDesignFormEx).SaveReport(path);
                }
                //DevExpress.XtraReports.UI.XtraReport rptTmp = (sender as XRDesignFormEx).Tag as XtraReport;

                DataRow dr = (sender as XRDesignFormEx).Tag as DataRow;
                if (dr== null) dr = drMau;
                if (dr == null)
                {
                    System.IO.Stream stream = System.IO.File.OpenRead(path);
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    long numBytes = new FileInfo(path).Length;
                    dr = tbMau.NewRow();
                    dr["systableID"] = _data.DrTable["sysTableID"];
                    dr["RDes"] = _data.DrTable["DienGiai"];
                    dr["RFile"] = _reportFile;
                    dr["RecordCount"] = 0;
                    dr["FileName"] = br.ReadBytes((int)numBytes);
                    _data.InsertPrintFile(dr);
                    tbMau.Rows.Add(dr);
                    gridLookUpEdit1.Properties.DataSource = tbMau;
                    gridLookUpEdit1.EditValue = dr["RFile"].ToString();
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    System.IO.Stream stream = System.IO.File.OpenRead(path);
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    long numBytes = new FileInfo(path).Length;
                    dr["FileName"] = br.ReadBytes((int)numBytes);
                    _data.updatePrintFile(dr);
                }
            }
            catch { }

        }

        private DataTable AddRecordToData(DataTable dtReport)
        {
            if (dtReport == null) return null;
            if(dtReport.Rows.Count==0) return dtReport;
            foreach (DataColumn col in dtReport.Columns)
            {
                if (col.ColumnName.Contains("m_"))
                {
                    col.DefaultValue = dtReport.Rows[0][col];
                }
                if (col.ColumnName.Contains("d_stt"))
                {
                    col.DefaultValue =int.MaxValue;
                }
            }
            if (record > 0)
            {
                int thieu=  dtReport.Rows.Count- dtReport.Rows.Count /record*record;
                for (int i = record; i>thieu; i--)
                {
                    DataRow tmpRow = dtReport.NewRow();
                    dtReport.Rows.Add(tmpRow);
                }
            }
            return dtReport;
        }

        void designForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.X)
                (sender as XRDesignFormEx).Close();
        }

        private void BeforePrint_Load(object sender, EventArgs e)
        {

            simpleButtonIn.Focus();
        }

        private void PrintOrPreview(int isPrint)
        {
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            //gridLookUpEdit1_EditValueChanged(gridLookUpEdit1, new EventArgs());
            string path = "";
            
            if (tbMau ==null || tbMau.Rows.Count == 0)// Không có dòng trong bảng dữ liệu file mẫu
            {
                if (_reportFile == string.Empty)
                {
                    //Thông báo không có tìm thấy tên file mẫu, yêu cầu thêm vào biểu mẫu
                    MessageBox.Show(Config.GetValue("Language").ToString() == "0" ? "Không tìm thấy file mẫu" : "Don't find template file");
                }
                else
                {
                    if (Config.GetValue("DuongDanBaoCao") != null)
                        path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                    else
                    {
                        if (!System.IO.File.Exists(path))
                            path = Application.StartupPath + "\\Reports\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                    }
                    if (System.IO.File.Exists(path)) // tồn tại file mẫu rồi
                    {
                        rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
                        System.IO.Stream stream = System.IO.File.OpenRead(path);
                        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        long numBytes = new FileInfo(path).Length;
                        drMau = tbMau.NewRow();
                        drMau["systableID"] = _data.DrTable["sysTableID"];
                        drMau["RDes"] = _data.DrTable["DienGiai"];
                        drMau["RFile"] = _reportFile;
                        drMau["RecordCount"] = 0;
                        drMau["FileName"] = br.ReadBytes((int)numBytes);
                        _data.InsertPrintFile(drMau);
                        tbMau.Rows.Add(drMau);
                        gridLookUpEdit1.Properties.DataSource = tbMau;
                        gridLookUpEdit1.EditValue = drMau["RFile"].ToString();
                        layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        // Thêm 1 dòng vào bảng sysReportFile
                        //hiện Showreview
                        for (int i = 0; i < _arrIndex.Length; i++)
                        {
                            int index = _arrIndex[i];
                            if (index == -1) continue;
                            for (int j = 1; j <= Int32.Parse(textEditSoLien.Text); j++)
                            {
                                this.PrintPreview(rptTmp, string.Empty, index, isPrint, j);
                                
                            }
                            if (!simpleButtonChapNhan.Enabled)
                            {
                                (_data as DataMasterDetail).UpdateLanIn(index);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Config.GetValue("Language").ToString() == "0" ? "Không tìm thấy file mẫu " + _reportFile : "Don't find template file " + _reportFile);
                    }

                }
            }
            else //Có dòng file mẫu in
            {
                //Bảng ko chứa cột Printindex
                if (!_data.DsData.Tables[0].Columns.Contains("PrintIndex") || !ChkAutochoose.Checked)
                {
                    if (drMau["FileName"] == DBNull.Value)
                    {
                        _reportFile = drMau["RFile"].ToString();
                        path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                        if (!System.IO.File.Exists(path))
                            path = Application.StartupPath + "\\Reports\\" + Config.GetValue("DbName").ToString() + "\\" + _reportFile + ".repx";
                        if (System.IO.File.Exists(path)) // tồn tại file mẫu rồi
                        {
                            rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
                            System.IO.Stream stream = System.IO.File.OpenRead(path);
                            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                            BinaryReader br = new BinaryReader(fs);
                            long numBytes = new FileInfo(path).Length;
                            drMau["FileName"] = br.ReadBytes((int)numBytes);
                            _data.updatePrintFile(drMau);
                            //drMau["FileName"]=stream.
                            //Upload file biểu mẫu vào

                            //hiện Showreview
                            for (int i = 0; i < _arrIndex.Length; i++)
                            {
                                int index = _arrIndex[i];
                                if (index == -1) continue;
                                for (int j = 1; j <= Int32.Parse(textEditSoLien.Text); j++)
                                {
                                    this.PrintPreview(rptTmp, drMau["Script"].ToString(), index, isPrint, j);

                                }
                                if (!simpleButtonChapNhan.Enabled)
                                {
                                    (_data as DataMasterDetail).UpdateLanIn(index);
                                }
                                
                            }
                        }
                    }
                    else
                    {

                        System.IO.MemoryStream ms = new System.IO.MemoryStream(drMau["FileName"] as byte[]);

                        for (int i = 0; i < _arrIndex.Length; i++)
                        {
                            int index = _arrIndex[i];
                            if (index == -1) continue;
                            rptTmp = XtraReport.FromStream(ms, true);
                            for (int j = 1; j <= Int32.Parse(textEditSoLien.Text); j++)
                            {
                                this.PrintPreview(rptTmp, drMau["Script"].ToString(), index, isPrint, j);

                            }
                            if (!simpleButtonChapNhan.Enabled)
                            {
                                (_data as DataMasterDetail).UpdateLanIn(index);
                            }
                            
                        }
                    }
                    // if(drMau["Script"] != DBNull.Value) 
                }
                else if (_data.DsData.Tables[0].Columns.Contains("PrintIndex") && ChkAutochoose.Checked)// In lan luot theo mau
                {
                    for (int i = 0; i < _arrIndex.Length; i++)
                    {
                        try
                        {
                            int index = _arrIndex[i];
                            if (index == -1) continue;
                            int printIndex = int.Parse(_data.DsData.Tables[0].Rows[index]["PrintIndex"].ToString());
                            drMau = tbMau.Rows[printIndex];
                            _reportFile = drMau["RFile"].ToString();
                            _title = drMau["RDes"].ToString();
                            _Script = drMau["SCript"].ToString();
                            System.IO.MemoryStream ms = new System.IO.MemoryStream(drMau["FileName"] as byte[]);
                            rptTmp = XtraReport.FromStream(ms, true);
                            
                            for (int j = 1; j <= Int32.Parse(textEditSoLien.Text); j++)
                            {
                                this.PrintPreview(rptTmp, _Script, index, isPrint, j);

                            }
                            if (!simpleButtonChapNhan.Enabled)
                            {
                                (_data as DataMasterDetail).UpdateLanIn(index);
                            }
                        }
                        catch { }
                    }
                }

            }




        }
        ///   <param name="rptTmp">DevReport file.</param>
        ///   <param name="_Script">Script get data for report.</param>
        ///   <param name="index">Index Row which get data</param>
        ///   <param name="isPrint">Print or Preview</param>
        ///   <param name="j">Liên Parameter</param>
        private void PrintPreview(DevExpress.XtraReports.UI.XtraReport rptTmp, string _Script, int index, int isPrint, int j)
        {
            DataTable dtReport = new DataTable();
            try
            {
                if (_Script == string.Empty)
                {
                    dtReport = _data.GetDataForPrint(index);
                    DataMasterDetail dta = (_data as DataMasterDetail);
                    if (dta != null) richTextBox1.Text = dta.PrintSQL;
                }
                else
                {
                    dtReport = _data.GetDataForPrint(index, _Script);
                    DataMasterDetail dta = (_data as DataMasterDetail);
                    if (dta != null) richTextBox1.Text = dta.PrintSQL;
                }
            }
            catch { }
            dtReport = AddRecordToData(dtReport);
            DevExpress.XtraReports.UI.XRControl xrcTitle = rptTmp.FindControl("title", true);
            if (xrcTitle != null)
                xrcTitle.Text = textEditTitle.Text.ToUpper();
            DevExpress.XtraReports.UI.XRControl xrcSoCTGoc = rptTmp.FindControl("SoCTGoc", true);
            if (xrcSoCTGoc != null)
                xrcSoCTGoc.Text = textEditSoCTGoc.Text;
            //SetVariables(rptTmp);


            if (dtReport == null)
                return;
            rptTmp.DataSource = dtReport;
            rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
            SetVariables(rptTmp);
            if (Config.GetValue("Language").ToString() == "1")
                Translate(rptTmp);
            SetVariables(rptTmp, j);
            rptTmp.ShowPrintMarginsWarning = false;
            if (isPrint == 0)
            {
                rptTmp.Print();
                //Update LanIn
                
            }
            else if (isPrint == 1)
            {
                rptTmp.ShowPreview();
            }
            else if (isPrint == 2)
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.Filter = "(*.xls)|*.xls";
                fd.AddExtension = true;
                fd.ShowDialog();                
                if (fd.FileName != string.Empty)
                    rptTmp.ExportToXls(fd.FileName);
            }
        }
        public void simpleButtonIn_Click(object sender, EventArgs e)
        {
            
                PrintOrPreview(0);
               // PrintOrPreview(0,j);
            
            
            
            this.Dispose();
        }

        private void BeforePrint_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.F7:
                    simpleButtonIn_Click(simpleButtonIn, new EventArgs());
                    break;
            }
        }

        private void Translate(XRControl xrc)
        {
            if (xrc.HasChildren)
                foreach (XRControl xrChild in xrc.Controls)
                    Translate(xrChild);
            xrc.Text = UIDictionary.Translate(xrc.Text);
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            int i = gridLookUpEdit1.Properties.GetIndexByKeyValue(gridLookUpEdit1.EditValue);
            if (i == -1) return;

            DataRow[] ldr = (gridLookUpEdit1.Properties.DataSource as System.Data.DataTable).Select("RFile='" + gridLookUpEdit1.EditValue.ToString() + "'");
            if (ldr.Length == 0) return;
            DataRow dr = ldr[0];
            if (dr != null) drMau = dr;
            if (dr !=null && dr["RecordCount"].ToString() != string.Empty)
                record = int.Parse(dr["RecordCount"].ToString());
            
            _reportFile = gridLookUpEdit1.EditValue.ToString();// (gridLookUpEdit1.EditValue as DataRowView)[gridLookUpEdit1.Properties.ValueMember].ToString();
            _title = gridLookUpEdit1.Text;
            try
            {
                _Script = dr["Script"].ToString();
            }
            catch { }
            if (dr.Table.Columns.Contains("AllowPreview"))
            {
                if (!bool.Parse(dr["AllowPreview"].ToString())){
                    textEditSoLien.Text = dr["Solien"].ToString();
                    textEditSoLien.Enabled = false;
                    simpleButtonChapNhan.Enabled = false;
                }
                else
                {
                    textEditSoLien.Enabled = true;
                    simpleButtonChapNhan.Enabled = true;
                }
            }
            else
            {
                textEditSoLien.Enabled = true;
                simpleButtonChapNhan.Enabled = true;
            }
            this.textEditTitle.Text = _title;
        }

        private void btShowCode_Click(object sender, EventArgs e)
        {
            if (bool.Parse(Config.GetValue("Admin").ToString()))
            {
                if (this.Height == 166)
                {
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.Height = 300;
                }
                else
                {
                    layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Height = 166;
                }
            }
            else
            {
                
            }
        }

        private void simpleButtonExport_Click(object sender, EventArgs e)
        {
            for (int j = 1; j <= Int32.Parse(textEditSoLien.Text); j++)
            {
                PrintOrPreview(2);
            }
            this.Dispose();
        }
    }
}
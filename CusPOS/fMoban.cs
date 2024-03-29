using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;

using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
//using publicCDT;
using DataFactory;
using System.Drawing.Drawing2D;
using FormFactory;
namespace CusPOS
{
    public partial class fMoban : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();
        cBan _cban;
        public DataSingle dmkh;
        private string _maposloaigia = "BT";
        
        public cBan Ban
        {
            get { return _cban; }
            set
            {
                _cban = value;
                bs = new BindingSource();
                bs.DataSource = _cban.tb;
                gridControl1.DataSource = bs;
                _cban.TienChange += _cban_TienChange;
                txtSoBan.Text = _cban.tSoBan;
                caTongTien.EditValue = _cban.tTien;
                grdmkh.EditValue = _cban.MaKH;
                grTk.EditValue = _cban.TK;
                caPtCK.EditValue = _cban.PtCK;                
                _cban.tb.ColumnChanged += Tb_ColumnChanged;
                _cban.tb.RowDeleted += Tb_RowDeleted;
                memoEdit1.Text = _cban.Ghichu;
            }
        }

        private void Tb_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (_cban.isPrinted)
            {
                _cban.tb.RejectChanges();
                return;
            }
            _cban.GetSum();
        }

        private void Tb_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                if (_cban.isPrinted)
                {
                    _cban.tb.RejectChanges();
                    return;
                }
                if (e.Column.ColumnName == "soluong" )
                {
                    if(dmposGia!=null )
                    {
                        DataRow[] ldr = dmposGia.Select("MaMon='" + e.Row["MaMon"].ToString() + "' and SLMin<=" + e.Row["Soluong"].ToString());
                        if (ldr.Length > 0 && _cban.PtCK == 0)
                        {
                            e.Row["PtCkCt"] =100- double.Parse(ldr[0]["Tile"].ToString())*100;
                        }
                        else e.Row["PtCkCt"] = 0;
                    }
                    _cban.GetSum();
                }
                if (e.Column.ColumnName == "PtCkCt")
                {
                    _cban.GetSum();
                }
                
            }
            catch { }
        }

        private void _cban_TienChange(object sender, EventArgs e)
        {
            caTongTien.EditValue = _cban.tTienH;
            caCK.EditValue = _cban.Ck;
            caThanhToan.EditValue = _cban.tTien;
            
        }
        public DataTable dmtk;
        public DataTable dmloaithe;
        public DataTable dmposLoaiGia;
        public DataTable dmposGia;
        public DataTable ctBieuphi;
        public double PtPhi = 0;
        PanelControl pcLoaiMon = new PanelControl();
        List<PanelControl> lpc = new List<PanelControl>();
        DataTable tbLoaiMon;
        DataTable tbMon;
        BindingSource bs = new BindingSource();
        public string MaPOSArea;
        public fMoban(string maposArea = "")
        {
            InitializeComponent();
            this.Resize += new EventHandler(fMoban_Resize);
            MaPOSArea = maposArea;
            Khoitao();
            gridControl1.KeyUp += new KeyEventHandler(gridControl1_KeyUp);
            
        }

        void gridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_cban.tb.Rows.Count > 0 && bs.Current != null && e.KeyCode == Keys.F4)
            {
                if (!_cban.isPrinted)
                    bs.RemoveCurrent();
            }
        }
        private void Khoitao()
        {
            dmkh = DataFactory.publicCDTData.findCDTData("DMKH", "", "");
            if (dmkh == null)
            {
                dmkh = DataFactory.DataFactory.Create(CDTControl.DataType.Single, "DMKH", Config.GetValue("sysPackageID").ToString()) as DataSingle;
                DataFactory.publicCDTData.AddCDTData(dmkh);
                dmkh.GetData();
            }
            
            this.grdmkh.Properties.DataSource = dmkh.DsData.Tables[0];
           // grdmkh.EditValue = _cban.MaKH;
            string  sql = "select mamon, tenmon, giaban, hinh, maloaimon from dmmon where mamon in (select mamon from  dmmon4area where MaPOSArea='" + MaPOSArea +"')";
            tbMon = db.GetDataTable(sql);
            sql= "select  maloaimon from dmmon where mamon in (select mamon from  dmmon4area where MaPOSArea='" + MaPOSArea + "')";
            sql = "select maloaimon, tenloaimon from dmloaimon where maloaimon in (" + sql +")";
            tbLoaiMon = db.GetDataTable(sql);
            repositoryItemGridLookUpEdit1.DataSource = tbMon;
            pcLoaiMon.Visible = true;
            panelControl1.Controls.Add(pcLoaiMon);
            pcLoaiMon.Width = 10;
            pcLoaiMon.Height = panelControl1.Height;
            int x = 0;
            pcLoaiMon.Left = sLeft.Width;

            foreach (DataRow drLM in tbLoaiMon.Rows)
            {
                SimpleButton sp = new SimpleButton();
                sp.Text = drLM["tenloaimon"].ToString();
                sp.Tag = drLM["maloaimon"].ToString();
                sp.Width = 100;
                sp.Height = pcLoaiMon.Height;
                sp.Font = new Font("Times New Roman", 10, FontStyle.Bold);
                sp.ForeColor = Color.Firebrick;
                sp.Left = x;
                sp.Top = 0;
                x += sp.Width;
                pcLoaiMon.Controls.Add(sp);
                pcLoaiMon.Width += sp.Width;
                sp.Click += new EventHandler(sp_Click);
                //Khởi tạo món
                DataRow[] ldrMon = tbMon.Select("maloaimon='" + sp.Tag.ToString() + "'");
                PanelControl pc = new PanelControl();
                pc.Text = sp.Tag.ToString();
                pc.Visible = true;
                this.Controls.Add(pc);
                pc.BringToFront();
                pc.Dock = DockStyle.Fill;

                lpc.Add(pc);
                PanelControl pc1 = new PanelControl();
                DevExpress.XtraEditors.VScrollBar VSsr = new DevExpress.XtraEditors.VScrollBar();
                VSsr.Minimum = 0;
                VSsr.Maximum = 1;
                VSsr.LargeChange = 1;
                VSsr.Scroll += VSsr_Scroll;
                pc.Controls.Add(VSsr);
                VSsr.Dock = DockStyle.Right;
                pc.Resize += Pc_Resize;
                pc1.Width = pc.Width - VSsr.Width ;
                pc1.Top = 0;
                pc1.Left = 0;
                VSsr.Tag = pc1;
                pc1.Height = 100;
                pc.Controls.Add(pc1);
                int xt = 0;
                int yt = 0;
                
                foreach (DataRow drMon in ldrMon)
                {
                    cMon sb = new cMon(drMon);
                    
                    pc1.Controls.Add(sb);
                    pc1.Resize += Pc1_Resize;                    
                    sb.Chonmon += new EventHandler(sb_Click);
                }
            }
        }

        

        private void Pc1_Resize(object sender, EventArgs e)
        {
            PanelControl pc1 = sender as PanelControl;
            int xt = 0;
            int yt = 0;
            
            foreach ( Control c in pc1.Controls)
            {
                c.Top = yt;
                c.Left = xt;
                if (xt < pc1.Width - 2 * c.Width)
                {
                    xt += c.Width;
                }
                else
                {
                    xt = 0;
                    yt += 100;                   
                }
            }
        }

        private void Pc_Resize(object sender, EventArgs e)
        {
            PanelControl pc = sender as PanelControl;
            DevExpress.XtraEditors.VScrollBar VSsr = pc.Controls[0] as DevExpress.XtraEditors.VScrollBar;
            PanelControl pc1 = VSsr.Tag as PanelControl;
            if (pc1 != null) pc1.Width = pc.Width - VSsr.Width;

            VSsr.Maximum = (int)(pc1.Controls.Count / (int)(pc1.Width / 120))+1;
            pc1.Height = 100 * VSsr.Maximum;
        }

        private void VSsr_Scroll(object sender, ScrollEventArgs e)
        {
           int i= e.NewValue;
            PanelControl p = (sender as DevExpress.XtraEditors.VScrollBar).Tag as PanelControl;
            p.Top = 0 - (i ) * 100;
        }

        void sb_Click(object sender, EventArgs e)
        {
            if (_cban.isPrinted) return;
            DataRow drMon=(sender as cMon).Tag as DataRow;
            DataRow[] ldr = _cban.tb.Select("MaMon='" + drMon["Mamon"].ToString() + "'");
            if (ldr.Length == 0)
            {
                DataRow dr = _cban.tb.NewRow();
                dr["mtposid"] = _cban.id;
                dr["ctposid"] = Guid.NewGuid();
                dr["mamon"] = drMon["Mamon"];
                dr["tenmon"] = drMon["TenMon"];
                dr["dongia"] = drMon["Giaban"];
                dr["ptckct"] = 0;
                _cban.tb.Rows.Add(dr);
                dr["soluong"] = 1;
            }
            else
            {
                ldr[0]["ptckct"] = 0;
                ldr[0]["soluong"] = double.Parse(ldr[0]["soluong"].ToString()) + 1;
            }
            
        }

        void fMoban_Resize(object sender, EventArgs e)
        {
            foreach (PanelControl p1 in lpc)
            {
                int x = 0; int y = 0;
                foreach (Control cb in p1.Controls)
                {
                    cb.Top = y;
                    cb.Left = x;
                    if (x > this.Width - panelControl4.Width - 2 * cb.Width)
                    {
                        x = 0;
                        y += cb.Height ;
                    }
                    else
                    {
                        x += cb.Width;
                    }
                }

            }
        }

        private void sLeft_Click(object sender, EventArgs e)
        {
          if(pcLoaiMon.Left<0)  pcLoaiMon.Left += 100;
        }

        private void sRight_Click(object sender, EventArgs e)
        {
           if(pcLoaiMon.Width+ pcLoaiMon.Left>100) pcLoaiMon.Left += -100;
        }
       

        private void fMoban_Load(object sender, EventArgs e)
        {
            //Khởi tạo nhóm món
            if (dmtk != null) grTk.Properties.DataSource = dmtk;
            if (dmloaithe != null) grLoaiThe.Properties.DataSource = dmloaithe ;
            if (_cban != null)
            {
                txtSoBan.Text = _cban.tSoBan;
                caTongTien.EditValue = _cban.tTienH;
                grdmkh.EditValue = _cban.MaKH;
                grTk.EditValue = _cban.TK;
                caPtCK.EditValue = _cban.PtCK;
                checkBox1.Checked = _cban.LayHD;
                caTTTM.EditValue = _cban.TTTM;
                caTTThe.EditValue = _cban.TTThe;
                caNo.EditValue = _cban.tTien - _cban.TTTM - _cban.TTThe;
                grLoaiThe.EditValue = _cban.MaLoaiThe;
            }

            if (lpc.Count > 0) lpc[0].BringToFront();
        }

        void sp_Click(object sender, EventArgs e)
        {
            foreach (PanelControl pn in lpc)
            {
                if (pn.Text == (sender as SimpleButton).Tag.ToString()) pn.BringToFront();
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            _cban.Save();
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Print();
        }
        private void Print()
        {
            string path;
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + "billPOS.repx";
            else
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + "billPOS.repx";

            if (System.IO.File.Exists(path))
                rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
            else
                rptTmp = new DevExpress.XtraReports.UI.XtraReport();

            if (rptTmp != null)
            {try
                {
                    _cban.Save();
                    rptTmp.DataSource = _cban.tb;
                    rptTmp.ScriptReferences = new string[] { Application.StartupPath + "\\CDTLib.dll" };
                    SetVariables(rptTmp);
                    DevExpress.XtraReports.UI.XRControl xrcTongTien = rptTmp.FindControl("TongTien", true);
                    if (xrcTongTien != null)
                        xrcTongTien.Text = double.Parse(_cban.tTienH.ToString()).ToString("### ### ### ###");
                    DevExpress.XtraReports.UI.XRControl xrcCK = rptTmp.FindControl("CK", true);
                    if (xrcCK != null)
                        xrcCK.Text = double.Parse(_cban.Ck.ToString()).ToString("### ### ### ###");
                    DevExpress.XtraReports.UI.XRControl xrcThanhToan = rptTmp.FindControl("Thanhtoan", true);
                    if (xrcThanhToan != null)
                        xrcThanhToan.Text = double.Parse(_cban.tTien.ToString()).ToString("### ### ### ###");
                    DevExpress.XtraReports.UI.XRControl xrcSophieu = rptTmp.FindControl("Sophieu", true);
                    if (xrcSophieu != null)
                        xrcSophieu.Text = _cban.SoCT;
                    DevExpress.XtraReports.UI.XRControl xrcMaBan = rptTmp.FindControl("MaBan", true);
                    if (xrcMaBan != null)
                        xrcMaBan.Text = _cban.tSoBan.ToString();
                    //DevExpress.XtraReports.UI.XRControl xrcDatt = rptTmp.FindControl("DaTT", true);
                    //if (xrcDatt != null)
                    //    xrcDatt.Text = double.Parse(cDaTT.EditValue.ToString()).ToString("### ### ### ###");
                    //DevExpress.XtraReports.UI.XRControl xrcConlai = rptTmp.FindControl("Conlai", true);
                    //if (xrcConlai != null)
                    //    xrcConlai.Text = double.Parse(cConlai.EditValue.ToString()).ToString("### ### ### ###");
                    DevExpress.XtraReports.UI.XRControl xrcID = rptTmp.FindControl("ID", true);
                    if (xrcID != null)
                        xrcID.Text = _cban.id.ToString();
                    //DevExpress.XtraReports.UI.XRControl xrbID = rptTmp.FindControl("BCID", true);
                    //if (xrbID != null)
                    //    xrbID.Text = txtCode.Text;

                    ////rptTmp.Print();
                    rptTmp.ShowPreview();
                    _cban.Printed();
                }
                catch (Exception ex) { }
            }
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
        private void editPrint()
        {
            string path;
            DevExpress.XtraReports.UI.XtraReport rptTmp = null;
            if (Config.GetValue("DuongDanBaoCao") != null)
                path = Config.GetValue("DuongDanBaoCao").ToString() + "\\" + Config.GetValue("Package").ToString() + "\\" + "billPOS.repx";
            else
                path = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\" + "billPOS.repx";

            if (System.IO.File.Exists(path))
                rptTmp = DevExpress.XtraReports.UI.XtraReport.FromFile(path, true);
            else
                rptTmp = new DevExpress.XtraReports.UI.XtraReport();
            if (rptTmp != null)
            {
                rptTmp.DataSource = _cban.tb;
                XRDesignFormEx designForm = new XRDesignFormEx();
                designForm.OpenReport(rptTmp);
                if (System.IO.File.Exists(path))
                    designForm.FileName = path;
                designForm.KeyPreview = true;
                designForm.Show();
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            editPrint();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

             _cban.TK = "1111";
            if (_cban.TTThe > 0)
            {
                if (_cban.TK == null)
                {
                    MessageBox.Show("Chọn tải khoản cần thanh toán");
                    return;
                }
                if(_cban.MaLoaiThe==null)
                {
                    MessageBox.Show("Chọn loại thẻ cần thanh toán");
                    return;
                }
            }
                
            
                _cban.Thanhtoan();
            this.Close();
        }

        private void grdmkh_EditValueChanged(object sender, EventArgs e)
        {
           this._cban.MaKH= (sender as GridLookUpEdit).EditValue.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this._cban.LayHD = checkBox1.Checked;
        }



        private void caPtCK_EditValueChanged(object sender, EventArgs e)
        {
            _cban.PtCK = double.Parse(caPtCK.EditValue.ToString());
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)//Tích vào checkbox TT Tiền mặt
            {
                _cban.TTTM = _cban.tTien;
                caTTTM.EditValue= _cban.tTien;
            }
            else
            {
                _cban.TTTM = 0;
                caTTTM.EditValue = 0;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)//Tích vào checkbox TT Thẻ
            {
                _cban.TTThe = _cban.tTien;
                _cban.TTTM = 0;
                caTTThe.EditValue = _cban.TTThe;
                caTTTM.EditValue = 0;
                
            }
        }

        private void caTTTM_EditValueChanged(object sender, EventArgs e)
        {
            _cban.TTTM = double.Parse(caTTTM.EditValue.ToString());
            _cban.TTThe = _cban.tTien - _cban.TTTM;
            caTTThe.EditValue = _cban.TTThe;
            caTTTM.EditValue = _cban.TTTM;
            caNo.EditValue = _cban.tTien - _cban.TTTM - _cban.TTThe;
        }

        private void panelControl3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void grLoaiThe_EditValueChanged(object sender, EventArgs e)
        {
            if (grLoaiThe.EditValue !=null)
            {
                _cban.MaLoaiThe = grLoaiThe.EditValue.ToString();
            }
            if(_cban.MaLoaiThe !=null  && _cban.TK != null)
            {
                //DataRow[] ldr = ctBieuphi.Select("MaLoaiThe='" + _cban.MaLoaiThe + "' and Tk='" + _cban.TK + "' and Muc<=" + _cban.TTThe);
                //if (ldr.Length > 0) _cban.PtPhi = double.Parse(ldr[0]["Tile"].ToString());
                //else _cban.PtPhi = 0;
                //caPtPhi.EditValue = _cban.PtPhi;
            }
        }

        private void grTk_EditValueChanged(object sender, EventArgs e)
        {
            if(grTk.EditValue!=null)
            _cban.TK = grTk.EditValue.ToString();
            grLoaiThe_EditValueChanged(sender, e);
        }

        private void caTTThe_EditValueChanged(object sender, EventArgs e)
        {            
            _cban.TTThe = double.Parse(caTTThe.EditValue.ToString());
            grLoaiThe_EditValueChanged(sender, e);
            caNo.EditValue = _cban.tTien - _cban.TTTM - _cban.TTThe;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _cban.TTTM = 0;_cban.TTThe = 0;
            caTTTM.EditValue = 0;caTTThe.EditValue = 0;
        }

        private void memoEdit1_EditValueChanged(object sender, EventArgs e)
        {
            
            _cban.Ghichu = memoEdit1.Text;
        }

        private void btAddKH_Click(object sender, EventArgs e)
        {
            if (dmkh == null) return;
            BindingSource bs = new BindingSource();
            bs.DataSource = dmkh.DsData.Tables[0];
            FrmSingleDt  frm = new FrmSingleDt(dmkh, bs);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                
            }
           
        }
    }
}
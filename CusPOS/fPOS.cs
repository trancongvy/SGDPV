using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using DataFactory;
namespace CusPOS
{
    
    public partial class fPOS : DevExpress.XtraEditors.XtraForm
    {
        double TileCk = double.Parse(Config.GetValue("TileCk").ToString());
        Database db = Database.NewDataDatabase();
        int TrangThai = 0;//0: Bình thường; 1: gộp bàn; 2: Tách bàn

        public fPOS()
        {
            InitializeComponent();

        }
        public DataTable dmposLoaiGia;
        public DataTable dmtk;
        public DataTable dmloaithe;
        public DataTable ctBieuphi;
        private void fPOS_Load(object sender, EventArgs e)
        {
            KhoiTaoKhuVuc();
            this.Resize += new EventHandler(fPOS_Resize);
            string sql = "select * from dmPosLoaiGia where apdung=1";
            dmposLoaiGia = db.GetDataTable(sql);
            dmposLoaiGia.PrimaryKey = new DataColumn[] { dmposLoaiGia.Columns["MaPOSLoaiGia"] };

            //caTileCk.EditValue = TileCk;
            
            sql = "select TK,TenTK from dmTK where (TK like '112%') and tk not in (select tkme from dmtk where tkme is not null) ";
             dmtk = db.GetDataTable(sql);
            
            sql = "select * from dmloaithe ";
            dmloaithe = db.GetDataTable(sql);
            dmloaithe.PrimaryKey = new DataColumn[] { dmloaithe.Columns["MaLoaiThe"] };

            sql = "select a.*, b.TenMon from dmPOSGia a inner join dmMon b on a.MaMon=b.MaMon inner join dmposloaigia c on a.maposloaigia=c.maposloaigia where Apdung=1  order by  mamon,tile,slMin";
            dmposGia = db.GetDataTable(sql);
           // sql = "select * from ctbieuphi order by maloaithe, muc desc";
           // ctBieuphi = db.GetDataTable(sql);
        }


        DataTable tbA;
        DataSingle _dbPosArea;
        private void KhoiTaoKhuVuc()
        {
            _dbPosArea = new DataSingle("dmPOSArea", Config.GetValue("sysPackageID").ToString());
            // string sql = "select * from dmPOSArea where macn='" + Config.GetValue("MaCN").ToString() + "'";
            _dbPosArea.GetData();
            if (_dbPosArea == null) return;

            tbA = _dbPosArea.DsData.Tables[0];
            int x = 0;
            foreach (DataRow drA in tbA.Rows)
            {
                SimpleButton bt = new SimpleButton();
                bt.Text = drA["TenPOSArea"].ToString();
                bt.Tag = drA["MaPOSArea"].ToString();
                panelControl1.Controls.Add(bt);
                bt.Left = x;
                bt.Top = 45;
                bt.Size = new Size(150, 100);
                bt.Visible = true;
                x += 151;
                bt.Click += new EventHandler(bt_Click);
                KhoiTaoBan(bt.Tag.ToString());
            }
        }
        List<PanelControl> lPan = new List<PanelControl>();
        void fPOS_Resize(object sender, EventArgs e)
        {
            //Sapsep
                foreach (PanelControl p1 in lPan)
                {
                    int x = 0; int y = 0;
                    foreach (Control cb in p1.Controls)
                    {
                        cb.Top = y;
                        cb.Left = x;
                        if (x > this.Width - 2 * cb.Width)
                        {
                            x = 0;
                            y += cb.Height + 2;
                        }
                        else
                        {
                            x += cb.Width + 2;
                        }
                    }

                }
        }
        private void KhoiTaoBan(string MaKV)
        {
            PanelControl p1 = new PanelControl();
            p1.Text = MaKV;
            p1.Dock = DockStyle.Fill;
            p1.Visible = true;
            this.Controls.Add(p1);
            lPan.Add(p1);
            p1.BringToFront();
            string sql = "select * from dmban where MaPOSArea='" + MaKV + "'";
            DataTable tb = db.GetDataTable(sql);
            int x = 0; int y = 0;
            foreach (DataRow dr in tb.Rows)
            {
                cBan cb = new cBan(dr["MaBan"].ToString());
                cb.Top = y;
                cb.Left = x;
                cb.Visible = true;
                cb.MaPOSArea = MaKV;
                p1.Controls.Add(cb);
                cb.ChonBan += new EventHandler(cb_ChonBan);
                if (x > this.Width - 2 * cb.Width)
                {
                    x = 0;
                    y += cb.Height + 2;
                }
                else
                {
                    x += cb.Width + 2;
                }
            }
        }

        void bt_Click(object sender, EventArgs e)
        {
            foreach (PanelControl c in lPan)
            {
                if (c.Text == (sender as SimpleButton).Tag.ToString())
                    c.BringToFront();
            }
            fMoBan = null;
            //Khởi tạo bàn
        }

        fMoban fMoBan;
        void cb_ChonBan(object sender, EventArgs e)
        {
            if (TrangThai == 0)
            {
                cBan cb = sender as cBan;
                if (fMoBan == null)
                {
                    fMoBan = new fMoban(cb.MaPOSArea);
                    fMoBan.dmposGia = this.dmposGia;
                    fMoBan.dmposLoaiGia = this.dmposLoaiGia;
                    fMoBan.dmtk = this.dmtk;
                    fMoBan.ctBieuphi = this.ctBieuphi;
                    fMoBan.dmloaithe = this.dmloaithe;
                }
                fMoBan.Ban = cb;
                fMoBan.ShowDialog();
            }
        }

        private void caTileCk_EditValueChanged(object sender, EventArgs e)
        {
            
        }
        public DataTable dmposGia;
        public string MaPOSLoaiGia;
        private void grLoaiGia_EditValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
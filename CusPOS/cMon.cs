using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTControl;
using CDTLib;

namespace CusPOS
{
   public class cMon : DevExpress.XtraEditors.XtraUserControl
    {
       
        private void InitializeComponent()
        {
            this.tTenMon = new System.Windows.Forms.Label();
            this.tGia = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tTenMon
            // 
            this.tTenMon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tTenMon.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.tTenMon.ForeColor = System.Drawing.Color.Navy;
            this.tTenMon.Location = new System.Drawing.Point(5, 5);
            this.tTenMon.Name = "tTenMon";
            this.tTenMon.Size = new System.Drawing.Size(110, 70);
            this.tTenMon.TabIndex = 0;
            this.tTenMon.Text = "";
            this.tTenMon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tGia
            // 
            this.tGia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tGia.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.tGia.ForeColor = System.Drawing.Color.Red;
            this.tGia.Location = new System.Drawing.Point(5, 75);
            this.tGia.Name = "tGia";
            this.tGia.Size = new System.Drawing.Size(110, 20);
            this.tGia.TabIndex = 1;
            this.tGia.Text = "";
            this.tGia.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cMon
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tGia);
            this.Controls.Add(this.tTenMon);
            this.Name = "cMon";
            this.Size = new System.Drawing.Size(120,100);
            this.ResumeLayout(false);
            this.MouseDown += CMon_MouseDown;
            this.MouseLeave += CMon_MouseLeave; this.MouseUp += CMon_MouseUp;
            this.tTenMon.MouseDown+= CMon_MouseDown;
            this.tTenMon.Click += TTenMon_Click;
            this.tTenMon.MouseLeave += CMon_MouseLeave; this.tTenMon.MouseUp += CMon_MouseUp;
            this.tGia.MouseDown += CMon_MouseDown;
            this.tGia.MouseLeave += CMon_MouseLeave; this.tGia.MouseUp += CMon_MouseUp;
            this.tGia.Click += TGia_Click;
        }

        private void TGia_Click(object sender, EventArgs e)
        {
            Chonmon(this, e);
        }

        public event EventHandler Chonmon;
        private void TTenMon_Click(object sender, EventArgs e)
        {
            Chonmon(this, e);
        }

        private void CMon_MouseLeave(object sender, EventArgs e)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void CMon_MouseUp(object sender, MouseEventArgs e)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void CMon_MouseDown(object sender, MouseEventArgs e)
        {
            this.BorderStyle = BorderStyle.Fixed3D;
        }

        public string MaMon;
        public string TenMon;
        private Label tTenMon;
        private Label tGia;
        public double Gia;
        public  cMon(DataRow dr)
        {
            InitializeComponent();
            this.Font = new Font("Times New Roman", 10, FontStyle.Bold);
           
            this.Visible = true;
            this.MaMon = dr["Mamon"].ToString();
            this.TenMon = dr["Tenmon"].ToString();
            this.Gia = double.Parse(dr["GiaBan"].ToString());
            //sb.Text = drMon["Tenmon"].ToString() + "\n" + double.Parse(drMon["GiaBan"].ToString()).ToString("### ### ### ###");
            this.tTenMon.Text = TenMon;
            this.tGia.Text = Gia.ToString("### ### ##0");
            this.Tag = dr;
            
        }
       
    }
}

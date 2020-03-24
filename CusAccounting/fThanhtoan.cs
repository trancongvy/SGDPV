using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CusAccounting
{
    public partial class fThanhtoan : DevExpress.XtraEditors.XtraForm
    {
        public double Sotien;
        public fThanhtoan()
        {
            InitializeComponent();
        }

        private void fThanhtoan_Load(object sender, EventArgs e)
        {
            calcEdit1.Select();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Sotien = double.Parse(calcEdit1.EditValue.ToString());
            this.Dispose();
        }
    }
}
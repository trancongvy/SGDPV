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
using CDTControl;
namespace CDTSystem
{
    public partial class fChonCN : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();
        public DataSingle _dbCN;

        public string MaCN = string.Empty;

        public string TenCN = string.Empty;
        public fChonCN()
        {
            InitializeComponent();
            _dbCN = new DataSingle("dmChinhanh", Config.GetValue("sysPackageID").ToString());
            _dbCN.GetData();
            gridLookUpEdit1.Properties.DataSource = _dbCN.DsData.Tables[0];
            if (_dbCN.DsData.Tables[0].Rows.Count == 1) gridLookUpEdit1.EditValue = _dbCN.DsData.Tables[0].Rows[0]["MaCN"].ToString();
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            MaCN = gridLookUpEdit1.EditValue.ToString();
            DataRow[] ldr  = _dbCN.DsData.Tables[0].Select("MaCN='" + MaCN + "'");
            if (ldr.Length > 0) TenCN = ldr[0]["TenCN"].ToString();
        }

        private void fChonCN_Load(object sender, EventArgs e)
        {
            DevLocalizer.Translate(this);
            gridLookUpEdit1.KeyDown += GridLookUpEdit1_KeyDown;
        }

        private void GridLookUpEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && !(sender as GridLookUpEdit).IsPopupOpen)
            {
                (sender as GridLookUpEdit).ShowPopup();
                e.Handled = true;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (MaCN != string.Empty)
            {
                Config.NewKeyValue("MaCN", MaCN);
                Config.NewKeyValue("TenCN", TenCN);

                this.DialogResult = DialogResult.OK;
            }
            
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

       
    }
}
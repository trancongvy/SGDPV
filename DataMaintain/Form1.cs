using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraEditors;

namespace DataMaintain
{
    public partial class Form1 : Form
    {
        string structDb;
        string dataDb;
        string sysPackageID;
        private string mtName = string.Empty;
        private string Condition = string.Empty;
        bool isremote = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!GetPackageInfo())
                return;
            DataTransfer dtf = new DataTransfer(structDb, dataDb, sysPackageID);
            dtf.Maintain(dtTuNgay.DateTime, dtDenNgay.DateTime, ceDeleteOnly.Checked,mtName, Condition);
            MessageBox.Show("Hoàn thành!");
        }

        private bool GetPackageInfo()
        {

            string H_KEY = "HKEY_CURRENT_USER\\Software\\SGD\\" + tePackageName.Text + "\\";
            structDb = Registry.GetValue(H_KEY, isremote?"RemoteServer": "StructDb", string.Empty).ToString();
            structDb = Security.DeCode64(structDb);
            Database db = Database.NewCustomDatabase(structDb);
            string sql = "select * from sysDB where DatabaseName = '" + tePackageName.Text + "'";
            DataTable dt = db.GetDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy thông tin gói phần mềm này!");
                return false;
            }
            sysPackageID = dt.Rows[0]["sysPackageID"].ToString();
            dataDb = Security.DeCode64(dt.Rows[0][isremote?"DBPathRemote": "DBPath"].ToString()) + "; database = " + dt.Rows[0]["DatabaseName"].ToString();

            return true;
        }

        private void tMtName_EditValueChanged(object sender, EventArgs e)
        {
            mtName = tMtName.Text;
        }

        private void tCondition_EditValueChanged(object sender, EventArgs e)
        {
            Condition = tCondition.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string packname = Config.GetValue("ProductName").ToString();
                tePackageName.Text = packname;
            }
            catch { }
        }

        private void ceDeleteOnly_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ckRemote_CheckedChanged(object sender, EventArgs e)
        {
            isremote = ckRemote.Checked;
        }
    }
}
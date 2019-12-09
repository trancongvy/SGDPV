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
    public partial class fChonCa : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();

        public DataSingle _dbCa;

        public string MaCa ;

        public fChonCa()
        {
            InitializeComponent();
            _dbCa = new DataSingle("dmCa", Config.GetValue("sysPackageID").ToString());
            _dbCa.GetData();
            gridLookUpEdit2.Properties.DataSource = _dbCa.DsData.Tables[0];
            if (bool.Parse(Config.GetValue("TheoCa").ToString()) && _dbCa.DsData.Tables[0].Rows.Count == 1)
            {
                gridLookUpEdit2.EditValue = _dbCa.DsData.Tables[0].Rows[0]["MaCa"].ToString();
            }
            else if(_dbCa.DsData.Tables[0].Rows.Count == 0)
            {
                gridLookUpEdit2.EditValue = null;

            }
            if (!bool.Parse(Config.GetValue("TheoCa").ToString()))gridLookUpEdit2.Visible = false;
        }

        
        private void fChonCa_Load(object sender, EventArgs e)
        {
            DevLocalizer.Translate(this);
           
        }

        

        private void simpleButton1_Click(object sender, EventArgs e)
        {

                Config.NewKeyValue("MaCa", MaCa);
                this.DialogResult = DialogResult.OK;

            
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void gridLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (gridLookUpEdit2.EditValue == null)
                MaCa = null;
            else
                MaCa = gridLookUpEdit2.EditValue.ToString();           
        }
    }
}
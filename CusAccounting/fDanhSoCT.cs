using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FormFactory;
using DataFactory;
using CDTDatabase;
using CDTLib;
namespace CusAccounting
{
    public partial class fDanhSoCT : XtraForm
    {
        public fDanhSoCT()
        {
            InitializeComponent();
        }
        Database _dbStruct = Database.NewStructDatabase();
        Database _dbData = Database.NewDataDatabase();
        DataMasterDetail _data;
        FormDesigner _designer;
        DevExpress.XtraGrid.GridControl gMain;
        private void fDanhSoCT_Load(object sender, EventArgs e)
        {
            string sql = " select a.systableid, a.TableName, a.MasterTable, b.Diengiai from systable a inner join systable b on a.mastertable = b.tablename" +
                            " where a.mastertable is not null and a.type = 3 and a.tablename like 'DT%'";
            DataTable tb = _dbStruct.GetDataTable(sql);
            grLoaiCT.Properties.DataSource = tb;
        }

        private void grLoaiCT_EditValueChanged(object sender, EventArgs e)
        {
            string systableID = grLoaiCT.EditValue.ToString();
            //int packageID = int.Parse(Config.GetValue("sysPackageID").ToString());
            _data = new DataMasterDetail(systableID);
            _data.GetData();
            _designer = new FormDesigner(_data);
            //_data.GetInfor()
            gMain = _designer.GenGridControl(_data.DsStruct.Tables[0], false, DockStyle.Fill);
            gMain.DataSource = _data.DsData.Tables[0];
            panelControl3.Controls.Clear();
            panelControl3.Controls.Add(gMain);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void btGetdata_Click(object sender, EventArgs e)
        {

        }
    }
}

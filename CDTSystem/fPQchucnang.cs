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
namespace CDTSystem
{
    public partial class fPQchucnang : DevExpress.XtraEditors.XtraForm
    {
        private Database _dbData = Database.NewDataDatabase();
        private DataSet DsData;
        BindingSource bs = new BindingSource();
        public fPQchucnang()
        {
            _dbData = Database.NewStructDatabase();
            InitializeComponent();
            GetData();
            
        }
        private void GetData()
        {
            string sqlMt = "select a.*, e.GroupName, e.DienGiai, c.Package, d.DatabaseName ";
            sqlMt += " from sysuserpackage a inner join sysUserGroup e on a.sysUserGroupID=e.sysUserGroupID ";
            sqlMt += "inner join syspackage c on a.syspackageid=c.syspackageid inner join sysdb d on a.sysdbID=d.sysdbid";
            string sqlDt = "select a.*, b.MenuName,b.sysPackageID, sysTableID,sysMenuParent ";
            sqlDt += " from  sysusermenu a inner join  sysmenu b on a.sysmenuid=b.sysmenuid where b.isVisible=1";
            //
           // sqlDt = "select * from (select a.*, b.MenuName,b.sysPackageID, sysTableID,sysMenuParent   from  sysusermenu a inner join  sysmenu b on a.sysmenuid=b.sysmenuid where b.isVisible=1) x " +
            //    " where sysMenuParent in (  select a.sysMenuID from sysusermenu a inner join  sysmenu b on a.sysmenuid=b.sysmenuid where b.isVisible=1)"    ;
            DsData = _dbData.GetDataSetDetail(sqlMt, sqlDt);
            DataColumn pk = DsData.Tables[1].Columns["sysUserPackageID"];
            DataColumn fk = DsData.Tables[0].Columns["sysUserPackageID"];
            DsData.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(fPQchucnang_ColumnChanged);
            if (pk != null && fk != null)
            {
                DataRelation dr = new DataRelation("sysUserPackage", pk, fk, true);
                DsData.Relations.Add(dr);
            }
        }

        void fPQchucnang_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.ToUpper() == "EXECUTABLE")
            {
                int sysMenuID = int.Parse(e.Row["sysMenuID"].ToString());
                int sysUserPackageID = int.Parse(e.Row["sysUserPackageID"].ToString());
                DataRow[] ldr = DsData.Tables[0].Select("sysMenuParent=" + sysMenuID.ToString() + " and sysUserPackageID=" + sysUserPackageID.ToString());
                foreach (DataRow dr in ldr)
                {
                    dr["Executable"] = e.Row["Executable"];
                }
            }
        }
        private void DisplayData()
        {
            bs.DataSource = this.DsData;
            bs.DataMember = this.DsData.Tables[1].TableName;
            gridView1.OptionsView.ShowDetailButtons = false;
            bs.CurrentChanged += new EventHandler(bs_CurrentChanged);
            bs_CurrentChanged(bs, new EventArgs());

            gridControl1.DataSource = bs;
            treeList1.DataSource = bs;
            treeList1.DataMember = "sysUserPackage";
            treeList1.BeginInit();
            
            treeList1.KeyFieldName = "sysMenuID";
            treeList1.ParentFieldName = "sysMenuParent" ;
            treeList1.OptionsView.AutoWidth = false;
            treeList1.EndInit();
        }

        void bs_CurrentChanged(object sender, EventArgs e)
        {
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DsData.Tables[0].DefaultView.RowStateFilter = DataViewRowState.ModifiedCurrent;

            foreach (DataRowView drData in DsData.Tables[0].DefaultView)
            {

                string sql = "update sysusermenu set Executable=" + (bool.Parse(drData["Executable"].ToString()) ? "1" : "0") + " where sysUserMenuID=" + drData["sysUserMenuID"].ToString();
                _dbData.UpdateByNonQuery(sql);
                if (_dbData.HasErrors)
                {
                    DsData.Tables[0].DefaultView.RowStateFilter = DataViewRowState.None;
                    break;
                }

            }
            if (!_dbData.HasErrors)
            {
                DsData.Tables[0].DefaultView.RowStateFilter = DataViewRowState.None;
                MessageBox.Show("Cập nhật thành công!");
            }
        }

        private void fPQchucnang_Load(object sender, EventArgs e)
        {
            DisplayData();
            //_dbData.UpdateDatabyStore("SynMenu", new string[] { "@sysPackageid" }, new object[] { int.Parse(Config.GetValue("sysPackageID").ToString()) });
        }
    }
}
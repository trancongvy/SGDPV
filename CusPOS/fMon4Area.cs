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
using CDTDatabase;
using CDTLib;
using DataFactory;
namespace CusPOS
{
    public partial class fMon4Area : XtraForm
    {
        public fMon4Area()
        {
            InitializeComponent();
        }
        public Database db = Database.NewDataDatabase();
        public DataTable dmLoaimon;
        public DataTable dmMon;
        public DataTable dmposMon;
        public DataTable dmposArea;
        public double tileck;
        
        public string MaPOSArea;
        DataFactory.DataSingle dbPOSArea;
        private void grLoaiGia_EditValueChanged(object sender, EventArgs e)
        {
            MaPOSArea = grPOSArea.EditValue.ToString();
            DataRow drPOSArea = dmposArea.Rows.Find(MaPOSArea);
            
            string sql = "select a.*, b.TenMon from dmMon4area a inner join dmMon b on a.MaMon=b.MaMon  where MaPOSArea='" + MaPOSArea + "'";
            dmposMon = db.GetDataTable(sql);
            grPOSGia.DataSource = dmposMon;
        }
        BindingSource bsLoaimon = new BindingSource();
 
        private void fPOSKM_Load(object sender, EventArgs e)
        {
            string sql = "select * from dmPosArea where apdung=1";
            dbPOSArea = new DataSingle("dmPosArea", "7");
            if (dbPOSArea == null) return;
            dbPOSArea.GetData();
            dmposArea = dbPOSArea.DsData.Tables[0];
            dmposArea.PrimaryKey = new DataColumn[] { dmposArea.Columns["MaPOSArea"] };
            grPOSArea.Properties.DataSource = dmposArea;
            sql = "select * from dmloaimon";
            dmLoaimon = db.GetDataTable(sql);
            bsLoaimon.DataSource = dmLoaimon;
            bsLoaimon.CurrentChanged += BsLoaimon_CurrentChanged;
            tlLoaiMon.DataSource = bsLoaimon;
            tlLoaiMon.KeyFieldName = "MaLoaiMon";
            tlLoaiMon.ParentFieldName = "MaLoaiMonMe";
            
        }

        private void BsLoaimon_CurrentChanged(object sender, EventArgs e)
        {
            if (bsLoaimon.Current == null) return;
            DataRowView drLoaimon = bsLoaimon.Current as DataRowView;
            dmMon = db.GetDataSetByStore("GetdmMonByLoaiMon", new string[] { "@maloaimon" }, new object[] { drLoaimon["MaLoaiMon"].ToString() });
            grdmMon.DataSource = dmMon;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (dmposMon == null) return;
            int[] oldIndex = gvMain.GetSelectedRows();
            int[] newIndex = oldIndex;

            for (int i = 0; i < oldIndex.Length; i++)
            {
                newIndex[i] = dmMon.Rows.IndexOf(gvMain.GetDataRow(oldIndex[i]));
                DataRow drMon = dmMon.Rows[newIndex[i]];
                DataRow[] exist = dmposMon.Select("MaMon='" + drMon["Mamon"].ToString() + "'");
                if (exist.Length > 0) dmposMon.Rows.Remove(exist[0]);
                DataRow dr = dmposMon.NewRow();
                dr["MaMon"] = drMon["Mamon"];
                dr["TenMon"] = drMon["TenMon"];
                dr["MaPOSArea"] = MaPOSArea;                
                dmposMon.Rows.Add(dr);
            }


        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            //Xóa những dòng delete
            db.BeginMultiTrans();
            DataView dv = dmposMon.DefaultView;
            dv.RowStateFilter = DataViewRowState.Deleted;
            string sql;
            foreach (DataRowView drv in dv)
            {
                drv.Row.RejectChanges();
                sql = "delete dmMon4area where stt=" + drv.Row["stt"].ToString();
                db.UpdateByNonQuery(sql);
                drv.Row.Delete();
            }
            //Update những dòng modify
            dv.RowStateFilter = DataViewRowState.None;
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            foreach (DataRowView drv in dv)
            {

                sql = "Update dmMon4area set MaMon=" + drv.Row["MaMon"].ToString() + " where stt=" + drv.Row["stt"].ToString();
                db.UpdateByNonQuery(sql);
                drv.Row.AcceptChanges();
            }
            //thêm mới những dòng added
            dv.RowStateFilter = DataViewRowState.None;
            dv.RowStateFilter = DataViewRowState.Added;
            foreach (DataRowView drv in dv)
            {

                sql = "insert into dmMon4area (MaMon,MaPOSArea) values(@MaMon,@MaPOSArea)";
                db.UpdateDatabyPara(sql, new string[] { "@MaMon", "@MaPOSArea" },
                    new object[] { drv.Row["MaMon"].ToString(), drv.Row["MaPOSArea"].ToString()});
                drv.Row.AcceptChanges();
            }
            if (db.HasErrors) db.RollbackMultiTrans();
            else
            {
                db.EndMultiTrans();
                dv.RowStateFilter = DataViewRowState.CurrentRows;
                MessageBox.Show("Update thành công!");
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (dmposMon == null) return;
            int[] oldIndex = gvGia.GetSelectedRows();
            int[] newIndex = oldIndex;

            for (int i = 0; i < oldIndex.Length; i++)
            {
                newIndex[i] = dmposMon.Rows.IndexOf(gvGia.GetDataRow(oldIndex[i]));
                if (newIndex[i] == -1) continue;
                    dmposMon.Rows[newIndex[i]].Delete();

            }
        }

        private void caPtCK_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

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
namespace CusPOS
{
    public partial class fPOSKM : XtraForm
    {
        public fPOSKM()
        {
            InitializeComponent();
        }
        public Database db = Database.NewDataDatabase();
        public DataTable dmLoaimon;
        public DataTable dmMon;
        public DataTable dmposGia;
        public DataTable dmposLoaiGia;
        public double tileck;
        public double TileCk
        {
            get { return tileck; }
            set { tileck = value;
                caPtCK.EditValue = tileck;
            }
        }
        public string MaPOSLoaiGia;

        private void grLoaiGia_EditValueChanged(object sender, EventArgs e)
        {
            MaPOSLoaiGia = grLoaiGia.EditValue.ToString();
            DataRow drLoaigia = dmposLoaiGia.Rows.Find(MaPOSLoaiGia);
            TileCk = double.Parse(drLoaigia["Tile"].ToString());
            string sql = "select a.*, b.TenMon from dmPOSGia a inner join dmMon b on a.MaMon=b.MaMon  where MaPOSLoaiGia='" + MaPOSLoaiGia + "'";
            dmposGia = db.GetDataTable(sql);
            grPOSGia.DataSource = dmposGia;
        }
        BindingSource bsLoaimon = new BindingSource();
        private void fPOSKM_Load(object sender, EventArgs e)
        {
            string sql = "select * from dmPosLoaiGia where apdung=1";
            dmposLoaiGia = db.GetDataTable(sql);
            dmposLoaiGia.PrimaryKey = new DataColumn[] { dmposLoaiGia.Columns["MaPOSLoaiGia"] };
            grLoaiGia.Properties.DataSource = dmposLoaiGia;
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
            if (dmposGia == null) return;
            int[] oldIndex = gvMain.GetSelectedRows();
            int[] newIndex = oldIndex;

            for (int i = 0; i < oldIndex.Length; i++)
            {
                newIndex[i] = dmMon.Rows.IndexOf(gvMain.GetDataRow(oldIndex[i]));
                DataRow drMon = dmMon.Rows[newIndex[i]];
                DataRow[] exist = dmposGia.Select("MaMon='" + drMon["Mamon"].ToString() + "'");
                if (exist.Length > 0) dmposGia.Rows.Remove(exist[0]);
                DataRow dr = dmposGia.NewRow();
                dr["MaMon"] = drMon["Mamon"];
                dr["TenMon"] = drMon["TenMon"];
                dr["MaPOSLoaiGia"] = MaPOSLoaiGia;
                dr["GiaBan"] = double.Parse(drMon["GiaBan"].ToString());
                dr["Tile"] = TileCk;
                dr["Gia"] = Math.Round(double.Parse(dr["GiaBan"].ToString()) * TileCk / 1000, 0) * 1000;
                dr["slMin"] = 1;
                dmposGia.Rows.Add(dr);
            }


        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            //Xóa những dòng delete
            db.BeginMultiTrans();
            DataView dv = dmposGia.DefaultView;
            dv.RowStateFilter = DataViewRowState.Deleted;
            string sql;
            foreach (DataRowView drv in dv)
            {
                drv.Row.RejectChanges();
                sql = "delete dmposgia where maposgia=" + drv.Row["maposgia"].ToString();
                db.UpdateByNonQuery(sql);
                drv.Row.Delete();
            }
            //Update những dòng modify
            dv.RowStateFilter = DataViewRowState.None;
            dv.RowStateFilter = DataViewRowState.ModifiedCurrent;
            foreach (DataRowView drv in dv)
            {

                sql = "Update dmposgia set slMin="+ drv.Row["slMin"].ToString() + " where maposgia=" + drv.Row["maposgia"].ToString();
                db.UpdateByNonQuery(sql);
                drv.Row.AcceptChanges();
            }
            //thêm mới những dòng added
            dv.RowStateFilter = DataViewRowState.None;
            dv.RowStateFilter = DataViewRowState.Added;
            foreach (DataRowView drv in dv)
            {

                sql = "insert into dmposgia (MaMon,MaPOSLoaiGia, Gia, Tile, GiaBan,slMin) values(@MaMon,@MaPOSLoaiGia, @Gia, @Tile, @GiaBan,@slMin)";
                db.UpdateDatabyPara(sql, new string[] { "@MaMon", "@MaPOSLoaiGia", "@Gia", "@Tile", "@GiaBan", "@slMin" },
                    new object[] { drv.Row["MaMon"].ToString(), drv.Row["MaPOSLoaiGia"].ToString(),double.Parse(drv.Row["Gia"].ToString()), double.Parse(drv.Row["Tile"].ToString()), double.Parse(drv.Row["GiaBan"].ToString()), double.Parse(drv.Row["slMin"].ToString())});
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
            if (dmposGia == null) return;
            int[] oldIndex = gvGia.GetSelectedRows();
            int[] newIndex = oldIndex;

            for (int i = 0; i < oldIndex.Length; i++)
            {
                newIndex[i] = dmposGia.Rows.IndexOf(gvGia.GetDataRow(oldIndex[i]));
                if (newIndex[i] == -1) continue;
                    dmposGia.Rows[newIndex[i]].Delete();

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

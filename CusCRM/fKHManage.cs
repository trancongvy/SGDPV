using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CDTDatabase;
using FormFactory;
using DataFactory;
using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
namespace CusCRM
{
    public partial class fKHManage : Form
    {
        public fKHManage()
        {
            InitializeComponent();
        }
        DateTime Tungay = DateTime.Now;
        DateTime Denngay = DateTime.Now;
        Database db = Database.NewDataDatabase();
        DataSingle dmKH;
        BindingSource bsDMKH = new BindingSource();
        string MaKH = "";
        private void fKHManage_Load(object sender, EventArgs e)
        {
            dmKH = new DataSingle("dmKH", "7");
            dmKH.GetData();
            bsDMKH.DataSource = dmKH.DsData.Tables[0];
            bsDMKH.CurrentItemChanged += BsDMKH_CurrentItemChanged;
            FormDesigner designer = new FormDesigner(dmKH);
            GridControl gc = designer.GenGridControl(dmKH.DsStruct.Tables[0], false, DockStyle.Fill);
            panelControl9.Controls.Add(gc);
            gc.DataSource = bsDMKH;
        }

        private void BsDMKH_CurrentItemChanged(object sender, EventArgs e)
        {
            if (bsDMKH.Current != null)
                MaKH = (bsDMKH.Current as DataRowView)["MaKH"].ToString();
            if (ckCAuto.Checked)
            {
                tbGetSumary_Click(sender, e);
            }
        }
        #region tbSumary


        private void tbGetSumary_Click(object sender, EventArgs e)
        {
            if (MaKH == null) return;
            DataTable tbSumary = db.GetDataSetByStore("CRM_getInfoKH", new string[] { "@MaKH" }, new object[] { MaKH });
            if (tbSumary == null || tbSumary.Rows.Count == 0) return;
            //set value for label
            tbTrangThaiKH.Text = tbSumary.Rows[0]["TrangThaiKH"].ToString();
            tbLastGD.Text = DateTime.Parse( tbSumary.Rows[0]["LastGD"].ToString()).ToString("dd/MM/yyyy");
            tbLastSale.Text = DateTime.Parse(tbSumary.Rows[0]["LastSale"].ToString()).ToString("dd/MM/yyyy");
            tbLastTT.Text = DateTime.Parse(tbSumary.Rows[0]["LastTT"].ToString()).ToString("dd/MM/yyyy");
            DataTable tbDSThang= db.GetDataSetByStore("CRM_getDSThang", new string[] { "@MaKH", "@Tungay", "@Denngay"}, new object[] { MaKH, Tungay, Denngay });
            ChartDSThang.DataSource = tbDSThang.DefaultView;
            ChartDSThang.Series[0].ArgumentDataMember = "thangnam";
            ChartDSThang.Series[0].ValueDataMembers.AddRange(new string[] { "DT" });

        }
        #endregion
        private void vDateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Tungay = DateTime.Parse( vDateEdit1.EditValue.ToString());
        }

        private void vDateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            Denngay = DateTime.Parse(vDateEdit2.EditValue.ToString());
        }

    }
}

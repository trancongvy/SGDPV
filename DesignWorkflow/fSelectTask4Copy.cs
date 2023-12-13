using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CDTDatabase;
namespace DesignWorkflow
{
    public partial class fSelectTask4Copy : DevExpress.XtraEditors.XtraForm
    {
        CDTDatabase.Database dbStruct = Database.NewStructDatabase();
        BindingSource bs = new BindingSource();
        DataTable tbTask;
       public string TaskID;
        public fSelectTask4Copy()
        {
            InitializeComponent();
            this.Disposed += FSelectTask4Copy_Disposed;
        }

        private void FSelectTask4Copy_Disposed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (TaskID == null)
                this.DialogResult = DialogResult.Cancel;
            else this.DialogResult = DialogResult.OK;
        }

        private void Bs_CurrentItemChanged(object sender, EventArgs e)
        {
            if (bs.Current != null)
                TaskID = (bs.Current as DataRowView).Row["ID"].ToString();
        }

        private void fSelectTask4Copy_Load(object sender, EventArgs e)
        {
            string sql = "select a.ID, a.TaskLabel, b.WFName, c.TableName, c.DienGiai  from systask a " +
                    " inner join sysWF b on a.WFID = b.ID " +
                    " inner join systable c on b.systableid = c.systableid";
            tbTask = dbStruct.GetDataTable(sql);
            bs.DataSource = tbTask;
            gridControl2.DataSource = bs;
            bs.CurrentItemChanged += Bs_CurrentItemChanged;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
namespace DesignWorkflow
{

    public partial class fUserAction : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewStructDatabase();
        DataTable tbUser;
        DataTable tbUserGr;
        public DataTable Data;
        public DataTable DataGr;
        BindingSource bs;
        BindingSource bsGr;
        Guid ID;
        public fUserAction(DataTable _data, DataTable _dataGr, Guid ActionID)
        {
            InitializeComponent();
            tbUser = db.GetDataTable("select * from sysUser");
            tbUserGr = db.GetDataTable("select * from sysUserGroup");
            Data = _data; DataGr = _dataGr;ID = ActionID;
            gridControl1.KeyUp += new KeyEventHandler(gridControl1_KeyUp);
            gridControl2.KeyUp += new KeyEventHandler(gridControl2_KeyUp);
        }

        void gridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                if (bs != null && bs.Current != null)
                {
                    bs.RemoveCurrent();
                    Data.AcceptChanges();
                }

            }
        }
        void gridControl2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                if (bsGr != null && bsGr.Current != null)
                {
                    bsGr.RemoveCurrent();
                    DataGr.AcceptChanges();
                }

            }
        }

        private void fUserTask_Load(object sender, EventArgs e)
        {
            bs = new BindingSource(Data, Data.TableName);
            repositoryItemLookUpEdit1.DataSource =tbUser;
            gridControl1.DataSource = bs;
            bsGr = new BindingSource(DataGr, DataGr.TableName);
            repositoryItemLookUpEdit2.DataSource = tbUserGr;
            gridControl2.DataSource = bsGr;    
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            fSelectAction4Copy fSelect = new fSelectAction4Copy();
            if (fSelect.ShowDialog() == DialogResult.OK)
            {
                Guid fromAction = Guid.Parse(fSelect.ActionID);
                DataTable tbAction = getSecurity(fromAction);
                foreach (DataRow fromRow in tbAction.Rows)
                {
                    DataRow dr = Data.NewRow();
                    foreach (DataColumn col in tbAction.Columns)
                    { dr[col.ColumnName] = fromRow[col.ColumnName]; }
                    Data.Rows.Add(dr);
                }
                DataTable tbGrAction = getSecurityGr(fromAction);
                foreach (DataRow fromRow in tbGrAction.Rows)
                {
                    DataRow dr = DataGr.NewRow();
                    foreach (DataColumn col in tbGrAction.Columns)
                    { dr[col.ColumnName] = fromRow[col.ColumnName]; }
                    DataGr.Rows.Add(dr);
                }
            }

        }
        public DataTable getSecurity(Guid id)
        {
            string sql = "select NULL as ID, sysUserID,'" +ID.ToString() + "' as ActionID, CAllow, GetMail  from sysUserAction where ActionID='" + id.ToString() + "'";
            DataTable tb = db.GetDataTable(sql);      
            return tb;
        }
        public DataTable getSecurityGr(Guid id)
        {
            string sql = "select 		NULL as ID, 	 sysUserGroupID,'" + ID.ToString() + "' as ActionID, CAllow, GetMail  from sysUserGrAction where ActionID='" + id.ToString() + "'";
            DataTable tb = db.GetDataTable(sql);

            return tb;
        }
    }
}
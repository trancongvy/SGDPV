using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DesignWorkflow
{
    public partial class fPara : DevExpress.XtraEditors.XtraForm
    {
        Designflow.Action A;
        BindingSource bs;
        public DataTable Data;
        public fPara(Designflow.Action _Action)
        {
            InitializeComponent();
            A = _Action;
            Data = A.tbPara;            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void fPara_Load(object sender, EventArgs e)
        {
            bs = new BindingSource(Data, Data.TableName);
            gridControl1.DataSource = bs;
            gridControl1.KeyUp += new KeyEventHandler(gridControl1_KeyUp);
        }

        private void gridControl1_KeyUp(object sender, KeyEventArgs e)
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
    }
}

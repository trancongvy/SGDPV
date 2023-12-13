using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDTSystem
{
    public partial class fTextResult : Form
    {
        public fTextResult(string name)
        {
            InitializeComponent();
            labelControl1.Text = name;
        }
        public string result = "";
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            result = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

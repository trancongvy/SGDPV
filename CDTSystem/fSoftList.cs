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
using Microsoft.Win32;
using DevExpress.XtraLayout;
namespace CDTSystem
{
    public partial class fSoftList : XtraForm
    {
        public fSoftList()
        {
            InitializeComponent();

            RegistryKey HKey = Registry.CurrentUser.OpenSubKey(@"Software\SGD\");
            if (HKey == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Software\SGD\");
                HKey = Registry.CurrentUser.OpenSubKey(@"Software\SGD\");
            }
            SoftList = HKey.GetSubKeyNames();
        }
        public string[] SoftList;
        public string Productname = "";
        private void fSoftList_Load(object sender, EventArgs e)
        {
           // if (SoftList.Length == 0) btCreate.Visible = true;
            foreach(string k in SoftList)
            {
                SimpleButton tb = new SimpleButton();
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SGD\" + k);
                try
                { if (k.Substring(0, 3) == "CBA")
                    {
                        string CompanyName = key.GetValue("CompanyName").ToString();
                        tb.Text = CompanyName + ", Dữ liệu: " + k;
                        tb.ToolTip = k;
                        LayoutControlItem it = new LayoutControlItem(this.layoutControl1, tb);
                        it.TextVisible = false;
                        tb.Click += Tb_Click;
                        tb.MouseUp += Tb_MouseUp;
                    }
                }
                catch { }
                
            }
        }

        private void Tb_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right && ModifierKeys == Keys.Control)
            {
                if (MessageBox.Show("Xóa đường gói phần mềm này?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    RegistryKey HKey = Registry.CurrentUser.OpenSubKey(@"Software\SGD\",true);
                    HKey.DeleteSubKeyTree((sender as SimpleButton).ToolTip, false);
                    LayoutControlItem it = layoutControl1.GetItemByControl((sender as SimpleButton));
                    if (it != null) it.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
        }

        private void Tb_Click(object sender, EventArgs e)
        {
            Productname = (sender as SimpleButton).ToolTip;
            if(Productname!=string.Empty) this.DialogResult = DialogResult.OK;
            
        }



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fTextResult f = new fTextResult("Gõ tên dữ liệu mới, không trùng với dữ liệu hiện có");
            DialogResult da = f.ShowDialog();
            string CompanyName = "CBA" +  f.result;
            if (da == DialogResult.OK && f.result != string.Empty && !SoftList.Contains(CompanyName))
            {
                SimpleButton tb = new SimpleButton();
                tb.Text = CompanyName + ", Dữ liệu: " + CompanyName;
                tb.ToolTip = CompanyName;
                LayoutControlItem it = new LayoutControlItem(this.layoutControl1, tb);
                it.TextVisible = false;
                tb.Click += Tb_Click;
                List<string> s = SoftList.ToList();
                s.Add(f.result);
                SoftList = s.ToArray();
                string P_KEY = @"Software\SGD\" + f.result;
                RegistryKey key = Registry.CurrentUser.CreateSubKey(P_KEY);
                key.SetValue("CompanyName", "SGD", RegistryValueKind.String);
                key.SetValue("CompanyName", "SGD", RegistryValueKind.String);
                key.SetValue("Created", "0", RegistryValueKind.DWord);
                key.SetValue("isDemo", "0", RegistryValueKind.DWord);
                key.SetValue("Language", "0", RegistryValueKind.DWord);
                key.SetValue("Package", "7", RegistryValueKind.String);
                key.SetValue("isOnline", "0", RegistryValueKind.DWord);
                key.SetValue("Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70", RegistryValueKind.ExpandString);
                key.SetValue("RegisterNumber", "", RegistryValueKind.String);
                key.SetValue("SavePassword", "True", RegistryValueKind.String);
                key.SetValue("StructDb", "SGD", RegistryValueKind.String);
                key.SetValue("RemoteServer", "SGD", RegistryValueKind.String);
                key.SetValue("Style", "Money Twins", RegistryValueKind.String);
                key.SetValue("SupportOnline", "SGD", RegistryValueKind.String);
                key.SetValue("UserName", "Admin", RegistryValueKind.String);
                key.SetValue("isRemote", "False", RegistryValueKind.String);
                key.SetValue("SoftType", "0", RegistryValueKind.DWord);
            }
            
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}

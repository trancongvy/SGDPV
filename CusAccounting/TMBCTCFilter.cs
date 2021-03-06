using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using System.Diagnostics;
using System.IO;

namespace CusAccounting
{
    public partial class TMBCTCFilter : DevExpress.XtraEditors.XtraForm
    {
        DateTime ngayct1;
        DateTime ngayct2;
        private string _file = (Config.GetValue("DuongDanBaoCao") + @"\" + Config.GetValue("Package").ToString() + @"\TMBCTC.xls");

        public TMBCTCFilter()
        {
            InitializeComponent();
            try
            {
                ngayct1 = vDateEdit1.DateTime;
                ngayct2 = vDateEdit2.DateTime;
            }
            catch { }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Config.NewKeyValue("@NgayCT1", ngayct1);
            Config.NewKeyValue("@NgayCT2", ngayct2);
            ngayct1 = vDateEdit1.DateTime;
            ngayct2 = vDateEdit2.DateTime;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel file (*.xls)|*.xls";
            dialog.AddExtension = true;
            dialog.ShowDialog();
            string fileName = dialog.FileName;
            if (fileName != string.Empty)
            {
                ExcelReport report = new ExcelReport(this._file, fileName,ngayct1, ngayct2);
                report.FillData();
                string key = "Chưa hoàn thành thuyết minh báo cáo tài chính! \nvui lòng kiểm tra số liệu trước khi xuất ra!";
               
                if (report.IsError)
                {
                    XtraMessageBox.Show(key);
                }
                Process.Start(fileName);
                //base.Close();

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (File.Exists(this._file))
            {
                Process.Start(this._file);
            }
            else
            {
                string key = "Không tìm thấy file mẫu!";
                if (Config.GetValue("Language").ToString() == "1")
                {
                    //key = UIDictionary.Translate(key);
                }
                XtraMessageBox.Show(key);
            }

        }
    }
}
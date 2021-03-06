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
    public partial class ExportVatFilter : DevExpress.XtraEditors.XtraForm
    {
        DateTime ngayct1;
        DateTime ngayct2;
        private string _fileBR = (Config.GetValue("DuongDanBaoCao") + @"\" + Config.GetValue("Package").ToString() + @"\BangkeBanra1.xls");
        private string _fileMV = (Config.GetValue("DuongDanBaoCao") + @"\" + Config.GetValue("Package").ToString() + @"\BangkeMuavao.xls");

        public ExportVatFilter()
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
                BangkeBanra report = new BangkeBanra(this._fileBR, fileName,ngayct1, ngayct2);
                report.AddDatatoFile();
                string key = "Chưa hoàn thành kết xuất dữ liệu thuế GTGT bán ra! \nvui lòng kiểm tra số liệu trước khi xuất ra!";
               
                if (report.IsError)
                {
                    XtraMessageBox.Show(key);
                }
                Process.Start(fileName);
                //base.Close();

            }

        }
        private void simpleButton3_Click(object sender, EventArgs e)
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
                BangkeMuavao report = new BangkeMuavao(this._fileMV, fileName, ngayct1, ngayct2);
                report.AddDatatoFile();
                string key = "Chưa hoàn thành kết xuất dữ liệu thuế GTGT mua vào! \nvui lòng kiểm tra số liệu trước khi xuất ra!";

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
            if (File.Exists(this._fileBR))
            {
                Process.Start(this._fileBR);
            }
            else
            {
                string key = "Không tìm thấy file mẫu GTGT bán ra!";
                if (Config.GetValue("Language").ToString() == "1")
                {
                    //key = UIDictionary.Translate(key);
                }
                XtraMessageBox.Show(key);
            }
            if (File.Exists(this._fileMV))
            {
                Process.Start(this._fileMV);
            }
            else
            {
                string key = "Không tìm thấy file mẫu GTGT mua vào!";
                if (Config.GetValue("Language").ToString() == "1")
                {
                    //key = UIDictionary.Translate(key);
                }
                XtraMessageBox.Show(key);
            }
        }

        private void ExportVatFilter_Load(object sender, EventArgs e)
        {

        }

        
    }
}
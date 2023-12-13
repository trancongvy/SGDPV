using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
namespace DevControls
{
    public partial class ImageContener : PictureEdit
    {
        public ImageContener(DataRow drf)
        {
            InitializeComponent();

            //simpleButton1.BringToFront();
            drField = drf;
        }
        public byte[] data;
        public DataRow drField;
        public bool updateNew = false;

        //private void simpleButton1_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog OFdg = new OpenFileDialog();
        //    OFdg.ShowDialog();
        //    if (OFdg.FileName != null)
        //    {
        //        this.Text = OFdg.FileName;

        //        FileStream fs = new FileStream(this.Text, FileMode.Open, FileAccess.Read);
        //        BinaryReader br = new BinaryReader(fs);
        //        long numBytes = new FileInfo(this.Text).Length;
        //       data = br.ReadBytes((int)numBytes);
        //       updateNew = true;
        //       this.RaiseLoadnewData(this, e);
        //    }
        //}

       


    }
}

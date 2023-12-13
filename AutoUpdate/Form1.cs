using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Globalization;

namespace AutoUpdate
{
    public partial class Form1 : Form
    {
        string _path;
        public Form1(string path)
        {
            InitializeComponent();
            _path = path;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = _path;
        }
        private bool UpdatePath(string p, string dir)
        {
            bool updateComplete = true;
            updateComplete= UpdateFile(p,dir);
            try
            {
                string[] drs = Directory.GetDirectories(p);

                foreach (string d in drs)
                {

                    string drec = Path.GetDirectoryName(d);
                    drec = dir + d.Replace(drec, "");

                    richTextBox1.Text += "\n to :" + drec;
                    if (!Directory.Exists(drec)) Directory.CreateDirectory(drec);
                    UpdatePath(d, drec);
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return updateComplete;
        }
        private bool UpdateFile(string p, string dir)
        {
            if (!Directory.Exists(p)) return false;
            string[] files = Directory.GetFiles(p);
            bool updateComplete = true;
            foreach (string f in files)
            {
                string fName = Path.GetFileName(f);
                richTextBox1.Text += "\n Updating file .... " + fName;
                try
                {
                    File.Copy(f, dir + "\\" + fName, true);
                }
                catch (Exception e)
                {
                    richTextBox1.Text += "\n" + e.Message;
                    //updateComplete = false;
                }
            }
            
            richTextBox1.Text += "\n Update complete " + dir;
            richTextBox1.ScrollToCaret();
            return updateComplete;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != string.Empty)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _path = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int milliseconds = 2000;
            Thread.Sleep(milliseconds);
            richTextBox1.Text = "Update from: " + _path;
            richTextBox1.Text += "\n to :" + Application.StartupPath;
            //if (!Directory.Exists(_path)) return;
            //UpdatePath(_path, Application.StartupPath);

            if (!Directory.Exists(Application.StartupPath + @"\UpdateCode"  ))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\UpdateCode");
                //Download về

            }
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                label3.Text = "Downloading...";
                wc.DownloadFileAsync(
                    new System.Uri(_path +@"BPM.zip"), Application.StartupPath + @"\UpdateCode\BPM.zip"
                );
            }
            
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label3.Text = "Extracting...";
            progressBar1.Value = 0;
            string startPath = Application.StartupPath + @"\UpdateCode\";
            string zipPath = Application.StartupPath + @"\UpdateCode\BPM.zip";
            string extractPath = Application.StartupPath + @"\UpdateCode\BPM\";
            int milliseconds = 2000;
            Thread.Sleep(milliseconds);
            // System.IO.Compression.ZipFile.CreateFromDirectory(startPath, "BPM.rar");
            try
            {

                if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);

            }
            catch { }
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            //Copy to 
            Thread.Sleep(milliseconds);
           if( UpdatePath(extractPath, Application.StartupPath))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(extractPath);
                setAttributesNormal(dir);
                try { Directory.Delete(extractPath, true); } catch { }
                //Update thành công
                string path = Application.StartupPath + "\\UpdateDate.txt";
                File.WriteAllText(path, DateTime.Today.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                this.Dispose();
            }

            //if (UpdateFile(extractPath, Application.StartupPath))
            //{
            //    UpdatePath(extractPath, Application.StartupPath);
            //    if (UpdateFile(extractPath + @"\Plugins\CBABPM\", Application.StartupPath + @"\Plugins\CBABPM"))
            //    {
            //        if (UpdateFile(extractPath + @"\LoginImage\", Application.StartupPath + @"\LoginImage"))
            //        {
                        
            //        }
            //        else
            //        {
            //            //update fail
            //        }
            //    }
            //}
        }
        private void setAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
                setAttributesNormal(subDir);
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using ErrorManager;
using System.Reflection;

namespace GetHash
{
    public partial class GetHash : Form
    {
        public GetHash()
        {
            InitializeComponent();
        }
        static string ComputeSHA256(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2")); // format hex

                return sb.ToString();
            }
        }
        static string ComputeSHA1(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
                
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2")); // format hex

                return sb.ToString();
            }
        }
        static string ComputeMD5(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2")); // format hex

                return sb.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string filepath = "";
            FolderBrowserDialog dD = new FolderBrowserDialog();
            //dD.ShowDialog();
            if (dD.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dD.SelectedPath;
                //GhiHash(textBox1.Text);
            }
            else { }

            //    OpenFileDialog dg = new OpenFileDialog();
            //dg.Filter = "Exefiles (*.exe)|*.exe| dllFile (*.dll)|*.dll";
            //dg.ShowDialog();
            //if(dg.FileName==null || dg.FileName == string.Empty)
            //{
            //    return;
            //}
            //filepath = dg.FileName;
            // textBox1.Text = filepath;

        }
        private void GhiHash(string folderPath)
        {
            //string folderPath = @"C:\Your\Target\Directory"; // Thay bằng đường dẫn của bạn

            if (Directory.Exists(folderPath))
            {
                // Lấy file .exe và .dll (chỉ trong thư mục hiện tại)
                var exeFiles = Directory.GetFiles(folderPath, "*.exe", SearchOption.TopDirectoryOnly);
                var dllFiles = Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly);

                var allFiles = exeFiles.Concat(dllFiles);
                
                LogFile.DeleteFile("HashFile.txt");
                LogFile.CreateFile("HashFile.txt");
                foreach (string file in allFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        var companyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                        var productAttr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
                        if (companyAttr?.Company.ToLower() == "SGD ltd,co".ToLower())
                        {
                            LogFile.AppendToFile("HashFile.txt", assembly.GetName().Name);
                            LogFile.AppendToFile("HashFile.txt", "SHA256: " + ComputeSHA256(file));
                            LogFile.AppendToFile("HashFile.txt", "SHA1: " + ComputeSHA1(file));
                            LogFile.AppendToFile("HashFile.txt", "MD5: " + ComputeMD5(file));
                        }
                    }
                    catch { }
                }
            }
        }

        private void GetHash_Load(object sender, EventArgs e)
        {
            textBox1.Text = Application.StartupPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GhiHash(textBox1.Text);
        }
    }
}

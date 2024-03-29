using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using System.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using CDTControl;
namespace CDTSystem
{
    public partial class fCopyPackage : DevExpress.XtraEditors.XtraForm
    {
        public fCopyPackage()
        {
            InitializeComponent();
        }
        Database _db = Database.NewDataDatabase();
        Database _dbStruct = Database.NewStructDatabase();
        string PackageName;
        string newPackageName = string.Empty;
        string newCDTName = string.Empty;
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (textEdit1.Text == string.Empty) return;
            newPackageName = "CBA" + textEdit1.Text;
            newCDTName = "CDT" + textEdit1.Text;
            if (newPackageName == PackageName)
            {
                MessageBox.Show("Trùng phần mềm");
                return;
            }
            //Tạo regedit 

            if (!checkBox1.Checked)
            {
                if (!copyDatabase())
                {
                    MessageBox.Show("Lỗi khi Copy");
                    return;
                }
                else
                {
                    if (updateScript())
                        MessageBox.Show("Copy Thành Công");
                    else
                    {
                        MessageBox.Show("Update Script không thành công");
                    }
                }
            }

            CreateRegedit();
            //Copy thư mục layout, plugins, reports
            copyDirectory();
        }

        private bool updateScript()
        {
            string con = Config.GetValue("DataConnection").ToString(); 
            Database newdbStruct = Database.NewCustomDatabase(con.Replace(PackageName, newCDTName));
           // newdbStruct = Database.NewDataDatabase();
            if(newdbStruct!=null)
            {
                string sql="select * from sysAction";
                DataTable tb=newdbStruct.GetDataTable(sql);

                foreach (DataRow dr in tb.Rows)
                {
                    string command = dr["Command"].ToString();
                    command = command.ToUpper().Replace(PackageName, newCDTName);
                    sql = "Update sysAction set Command=@Command where ID='" + dr["ID"].ToString() + "'";
                    string afterUpdate = dr["afterUpdate"].ToString();
                    afterUpdate = afterUpdate.ToUpper().Replace(PackageName, newCDTName);
                    
                    string sql1;
                    sql1 = "Update sysAction set afterUpdate=@afterUpdate where ID='" + dr["ID"].ToString() + "'";
                    newdbStruct.BeginMultiTrans();
                    try
                    {
                        if (command != string.Empty)
                            newdbStruct.UpdateDatabyPara(sql, new string[] { "Command" }, new object[] { command });
                        if (afterUpdate != string.Empty)
                            newdbStruct.UpdateDatabyPara(sql1, new string[] { "afterUpdate" }, new object[] { afterUpdate });
                    }
                    catch (Exception ex)
                    {
                        newdbStruct.RollbackMultiTrans();
                        return false;
                    }
                    finally
                    {
                        newdbStruct.EndMultiTrans();
                    }

                }
                sql = "select * from sysField where QueryInsertDt is not null";
                tb = newdbStruct.GetDataTable(sql);
                foreach (DataRow dr in tb.Rows)
                {
                    string command = dr["QueryInsertDt"].ToString();
                    command = command.Replace(PackageName, newCDTName);

                    sql = "Update sysField set QueryInsertDt=@command where sysFieldID=" + dr["sysFieldID"].ToString();
                    newdbStruct.BeginMultiTrans();
                    try
                    {
                        newdbStruct.UpdateDatabyPara(sql, new string[] { "command" }, new object[] { command });
                    }
                    catch(Exception ex)
                    {
                        newdbStruct.RollbackMultiTrans();
                        return false;
                    }
                    finally
                    {
                        newdbStruct.EndMultiTrans();
                    }
                    if (newdbStruct.HasErrors) newdbStruct.HasErrors = false;
                }
            }
            return true;
        }

        private void fCopyPackage_Load(object sender, EventArgs e)
        {
            PackageName = Config.GetValue("Package").ToString();
        }
        private void CreateRegedit()
        {
            string H_KEY = "HKEY_CURRENT_USER\\Software\\SGD\\" + newPackageName +"\\";
            Registry.SetValue(H_KEY, "CompanyName","SGD");
            Registry.SetValue(H_KEY, "Created", 0);
            Registry.SetValue(H_KEY, "isDemo", 0);
            Registry.SetValue(H_KEY, "Language", 0);
            Registry.SetValue(H_KEY, "Package", 7);
            Registry.SetValue(H_KEY, "Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70");
            Registry.SetValue(H_KEY, "RegisterNumber", "");
            Registry.SetValue(H_KEY, "SavePassword", "True");
            Registry.SetValue(H_KEY, "StructDb", "");
            Registry.SetValue(H_KEY, "RemoteServer", "SGD", RegistryValueKind.String);
            Registry.SetValue(H_KEY, "Style", "Money Twins");
            Registry.SetValue(H_KEY, "SupportOnline", "False");
            Registry.SetValue(H_KEY, "Username", "Admin");
            Registry.SetValue(H_KEY, "SoftType", "0", RegistryValueKind.DWord);
            H_KEY = "HKEY_CURRENT_USER\\Software\\SGD\\" + newCDTName + "\\";
            Registry.SetValue(H_KEY, "CompanyName", "SGD");
            Registry.SetValue(H_KEY, "Created", 0);
            Registry.SetValue(H_KEY, "isDemo", 0);
            Registry.SetValue(H_KEY, "Language", 0);
            Registry.SetValue(H_KEY, "Package", 7);
            Registry.SetValue(H_KEY, "Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70");
            Registry.SetValue(H_KEY, "RegisterNumber", "");
            Registry.SetValue(H_KEY, "SavePassword", "True");
            Registry.SetValue(H_KEY, "StructDb", "");
            Registry.SetValue(H_KEY, "RemoteServer", "SGD", RegistryValueKind.String);
            Registry.SetValue(H_KEY, "Style", "Money Twins");
            Registry.SetValue(H_KEY, "SupportOnline", "False");
            Registry.SetValue(H_KEY, "Username", "Admin");
            Registry.SetValue(H_KEY, "SoftType", "0", RegistryValueKind.DWord);

        }
        private void CreateShotcut()
        {
            string path = Application.StartupPath;
            WshShellClass wsh = new WshShellClass();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + newPackageName + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = "\"" + newPackageName+ "\"" ;
           
            shortcut.TargetPath = path + "\\CDT.exe";
            // not sure about what this is for
            shortcut.WindowStyle = 1;
            shortcut.Description = newPackageName;
            shortcut.WorkingDirectory = path;
            shortcut.Save();
        }
        private bool copyDatabase()
        {
           // PackageName = Config.GetValue("Package").ToString();
            try
            {
                //backup
                if (!System.IO.Directory.Exists(Application.StartupPath + "\\Backup\\"))
                    System.IO.Directory.CreateDirectory(Application.StartupPath + "\\Backup\\");
                string backupDataDb = "backup database " + PackageName + " to DISK = '" + Application.StartupPath + "\\Backup\\" + PackageName + DateTime.Now.ToString("dd_MM_yyyy") + ".dat' with init";
                bool re1=_db.UpdateByNonQueryNoTrans(backupDataDb);
                backupDataDb = "backup database " + PackageName.Replace("CDT", "CBA") + " to DISK = '" + Application.StartupPath + "\\Backup\\" + PackageName.Replace("CDT", "CBA") + DateTime.Now.ToString("dd_MM_yyyy") + ".dat' with init";
                _dbStruct.UpdateByNonQueryNoTrans(backupDataDb);


                DataMaintain dmRtas = new DataMaintain();
                bool re;
                re = dmRtas.RestoreDataToAnother(Application.StartupPath + "\\Data2005\\", Application.StartupPath + "\\Backup\\" + PackageName + DateTime.Now.ToString("dd_MM_yyyy") + ".dat", newCDTName);
                re = re && dmRtas.RestoreDataToAnother(Application.StartupPath + "\\Data2005\\", Application.StartupPath + "\\Backup\\" + PackageName.Replace("CDT","CBA") + DateTime.Now.ToString("dd_MM_yyyy") + ".dat", newPackageName);
                //update Script
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        private void copyDirectory()
        {
            DirectoryCopy(Application.StartupPath + "\\Layouts\\" + PackageName.Replace("CDT", "CBA"), Application.StartupPath + "\\Layouts\\" + newPackageName, true);
            DirectoryCopy(Application.StartupPath + "\\Plugins\\" + PackageName.Replace("CDT", "CBA"), Application.StartupPath + "\\Plugins\\" + newPackageName, true);
            DirectoryCopy(Application.StartupPath + "\\Reports\\" + PackageName.Replace("CDT", "CBA"), Application.StartupPath + "\\Reports\\" + newPackageName, true);
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath,true);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
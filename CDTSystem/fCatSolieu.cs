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
    public partial class fCatSolieu : DevExpress.XtraEditors.XtraForm
    {
        public fCatSolieu()
        {
            InitializeComponent();
        }
        Database _db = Database.NewDataDatabase();
        Database _dbStruct = Database.NewStructDatabase();

        string newCDTName = string.Empty;
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //Các bước chốt số liệu
            //1. Kiểm tra dữ liệu đích: Đã trùng tên chưa    

            DateTime NgayChot = cDateEdit1.DateTime;
            string newCDTDataName = "";
            //2. Kiểm tra trong sysDB có bao nhiêu database
            DataTable lstData = _dbStruct.GetDataTable("SELECT a.syspackageid, a.DatabaseName, b.Package, a.sysDBID   FROM sysDB a inner join syspackage b on a.syspackageid=b.syspackageid");
            foreach(DataRow dr in lstData.Rows)
            {
                string DbName = dr["DatabaseName"].ToString();

                string Package= dr["Package"].ToString();
                int ii = Package.IndexOf("_");
                if (ii > 0) Package = Package.Substring(0, ii);
                string NewDbName = DbName;
                NewDbName = NewDbName.Replace(Package, "");
                int ix = NewDbName.IndexOf("_");
                if (ix >= 0) NewDbName = NewDbName.Substring(0, ix);//remove cái đuôi năm đi
                 NewDbName = Package + NewDbName + "_" + (NgayChot.Year+1).ToString();
                //3.Đặt tên cho các database mới.
                string NewStructDB = _dbStruct.Connection.Database;
                ix = NewStructDB.IndexOf("_");
                if (ix >= 0) NewStructDB = NewStructDB.Substring(0,  ix);
                NewStructDB = NewStructDB + "_" + (NgayChot.Year + 1).ToString();
                if (!vCheckEdit1.Checked)
                {
                    object exits = _db.GetValue("select DB_ID('" + NewDbName + "') ");
                    if (exits != DBNull.Value) continue;
                    if (!copyDatabase(DbName, NewDbName))
                    {
                        MessageBox.Show("Lỗi khi Copy dữ liệu");
                        return;
                    }
                    else
                    {
                        if (Package.Contains("CDT"))//Dữ liệu struct
                        {
                            newCDTDataName = NewDbName;
                            if (!updateScript(DbName, NewDbName))
                            {
                                MessageBox.Show("Update Script không thành công");
                            }
                        }
                        else //dữ liệu Data
                        {
                            //Update sysDB
                            string sysDBID = dr["sysDBID"].ToString();
                            string s = "update " + newCDTDataName + ".dbo.sysDB set DatabaseName='" + NewDbName + "' where sysDBID=" + sysDBID;
                            _db.UpdateByNonQuery(s);
                            ChotSolieu(NgayChot, NewDbName);
                        }
                    }
                }
                //7.Tạo regedit
                //8.Tạo các foder
                string sCon = Config.GetValue("StructConnection").ToString();
                sCon = sCon.Replace(_dbStruct.Connection.Database, NewStructDB);
                CreateRegedit(NewDbName,sCon);
                //Copy thư mục layout, plugins, reports
                copyDirectory(DbName,NewDbName);
            }

        }

        private void ChotSolieu(DateTime ngayChot, string newDbName)
        {
            // a. Chốt số dư BLTK, BLVT=> table tạm
            // b. Chạy cript xóa số liệu <= ngày chốt
            // c. Đưa số dư vào lại OBTK, OBVT
            string sCon = Config.GetValue("DataConnection").ToString();
            sCon = sCon.Replace(_db.Connection.Database, newDbName);
            Database db = Database.NewCustomDatabase(sCon);
            if (db.UpdateDatabyStore("ChotSolieu", new string[] { "NgayCT" }, new object[] { ngayChot })){
                MessageBox.Show("Chốt số liệu " + newDbName + " thành công");
            }

        }

        private bool updateScript(string PackageName, string newCDTName)
        {
            string sCon = Config.GetValue("DataConnection").ToString();
            sCon = sCon.Replace(_db.Connection.Database, newCDTName);
            Database newdbStruct = Database.NewCustomDatabase(sCon);
           // newdbStruct = Database.NewDataDatabase();
            if(newdbStruct!=null)
            {
                //Update Connection
                
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
            
        }
        private void CreateRegedit(string newPackageName,string newScon)
        {
            newScon = Security.EnCode64(newScon);
            string H_KEY = "HKEY_CURRENT_USER\\Software\\SGD\\" + newPackageName +"\\";
            Registry.SetValue(H_KEY, "CompanyName","SGD");
            Registry.SetValue(H_KEY, "Created", 1);
            Registry.SetValue(H_KEY, "isDemo", 0);
            Registry.SetValue(H_KEY, "Language", 0);
            Registry.SetValue(H_KEY, "Package", 7);
            Registry.SetValue(H_KEY, "Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70");
            Registry.SetValue(H_KEY, "RegisterNumber", "");
            Registry.SetValue(H_KEY, "SavePassword", "True");
            Registry.SetValue(H_KEY, "StructDb", newScon);
            Registry.SetValue(H_KEY, "RemoteServer", newScon);
            Registry.SetValue(H_KEY, "Style", "Money Twins");
            Registry.SetValue(H_KEY, "SupportOnline", "False");
            Registry.SetValue(H_KEY, "Username", "Admin");
            Registry.SetValue(H_KEY, "SoftType", "0", RegistryValueKind.DWord);           

        }
        private void CreateShotcut(string newPackageName)
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
        private bool copyDatabase(string Dbname, string NewName)
        {
           // PackageName = Config.GetValue("Package").ToString();
            try
            {
                //backup
                if (!System.IO.Directory.Exists(Application.StartupPath + "\\Backup\\"))
                    System.IO.Directory.CreateDirectory(Application.StartupPath + "\\Backup\\");
                string backupDataDb = "backup database " + Dbname + " to DISK = '" + Application.StartupPath + "\\Backup\\" + Dbname + DateTime.Now.ToString("dd_MM_yyyy") + ".dat' with init";
                _db.UpdateByNonQueryNoTrans(backupDataDb);
                //backupDataDb = "backup database " + PackageName.Replace("CDT", "CBA") + " to DISK = '" + Application.StartupPath + "\\Backup\\" + PackageName.Replace("CDT", "CBA") + DateTime.Now.ToString("dd_MM_yyyy") + ".dat' with init";
                //_dbStruct.UpdateByNonQueryNoTrans(backupDataDb);


                DataMaintain dmRtas = new DataMaintain();
                bool re;
                re = dmRtas.RestoreDataToAnother(Application.StartupPath + "\\Data2005\\", Application.StartupPath + "\\Backup\\" + Dbname + DateTime.Now.ToString("dd_MM_yyyy") + ".dat", NewName);
               // re = re && dmRtas.RestoreDataToAnother(Application.StartupPath + "\\Data2005\\", Application.StartupPath + "\\Backup\\" + PackageName.Replace("CDT","CBA") + DateTime.Now.ToString("dd_MM_yyyy") + ".dat", newPackageName);
                //update Script
                

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        private void copyDirectory(string PackageName, string newPackageName)
        {
            DirectoryCopy(Application.StartupPath + "\\Layouts\\" + PackageName, Application.StartupPath + "\\Layouts\\" + newPackageName, true);
            DirectoryCopy(Application.StartupPath + "\\Plugins\\" + PackageName, Application.StartupPath + "\\Plugins\\" + newPackageName, true);
            DirectoryCopy(Application.StartupPath + "\\Reports\\" + PackageName, Application.StartupPath + "\\Reports\\" + newPackageName, true);
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists) return;
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
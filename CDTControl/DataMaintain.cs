using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using CDTDatabase;
using System.Data.SqlClient;
using CDTLib;
using System.IO;
using CDTControl;
using System.Data;
namespace CDTControl
{
    public class DataMaintain
    {
        private string H_KEY = Config.GetValue("H_KEY").ToString();
        private string _serverName = string.Empty;
        private string _RemoteServer = string.Empty;
        private string _databaseName = string.Empty;
        private int _cnnType;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        public bool isServer2005;
        public string Connection;
        public DataMaintain()
        {
            _databaseName = Config.GetValue("ProductName").ToString();
        }

        public DataMaintain(string serverName,string Remoteserver, int cnnType, string userName, string password)
        {
            _serverName = serverName;
            _RemoteServer = Remoteserver;
            _cnnType = cnnType;
            _databaseName = Config.GetValue("ProductName").ToString();
            _userName = userName;
            _password = password;
        }
        public void UpdateLocalServer(string ServerName,string Ver)
        {
            string str2 = "server = " + this._serverName + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            string originalString = "server=" + ServerName + "; User=" + this._userName + "; Password=" + this._password;
            Database database2 = Database.NewCustomDatabase(str2);
            database2.UpdateByNonQuery("update sysDB set DBPath='" + Security.EnCode64(originalString) + "'");
            Connection = str2;
            //string connection = "server = " + ServerName + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            //Registry.SetValue(this.H_KEY, "RemoteServer", Security.EnCode64(connection));
        }
        public void UpdateRemoteServer(string RemoteServerName, string Ver)
        {
            string str2 = "server = " + this._serverName + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            string originalString = "server=" + RemoteServerName + "; User=" + this._userName + "; Password=" + this._password;
            Database database2 = Database.NewCustomDatabase(str2);
            database2.UpdateByNonQuery("update sysDB set DBPathRemote='" + Security.EnCode64(originalString) + "'");
            string connection = "server = " + RemoteServerName + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            Registry.SetValue(this.H_KEY, "RemoteServer", Security.EnCode64(connection));
            Connection = str2;
        }
        public bool ClientExecute(string Ver)
        {
            string strConnection = "server = " + this._serverName + "; database = " + this._databaseName + "; User = " + this._userName + "; Password = " + this._password;
            string str2 = "server = " + this._serverName + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            string str2Remote = "server = " + this._RemoteServer + "; database =" + Ver + "; User = " + this._userName + "; Password = " + this._password;
            if (this._cnnType == 1)
            {
                strConnection = "server = " + this._serverName + "; database = " + this._databaseName + "; integrated security = true";
                str2 = "server = " + this._serverName + "; database = " + Ver + "; integrated security = true";
            }
           // Database database = Database.NewCustomDatabase(strConnection);
            Database database2 = Database.NewCustomDatabase(str2);
            
            try
            {
                database2.Connection.Open();
               // database.Connection.Open();
                string originalString = string.Empty;
                if (this._cnnType == 1)
                {
                    originalString = "server = " + this._serverName + "; integrated security = true";
                }
                else
                {
                    originalString = "server=" + this._serverName + "; User=" + this._userName + "; Password=" + this._password;
                }
                //database2.UpdateByNonQuery("update sysDB set DbPath='" + Security.EnCode64(originalString) + "'");
            }
            catch (Exception)
            {
                return false;

            }
            Connection = str2;
            Registry.SetValue(this.H_KEY, "StructDb", Security.EnCode64(str2));
            Registry.SetValue(this.H_KEY, "RemoteServer", Security.EnCode64(str2Remote));
            return true;
        }


        public bool ServerExecute(string dataFilePath, string Ver)
        {
            string str4;
            bool flag2;
            string str5;
            string strConnection = string.Empty;
            string strRemote = string.Empty;
            string cnn = string.Empty;
            string cnnRemote = string.Empty;
            string str3 = string.Empty;
            if (this._cnnType == 1)
            {
                strConnection = "server = " + this._serverName + "; integrated security = true";
                cnn = "server = " + this._serverName + "; database =" + Ver + "; integrated security = true";
                str3 = "server = " + this._serverName + "; database = " + this._databaseName + "; integrated security = true";
                strRemote = strConnection;
            }
            else
            {
                strConnection = "server = " + this._serverName + "; User = " + this._userName + "; Password = " + this._password;
                cnn = "server = " + this._serverName + "; database = " + Ver + "; User = " + this._userName + "; Password = " + this._password;
                str3 = "server = " + this._serverName + "; database = " + this._databaseName + "; User = " + this._userName + "; Password = " + this._password;
                strRemote = "server = " + this._RemoteServer + "; User = " + this._userName + "; Password = " + this._password;
                cnnRemote = "server = " + this._RemoteServer + "; database = " + Ver + "; User = " + this._userName + "; Password = " + this._password;
            }
            Database database = Database.NewCustomDatabase(strConnection);
            bool flag = this._databaseName != Ver;
            if (this.isServer2005)
            {
                str4 = "if not exists (SELECT name FROM master.dbo.sysdatabases WHERE name = '" + Ver + "') execute sp_attach_db @dbname = N'" + Ver + "',  @filename1 = N'" + dataFilePath + @"\Data2005\" + Ver + ".mdf',@filename2 = N'" + dataFilePath + @"\Data2005\" + Ver + "_log.ldf'";
                flag2 = File.Exists(@"Data2005\" + this._databaseName + ".mdf");
                str5 = "if not exists (SELECT name FROM master.dbo.sysdatabases WHERE name = '" + this._databaseName + "') execute sp_attach_db @dbname = N'" + this._databaseName + "',  @filename1 = N'" + dataFilePath + @"\Data2005\" + this._databaseName + ".mdf',@filename2 = N'" + dataFilePath + @"\Data2005\" + this._databaseName + "_log.ldf'";
            }
            else
            {
                str4 = "if not exists (SELECT name FROM master.dbo.sysdatabases WHERE name = '" + Ver + "') execute sp_attach_db @dbname = N'" + Ver + "',  @filename1 = N'" + dataFilePath + @"\Data\" + Ver + ".mdf',@filename2 = N'" + dataFilePath + @"\Data\" + Ver + "_log.ldf'";
                flag2 = File.Exists(@"Data\" + this._databaseName + ".mdf");
                str5 = "if not exists (SELECT name FROM master.dbo.sysdatabases WHERE name = '" + this._databaseName + "') execute sp_attach_db @dbname = N'" + this._databaseName + "',  @filename1 = N'" + dataFilePath + @"\Data\" + this._databaseName + ".mdf',@filename2 = N'" + dataFilePath + @"\Data\" + this._databaseName + "_log.ldf'";
            }
            bool flag3 = File.Exists(@"Data\" + this._databaseName + ".dat");
            if (((!flag || database.UpdateByNonQueryNoTrans(str4)) && (!flag2 || database.UpdateByNonQueryNoTrans(str5))) && this.UpdateImage(cnn))
            {
                Registry.SetValue(this.H_KEY, "StructDb", Security.EnCode64(cnn));
                this.UpdateDbPathForPackage(strConnection);
                Registry.SetValue(this.H_KEY, "RemoteServer", Security.EnCode64(cnnRemote));
                this.UpdateDbPathRemoteForPackage(strRemote);
                return true;
            }
            return false;
        }


        private bool UpdateImage(string cnn)
        {
            Database db = Database.NewCustomDatabase(cnn);
            string sql = string.Empty;
            string DirData = "Data";
            if (isServer2005) DirData += "2005";
            string[] paraNames = new string[] { "@Image", "@ID" };
            string[] iPaths = Directory.GetDirectories(DirData);
            foreach (string iPath in iPaths)
            {
                string[] sPath = iPath.Split("_".ToCharArray());
                string[] iFiles = Directory.GetFiles(iPath, "*.png");
                sql += "update " + sPath[1].Replace(DirData + "\\", "") + " set " + sPath[2] + " = @Image where " + sPath[3] + " = @ID";
                foreach (string iFile in iFiles)
                {
                    bool result = true;
                    FileInfo f = new FileInfo(iFile);
                    object[] paraValues = new object[] { File.ReadAllBytes(iFile), f.Name.Replace(".png", "") };
                    result = db.UpdateDatabyPara(sql, paraNames, paraValues);
                    if (!result)
                        return false;
                }
            }
            return true;
        }

        private void UpdateDbPathForPackage(string dataConnection)
        {
            string StructConnection = Registry.GetValue(H_KEY, "StructDb", string.Empty).ToString();
            StructConnection = Security.DeCode64(StructConnection);
            if (StructConnection == string.Empty)
                return;
            Database db = Database.NewCustomDatabase(StructConnection);
            string sql = "update sysDB set DbPath = '" + Security.EnCode64(dataConnection) + "'";// +"' where package = '" + _databaseName +
            db.UpdateByNonQuery(sql);
        }
        private void UpdateDbPathRemoteForPackage(string dataConnection)
        {
            string StructConnection = Registry.GetValue(H_KEY, "StructDb", string.Empty).ToString();
            StructConnection = Security.DeCode64(StructConnection);
            if (StructConnection == string.Empty)
                return;
            Database db = Database.NewCustomDatabase(StructConnection);
            string sql = "update sysDB set DbPathRemote = '" + Security.EnCode64(dataConnection) + "'";// +"' where package = '" + _databaseName +
            db.UpdateByNonQuery(sql);
        }
        public bool BackupData(string dataFilePath)
        {
            if (!System.IO.Directory.Exists(dataFilePath + "\\Backup\\"))
                System.IO.Directory.CreateDirectory(dataFilePath + "\\Backup\\");
            string backupDataDb = "backup database " + _databaseName + " to DISK = '" + dataFilePath + "\\Backup\\" + _databaseName + DateTime.Now.ToString("dd_MM_yyyy") + ".dat' with init";
            Database db = Database.NewDataDatabase();
            bool kq = db.UpdateByNonQueryNoTrans(backupDataDb);
            if (kq)
            {
                System.Diagnostics.ProcessStartInfo sf = new System.Diagnostics.ProcessStartInfo("Winrar.exe");
                string fileName = _databaseName + DateTime.Now.ToString("dd_MM_yyyy");
                sf.Arguments = string.Format("a {0} {1} -r ", fileName + ".rar", fileName + ".dat");
                sf.WorkingDirectory = System.Windows.Forms.Application.StartupPath + "\\BackUp";
                sf.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                // Process.Start(sf);

                using (System.Diagnostics.Process exeProcess = System.Diagnostics.Process.Start(sf))
                {
                    exeProcess.WaitForExit();
                }
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\BackUp\\" + fileName + ".dat");
                if (Config.Variables.Contains("BackupPath") && Config.GetValue("BackupPath").ToString() != string.Empty)
                {
                    //File.Copy(System.Windows.Forms.Application.StartupPath + "\\BackUp\\" + fileName + ".rar", Config.GetValue("BackupPath").ToString() + "\\" + fileName + ".rar", true);
                    //File.Delete(System.Windows.Forms.Application.StartupPath + "\\BackUp\\" + fileName + ".rar");
                }
            }
            return kq;

        }

        public bool RestoreData(string dataFilePath)
        {
            string restoreDataDb = "restore database " + _databaseName + " from DISK = '" + dataFilePath +"' with replace";
            Database db = Database.NewStructDatabase();
            return (db.UpdateByNonQueryNoTrans(restoreDataDb)) ;
        }
        public bool RestoreDataToAnother(string dataFilePath,string BackUpFilePath, string DataAnother)
        {
             Database db = Database.NewStructDatabase();
             DataTable listFile = db.GetDataTable("RESTORE FILELISTONLY    FROM DISK ='" + BackUpFilePath + "'");
             if(listFile.Rows.Count!=2) return false;
             string sql = "RESTORE DATABASE " + DataAnother + " from disk ='" + BackUpFilePath + "'  WITH   MOVE '" +
                    listFile.Rows[0]["LogicalName"].ToString() + "' TO '" + dataFilePath + "\\" + DataAnother + ".mdf' , MOVE '" +
                    listFile.Rows[1]["LogicalName"].ToString() + "' TO '" + dataFilePath + "\\" + DataAnother + "_log.ldf' , replace";
          bool re= db.UpdateByNonQueryNoTrans(sql);
          if (re&& DataAnother.IndexOf("CDT")>=0)
          {
             
              string dataDatabase = DataAnother.Replace("CDT","CBA");
              sql = "update " + DataAnother + ".dbo." + "syspackage set Package='" + DataAnother + "' where syspackageid=5";
              db.UpdateByNonQuery(sql);
              sql = "update " + DataAnother + ".dbo." + "syspackage set Package='" + dataDatabase + "' where syspackageid<>5";
              db.UpdateByNonQuery(sql);
              sql = "update " + DataAnother + ".dbo." + "sysDB set DatabaseName='" + DataAnother + "' where syspackageid=5";
              db.UpdateByNonQuery(sql);
              sql = "update " + DataAnother + ".dbo." + "sysDB set DatabaseName='" + dataDatabase + "' where syspackageid<>5";
              db.UpdateByNonQuery(sql);
          }
          return re;
        }
    }
}

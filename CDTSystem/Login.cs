using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Localization;
using System.Configuration;
using Microsoft.Win32;
using CDTControl;
using FormFactory;
using CDTLib;
using CDTControl.CDTControl;
using ErrorManager;
using System.IO;
using System.Diagnostics;
using CDTDatabase;
using Newtonsoft.Json;
using System.Globalization;

namespace CDTSystem
{
    public partial class Login : DevExpress.XtraEditors.XtraForm
    {
        private string pwd;
        private string H_KEY;
        private string P_KEY="";
        public DataRow drPackage;
        public DataRow drUser;
        private SysUser _sysUser ;//= new SysUser();
        private SysPackage _sysPackage;// = new SysPackage();
        private SysConfig _sysConfig ;//= new SysConfig();
        
        public Login()
        {

            InitializeComponent();
            //this.WindowState = FormWindowState.Normal;
            //this.Width = 800;
            //this.Height = 500;
            H_KEY = Config.GetValue("H_KEY").ToString();
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\LoginImage")) return;
                DirectoryInfo d = new DirectoryInfo(Application.StartupPath + "\\LoginImage");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles(); //Getting Text files
                if (Files.Length == 0) return;
                Random rnd = new Random();
                int i = rnd.Next(Files.Length-1);

                FileInfo file = Files[i];
                lG1.BackgroundImage = Image.FromFile(file.FullName);
            }
            catch { }

        }

        #region Events
        string StructConnection;
        string RemoteStructConnection;
        private void Login_Load(object sender, EventArgs e)
        {
            textEditUser.Properties.CharacterCasing = CharacterCasing.Upper;
            vCheckEditRemote.EditValue = bool.Parse(Config.GetValue("isRemote").ToString());
       string     StructConnectionEx = Registry.GetValue(H_KEY, "StructDb", string.Empty).ToString();
            StructConnection = Security.DeCode64(StructConnectionEx);

            RemoteStructConnection = Registry.GetValue(H_KEY, "RemoteServer", string.Empty).ToString();
            RemoteStructConnection = Security.DeCode64(RemoteStructConnection);
            vCheckEditRemote_CheckedChanged(vCheckEditRemote, new EventArgs());
               
            P_KEY = H_KEY;
            GetData();
            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
                comboBoxEditStyle.Properties.Items.Add(cnt.SkinName);
            if (comboBoxEditStyle.Text != string.Empty)
                defaultLookAndFeelMain.LookAndFeel.SetSkinStyle(comboBoxEditStyle.Text);
            //this.Height = 260;
            Database _dbStruct = Database.NewCustomDatabase(StructConnection);
           

            if (_dbStruct.Connection.DataSource.Contains("45."))
            {
                string _databaseName = _dbStruct.Connection.Database;
                CPUid cpu = new CPUid(Config.GetValue("ProductName").ToString());
                ComputerConnection computer = new ComputerConnection();
                computer.ComputerName = SystemInformation.ComputerName;
                computer.CPUID = cpu.MaMay;
                computer.DatabaseName = _databaseName;
                computer.LicenceKey = "";//cpu.GetKeyString();
                computer.StructDB = Security.EnCode64(StructConnection);
                string ob = JsonConvert.SerializeObject(computer);
                CDTControl.Log log = new CDTControl.Log();
                string re = log.CheckComputer(ob);
                if (re == null || re == "")
                {
                    MessageBox.Show("Máy này đã không còn được phép truy cập vào dữ liệu, do đổi tên máy hoặc đã bị Admin khóa");
                    this.DialogResult = DialogResult.Cancel;
                }



            }
        }

        private void simpleButtonOk_Click(object sender, EventArgs e)
        {
            _sysUser = new SysUser();
            _sysPackage = new SysPackage();
            _sysConfig = new SysConfig();
            
            if (dxErrorProviderMain.HasErrors)
            {
                XtraMessageBox.Show("Vui lòng cung cấp đủ thông tin yêu cầu");
                return;
            }
            if (textEditPassword.Text != _sysUser.maskPwd)
                pwd = Security.EnCode(textEditPassword.Text);
            if (!_sysUser.CheckLogin(textEditUser.Text, pwd))
            {
                XtraMessageBox.Show("Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại!");
                return;
            }
            drUser = _sysUser.DrUser;

            DataTable dt1 = _sysPackage.GetPackageForUser(_sysUser);
            if (dt1 == null)
                return;
            if (dt1.Rows.Count == 0)
            {
                XtraMessageBox.Show("Người dùng này chưa được phân quyền sử dụng gói phần mềm nào!");
                return;
            }
            //dang nhap thanh cong
            Config.NewKeyValue("StructServer", _sysPackage.StructServer);
            lookUpEditPackage.Properties.DataSource = dt1;
            lookUpEditPackage.Properties.DisplayMember = radioGroupLanguage.SelectedIndex == 0 ? "PackageName" : "PackageName2";
            lookUpEditPackage.Properties.ValueMember = "sysDBID";
            if (dt1.Rows.Count == 1)
            {
                drPackage = dt1.Rows[0];
                lookUpEditPackage.EditValue = dt1.Rows[0]["sysDBID"];

                    Config.NewKeyValue("Admin", bool.Parse(dt1.Rows[0]["isAdmin"].ToString()));
                    
                Config.NewKeyValue("sysUserPackageID", dt1.Rows[0]["sysUserPackageID"]);

                if (_sysConfig.DsStartConfig==null ||_sysConfig.DsStartConfig.Tables[0].Rows.Count == 0)
                {
                    DangNhap();
                }
                else
                {
                    simpleButtonLogin_Click(simpleButtonLogin, new EventArgs());
                    //this.Height = 300;
                    //layoutControl2.Visible = true;
                    ////layoutControlItemLogin.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //layoutControlItemStartConfig.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //layoutControlItemPackage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            else
            {
                // this.Height = 340;
                //layoutControl2.Size = new System.Drawing.Size(583, 40);
                simpleButtonLogin.Visible = true;
                lookUpEditPackage.Visible = true;
               // layoutControl2.Visible = true;
                lc1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lc2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lookUpEditPackage.Focus();
                //layoutControlItemStartConfig.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //layoutControlItemPackage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void simpleButtonLogin_Click(object sender, EventArgs e)
        {
            if (dxErrorProviderMain.HasErrors)
            {
                XtraMessageBox.Show("Vui lòng cung cấp đủ thông tin yêu cầu");
                return;
            }
            
            if (drPackage == null)
                drPackage = (lookUpEditPackage.Properties.GetDataSourceRowByKeyValue(lookUpEditPackage.EditValue) as DataRowView).Row;
            P_KEY = H_KEY;
            Config.NewKeyValue("P_KEY", P_KEY);
            string Product = drPackage["DbName"].ToString().Trim();
            string oldProduct = Config.GetValue("ProductName").ToString();
            //P_KEY = P_KEY +  Product + "\\";
            if (Product != oldProduct)
            {
                string subkey = @"Software\SGD\" + Product;
                
                RegistryKey pKey = Registry.CurrentUser.OpenSubKey(subkey);
                P_KEY = P_KEY.Replace(oldProduct, Product);
                if (pKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(subkey);
                    
                    Registry.SetValue(P_KEY, "CompanyName", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "Created", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY, "isDemo", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY, "Language", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY, "Package", "7", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70", RegistryValueKind.ExpandString);
                    Registry.SetValue(P_KEY, "RegisterNumber", "", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "SavePassword", "True", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "StructDb", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "RemoteServer", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "Style", "Money Twins", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "SupportOnline", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "UserName", "Admin", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "isRemote", "False", RegistryValueKind.String);
                    Registry.SetValue(P_KEY, "SoftType", "0", RegistryValueKind.DWord);
                }
                
                Config.NewKeyValue("H_KEY", P_KEY);
                string Company = Registry.GetValue(P_KEY, "CompanyName", "").ToString();
                int softType = int.Parse(Registry.GetValue(P_KEY, "SoftType", "0").ToString());
                CPUid Cpu;
                if (softType == 0)
                {
                    Cpu = new CPUid(Company + Product + "SGDEMTOnline");

                }else
                    Cpu = new CPUid(Company + Product + "SGDBPMOnline");
                string RegisterNumber = Registry.GetValue(P_KEY, "RegisterNumber", "").ToString();
                if (RegisterNumber != Cpu.KeyString)
                {
                    Config.NewKeyValue("isDemo", 1);
                    if (MessageBox.Show("Bạn đang dùng phiên bản " + Product + " - demo, bạn có muốn đăng ký lại không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        RegisterF rf = new RegisterF();
                        rf.producName = Product;
                        rf.ShowDialog();                        
                        if (rf.DialogResult == DialogResult.Cancel) return;
                        Config.NewKeyValue("isDemo", 0);
                    }              
                
                }
                Config.NewKeyValue("ProductName", Product);
            }

                Config.NewKeyValue("Admin", bool.Parse(drPackage["isAdmin"].ToString()));
            
            Config.NewKeyValue("sysUserPackageID", drPackage["sysUserPackageID"]);
            DangNhap();            
        }

        private void textEditUser_EditValueChanged(object sender, EventArgs e)
        {
            SetError(sender as BaseEdit);
        }

        private void comboBoxEditStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetError(sender as BaseEdit);
            if (comboBoxEditStyle.Text != string.Empty)
                defaultLookAndFeelMain.LookAndFeel.SetSkinStyle(comboBoxEditStyle.Text);
        }

        private void lookUpEditPackage_EditValueChanged(object sender, EventArgs e)
        {
            
            
            if ( lookUpEditPackage.Properties.GetDataSourceRowByKeyValue(lookUpEditPackage.EditValue) !=null)
                drPackage = (lookUpEditPackage.Properties.GetDataSourceRowByKeyValue(lookUpEditPackage.EditValue) as DataRowView).Row;
            try
            {
                if (lookUpEditPackage.EditValue != null && lookUpEditPackage.EditValue.ToString() != "")
                {
                 //   _sysConfig.GetStartConfig(lookUpEditPackage.EditValue.ToString(), drPackage["sysDBID"].ToString());
                    //if (_sysConfig.DsStartConfig != null)
                    //    gcMain.DataSource = _sysConfig.DsStartConfig.Tables[0];
                }
            }
            catch { }
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
        #endregion

        #region Function
        private void GetData()
        {
            lookUpEditPackage.EditValue = Registry.GetValue(H_KEY, "Package", "");
            comboBoxEditStyle.EditValue = Registry.GetValue(H_KEY, "Style", "");
            radioGroupLanguage.SelectedIndex = Int32.Parse(Registry.GetValue(H_KEY, "Language", "0").ToString());
            LanguageForPackage(radioGroupLanguage.SelectedIndex == 0);
            if (Registry.GetValue(H_KEY, "SavePassword", "false").ToString() != string.Empty)
                vCheckEditSavePwd.Checked = Boolean.Parse(Registry.GetValue(H_KEY, "SavePassword", "false").ToString());
            vCheckEditRemote.Checked = Boolean.Parse(Config.GetValue("isRemote").ToString());
            textEditUser.EditValue = Registry.GetValue(H_KEY, "UserName", "");
            if (vCheckEditSavePwd.Checked && Registry.GetValue(H_KEY, "SavePassword", "false").ToString() != string.Empty)
            {
                textEditPassword.Text = "************";
                pwd = Registry.GetValue(H_KEY, "Password", string.Empty).ToString();
            }
        }

        private void DangNhap()
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            SetData();
            _sysConfig.UpdateStartConfig();
            _sysPackage.InitSysvar(drPackage["sysPackageID"].ToString(), drPackage["sysDBID"].ToString());
            //Kiểm tra update
            if (Config.Variables.Contains("UpdatePath"))
            {
                LogFile log=new LogFile();
                string updatePath = Config.GetValue("UpdatePath").ToString();
                DateTime lastUpdate = _sysPackage.LastUpdate();
                string path=Application.StartupPath + "\\UpdateDate.txt";
                if (!File.Exists(path))
                {
                    LogFile.CreateFile(path);
                    
                }
                string[] l = File.ReadAllLines(path); //LogFile.readFile(path);
                try
                {
                    if (l.Length == 0)
                    {
                        this.UpdateProgram(updatePath);
                        return;
                    }                    
                    DateTime curUpdateTime=DateTime.ParseExact(l[0],"MM/dd/yyyy", provider);

                    if (curUpdateTime < lastUpdate)
                    {
                        //Cập nhật
                            this.UpdateProgram(updatePath);
                        
                        return;
                       
                    }
                }
                catch { }
            }
            if (radioGroupLanguage.SelectedIndex == 0)
            {
                Localizer.Active = new DevLocalizer.MyLocalizer();
                GridLocalizer.Active = new DevLocalizer.MyGridLocalizer();
                ReportLocalizer.Active = new DevLocalizer.MyReportLocalizer();
                PreviewLocalizer.Active = new DevLocalizer.MyPreviewLocalizer();
            }
            try
            {
                if (Config.GetValue("sysPackageID").ToString() != "5" && Config.GetValue("MaCN") == null)
                {

                    fChonCN fchoncn = new fChonCN();
                    if (fchoncn._dbCN.DsData.Tables[0].Rows.Count != 1)
                    {
                        if (fchoncn.ShowDialog() == DialogResult.Cancel)
                        {
                            this.DialogResult = DialogResult.Cancel;
                            return;
                        }
                    }
                    else if (fchoncn._dbCN.DsData.Tables[0].Rows.Count == 1)
                    {
                        Config.NewKeyValue("MaCN", fchoncn._dbCN.DsData.Tables[0].Rows[0]["MaCN"].ToString());
                    }
                    if (!Config.Variables.Contains("TheoCa")) Config.NewKeyValue("TheoCa", false);
                    if (bool.Parse(Config.GetValue("TheoCa").ToString())){
                        fChonCa fchonca = new fChonCa();
                        if (fchonca._dbCa.DsData.Tables[0].Rows.Count != 1)
                        {
                            if (fchonca.ShowDialog() == DialogResult.Cancel)
                            {
                                this.DialogResult = DialogResult.Cancel;
                                return;
                            }
                        }
                        else if (fchonca._dbCa.DsData.Tables[0].Rows.Count == 1)
                        {
                            Config.NewKeyValue("MaCa", fchonca._dbCa.DsData.Tables[0].Rows[0]["MaCa"].ToString());
                        }
                        else
                        {
                            Config.NewKeyValue("MaCa", null);
                        }
                        
                    }
                    else
                    {
                        Config.NewKeyValue("MaCa", null);
                    }

                }
                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
            }
           
        }
        private void UpdateProgram(string path)
        {
            this.DialogResult = DialogResult.Cancel;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "AutoUpdate.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = " " + path;
            startInfo.Verb = "runas";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                   // exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }
        private void SetData()
        {
            string sysPackageID = drPackage["sysPackageID"].ToString();

            string sysDBID = drPackage["sysDBID"].ToString();
            string DbName=drPackage["DbName"].ToString();
            //string DBName = drPackage["PackageName"].ToString();
            
            string DataConnection = Security.DeCode64(drPackage["DbPath"].ToString()) + ";Database = " + DbName;
            
            if(Boolean.Parse(Config.GetValue("isRemote").ToString()))
                DataConnection = Security.DeCode64(drPackage["DbPathRemote"].ToString()) + ";Database = " + DbName;
            string CompanyName = Registry.GetValue(P_KEY, "CompanyName", string.Empty).ToString();
            Config.NewKeyValue("ComputerName", SystemInformation.ComputerName);
            Config.NewKeyValue("TenCongTy", CompanyName);
            Config.NewKeyValue("sysPackageID", sysPackageID);
            Config.NewKeyValue("sysDBID", sysDBID);
            Config.NewKeyValue("DbName", DbName);
            
            Config.NewKeyValue("DataConnection", DataConnection);
            CDTDatabase.Database db = CDTDatabase.Database.NewDataDatabase();
            if(db.Connection.DataSource!=null)
                Config.NewKeyValue("DataServer", db.Connection.DataSource);
            Config.NewKeyValue("Package", drPackage["Package"].ToString());
            Config.NewKeyValue("Version", drPackage["Version"].ToString());
            Config.NewKeyValue("Copyright", drPackage["Copyright"].ToString());
            Config.NewKeyValue("PackageName", drPackage["PackageName"].ToString());
            Config.NewKeyValue("sysUserID", drUser["sysUserID"].ToString());
            Config.NewKeyValue("sysUserGroupID", drUser["sysUserGroupID"].ToString());
            Config.NewKeyValue("GroupName", drUser["GroupName"].ToString());
            Config.NewKeyValue("Style", comboBoxEditStyle.Text);
            Config.NewKeyValue("Language", radioGroupLanguage.SelectedIndex);
            Registry.SetValue(H_KEY, "Package", sysDBID);
            Registry.SetValue(H_KEY, "Style", comboBoxEditStyle.Text);
            Registry.SetValue(H_KEY, "Language", radioGroupLanguage.SelectedIndex);
            Registry.SetValue(H_KEY, "SavePassword", vCheckEditSavePwd.Checked.ToString());
            if (!Boolean.Parse(_sysUser.DrUser["CoreAdmin"].ToString()))
                Registry.SetValue(H_KEY, "UserName", textEditUser.Text);
            Config.NewKeyValue("UserName", textEditUser.Text);
            Config.NewKeyValue("FullName", drUser["FullName"].ToString());
            if (drUser.Table.Columns.Contains("UserCode"))
            {
                Config.NewKeyValue("UserCode", drUser["UserCode"].ToString());
            }
            if (drUser.Table.Columns.Contains("SdtUser"))
            {
                Config.NewKeyValue("SdtUser", drUser["SdtUser"].ToString());
            }
            if (drUser.Table.Columns.Contains("TheoCa"))
            {
                Config.NewKeyValue("TheoCa", drUser["TheoCa"]==DBNull.Value? false: bool.Parse(drUser["TheoCa"].ToString()));
            }
            if (drUser.Table.Columns.Contains("MaCN") && drUser["MaCN"] != DBNull.Value && drUser["MaCN"].ToString() != string.Empty)
            {
                Config.NewKeyValue("MaCN", drUser["MaCN"].ToString());
            }
            Config.NewKeyValue("NgayHethong", _sysPackage.ngayht());
            if (!Boolean.Parse(_sysUser.DrUser["CoreAdmin"].ToString()) && vCheckEditSavePwd.Checked)
            {
                if (textEditPassword.Text != _sysUser.maskPwd)
                    Registry.SetValue(H_KEY, "Password", Security.EnCode(textEditPassword.Text));
                else
                    Registry.SetValue(H_KEY, "Password", pwd);
            }
            Config.NewKeyValue("fbID", Registry.GetValue(H_KEY, "fbID", string.Empty).ToString());
            Config.NewKeyValue("SoftType", Registry.GetValue(H_KEY, "SoftType", 0).ToString());
            var k = Config.GetValue("SoftType");
        }

        private void SetError(BaseEdit be)
        {
            if (be.EditValue == null || be.EditValue.ToString() == string.Empty)
                dxErrorProviderMain.SetError(be, "Phải nhập");
            else
                dxErrorProviderMain.SetError(be, string.Empty);
        }
        #endregion

        private void radioGroupLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageForPackage(radioGroupLanguage.SelectedIndex == 0);
            try
            {
                _sysPackage = new SysPackage();
                _sysPackage.InitDictionary();
                DevLocalizer.Translate(this);
            }
            catch { }
        }

        private void LanguageForPackage(bool isVietnamese)
        {
            lookUpEditPackage.Properties.Columns[1].Visible = isVietnamese;
            lookUpEditPackage.Properties.Columns[2].Visible = !isVietnamese;
            lookUpEditPackage.Properties.DisplayMember = isVietnamese ? "PackageName" : "PackageName2";
        }

        private void vCheckEditRemote_CheckedChanged(object sender, EventArgs e)
        {
            if (vCheckEditRemote.EditValue == null) return;
            bool isRemote = bool.Parse(vCheckEditRemote.EditValue.ToString());

            Config.NewKeyValue("isRemote", isRemote);
            if (isRemote)
                Config.NewKeyValue("StructConnection", RemoteStructConnection);
            else
                Config.NewKeyValue("StructConnection", StructConnection);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            fSoftList f = new fSoftList();
            f.ShowDialog();
            Application.Restart();
        }
    }
}
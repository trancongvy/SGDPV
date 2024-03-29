using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using DevExpress.XtraEditors;
using CDTControl;
using DevExpress.XtraLayout.Utils;
using CDTLib;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using CDTControl.CDTControl;
using Newtonsoft.Json;
using ErrorManager;
namespace CDTSystem
{
    public partial class CreateData : DevExpress.XtraEditors.XtraForm
    {
        string _ver;
        private bool ismorong = false;
        private CheckEdit cEis2005;

        public CreateData(string Ver)
        {
            this._ver = Ver;
            InitializeComponent();
            radioGroupType.SelectedIndex = 0;
            radioGroupCnnType.SelectedIndex = 0;

        }

        private void radioGroupCnnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textEditUser.Enabled = this.radioGroupCnnType.SelectedIndex == 0;
            this.textEditPassword.Enabled = this.radioGroupCnnType.SelectedIndex == 0;
            if (this.radioGroupCnnType.SelectedIndex == 1)
            {
                this.textEditUser.EditValue = "sa";
                this.textEditPassword.EditValue = "sa";
            }
        }

        private void radioGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.radioGroupType.SelectedIndex == 0)
            {
                this.textEditServer.EditValue = SystemInformation.ComputerName + "\\SQLSGD2005";
                txtRemoteServer.EditValue=SystemInformation.ComputerName + "\\SQLSGD2005";
                CkUpdateLocal.Checked = true;
                ckUpdateRemote.Checked = true;
            }
            else
            {
                CkUpdateLocal.Checked = false;
                ckUpdateRemote.Checked = false;
            }
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

                this.layoutControlItem3.Visibility = LayoutVisibility.Always;
                this.layoutControlItem4.Visibility = LayoutVisibility.Always;
                this.layoutControlItem7.Visibility = LayoutVisibility.Always;
                this.layoutControlItem8.Visibility = LayoutVisibility.Always;
                this.layoutControlItem9.Visibility = LayoutVisibility.Always;

        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
        }

        private void simpleButtonOk_Click(object sender, EventArgs e)
        {
            if (this.dxErrorProviderMain.HasErrors)
            {
                XtraMessageBox.Show("Thông tin chưa hợp lệ, vui lòng kiểm tra lại!");
            }
            else
            {
                this._ver = this.txtCDT.Text;
                string text = this.textEditServer.Text;
                string textRemote = this.txtRemoteServer.Text;
                DataMaintain maintain = new DataMaintain(text, textRemote, this.radioGroupCnnType.SelectedIndex, this.textEditUser.Text, this.textEditPassword.Text);

                maintain.isServer2005 = this.cEis2005.Checked;


                this.layoutControl1.Refresh();
                bool flag = false;
                if (this.radioGroupType.SelectedIndex == 1)
                {
                    flag = maintain.ClientExecute(this._ver);
                    string H_KEY = Config.GetValue("H_KEY").ToString();
                    if (flag)
                    {

                    }
                    if (flag && ckUpdateRemote.Checked)
                        maintain.UpdateRemoteServer(txtRemoteServer.Text,this._ver);
                    if (flag && CkUpdateLocal.Checked)
                        maintain.UpdateLocalServer(text, this._ver);
                }
                else
                {
                    flag = maintain.ServerExecute(Application.StartupPath, this._ver);
                }
                if (flag)
                {
                    //Đăng ký sử dụng nếu là dataonline
                    if (text.Contains("45."))
                    {
                      string  _databaseName = _ver;
                        CPUid cpu = new CPUid(Config.GetValue("ProductName").ToString());
                        ComputerConnection computer = new ComputerConnection();
                        computer.ComputerName = SystemInformation.ComputerName;
                        computer.CPUID = cpu.MaMay;
                        computer.DatabaseName = _databaseName;
                        computer.LicenceKey = "";//cpu.GetKeyString();
                        computer.StructDB = Security.EnCode64(maintain.Connection);
                        string ob = JsonConvert.SerializeObject(computer);
                        
                        CDTControl.Log log = new CDTControl.Log();
                        string re = log.RegistComputer(ob);
                        if(re ==null || re == "")
                        {
                            XtraMessageBox.Show("Có lỗi trong quá trình tạo số liệu, vui lòng kiểm tra lại!");
                            LogFile.AppendNewText("log.txt", ob);
                            return;
                        }

                        

                    }
                        base.DialogResult = DialogResult.OK;
                }
                else
                {
                    XtraMessageBox.Show("Có lỗi trong quá trình tạo số liệu, vui lòng kiểm tra lại!");
                    LogFile.AppendNewText("log.txt", "Lỗi");
                }
            }
        }

        private void textEditServer_EditValueChanged(object sender, EventArgs e)
        {
            if (this.textEditServer.Text == string.Empty)
            {
                this.dxErrorProviderMain.SetError(this.textEditServer, "Phải nhập");
            }
            else
            {
                this.dxErrorProviderMain.SetError(this.textEditServer, string.Empty);
            }
        }

        private void textEditUser_EditValueChanged(object sender, EventArgs e)
        {
            if ((this.radioGroupCnnType.SelectedIndex == 1) && (this.textEditUser.Text == string.Empty))
            {
                this.dxErrorProviderMain.SetError(this.textEditUser, "Phải nhập");
            }
            else
            {
                this.dxErrorProviderMain.SetError(this.textEditUser, string.Empty);
            }
        }

        private void CreateData_Load(object sender, EventArgs e)
        {
            txtCDT.Text = this._ver.Replace("CBA", "CDT");
            this.textEditServer.EditValue = SystemInformation.ComputerName + "\\SQLSGD2005";
            txtRemoteServer.EditValue = SystemInformation.ComputerName + "\\SQLSGD2005";
            CkUpdateLocal.Checked = true;
            ckUpdateRemote.Checked = true;
            sbInstallSQL.Enabled = !CheckSQL();
            if (sbInstallSQL.Enabled)
                layoutControlItem14.Visibility = LayoutVisibility.Always;
            else layoutControlItem14.Visibility = LayoutVisibility.Never;
            //Phan quyen cho thu muc
            try
            {
                FileSystemRights Rights = (FileSystemRights)0;
                Rights = FileSystemRights.FullControl;
                DirectoryInfo dirInfo = new DirectoryInfo(Application.StartupPath);
                DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
                FileSystemAccessRule AccessRule = new FileSystemAccessRule("NETWORK SERVICE", Rights, InheritanceFlags.ContainerInherit |
                InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
                dirSecurity.AddAccessRule(AccessRule);



                dirInfo.SetAccessControl(dirSecurity);
            }
            catch { }
            //bool Result = false;
            //dirSecurity.ModifyAccessRule(AccessControlModification.Set, AccessRule, out Result);

            //AccessRule = new FileSystemAccessRule("NETWORK SERVICE", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit |
            //InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly,
            //AccessControlType.Allow);
            
            //Result = false;
            //dirSecurity.ModifyAccessRule(AccessControlModification.Set, AccessRule, out Result);
            //dirInfo.SetAccessControl(dirSecurity);
            //Cài SQL
        }
        private bool CheckSQL()
        {
            RegistryView registryView =  RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null )
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        if (instanceName == "SQLSGD2005")
                            return true;
                    }
                }
            }
            if (Environment.Is64BitOperatingSystem)
            {
                registryView = RegistryView.Registry64;
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                    if (instanceKey != null)
                    {
                        foreach (var instanceName in instanceKey.GetValueNames())
                        {
                            if (instanceName == "SQLSGD2005")
                                return true;
                        }
                    }
                }
            }
            return false;
        }


        private void txtCDT_EditValueChanged(object sender, EventArgs e)
        {
           // this._ver = txtCDT.Text;
        }

        private void sbInstallSQL_Click(object sender, EventArgs e)
        {
            Process pro = new Process();
            pro.StartInfo.FileName = Application.StartupPath + @"\SQLEXPR_SP2.EXE";
            pro.StartInfo.Arguments = "/Q ADDLOCAL=ALL INSTANCENAME=SQLSGD2005 SECURITYMODE=SQL SAPWD=passwordcongtysgd SQLAUTOSTART=1 SQLBROWSERAUTOSTART=1 DISABLENETWORKPROTOCOLS=0 ADDCURRENTUSERASSQLADMIN=1  ";

            pro.StartInfo.CreateNoWindow = true;
            pro.Start();
            pro.WaitForExit();
        }

        private void textEditPassword_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
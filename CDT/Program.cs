using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Globalization;
using CDTLib;
using CDTSystem;
using CDTControl.CDTControl;

using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ErrorManager;
using System.IO;
using System.Configuration;

//using Windows.Data.Xml.Dom;
//using Windows.UI.Notifications;
namespace CDT
{
    static class Program
    {
        [STAThread]
        [Obsolete]
        public static  void Main(string[] args)
       {

            //double x = 0.345000;
            //MessageBox.Show(x.ToString());
           // args = new string[] { "CBADONGTIEN1" };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);
            BonusSkins.Register();
            OfficeSkins.Register();
            string defaultStyle2 = "Money Twins";
            DefaultLookAndFeel defaultLookAndFeelMain2 = new DefaultLookAndFeel();
            if (defaultStyle2 != string.Empty)
            {
                defaultLookAndFeelMain2.LookAndFeel.SetSkinStyle(defaultStyle2);
            }
             string H_KEY = "HKEY_CURRENT_USER\\Software\\SGD\\";
            RegistryKey HKey = Registry.CurrentUser.OpenSubKey("Software\\SGD\\");
            if (HKey == null)
            {
                Registry.CurrentUser.CreateSubKey("Software\\SGD\\");
                HKey = Registry.CurrentUser.OpenSubKey("Software\\SGD\\");
            }
            string server = ConfigurationSettings.AppSettings["WebServer"];
            Config.NewKeyValue("WebServer", server);
            string productName3 = "CBASGD133";
            string[] softList = HKey.GetSubKeyNames();
            string P_KEY2 = "";
            if (args.Length != 0)
            {
                productName3 = args[0];
                softList = new[] { args[0] };
            }
           
            
           
            
           
            if (softList.Length > 1)
            {
                fSoftList fsl = new fSoftList();
                fsl.ShowDialog();
                productName3 = fsl.Productname;
                P_KEY2 = H_KEY + productName3 + "\\";
                if (productName3 == string.Empty)
                {
                    return;
                }
            }
            else if (softList.Length == 0)
            {
                //productName3 = "CBASGD133";
                string subkey = "Software\\SGD\\" + productName3;
                P_KEY2 = H_KEY + productName3 + "\\";
                RegistryKey pKey = Registry.CurrentUser.OpenSubKey(subkey);
                if (pKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(subkey);
                    Registry.SetValue(P_KEY2, "CompanyName", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "Created", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY2, "isDemo", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY2, "Language", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY2, "Package", "7", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "isOnline", "0", RegistryValueKind.DWord);
                    Registry.SetValue(P_KEY2, "Password", "20-2C-B9-62-AC-59-07-5B-96-4B-07-15-2D-23-4B-70", RegistryValueKind.ExpandString);
                    Registry.SetValue(P_KEY2, "RegisterNumber", "", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "SavePassword", "True", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "StructDb", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "RemoteServer", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "Style", "Money Twins", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "SupportOnline", "SGD", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "UserName", "Admin", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "isRemote", "False", RegistryValueKind.String);
                    Registry.SetValue(P_KEY2, "SoftType", "0", RegistryValueKind.DWord);
                }
            }
            else
            {
                productName3 = softList[0];
                P_KEY2 = H_KEY + productName3 + "\\";
            }
            Config.NewKeyValue("ProductName", productName3);
            Config.NewKeyValue("H_KEY", P_KEY2);
            defaultStyle2 = Registry.GetValue(P_KEY2, "Style", string.Empty).ToString();
            defaultLookAndFeelMain2 = new DefaultLookAndFeel();
            if (defaultStyle2 != string.Empty)
            {
                defaultLookAndFeelMain2.LookAndFeel.SetSkinStyle(defaultStyle2);
            }
            string created = Registry.GetValue(P_KEY2, "Created", 0).ToString();
            if (created == "0")
            {
                CreateData frmCreateData = new CreateData(productName3);
                frmCreateData.ShowDialog();
                if (frmCreateData.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                Registry.SetValue(P_KEY2, "Created", 1);
            }
            Config.NewKeyValue("H_KEY", P_KEY2);
            int SoftType = (int)Registry.GetValue(P_KEY2, "SoftType", 1);
            int isOnline = 0;
            try { isOnline = (int)Registry.GetValue(P_KEY2, "isOnline", 0); } catch { }
            string extend = isOnline == 0 ? "" : "1";
            string Company = Registry.GetValue(P_KEY2, "CompanyName", "").ToString();
            CPUid Cpu = (SoftType != 0) ? new CPUid(Company + productName3 + "SGDBPMOnline" + extend) : new CPUid(Company + productName3 + "SGDEMTOnline" + extend);
            string RegisterNumber = Registry.GetValue(P_KEY2, "RegisterNumber", "").ToString();
            if (RegisterNumber != Cpu.KeyString)
            {
                Config.NewKeyValue("isDemo", 1);
                if (MessageBox.Show("Bạn đang dùng phiên bản demo, bạn có muốn đăng ký lại không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    RegisterF rf = new RegisterF();
                    rf.producName = productName3;
                    rf.ShowDialog();
                    Config.NewKeyValue("isDemo", 0);
                    if (rf.DialogResult == DialogResult.Cancel)
                    {
                        return;
                    }
                    RegisterNumber = rf.textEditValue.Text;
                }
            }
            else
            {
                Config.NewKeyValue("isDemo", 0);
                Config.NewKeyValue("CompanyName", Company);
                Config.NewKeyValue("SoftType", SoftType);
                Config.NewKeyValue("isOnline", isOnline);

            }
            SetEnvironment();
            //Check data online phải có register trên server

            

            Login frmLogin = new Login();
            frmLogin.ShowDialog();
            if (frmLogin.DialogResult != DialogResult.Cancel)
            {
                if (isOnline == 1)//Kiểm tra ngày hết hạn
                {
                    
                    try
                    {
                        CDTControl.Log log = new CDTControl.Log();
                        string uKey =  log.GetExDate(RegisterNumber);
                        UserKey key = JsonConvert.DeserializeObject<UserKey>(uKey);
                        if (key == null)
                        {
                            
                            if (File.Exists(productName3 +".txt"))
                            {
                                MessageBox.Show("Máy bạn đã hết hạn sử dụng gói phần mềm này! \n Liên hệ : 0935.45.75.15 Vỹ Công Trần để được hướng dẫn");
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Bạn cần kết nối internet để kiểm tra cập nhật mới nhất!");
                            }
                        }
                        else if (DateTime.Now > key.Exdate)
                        {
                            MessageBox.Show("Máy bạn đã hết hạn sử dụng gói phần mềm này! \n Liên hệ : 00935.45.75.15 Vỹ Công Trần để được hướng dẫn");
                            LogFile.CreateFile(productName3 +".txt");
                            return;
                        }
                        else
                        {
                            LogFile.DeleteFile(productName3 + ".txt");
                        }
                    }
                    catch { }
                }
                Application.Run(new Main(frmLogin.drUser, frmLogin.drPackage));
            }
        }

        private static void SetEnvironment()
        {
            CultureInfo CultureInfo2 = Application.CurrentCulture.Clone() as CultureInfo;
            CultureInfo2 = new CultureInfo("en-US");
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.LongDatePattern = "MM/dd/yyyy h:mm:ss tt";
            dtInfo.ShortDatePattern = "MM/dd/yyyy";
            
            CultureInfo2.DateTimeFormat = dtInfo;
            Application.CurrentCulture = CultureInfo2;
            string H_KEY = Config.GetValue("H_KEY").ToString();
            string isRemote2 = "false";
            isRemote2 = Registry.GetValue(H_KEY, "isRemote", "false").ToString();
            Config.NewKeyValue("isRemote", isRemote2);
            Config.NewKeyValue("StartupPath", Application.StartupPath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CDTLib;
using Microsoft.Win32;
using CDTControl.CDTControl;
using Newtonsoft.Json;
using DevExpress.XtraEditors;
using CDTControl;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;

namespace CDTSystem
{
    public partial class RegisterF : XtraForm
    {
        string H_KEY;
        CPUid cpu;
       
        public RegisterF()
        {
            InitializeComponent();            
            H_KEY = Config.GetValue("H_KEY").ToString();
        }
        private string _ProductName;
        public string producName
        {
            get{return _ProductName;}
            set
            {
                _ProductName = value;
                textProduct.Text = _ProductName;
            }
        }

        int sl = 0;



        private void RegisterF_Load(object sender, EventArgs e)
        {
            this.Width =(int)( Screen.PrimaryScreen.WorkingArea.Width*2/3);
            this.Height = (int)(Screen.PrimaryScreen.WorkingArea.Height   / 2);
        }



        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            //CDTControl.Log log = new CDTControl.Log();
            //cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDEMTOnline");
            //txtMaMay.Text = cpu.MaMay;
            //bool kq = false;
            //try
            //{
            //    kq = log.Check(tUser.Text, tPass.Text); //Check trên sgdsoft.com
            //    if (kq)
            //    {

            //        textEditMaskcode.Text = cpu.MixString;
            //        string key = cpu.KeyString;
            //        string keyGet = log.log(textCompanyName.Text, _ProductName, key, tUser.Text, tPass.Text);
            //        if (keyGet == key)
            //        {
            //            textEditValue.Text = key;
            //            simpleButtonRegister.Enabled = true;
            //            Registry.SetValue(H_KEY, "CompanyName", textCompanyName.Text);
            //            Registry.SetValue(H_KEY, "RegisterNumber", key);
            //            Registry.SetValue(H_KEY, "isDemo", 0);
            //            this.DialogResult = DialogResult.OK;
            //        }
            //        else
            //        {
            //            textEditValue.Text = keyGet;
            //        }

            //    }
            //    else
            //    {
            //        //Check tiếp trên phanmemsgd.com

            //        textEditValue.Text = "User hoặc pass chưa đúng,liên hệ SGDSoft để lấy key";
            //        _ProductName = textProduct.Text.Trim().ToUpper();
            //        cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDEMTOnline");
            //        textEditMaskcode.Text = cpu.MixString;
            //    }
            //}
            //catch(Exception ex) {
            //    MessageBox.Show(ex.Message);
            //}
            Log log = new Log();
            //try
            //{
                LoginViewModel LgView = new LoginViewModel();
                LgView.UserName = tUser.Text;
                LgView.Password = tPass.Text;
                LgView.RememberMe = false;
                string ob = JsonConvert.SerializeObject(LgView);

                string fbID = log.logWeb(ob);
                tfbID.Text = fbID;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        private void simpleButtonRegister_Click(object sender, EventArgs e)
        {
            _ProductName = textProduct.Text.Trim().ToUpper();
            string isOnline = int.Parse(rgOnline.EditValue.ToString()) == 0 ? "" : "1";
            if (rgCnnType.SelectedIndex == 0)
                cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDEMTOnline" + isOnline);
            else if (rgCnnType.SelectedIndex == 1)
                cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDBPMOnline" + isOnline);
            textEditMaskcode.Text = cpu.MixString;
            if (textCompanyName.Text == "SGD")
                textEditValue.Text = cpu.KeyString;
            cpu.KeyString = string.Empty;
            if (cpu == null) return;
            string key = cpu.GetKeyString();
            //Registry.SetValue(H_KEY, "Structtmp", key);
            if (textEditValue.Text == key)
            {
                Registry.SetValue(H_KEY, "CompanyName", textCompanyName.Text);
                Registry.SetValue(H_KEY, "RegisterNumber", textEditValue.Text);
                Registry.SetValue(H_KEY, "isDemo", 0);
                Registry.SetValue(H_KEY, "isOnline", int.Parse(rgOnline.EditValue.ToString()));
                Registry.SetValue(H_KEY, "SoftType", rgCnnType.SelectedIndex.ToString(), RegistryValueKind.DWord);
                if(tfbID.Text!="") Registry.SetValue(H_KEY, "fbID", tfbID.Text);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                    MessageBox.Show("Vui lòng liên hệ SGD soft để được đăng ký sử dụng!");
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            
            fbLogin f = new fbLogin();
            f.Id_changed += F_Id_changed;
            f.ShowDialog();            
        }

        private void F_Id_changed(object sender, EventArgs e)
        {
            fbLogin f = sender as fbLogin;
            if (f.ID != null)
            {
                this.tfbID.Text = f.ID;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            CDTControl.Log log = new CDTControl.Log();
            string isOnline = int.Parse(rgOnline.EditValue.ToString()) == 0 ? "" : "1";
            if (rgCnnType.SelectedIndex==0)
                cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDEMTOnline" + isOnline);
            else if (rgCnnType.SelectedIndex == 1)
                cpu = new CPUid(textCompanyName.Text + _ProductName + "SGDBPMOnline" + isOnline);
            txtMaMay.Text = cpu.MaMay;
            bool kq = false;
            try
            {
                UserKey uKey = new UserKey();
                uKey.ID = 0;uKey.UserID = tfbID.Text;uKey.companyName = textCompanyName.Text;uKey.Mamay = txtMaMay.Text;
                uKey.CDate = DateTime.Now;uKey.DaTT = 0;uKey.IDGT = tIDGT.Text;uKey.isOnline = int.Parse(rgOnline.EditValue.ToString());
                uKey.Product = rgCnnType.SelectedIndex == 0 ? "Phần mềm kế toán" : "Giải pháp BPM";
                uKey.Maskcode = cpu.MixString;uKey.LicenseKey = cpu.KeyString;uKey.Exdate = DateTime.Now.AddYears(int.Parse(tSonam.EditValue.ToString()));

                string ob = JsonConvert.SerializeObject(uKey);

                textEditMaskcode.Text = cpu.MixString;
                    string key = cpu.KeyString;
                    string keyGet = log.logFb(ob);
                    if (keyGet !="")
                    {
                        UserKey returnKey1 = JsonConvert.DeserializeObject<UserKey>(keyGet);
                        textEditValue.Text = returnKey1.LicenseKey;
                        simpleButtonRegister.Enabled = true;
                        simpleButtonRegister_Click(sender, e);
                    }
                    else
                    {
                       // textEditValue.Text = keyGet;
                    }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string logWeb(string ob)
        {
            string url = "https://www.phanmemsgd.com/Account/LoginfromAPI";
            string sContentType = "application/json";
            HttpContent s = new StringContent(ob, Encoding.UTF8, sContentType);
            HttpClient oHttpClient = new HttpClient();
            try
            {
                Task<HttpResponseMessage> oTaskPostAsync = oHttpClient.PostAsync(url, s);
                if (oTaskPostAsync.Result.StatusCode == HttpStatusCode.BadRequest)
                {
                    return "";
                }
                if (oTaskPostAsync.Result.StatusCode == HttpStatusCode.OK)
                {
                    return oTaskPostAsync.Result.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter()
                        .GetResult();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
    public partial class UserKey
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> CDate { get; set; }
        public string Mamay { get; set; }
        public string companyName { get; set; }
        public string Product { get; set; }
        public string Maskcode { get; set; }
        public string LicenseKey { get; set; }
        public int DaTT { get; set; }
        public string IDGT { get; set; }
        public int isOnline { get; set; }
        public DateTime Exdate { get; set; }
    }
    public class LoginViewModel
    {
        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool RememberMe
        {
            get;
            set;
        }
    }

}
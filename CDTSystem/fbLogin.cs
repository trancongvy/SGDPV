using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;
using System.Dynamic;

namespace CDTSystem
{
    public partial class fbLogin : Form
    {
        public FacebookClient fb = new FacebookClient();
        WebBrowser lg = new WebBrowser();

        string AppID = "354210151991058";
        string SecretID="4e97445b73730188786a0dde6c506468";
        string token = "";
        public string ID = "";
        public event EventHandler Id_changed;
        public fbLogin()
        {
            
            InitializeComponent();
            panel2.Controls.Add(lg);
            lg.Dock = DockStyle.Fill;
           // lg.Width = panel2.Width;lg.Height = panel2.Height - 40;lg.Location = new Point(0, 0);
            this.FormClosing += FbLogin_FormClosing;
            this.Resize += FbLogin_Resize;
            lg.Navigated += Lg_Navigated;
            
        }

        private void Lg_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            
            if (lg.Url.Query.Contains("error") || lg.Url.Fragment.Length == 0)
            {
            }
            else
            {
                string query = lg.Url.Fragment.Remove(0, 1);
                string[] para = query.Split("&".ToArray());
                if (para.Length > 0)
                {
                    foreach (string i in para)
                    {
                        if (i.Contains("access_token"))
                        {
                            this.token = i.Replace("access_token=", "");
                            fb.AccessToken = this.token;
                            dynamic result = fb.Get("me", new { access_token = this.token });
                            try
                            {
                                ID = (string)result.id;
                                Id_changed(this, new EventArgs());
                                Uri logoutUrl = fb.GetLogoutUrl(parameter);
                                var result1 = fb.Post(logoutUrl.AbsoluteUri, new { access_token = this.token });
                                //tUrl.Text = logoutUrl.AbsoluteUri;
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
            }

            tUrl.Text = lg.Url.AbsoluteUri;
        }

        private void FbLogin_Resize(object sender, EventArgs e)
        {
           // lg.Width = panel2.Width; lg.Height = panel2.Height - 40; lg.Location = new Point(0, 0);
        }

        private void FbLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("Bạn đã đăng xuất trước khi rời đi chưa?","",MessageBoxButtons.YesNo)==DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        Uri _loginUrl;
        dynamic parameter;
        private const string _ExtenderPermissions = "";//"user_about_me";
        private void fbLogin_Load(object sender, EventArgs e)
        {
            
            parameter = new ExpandoObject();
            parameter.client_id = AppID;
            parameter.redirect_uri = "https://www.facebook.com";
            parameter.response_type = "token";
            parameter.display = "popup";
            if (!string.IsNullOrWhiteSpace(_ExtenderPermissions))
            {
                parameter.scope = _ExtenderPermissions;
            }
            fb.AppId = AppID;fb.AppSecret = SecretID;
            _loginUrl = fb.GetLoginUrl(parameter);
            //lg.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,'')){for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+'; expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");
            lg.Navigate(fb.GetLogoutUrl(parameter));
            tUrl.Text = _loginUrl.AbsoluteUri;
            lg.Navigate(tUrl.Text);
        }

        private void tbGo_Click(object sender, EventArgs e)
        {
            lg.Navigate(tUrl.Text);
        }

        private void tBack_Click(object sender, EventArgs e)
        {
            lg.GoBack();
        }
    }
}

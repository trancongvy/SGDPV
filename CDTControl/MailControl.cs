using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using CDTLib;
namespace CDTControl
{
    public class MailControl
    {
        public int Port = 587;
        public string Host = "smtp.gmail.com";
        public string FromMail = "sgdbpm@gmail.com";
        public string Password = "phanmemBPMSGD";
        public string Nguoigui = "Phần mềm BPM SGD";
        public MailControl(int port, string host, string fromMail, string password, string nguoigui)
        {
            Host = host;
            Port = port;
            FromMail = fromMail;
            Password = password;
            Nguoigui = nguoigui;
        }
        public MailControl()
        {

        }
        public MailControl(int i)
        {
            Port = int.Parse(CDTLib.Config.GetValue("MailPort").ToString());
            Host = CDTLib.Config.GetValue("MailSMTP").ToString();
            FromMail = CDTLib.Config.GetValue("MailAddress").ToString();
            Password = CDTLib.Config.GetValue("MailPass").ToString();
            Nguoigui = CDTLib.Config.GetValue("EmailOwner").ToString();
        } 
        public bool SendMail(string email, string Content, string  SubText)
        {

            if (email.Trim() == "") return false;
            SmtpClient client = new SmtpClient();
            client.Port = Port; //587;//25;//465;//
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = Host;// "smtp.gmail.com";// "smtp.sgdsoft.com";

            //client.Credentials = new NetworkCredential("support@sgdsoft.com", "Support1@sgdsoft.com");
            client.Credentials = new NetworkCredential(FromMail, Password);
            client.EnableSsl = true;//false;//
            MailAddress from = new MailAddress(FromMail, Nguoigui);
            MailAddress to;
            try
            {
                to = new MailAddress(email);
            }
            catch  {
                return false; }
            if (to == null) return false;
            MailMessage mail = new MailMessage(from, to);
            string htmlbody = Content;
            List<LinkedResource> Ress = new List<LinkedResource>();
            //for (int i = 0; i < lFile.Items.Count; i++)
            //{
            //    string path = lFile.Items[i].ToString();
            //    //mail.Attachments.Add(new Attachment(path));
            //    LinkedResource res = new LinkedResource(path);
            //    res.ContentId = Guid.NewGuid().ToString();
            //    htmlbody = htmlbody.Replace("{" + i.ToString() + "}", res.ContentId);
            //    //htmlbody = String.Format(htmlbody, res.ContentId);
            //    Ress.Add(res);

            //}
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlbody, null, MediaTypeNames.Text.Html);
            foreach (LinkedResource res in Ress)
            {
                alternateView.LinkedResources.Add(res);
            }
            mail.AlternateViews.Add(alternateView);

            client.Timeout = (60 * 5 * 1000);
            mail.IsBodyHtml = true;
            mail.Subject = SubText;
            mail.Body = htmlbody;
            try
            {
                client.Send(mail);
                
            }
            catch (Exception ex)
            {
               

            }
            return true;
        }
    }
}

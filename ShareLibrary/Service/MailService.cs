using System.Net.Mail;
using System.Text;
using TpePrmcyWms.Models.DOM;

namespace ShareLibrary.Models.Service
{
    public class MailService
    {        
        string SmtpServer = "owa.tpech.gov.tw"; //smtp.gmail.com
        int SmtpPort = 25; //587
        string FromMail = "APServerAutoCheck@tpech.gov.tw"; //發信mail
        string Account = "spsadm"; //發信帳號
        string TempPwd = "&j941j;3ej03xu3@!"; //應用程式密碼 wdfv lucj uyxp zenl

        public async Task Send(string toMail, string subject, string bodycontent)
        {
            try
            {
                if (string.IsNullOrEmpty(toMail)) { return; }
                List<string> emails = toMail.Split(";").ToList();

                MailMessage mms = new MailMessage();
                mms.From = new MailAddress(FromMail);
                mms.Subject = subject;
                mms.Body = bodycontent;
                mms.IsBodyHtml = true;
                mms.SubjectEncoding = Encoding.UTF8;
                foreach (string email in emails) { mms.To.Add(new MailAddress(email)); }
                using (SmtpClient client = new SmtpClient(SmtpServer))
                {
                    client.EnableSsl = false;
                    await client.SendMailAsync(mms); //寄出信件
                }
            }
            catch (Exception ex)
            {
                //qwFunc.savelog($"{ex}");
            }
        }

        public bool Sending(string toMail, string subject, string bodycontent)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(toMail)) { return false; }
                List<string> emails = toMail.Split(";").ToList();

                MailMessage mms = new MailMessage();
                mms.From = new MailAddress(FromMail);
                mms.Subject = subject;
                mms.Body = bodycontent;
                mms.IsBodyHtml = true;
                mms.SubjectEncoding = Encoding.UTF8;
                foreach (string email in emails) { mms.To.Add(new MailAddress(email)); }
                using (SmtpClient client = new SmtpClient(SmtpServer))
                {
                    client.EnableSsl = false;
                    client.SendMailAsync(mms); //寄出信件
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //qwFunc.savelog($"{ex}");
            }
            return result;
        }

    }
}

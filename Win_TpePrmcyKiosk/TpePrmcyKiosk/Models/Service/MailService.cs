using System.Net.Mail;
using System.Text;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.DOM;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk.Models.Service
{
    public class MailService
    {
        DBcPharmacy _db = new DBcPharmacy();
        
        string SmtpServer = "owa.tpech.gov.tw"; //smtp.gmail.com
        int SmtpPort = 25; //587
        string FromMail = "APServerAutoCheck@tpech.gov.tw"; //發信mail
        string Account = "spsadm"; //發信帳號
        string TempPwd = "&j941j;3ej03xu3@!"; //應用程式密碼 wdfv lucj uyxp zenl

        public async Task Send(string toMail, string subject, string bodycontent)
        {
            try
            {
                MailMessage mms = new MailMessage();
                mms.From = new MailAddress(FromMail);
                mms.Subject = subject;
                mms.Body = bodycontent;
                mms.IsBodyHtml = true;
                mms.SubjectEncoding = Encoding.UTF8;
                mms.To.Add(new MailAddress(toMail));
                using (SmtpClient client = new SmtpClient(SmtpServer))
                {
                    client.EnableSsl = false;
                    await client.SendMailAsync(mms); //寄出信件
                }
            }
            catch (Exception ex)
            {
                qwFunc.savelog($"{ex}");
            }
        }

        public async void AlertMsg(SensorComuQuee qu) 
        {
            /*
             alertType:
                lowstock 低於安全量
                errstocktak 盤點數量不一致
                dooropen 門超時未關
                apichkerr
             */
            //if(!(qu.DrawFid > 0)) { return; }

            //AlertNotification amsg = new AlertNotification()
            //{
            //    AlertType = "dooropen",
            //    StockBillFid = qu.stockBillFid,
            //    Msg = "",
            //    OccTime = DateTime.Now,
            //}; 
            //try
            //{
            //    Drawers drawer = _db.Drawers.Find(qu.DrawFid);
            //    Cabinet cabinet = _db.Cabinet.Find(qu.CbntFid);
            //    employee emp = _db.employee.Find(qu.modid);
            //    List<employee> mailto = _db.employee.Where(x => x.comFid == cabinet.comFid && x.Notified == true).ToList();

            //    string subject = "智慧藥櫃系統警報-櫃門未關";
            //    amsg.Msg = $"櫃名:{cabinet.CbntName}#{drawer.No} 開啟時間過久。";
            //    if (emp != null) { amsg.Msg += $"開櫃使用者:{emp.name}"; }

            //    _db.AlertNotification.Add(amsg);
            //    _db.SaveChanges();

            //    //sending
            //    foreach (employee m in mailto)
            //    {
            //        await Send(m.name, subject, amsg.Msg);
            //    }
            //}
            //catch (Exception ex) { qwFunc.savelog($"{ex}"); }
        }

    }
}

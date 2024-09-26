using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Unit.Back;
using System.Net.Mail;
using System.Text;

namespace TpePrmcyWms.Models.Service
{
    public class MailService
    {
        LoginInfo loginfo;
        DBcPharmacy _db = new DBcPharmacy();
        public MailService(LoginInfo _loginfo) {
            loginfo = _loginfo;
        }

        string SmtpServer = "owa.tpech.gov.tw"; //smtp.gmail.com
        int SmtpPort = 25; //587
        string FromMail = "APServerAutoCheck@tpech.gov.tw"; //發信mail
        string Account = "spsadm"; //發信帳號
        string TempPwd = "&j941j;3ej03xu3@!"; //應用程式密碼 wdfv lucj uyxp zenl

        public void Send(string toMail, string subject, string bodycontent)
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
                    client.Send(mms); //寄出信件
                }
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(loginfo, ex);
            }
        }

        public async void AlertMsg(StockBill obj, string alertType) 
        {
            /*
             alertType:
                lowstock 低於安全量
                errstocktak 盤點數量不一致
                dooropen 門超時未關
                apichkerr
             */
            AlertMsg amsg = new AlertMsg()
            {
                AlertType = alertType,
                StockBillFid = obj.FID,
                Msg = "",
                OccTime = DateTime.Now,
            };

            var infos = (from bill in _db.StockBill
                         join grid in _db.DrugGrid on bill.DrugGridFid equals grid.FID
                         join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                         join draw in _db.Drawers on grid.DrawFid equals draw.FID                         
                         join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                         where bill.FID == obj.FID
                         select new
                         {
                             key = bill.FID,
                             qty = grid.Qty,
                             safetystock = grid.SafetyStock,
                             drugcode = drug.DrugCode,
                             drugname = drug.DrugName,
                             cbntname = cbnt.CbntName,
                             drawerno = draw.No,
                         }).First();

            employee emp = _db.employee.Find(obj.modid);
            List<employee> mailto = _db.employee.Where(x => x.comFid == obj.comFid && x.Notified == true).ToList();

            string subject = "";
            switch (alertType)
            {
                case "lowstock":
                    subject = "智慧藥櫃系統警報-藥品數量低於警戒值";
                    amsg.Msg = $"櫃名:{infos.cbntname}#{infos.drawerno}/藥品:{infos.drugname}/庫存:{infos.qty}/警戒:{infos.safetystock}";
                    break;
                case "errstocktak":
                    subject = "智慧藥櫃系統警報-盲點數量錯誤";
                    amsg.Msg = $"櫃名:{infos.cbntname}#{infos.drawerno}/藥品:{infos.drugname}/原本庫存:{obj.SysChkQty + obj.Qty}/{(obj.TradeType ? "進" : "出")}貨數量:{obj.Qty}/盤點第一次數量:{obj.UserChk1Qty}/盤點第二次數量:{obj.UserChk2Qty}/使用者:{emp.name}/操作:{(obj.TakeType == "1" ? "明" : "暗")}盤";
                    break;
                case "dooropen":
                    subject = "智慧藥櫃系統警報-櫃門未關";
                    amsg.Msg = $"櫃名:{infos.cbntname}#{infos.drawerno} 開啟時間過久。開櫃使用者:{emp.name}";
                    break;
                case "apichkerr":
                    amsg.Msg = $"";
                    break;
            }
            _db.AlertMsg.Add(amsg);
            _db.SaveChanges();

            //sending
            foreach(employee m in mailto)
            {
                //Send(m.name, subject, amsg.Msg);
            }
        }

    }
}

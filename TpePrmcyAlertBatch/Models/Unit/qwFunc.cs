using System;
using System.IO;  //file
using System.Configuration;
using System.Net.Mail;
using System.Text;



namespace TpePrmcyAlertBatch.Models.Unit
{
    static class qwFunc
    {
        static string LogPath = ConfigurationManager.AppSettings["LogPath"] == "" ? "log" : ConfigurationManager.AppSettings["LogPath"];
        static string sysLog_Name = "TpePrmcyAlertBatchLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";


        #region 清單日log
        public static void cleanlog()
        {
            File.WriteAllText(Path.Combine(LogPath, sysLog_Name), "");
        }
        #endregion

        #region 產生log檔
        public static bool createlog()
        {
            try { Directory.CreateDirectory(LogPath); }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        
        public static bool log(string content)
        {
            try
            {
                string header = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} > ";
                if (createlog())
                {
                    File.AppendAllText(Path.Combine(LogPath, sysLog_Name), header + content + Environment.NewLine);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("!! Saving Log File Error !! Msg: " + ex.ToString());
                return false;
            }

        }

        public static void logshow(string content)
        {
            Console.WriteLine (content);
            log(content);
        }

        #endregion

        #region 讀取log

        public static string ReadLog()
        {
            string logfile = Path.Combine(LogPath, sysLog_Name);            
            return System.IO.File.ReadAllText(logfile);
        }
        #endregion


                
        //public static bool SendMail(string MailContent)
        //{
        //    string sMsg = "";            
        //    string sEMailServer = ConfigurationManager.AppSettings["MailServer"];
        //    string sSender = ConfigurationManager.AppSettings["MailSender"];
        //    string sTraceMail = ConfigurationManager.AppSettings["TraceMail"];

        //    try
        //    {
        //        //string sEMailAddr, sMailName;
        //        using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage())
        //        {
        //            message.From = new MailAddress(sSender, "MAPS資料轉移通知", Encoding.UTF8);
        //            message.Subject = "MAPS資料轉移通知!!";
        //            message.SubjectEncoding = System.Text.Encoding.UTF8;
        //            message.IsBodyHtml = true;
        //            //message.Body = sMessage + "<br/><br/>";
        //            message.BodyEncoding = Encoding.UTF8;
        //            string[] email = sTraceMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (string address in email)
        //            {
        //                message.To.Add(new MailAddress(address, address, Encoding.UTF8));
        //            }

        //            string sBody = string.Format("<html><head></head><body><font color='blue'>{0}</font></body></html>", MailContent);
        //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(sBody, System.Text.Encoding.UTF8, "text/html");
        //            message.AlternateViews.Add(htmlView);
        //            SmtpClient mailClient = new SmtpClient(sEMailServer, 25);
        //            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            mailClient.Send(message);
        //            return true;
        //        }
        //    }
        //    catch (FormatException ex)
        //    {
        //        sMsg = "FormatException:" + ex.Message;
        //        if (ex.InnerException != null)
        //            sMsg += "," + ex.InnerException.Message;
        //    }
        //    catch (SmtpException ex)
        //    {
        //        sMsg = "SmtpException:" + ex.Message;
        //        if (ex.InnerException != null)
        //            sMsg += "," + ex.InnerException.Message;
        //    }
        //    catch (Exception ex)
        //    {
        //        sMsg = ex.Message;
        //        if (ex.InnerException != null)
        //            sMsg += "," + ex.InnerException.Message;
        //    }
        //    if (sMsg != "")
        //    {
        //        log("EMAIL傳送失敗:" + sMsg);
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        

    }



}

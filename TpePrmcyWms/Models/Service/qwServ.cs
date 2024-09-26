using System;
using System.IO;  //file
using System.Net.Mail;
using System.Text;
using System.Globalization;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace TpePrmcyWms.Models.Service
{
    static class qwServ
    {
        static string LogPath = SysBaseServ.JsonConf("Path:Log");
        static string sysLog_Name = "sysLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

        

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
        #endregion

        #region 存log檔 分每日一個檔案
        public static bool savelog(string content)
        {
            createlog();
            try
            {
                File.AppendAllText(Path.Combine(LogPath, sysLog_Name), $"{DateTime.Now.ToString("HH:mm:ss")} > {content}"  + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("!! Saving Log File Error !! Msg: " + ex.ToString());
                return false;
            }

        }
        #endregion

        #region 讀取log

        public static string ReadLog()
        {
            string logfile = Path.Combine(LogPath, sysLog_Name);
            return File.ReadAllText(logfile);
        }
        #endregion

        #region 加解密
        private static char getRandomChar(string input)
        {
            int random = 0;
            do
            {
                random = new Random().Next() % 128;
            } while (input.IndexOf(random.ToString()) >= 0);
            return (char)random;
        }

        private static string charToHexString(char inChar)
        {
            string outChar = Convert.ToByte(inChar).ToString("X2");
            return outChar;
        }

        public static string encrypt(string input)
        {
            if (Encoding.Default.GetBytes(input).Length % 2 != 0)
            {
                input += " ";
            }
            byte[] inputArray = Encoding.Default.GetBytes(input);
            int length = inputArray.Length;

            char hChar = getRandomChar(input);
            char lChar = getRandomChar(input);
            char[] tempArray = new char[length];
            char[] outputArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                outputArray[i] = (char)inputArray[i];
            }

            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < length; k++)
                {
                    if (k < length / 2)
                    {
                        tempArray[k + length / 2] = outputArray[k];
                    }
                    else
                    {
                        tempArray[k - length / 2] = outputArray[k];
                    }
                }
                for (int j = 0; j < length; j++)
                {
                    if (j % 2 == 0)
                    {
                        outputArray[j] = (char)(tempArray[j] ^ hChar);
                    }
                    else
                    {
                        outputArray[j] = (char)(tempArray[j] ^ lChar);
                    }
                }
            }

            string output = charToHexString(hChar).ToUpper();
            for (int i = 0; i < outputArray.Length; i++)
            {
                output += charToHexString(outputArray[i]).ToUpper();
            }
            output += charToHexString(lChar).ToUpper();
            return output;
        }

        public static string decrypt(string input)
        {
            try
            {
                char hChar = Convert.ToChar(int.Parse(input.Substring(0, 2), NumberStyles.HexNumber));
                char lChar = Convert.ToChar(int.Parse(input.Substring(input.Length - 2), NumberStyles.HexNumber));

                int length = input.Length / 2 - 1;

                char[] retrivePwd = new char[length - 1];

                for (int i = 1; i < length; i++)
                {
                    retrivePwd[i - 1] = Convert.ToChar(int.Parse(input.Substring(i * 2, 2), NumberStyles.HexNumber));
                }

                length = retrivePwd.Length;
                byte[] tempArray = new byte[length];
                char[] outputArray = new char[length];

                for (int i = 0; i < retrivePwd.Length; i++)
                {
                    tempArray[i] = (byte)retrivePwd[i];
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (j % 2 == 0)
                        {
                            outputArray[j] = (char)(tempArray[j] ^ hChar);
                        }
                        else
                        {
                            outputArray[j] = (char)(tempArray[j] ^ lChar);
                        }
                    }

                    for (int k = 0; k < length; k++)
                    {
                        if (k < length / 2)
                        {
                            tempArray[k + length / 2] = (byte)outputArray[k];
                        }
                        else
                        {
                            tempArray[k - length / 2] = (byte)outputArray[k];
                        }
                    }
                }

                if (tempArray.ToString().Trim().Length < 1)
                {
                    return "[ERROR]:DECRYPTION_LENGTH_ZERO";
                }
                else
                {
                    return Encoding.Default.GetString(tempArray).Trim();
                }
            }
            catch (Exception e)
            {
                return "[ERROR]:" + e.Message;
            }
        }

        #endregion

        #region 磅秤相關 目前沒用
        public static string DecimalAryToStr(decimal[] obj)
        {
            string res = "";
            foreach (decimal o in obj)
            {
                res += o + ";";
            }
            if (!string.IsNullOrEmpty(res)) { res = res.Substring(0, res.Length - 1); }
            return res;
        }
        public static decimal[] StrToDecimalAry(string obj)
        {
            string[] str = obj.Split(";");
            decimal[] arr = new decimal[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                arr[i] = Convert.ToDecimal(str[i]);
            }
            return arr;
        }
        #endregion
        public static bool SendMail(string MailContent)
        {
            string sMsg = "";
            string sEMailServer = ConfigurationManager.AppSettings["MailServer"];
            string sSender = ConfigurationManager.AppSettings["MailSender"];
            string sTraceMail = ConfigurationManager.AppSettings["TraceMail"];

            try
            {
                //string sEMailAddr, sMailName;
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(sSender, "MAPS資料轉移通知", Encoding.UTF8);
                    message.Subject = "MAPS資料轉移通知!!";
                    message.SubjectEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    //message.Body = sMessage + "<br/><br/>";
                    message.BodyEncoding = Encoding.UTF8;
                    string[] email = sTraceMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string address in email)
                    {
                        message.To.Add(new MailAddress(address, address, Encoding.UTF8));
                    }

                    string sBody = string.Format("<html><head></head><body><font color='blue'>{0}</font></body></html>", MailContent);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(sBody, Encoding.UTF8, "text/html");
                    message.AlternateViews.Add(htmlView);
                    SmtpClient mailClient = new SmtpClient(sEMailServer, 25);
                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailClient.Send(message);
                    return true;
                }
            }
            catch (FormatException ex)
            {
                sMsg = "FormatException:" + ex.Message;
                if (ex.InnerException != null)
                    sMsg += "," + ex.InnerException.Message;
            }
            catch (SmtpException ex)
            {
                sMsg = "SmtpException:" + ex.Message;
                if (ex.InnerException != null)
                    sMsg += "," + ex.InnerException.Message;
            }
            catch (Exception ex)
            {
                sMsg = ex.Message;
                if (ex.InnerException != null)
                    sMsg += "," + ex.InnerException.Message;
            }
            if (sMsg != "")
            {
                savelog("EMAIL傳送失敗:" + sMsg);
                return false;
            }
            else
            {
                return true;
            }
        }

        #region 相差多少%
        public static decimal DiffPercent(decimal a, decimal b)
        {
            decimal Displacement = a > b ? b : a;
            if (Displacement < 1)
            {
                a = a + Math.Abs(Displacement) + 1;
                b = b + Math.Abs(Displacement) + 1;
            }
            return Math.Round(Math.Abs(a - b) / a, 2);
        }
        #endregion

        #region 醫院的7碼日期轉換 (string)1110512 < > (date)2022/05/12
        public static DateTime EraStringToDate(string era)
        {
            int len = era.Length;
            //if (len != 7 && len != 6) { return; }
            string datestring = $"{Convert.ToInt16(era.Substring(0, len - 4)) + 1911}/" +
                $"{era.Substring(len - 4, 2)}/" +
                $"{era.Substring(len - 2, 2)}";
            return Convert.ToDateTime(datestring);
        }
        public static string DateToEraString(DateTime dt)
        {
            return $"{dt.Year - 1911}{dt.ToString("MMdd")}";
        }

        #endregion

        #region list to string for sql
        public static string ToSqlString<T>(List<T> data, string splitchar)
        {
            if(data.Count == 0) { return ""; }
            string result = "";
            foreach(var item in data)
            {
                if (item != null) { result += item.ToString() + splitchar; }
            }
            return result.Substring(0, result.Length - splitchar.Length);
        }
        #endregion

    }



}

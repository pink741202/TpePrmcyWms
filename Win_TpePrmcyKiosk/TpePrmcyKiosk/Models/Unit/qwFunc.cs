using System;
using System.IO;  //file
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Globalization;
using TpePrmcyWms.Models.DOM;

namespace TpePrmcyKiosk.Models.Unit
{
    static class qwFunc
    {
        static string LogPath = ConfigurationManager.AppSettings["LogPath"] == "" ? "log" : ConfigurationManager.AppSettings["LogPath"];
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
            //string startupPath = Directory.GetCurrentDirectory();  
            try
            {
                File.AppendAllText(Path.Combine(LogPath, sysLog_Name), content + Environment.NewLine);
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
            return System.IO.File.ReadAllText(logfile);
        }
        #endregion

        #region 加解密
        private static char getRandomChar(String input)
        {
            int random = 0;
            do
            {
                random = (int)(((new System.Random()).Next()) % 128);
            } while (input.IndexOf(random.ToString()) >= 0);
            return (char)random;
        }

        private static String charToHexString(char inChar)
        {
            String outChar = Convert.ToByte(inChar).ToString("X2");
            return outChar;
        }

        public static String encrypt(String input)
        {
            if (System.Text.Encoding.Default.GetBytes(input).Length % 2 != 0)
            {
                input += " ";
            }
            byte[] inputArray = System.Text.Encoding.Default.GetBytes(input);
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
                        tempArray[k + (length / 2)] = (char)outputArray[k];
                    }
                    else
                    {
                        tempArray[k - (length / 2)] = (char)outputArray[k];
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

            String output = charToHexString(hChar).ToUpper();
            for (int i = 0; i < outputArray.Length; i++)
            {
                output += charToHexString(outputArray[i]).ToUpper();
            }
            output += charToHexString(lChar).ToUpper();
            return output;
        }

        public static String decrypt(String input)
        {
            try
            {
                char hChar = Convert.ToChar(Int32.Parse(input.Substring(0, 2), NumberStyles.HexNumber));
                char lChar = Convert.ToChar(Int32.Parse(input.Substring(input.Length - 2), NumberStyles.HexNumber));

                int length = input.Length / 2 - 1;

                char[] retrivePwd = new char[length - 1];

                for (int i = 1; i < length; i++)
                {
                    retrivePwd[i - 1] = Convert.ToChar(Int32.Parse(input.Substring(i * 2, 2), NumberStyles.HexNumber));
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
                            tempArray[k + (length / 2)] = (byte)outputArray[k];
                        }
                        else
                        {
                            tempArray[k - (length / 2)] = (byte)outputArray[k];
                        }
                    }
                }

                if (tempArray.ToString().Trim().Length < 1)
                {
                    return "[ERROR]:DECRYPTION_LENGTH_ZERO";
                }
                else
                {
                    return System.Text.Encoding.Default.GetString(tempArray).Trim();
                }
            }
            catch (Exception e)
            {
                return "[ERROR]:" + e.Message;
            }
        }

        #endregion

        public static string DecimalAryToStr(decimal[] obj)
        {
            string res = "";
            foreach(decimal o in obj)
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
            for(int i = 0; i < str.Length; i++)
            {
                arr[i] = Convert.ToDecimal(str[i]);
            }
            return arr;
        }
        public static string toTrustSymbol(bool val) {
            return (val) ? "☑" : "✕";
        }
        public static bool SendMail(string MailContent)
        {
            string sMsg = "";            
            string sEMailServer = ConfigurationManager.AppSettings["MailServer"];
            string sSender = ConfigurationManager.AppSettings["MailSender"];
            string sTraceMail = ConfigurationManager.AppSettings["TraceMail"];

            try
            {
                //string sEMailAddr, sMailName;
                using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage())
                {
                    message.From = new MailAddress(sSender, "MAPS資料轉移通知", Encoding.UTF8);
                    message.Subject = "MAPS資料轉移通知!!";
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.IsBodyHtml = true;
                    //message.Body = sMessage + "<br/><br/>";
                    message.BodyEncoding = Encoding.UTF8;
                    string[] email = sTraceMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string address in email)
                    {
                        message.To.Add(new MailAddress(address, address, Encoding.UTF8));
                    }

                    string sBody = string.Format("<html><head></head><body><font color='blue'>{0}</font></body></html>", MailContent);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(sBody, System.Text.Encoding.UTF8, "text/html");
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
        public static void Alert(string msg)
        {
            FormPopupMessage fm = new FormPopupMessage(msg);
            fm.ShowDialog();
        }
        public static string CheckTextBoxMaxLength(string val, int maxlength)
        {
            try
            {
                if (val.Length > 4)
                {
                    val = val.Substring(val.Length - 4);
                }
                return val;
            }
            catch (Exception ex) { }
            return "";
        }
        public static bool LimitKeyIn_Num_Point(KeyPressEventArgs e, string text)
        {            
            if (!((e.KeyChar > 47 && e.KeyChar < 58) || e.KeyChar == 8 || e.KeyChar == 46)) { return true; }
            else if (e.KeyChar == 46 && (text.Contains(".") || text.StartsWith("."))) { return true; }
            return false;
        }
        #region autoda modbus 用,16進轉10進,精度為0.00
        public static decimal HexToDecimal(string hex)
        {
            int getIntValue = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return (decimal)getIntValue / 100;
        }
        #endregion

        public static decimal DiffPercent(decimal a, decimal b)
        {
            decimal Displacement = a > b ? b : a;
            if (Displacement < 1) {
                a = a + Math.Abs(Displacement) + 1;
                b = b + Math.Abs(Displacement) + 1;
            }
            return Math.Round(Math.Abs(a - b) / a, 2);
        }

        #region 換算單位數量
        public static bool CalculateWeightToQty(
            decimal Weight, decimal? UnitW, decimal? UnitQ,
            out decimal ResultQty, out decimal ResultTolerance, out bool ResultTrustable,
            out string msg
            )
        {
            ResultQty = 0;
            ResultTolerance = 0;
            ResultTrustable = false;
            msg = string.Empty;
            int pointnum = 2; //取到小數位

            Weight = Math.Round(Weight, pointnum) * -1;
            int TolerancePercent = Convert.ToInt16(ConfigurationManager.AppSettings["TolerancePercent"]);
            double Dividend = (double)Weight; //被除數
            double Divisor = (double)UnitW; //除數
            double EachQ = (double)UnitQ; //乘數
            double TakeOrPut = (Dividend == 0) ? 1 : (Dividend / Math.Abs(Dividend));

            try
            {
                double Quotient = Math.Floor(Math.Abs(Dividend) / Divisor) * TakeOrPut; //商數
                double Remainder = Math.Abs(Dividend) % Divisor; //餘數(差)


                //餘數如果跟單顆重差5%以上 = 重量不準
                double RemainderPersent = Math.Round((Remainder / Divisor) * 100, 2);
                if (RemainderPersent > 80) { Quotient += TakeOrPut; } //當做多一顆

                ResultQty = Convert.ToDecimal(Quotient * EachQ);
                ResultTolerance = Convert.ToDecimal(RemainderPersent);
                ResultTrustable = RemainderPersent > (100 - TolerancePercent) || RemainderPersent < TolerancePercent;

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            return true;
        }
        #endregion

        #region 取得顏色
        public static Color toColor(string val)
        {
            Color result = new Color();
            switch (val)
            {
                case "Green": result = Color.Green; break;
                case "Yellow": result = Color.Yellow; break;
                case "Blue": result = Color.Blue; break;
                case "Orange": result = Color.Orange; break;
                case "Red": result = Color.Red; break;
                case "OrangeRed": result = Color.OrangeRed; break;

            }
            return result;
        }
        #endregion

        #region int to hex string "00 00"
        public static string toHexWithSpace(int val)
        {
            string hex = val.ToString("X4");
            return $"{hex.Substring(0, 2)} {hex.Substring(2, 2)}";
        }
        #endregion

    }



}

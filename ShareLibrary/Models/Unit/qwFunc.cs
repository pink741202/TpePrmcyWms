using System;
using System.IO;  //file
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace ShareLibrary.Models.Unit
{
    static class qwFunc
    {
        static string LogPath = ConfigurationManager.AppSettings["LogPath"] == "" ? "log" : ConfigurationManager.AppSettings["LogPath"];
        static string sysLog_Name = "ShareLibraryLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";


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


        #endregion

        #region 讀取log

        public static string ReadLog()
        {
            string logfile = Path.Combine(LogPath, sysLog_Name);            
            return System.IO.File.ReadAllText(logfile);
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

    }



}

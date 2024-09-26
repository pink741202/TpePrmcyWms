using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Unit.Back;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Diagnostics;

namespace TpePrmcyWms.Models.Service
{
    public class SysBaseServ
    {
        #region sql injection
        public static string ChkSqlInject(string tmpstr)
        {
            tmpstr = tmpstr == null ? "" : tmpstr;
            string replaceparams = "ALERT|SCRIPT|WINDOW|ONMOUSEOVER|UPDATE |INSERT |DELETE | OR |DECLARE|EXEC|DROP |'|%|;|\"|\\|\'";
            Regex rgx = new Regex(@"" + replaceparams, RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(tmpstr))
            {
                tmpstr = rgx.Replace(tmpstr, String.Empty);
                tmpstr = tmpstr.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace("\\", "&#092;");
            }
            return tmpstr;
        }
        #endregion

        #region operate/error log
        public static void Log(LoginInfo info, Exception ex, string? msg = "")
        {
            Log(info, "", null, msg, ex);
        }
        public static void Log(LoginInfo info, string OperateType, Exception ex, string? msg = "")
        {
            Log(info, OperateType, false, msg, ex);
        }        
        public static void Log(LoginInfo? info, string operateType,  bool? operateSuccess = null, string? msg = "",  Exception? ex = null)
        {
            try
            {
                string errorClass = "", errorFunc = "";
                if (ex != null) {
                    var st = new StackTrace(ex, true);
                    var frame = st.GetFrame(0);
                    errorClass = frame?.GetMethod()?.DeclaringType?.FullName ?? "";
                    errorFunc = frame?.GetMethod()?.Name ?? "";
                }

                OperateLog ins = new OperateLog();
                ins.OperateType = operateType;
                if (info == null)
                {
                    ins = new OperateLog
                    {
                        LogType = "Operate",
                        OperateType = "Auth",
                        OperateSuccess = false,
                        ErrorClass = errorClass,
                        ErrorFunction = errorFunc,
                        LogMsg = msg,
                        ErrorMsg = ex != null ? ex.Message : "",
                        ErrorTrace = ex != null && ex.StackTrace != null ? ex.StackTrace.ToString() : "",
                        LogTime = DateTime.Now,
                    };
                }
                else
                {
                    ins = new OperateLog
                    {
                        empFid = info.User.Fid,
                        LinkMethod = info.Linkinfo.ContrlName == "" ? "" : $"{info.Linkinfo.ContrlName},{info.Linkinfo.ActionName}({info.Linkinfo.Method})",
                        LogType = ex != null ? "Exception" : "Operate",
                        OperateType = operateType,
                        OperateSuccess = ex != null ? false : operateSuccess,
                        ErrorClass = errorClass,
                        ErrorFunction = errorFunc,
                        LogMsg = msg,
                        ErrorMsg = ex != null ? ex.Message : "",
                        ErrorTrace = ex != null && ex.StackTrace != null ? ex.StackTrace.ToString() : "",
                        LogTime = DateTime.Now,
                        comFid = info.User.comFid
                    };
                }
                
                using (DBcPharmacy db = new DBcPharmacy())
                {
                    db.OperateLog.Add(ins);
                    db.SaveChanges();
                }
            }
            catch(Exception x) {
                
            }
        }
        
        #endregion

        #region 取得環境設定
        public static string JsonConfConnString(string ConnectionStringName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            return configuration.GetConnectionString(ConnectionStringName) ?? "";
        }
        public static string JsonConf(string ConfigName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            return configuration.GetValue<string>(ConfigName) ?? "";
        }
        #endregion

        #region 目錄


        #endregion

        #region 密碼加密
        public static string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        #endregion

    }
}

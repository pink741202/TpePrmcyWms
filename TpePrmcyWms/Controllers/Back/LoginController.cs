using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using static Dapper.SqlMapper;

namespace TpePrmcyWms.Controllers.Back
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index(int AtCbntFid)
        {
            int from_kiosk = AtCbntFid; //priarty 1
            int from_config = Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid")); //priarty 2
            int final = from_kiosk != 0 ? from_kiosk : from_config;
            HttpContext.Session.SetInt32("AtCbntFid", final);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginObj data)
        {
            data.UserAcc = SysBaseServ.ChkSqlInject(data.UserAcc);
            data.Password = SysBaseServ.ChkSqlInject(data.Password);
            data.CardNo = SysBaseServ.ChkSqlInject(data.CardNo);
            int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;

            using (DBcPharmacy con = new DBcPharmacy())
            {
                LoginInfo Loginfo = new LoginInfo();
                try
                {
                    string encodepw = SysBaseServ.GenerateSHA512String(data.Password);
                    employee? user = null;
                    if (data.CardNo != "" && user == null)
                    {
                        user = con.employee.Where(x => x.CardNo == data.CardNo && x.ifuse).FirstOrDefault();
                    }
                    if (data.UserAcc != "" && data.Password != "" && user == null)
                    {
                        user = con.employee.Where(x => x.empacc == data.UserAcc && x.opensesame == encodepw && x.ifuse).FirstOrDefault();
                    }
                    if (user == null)
                    {
                        TempData["AlertMsg"] = "登入者錯誤，請重新輸入";
                        return RedirectToAction("Index");
                    }
                    //登入者資訊
                    if (!Loginfo.set(user, AtCbntFid)) { TempData["AlertMsg"] = $"取得權限發生錯誤"; return RedirectToAction("Index"); }
                }
                catch (Exception ex) {
                    SysBaseServ.Log(Loginfo, "L", ex);
                    TempData["AlertMsg"] = $"登入錯誤:{ex.Message}";
                    return RedirectToAction("Index");
                }

                //登入者IP
                string LoginIP = HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? "";
                string LoginIP2 = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                if (LoginIP == "::1") { LoginIP = "127.0.0.1"; }
                Loginfo.FromIP = LoginIP;

                HttpContext.Session.SetString("Loginfo", Newtonsoft.Json.JsonConvert.SerializeObject(Loginfo));

                if (Loginfo.Trees == null || Loginfo.Trees.Count == 0)
                {
                    SysBaseServ.Log(Loginfo, "L", false,  "權限不足");
                    TempData["AlertMsg"] = "權限不足,請洽系統人員!!";
                    return RedirectToAction("Index");
                }
                MenuTree t = Loginfo.Trees.Where(x => !string.IsNullOrEmpty(x.L1Link) || !string.IsNullOrEmpty(x.L2Link)).FirstOrDefault();
                string LinkTo = t.L1Link.Length > 0 ? t.L1Link : t.L2Link;
                SysBaseServ.Log(Loginfo, "L", true);
                return RedirectToAction((LinkTo.Contains(",") ? LinkTo.Split(',')[1] : "Index"), LinkTo.Split(',')[0]);
            }
        }
        public ActionResult logout(string msg)
        {
            TempData["AlertMsg"] = string.IsNullOrEmpty(msg) ? "" : msg;
            HttpContext.Session.Remove("Loginfo");
            HttpContext.Session.Remove("AtCbntFid");
            return RedirectToAction("Index");
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;

namespace TpePrmcyWms.Controllers
{
    public class BaseController : Controller
    {
        public LoginInfo Loginfo { get; set; }
        public int AtCbntFid { get; set; }
        public BaseController() : base()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context != null)
            {
                #region 判斷登入session資訊是否正確
                string errormsg = "";
                string loginfojson = HttpContext.Session.GetString("Loginfo") ?? "";
                if (string.IsNullOrEmpty(loginfojson))
                {
                    errormsg = "登入者資訊不存在，已自動退出系統"; TempData["AlertMsg"] = errormsg; HttpContext.Session.Remove("Loginfo");
                    SysBaseServ.Log(Loginfo, "Auth", false, errormsg);
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                    return;
                }
                try { 
                    Loginfo = JsonConvert.DeserializeObject<LoginInfo>(loginfojson); 
                    if (Loginfo == null) {
                        errormsg = "登入者資訊取得失敗，已自動退出系統"; TempData["AlertMsg"] = errormsg; HttpContext.Session.Remove("Loginfo");
                        SysBaseServ.Log(Loginfo, "Auth", false, "登入者資訊取得失敗，已自動退出系統");
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                        return; 
                    }
                }
                catch {                    
                    errormsg = "登入者資訊錯誤，已自動退出系統"; TempData["AlertMsg"] = errormsg; HttpContext.Session.Remove("Loginfo");
                    SysBaseServ.Log(Loginfo, "Auth", false, errormsg);
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                    return;
                }
                
                #endregion
                #region 判斷權限並寫入session
                try
                {
                    string ctrlerName = context.RouteData.Values["controller"]?.ToString() ?? "";
                    string actionName = context.RouteData.Values["action"]?.ToString() ?? "";
                    string method = context.HttpContext.Request.Method;
                    string CtrlerActionForCompare = $"{ctrlerName},{(actionName.StartsWith("Sub") ? $"List{actionName.Substring(3)}" : actionName)}".ToLower();
                    MenuTree? t = Loginfo?.Trees.Where(x => CtrlerActionForCompare.StartsWith(x.L2Link.ToLower())).FirstOrDefault();
                    if (t is not null || CtrlerActionForCompare == "employee,person") //個人資訊不需權限
                    {
                        Loginfo.Linkinfo.MenuLFid = t.L2fid;
                        Loginfo.Linkinfo.ActionName = actionName;
                        Loginfo.Linkinfo.ContrlName = ctrlerName;
                        Loginfo.Linkinfo.Method = method;
                        HttpContext.Session.SetString("Loginfo", JsonConvert.SerializeObject(Loginfo));
                        ViewBag.Loginfo = Loginfo;
                        #region 頁面操作權限
                        AuthCatelog Auths = Loginfo.AuthDetail.Where(x => x.MenuLFid == t.L2fid).First();
                        ViewBag.OperateAuths = new Dictionary<string, bool>()
                                                    {
                                                        { "Queryable", Auths.Queryable },
                                                        { "Creatable", Auths.Creatable },
                                                        { "Updatable", Auths.Updatable },
                                                        { "Deletable", Auths.Deletable },
                                                    };
                        ViewBag.OperateSystem = t.System;
                        #endregion
                        //前台only
                        if (t.System == "FRONTEND") { AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0; }
                    }
                    else
                    {
                        TempData["AlertMsg"] = "無權限進入頁面，已自動退出系統";
                        SysBaseServ.Log(Loginfo, "Auth", false, "無權限進入頁面，已自動退出系統");
                        HttpContext.Session.Remove("Loginfo");
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                    }
                }
                catch (Exception ex)
                {
                    SysBaseServ.Log(Loginfo, "Auth", ex);
                }
                #endregion
            }
        }
    }
}

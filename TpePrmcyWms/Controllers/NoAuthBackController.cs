using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Unit.Back;

namespace TpePrmcyWms.Controllers
{
    public class NoAuthBackController : Controller
    {
        public LoginInfo Loginfo { get; set; }
        public int AtCbntFid = 0;
        public NoAuthBackController() : base()
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
                    errormsg = "登入者資訊不存在，已自動退出系統";
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                    return;
                }
                try { Loginfo = JsonConvert.DeserializeObject<LoginInfo>(loginfojson); }
                catch { errormsg = "登入者資訊錯誤，已自動退出系統"; }
                if (errormsg != "")
                {
                    TempData["AlertMsg"] = errormsg;
                    HttpContext.Session.Remove("Loginfo");
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Login" }, { "Action", "Index" } });
                    return;
                }
                #endregion

                
            }
        }
    }
}

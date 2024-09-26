using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TpePrmcyWms.Filters
{
    public class FrontendSessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int AtCbntFid = filterContext.HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            if (AtCbntFid == 0)
            {
                filterContext.Result = new RedirectToRouteResult
                    (new RouteValueDictionary(new { 
                        controller = "Login", 
                        action = "logout", 
                        msg = "櫃位設定已消失，請重新開啟軟體" 
                    }));
            }
            else
            {
                filterContext.HttpContext.Session.SetInt32("AtCbntFid", AtCbntFid);
            }
        }
        //櫃位設定已消失,請重新開啟軟體
    }
}

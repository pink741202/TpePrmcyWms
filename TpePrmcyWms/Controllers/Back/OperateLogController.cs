using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;


namespace TpePrmcyWms.Controllers.Back
{
    public class OperateLogController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString
            , DateTime? qDate1, DateTime? qDate2, string qType)
        {
            IQueryable<OperateLog> obj = _db.OperateLog.Where(x => x.comFid == Loginfo.User.comFid);

            #region 查詢          
            ViewData["qKeyString"] = qKeyString ?? "";
            if (!String.IsNullOrEmpty(qKeyString))
            {
                obj = obj.Where(s => s.LinkMethod.ToUpper().StartsWith(qKeyString.ToUpper()));
            }

            ViewData["qDate1"] = qDate1;
            if (qDate1 != null)
            {
                ViewData["qDate1"] = ((DateTime)qDate1).ToString("yyyy-MM-dd");
                obj = obj.Where(s => s.LogTime >= qDate1);
            }

            ViewData["qDate2"] = qDate2;
            if (qDate2 != null)
            {
                ViewData["qDate2"] = ((DateTime)qDate2).ToString("yyyy-MM-dd");
                obj = obj.Where(s => s.LogTime >= qDate2);
            }

            ViewData["qType"] = qType ?? "";
            if (!String.IsNullOrEmpty(qType))
            {
                switch (qType)
                {
                    case "O":
                        obj = obj.Where(s => s.ErrorMsg == string.Empty);
                        break;
                    case "E":
                        obj = obj.Where(s => s.ErrorMsg != string.Empty);
                        break;
                };                
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "LogTime_desc": obj = obj.OrderByDescending(s => s.LogTime); break;
                case "LogTime": obj = obj.OrderBy(s => s.LogTime); break;
                default: sortOrder = "LogTime"; obj = obj.OrderByDescending(s => s.LogTime); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<OperateLog>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            OperateLog obj = _db.OperateLog.Find(fid);
            return obj == null ? View(new OperateLog()) : View(obj);
        }
        
        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            OperateLog? obj = _db.OperateLog.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new { code = 1, message = "刪檔失敗" }); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                return Json(new { code = 0, message = "刪檔成功" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid}");
                return Json(new { code = 2, message = "刪檔失敗" });
            }
        }
        #endregion

    }
}

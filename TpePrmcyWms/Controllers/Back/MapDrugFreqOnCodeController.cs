using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;


namespace TpePrmcyWms.Controllers.Back
{
    public class MapDrugFreqOnCodeController : BaseController
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly DBcPharmacy _db = new DBcPharmacy();
        public MapDrugFreqOnCodeController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString)
        {
            IQueryable<MapDrugFreqOnCode> obj = _db.MapDrugFreqOnCode;

            #region 查詢
            
            ViewData["qKeyString"] = qKeyString;
            if (!String.IsNullOrEmpty(qKeyString) && obj != null)
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.HsptlFreqCode ?? "").Contains(qKeyString)
                      || (s.FreqName ?? "").Contains(qKeyString)
                );
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "HsptlFreqCode_desc": obj = obj.OrderByDescending(s => s.HsptlFreqCode); break;
                case "HsptlFreqCode": obj = obj.OrderBy(s => s.HsptlFreqCode); break;
                case "FreqTimeDesc_desc": obj = obj.OrderByDescending(s => s.FreqTimeDesc); break;
                case "FreqTimeDesc": obj = obj.OrderBy(s => s.FreqTimeDesc); break;
                default: sortOrder = "HsptlFreqCode"; obj = obj.OrderBy(s => s.HsptlFreqCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<MapDrugFreqOnCode>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            MapDrugFreqOnCode? obj = _db.MapDrugFreqOnCode.Find(fid);
            return obj == null ? View(new MapDrugFreqOnCode()) : View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(MapDrugFreqOnCode vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.MapDrugFreqOnCode.Any(i => i.HsptlFreqCode == vobj.HsptlFreqCode && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.HsptlFreqCode), "院頻代碼已存在!");
            }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value?.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{vobj.HsptlFreqCode}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{vobj.HsptlFreqCode}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            MapDrugFreqOnCode? obj = _db.MapDrugFreqOnCode.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{obj.HsptlFreqCode}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{obj.HsptlFreqCode}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

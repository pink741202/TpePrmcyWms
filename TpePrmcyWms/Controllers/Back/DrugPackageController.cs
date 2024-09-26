using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TpePrmcyWms.Controllers.Back
{
    public class DrugPackageController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString)
        {
            IQueryable<DrugPackage> obj = _db.DrugPackage.Where(x=>x.FID > 0);

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                List<int> drugfids = _db.DrugInfo.Where(s => (s.DrugName ?? "").Contains(qKeyString) || (s.DrugCode ?? "").Contains(qKeyString)).Select(s => s.FID).ToList();
                obj = obj.Where(x => x.UnitTitle.Contains(qKeyString) || drugfids.Contains(x.DrugFid)
                || (x.BarcodeNo ?? "").Contains(qKeyString));
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "moddate_desc": obj = obj.OrderByDescending(s => s.moddate); break;
                case "moddate": obj = obj.OrderBy(s => s.moddate); break;
                case "DrugFid_desc": obj = obj.OrderByDescending(s => s.DrugFid); break;
                case "DrugFid": obj = obj.OrderBy(s => s.DrugFid); break;
                default: sortOrder = "DrugFid"; obj = obj.OrderBy(s => s.DrugFid); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<DrugPackage>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            DrugPackage obj = _db.DrugPackage.Find(fid);
            if (obj == null)
            {
                obj = new DrugPackage();
            }
            else
            {
                DrugInfo? qdrug = _db.DrugInfo.Find(obj.DrugFid);
                if (qdrug != null)
                {
                    obj.DrugName = $"{qdrug.DrugCode}，{qdrug.DrugName}，{qdrug.FID}";
                }
            }
            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(DrugPackage vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            try
            {
                vobj.DrugFid = Convert.ToInt32(vobj.DrugName?.Split("，")[2]);
            }
            catch { ModelState.AddModelError(nameof(vobj.DrugFid), "請選擇正確藥品名稱!"); }
            
            if (vobj.DrugFid == 0) { ModelState.AddModelError(nameof(vobj.DrugFid), "請選擇正確藥品名稱!"); }
            if (_db.DrugPackage.Any(i => i.DrugFid == vobj.DrugFid && i.UnitQty == vobj.UnitQty && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.DrugFid), "此設定已存在!");
            }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            vobj.moddate = DateTime.Now;
            vobj.modid = Loginfo.User.Fid;

            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            string drugcode = _db.DrugInfo.Find(vobj.DrugFid)?.DrugCode ?? "";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{drugcode}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{drugcode}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            DrugPackage obj = _db.DrugPackage.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new { code = 1, message = "刪檔失敗" }); }
            
            string drugcode = _db.DrugInfo.Find(obj.DrugFid).DrugCode;
            decimal qty = obj.UnitQty;
            string title = obj.UnitTitle;
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{drugcode}/{qty} {title}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{drugcode}/{qty} {title}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

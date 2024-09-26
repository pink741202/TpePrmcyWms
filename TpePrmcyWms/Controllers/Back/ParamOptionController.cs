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
    public class ParamOptionController : BaseController
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
            IQueryable<ParamOption> obj = _db.ParamOption.Where(x => x.SysType != "BaseSys");

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!String.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.GroupCode ?? "").Contains(qKeyString) 
                      || (s.GroupName ?? "").Contains(qKeyString)
                      || s.OptionCode.Contains(qKeyString)
                      || s.OptionName.Contains(qKeyString)
                );
            }
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "GroupCode_desc": obj = obj.OrderByDescending(s => s.GroupCode).ThenBy(s => s.Sorting); break;
                case "GroupCode": obj = obj.OrderBy(s => s.GroupCode).ThenBy(s => s.Sorting); break;
                case "GroupName_desc": obj = obj.OrderByDescending(s => s.GroupName).ThenBy(s => s.Sorting); break;
                case "GroupName": obj = obj.OrderBy(s => s.GroupName).ThenBy(s => s.Sorting); break;
                case "OptionCode_desc": obj = obj.OrderByDescending(s => s.OptionCode); break;
                case "OptionCode": obj = obj.OrderBy(s => s.OptionCode); break;
                case "OptionName_desc": obj = obj.OrderByDescending(s => s.OptionName); break;
                case "OptionName": obj = obj.OrderBy(s => s.OptionName); break;
                default: sortOrder = "GroupName"; obj = obj.OrderBy(s => s.GroupName).ThenBy(s => s.Sorting); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<ParamOption>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            ParamOption? obj = _db.ParamOption.Find(fid);
            if (obj == null)
            {
                obj = new ParamOption();
                obj.SysType = "Cust";
            }

            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(ParamOption vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.ParamOption.Any(i => i.FID != vobj.FID && i.GroupCode == vobj.GroupCode && i.OptionCode == vobj.OptionCode  ))
            {
                ModelState.AddModelError(nameof(vobj.OptionCode), "參數代碼已存在!");
            }
            if (_db.ParamOption.Any(i => i.FID != vobj.FID && i.GroupCode == vobj.GroupCode && i.OptionName == vobj.OptionName))
            {
                ModelState.AddModelError(nameof(vobj.OptionName), "參數名稱已存在!");
            }
            if (_db.ParamOption.Any(i => i.FID != vobj.FID && i.GroupCode == vobj.GroupCode && i.GroupName != vobj.GroupName))
            {
                ModelState.AddModelError(nameof(vobj.OptionName), "群組代碼已存在但群組名稱錯誤!");
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
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); } //create
                else { _db.Update(vobj); }
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{vobj.GroupName}-{vobj.OptionName}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{vobj.GroupName}-{vobj.OptionName}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            ParamOption? obj = _db.ParamOption.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{obj.GroupName}-{obj.OptionName}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{obj.GroupName}-{obj.OptionName}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

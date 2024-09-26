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
    public class DepartmentController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString, int qcomFid)
        {
            IQueryable<Department> obj = _db.Department;

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!String.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.dptid ?? "").Contains(qKeyString) || (s.dpttitle ?? "").Contains(qKeyString)
                );
            }
            ViewData["qcomFid"] = qcomFid;
            if (qcomFid != 0)
            {
                obj = obj.Where(s => s.comFid == qcomFid);
            }
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "dptid_desc": obj = obj.OrderByDescending(s => s.dptid); break;
                case "dptid": obj = obj.OrderBy(s => s.dptid); break;
                case "dpttitle_desc": obj = obj.OrderByDescending(s => s.dpttitle); break;
                case "dpttitle": obj = obj.OrderBy(s => s.dpttitle); break;
                default: sortOrder = "dptid"; obj = obj.OrderBy(s => s.dptid); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<Department>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            Department? obj = _db.Department.Find(fid);
            if (obj == null)
            {
                obj = new Department();
                obj.comFid = Loginfo.User.comFid ?? 0;
            }

            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(Department vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.Department.Any(i => i.dptid == vobj.dptid && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.dptid), "部門代碼已存在!");
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
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{vobj.dpttitle}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{vobj.dpttitle}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            Department? obj = _db.Department.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{obj.dpttitle}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{obj.dpttitle}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

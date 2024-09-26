using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;


namespace TpePrmcyWms.Controllers.Back
{
    public class CompanyController : BaseController
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly DBcPharmacy _db = new DBcPharmacy();
        public CompanyController(IWebHostEnvironment environment)
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
            IQueryable<Company> obj = _db.Company;

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!String.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.comid ?? "").Contains(qKeyString)
                      || (s.comtitle ?? "").Contains(qKeyString)
                      || (s.comsttitle ?? "").Contains(qKeyString)
                );
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "comid_desc": obj = obj.OrderByDescending(s => s.comid); break;
                case "comid": obj = obj.OrderBy(s => s.comid); break;
                case "comtitle_desc": obj = obj.OrderByDescending(s => s.comtitle); break;
                case "comtitle": obj = obj.OrderBy(s => s.comtitle); break;
                default: sortOrder = "comid"; obj = obj.OrderBy(s => s.comid); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<Company>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            Company obj = _db.Company.Find(fid);
            return obj == null ? View(new Company()) : View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(Company vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.Company.Any(i => i.comid == vobj.comid && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.comid), "統編已存在!");
            }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            #region 檔案處理
            if(logopic != null && logopic.Length > 0)
            {
                string uploads = Path.Combine(_hostingEnvironment.WebRootPath, @"UploadFiles\CompLogo");
                string filePath = Path.Combine(uploads, logopic.FileName);
                try
                {
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logopic.CopyToAsync(fileStream);
                    }
                    vobj.logopic = logopic.FileName;
                }
                catch (Exception ex)
                {
                    qwServ.savelog($"存檔失敗[{filePath}]:{ex.ToString()}");
                }
                
            }
            #endregion

            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"key={vobj.FID} name={vobj.comtitle}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"key={vobj.FID} name={vobj.comtitle}");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            Company obj = _db.Company.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"key={fid} name={obj.comtitle}");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex);
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

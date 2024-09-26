using Dapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TpePrmcyWms.Models.Unit;

namespace TpePrmcyWms.Controllers.Back
{
    public class AuthRoleController : BaseController
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
            IQueryable<AuthRole> obj = _db.AuthRole.Where(x => x.comFid == Loginfo.User.comFid);

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => s.RoleName.Contains(qKeyString)
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
                case "RoleName_desc": obj = obj.OrderByDescending(s => s.RoleName); break;
                case "RoleName": obj = obj.OrderBy(s => s.RoleName); break;
                default: sortOrder = "RoleName"; obj = obj.OrderBy(s => s.RoleName); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<AuthRole>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            AuthRole obj = _db.AuthRole.Find(fid);
            if (obj == null)
            {
                obj = new AuthRole();
                obj.comFid = Loginfo.User.comFid ?? 0; 
            }

            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(AuthRole vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.AuthRole.Any(i => i.RoleName == vobj.RoleName && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.RoleName), "統編已存在!");
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
                SysBaseServ.Log(Loginfo, edittype, true, vobj.RoleName);
                return Json(new { code = 0, message = "存檔成功" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, vobj.RoleName);
                return Json(new { code = "ex", message = "存檔失敗" });
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            AuthRole obj = _db.AuthRole.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new { code = 1, message = "刪檔失敗" }); }
            string edittype = "D";
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{fid} [{obj.RoleName}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{fid} [{obj.RoleName}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

        #region 權限設定
        [HttpGet, ActionName("ListAuthEdit")]
        public async Task<IActionResult> ListAuthEditGet(int fid)
        {
            if (fid == 0) { RedirectToAction("List"); }

            List<MenuTree> Trees = new List<MenuTree>();
            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
            {
                string sql = "select ml.MnTFid MtFid, mt.MenuName MtName, mt.link as MtLink " +
                ", ml.fid as L1fid, ml.CatelogName as L1Name, ml.Link as L1Link " +
                ", isnull(ml2.fid,0) as L2fid, isnull(ml2.CatelogName,'') as L2Name, isnull(ml2.link,'') as L2Link " +
                ", mt.System " +
                "from MenuLeft ml " +
                "left join MenuTop mt on mt.FID = ml.MnTFid " +
                "left join MenuLeft ml2 on ml.fid = ml2.upFID and ml2.LAYER = 2 " +
                "where isnull(ml.MnTFid,'') <> '' and ml.LAYER = 1 and mt.System = 'BACKEND' " +
                "order by ml.MnTFid, ml.sorting, ml2.sorting ";
                Trees = conn.Query<MenuTree>(sql).ToList();

                //檢查 
                foreach (MenuTree t in Trees)
                {
                    if (t.L2Link.IndexOf(',') < 0) { t.L2Link += ",List"; }
                }
            }
            ViewBag.RoleAuth = _db.AuthCatelog.Where(x => x.RoleFid == fid).ToList();
            ViewBag.RoleInfo = _db.AuthRole.Where(x => x.FID == fid).First();

            return View(Trees);
        }
        [HttpPost, ActionName("ListAuthEdit")]
        public async Task<JsonResult> ListAuthEditPost(int fid, List<int> AllFid,
            List<int> Query, List<int> Crate, List<int> Updte, List<int> Delet)
        {
            #region 驗證判斷
            if (fid == 0)
            {
                return Json(new ResponObj<string>("invalid", "錯誤"));
            }
            #endregion

            #region 存檔
            AuthRole? role = new AuthRole();
            string edittype = "U";
            try
            {
                role = _db.AuthRole.Find(fid);
                foreach (int tree in AllFid)
                {
                    if (tree == 0) { continue; }
                    
                    try
                    {
                        AuthCatelog? rec = await _db.AuthCatelog.Where(x => x.RoleFid == fid && x.MenuLFid == tree).FirstOrDefaultAsync();
                        if (rec != null)
                        {
                            rec.Queryable = Query.Contains(tree);
                            rec.Creatable = Crate.Contains(tree);
                            rec.Updatable = Updte.Contains(tree);
                            rec.Deletable = Delet.Contains(tree);
                            rec.modid = Loginfo.User.Fid;
                            rec.moddate = DateTime.Now;
                        }
                        else
                        {
                            edittype = "C";
                            rec = new AuthCatelog()
                            {
                                RoleFid = fid,
                                MenuLFid = tree,
                                Queryable = Query.Contains(tree),
                                Creatable = Crate.Contains(tree),
                                Updatable = Updte.Contains(tree),
                                Deletable = Delet.Contains(tree),
                                modid = Loginfo.User.Fid,
                                moddate = DateTime.Now,
                            };
                            _db.AuthCatelog.Add(rec);
                        };
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SysBaseServ.Log(Loginfo, edittype, ex, $"LeftMenuFid:{tree}" );
                    }
                }
                SysBaseServ.Log(Loginfo, edittype, true, $"{role?.RoleName}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"{role?.RoleName}");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion

        }
        #endregion

    }
}

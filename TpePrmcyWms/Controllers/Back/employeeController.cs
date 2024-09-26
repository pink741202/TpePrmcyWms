using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using System.Linq;
using TpePrmcyWms.Models.Unit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace TpePrmcyWms.Controllers.Back
{
    public class employeeController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        
        //ref: https://learn.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region 個人資訊
        [HttpGet, ActionName("person")]
        public async Task<IActionResult> personView()
        {
            employee obj = new employee();
            using (DBcPharmacy db = new DBcPharmacy())
            {
                obj = await db.employee.FirstOrDefaultAsync(m => m.FID == Loginfo.User.Fid);
            }
            if (obj == null)
            {
                return NotFound();
            }
                  
            return View(obj);
        }

        [HttpPost, ActionName("person")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> personPost(int? fid)
        {            
            if (fid == null)
            {
                return NotFound();
            }

            using (DBcPharmacy db = new DBcPharmacy())
            {
                employee obj = db.employee.Find(fid);
                if (await TryUpdateModelAsync<employee>(obj, ""))
                {
                    try
                    {
                        obj.modid = Loginfo.User.Fid;
                        obj.moddate = DateTime.Now;
                        await db.SaveChangesAsync();
                        TempData["AlertMsg"] = "存檔成功";
                        SysBaseServ.Log(Loginfo, "U", true);
                    }
                    catch (Exception ex)
                    {
                        SysBaseServ.Log(Loginfo, "U", ex);
                        TempData["AlertMsg"] = "存檔失敗";
                        return RedirectToAction("person");
                    }
                    
                    return RedirectToAction("person");

                }
                else
                {
                    var errors = ModelState.Select(e => e.Value.Errors);                    
                    return View(obj);
                }
                
                
            }
                
        }
        #endregion

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString, int qRoleFid)
        {
            IQueryable<employee> obj = _db.employee.Where(x => x.comFid == Loginfo.User.comFid);

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.name ?? "").Contains(qKeyString)
                      || (s.empacc ?? "").Contains(qKeyString)
                      || (s.enname ?? "").Contains(qKeyString)
                );
            }
            ViewData["qRoleFid"] = qRoleFid;
            if (qRoleFid != 0)
            {
                obj = obj.Where(s => s.RoleFid == qRoleFid);
            }
            #endregion

            #region 排序
            ViewData["sortOrder"] = sortOrder;
            switch (sortOrder)
            {
                case "name_desc": obj = obj.OrderByDescending(s => s.name); break;
                case "name": obj = obj.OrderBy(s => s.name); break;
                case "empacc_desc": obj = obj.OrderByDescending(s => s.empacc); break;
                case "empacc": obj = obj.OrderBy(s => s.empacc); break;
                case "enname_desc": obj = obj.OrderByDescending(s => s.enname); break;
                case "enname": obj = obj.OrderBy(s => s.enname); break;
                default: obj = obj.OrderBy(s => s.name); break;
            }
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<employee>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            employee obj = _db.employee.Find(fid);
            if(obj == null)
            {
                obj = new employee();
                obj.comFid = Loginfo.User.comFid;
            }

            return View(obj);
        }

        [HttpPost, ActionName("ListEditX")] //初版一般用,若用ajax就不用這個function
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListEditPost(int fid , employee vobj)
        {
            //if (!ModelState.IsValid) { var errors = ModelState.Select(e => e.Value.Errors); return View(vobj); }
            
            string edittype = "U";
            try
            {
                if (fid == 0)
                { //create
                    edittype = "C";
                    vobj.empstatus = "1"; //預設,目前沒用
                    vobj.emptype = "1"; //預設,目前沒用
                    vobj.modid = Loginfo.User.Fid;
                    vobj.moddate = DateTime.Now;
                    _db.Add(vobj);
                    await _db.SaveChangesAsync();
                }
                else
                { //edit
                    employee obj = _db.employee.Find(fid);
                    obj.opensesame = SysBaseServ.GenerateSHA512String(obj.opensesame);
                    obj.modid = Loginfo.User.Fid;
                    obj.moddate = DateTime.Now;
                    if (await TryUpdateModelAsync<employee>(obj, ""))
                    {
                        await _db.SaveChangesAsync();
                    }
                    else { var errors = ModelState.Select(e => e.Value.Errors); return View(vobj); }
                }
                TempData["AlertMsg"] = "存檔成功";
                SysBaseServ.Log(Loginfo, edittype, true);
                return RedirectToAction("ListEdit", new { fid= vobj.FID});
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex);
                TempData["AlertMsg"] = "存檔失敗";
                return RedirectToAction("ListEdit");
            }
        }
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(employee vobj)
        {
            #region 驗證判斷
            if (_db.employee.Any(i => i.empacc == vobj.empacc && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.empacc), "帳號已存在!");
            }
            if (vobj.comFid == 0)
            {
                ModelState.AddModelError(nameof(vobj.comFid), "請選擇公司!");
            }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            vobj.modid = Loginfo.User.Fid;
            vobj.moddate = DateTime.Now;

            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{vobj.emp_no}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{vobj.emp_no}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            employee? obj = _db.employee.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", false, $"#{fid}  [{obj.emp_no}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid}  [{obj.emp_no}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

        #region 同步醫院系統
        [HttpGet, ActionName("ListNew")]
        public async Task<IActionResult> ListNewGet()
        {
            return View(new employee());
        }

        [HttpPost, ActionName("ListSyncHsptlApi")]
        public async Task<JsonResult> ListSyncHsptlApiPost([FromBody] string emp_no)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(emp_no) || emp_no.Length > 10) { return Json(new ResponObj<string>("Err", "查詢條件錯誤")); }

            try
            {
                HsptlApiService hsptlServ = new HsptlApiService(Loginfo);
                ResponObj<employee?> fromapi = await hsptlServ.getEmpInfo(emp_no);
                if (fromapi.returnData != null)
                {
                    string msg = "";
                    employee? fromDB = _db.employee.Where(x => x.emp_no == emp_no).FirstOrDefault();
                    if (fromDB != null)
                    {
                        msg = "更新";
                        fromDB.name = fromapi.returnData.name;
                        fromDB.emp_dep_code = fromapi.returnData.emp_dep_code;
                        fromDB.emp_pos_code = fromapi.returnData.emp_pos_code;
                        fromDB.emp_pos_name = fromapi.returnData.emp_pos_name;
                        fromDB.emp_location = fromapi.returnData.emp_location;
                        fromDB.emp_cost_center = fromapi.returnData.emp_cost_center;
                        fromDB.emp_birth = fromapi.returnData.emp_birth;
                        fromDB.CardNo = fromapi.returnData.CardNo;
                        fromDB.enname = fromapi.returnData.enname;
                        fromDB.emp_birth = fromapi.returnData.emp_birth;
                        fromDB.mobile_tel = fromapi.returnData.mobile_tel;
                        fromDB.mobile_code = fromapi.returnData.mobile_code;
                        fromDB.email = fromapi.returnData.email;

                        _db.employee.Update(fromDB);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        msg = "新增";
                        fromapi.returnData.empacc = fromapi.returnData.emp_no ?? "";
                        fromapi.returnData.opensesame = SysBaseServ.GenerateSHA512String(fromapi.returnData.emp_birth ?? "ABCD1234");
                        fromapi.returnData.ifuse = true;
                        fromapi.returnData.comFid = Loginfo.User.comFid;
                        fromapi.returnData.dptFid = Loginfo.User.dptFid;
                        fromapi.returnData.SyncAsRole = true;
                        fromapi.returnData.pagesize = 20;

                        _db.employee.Add(fromapi.returnData);
                        await _db.SaveChangesAsync();
                    }
                    SysBaseServ.Log(Loginfo, "API", true, $"取得{emp_no}資料成功並{msg}");
                    return Json(new ResponObj<int>("0", fromapi.returnData.FID, "取得資料成功"));
                }
                else
                {
                    SysBaseServ.Log(Loginfo, "API", false, fromapi.message);
                    return Json(new ResponObj<string>("Err", fromapi.message));
                }

            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "API", ex);
                return Json(new ResponObj<string>("0", "查詢失敗"));
            }
        }
        #endregion

        #region 權限設定 (藥櫃+功能)
        [HttpGet, ActionName("ListUserCbntFnAuthEdit")]
        public async Task<IActionResult> ListUserCbntFnAuthEditGet(int fid)
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
                "where isnull(ml.MnTFid,'') <> '' and ml.LAYER = 1 and ml2.CatelogName <> '' " +
                "order by ml.MnTFid, ml.sorting, ml2.sorting ";
                Trees = conn.Query<MenuTree>(sql).ToList();

                //檢查 
                foreach (MenuTree t in Trees)
                {
                    if (t.L2Link.IndexOf(',') < 0) { t.L2Link += ",List"; }
                }
            }
            ViewBag.Cbnts = _db.Cabinet.Where(x => x.comFid == Loginfo.User.comFid).ToList();
            ViewBag.UserAuth = _db.UserCbntFnAuth.Where(x => x.EmpFid == fid).ToList();
            ViewBag.EmpInfo = _db.employee.Find(fid);

            return View(Trees);
        }
        [HttpPost, ActionName("ListUserCbntFnAuthEdit")]
        public async Task<JsonResult> ListUserCbntFnAuthEditPost(int fid, List<string> FnChk)
        {
            #region 驗證判斷
            if (fid == 0)
            {
                return Json(new ResponObj<string>("invalid", "錯誤"));
            }
            #endregion

            #region 存檔
            employee? emp = new employee();
            try
            {
                emp = _db.employee.Find(fid);
                _db.RemoveRange(_db.UserCbntFnAuth.Where(x => x.EmpFid == fid));
                await _db.SaveChangesAsync();

                foreach (string item in FnChk)
                {
                    string[] v = item.Split(',');
                    if (v.Length != 2) { continue; }
                    try { Convert.ToInt32(v[0]); } catch { continue; }
                    try { Convert.ToInt32(v[1]); } catch { continue; }
                    try
                    {
                        List<UserCbntFnAuth> addlist = new List<UserCbntFnAuth>();
                        addlist.Add(new UserCbntFnAuth
                        {
                            GID = new Guid(),
                            EmpFid = fid,
                            MnLFid = Convert.ToInt32(v[1]),
                            CbntFid = Convert.ToInt32(v[0]),
                            Active = true,
                        });
                        await _db.AddRangeAsync(addlist);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SysBaseServ.Log(Loginfo, "U", ex, $"{emp.emp_no} 功能id:{item}");
                    }
                }
                SysBaseServ.Log(Loginfo, "U", true, $"#{emp.FID} [{emp.emp_no}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "U", ex, $"#{emp.FID} [{emp.emp_no}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion

        }

        #endregion
    }
}

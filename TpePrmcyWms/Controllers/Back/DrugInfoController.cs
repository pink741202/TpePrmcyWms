using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.HsptlApiUnit;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TpePrmcyWms.Controllers.Back
{
    public class DrugInfoController : BaseController
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
            IQueryable<DrugInfo> obj = _db.DrugInfo.Where(x=> !x.DrugCode.EndsWith("_b"));

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.DrugCode ?? "").Contains(qKeyString)
                      || (s.DrugName ?? "").Contains(qKeyString)
                );
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                default: sortOrder = "DrugCode"; obj = obj.OrderBy(s => s.DrugCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<DrugInfo>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            DrugInfo? obj = _db.DrugInfo.Find(fid);
            if (obj != null && obj.ReplaceTo != null)
            {
                DrugInfo? replacer = _db.DrugInfo.Find(obj.ReplaceTo);
                if (replacer != null)
                {
                    obj.ReplaceToDrugName = $"{replacer.DrugCode}，{replacer.DrugName}，{replacer.FID}";
                }
            }
            return obj == null ? View(new DrugInfo()) : View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(DrugInfo vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (_db.DrugInfo.Any(i => i.DrugCode == vobj.DrugCode && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.DrugCode), "藥品代碼已存在!");
            }
            if (!string.IsNullOrEmpty(vobj.ReplaceToDrugName) && vobj.ReplaceToDrugName?.Split("，").Length != 3)
            {
                ModelState.AddModelError(nameof(vobj.ReplaceToDrugName), "異動時須替代成的藥品 格式錯誤!");
            }
            try
            {
                vobj.ReplaceTo = Convert.ToInt32(vobj.ReplaceToDrugName?.Split("，")[2]);
            }
            catch { ModelState.AddModelError(nameof(vobj.ReplaceToDrugName), "異動時須替代成的藥品 錯誤!"); }
            if(vobj.FID == vobj.ReplaceTo) { ModelState.AddModelError(nameof(vobj.ReplaceToDrugName), "異動時須替代成的藥品 不得與本藥品相同!"); }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value?.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors) );
            }
            #endregion
               
            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"key={vobj.FID} name={vobj.DrugCode}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"key={vobj.FID} name={vobj.DrugCode}");
                return Json(new ResponObj<string>("0", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            DrugInfo obj = _db.DrugInfo.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("0", "刪除失敗")); }
            
            try
            {
                if(_db.DrugGrid.Where(x=>x.DrugFid == fid).Count() > 0)
                {
                    return Json(new ResponObj<string>("Err", "使用中，不得刪除"));
                }
                if (_db.StockBill.Where(x => x.DrugCode == obj.DrugCode).Count() > 0)
                {
                    return Json(new ResponObj<string>("Err", "使用過，不得刪除"));
                }

                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{obj.DrugCode}]");
                return Json(new ResponObj<string>("0", "刪除成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{obj.DrugCode}]");
                return Json(new ResponObj<string>("0", "刪檔失敗"));
            }
        }
        #endregion

        #region 同步醫院系統
        [HttpPost, ActionName("ListSyncHsptlApi")]
        public async Task<JsonResult> ListSyncHsptlApiPost([FromBody] string querycode)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(querycode) || querycode.Length > 12) { return Json(new ResponObj<string>("Err", "查詢條件錯誤")); }
            string edittype = "";
            try
            {
                HsptlApiService hsptlServ = new HsptlApiService(Loginfo);
                ResponObj<DrugInfo?> fromapi = await hsptlServ.getDrugInfo(querycode);
                if (fromapi.returnData != null)
                {
                    DrugInfo? fromDB = _db.DrugInfo.Where(x => x.DrugCode == querycode).FirstOrDefault();
                    string msg = "";
                    if (fromDB != null)
                    {
                        edittype = "U"; msg = "更新";
                        fromapi.returnData.FID = fromDB.FID;
                        fromapi.returnData.isVax = fromDB.isVax;
                        fromapi.returnData.ChkDrugTakedLv = fromDB.ChkDrugTakedLv;
                        _db.DrugInfo.Update(fromapi.returnData);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        edittype = "C"; msg = "新增";
                        fromapi.returnData.isVax = fromapi.returnData.DrugName?.Contains("vaccine") ?? false;
                        fromapi.returnData.ChkDrugTakedLv = 1;
                        _db.DrugInfo.Add(fromapi.returnData);
                        await _db.SaveChangesAsync();
                    }
                    SysBaseServ.Log(Loginfo, "API", true, $"取得{querycode}資料成功並{msg}");
                    return Json(new ResponObj<int>("0", fromapi.returnData.FID, $"取得資料成功並{msg}"));
                }
                else
                {
                    SysBaseServ.Log(Loginfo, "API", false, fromapi.message);
                    return Json(new ResponObj<string>("Err", fromapi.message));
                }

            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "API", ex, querycode);
                return Json(new ResponObj<string>("0", "查詢失敗"));
            }
        }
        #endregion

        #region 限定藥品設定(贈藥/臨採)
        [HttpGet, ActionName("SubEditFreeTrial")]
        public async Task<IActionResult> SubEditFreeTrialGet(int fid)
        {
            if (fid == 0) { RedirectToAction("List"); }

            List<DrugLimitedTo> obj = await _db.DrugLimitedTo.Where(x => x.ActiveType == "FreeTrial" && x.DrugFid == fid).OrderBy(x => x.TargetPatient).ToListAsync();
            obj.Add(new DrugLimitedTo
            {
                DrugFid = fid,
                ActiveType = "FreeTrial",
                comFid = Loginfo.User.comFid ?? 0,
                modid = Loginfo.User.Fid,
            });

            ViewBag.drug = _db.DrugInfo.Find(fid);

            return View(obj);
        }

        [HttpPost, ActionName("SubEditFreeTrial")]
        public async Task<JsonResult> SubEditFreeTrialPost([FromBody] DrugLimitedToPostObj data)
        {
            #region 驗證判斷
            if (data.fid == 0)
            {
                return Json(new { code = "Err", returnData = $"藥品ID錯誤" });
            }
            List<DrugLimitedTo> vobj = data.list;
            foreach (DrugLimitedTo item in vobj)
            {
                if (item.FID == 0 && string.IsNullOrEmpty(item.TargetPatient)) { continue; }
                if (!string.IsNullOrEmpty(item.TargetPatient) && item.TargetPatient.Length != 8)
                {
                    return Json(new { code = "Err", returnData = $"病歷號{item.TargetPatient}格式錯誤" });
                }
                //確定病歷是否重複
                if (vobj.Where(x=>x.TargetPatient == item.TargetPatient && !string.IsNullOrEmpty(x.TargetPatient)).Count() > 1)
                {
                    return Json(new { code = "Err", returnData = $"病歷號{item.TargetPatient}重複" });
                }
                if (!string.IsNullOrEmpty(item.TargetPatient) && item.Qty == null) { item.Qty = 0; }
            }
            #endregion

            #region 存檔
            DrugInfo? drug = new DrugInfo();
            try
            {
                drug = _db.DrugInfo.Find(data.fid);
                for (int i = 0; i < vobj.Count; i++)
                {
                    if (!string.IsNullOrEmpty(vobj[i].TargetPatient) && vobj[i].Qty == null) { vobj[i].Qty = 0; }
                    vobj[i].modid = Loginfo.User.Fid;
                    vobj[i].moddate = DateTime.Now;
                    if (vobj[i].FID == 0 && !string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        vobj[i].ActiveType = "FreeTrial";
                        //item.GID = item.GID == Guid.Empty ? new Guid() : item.GID;
                        vobj[i].comFid = Loginfo.User.comFid ?? 0;
                        _db.DrugLimitedTo.Add(vobj[i]);
                        _db.SaveChanges();
                    }
                    if(vobj[i].FID != 0 && !string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        _db.DrugLimitedTo.Update(vobj[i]);
                        _db.SaveChanges();
                    }
                    if(vobj[i].FID != 0 && string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        _db.DrugLimitedTo.Remove(vobj[i]);
                        _db.SaveChanges();
                    }
                }
                SysBaseServ.Log(Loginfo, "U", true, drug?.DrugCode ?? "");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "U", ex, drug?.DrugCode ?? "");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion

        }

        [HttpGet, ActionName("SubEditAdHocProc")]
        public async Task<IActionResult> SubEditAdHocProcGet(int fid)
        {
            if (fid == 0) { RedirectToAction("List"); }

            List<DrugLimitedTo> obj = await _db.DrugLimitedTo.Where(x => x.ActiveType == "AdHocProc" && x.DrugFid == fid).OrderBy(x => x.TargetPatient).ToListAsync();
            if(obj.Where(x => x.TargetPatient == "Pool").Count() == 0)
            {
                obj.Add(new DrugLimitedTo
                {
                    DrugFid = fid,
                    ActiveType = "AdHocProc",
                    TargetPatient = "Pool",
                    Qty = 0,
                    comFid = Loginfo.User.comFid ?? 0,
                    modid = Loginfo.User.Fid,
                });
            }
            obj.Add(new DrugLimitedTo
            {
                DrugFid = fid,
                ActiveType = "AdHocProc",
                TargetPatient = "",
                comFid = Loginfo.User.comFid ?? 0,
                modid = Loginfo.User.Fid,
            });


            ViewBag.drug = _db.DrugInfo.Find(fid);

            return View(obj);
        }

        [HttpPost, ActionName("SubEditAdHocProc")]
        public async Task<JsonResult> SubEditAdHocProcPost([FromBody] DrugLimitedToPostObj data)
        {
            #region 驗證判斷
            if (data.fid == 0)
            {
                return Json(new { code = "Err", returnData = $"藥品ID錯誤" });
            }
            List<DrugLimitedTo> vobj = data.list;
            foreach (DrugLimitedTo item in vobj)
            {
                if (item.FID == 0 && string.IsNullOrEmpty(item.TargetPatient)) { continue; }
                if (!string.IsNullOrEmpty(item.TargetPatient) && item.TargetPatient.Length != 8 && item.TargetPatient != "Pool")
                {
                    return Json(new { code = "Err", returnData = $"病歷號{item.TargetPatient}格式錯誤" });
                }
                //確定病歷是否重複
                if (vobj.Where(x => x.TargetPatient == item.TargetPatient && !string.IsNullOrEmpty(x.TargetPatient)).Count() > 1)
                {
                    return Json(new { code = "Err", returnData = $"病歷號{item.TargetPatient}重複" });
                }
                if (!string.IsNullOrEmpty(item.TargetPatient) && item.Qty == null) { item.Qty = 0; }
            }
            #endregion

            #region 存檔
            DrugInfo? drug = new DrugInfo();
            try
            {
                drug = _db.DrugInfo.Find(data.fid);
                for (int i = 0; i < vobj.Count; i++)
                {
                    if (!string.IsNullOrEmpty(vobj[i].TargetPatient) && vobj[i].Qty == null) { vobj[i].Qty = 0; }
                    vobj[i].modid = Loginfo.User.Fid;
                    vobj[i].moddate = DateTime.Now;
                    if (vobj[i].FID == 0 && !string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        vobj[i].ActiveType = "AdHocProc";
                        //item.GID = item.GID == Guid.Empty ? new Guid() : item.GID;
                        vobj[i].comFid = Loginfo.User.comFid ?? 0;
                        _db.DrugLimitedTo.Add(vobj[i]);
                        _db.SaveChanges();
                    }
                    if (vobj[i].FID != 0 && !string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        _db.DrugLimitedTo.Update(vobj[i]);
                        _db.SaveChanges();
                    }
                    if (vobj[i].FID != 0 && string.IsNullOrEmpty(vobj[i].TargetPatient))
                    {
                        _db.DrugLimitedTo.Remove(vobj[i]);
                        _db.SaveChanges();
                    }
                }
                SysBaseServ.Log(Loginfo, "U", true, drug?.DrugCode ?? "");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "U", ex, drug?.DrugCode ?? "");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion

        }
        #endregion

    }
}

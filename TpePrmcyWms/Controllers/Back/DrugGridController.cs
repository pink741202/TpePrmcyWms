using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;


namespace TpePrmcyWms.Controllers.Back
{
    public class DrugGridController : BaseController
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
            , string qSafetyStock, int qCbnt)
        {
            IQueryable<DrugGrid> obj = from grid in _db.DrugGrid
                                       join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                       join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                       join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                       where cbnt.comFid == Loginfo.User.comFid && !drug.DrugCode.Contains("_")
                                       && Loginfo.CbntAuth.Contains(cbnt.FID)
                                       select new DrugGrid
                                       {
                                           FID = grid.FID,
                                           CbntFid = cbnt.FID,
                                           CbntName = cbnt.CbntName,
                                           DrawerNo = draw.No,
                                           DrugFid = drug.FID,
                                           DrugCode = drug.DrugCode,
                                           DrugName = drug.DrugName,
                                           Qty = grid.Qty,
                                           SafetyStock = grid.SafetyStock,
                                           BatchActive = grid.BatchActive,
                                           modid = grid.modid,
                                           moddate = grid.moddate,
                                       };

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                List<int> drugfids = _db.DrugInfo.Where(s => (s.DrugName ?? "").Contains(qKeyString)
                        || (s.DrugCode ?? "").Contains(qKeyString)).Select(s => s.FID).ToList();
                obj = obj.Where(x => drugfids.Contains(x.DrugFid));
            }
            ViewData["qSafetyStock"] = qSafetyStock;
            if (qSafetyStock == "Y")
            {
                obj = obj.Where(x=>x.Qty < x.SafetyStock);
            }
            ViewData["qCbnt"] = qCbnt;
            if (qCbnt != 0)
            {
                obj = obj.Where(x => x.CbntFid == qCbnt);
            }

            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "CbntName_desc": obj = obj.OrderByDescending(s => s.CbntName); break;
                case "CbntName": obj = obj.OrderBy(s => s.CbntName); break;
                case "DrawerNo_desc": obj = obj.OrderByDescending(s => s.DrawerNo); break;
                case "DrawerNo": obj = obj.OrderBy(s => s.DrawerNo); break;
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                case "DrugName_desc": obj = obj.OrderByDescending(s => s.DrugName); break;
                case "DrugName": obj = obj.OrderBy(s => s.DrugName); break;
                default: sortOrder = "CbntName"; obj = obj.OrderBy(s => s.CbntName); break;
            }
            ViewData["sortOrder"] = sortOrder;
            ViewBag.CbntAuth = Loginfo.CbntAuth.ConvertAll<string>(delegate (int i) { return i.ToString(); });
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<DrugGrid>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            DrugGrid? obj = _db.DrugGrid.Find(fid);
            if (obj == null)
            {
                obj = new DrugGrid();
            }
            else
            {
                DrugInfo? qdrug = _db.DrugInfo.Find(obj.DrugFid);
                if (qdrug != null)
                {
                    obj.DrugCode = qdrug.DrugCode;
                    obj.DrugName = $"{qdrug.DrugCode}，{qdrug.DrugName}，{qdrug.FID}";
                }
                obj.CbntName = _db.Cabinet.Find(obj.CbntFid)?.CbntName;
                obj.OffsetCbntFid = _db.DrugGrid.Where(x => x.FID == obj.OffsetTo).FirstOrDefault()?.CbntFid ?? null;
                obj.OffsetDrawFid = _db.DrugGrid.Where(x => x.FID == obj.OffsetTo).FirstOrDefault()?.DrawFid ?? null;
            }
            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(DrugGrid vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            if (vobj.CbntFid == 0) { ModelState.AddModelError(nameof(vobj.CbntFid), "請選擇藥櫃!"); }
            if (vobj.DrawFid == 0) { ModelState.AddModelError(nameof(vobj.DrawFid), "請選擇藥格!"); }
            try
            {
                vobj.DrugFid = Convert.ToInt32(vobj.DrugName?.Split("，")[2]);
            }
            catch { ModelState.AddModelError(nameof(vobj.DrugFid), "請選擇正確藥品名稱!"); }
            if (vobj.DrugFid == 0) { ModelState.AddModelError(nameof(vobj.DrugFid), "請選擇正確藥品名稱!"); }            
            if (_db.DrugGrid.Any(i => i.CbntFid == vobj.CbntFid && i.DrawFid == vobj.DrawFid && i.DrugFid == vobj.DrugFid && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.DrugFid), "此設定已存在!");
            }
            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            vobj.OffsetActive = vobj.OffsetCbntFid > 0 && vobj.OffsetDrawFid > 0;
            vobj.StockTakeType = vobj.StockTakeType == null ? "" : vobj.StockTakeType;
            vobj.moddate = DateTime.Now;
            vobj.modid = Loginfo.User.Fid;
            

            #region 存檔
            string edittype = vobj.FID == 0 ? "C" : "U";
            try
            {
                if (vobj.FID == 0) { _db.Add(vobj); }//create
                else { _db.Update(vobj); }//edit  //另一種寫法 TryUpdateModelAsync() 自動判斷要更新的屬性
                await _db.SaveChangesAsync();

                try
                {
                    #region 沖銷對應藥單放置櫃 OffsetTo
                    if (!(vobj.OffsetActive ?? false)) { vobj.OffsetTo = null; }
                    if (vobj.OffsetActive ?? false)
                    {
                        DrugInfo? Drug = _db.DrugInfo.Find(vobj.DrugFid);
                        if (Drug == null) { return Json(new ResponObj<string>("Err", "更新沖銷對應藥單放置櫃失敗")); }

                        #region 判斷沖銷藥名(_b) OffsetDrug
                        DrugInfo? OffsetDrug = _db.DrugInfo.Where(x => x.DrugCode == Drug.DrugCode + "_b").FirstOrDefault();
                        if (OffsetDrug == null) //沒有,新增
                        {
                            DrugInfo adddrug = new DrugInfo
                            {
                                DrugCode = Drug.DrugCode + "_b",
                                DrugName = Drug.DrugName?.Replace("(空瓶)", "").Replace("(藥單)", "") + ((vobj.ReturnEmptyBottle ?? false) ? "(空瓶)" : "(藥單)"),
                                isVax = false,
                            };
                            _db.DrugInfo.Add(adddrug);
                            _db.SaveChanges();
                            OffsetDrug = adddrug;
                        }
                        else //有,更新
                        {
                            OffsetDrug.DrugName = OffsetDrug.DrugName?.Replace("(空瓶)", "").Replace("(藥單)", "") + ((vobj.ReturnEmptyBottle ?? false) ? "(空瓶)" : "(藥單)");
                            _db.DrugInfo.Update(OffsetDrug);
                            _db.SaveChanges();
                        }
                        #endregion

                        #region 判斷沖銷藥格 OffsetGrid
                        DrugGrid? OffsetGrid = _db.DrugGrid.Where(x => x.DrugFid == OffsetDrug.FID).FirstOrDefault();
                        if (OffsetDrug == null)
                        {
                            DrugGrid newOffsetGrid = new DrugGrid
                            {
                                CbntFid = vobj.OffsetCbntFid ?? 0,
                                DrawFid = vobj.OffsetDrawFid ?? 0,
                                DrugFid = OffsetDrug?.FID ?? 0,
                                Qty = 0,
                                SafetyStock = 0,
                                StockTakeType = "1",
                                Alert = false,
                            };
                            _db.DrugGrid.Add(newOffsetGrid);
                            _db.SaveChanges();
                            OffsetGrid = newOffsetGrid;
                        }
                        else
                        {
                            OffsetGrid.CbntFid = vobj.OffsetCbntFid ?? 0;
                            OffsetGrid.DrawFid = vobj.OffsetDrawFid ?? 0;
                            _db.DrugGrid.Update(OffsetGrid);
                            _db.SaveChanges();
                        }
                        vobj.OffsetTo = OffsetGrid?.FID ?? 0;
                        _db.DrugGrid.Update(vobj);
                        _db.SaveChanges();
                        #endregion

                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    SysBaseServ.Log(Loginfo, edittype, ex, "更新沖銷對應藥單放置櫃錯誤");
                    return Json(new ResponObj<string>("ex", "更新沖銷對應藥單放置櫃錯誤"));
                }
                SysBaseServ.Log(Loginfo, edittype, true, $"key={vobj.FID} name={vobj.DrawerNo} + {vobj.DrugCode}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"key={vobj.FID} name={vobj.DrawerNo} + {vobj.DrugCode}");
                return Json(new ResponObj<string>("ex", "存檔錯誤"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            DrugGrid? obj = _db.DrugGrid.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            if (obj.Qty > 0)
            {
                return Json(new { code = "Err", message = "此藥在櫃中尚有庫存,不得刪除!" });
            }

            string drugcode = _db.DrugInfo.Find(obj.DrugFid)?.DrugCode ?? "";
            string drawerno = _db.Drawers.Find(obj.DrawFid)?.No.ToString() ?? "";            
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{drawerno} + {drugcode}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{drawerno} + {drugcode}]");
                return Json(new ResponObj<string>("ex", "刪檔錯誤"));
            }
        }
        #endregion

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using TpePrmcyWms.Models.Unit;
using Microsoft.Data.SqlClient;
using Dapper;
using NuGet.Packaging;
using System.Data;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Components.RenderTree;

namespace TpePrmcyWms.Controllers.Back
{
    public class CabinetController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        
        //ref: https://learn.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region CRUD
        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString, int qcomFid)
        {
            IQueryable<Cabinet> obj = _db.Cabinet.Where(x=>x.comFid == Loginfo.User.comFid);

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!String.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.CbntName ?? "").Contains(qKeyString)
                      || (s.CbntDesc ?? "").Contains(qKeyString)
                );
            }
            ViewData["qcomFid"] = qcomFid;
            if (qcomFid != 0)
            {
                obj = obj.Where(s => s.comFid == qcomFid);
            }
            #endregion

            #region 排序
            ViewData["sortOrder"] = sortOrder;
            switch (sortOrder)
            {
                case "CbntName_desc": obj = obj.OrderByDescending(s => s.CbntName); break;
                case "CbntName": obj = obj.OrderBy(s => s.CbntName); break;
                case "comFid_desc": obj = obj.OrderByDescending(s => s.comFid); break;
                case "comFid": obj = obj.OrderBy(s => s.comFid); break;
                default: obj = obj.OrderBy(s => s.CbntName); break;
            }
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<Cabinet>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            Cabinet obj = _db.Cabinet.Find(fid);
            if(obj == null)
            {
                obj = new Cabinet();
                obj.comFid = Loginfo.User.comFid;
                obj.dptFid = Loginfo.User.dptFid;
                obj.StoreGuid = new Guid();
            }
            else
            {
                obj.DrawerCount = _db.Drawers.Where(x => x.CbntFid == fid).Count();
                if (!string.IsNullOrEmpty(obj.StockTakeConfig_Time))
                {
                    obj.StockTakeConfig_TimeList.AddRange(obj.StockTakeConfig_Time.Replace("~", ",").Split(","));
                }
                
            }
            return View(obj);
        }

        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(Cabinet vobj)
        {
            #region 驗證判斷
            if (_db.Cabinet.Any(i => i.CbntName == vobj.CbntName && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.CbntName), "帳號已存在!");
            }
            if (vobj.comFid == 0 || vobj.comFid == null)
            {
                ModelState.AddModelError(nameof(vobj.comFid), "請選擇公司!");
            }
            if (!string.IsNullOrEmpty(vobj.StockTakeConfig_Day))
            {
                try
                {
                    List<int> ls = vobj.StockTakeConfig_Day.Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList();
                    foreach (int i in ls)
                    {
                        if (ls.Where(x => x == i).Count() > 1 || i > 6)
                        {
                            ModelState.AddModelError(nameof(vobj.StockTakeConfig_Day), "盤點設定-星期 出現重覆或不在星期範圍內!");
                            break;
                        }
                    }
                    vobj.StockTakeConfig_Day = string.Join(",", ls);
                }
                catch { ModelState.AddModelError(nameof(vobj.StockTakeConfig_Day), "盤點設定-星期 出現錯誤!"); }
            }
            if (!string.IsNullOrEmpty(vobj.StockTakeConfig_Time))
            {
                try
                {
                    List<int> ls = vobj.StockTakeConfig_Time.Replace(":", "").Replace("~", ",").Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList();
                    bool logicErr = false;
                    for (int i = 0; i < ls.Count; i += 2)
                    {
                        for (int j = 0; j < ls.Count; j += 2)
                        {
                            if (i == j) { continue; }
                            if (ls[j] <= ls[i] && ls[i] < ls[j + 1]) { logicErr = true; break; }
                            if (ls[j] < ls[i + 1] && ls[i + 1] <= ls[j + 1]) { logicErr = true; break; }
                        }
                        if (logicErr) { break; }
                    }
                    if (logicErr) { ModelState.AddModelError(nameof(vobj.StockTakeConfig_Time), "盤點設定-時段 出現重疊!"); }
                    else { vobj.StockTakeConfig_Time = string.Join(",", vobj.StockTakeConfig_Time.Split(",").Where(x => !string.IsNullOrEmpty(x))); }
                }
                catch (Exception ex) {
                    ModelState.AddModelError(nameof(vobj.StockTakeConfig_Time), "盤點設定-時段 出現錯誤!"); 
                }
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

                #region 變更藥格數量
                string alertmsg = "";
                List<Drawers> draws = _db.Drawers.Where(x => x.CbntFid == vobj.FID).ToList();
                if(draws.Count != vobj.DrawerCount)
                {
                    int orn = (vobj.DrawerCount - draws.Count) / Math.Abs(vobj.DrawerCount - draws.Count);
                    if (orn > 0)
                    {
                        for (int x = draws.Count; x < vobj.DrawerCount; x++)
                        {
                            Drawers add = new Drawers()
                            {
                                CbntFid = vobj.FID,
                                No = x + 1,
                                comFid = vobj.comFid,
                            };
                            _db.Add(add);
                            _db.SaveChanges();
                        }
                    }
                    if (orn < 0)
                    {
                        for (int x = draws.Count; x > vobj.DrawerCount; x--)
                        {
                            Drawers del = draws.Where(r => r.No == x).First();
                            if (_db.DrugGrid.Where(x => x.DrawFid == del.FID).Count() > 0)
                            {
                                alertmsg = "存檔成功，但部分藥格仍有設定藥品，無法減少藥格數量";
                                break;
                            }
                            _db.Remove(del);
                            _db.SaveChanges();
                        }

                    }
                }
                #endregion
                if (alertmsg != "") {
                    SysBaseServ.Log(Loginfo, edittype, true, $"{vobj.CbntName}-{alertmsg}");
                    return Json(new ResponObj<string>("0", alertmsg)); 
                }
                SysBaseServ.Log(Loginfo, edittype, true, $"{vobj.CbntName}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"{vobj.CbntName}" );
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            Cabinet? obj = _db.Cabinet.Find(fid);
            string edittype = "D";
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }

            if(_db.DrugGrid.Any(c => c.CbntFid == fid))
            {
                SysBaseServ.Log(Loginfo, edittype, false, $"#{fid} [{obj.CbntName}] 此櫃尚有設定藥品,不得刪除!");
                return Json(new ResponObj<string>("Err", "此櫃尚有設定藥品,不得刪除!"));
            }

            try
            {
                _db.RemoveRange(_db.MapMenuOnCbnt.Where(x => x.CbntFid == fid).ToList());
                _db.RemoveRange(_db.DrugGrid.Where(x => x.CbntFid == fid).ToList());
                _db.RemoveRange(_db.Drawers.Where(x => x.CbntFid == fid).ToList());
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, edittype, true, $"#{fid} {obj.CbntName}");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{fid} {obj.CbntName}");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

        #region 使用功能設定
        [HttpGet, ActionName("SubEditUsableMenu")]
        public async Task<IActionResult> SubEditUsableMenuGet(int fid)
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
                "where isnull(ml.MnTFid,'') <> '' and ml.LAYER = 1 and mt.System = 'FRONTEND' " +
                "order by ml.MnTFid, ml.fid, ml.sorting, ml2.sorting ";
                var datas = await conn.QueryAsync<MenuTree>(sql);
                Trees = datas.ToList();

                //檢查 
                foreach (MenuTree t in Trees)
                {
                    if (t.L2Link.IndexOf(',') < 0) { t.L2Link += ",List"; }
                }
            }
            ViewBag.MnAbleList = _db.MapMenuOnCbnt.Where(x => x.CbntFid == fid).ToList();
            ViewBag.CbntInfo = _db.Cabinet.Where(x => x.FID == fid).First();

            return View(Trees);
        }
        [HttpPost, ActionName("SubEditUsableMenu")]
        public async Task<JsonResult> SubEditUsableMenuPost(int fid, List<int> AllFid,
            List<int> Query, List<string> custTitle)
        {
            #region 驗證判斷
            if (fid == 0)
            {
                return Json(new { code = "invalid", message = "錯誤" });
            }
            #endregion

            #region 存檔
            Cabinet? cabinet = new Cabinet();
            string edittype = "U";
            try
            {
                cabinet = _db.Cabinet.Find(fid);
                
                foreach (int tree in AllFid)
                {
                    if (tree == 0) { continue; }
                    
                    try
                    {

                        MapMenuOnCbnt? rec = await _db.MapMenuOnCbnt.Where(x => x.CbntFid == fid && x.MnLFid == tree).FirstOrDefaultAsync();
                        if (rec != null)
                        {
                            //rec.GID = new Guid();
                            rec.CbntFid = fid;
                            rec.MnLFid = tree;
                            rec.Able = Query.Contains(tree);
                            rec.MnLTitle = custTitle[AllFid.IndexOf(tree)];
                        }
                        else
                        {
                            edittype = "C";
                            rec = new MapMenuOnCbnt()
                            {
                                GID = new Guid(),
                                CbntFid = fid,
                                MnLFid = tree,
                                Able = Query.Contains(tree),
                                MnLTitle = custTitle[AllFid.IndexOf(tree)],
                        };
                            _db.MapMenuOnCbnt.Add(rec);
                        };
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SysBaseServ.Log(Loginfo, edittype, ex, $"#{tree}");
                    }
                }
                SysBaseServ.Log(Loginfo, edittype, true, $"{cabinet?.CbntName}");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"{cabinet?.CbntName}");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion

        }

        #endregion

        
    }
}

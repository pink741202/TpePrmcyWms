using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;


namespace TpePrmcyWms.Controllers.Back
{
    public class SensorDeviceController : BaseController
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
            , string qSensorType)
        {
            IQueryable<SensorDevice> obj = _db.SensorDevice;

            #region 查詢
            ViewBag.qKeyString = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                string qKeyStringUpper = qKeyString.ToUpper();                
                obj = obj.Where(x => x.TargetTable.ToUpper() == qKeyStringUpper);
            }
            ViewBag.qSensorType = qSensorType;
            if (qSensorType != null)
            {
                string qSensorTypeUpper = qSensorType.ToUpper();
                obj = obj.Where(x => x.SensorType.ToUpper() == qSensorTypeUpper);
            }
            
            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "moddate_desc": obj = obj.OrderByDescending(s => s.moddate); break;
                case "moddate": obj = obj.OrderBy(s => s.moddate); break;
                default: sortOrder = "moddate_desc"; obj = obj.OrderByDescending(s => s.moddate); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            
            #endregion

            return View(await PaginatedList<SensorDevice>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));
            
        }

        [HttpGet, ActionName("ListEdit")]
        public async Task<IActionResult> ListEditGet(int fid)
        {
            SensorDevice? obj = _db.SensorDevice.Find(fid);
            if (obj == null)
            {
                obj = new SensorDevice();
            }
            else
            {
                //找櫃 找藥
                if(obj.TargetTable== "DrugGrid")
                {
                    var rec = _db.DrugGrid.Find(obj.TargetObjFid);
                    obj.CbntFid = rec.CbntFid;
                    obj.DrawFid = rec.DrawFid;
                    obj.DrGridFid = rec.FID;
                }
                else
                {
                    var rec = _db.Drawers.Find(obj.TargetObjFid);
                    obj.CbntFid = rec.CbntFid;
                    obj.DrawFid = obj.TargetObjFid;
                }
                //找包材
                if(obj.SensorType == "SCALE")
                {
                    MapPackOnSensor rec = await _db.MapPackOnSensor.Where(x => x.SensorFid == obj.FID).FirstOrDefaultAsync();
                    if (rec != null)
                    {
                        obj.PackageFid = rec.PackageFid;
                    }
                }
            }

            //組serialport下拉
            List<string> ports = SysBaseServ.JsonConf("ModbusSetting:Options").Split(',').ToList();
            List<SelectListItem> SerialPortList = new List<SelectListItem>();
            foreach(string item in ports)
            {
                SerialPortList.Add(new SelectListItem()
                {
                    Value = item,
                    Text = item
                });
            }
            ViewBag.SerialPortList = SerialPortList;
            return View(obj);
        }
        
        [HttpPost, ActionName("ListEdit")]
        public async Task<JsonResult> ListEditPost(SensorDevice vobj, IFormFile? logopic)
        {
            #region 驗證判斷
            //先設定 TargetTable + TargetObjFid
            if ((vobj.DrGridFid??0) != 0)
            {
                vobj.TargetTable = "DrugGrid";
                vobj.TargetObjFid = vobj.DrGridFid;
            }
            else
            {
                vobj.TargetTable = "Drawer";
                vobj.TargetObjFid = (int)vobj.DrawFid;
            } 
            
            if (vobj.CbntFid == 0) { ModelState.AddModelError(nameof(vobj.CbntFid), "請選擇藥櫃!"); }
            if (vobj.DrawFid == 0) { ModelState.AddModelError(nameof(vobj.CbntFid), "請選擇藥格!"); }
            
            if (_db.SensorDevice.Any(i => i.TargetTable == vobj.TargetTable && i.TargetObjFid == vobj.TargetObjFid
                    && i.SensorType == vobj.SensorType && i.SensorNo == vobj.SensorNo
                    && i.FID != vobj.FID))
            {
                ModelState.AddModelError(nameof(vobj.SensorType), "此配置已存在!");
                ModelState.AddModelError(nameof(vobj.SensorNo), "此配置已存在!");
            }

            if (!ModelState.IsValid)
            {
                List<ValidateReturnMsg> errors = ModelState.Where(w => w.Value.Errors.Count > 0).Select(e => new ValidateReturnMsg { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage ?? "" }).ToList();
                return Json(new ResponObj<List<ValidateReturnMsg>>("invalid", errors));
            }
            #endregion

            //設定公司
            int CbntFid = vobj.CbntFid ?? 0;
            if (vobj.TargetTable == "DrugGrid")
            {
                CbntFid = _db.DrugGrid.Find(vobj.TargetObjFid).CbntFid;
            }
            vobj.comFid = _db.Cabinet.Find(CbntFid).comFid;

            #region 處理重量換算對應表
            try
            {
                MapPackOnSensor? maps = await _db.MapPackOnSensor.Where(x => x.SensorFid == vobj.FID).FirstOrDefaultAsync();
                if (maps == null)
                {
                    maps = new MapPackOnSensor() { SensorFid = vobj.FID, PackageFid = vobj.PackageFid };
                    _db.Add(maps);
                }
                else
                {
                    maps.PackageFid = vobj.PackageFid;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new ResponObj<string>("ex", "處理重量換算對應表 存檔失敗"));
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
                SysBaseServ.Log(Loginfo, edittype, true, $"#{vobj.FID} [{vobj.SensorNo}-{vobj.SensorType}]");
                return Json(new ResponObj<string>("0", "存檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, edittype, ex, $"#{vobj.FID} [{vobj.SensorNo}-{vobj.SensorType}]");
                return Json(new ResponObj<string>("ex", "存檔失敗"));
            }
            #endregion
        }

        [HttpPost, ActionName("ListDelete")]
        public JsonResult ListDeletePost([FromBody] int fid)
        {
            SensorDevice? obj = _db.SensorDevice.Find(fid);
            if (!ModelState.IsValid || obj == null) { return Json(new ResponObj<string>("Err", "刪檔失敗")); }
            
            string storage = obj.TargetTable == "DrugGrid" ? "藥格" : "藥櫃";
            
            try
            {
                _db.Remove(obj);
                _db.SaveChangesAsync();
                SysBaseServ.Log(Loginfo, "D", true, $"#{fid} [{obj.SensorNo}-{obj.SensorType}]");
                return Json(new ResponObj<string>("0", "刪檔成功"));
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, "D", ex, $"#{fid} [{obj.SensorNo}-{obj.SensorType}]");
                return Json(new ResponObj<string>("ex", "刪檔失敗"));
            }
        }
        #endregion

    }
}

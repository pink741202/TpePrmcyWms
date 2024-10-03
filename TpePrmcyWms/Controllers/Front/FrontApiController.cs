using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Front;
using TpePrmcyWms.Filters;
using System.Collections.Generic;
using TpePrmcyWms.Models.Unit.Back;
using System.IdentityModel.Tokens.Jwt;
using Humanizer;
using System.Security.AccessControl;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TpePrmcyWms.Models.Unit;

namespace TpePrmcyWms.Controllers.Front
{
    [FrontendSessionFilter]
    public class FrontApiController : NoAuthController
    {
        DBcPharmacy _db = new DBcPharmacy();

        #region kiosk互動 感應器資料
        [HttpPost]
        public JsonResult AddQu([FromBody] SensorComuQuee data)
        {
            if(string.IsNullOrEmpty(data.DrugCode) || string.IsNullOrEmpty(data.OperType))
            {
                return Json(new { code = 1, ReturnData = "invalid" });
            }
            SensorComuQuee add = new SensorComuQuee();
            try
            {
                add = new SensorComuQuee()
                {
                    DrugCode = data.DrugCode,
                    OperType = data.OperType,
                    CbntFid = AtCbntFid,
                    modid = Loginfo.User.Fid,
                };
                //無kiosk
                if (Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid")) != 0)
                {
                    switch (_db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.oprState != "H").Count())
                    {
                        case 0:
                            add.LEDColor = "Red";
                            break;
                        case 1:
                            add.LEDColor = "Orange";
                            break;
                        case 2:
                            add.LEDColor = "Green";
                            break;
                        case 3:
                            add.LEDColor = "Blue";
                            break;
                    }
                    
                }

                _db.SensorComuQuee.Add(add);
                _db.SaveChanges();
            }
            catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); return Json(new { code = 1, ReturnData = "error" }); }
            return Json(new { code = 0, ReturnData = add });
        }
        [HttpPost]
        public JsonResult GetQu()
        {            
            List<SensorComuQuee> list = new List<SensorComuQuee>();
            try
            {
                list = _db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.oprState != "H").ToList();
                //無kiosk
                if (Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid")) != 0)
                {
                    foreach (SensorComuQuee s in list)
                    {
                        if (s.oprState == "0") { s.ssrState = "1"; }
                        if (s.oprState == "1") { s.ssrState = "2"; }
                    }
                    _db.SensorComuQuee.UpdateRange(list);
                    _db.SaveChanges(true);
                }
            }
            catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); return Json(new { code = 1, ReturnData = "error" }); }
            return Json(new { code = 0, ReturnData = list });
        }
        [HttpPost]
        public JsonResult SetState([FromBody] SensorComuQuee data)
        {
            if (data == null) { return Json(new { code = "Err", ReturnData = "data null" }); }
            if (string.IsNullOrEmpty(data.DrugCode)) { return Json(new { code = "Err", ReturnData = "drugcode null" }); }
            if (data.oprState == "D")
            {
                try
                {
                    SensorComuQuee up = _db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.DrugCode == data.DrugCode && x.oprState != "H").First();
                    if(up.ssrState != "1") { _db.SensorComuQuee.Remove(up); _db.SaveChanges(); } //非開門狀態
                }
                catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); return Json(new { code = 1, ReturnData = "error" }); }
                return Json(new { code = 0, ReturnData = "" });
            }
            try
            {
                SensorComuQuee up = _db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.DrugCode == data.DrugCode && x.oprState != "H").First();
                up.oprState = data.oprState == "" ? up.oprState : data.oprState;
                up.DrawFid = data.DrawFid == 0 ? up.DrawFid : data.DrawFid;
                up.stockBillFid = data.stockBillFid == 0 ? up.stockBillFid : data.stockBillFid;
                _db.SaveChanges(true);
            }
            catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); return Json(new { code = 1, ReturnData = "error" }); }

            //測試環境,自動刪佇列
            if (data.oprState == "2")
            {
                try
                {
                    
                    if (Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid")) != 0) //沒kiosk
                    {
                        _db.SensorComuQuee.RemoveRange(_db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.DrawFid == data.DrawFid && x.DrugCode == data.DrugCode && x.oprState == "2").ToList());
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); }
            }

            return Json(new { code = 0, ReturnData = "" });
        }
        #endregion

        #region 庫存異動單 存檔
        [HttpPost]
        public JsonResult StockBillSave([FromBody] StockBill_Prscpt data)
        {
            try
            {
                FrontendService frserv = new FrontendService(data.CbntFid ?? 0, Loginfo);
                data.DrugGridFid = frserv.getDrugGridFid(data.DrawFid ?? 0, data.DrugCode);
                //StockBill rec = frserv.SetStockBill(data);
                data.moddate = DateTime.Now;
                data.JobDone = data.Qty > 0;
                if (data.FID == 0) {
                    data.addid = Loginfo.User.Fid;
                    data.adddate = DateTime.Now;
                    _db.StockBill.Add(data); 
                    _db.SaveChanges(); 
                }
                else { _db.Update(data); _db.SaveChanges(); }

                //更新藥單
                if (data.TargetQty == data.Qty && data.PrscptFid != null && data.PrscptFid.Count > 0)
                {
                    PrscptBill? rec = _db.PrscptBill.Find(data.PrscptFid[0]);
                    if (rec != null)
                    {
                        rec.modid = Loginfo.User.Fid;
                        rec.moddate = DateTime.Now;
                        rec.DoneFill = true;
                        _db.SaveChanges();
                    }
                    
                }
                //空瓶放入分次,自動新增下一筆
                if(data.BillType == "BRT" && data.TargetQty != data.Qty)
                {
                    StockBill add = JsonConvert.DeserializeObject<StockBill>(JsonConvert.SerializeObject(data));
                    add.FID = 0;
                    add.JobDone = false;
                    add.TargetQty = data.TargetQty - data.Qty;
                    add.Qty = 0;
                    add.SysChkQty = 0;
                    add.UserChk1Qty = null;
                    add.UserChk2Qty = null;
                    add.addid = Loginfo.User.Fid;
                    add.adddate = DateTime.Now;
                    _db.StockBill.Add(add);
                    _db.SaveChanges(true);
                }
                //疫苗出入庫回寫至疫苗明細
                if(data.FromFid > 0 && (data.BillType == "VXO" || data.BillType == "VXR"))
                {
                    decimal? ttl = _db.StockBill
                        .Where(x => x.BillType == data.BillType && x.FromFid == data.FromFid)
                        .Select(x => x.Qty).Sum();
                    if(data.BillType == "VXO")
                    {
                        _db.VaxSkdDtl.Find(data.FromFid ?? 0).QtyStockOut = ttl ?? 0;
                        _db.SaveChanges();
                    }
                    if(data.BillType == "VXR")
                    {
                        _db.VaxSkdDtl.Find(data.FromFid ?? 0).QtyStockIn = ttl ?? 0;
                        _db.SaveChanges();
                    }
                }
                //Offset更新對應表及庫存
                if (data.BillType == "OSA")
                {
                    frserv.SaveOffsetBill(data);
                }
                else { frserv.SavePrscptMapping(data); }
                frserv.AfterStocking(data);

            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = data.FID });
        }
        [HttpPost]
        public JsonResult BatchFillSave([FromBody] QryBatchDrawers data)
        {
            try
            {
                int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
                DateTime dt = DateTime.Now;
                FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
                data.stockBill.DrugGridFid = frserv.getDrugGridFid(data.stockBill.DrawFid ?? 0, data.DrugCode);

                //cc的單位轉換 異動單的數量應是瓶數,所以不用轉成cc...吧?! 8/10
                DrugGrid? useGrid = _db.DrugGrid.Find(data.stockBill.DrugGridFid);
                //decimal? unitconvert = useGrid?.UnitConvert ?? null;
                //if (unitconvert != null) { data.stockBill.TargetQty = data.stockBill.TargetQty * unitconvert ?? 1; data.stockBill.Qty = data.stockBill.TargetQty; }

                //庫存異動單及藥單存單           
                data.stockBill.moddate = dt;
                data.stockBill.JobDone = true;

                if (data.stockBill.FID == 0) { 
                    data.stockBill.adddate = dt;
                    data.stockBill.addid = Loginfo.User.Fid;
                    _db.StockBill.Add(data.stockBill); 
                    _db.SaveChanges(); }
                else { _db.StockBill.Update(data.stockBill); _db.SaveChanges(); }
                for (int i = 0; i < data.bills.Count; i++)
                {
                    if (!string.IsNullOrEmpty(data.bills[i].msg)) { continue; }
                    data.bills[i].DoneFill = true;
                    data.bills[i].modid = Loginfo.User.Fid;
                    data.bills[i].moddate = dt;
                    _db.PrscptBill.Update(data.bills[i]);
                    _db.SaveChanges();                    
                }

                //更新庫存
                frserv.SavePrscptMapping(data.stockBill);
                frserv.AfterStocking(data.stockBill);

                #region 產生待還空瓶異動紀錄
                if ((useGrid?.OffsetActive ?? false) && useGrid.OffsetTo > 0 && (useGrid?.ReturnEmptyBottle ?? false))
                {
                    DrugGrid? drug_b = _db.DrugGrid.Find(useGrid.OffsetTo);                    
                    if(drug_b != null)
                    {
                        StockBill add = new StockBill()
                        {
                            CbntFid = drug_b.CbntFid,
                            DrawFid = drug_b.DrawFid,
                            DrugFid = drug_b.DrugFid,
                            BillType = "BRT",
                            TradeType = true,
                            FromFid = data.stockBill.FID,
                            comFid = Loginfo.User.comFid,
                            modid = Loginfo.User.Fid,
                            JobDone = false,
                            DrugCode = data.DrugCode + "_b",
                            TargetQty = data.stockBill.TargetQty,
                            adddate = DateTime.Now,
                            addid = Loginfo.User.Fid,
                        };
                        _db.StockBill.Add(add);
                        _db.SaveChanges(true);
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = data.stockBill.FID });
        }
        [HttpPost]
        public JsonResult TransGotoSave([FromBody] QryDrawersForTrans data)
        {
            int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            ResponObj<string> result = frserv.SaveTransApply(data);
            if(result.code == "0")
            {
                return Json(new { code = 0, returnData = result.returnData ?? "" });
            }
            else
            {
                return Json(new { code = "Err", returnData = result.returnData ?? "" });
            }
        }
        [HttpPost]
        public JsonResult TransIntoSave([FromBody] QryDrawersForTrans data)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            ResponObj<string> result = frserv.SaveTransApply(data);
            if (result.code == "0")
            {
                return Json(new { code = 0, returnData = result.returnData ?? "" });
            }
            else
            {
                return Json(new { code = "Err", returnData = result.returnData ?? "" });
            }
        }
        [HttpPost]
        public JsonResult TransGoListSave([FromBody] StockBill_Prscpt data)
        {
            int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            ResponObj<string> result = frserv.SaveTransUpdate(data);
            if (result.code == "0")
            {
                return Json(new { code = 0, returnData = result.returnData ?? "" });
            }
            else
            {
                return Json(new { code = "Err", returnData = result.returnData ?? "" });
            }
        }
        [HttpPost]
        public JsonResult TransInListSave([FromBody] StockBill_Prscpt data)
        {
            int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            ResponObj<string> result = frserv.SaveTransUpdate(data);
            if (result.code == "0")
            {
                return Json(new { code = 0, returnData = result.returnData ?? "" });
            }
            else
            {
                return Json(new { code = "Err", returnData = result.returnData ?? "" });
            }
        }
        [HttpPost]
        public JsonResult StockBillOffsetSave([FromBody] StockBill_Prscpt data)
        {
            try
            {
                FrontendService frserv = new FrontendService(data.CbntFid ?? 0, Loginfo);
                if (string.IsNullOrEmpty(data.Scantext))
                {
                    data.DrugGridFid = frserv.getDrugGridFid(data.DrawFid ?? 0, data.DrugCode);
                    //StockBill rec = frserv.SetStockBill(data);
                    data.moddate = DateTime.Now;
                    data.JobDone = data.Qty > 0;

                    //借藥跟還空瓶 轉成一筆筆建立
                    if (!data.TradeType || (data.TradeType && data.DrugCode.Contains("_b")))
                    {
                        for (int i = 0; i < data.TargetQty; i++)
                        {
                            StockBill_Prscpt add = JsonConvert.DeserializeObject<StockBill_Prscpt>(JsonConvert.SerializeObject(data));
                            add.addid = Loginfo.User.Fid;
                            add.adddate = DateTime.Now;
                            add.Qty = 1;
                            _db.StockBill.Add(add);
                            _db.SaveChanges();
                            frserv.SaveOffsetBill(add);
                            if (i == data.TargetQty - 1) { data.FID = add.FID; } //後面寄mail要用
                        }
                    }
                    //退藥,直接存,然後自動沖借藥量
                    if (data.TradeType && !data.DrugCode.Contains("_b"))
                    {
                        data.addid = Loginfo.User.Fid;
                        data.adddate = DateTime.Now;
                        _db.StockBill.Add(data);
                        _db.SaveChanges();
                        frserv.SaveOffsetBill(data);

                        QryOffsetDrawers offset = new QryOffsetDrawers($"{data.DrugCode}+{data.ToFloor}+{data.CbntFid}");
                        offset.stockBills.Add(data);
                        List<StockBill> recs = (from map in _db.MapPrscptOnBill
                                                join stock in _db.StockBill on map.StockbillFid equals stock.FID
                                                where map.OffsetQryKey == offset.OffsetQryKey
                                                && map.OffsetGroup == "" && map.PrscptFid == 0
                                                orderby map.moddate
                                                select stock).Take(Convert.ToInt16(data.Qty)).ToList();
                        offset.stockBills.AddRange(recs);
                        frserv.SaveOffsetGroupup(offset);
                    }
                    frserv.AfterStocking(data);

                }
                else //藥單
                {
                    PrscptBill rec = frserv.GetPrscptBill(data.Scantext.Split(";"));
                    if (rec != null && rec.DoneFill) { return Json(new { code = "Err", returnData = "藥單已被使用過" }); }
                    frserv.SaveOffsetBill(data);  
                }                                
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = data.FID });
        }
        [HttpPost]
        public JsonResult OffsetGroupSave([FromBody] QryOffsetDrawers data)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            //確定已平衡可沖銷
            data = frserv.GetOffsetQtys(data);
            decimal prscptQ = (data.UnitConvert != null && data.Qty_Prscpt > 0) ? Math.Round(data.Qty_Prscpt / data.UnitConvert ?? 1, 1) : data.Qty_Prscpt;
            decimal usedQ = data.Qty_Apply - data.Qty_ReturnDrug;
            bool isBalance = prscptQ == usedQ && (data.Qty_ReturnEmpty == -1 || usedQ == data.Qty_ReturnEmpty) && usedQ > 0;
            if (!isBalance) { return Json(new { code = "Err", returnData = "沖銷數量錯誤！" }); }

            bool SaveOffset = frserv.SaveOffsetGroupup(data);

            //不用還空瓶,就累紀藥單數量,為了計算常備量
            if (SaveOffset && data.Qty_ReturnEmpty == -1) { ResponObj<string> resp = frserv.AddBillQtyToGrid(data.stockBills); }
            if (SaveOffset) { return Json(new { code = "0", returnData = "" }); }
            else { return Json(new { code = "Err", returnData = "錯誤" }); }

        }
        [HttpPost]
        public JsonResult DeletePrscptInOffset([FromBody] StockBill_Prscpt data)
        {
            try
            {
                if(data.PrscptFid == null || data.PrscptFid.Count < 1 || data.PrscptFid[0] == 0)
                {
                    return Json(new { code = "Err", returnData = "資料錯誤" });
                }
                FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
                string OffsetQryKey = $"{data.DrugCode}+{data.ToFloor}+{data.CbntFid}";
                PrscptBill? prscpt = _db.PrscptBill.Find(data.PrscptFid[0]);
                if (prscpt != null)
                {
                    prscpt.DoneFill = false;
                    prscpt.modid = Loginfo.User.Fid;
                    prscpt.moddate = DateTime.Now;
                    _db.PrscptBill.Update(prscpt);
                    _db.SaveChanges();
                }
                
                MapPrscptOnBill map = _db.MapPrscptOnBill.Where(x => x.PrscptFid == data.PrscptFid[0]
                    && x.OffsetQryKey == OffsetQryKey
                    && x.OffsetGroup == ""
                    && x.StockbillFid == 0).First();
                _db.Remove(map);
                _db.SaveChanges(true);
            }
            catch(Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "錯誤" });
            }
            return Json(new { code = 0, returnData = "" });

        }
        [HttpPost]
        public JsonResult StockTakingSave([FromBody] StockBill_Prscpt data)
        {
            try
            {
                FrontendService frserv = new FrontendService(data.CbntFid ?? 0, Loginfo);
                data.DrugGridFid = frserv.getDrugGridFid(data.DrawFid ?? 0, data.DrugCode);
                data.addid = Loginfo.User.Fid;
                data.adddate = DateTime.Now;
                data.moddate = DateTime.Now;
                data.JobDone = true;

                _db.StockBill.Add(data);
                _db.SaveChanges();
                
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = data.FID });
        }
        [HttpPost]
        public JsonResult BottleExOutSave([FromBody] StockBill_Prscpt data)
        {
            int AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            DateTime dt = DateTime.Now;
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            int returnFid = 0;
            try
            {
                decimal qty = data.Qty;
                data.DrugGridFid = frserv.getDrugGridFid(data.DrawFid ?? 0, data.DrugCode);
                data.addid = Loginfo.User.Fid;
                data.adddate = DateTime.Now;
                data.moddate = dt;
                data.JobDone = true;
                //查原藥藥格,不須還空瓶的庫存異動,要把數量改為0
                if (!(_db.DrugGrid.Where(x=>x.OffsetTo == data.DrugGridFid).First().ReturnEmptyBottle ?? false))
                {
                    data.Qty = 0; 
                    data.TargetQty = 0; 
                }
                _db.StockBill.Add(data);
                _db.SaveChanges();

                //更新庫存 不管是拿空瓶還是只拿單,都要扣藥格數量(為了算常備量)
                data.Qty = qty;
                data.TargetQty = qty;
                frserv.AfterStocking(data);

                returnFid = data.FID;

                DrugGrid grid = _db.DrugGrid.Where(x => x.OffsetTo == data.DrugGridFid).First();
                data.FID = 0;
                data.FromFid = returnFid;
                data.CbntFid = grid.CbntFid;
                data.DrawFid = grid.DrawFid;
                data.BillType = "BXI";
                data.DrugCode = data.DrugCode.Replace("_b", "");
                data.TradeType = true;
                data.DrugGridFid = grid.FID;
                data.TargetQty = qty;
                data.Qty = 0;
                data.addid = Loginfo.User.Fid;
                data.adddate = DateTime.Now;
                data.moddate = null;
                data.JobDone = false;
                _db.StockBill.Add(data);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = returnFid });
        }
        [HttpPost]
        public JsonResult StockBillMsdSave([FromBody] StockBill_MSD data)
        {
            try
            {
                FrontendService frserv = new FrontendService(data.CbntFid ?? 0, Loginfo);
                data.DrugGridFid = data.DrugGridFid == 0 ? frserv.getDrugGridFid(data.DrawFid ?? 0, data.DrugCode) : data.DrugGridFid;
                data.moddate = DateTime.Now;
                data.JobDone = data.Qty > 0;
                if (data.FID == 0)
                {
                    data.addid = Loginfo.User.Fid;
                    data.adddate = DateTime.Now;
                    _db.StockBill.Add(data);
                    _db.SaveChanges();
                }
                else { _db.Update(data); _db.SaveChanges(); }
                
                //存美沙冬表單
                MethadonBill? bill = _db.MethadonBill.Where(x=>x.RecordDate == DateTime.Now.Date).FirstOrDefault();
                if (bill == null)
                {
                    bill = new MethadonBill();
                }
                if (data.BillType == "MSDT") //領
                {
                    bill.TakeTime = DateTime.Now;
                    bill.TakeEmpFid = Loginfo.User.Fid;
                    bill.TakeSuperFid = data.superFid;
                    bill.TakeWeight = data.This_Weight;
                    bill.TakeCC = data.This_CC;
                    bill.adddate = data.adddate ?? DateTime.Now;
                }
                if (data.BillType == "MSDR") //還
                {
                    bill.RetnTime = DateTime.Now;
                    bill.RetnEmpFid = Loginfo.User.Fid;
                    bill.RetnSuperFid = data.superFid;
                    bill.RetnWeight = data.This_Weight;
                    bill.RetnCC = data.This_CC;
                    bill.moddate = data.adddate ?? DateTime.Now;

                    bill.UsedPatientCnt = data.UsedPatientCnt ?? 0;
                    bill.UsedCC = data.UsedCC;
                    bill.SysRemainCC = data.SysChkQty;
                    bill.TakeRemainCC = data.UserChk1Qty;
                    bill.StockTakeBalance = data.StockTakeBalance;
                }
                if(bill.FID == 0) { _db.MethadonBill.Add(bill); }
                else { _db.MethadonBill.Update(bill); }
                _db.SaveChanges();

                //Offset更新對應表及庫存
                frserv.AfterStocking(data);

            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "存檔錯誤！" });
            }

            return Json(new { code = 0, returnData = "存檔完成！" });
        }
        #endregion

        #region 替代藥碼
        [HttpPost]
        public JsonResult FindReplaceDrugCode([FromBody] string code)
        {
            string result = "";
            try
            {
                int replacefid = _db.DrugInfo.Where(x => x.DrugCode == code).FirstOrDefault()?.ReplaceTo ?? 0;
                if (replacefid != 0)
                {
                    result = _db.DrugInfo.Find(replacefid)?.DrugCode ?? "";
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = "Err", ReturnData = "查詢替代藥碼 出現錯誤！" });
            }
            
            return Json(new { code = 0, ReturnData = result });
        }
        #endregion

        #region 施打疫苗 - 存檔異動資料庫   
        [HttpPost, ActionName("VaxSkdSave")]
        public JsonResult VaxSkdSave(VaxSkd vobj)
        {
            #region 驗證判斷 
            if (vobj.VaxDate < DateTime.Now.Date)
            {
                ModelState.AddModelError(nameof(vobj.VaxDate), "須大於今日!");
            }
            vobj.CaseClose = vobj.CaseClose??false;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(w => w.Value?.Errors.Count > 0).Select(e => new { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage });
                return Json(new { code = "invalid", returnData = errors });
            }
            #endregion

            try
            {
                vobj.comFid = Loginfo.User.comFid;
                vobj.dptFid = Loginfo.User.dptFid;
                vobj.modid = Loginfo.User.Fid;
                vobj.moddate = DateTime.Now;
                if (vobj.FID == 0)
                {
                    _db.VaxSkd.Add(vobj);
                }
                else
                {
                    _db.VaxSkd.Update(vobj);
                }
                _db.SaveChanges();
                return Json(new { code = "0", returnData = "" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "資料異動錯誤！" });
            }
        }       
        [HttpPost, ActionName("VaxSkdDtlSave")]
        public JsonResult VaxSkdDtlSave(VaxSkdDtl vobj)
        {
            vobj.DrugCode = vobj.DrugName?.Split("，")[0];
            vobj.DrugName = vobj.DrugName?.Split("，")[1];
            try
            {
                vobj.DrugFid = _db.DrugInfo.Where(x => x.DrugCode == vobj.DrugCode).FirstOrDefault()?.FID ?? 0;
            }
            catch { }
            #region 驗證判斷 
            if (vobj.DrugFid == 0)
            {
                ModelState.AddModelError(nameof(vobj.DrugFid), "查無藥品或藥品格式錯誤！");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(w => w.Value?.Errors.Count > 0).Select(e => new { Name = e.Key, ErrorMsg = e.Value?.Errors[0].ErrorMessage });
                return Json(new { code = "invalid", returnData = errors });
            }
            #endregion

            try
            {
                vobj.modid = Loginfo.User.Fid;
                vobj.moddate = DateTime.Now;
                if (vobj.FID == 0)
                {
                    _db.VaxSkdDtl.Add(vobj);
                }
                else
                {
                    _db.VaxSkdDtl.Update(vobj);
                }
                _db.SaveChanges();
                return Json(new { code = "0", returnData = "" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "資料異動錯誤！" });
            }
        }

        [HttpPost, ActionName("VaxSkdDelete")]
        public JsonResult VaxSkdDelete([FromBody] int fid)
        {
            #region 驗證判斷 
            if (fid == 0)
            {
                return Json(new { code = "invalid", returnData = "未輸入關鍵值！" });
            }
            #endregion

            try
            {
                VaxSkd? del = _db.VaxSkd.Find(fid) ?? null;
                if (del == null)
                {
                    return Json(new { code = "Err", returnData = "資料不存在！" });
                }
                else
                {
                    List<VaxSkdDtl> dtls = _db.VaxSkdDtl.Where(x => x.VaxSkdFid == fid).ToList();
                    _db.VaxSkdDtl.RemoveRange(dtls);
                    _db.VaxSkd.Remove(del);
                    _db.SaveChanges(true);
                }
                return Json(new { code = "0", returnData = "" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "資料異動錯誤！" });
            }
        }
        [HttpPost, ActionName("VaxSkdDtlDelete")]
        public JsonResult VaxSkdDtlDelete([FromBody] int fid)
        {
            #region 驗證判斷 
            if (fid == 0)
            {
                return Json(new { code = "invalid", returnData = "未輸入關鍵值！" });
            }
            #endregion

            try
            {
                VaxSkdDtl? del = _db.VaxSkdDtl.Find(fid) ?? null;
                if (del == null)
                {
                    return Json(new { code = "Err", returnData = "資料不存在！" });
                }
                else
                {
                    _db.VaxSkdDtl.Remove(del);
                    _db.SaveChanges();
                }
                return Json(new { code = "0", returnData = "" });
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "資料異動錯誤！" });
            }
        }
        #endregion

    }
}

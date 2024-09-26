using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Linq;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.HsptlApiUnit;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;
using TpePrmcyWms.Models.Unit.Front;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TpePrmcyWms.Models.Service
{
    public class FrontendService
    {
        private readonly ILogger<FrontendService>? _logger;
        private LoginInfo _loginfo;
        DBcPharmacy _db = new DBcPharmacy();
        int AtCbntFid = 0;
        string Comid = "";
        public FrontendService(int atcbntfid, LoginInfo loginfo)
        {
            AtCbntFid = atcbntfid;
            _loginfo = loginfo;
            Comid = (from com in _db.Company
                     join cbt in _db.Cabinet on com.FID equals cbt.comFid
                     where cbt.FID == AtCbntFid
                     select com.comid)
                    .First();
        }

        #region 紀錄藥品引用次數
        public void DrugPickAdd(string DrugCode) { 
            using(DBcPharmacy db = new DBcPharmacy())
            {
                DrugInfo info = db.DrugInfo.Where(x=>x.DrugCode == DrugCode).FirstOrDefault();
                DrugPickLog rec = db.DrugPickLog.Where(x=>x.DrugFid == info.FID).FirstOrDefault();
                if (rec != null)
                {
                    rec.PickTimes = rec.PickTimes + 1;
                }
                else
                {
                    DrugPickLog add = new DrugPickLog() {
                        DrugFid = info.FID,
                        PickTimes = 1,
                        comFid = 1
                    };
                    db.Add(add);
                }
                db.SaveChanges();
            }
        }
        #endregion

        #region 取得藥品id
        public int getDrugFid(string DrugCode)
        {
            int result = -1;
            DrugInfo rec = _db.DrugInfo.Where(x => x.DrugCode == DrugCode).FirstOrDefault();
            if (rec != null) { result = rec.FID; }
            return result;
        }
        #endregion

        #region 取得藥格藥品id
        public int getDrugGridFid(int drawerfid, string DrugCode)
        {
            int result = (from dg in _db.DrugGrid
                           join di in _db.DrugInfo on dg.DrugFid equals di.FID
                           where di.DrugCode.Equals(DrugCode) && dg.DrawFid.Equals(drawerfid) && dg.CbntFid.Equals(AtCbntFid)
                           select dg.FID).FirstOrDefault();
            return result;
        }
        #endregion

        #region PrscptBill 藥單掃瞄結果存入資料庫,並確定狀態
        public PrscptBillInfo SetPrscptBill(string[] data)
        {
            PrscptBillInfo result = new PrscptBillInfo();
            if (data.Length != 17) { result.FID = -1; return result; }
            #region 藥單
            try
            {                
                result = new PrscptBillInfo(data);
                result.Pharmarcy = string.IsNullOrEmpty(result.Pharmarcy) ? Comid : result.Pharmarcy; //手輸就當是跟櫃子同院
                int chkDrugTakedLv = _db.DrugInfo.Where(dr => dr.DrugCode.Equals(result.DrugCode)).First().ChkDrugTakedLv ?? 1;

                PrscptBill? rec = null;
                if (chkDrugTakedLv == 1)
                {//查同院1個月內
                    rec = _db.PrscptBill.Where(
                       x => x.PrscptNo == result.PrscptNo
                        && x.DrugCode == result.DrugCode
                        && x.PatientNo == result.PatientNo
                        && x.PrscptDate == result.PrscptDate
                        && x.Pharmarcy == result.Pharmarcy
                        && x.PrscptDate >= DateTime.Now.AddMonths(-1).Date
                    ).FirstOrDefault();
                }
                if (chkDrugTakedLv == 2)
                {//查全院2週內
                    rec = _db.PrscptBill.Where(
                       x => x.PrscptNo == result.PrscptNo
                        && x.DrugCode == result.DrugCode
                        && x.PatientNo == result.PatientNo
                        && x.PrscptDate == result.PrscptDate
                        && x.PrscptDate >= DateTime.Now.AddDays(-14).Date
                    ).FirstOrDefault();
                }

                if (rec == null && result.TtlQty != null && result.TtlQty > 0)
                {
                    if(SysBaseServ.JsonConf("TestEnvironment:HsptlApi") != "Y") //正式環境,才有HIS
                    {
                        HsptlApiService hsptlServ = new HsptlApiService(_loginfo);
                        ApiQueryObj apiQ = new ApiQueryObj(data);
                        result.HISchk = hsptlServ.getOutStorage(apiQ);
                    }
                    _db.Add(result);
                    _db.SaveChanges();
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Pharmarcy)) { rec.ScanTime++; _db.SaveChanges(); }
                    result.FID = rec == null ? -1 : rec.FID;
                    result.ScanTime = rec == null ? 0 : rec.ScanTime;
                    result.DoneFill = rec == null ? false : rec.DoneFill;
                }
            }
            catch (Exception ex)
            {
                result.FID = -1;
                if (result.TtlQty != null) { qwServ.savelog(ex.ToString()); } //藥單沒數量,可能是故意的,不記到log
            }

            #endregion
            return result;
        }

        public PrscptBillInfo GetPrscptBill(string[] data) //單純取得id
        {
            PrscptBillInfo result = new PrscptBillInfo();
            if (data.Length != 17) { result.FID = -1; return result; }
            #region 藥單
            try
            {
                PrscptBill bill = new PrscptBill(data);

                PrscptBill? rec = _db.PrscptBill.Where(
                       x => x.PrscptNo == bill.PrscptNo
                    && x.DrugCode == bill.DrugCode
                    && x.PatientNo == bill.PatientNo
                    && x.PrscptDate == bill.PrscptDate
                ).FirstOrDefault();
                if (rec != null)
                {
                    bill = rec;
                }
                result = new PrscptBillInfo(bill);
            }
            catch (Exception ex)
            {
                result.FID = -1;
                qwServ.savelog(ex.ToString());
            }

            #endregion
            return result;
        }
        #endregion

        #region Packages取得包裝
        public List<DrugPackage> GetPackages(int DrugFid)
        {
            return _db.DrugPackage.Where(x => x.DrugFid == DrugFid && x.UseFor == "input").ToList();
        }
        #endregion

        #region DrugGrids取得藥格藥品
        public List<DrugGridInfo>? GetDrugGrids(int DrugFid)
        {
            List<DrugGridInfo>? result = new List<DrugGridInfo>();
            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
            {
                try
                {
                    result = (from grid in _db.DrugGrid
                            join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                            join draw in _db.Drawers on grid.DrawFid equals draw.FID
                            where grid.DrugFid == DrugFid && grid.CbntFid == AtCbntFid
                            orderby grid.Qty descending
                            select new DrugGridInfo
                            {
                                FID = grid.FID,
                                DrawFid = grid.DrawFid,
                                DrugFid = grid.DrugFid,
                                StockQty= grid.Qty,
                                StockTakeType = grid.StockTakeType??"",
                                SafetyStock=grid.SafetyStock,
                                DrawerNo = draw.No.ToString(),
                                UnitConvert = grid.UnitConvert,
                                OffsetCbntFid = grid.OffsetCbntFid,
                                OffsetDrawFid = grid.OffsetDrawFid,
                                ReturnEmptyBottle = grid.ReturnEmptyBottle,
                                DailyMaxTake = grid.DailyMaxTake,
                                BatchActive = grid.BatchActive,
                                MaxLimitQty = grid.MaxLimitQty,
                            }).ToList();

                    foreach(var item in result)
                    {
                        item.gridBatches = _db.DrugGridBatchNo
                            .Where(x => x.GridFid == item.FID && x.Qty > 0 && x.ExpireDate >= DateTime.Now.AddMonths(-3).Date)
                            .OrderByDescending(x => x.ExpireDate)
                            .Take(5)
                            .ToList();
                    }

                }
                catch(Exception ex) { result = null; SysBaseServ.Log(_loginfo, ex); }
            }
            return result;
        }
        #endregion

        #region Offset 取得沖銷資料
        public QryOffsetDrawers GetOffsetDatas(StockBill obj)
        {
            string OffsetQryKey = $"{obj.DrugCode}+{obj.ToFloor}+{obj.CbntFid}";
            QryOffsetDrawers result = new QryOffsetDrawers(OffsetQryKey);
            try
            {
                //先確認藥格設定是否都完全
                List<DrugGridInfo>? grids = GetDrugGrids(getDrugFid(result.DrugCode));
                if (grids == null)
                {
                    if (grids == null || grids.Count == 0) { result.isValid = false; result.InvalidMsg = "查詢庫存錯誤"; return result; }
                }
                if (grids.Count == 0)
                {
                    if (grids == null || grids.Count == 0) { result.isValid = false; result.InvalidMsg = "查無庫存"; return result; }
                }
                int convertcnt = grids.Where(x => x.UnitConvert != null && x.UnitConvert > 0).ToList().Count;
                if (convertcnt > 0)
                {
                    if (grids[0].UnitConvert != grids.Average(x => x.UnitConvert ?? 0)) //多筆cc轉換一瓶 設定不一致
                    {
                        result.isValid = false; result.InvalidMsg = "藥格的單位數量轉設定不一致，無法繼續操作！"; return result;
                    }
                    else
                    {
                        result.UnitConvert = grids[0].UnitConvert;
                    }
                    //是否還空瓶
                    result.Qty_ReturnEmpty = grids[0].ReturnEmptyBottle ?? false ? 0 : -1;
                }                

                //取得對沖資料
                List<MapPrscptOnBill> rec = _db.MapPrscptOnBill
                    .Where(x => x.OffsetQryKey == OffsetQryKey && x.OffsetGroup == "")
                    .OrderBy(x => x.moddate).ToList();
                foreach (MapPrscptOnBill r in rec)
                {
                    if (r.PrscptFid == 0 && r.StockbillFid > 0)
                    {
                        result.stockBills.Add(_db.StockBill.Find(r.StockbillFid));
                        DrugInfo drug = (from d in _db.DrugInfo
                                        join grid in _db.DrugGrid on d.FID equals grid.DrugFid
                                        where grid.FID == result.stockBills.Last().DrugGridFid
                                        select d).First();
                        result.stockBills.Last().DrugCode = drug.DrugCode;
                        result.stockBills.Last().DrugName = drug.DrugName ?? "";
                        result.stockBills.Last().DrugFid = drug.FID;
                    }
                    if (r.PrscptFid > 0 && r.StockbillFid == 0)
                    {
                        result.bills.Add(new PrscptBillInfo(_db.PrscptBill.Find(r.PrscptFid)));
                    }
                }
            }
            catch(Exception ex) {
                result.isValid = false; result.InvalidMsg = "出現錯誤";
                SysBaseServ.Log(_loginfo, ex); 
            }
            
            return GetOffsetQtys(result);
        }
        public QryOffsetDrawers GetOffsetQtys(QryOffsetDrawers obj)
        {
            try
            {
                obj.Qty_Prscpt = 0;
                obj.Qty_Apply = 0;
                obj.Qty_ReturnDrug = 0;
                obj.Qty_ReturnEmpty = obj.Qty_ReturnEmpty == -1 ? obj.Qty_ReturnEmpty : 0; //不還空瓶為-1,則不變,否則歸零
                obj.Qty_Prscpt = obj.bills.Select(x => x.TtlQty).Sum() ?? 0;
                foreach (StockBill r in obj.stockBills)
                {
                    //取實瓶
                    if (!r.TradeType && r.DrugCode == obj.DrugCode) { obj.Qty_Apply += r.Qty; continue; }
                    //放實瓶
                    if(r.TradeType && r.DrugCode == obj.DrugCode) { obj.Qty_ReturnDrug += r.Qty; continue; }
                    //放空瓶
                    if (r.TradeType && r.DrugCode != obj.DrugCode && obj.Qty_ReturnEmpty != -1) { obj.Qty_ReturnEmpty += r.Qty; continue; }
                }
            }
            catch (Exception ex){ SysBaseServ.Log(_loginfo, ex); }
            return obj;
        }
        #endregion

        #region GetDrawers 用StockBill找有哪些藥格,並整合所有資訊到view .
        //領藥,調入調出清單
        public QryDrawers GetDrawers(StockBill_Prscpt obj)
        {
            QryDrawers result = new QryDrawers(obj.DrugCode);
            try
            {
                //確認藥品代碼
                result.stockBill = obj;
                DrugInfo? theDrug = _db.DrugInfo.Where(x => x.DrugCode == obj.DrugCode).FirstOrDefault();
                if (theDrug == null) { result.isValid = false; result.InvalidMsg = "查無此藥品"; return result; }
                result.stockBill.DrugFid = theDrug.FID;

                //確認藥單
                if (!string.IsNullOrEmpty(obj.Scantext))
                {
                    if (result.stockBill.BillType != "RTD") { result.bill = SetPrscptBill(obj.Scantext.Split(';')); }
                    else { result.bill = GetPrscptBill(obj.Scantext.Split(';')); } //退藥沒判斷HIS,之前有問題再說

                    if (result.bill.FID < 0) { result.isValid = false; result.InvalidMsg = "此藥單出現錯誤"; return result; }
                    if (result.bill.FID == 0 && result.stockBill.BillType == "RTD") { result.isValid = false; result.InvalidMsg = "查無此藥單，可至批次領藥處先行刷入。"; return result; }

                    if (result.bill.DoneFill && result.stockBill.BillType.StartsWith("DF")) //領藥完成
                    { result.isValid = false; result.InvalidMsg = "此藥單已完成領藥"; return result; }

                    //限定藥品-贈藥FreeTrial
                    if (result.stockBill.BillType.StartsWith("DF"))
                    {
                        List<DrugLimitedTo> limited = _db.DrugLimitedTo.Where(x => x.DrugFid == theDrug.FID && x.ActiveType == "FreeTrial").ToList();
                        if (limited.Count > 0)
                        {
                            DrugLimitedTo? has = limited.Where(x => x.TargetPatient == result.bill.PatientNo).FirstOrDefault();
                            if (has == null) { result.isValid = false; result.InvalidMsg = "此病歷號不在贈藥名單內，不得領藥！"; return result; }
                            if (has.Qty < result.bill.TtlQty) { result.isValid = false; result.InvalidMsg = "此贈藥名單數量不足，不得領藥！"; return result; }
                        }
                    }


                    //查舊異動單,為了看領過的數量
                    List<string> RTtypes = new List<string> { "RTD", "RTS" };
                    if (result.bill.FID > 0 && (result.stockBill.BillType.StartsWith("DF") || result.stockBill.BillType.StartsWith("RTD")))
                    {
                        MapPrscptOnBill? map = _db.MapPrscptOnBill
                            .Where(x => x.PrscptFid == result.bill.FID &&
                            (RTtypes.Contains(x.BillType) ? x.BillType.Contains(x.BillType) : x.BillType == result.stockBill.BillType))
                            .OrderByDescending(x => x.moddate)
                            .FirstOrDefault();
                        if (map != null)
                        {
                            result.stockBill = new StockBill_Prscpt(_db.StockBill.Find(map.StockbillFid));
                            result.stockBill.BillType = obj.BillType;
                        }
                        else { result.stockBill.TargetQty = result.bill.TtlQty ?? 0; }
                        result.stockBill.PrscptFid = new List<int>() { result.bill.FID };
                    }

                    //服用頻次表
                    if (!result.stockBill.TradeType && !string.IsNullOrEmpty(result.bill.DrugFrequency))
                    {
                        decimal? FreqTime = _db.MapDrugFreqOnCode.Where(x => x.HsptlFreqCode == result.bill.DrugFrequency).Select(x => x.FreqTime).FirstOrDefault();
                        decimal DrugDose = 0;
                        try { DrugDose = Convert.ToDecimal(result.bill.DrugDose); } catch { }
                        result.bill.DailyTake = ((FreqTime ?? 0) * DrugDose);
                    }

                    //補齊可能缺失但後續要的資料
                    result.stockBill.DrugFid = theDrug.FID;
                    result.stockBill.DrugCode = theDrug.DrugCode;
                    result.stockBill.DrugName = theDrug.DrugName ?? "";

                }

                //是否分次 變成新的 庫存異動單
                if (result.stockBill.FID > 0 && result.stockBill.JobDone && result.stockBill.TargetQty == result.stockBill.Qty)
                { result.isValid = false; result.InvalidMsg = "本藥單已完成操作數量！"; return result; }
                bool CopyBill = result.stockBill.FID > 0 && result.stockBill.JobDone && result.stockBill.TargetQty != result.stockBill.Qty;
                if (CopyBill) //分次領藥, 本次變成一份新的庫存異動單
                {
                    result.stockBill.FID = 0;
                    result.stockBill.TargetQty = result.stockBill.TargetQty - result.stockBill.Qty;
                    result.stockBill.Qty = result.stockBill.TargetQty;
                    result.stockBill.Scantext = obj.Scantext;
                }

                //藥格
                #region 庫存資訊
                result.drGridinfo = GetDrugGrids(theDrug.FID);
                if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無藥格設定"; return result; }
                if (result.drGridinfo.Where(x => x.StockQty >= result.stockBill.TargetQty).ToList().Count <= 0 && !result.stockBill.TradeType)
                {
                    result.isValid = false; result.InvalidMsg = "庫存不足"; return result;
                }
                if (result.drGridinfo.Select(x => x.StockQty).Sum() == 0) { result.isValid = false; result.InvalidMsg = "庫存為0"; return result; };

                //多筆cc轉換一瓶 可用功能頁: DFB,OSA
                List<string> CCtypes = new List<string> { "DFB", "OSA", "BXI", "SRI", "SRO", "LRG" }; //可用cc之白名單
                if (result.drGridinfo.Where(x => x.UnitConvert != null).ToList().Count > 0 && !CCtypes.Contains(obj.BillType))
                {
                    result.isValid = false; result.InvalidMsg = "此藥品不得在此功能操作"; return result;
                }
                #endregion

                //包裝
                #region 包裝
                result.PackageCnt = GetPackages(theDrug.FID).Count;
                #endregion
                return result;
            }
            catch (Exception ex) {
                result.isValid = false; result.InvalidMsg = "查詢藥格資訊出錯誤！"; return result;
            }
            
        }
        //批次,cc 多藥單vs一庫存異動單
        public QryBatchDrawers GetDrawers(QryBatchDrawers obj)
        {
            try
            {
                if (obj.stockBill == null) { return null; }
                obj.InvalidMsg = "";
                //確認藥品代碼
                if (string.IsNullOrEmpty(obj.DrugCode)) { obj.DrugCode = obj.stockBill.DrugCode; }
                DrugInfo? theDrug = _db.DrugInfo.Where(x => x.DrugCode == obj.DrugCode).FirstOrDefault();
                if (theDrug == null) { obj.isValid = false; obj.InvalidMsg = "查無此藥品"; return obj; }


                //確認藥單
                if (!string.IsNullOrEmpty(obj.stockBill.Scantext)) //是否有藥單資訊
                {
                    PrscptBillInfo newbill = SetPrscptBill(obj.stockBill.Scantext.Split(';'));
                    newbill.Scantext = obj.stockBill.Scantext;
                    if (newbill.FID < 0 || newbill.DoneFill) //藥單有問題
                    {
                        newbill.FID = -1;
                        newbill.msg = newbill.DoneFill ? "此藥單已完成領藥" : "此藥單出現錯誤";
                    }

                    if (newbill.FID > 0) //有刷過,領過一半也不能在這操作
                    {
                        MapPrscptOnBill? map = _db.MapPrscptOnBill
                            .Where(x => x.PrscptFid == newbill.FID).FirstOrDefault();
                        if (map != null)
                        {
                            StockBill oldstbill = _db.StockBill.Find(map.StockbillFid);
                            if (oldstbill != null)
                            {
                                if (oldstbill.Qty > 0) { newbill.msg = "此藥單已操作過，不得在此繼續"; }
                            }
                        }
                    }

                    //限定藥品-贈藥FreeTrial
                    List<DrugLimitedTo> limited = _db.DrugLimitedTo.Where(x => x.DrugFid == theDrug.FID && x.ActiveType == "FreeTrial").ToList();
                    if (limited.Count > 0)
                    {
                        DrugLimitedTo? has = limited.Where(x => x.TargetPatient == newbill.PatientNo).FirstOrDefault();
                        if (has == null) { newbill.msg = "此病歷號不在贈藥名單內，不得領藥！"; }
                        if (has != null && has.Qty < newbill.TtlQty) { newbill.msg = "此贈藥名單數量不足，不得領藥！"; }
                    }

                    //服用頻次表
                    if (!obj.stockBill.TradeType && !string.IsNullOrEmpty(newbill.DrugFrequency))
                    {
                        decimal? FreqTime = _db.MapDrugFreqOnCode.Where(x => x.HsptlFreqCode == newbill.DrugFrequency).Select(x => x.FreqTime).FirstOrDefault();
                        decimal DrugDose = 0;
                        try { DrugDose = Convert.ToDecimal(newbill.DrugDose); } catch { }
                        newbill.DailyTake = ((FreqTime ?? 0) * DrugDose);
                    }

                    //將 藥單資料 整合至 庫存異動單
                    obj.bills.Add(newbill);
                    obj.stockBill.DrugCode = theDrug.DrugCode;
                    obj.stockBill.DrugName = theDrug.DrugName ?? "";

                    if (obj.bills.Where(x => (x.msg ?? "") == "").Count() == 0)
                    {
                        obj.isValid = false;
                        obj.InvalidMsg = "無可領藥單據!";
                    }


                }
                //新增刪除都要整合至 庫存異動單 的資料
                obj.stockBill.PrscptFid = obj.bills.Where(x => string.IsNullOrEmpty(x.msg)).Select(x => x.FID).ToList();
                obj.stockBill.DrugFid = theDrug.FID;
                obj.stockBill.Qty = obj.bills.Where(x => string.IsNullOrEmpty(x.msg)).Select(x => x.TtlQty ?? 0).Sum();
                obj.stockBill.TargetQty = obj.bills.Where(x => string.IsNullOrEmpty(x.msg)).Select(x => x.TtlQty ?? 0).Sum();
                obj.stockBill.Scantext = "";

                //藥格
                #region 庫存資訊   
                //換算整瓶邏輯
                if (obj.drGridinfo?.Count == 0)
                {
                    obj.drGridinfo = GetDrugGrids(theDrug.FID);
                    if (obj.drGridinfo == null || obj.drGridinfo.Count == 0) { obj.isValid = false; obj.InvalidMsg = "查無藥格設定"; return obj; }
                }
                int convertcnt = obj.drGridinfo.Where(x => x.UnitConvert != null && x.UnitConvert > 0).ToList().Count;
                if (convertcnt > 0)
                {
                    if (obj.drGridinfo[0].UnitConvert != obj.drGridinfo.Average(x => x.UnitConvert ?? 0)) //多筆cc轉換一瓶 設定不一致
                    {
                        obj.isValid = false; obj.InvalidMsg = "藥格的單位數量轉設定不一致，無法繼續操作！"; return obj;
                    }
                    if (obj.isValid && convertcnt == obj.drGridinfo.Count) //多筆cc轉換一瓶
                    {
                        decimal ttlcc = obj.stockBill.TargetQty;
                        decimal UnitConvert = obj.drGridinfo[0].UnitConvert ?? 0;
                        obj.stockBill.TargetQty = Math.Round(ttlcc / UnitConvert, 0); obj.stockBill.Qty = obj.stockBill.TargetQty;
                        if (ttlcc % UnitConvert > 0) { obj.isValid = false; obj.InvalidMsg = "藥品數量不滿整瓶，無法開櫃！"; return obj; }
                    }
                }

                if (obj.drGridinfo.Where(x => x.StockQty >= obj.stockBill.TargetQty).ToList().Count <= 0)
                {
                    obj.isValid = false; obj.InvalidMsg = "查無庫存"; return obj;
                }
                #endregion

                //包裝
                #region 包裝
                obj.PackageCnt = GetPackages(theDrug.FID).Count;
                #endregion
                return obj;
            }
            catch (Exception ex){ obj.isValid = false; obj.InvalidMsg = "查詢藥格資訊出錯誤！"; return obj; }
        }
        //調入調出
        public QryDrawersForTrans GetDrawers(QryDrawersForTrans obj)
        {
            StockBill bill = new StockBill();
            QryDrawersForTrans result = obj;

            //判斷要開櫃的是哪個異動單
            if(obj.TransGoBill != null && obj.TransInBill != null)
            {
                bill = obj.TransGoBill.BillType == obj.sBillType ? obj.TransGoBill : obj.TransInBill;
            }
            else
            {
                bill = obj.TransGoBill != null ? obj.TransGoBill : obj.TransInBill;
            }            
            if (bill == null) { result.isValid = false; result.InvalidMsg = "異動資訊出現錯誤"; return result; }
            obj.DrugCode = bill.DrugCode;
            //確認藥品代碼
            int DrugFid = getDrugFid(obj.DrugCode);
            if (DrugFid == -1) { result.isValid = false; result.InvalidMsg = "查無藥品代碼"; return result; }
            if (DrugFid == -1) { result.isValid = false; result.InvalidMsg = "查無藥品代碼"; return result; }

            //藥格
            #region 庫存資訊            
            result.drGridinfo = GetDrugGrids((int)DrugFid);
            if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無藥格設定"; return result; }
            if (!bill.TradeType && result.drGridinfo.Where(x => x.StockQty >= bill.TargetQty).ToList().Count <= 0)
            {
                result.isValid = false; result.InvalidMsg = "查無庫存"; return result;
            }
            //判斷是否有放得下的藥格
            if((obj.TransGoBill?.CbntFid ?? 0) != 0)
            {
                if(result.drGridinfo.Where(x => ((x.StockQty + bill.TargetQty) < x.MaxLimitQty) || x.MaxLimitQty == null).Count() == 0)
                {
                    result.isValid = false; result.InvalidMsg = "調入的藥櫃查無有足夠空間存放的藥格"; return result;
                }
            }


            #endregion

            //包裝
            #region 包裝
            result.PackageCnt = GetPackages(DrugFid).Count;
            #endregion
            return result;
        }
        //住院退藥
        public QryDrawers GetDrawers_RTS(StockBill_Prscpt obj)
        {
            //確認藥品代碼
            QryDrawers result = new QryDrawers(obj.DrugCode);
            result.stockBill = obj;
            DrugInfo? theDrug = _db.DrugInfo.Where(x => x.DrugCode == obj.DrugCode).FirstOrDefault();
            if (theDrug == null) { result.isValid = false; result.InvalidMsg = "查無此藥品"; return result; }
            result.stockBill.DrugFid = theDrug.FID;
            
            //確認藥單
            if (!string.IsNullOrEmpty(obj.Scantext))
            {
                result.bill = new PrscptBillInfo(new PrscptBill(obj.Scantext.Split(';')));
                try
                {
                    PrscptBill? obill = _db.PrscptBill.Where(
                           x => x.PrscptNo == result.bill.PrscptNo
                        && x.DrugCode == result.bill.DrugCode
                        && x.PatientNo == result.bill.PatientNo
                        && x.BedCode == result.bill.BedCode
                    ).FirstOrDefault();
                    if (obill != null) { result.bill = new PrscptBillInfo(obill); }
                }
                catch { result.bill.FID = -1; }

                if (result.bill.FID < 0) { result.isValid = false; result.InvalidMsg = "此藥單出現錯誤"; return result; }
                
                //查舊異動單,為了看領過的數量
                if (result.bill.FID > 0)
                {
                    List<string> RTtypes = new List<string> { "RTD", "RTS" };
                    MapPrscptOnBill? map = _db.MapPrscptOnBill
                        .Where(x => x.PrscptFid == result.bill.FID && RTtypes.Contains(result.stockBill.BillType))
                        .OrderByDescending(x => x.moddate).FirstOrDefault();
                    if (map != null) {
                        result.stockBill = new StockBill_Prscpt(_db.StockBill.Find(map.StockbillFid));
                        result.stockBill.BillType = "RTS";
                    }
                    else { result.stockBill.TargetQty = result.bill.TtlQty ?? 0; }
                    result.stockBill.PrscptFid = new List<int>() { result.bill.FID };
                }

                //補齊可能缺失但後續要的資料
                result.stockBill.DrugFid = theDrug.FID;
                result.stockBill.DrugCode = theDrug.DrugCode;
                result.stockBill.DrugName = theDrug.DrugName ?? "";
                result.stockBill.ReturnSheet = obj.ReturnSheet;
            }

            //是否分次 變成新的 庫存異動單
            if (result.stockBill.FID > 0 && result.stockBill.JobDone && result.stockBill.TargetQty == result.stockBill.Qty)
            { result.isValid = false; result.InvalidMsg = "本藥單已完成操作數量！"; return result; }
            bool CopyBill = result.stockBill.FID > 0 && result.stockBill.JobDone && result.stockBill.TargetQty != result.stockBill.Qty;
            if (CopyBill) //分次領藥, 本次變成一份新的庫存異動單
            {
                result.stockBill.FID = 0;
                result.stockBill.TargetQty = result.stockBill.TargetQty - result.stockBill.Qty;
                result.stockBill.Qty = result.stockBill.TargetQty < obj.Qty ? result.stockBill.TargetQty : obj.Qty;
                result.stockBill.Scantext = obj.Scantext;
            }

            //藥格
            #region 庫存資訊            
            result.drGridinfo = GetDrugGrids(theDrug.FID);
            if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無藥格設定"; return result; }
            if (result.drGridinfo.Where(x => x.StockQty >= result.stockBill.TargetQty).ToList().Count <= 0)
            {
                result.isValid = false; result.InvalidMsg = "查無庫存"; return result;
            }
            #endregion

            //包裝
            #region 包裝
            result.PackageCnt = GetPackages(theDrug.FID).Count;
            #endregion
            return result;
        }
        //借還藥(判斷 空瓶/實瓶/藥單)
        public QryDrawers GetDrawers_Offset(StockBill_Prscpt obj)
        {
            //確認藥品代碼
            QryDrawers result = new QryDrawers(obj.DrugCode);
            result.stockBill = obj;
            int DrugFid = getDrugFid(obj.DrugCode);
            result.stockBill.DrugFid = DrugFid;
            if (DrugFid == -1) { result.isValid = false; result.InvalidMsg = "查無藥品代碼"; return result; }

            //確認藥單
            if (!string.IsNullOrEmpty(obj.Scantext))
            {
                result.bill = SetPrscptBill(obj.Scantext.Split(';'));

                if(result.bill.TtlQty != null) //真有藥單 否則 沒藥單,開門放藥專用
                {
                    if (result.bill.FID < 0) { result.isValid = false; result.InvalidMsg = "此藥單出現錯誤"; return result; }

                    if (result.bill.DoneFill) //沖藥單應該是只能用一次
                    { result.isValid = false; result.InvalidMsg = "此藥單已在系統中，不得再次沖銷"; return result; }

                    
                }
                
                //補齊可能缺失但後續要的資料
                result.stockBill.DrugFid = DrugFid;
                result.stockBill.DrugCode = result.bill.DrugCode ?? "";
                result.stockBill.DrugName = result.bill.DrugName ?? "";
            }

            //藥格
            #region 庫存資訊            
            result.drGridinfo = GetDrugGrids((int)DrugFid);
            if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無藥格設定"; return result; }

            //是否改開空瓶櫃 藥單
            if ((result.drGridinfo[0].ReturnEmptyBottle ?? false) && (!string.IsNullOrEmpty(obj.Scantext)))
            {
                string BottleCode = $"{result.DrugCode}_b";
                result.drGridinfo = GetDrugGrids(getDrugFid(BottleCode));
                if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無空瓶庫存設定"; return result; }
            }

            if (!obj.TradeType && result.drGridinfo.Where(x => x.StockQty >= obj.TargetQty).ToList().Count <= 0)
            {
                result.isValid = false; result.InvalidMsg = "查無庫存"; return result;
            }

            #endregion

            //包裝
            #region 包裝
            result.PackageCnt = 0;
            #endregion
            return result;
        }
        //盤點
        public QryDrawers GetDrawers_StockTaking(StockBill_Prscpt obj)
        {
            //確認藥品代碼
            QryDrawers result = new QryDrawers(obj.DrugCode);
            result.stockBill = obj;
            int DrugFid = getDrugFid(obj.DrugCode);
            result.stockBill.DrugFid = DrugFid;
            if (DrugFid == -1) { result.isValid = false; result.InvalidMsg = "查無藥品代碼"; return result; }

            //藥格
            #region 庫存資訊            
            result.drGridinfo = GetDrugGrids((int)DrugFid).Where(x => x.DrawFid == obj.DrawFid).ToList();
            if (result.drGridinfo == null || result.drGridinfo.Count == 0) { result.isValid = false; result.InvalidMsg = "查無藥格設定"; return result; }
            #endregion

            //包裝
            #region 包裝
            result.PackageCnt = GetPackages(DrugFid).Count;
            #endregion

            //預設實取量 盤點(STK)=0, 領取空瓶(BEO)=庫存量
            result.stockBill.TargetQty = obj.BillType == "STK" ? 0 : result.drGridinfo[0].StockQty;

            return result;
        }
        #endregion

        #region Trans 調出調入的存檔
        public ResponObj<string> SaveTransApply(QryDrawersForTrans obj)
        {
            try
            {
                //儲存本次異動
                if(string.IsNullOrEmpty(obj.sBillType) || !(new List<string> { "TG1","TI1" }).Contains(obj.sBillType))
                {
                    return new ResponObj<string>("Err", "調入調出申請存檔時，異動資料類別有誤");
                }                
                StockBill? bill = obj.sBillType == "TG1" ? obj.TransGoBill : obj.TransInBill;
                if (bill == null) { return new ResponObj<string>("Err", "調入調出申請存檔時，查無異動資料"); }                
                bill.DrugGridFid = getDrugGridFid(bill.DrawFid ?? 0, obj.DrugCode);
                if (bill.DrugGridFid == 0) { return new ResponObj<string>("Err", "調入調出申請存檔時，查無異動的藥格資料"); }
                bill.moddate = DateTime.Now;
                bill.JobDone = true;
                _db.Add(bill);
                _db.SaveChanges();
                //更新庫存
                AfterStocking(bill);

                //產生對象異動單
                StockBill? tobill = obj.sBillType == "TG1" ? obj.TransInBill : obj.TransGoBill;
                if (tobill == null) { return new ResponObj<string>("Err", "調入調出申請存檔時，查無異動對相資料"); }
                if (tobill.CbntFid == 0) { return new ResponObj<string>("0", bill.FID.ToString()); } //無對象,結束
                tobill.DrugGridFid = 0;
                tobill.FromFid = bill.FID;
                tobill.DrugCode = bill.DrugCode;
                tobill.ExpireDate = bill.ExpireDate;
                tobill.BatchNo = bill.BatchNo;
                tobill.TargetQty = bill.TargetQty;
                tobill.moddate = bill.moddate;
                tobill.JobDone = false;
                if (tobill.CbntFid > 0)
                {                    
                    _db.Add(tobill);
                    _db.SaveChanges();
                }                

                //產生可退回異動單
                if (bill.CbntFid != tobill.CbntFid)
                {
                    StockBill? rejbill = JsonConvert.DeserializeObject<StockBill>(JsonConvert.SerializeObject(tobill));
                    if (rejbill == null) { return new ResponObj<string>("Err", "調入調出申請存檔時，產生退回異動單失敗"); }
                    rejbill.CbntFid = bill.CbntFid;
                    rejbill.FID = 0;
                    _db.Add(rejbill);
                    _db.SaveChanges();
                }

                //產生全院櫃子的異動單
                if (tobill.CbntFid == -1)
                {
                    List<int> allcbnt = _db.Cabinet.Where(x => x.comFid == tobill.comFid && x.FID != bill.CbntFid)
                        .Select(x => x.FID)
                        .ToList();
                    List<StockBill> toall = new List<StockBill>();
                    foreach (int i in allcbnt)
                    {
                        StockBill? newto = JsonConvert.DeserializeObject<StockBill>(JsonConvert.SerializeObject(tobill));
                        if (newto == null) { return new ResponObj<string>("Err", "調入調出申請存檔時，產生多筆對象異動單失敗"); }
                        newto.CbntFid = i; newto.FID = 0; toall.Add(newto);
                    }
                    _db.AddRange(toall);
                    _db.SaveChanges();
                }

                return new ResponObj<string>("0", bill.FID.ToString());
            }
            catch (Exception ex) { 
                SysBaseServ.Log(_loginfo, ex);
                return new ResponObj<string>("ex", "調入調出申請存檔發生錯誤(SaveTransApply)");
            }

        }

        public ResponObj<string> SaveTransUpdate(StockBill_Prscpt obj)
        {
            try
            {
                //儲存本次異動                
                StockBill? bill = obj;
                if (bill == null) { return new ResponObj<string>("Err", "調入調出異動存檔時，查無異動資料"); }
                bill.DrugGridFid = getDrugGridFid(bill.DrawFid ?? 0, obj.DrugCode);
                if (bill.DrugGridFid == 0 && bill.Qty > 0) { return new ResponObj<string>("Err", "調入調出異動存檔時，查無異動的藥格資料"); }
                bill.moddate = DateTime.Now;
                bill.JobDone = true;
                bill.RecNote = null;
                _db.Update(bill);
                _db.SaveChanges();
                //更新庫存
                if(bill.Qty > 0) { AfterStocking(bill); }

                decimal remainQty = bill.TargetQty - bill.Qty; //剩餘量

                //有剩 同步其它的目標量,原異動加一筆(分次)
                if (remainQty > 0)
                {
                    List<StockBill> bills = _db.StockBill
                        .Where(x => x.FromFid == bill.FromFid && !x.JobDone && x.FID != bill.FID)
                        .ToList();
                    foreach(StockBill item in bills)
                    {
                        item.TargetQty = remainQty;
                    }
                    _db.StockBill.UpdateRange(bills);
                    _db.SaveChanges(true);

                    StockBill? addbill = JsonConvert.DeserializeObject<StockBill>(JsonConvert.SerializeObject(bill));
                    if (addbill == null) { return new ResponObj<string>("Err", "調入調出異動存檔時，產生分次異動錯誤"); }
                    addbill.FID = 0;
                    addbill.TargetQty = remainQty;
                    addbill.Qty = 0;
                    addbill.JobDone = false;
                    addbill.DrugGridFid = 0;
                    addbill.SysChkQty = 0;
                    addbill.UserChk1Qty = null;
                    addbill.UserChk2Qty = null;
                    addbill.TakeType = null;
                    _db.StockBill.Add(addbill);
                    _db.SaveChanges();
                }

                //沒剩 還沒用的全刪
                if(remainQty == 0)
                {
                    List<StockBill> bills = _db.StockBill.Where(x => x.FromFid == bill.FromFid && !bill.JobDone).ToList();
                    _db.StockBill.RemoveRange(bills);
                    _db.SaveChanges(true);
                }

                return new ResponObj<string>("0", bill.FID.ToString());
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(_loginfo, ex);
                return new ResponObj<string>("ex", "調入調出異動存檔發生錯誤(SaveTransApply)");
            }

        }

        #endregion

        #region After saved 庫存異動後的更新及通知
        //新增藥單與庫存異動單對應
        public void SavePrscptMapping(StockBill_Prscpt bill)
        {
            
            if (bill.PrscptFid != null)
            {
                foreach (int id in bill.PrscptFid)
                {
                    if (id == 0) { continue; }
                    try
                    {
                        MapPrscptOnBill rec = _db.MapPrscptOnBill.Where(x => x.BillType == bill.BillType && x.PrscptFid == id && x.StockbillFid == bill.FID).FirstOrDefault();
                        if (rec == null)
                        {
                            MapPrscptOnBill add = new MapPrscptOnBill
                            {
                                BillType = bill.BillType,
                                PrscptFid = id,
                                StockbillFid = bill.FID,
                                ReturnSheet = string.IsNullOrEmpty(bill.ReturnSheet) ? null : bill.ReturnSheet,
                            };
                            _db.Add(add);
                            _db.SaveChanges();
                        }
                    }
                    catch (Exception ex) { SysBaseServ.Log(_loginfo, ex); }
                    
                    
                }
            }
        }
        //沖銷群組-庫存異動
        public void SaveOffsetBill(StockBill_Prscpt bill)
        {
            if (string.IsNullOrEmpty(bill.ToFloor) || string.IsNullOrEmpty(bill.DrugCode)) { return; }
            string qrykey = $"{bill.DrugCode.Replace("_b", "")}+{bill.ToFloor}+{bill.CbntFid}";
            
            try
            {
                if (!string.IsNullOrEmpty(bill.Scantext)) //藥單
                {
                    PrscptBill prscpt = SetPrscptBill(bill.Scantext.Split(";"));
                    if (prscpt != null && prscpt.FID > 0 && prscpt.DoneFill == false)
                    {
                        PrscptBill? update = _db.PrscptBill.Find(prscpt.FID);
                        if (update != null)
                        {
                            update.TtlQty = prscpt.TtlQty;
                            update.DoneFill = true;
                            _db.Update(update);
                            _db.SaveChanges(true);
                        }
                        

                        MapPrscptOnBill? rec =
                        _db.MapPrscptOnBill.Where(x => x.OffsetQryKey == qrykey && x.OffsetGroup == "" && x.PrscptFid == prscpt.FID)
                        .FirstOrDefault();
                        if (rec == null)
                        {
                            MapPrscptOnBill add = new MapPrscptOnBill
                            {
                                BillType = bill.BillType,
                                PrscptFid = prscpt.FID,
                                StockbillFid = 0,
                                OffsetQryKey = qrykey,
                                OffsetGroup = "",
                            };
                            _db.Add(add);
                            _db.SaveChanges();
                        }
                    }                    
                }
                else
                { //庫存異動
                    MapPrscptOnBill? rec =
                        _db.MapPrscptOnBill.Where(x => x.OffsetQryKey == qrykey && x.OffsetGroup == "" && x.StockbillFid == bill.FID)
                        .FirstOrDefault();
                    if (rec == null)
                    {
                        MapPrscptOnBill add = new MapPrscptOnBill
                        {
                            BillType = bill.BillType,
                            PrscptFid = 0,
                            StockbillFid = bill.FID,
                            OffsetQryKey = qrykey,
                            OffsetGroup = "",
                        };
                        _db.Add(add);
                        _db.SaveChanges();
                    }
                }
                
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex); }

        }

        //建立沖銷群組
        public bool SaveOffsetGroupup(QryOffsetDrawers data)
        {
            string OffsetGroup = DateTime.Now.ToString("yyMMddHHmmss");
            try
            {
                List<MapPrscptOnBill> toSave = _db.MapPrscptOnBill
                    .Where(x => x.OffsetGroup == "" && x.OffsetQryKey == data.OffsetQryKey)
                    .ToList();
                foreach (PrscptBillInfo item in data.bills)
                {
                    toSave.Where(x => x.PrscptFid == item.FID && x.StockbillFid == 0).First().OffsetGroup = OffsetGroup;
                }
                foreach (StockBill item in data.stockBills)
                {
                    toSave.Where(x => x.PrscptFid == 0 && x.StockbillFid == item.FID).First().OffsetGroup = OffsetGroup;
                }
                _db.UpdateRange(toSave);
                _db.SaveChanges();
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex); return false; };
            return true;
        }

        //藥單數量寫入沖銷對應藥格(for常備量)
        public ResponObj<string> AddBillQtyToGrid(List<StockBill> data)
        {
            try
            {
                if (data.Count() == 0) { return new ResponObj<string>("Err", "寫入沖銷對應藥格時，無庫存異動紀錄"); }
                DrugGrid? grid = _db.DrugGrid.Find(data[0].DrugGridFid);
                if (grid == null) { return new ResponObj<string>("Err", "寫入沖銷對應藥格時，藥品查無藥格設定"); }
                DrugGrid? offgrid = _db.DrugGrid.Find(grid.OffsetTo);
                if (offgrid == null) { return new ResponObj<string>("Err", "寫入沖銷對應藥格時，查無沖銷對應藥格設定"); }

                decimal addqty = data.Select(x => x.Qty).Sum();
                offgrid.Qty += addqty;
                _db.DrugGrid.Update(offgrid);
                _db.SaveChanges();
                return new ResponObj<string>("0", ""); 
            }
            catch (Exception ex) {
                return new ResponObj<string>("ex", "藥單數量寫入沖銷對應藥格發生錯誤(AddBillQtyToGrid)");
            }

            
        }
        public void AfterStocking(StockBill bill)
        {
            try
            {
                if (bill.Qty == 0) { return; } //無需異動
                //更新庫存
                DrugGrid? dg = _db.DrugGrid.Find(bill.DrugGridFid);
                if (dg == null) { return; }
                dg.Qty = bill.SysChkQty;
                _db.SaveChanges();

                //更新批號數量
                if (!string.IsNullOrEmpty(bill.BatchNo) && bill.Qty > 0)
                {
                    DrugGridBatchNo? batch = _db.DrugGridBatchNo.Where(x=>x.GridFid == bill.DrugGridFid && x.BatchNo == bill.BatchNo).FirstOrDefault();
                    if (batch == null && bill.TradeType)
                    {
                        DrugGridBatchNo add = new DrugGridBatchNo
                        {
                            BatchNo = bill.BatchNo,
                            ExpireDate = _db.DrugGridBatchNo.Where(x=>x.BatchNo == bill.BatchNo).FirstOrDefault()?.ExpireDate ?? DateTime.Now.AddMonths(3).Date,
                            GridFid = bill.DrugGridFid,
                            Qty = bill.Qty,
                        };
                        _db.DrugGridBatchNo.Add(add);
                        _db.SaveChanges();
                    }
                    if(batch != null)
                    {
                        batch.Qty += (bill.Qty * (bill.TradeType ? 1 : -1));
                        _db.DrugGridBatchNo.Update(batch);
                        _db.SaveChanges();
                        if (batch.Qty < 0 && !bill.TradeType) //不足以扣,再扣最舊的批號
                        {
                            decimal overQty = Math.Abs(batch.Qty);
                            batch.Qty = 0;
                            _db.DrugGridBatchNo.Update(batch);
                            _db.SaveChanges();

                            //餘量自動扣舊的批號數量
                            List<DrugGridBatchNo> batchs = _db.DrugGridBatchNo.Where(x => x.GridFid == bill.DrugGridFid && x.Qty > 0 && batch.GID != x.GID).OrderBy(x => x.ExpireDate).ToList();
                            foreach(DrugGridBatchNo item in batchs)
                            {
                                item.Qty -= overQty;
                                if (item.Qty < 0) { overQty = Math.Abs(batch.Qty); item.Qty = 0; }
                                else { overQty = 0; }
                                _db.DrugGridBatchNo.Update(item);
                                _db.SaveChanges(true);
                                if (overQty == 0) { break; }
                            }
                            
                        }
                        
                    }
                }

                //更新限量數量(贈藥,臨採)
                List<string> limitedchk = new List<string>() { "DFM", "DFB", "RTD" };
                if(limitedchk.Contains(bill.BillType))
                {
                    string PatientNo = (from prsc in _db.PrscptBill
                                       join map in _db.MapPrscptOnBill on prsc.FID equals map.PrscptFid
                                       where map.StockbillFid == bill.FID
                                       select prsc.PatientNo).First();

                    int drugfid = _db.DrugInfo.Where(x => x.DrugCode == bill.DrugCode).First().FID;

                    List<DrugLimitedTo> limited = _db.DrugLimitedTo
                        .Where(x => x.DrugFid == drugfid && x.TargetPatient == PatientNo)
                        .ToList();
                        
                    if (limited.Count() > 0)
                    {
                        //贈藥
                        foreach (var item in limited.Where(x=>x.ActiveType == "FreeTrial"))
                        {
                            item.Qty += (bill.Qty * (bill.TradeType ? 1 : -1));
                        }
                        _db.DrugLimitedTo.UpdateRange(limited);
                        _db.SaveChanges();

                        //臨採
                        if(limited.Where(x => x.ActiveType == "AdHocProc").Count() > 0)
                        {
                            DrugLimitedTo? AdHocProc = _db.DrugLimitedTo
                                .Where(x=>x.DrugFid == drugfid && x.ActiveType == "AdHocProc" && x.TargetPatient == "Pool")
                                .FirstOrDefault();
                            if (AdHocProc != null)
                            {
                                AdHocProc.Qty += (bill.Qty * (bill.TradeType ? 1 : -1));
                                _db.DrugLimitedTo.Update(AdHocProc);
                                _db.SaveChanges();
                            }
                        }

                    }
                }
                
                //若是可借還沖消,要順便異動藥單數量
                if ((dg?.OffsetActive ?? false) && dg.OffsetTo > 0)
                {
                    DrugGrid? dg_b = _db.DrugGrid.Find(dg.OffsetTo);
                    if (dg_b != null)
                    {
                        dg_b.Qty += bill.Qty;
                        _db.DrugGrid.Update(dg_b);
                        _db.SaveChanges(true);
                    }
                }

                #region check need send alert mail
                MailService mail = new MailService(_loginfo);
                if (bill.SysChkQty != bill.UserChk2Qty && bill.DrugGridFid > 0)
                {
                    mail.AlertMsg(bill, "errstocktak");
                }
                if (dg.Qty < dg.SafetyStock && bill.TakeType != "" && bill.DrugGridFid > 0)
                {
                    mail.AlertMsg(bill, "lowstock");
                }

                #endregion
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex); }

        }
        #endregion
    }

}


using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Front;
using TpePrmcyWms.Filters;
using System.Collections.Generic;
using TpePrmcyWms.Models.Unit.Back;
using ShareLibrary.Models.HsptlApiUnit;
using ShareLibrary.Models.Service;
using System.Security.Cryptography;

namespace TpePrmcyWms.Controllers.Front
{
    [FrontendSessionFilter]
    public class FrontLoadController : NoAuthController
    {
        DBcPharmacy _db = new DBcPharmacy();

        #region 整合並取得櫃格抽屜
        public IActionResult DrawerTask(StockBill_Prscpt vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawers qDrawers = frserv.GetDrawers(vobj);
            if (qDrawers.isValid && qDrawers.bill != null)
            {
                frserv.DrugPickAdd(vobj.DrugCode);
            }

            return View(qDrawers);
        }

        public IActionResult DrawerBatch_Prscpt(QryBatchDrawers vobj)
        {
            if(vobj.stockBill.DrugCode == "") { return new EmptyResult(); }
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryBatchDrawers qDrawers = frserv.GetDrawers(vobj);
            if (qDrawers.isValid)
            {
                frserv.DrugPickAdd(qDrawers.DrugCode);
            }
            return View(vobj);
        }

        public IActionResult DrawerTask_Trans(QryDrawersForTrans vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawersForTrans qDrawers = frserv.GetDrawers(vobj);
            
            return View(qDrawers);
        }

        public IActionResult DrawerTask_RTS(StockBill_Prscpt vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawers qDrawers = frserv.GetDrawers_RTS(vobj);
            return View(qDrawers);
        }

        public IActionResult DrawerTask_Offset(StockBill_Prscpt vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawers qDrawers = frserv.GetDrawers_Offset(vobj);
            return View(qDrawers);
        }

        public IActionResult DrawerTask_StockTaking(StockBill_Prscpt vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawers qDrawers = frserv.GetDrawers_StockTaking(vobj);
            
            return View(qDrawers);
        }

        public IActionResult DrawerTask_MSD(StockBill_MSD vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryDrawers qDrawers = frserv.GetDrawers_MSD(vobj);            

            return View(qDrawers);
        }

        #endregion

        #region 庫存異動時輸入批號
        public IActionResult BatchNoInputer(int DrugGridFid) //-1時,為入庫
        {
            List<DrugGridBatchNo> result = new List<DrugGridBatchNo>();
            ViewBag.DrugGridFid = DrugGridFid;
            if (DrugGridFid == 0 || DrugGridFid == -1) { return View(result); }
            try
            {
                ViewBag.BatchActive = _db.DrugGrid.Find(DrugGridFid)?.BatchActive ?? false;
                result = _db.DrugGridBatchNo
                     .Where(x => x.GridFid == DrugGridFid && x.ExpireDate >= DateTime.Now.AddMonths(-3).Date && x.Qty > 0)
                     .OrderByDescending(x => x.ExpireDate)
                     .Take(5)
                     .ToList();
                return View(result);
            }
            catch (Exception ex) { return View(null); }
        }
        #endregion

        #region 用包裝輸入數量
        public IActionResult PackageCounter(string qDrugCode)
        {
            List<DrugPackage> result = new List<DrugPackage>();
            ViewBag.DrugCode = qDrugCode;
            if (!string.IsNullOrEmpty(qDrugCode))
            {
                try
                {
                    result = (from pk in _db.DrugPackage
                             join info in _db.DrugInfo on pk.DrugFid equals info.FID
                             where info.DrugCode == qDrugCode && pk.UseFor == "input"
                             select pk).ToList();
                }
                catch (Exception ex) { SysBaseServ.Log(Loginfo, ex); }
            }
            return View(result);
        }
        #endregion

        #region 退藥介面連動HIS專用
        public async Task<IActionResult> ReturnSheet_PrscptNo(StockBill_Prscpt vobj)
        {
            if (string.IsNullOrEmpty(vobj.PatientNo)) { return View(new List<Data_IPDPatSeq>()); }
            if (vobj.PatientNo.Length != 8) { return View(new List<Data_IPDPatSeq>()); }

            string Comid = (from com in _db.Company
                            join cbt in _db.Cabinet on com.FID equals cbt.comFid
                            where cbt.FID == AtCbntFid
                            select com.comid)
                    .First();
            
            List<Data_IPDPatSeq> res = null;
            if(SysBaseServ.JsonConf("TestEnvironment:HsptlApi") == "Y")
            {
                if(vobj.PatientNo.Substring(0, 1) == "0") { res = new List<Data_IPDPatSeq>(); }
                if(vobj.PatientNo.Substring(0, 1) == "1" || vobj.PatientNo.Substring(0, 1) == "2")
                {
                    res = new List<Data_IPDPatSeq>();
                    res.Add(new Data_IPDPatSeq { PAT_NO = "10110001", PAT_SEQ = "K01063", BED_CODE = "0", REAL_OUT_DATE = "1130509" });
                    if (vobj.PatientNo.Substring(0, 1) == "2")
                    {
                        res.Add(new Data_IPDPatSeq { PAT_NO = "10110001", PAT_SEQ = "K01063", BED_CODE = "0", REAL_OUT_DATE = null });
                    }
                }
            }
            else
            {
                try
                {
                    HsptlApiService hsptlServ = new HsptlApiService();
                    Qry_IPDPatSeq apiQ = new Qry_IPDPatSeq();
                    apiQ.HospId = Comid;
                    apiQ.PatNo = vobj.PatientNo;
                    res = await hsptlServ.getIPDPatSeq(apiQ);
                }
                catch (Exception ex) { res = null; }
            }

            return View(res);
        }
        public async Task<IActionResult> ReturnSheet_ReturnSheet(StockBill_Prscpt vobj)
        {
            if (string.IsNullOrEmpty(vobj.PatientNo) ||
                string.IsNullOrEmpty(vobj.PatientSeq) ||
                string.IsNullOrEmpty(vobj.ReturnSheet)) { return View(new List<Data_IPDPatSeq>()); }
            if (vobj.PatientNo.Length != 8 ||
                vobj.PatientSeq.Length != 6 ||
                vobj.ReturnSheet.Length != 13 ) { return View(new List<Data_IPDPatSeq>()); }

            string Comid = (from com in _db.Company
                            join cbt in _db.Cabinet on com.FID equals cbt.comFid
                            where cbt.FID == AtCbntFid
                            select com.comid)
                    .First();

            List<Data_IPDReturn> res = null;
            if (SysBaseServ.JsonConf("TestEnvironment:HsptlApi") == "Y")
            {
                res = new List<Data_IPDReturn>();
                res.Add(new Data_IPDReturn {
                    PAT_NO = "10110001",
                    PAT_SEQ = "K01063", 
                    ODR_CODE = "OGLUC4", 
                    ODR_NAME = "Glucophage 500mg TAB (Metformin)",
                    DRUGBAG_SEQ = "A0001",
                });
            }
            else
            {
                try
                {
                    HsptlApiService hsptlServ = new HsptlApiService();
                    Qry_IPDReturn apiQ = new Qry_IPDReturn();
                    apiQ.HospId = Comid;
                    apiQ.PatNo = vobj.PatientNo;
                    res = await hsptlServ.getIPDReturnStorage(apiQ);
                }
                catch (Exception ex) { res = null; }
            }

            return View(res);
        }
        #endregion

        #region 借還沖銷介面
        public IActionResult OffsetPanel(StockBill vobj)
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            QryOffsetDrawers qDrawers = frserv.GetOffsetDatas(vobj);
            ViewBag.WaitToOffsetTime = SysBaseServ.JsonConf("CustomSetting:WaitToOffsetTime");
            return View(qDrawers);
        }
        #endregion

        #region 施打疫苗 - 增修        
        public IActionResult VaxSkdGet(int fid)
        {
            try
            {
                VaxSkd? obj = _db.VaxSkd.Find(fid);
                if (obj == null)
                {
                    obj = new VaxSkd();
                    obj.comFid = Loginfo.User.comFid;
                    obj.dptFid = Loginfo.User.dptFid;
                    obj.modid = Loginfo.User.Fid;
                    obj.CaseClose = false;
                }
                return View(obj);
            }
            catch(Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "發生錯誤！" });
            }
        }
        public IActionResult VaxSkdDtlGet(int fid, int VaxSkdFid)
        {
            if (VaxSkdFid == 0) { return Json(new { code = "Err", returnData = "未選擇場次！" }); }
            try
            {
                VaxSkdDtl? obj = _db.VaxSkdDtl.Find(fid);
                if (obj == null)
                {
                    obj = new VaxSkdDtl();
                    obj.VaxSkdFid = VaxSkdFid;
                    obj.modid = Loginfo.User.Fid;
                }
                else
                {
                    obj.DrugName = _db.DrugInfo.Where(x => x.FID == obj.DrugFid).Select(x => $"{x.DrugCode}，{x.DrugName}，{x.FID}" ).FirstOrDefault();
                    obj.QtyStockUp = Math.Round(obj.QtyStockUp ?? 0, 0);
                    obj.QtyStockIn = Math.Round(obj.QtyStockIn ?? 0, 0);
                    obj.QtyStockOut = Math.Round(obj.QtyStockOut ?? 0, 0);
                    obj.QtyVax = Math.Round(obj.QtyVax ?? 0, 0);
                    obj.QtyReceive = Math.Round(obj.QtyReceive ?? 0, 0);
                    obj.QtyReturn = Math.Round(obj.QtyReturn ?? 0, 0);
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "發生錯誤！" });
            }
        }
        public IActionResult VaxSkdDtl(int fid)
        {
            try
            {
                List<VaxSkdDtl> data = _db.VaxSkdDtl.Where(x=>x.VaxSkdFid == fid).ToList();                
                return View(data);
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "發生錯誤！" });
            }
        }
        public IActionResult VaxStockDtl(int fid)
        {
            try
            {
                List<VaxSkdDtl> data = _db.VaxSkdDtl.Where(x => x.VaxSkdFid == fid).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "發生錯誤！" });
            }
        }
        public IActionResult VaxStocking(int fid, int VaxSkdFid)
        {
            if (VaxSkdFid == 0) { return Json(new { code = "Err", returnData = "未選擇場次！" }); }
            try
            {
                VaxSkdDtl? obj = _db.VaxSkdDtl.Find(fid);
                if (obj == null)
                {
                    obj = new VaxSkdDtl();
                    obj.VaxSkdFid = VaxSkdFid;
                    obj.modid = Loginfo.User.Fid;
                }
                else
                {
                    obj.DrugName = _db.DrugInfo.Where(x => x.FID == obj.DrugFid).Select(x => $"{x.DrugCode}，{x.DrugName}，{x.FID}").FirstOrDefault();
                    obj.QtyStockUp = Math.Round(obj.QtyStockUp ?? 0, 0);
                    obj.QtyStockIn = Math.Round(obj.QtyStockIn ?? 0, 0);
                    obj.QtyStockOut = Math.Round(obj.QtyStockOut ?? 0, 0);
                    obj.QtyVax = Math.Round(obj.QtyVax ?? 0, 0);
                    obj.QtyReceive = Math.Round(obj.QtyReceive ?? 0, 0);
                    obj.QtyReturn = Math.Round(obj.QtyReturn ?? 0, 0);
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                SysBaseServ.Log(Loginfo, ex);
                return Json(new { code = "Err", returnData = "發生錯誤！" });
            }
        }
        #endregion

        #region 刷員工卡跳出視窗
        public IActionResult EmpCardScanInterface()
        {
            List<EmpCardScan> result = _db.employee.Where(x=>x.comFid == Loginfo.User.comFid && (x.emp_no ?? "") != "")
                .Select(x=> new EmpCardScan()
                {
                    Fid = x.FID,
                    emp_no = x.emp_no ?? "",
                    name = x.name ?? "",
                    CardNo = x.CardNo ?? "",
                })
                .ToList();            
            return View(result);
        }
        #endregion
    }
}

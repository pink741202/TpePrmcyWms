using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Front;

namespace TpePrmcyWms.Controllers.Front
{
    public class OperationController : BaseController
    {
        private readonly ILogger<OperationController> _logger;
        DBcPharmacy _db = new DBcPharmacy();

        public IActionResult Index()
        {
            ViewBag.AtCbntFid = AtCbntFid;
            List<MapMenuOnCbnt> ableFunctions = _db.MapMenuOnCbnt.Where(x => x.CbntFid == AtCbntFid && x.Able == true).ToList();
            return View(ableFunctions);
        }
        public OperationController(ILogger<OperationController> logger)
        {
            _logger = logger;

            if(Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid")) != 0) //沒kiosk
            {
                _db.RemoveRange(_db.SensorComuQuee.Where(x => x.oprState == "H").ToList());
                _db.SaveChanges();
            }
            
        }

        #region 領藥 多筆 MultiFill-DFM
        public IActionResult MultiFill()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "DFM";
            obj.TradeType = false; //減項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            return View(obj);
        }
        
        #endregion               
        #region 領藥 批次 BatchFill-DFB
        public IActionResult BatchFill()
        {
            QryBatchDrawers qbd = new QryBatchDrawers("");
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "DFB";
            obj.TradeType = false; //減項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            qbd.stockBill = obj;
            ViewBag.qryBatchDrawers = qbd; 
            return View(obj);
        }
        
        #endregion
        #region 退藥 ReturnDrug-RTD
        public IActionResult ReturnDrug()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "RTD";
            obj.TradeType = true; //加項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            return View(obj);
        }

        #endregion
        #region 住院退藥 ReturnSheet-RTS
        public IActionResult ReturnSheet()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "RTS";
            obj.TradeType = true; //加項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            return View(obj);
        }
        
        #endregion
        #region 調入調出 Trans-TG?/TI?
        public IActionResult TransGoto()
        {
            QryDrawersForTrans obj = new QryDrawersForTrans();
            obj.sBillType = "TG1";
            obj.TransGoBill = new StockBill();
            obj.TransInBill = new StockBill();
            obj.TransGoBill.CbntFid = AtCbntFid;
            obj.TransInBill.CbntFid = 0;

            obj.TransGoBill.BillType = "TG1";
            obj.TransInBill.BillType = "TI2";
            obj.TransGoBill.TradeType = false; //減項
            obj.TransInBill.TradeType = true; //加項
            obj.TransGoBill.comFid = Loginfo.User.comFid;
            obj.TransInBill.comFid = Loginfo.User.comFid;
            obj.TransGoBill.modid = Loginfo.User.Fid;
            obj.TransInBill.modid = Loginfo.User.Fid;
            obj.TransGoBill.JobDone = false;
            obj.TransInBill.JobDone = false;

            ViewBag.CbntComFid = _db.Company.Find(_db.Cabinet.Find(AtCbntFid)?.comFid)?.FID ?? 0;
            return View(obj);
        }

        public IActionResult TransInto()
        {
            QryDrawersForTrans obj = new QryDrawersForTrans();
            obj.sBillType = "TI1";
            obj.TransGoBill = new StockBill();
            obj.TransInBill = new StockBill();
            obj.TransGoBill.CbntFid = 0;
            obj.TransInBill.CbntFid = AtCbntFid;

            obj.TransGoBill.BillType = "TG2";
            obj.TransInBill.BillType = "TI1";
            obj.TransGoBill.TradeType = false; //減項
            obj.TransInBill.TradeType = true; //加項
            obj.TransGoBill.comFid = Loginfo.User.comFid;
            obj.TransInBill.comFid = Loginfo.User.comFid;
            obj.TransGoBill.modid = Loginfo.User.Fid;
            obj.TransInBill.modid = Loginfo.User.Fid;
            obj.TransGoBill.JobDone = false;
            obj.TransInBill.JobDone = false;

            ViewBag.CbntComFid = _db.Company.Find(_db.Cabinet.Find(AtCbntFid).comFid).FID;
            return View(obj);
        }

        public IActionResult TransGoList()
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            List<StockBill> q = _db.StockBill.Where(x => x.BillType == "TG2" && !x.JobDone && x.CbntFid == AtCbntFid).ToList();

            var frinfo = (from bill in _db.StockBill
                         join fr in _db.StockBill on bill.FromFid equals fr.FID
                         join frcbnt in _db.Cabinet on fr.CbntFid equals frcbnt.FID
                         join frgrid in _db.DrugGrid on fr.DrugGridFid equals frgrid.FID
                         join frdrug in _db.DrugInfo on frgrid.DrugFid equals frdrug.FID
                         join frdraw in _db.Drawers on frgrid.DrawFid equals frdraw.FID
                         where bill.JobDone == false && bill.BillType == "TG2" & bill.CbntFid == AtCbntFid
                         select new
                         {
                             key = bill.FID,
                             drugfid = frdrug.FID,
                             drugcode = frdrug.DrugCode,
                             drugname = frdrug.DrugName,
                             cbntfid = frcbnt.FID,
                             cbntname = frcbnt.CbntName,
                             drawno = frdraw.No,
                         }).ToList();


            foreach (StockBill item in q)
            {
                var info = frinfo.Where(x => x.key == item.FID).First();
                item.DrugFid = info.drugfid;
                item.DrugCode = info.drugcode;
                item.DrugName = info.drugname;
                item.RecNote = info.cbntname + " " + info.drawno + "號櫃"; //借放 來源櫃子
                item.rejectBill = info.cbntfid == item.CbntFid && _db.StockBill.Where(x => x.FromFid == item.FromFid).Count() > 1;
            }
            return View(q);
        }

        public IActionResult TransInList()
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            List<StockBill> q = _db.StockBill.Where(x => x.BillType == "TI2" && x.JobDone == false && x.CbntFid == AtCbntFid).ToList();

            var frinfo = (from bill in _db.StockBill
                         join fr in _db.StockBill on bill.FromFid equals fr.FID
                         join frcbnt in _db.Cabinet on fr.CbntFid equals frcbnt.FID
                         join frgrid in _db.DrugGrid on fr.DrugGridFid equals frgrid.FID
                         join frdrug in _db.DrugInfo on frgrid.DrugFid equals frdrug.FID
                         join frdraw in _db.Drawers on frgrid.DrawFid equals frdraw.FID
                         where bill.JobDone == false && bill.BillType == "TI2" & bill.CbntFid == AtCbntFid
                         select new
                         {
                             key = bill.FID,
                             drugfid = frdrug.FID,
                             drugcode = frdrug.DrugCode,
                             drugname = frdrug.DrugName,
                             cbntfid = frcbnt.FID,
                             cbntname = frcbnt.CbntName,
                             drawno = frdraw.No,
                         }).ToList();

            
            foreach (StockBill item in q)
            {
                var info = frinfo.Where(x => x.key == item.FID).First();
                item.DrugFid = info.drugfid;
                item.DrugCode = info.drugcode;
                item.DrugName = info.drugname;
                item.RecNote = info.cbntname + " " + info.drawno + "號櫃"; //借放 來源櫃子
                item.rejectBill = info.cbntfid == item.CbntFid && _db.StockBill.Where(x=>x.FromFid == item.FromFid).Count() > 1;
            }
            return View(q);
        }
        #endregion
        #region 盤點 STK
        public IActionResult StockTaking()
        {
            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "STK";
            obj.TradeType = true;
            obj.Qty = 0;
            obj.TargetQty = 0;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            List<Drawers> drawers = _db.Drawers.Where(x=>x.CbntFid == AtCbntFid).OrderBy(x=>x.No).ToList();
            List<DrugGrid> grids = (from grid in _db.DrugGrid
                        join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                        where grid.CbntFid == AtCbntFid && !drug.DrugCode.Contains("_") && ((drug.ReplaceTo ?? 0) == 0)
                        select new DrugGrid
                        {
                            FID = grid.FID,
                            DrugCode = drug.DrugCode,
                            DrugName = drug.DrugName,
                            Qty = grid.Qty,
                            StockTakeType = string.IsNullOrEmpty(grid.StockTakeType) ? "1" : grid.StockTakeType,
                            DrawFid = grid.DrawFid,
                            CbntFid = grid.CbntFid,
                        }).ToList();

            List<string> skding = _db.Cabinet.Find(AtCbntFid)?.StockTakeConfig_Time.Split(",").ToList() ?? new List<string>();
            DateTime period0 = DateTime.Now.Date;
            DateTime period1 = DateTime.Now.Date.AddDays(1);
            int isnow = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
            foreach(string item in skding)
            {
                string s = item.Replace(":", string.Empty);
                int comp0 = Convert.ToInt32(s.Split("~")[0]);
                int comp1 = Convert.ToInt32(s.Split("~")[1]); comp1 = comp1 == 0 ? 2400 : comp1;
                if (comp0 <= isnow && isnow < comp1)
                {
                    period1 = period0.AddHours(comp1/100).AddMinutes(comp1%100);
                    period0 = period0.AddHours(comp0/100).AddMinutes(comp0%100);
                }
            }

            List<int> taked = (from stock in _db.StockBill
                               join grid in _db.DrugGrid on stock.DrugGridFid equals grid.FID
                               where stock.CbntFid == AtCbntFid
                                   && stock.moddate != null
                                   && (DateTime)stock.moddate >= period0
                                   && (DateTime)stock.moddate < period1
                                   && stock.BillType == "STK"
                               select grid.FID)
                               .ToList();

            ViewBag.drawers = drawers; //所有的藥格
            ViewBag.grids = grids; //藥格裡的藥品
            ViewBag.stockTakedList = taked; //已盤點過的清單
            return View(obj);
        }
        #endregion
        #region 損耗登記 LRG
        public IActionResult LossRegistry()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "LRG";
            obj.TradeType = false; //減項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            return View(obj);
        }

        #endregion

        #region 借藥暫存 OSA
        public IActionResult OffsetApply()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            obj.BillType = "OSA";
            obj.TradeType = false; //減項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            ViewBag.AtCbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            return View(obj);
        }
        #endregion
        #region 沖銷歸回 OSR
        public IActionResult OffsetReturn()
        {
            StockBill_Prscpt obj = new StockBill_Prscpt();
            obj.CbntFid = HttpContext.Session.GetInt32("AtCbntFid") ?? 0;
            obj.BillType = "OSR";
            obj.TradeType = true; //減項
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;
            return View(obj);
        }
        #endregion        
        #region 空瓶領用 BXO
        public IActionResult BottleExOut()
        {
            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "BXO";
            obj.TradeType = false;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            List<DrugGrid> grids = (from grid in _db.DrugGrid
                                    join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                    where grid.CbntFid == AtCbntFid
                                        && drug.DrugCode.EndsWith("_b")
                                        && grid.Qty > 0
                                    select new DrugGrid
                                    {
                                        DrugCode = drug.DrugCode,
                                        DrugName = drug.DrugName,
                                        Qty = grid.Qty,
                                        StockTakeType = string.IsNullOrEmpty(grid.StockTakeType) ? "1" : grid.StockTakeType,
                                        DrawFid = grid.DrawFid,
                                        CbntFid = grid.CbntFid,
                                    }).ToList();

            List<Drawers> drawers = _db.Drawers
                .Where(x => x.CbntFid == AtCbntFid && grids.Select(gr => gr.DrawFid).Contains(x.FID))
                .OrderBy(x => x.No)
                .ToList();

            ViewBag.drawers = drawers; //有設定空瓶的藥格
            ViewBag.grids = grids; //藥格裡的藥品項目
            return View(obj);
        }
        #endregion
        #region 藥品調入 BXI
        public IActionResult BottleExInList()
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            List<StockBill> q = _db.StockBill
                .Where(x => x.BillType == "BXI" && x.JobDone == false && x.CbntFid == AtCbntFid)
                .ToList();

            var drawinfos = (from bill in _db.StockBill
                             join fr in _db.StockBill on bill.FromFid equals fr.FID
                             join cbnt in _db.Cabinet on fr.CbntFid equals cbnt.FID
                             join grid in _db.DrugGrid on fr.DrugGridFid equals grid.FID
                             join draw in _db.Drawers on grid.DrawFid equals draw.FID
                             where q.Select(x => x.FID).Contains(bill.FID)
                             select new
                             {
                                 key = bill.FID,
                                 drawtitle = cbnt.CbntName + " #" + draw.No
                             }).ToList();
            var druginfos = _db.DrugInfo.Where(x => q.Select(x => x.DrugCode).ToList().Contains(x.DrugCode)).ToList();

            foreach (StockBill item in q)
            {
                item.DrugFid = druginfos.Where(x => x.DrugCode == item.DrugCode).First().FID;
                item.DrugName = druginfos.Where(x => x.DrugCode == item.DrugCode).First().DrugName ?? "";
                item.RecNote = drawinfos.Where(x => x.key == item.FID).First().drawtitle; //借放 來源櫃子
            }
            return View(q);
        }
        #endregion
        #region 空瓶放入 BRT
        public IActionResult BottleReturn()
        {
            FrontendService frserv = new FrontendService(AtCbntFid, Loginfo);
            List<StockBill> q = _db.StockBill
                .Where(x => x.BillType == "BRT" && x.CbntFid == AtCbntFid && x.JobDone == false)
                .ToList();

            var drawinfos = (from bill in _db.StockBill
                             join fr in _db.StockBill on bill.FromFid equals fr.FID
                             join grid in _db.DrugGrid on fr.DrugGridFid equals grid.FID
                             join draw in _db.Drawers on grid.DrawFid equals draw.FID
                             join cbnt in _db.Cabinet on fr.CbntFid equals cbnt.FID
                             where bill.BillType == "BRT" & bill.CbntFid == AtCbntFid && bill.JobDone == false
                             select new
                             {
                                 key = bill.FID,
                                 drawtitle = cbnt.CbntName + " #" + draw.No
                             }).ToList();
            var druginfos = _db.DrugInfo.Where(x => q.Select(x => x.DrugCode).ToList().Contains(x.DrugCode)).ToList();

            foreach (StockBill item in q)
            {
                item.DrugFid = druginfos.Where(x => x.DrugCode == item.DrugCode).First().FID;
                item.DrugName = druginfos.Where(x => x.DrugCode == item.DrugCode).First().DrugName ?? "";
                item.RecNote = drawinfos.Where(x => x.key == item.FID).First().drawtitle; //借放 來源櫃子
            }
            return View(q);
        }
        #endregion

        #region 出入庫 SRI/SRO
        public IActionResult StoreRoomIn()
        {
            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.DrawFid = _db.Drawers.Where(x => x.CbntFid == AtCbntFid).FirstOrDefault()?.FID ?? 0;
            obj.BillType = "SRI";
            obj.TradeType = true; //加項
            obj.TargetQty = 0;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            var drugs = (from drug in _db.DrugInfo
                         join grid in _db.DrugGrid on drug.FID equals grid.DrugFid
                         where grid.CbntFid == AtCbntFid && !drug.DrugCode.EndsWith("_b")
                            && (drug.ReplaceTo ?? 0) == 0
                         group drug by new { drug.FID, drug.DrugCode, drug.DrugName }
                        ).ToList();

            List<DrugsForSearch> DrugList = (from ls in drugs
                               select new DrugsForSearch
                               {
                                   DrugFid = ls.Key.FID,
                                   DrugCode = ls.Key.DrugCode ?? "",
                                   DrugName = ls.Key.DrugName ?? "",
                                   BarcodeNo = "",
                               }).OrderBy(x=>x.DrugCode).ToList();

            foreach (var drug in DrugList)
            {
                drug.BarcodeNo = String.Join(",", _db.DrugPackage.Where(x => x.DrugFid == drug.DrugFid && !string.IsNullOrEmpty(x.BarcodeNo)).Select(x=>x.BarcodeNo).ToList());
            }
            ViewBag.DrugList = DrugList;
            return View(obj);
        }
        public IActionResult StoreRoomOut()
        {
            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.DrawFid = _db.Drawers.Where(x => x.CbntFid == AtCbntFid).FirstOrDefault()?.FID ?? 0;
            obj.BillType = "SRO";
            obj.TradeType = false; //加項
            obj.TargetQty = 0;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            var drugs = (from drug in _db.DrugInfo
                         join grid in _db.DrugGrid on drug.FID equals grid.DrugFid
                         where grid.CbntFid == AtCbntFid && !drug.DrugCode.EndsWith("_b")
                            && (drug.ReplaceTo ?? 0) == 0
                         group drug by new { drug.FID, drug.DrugCode, drug.DrugName }
                        ).ToList();

            ViewBag.DrugList = (from ls in drugs
                                select new
                                {
                                    DrugFid = ls.Key.FID,
                                    DrugCode = ls.Key.DrugCode ?? "",
                                    DrugName = ls.Key.DrugName ?? "",
                                    BarcodeNo = "",
                                }).OrderBy(x => x.DrugCode);

            return View(obj);
        }
        #endregion
        #region 施打疫苗 VXO VXR
        public IActionResult VaxSkd()
        {
            List<VaxSkd> data = _db.VaxSkd.Where(x => x.comFid == Loginfo.User.comFid && x.CaseClose == false)
                .OrderBy(x=>x.VaxDate)
                .ToList();

            return View(data);
        }
        public IActionResult VaxStockOut()
        {
            List<VaxSkd> data = _db.VaxSkd.Where(x => x.comFid == Loginfo.User.comFid && x.CaseClose == false)
                .OrderBy(x => x.VaxDate)
                .ToList();
            ViewBag.VaxList = data;

            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "VXO";
            obj.TradeType = false;
            obj.Qty = 0;
            obj.TargetQty = 0;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            return View(obj);
        }
        public IActionResult VaxStockReturn()
        {
            List<VaxSkd> data = _db.VaxSkd.Where(x => x.comFid == Loginfo.User.comFid && x.CaseClose == false)
                .OrderBy(x => x.VaxDate)
                .ToList();
            ViewBag.VaxList = data;

            StockBill obj = new StockBill();
            obj.CbntFid = AtCbntFid;
            obj.BillType = "VXR";
            obj.TradeType = true;
            obj.Qty = 0;
            obj.TargetQty = 0;
            obj.comFid = Loginfo.User.comFid;
            obj.modid = Loginfo.User.Fid;
            obj.JobDone = false;

            return View(obj);
        }
        #endregion

        #region 美沙冬領藥 MethadonTake-MSDT
        public IActionResult MethadonTake()
        {
            DrugInfo? MSDdrug = _db.DrugInfo.Find(3452);
            MethadonBill? msdBill_last = _db.MethadonBill
                .Where(x => x.adddate < DateTime.Now.Date)
                .OrderByDescending(x => x.adddate)
                .FirstOrDefault();
            MethadonBill? msdBill_today = _db.MethadonBill
                .Where(x => x.RecordDate == DateTime.Now.Date)
                .FirstOrDefault();

            StockBill_MSD obj = new StockBill_MSD()
            {
                CbntFid = AtCbntFid,
                DrugFid = MSDdrug.FID,
                DrugCode = MSDdrug.DrugCode,
                DrugName = MSDdrug.DrugName,
                BillType = "MSDT",
                TradeType = false, //減項           
                comFid = Loginfo.User.comFid,
                modid = Loginfo.User.Fid,
                JobDone = false,

                Last_CC = Math.Round(msdBill_last?.RetnCC ?? 0, 0),
                Last_Weight = Math.Round(msdBill_last?.RetnWeight ?? 0, 0),
                This_Weight = Math.Round(msdBill_today?.TakeWeight ?? 0, 0),
                This_CC = Math.Round(msdBill_today?.TakeCC ?? 0, 0),
                AddEmpName = Loginfo.User.Name,
                BottleWegiht = _db.DrugPackage.Where(x => x.DrugFid == MSDdrug.FID).FirstOrDefault()?.PackageWeight - 50 ?? 400,
            };

            return View(obj);
        }
        #endregion

        #region 美沙冬還藥 MethadonReturn-MSDR
        public IActionResult MethadonReturn()
        {
            DrugInfo? MSDdrug = _db.DrugInfo.Find(3452);
            MethadonBill? msdBill_today = _db.MethadonBill
                .Where(x => x.RecordDate == DateTime.Now.Date)
                .FirstOrDefault();

            StockBill_MSD obj = new StockBill_MSD()
            {
                CbntFid = AtCbntFid,
                DrugFid = MSDdrug.FID,
                DrugCode = MSDdrug.DrugCode,
                DrugName = MSDdrug.DrugName,
                BillType = "MSDR",
                TradeType = true, //加項
                comFid = Loginfo.User.comFid,
                modid = Loginfo.User.Fid,
                JobDone = false,

                Last_CC = Math.Round(msdBill_today?.TakeCC ?? 0, 0) ,
                Last_Weight = Math.Round(msdBill_today?.TakeWeight ?? 0, 0),
                This_Weight = Math.Round(msdBill_today?.RetnWeight ?? 0, 0),
                This_CC = Math.Round(msdBill_today?.RetnCC ?? 0, 0),
                AddEmpName = Loginfo.User.Name,
                RecordDate = DateTime.Now.Date,
                BottleWegiht = _db.DrugPackage.Where(x => x.DrugFid == MSDdrug.FID).FirstOrDefault()?.PackageWeight - 50 ?? 400,
            };

            return View(obj);
        }

        #endregion
    }
}
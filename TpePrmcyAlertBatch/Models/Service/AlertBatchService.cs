using System;
using TpePrmcyAlertBatch.Models.DOM;
using TpePrmcyAlertBatch.Models.Unit;
using ShareLibrary.Models.Unit;
using ShareLibrary.Models.Service;
using TpePrmcyWms.Models.DOM;
using ShareLibrary.Models.HsptlApiUnit;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace TpePrmcyAlertBatch.Models.Service
{
    internal class AlertBatchService
    {
        DBcPharmacy _db = new DBcPharmacy();
        List<UserEmail> empEmail = new List<UserEmail>();        

        public AlertBatchService(List<DateTime> FlagTimes, List<bool> ifRun) 
        {
            //ifRun 0:each Day 1:each Hour 
            #region 預備資料
            try
            {
                empEmail = (from emp in _db.employee
                                            join auth in _db.UserCbntFnAuth
                                            on emp.FID equals auth.EmpFid
                                            where auth.CbntFid > 0 && !string.IsNullOrEmpty(emp.email)
                                            group new { emp, auth } by new { emp.FID, emp.email, emp.name, auth.CbntFid } into g
                                            select new UserEmail
                                            {
                                                FID = g.Key.FID,
                                                Email = g.Key.email,
                                                Name = g.Key.name,
                                                CbntFid = g.Key.CbntFid
                                            }).ToList();
            }
            catch (Exception ex) { qwFunc.logshow($"執行:取得人員email 出錯:{ex.Message}"); return; }
            #endregion

            #region SensorDevice (櫃門故障)
            if (ifRun[1])
            {
                try
                {
                    string tableName = "SensorDevice";
                    int alertType = (int)AlertTypeClass.DoorOpend;

                    List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                    List<SensorDevice> needAlert = _db.SensorDevice
                        .Where(x => !string.IsNullOrEmpty(x.NotWork) && !alerted.Contains(x.FID))
                        .ToList();
                    foreach (var alert in needAlert)
                    {
                        try
                        {
                            string content = "";
                            if (alert.TargetTable == "Drawer")
                            {
                                var info = (from draw in _db.Drawers
                                            join cbnt in _db.Cabinet on draw.CbntFid equals cbnt.FID
                                            where draw.FID == alert.TargetObjFid
                                            select new { draw.No, cbnt.CbntName })
                                            .First();
                                content = $"櫃名：{info.CbntName} {info.No}號抽屜 櫃門偵測到已故障，請通知維修人員。";
                            }


                            AlertNotification adding = new AlertNotification()
                            .create(alertType, tableName, alert.FID,
                            string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                            "櫃門故障",
                            content);
                            _db.AlertNotification.Add(adding);
                            _db.SaveChanges();
                        }
                        catch (Exception ex) { qwFunc.logshow($"執行Drawer#{alert.FID}:櫃子壞掉(迴圈) 出錯:{ex.Message}"); }

                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:櫃子壞掉 出錯:{ex.Message}"); }
            }
            


            #endregion

            #region Drawers (櫃門未關)     
            try
            {
                string tableName = "Drawers";
                int alertType = (int)AlertTypeClass.DoorOpend;

                List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                List<Drawers> needAlert = _db.Drawers
                    .Where(x => x.DoorStatus == true && !alerted.Contains(x.FID))
                    .ToList();
                foreach (var alert in needAlert)
                {
                    try
                    {
                        var info = (from cbnt in _db.Cabinet
                                    where cbnt.FID == alert.CbntFid
                                    select new { cbnt.CbntName })
                                        .First();
                        string content = $"櫃名：{info.CbntName} {alert.No}號抽屜 櫃門偵測到未關閉，請派人員關門。";

                        AlertNotification adding = new AlertNotification()
                        .create(alertType, tableName, alert.FID,
                        string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                        "櫃門未關",
                        content);
                        _db.AlertNotification.Add(adding);
                        _db.SaveChanges();
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行Drawers#{alert.FID}:門沒關(迴圈) 出錯:{ex.Message}"); }
                }
            }
            catch (Exception ex) { qwFunc.logshow($"執行:門沒關 出錯:{ex.Message}"); }



            #endregion

            #region DrugGrid (數量超過藥格上限,低於安全庫存,近效期+DrugGridBatchNo)
            if (ifRun[1])
            {
                try
                {
                    string tableName = "DrugGrid";
                    int alertType = (int)AlertTypeClass.QtyOverGrid;

                    List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                    List<DrugGrid> needAlert = _db.DrugGrid
                        .Where(x => x.Qty > x.MaxLimitQty && x.MaxLimitQty > 0 && !alerted.Contains(x.FID))
                        .ToList();
                    foreach (var alert in needAlert)
                    {
                        try
                        {
                            var info = (from grid in _db.DrugGrid
                                        join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                        join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                        join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                        where grid.FID == alert.FID
                                        select new { draw.No, drug.DrugName, cbnt.CbntName })
                                            .First();
                            string content = $"櫃名：{info.CbntName} {info.No}號抽屜 藥名:{info.DrugName}，數量：{alert.Qty}，超過藥格設定的上限：{alert.MaxLimitQty}";

                            AlertNotification adding = new AlertNotification()
                            .create(alertType, tableName, alert.FID,
                            string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                            "數量超過藥格上限",
                            content);
                            _db.AlertNotification.Add(adding);
                            _db.SaveChanges();
                        }
                        catch (Exception ex) { qwFunc.logshow($"執行DrugGrid#{alert.FID}:數量超過藥格上限 出錯:{ex.Message}"); }
                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:數量超過藥格上限 出錯:{ex.Message}"); }

                try
                {
                    string tableName = "DrugGrid";
                    int alertType = (int)AlertTypeClass.QtyOverGrid;

                    List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                    List<DrugGrid> needAlert = _db.DrugGrid
                        .Where(x => x.Qty < x.SafetyStock && x.SafetyStock > 0 && !alerted.Contains(x.FID))
                        .ToList();
                    foreach (var alert in needAlert)
                    {
                        try
                        {
                            var info = (from grid in _db.DrugGrid
                                        join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                        join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                        join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                        where grid.FID == alert.FID
                                        select new { draw.No, drug.DrugName, cbnt.CbntName })
                                            .First();
                            string content = $"櫃名：{info.CbntName} {info.No}號抽屜 藥名:{info.DrugName}，數量：{alert.Qty}，低於藥格設定的安全庫存：{alert.SafetyStock}";

                            AlertNotification adding = new AlertNotification()
                            .create(alertType, tableName, alert.FID,
                            string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                            "低於安全庫存",
                            content);

                            _db.AlertNotification.Add(adding);
                            _db.SaveChanges();
                        }
                        catch (Exception ex) { qwFunc.logshow($"執行DrugGrid#{alert.FID}:低於安全庫存(迴圈) 出錯:{ex.Message}"); }


                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:低於安全庫存 出錯:{ex.Message}"); }
            }

            if (ifRun[0])
            {
                try
                {
                    string tableName = "DrugGridBatchNo";
                    int alertType = (int)AlertTypeClass.NearExpiryAlert;

                    var needAlert = (from grid in _db.DrugGrid
                                     join bach in _db.DrugGridBatchNo on grid.FID equals bach.GridFid
                                     where bach.Qty > 0 && (grid.NearExpiryAlert ?? 0) > 0
                                     && DateTime.Now.AddMonths((grid.NearExpiryAlert ?? 0)).Date > bach.ExpireDate.Date
                                     select new { bach.GID, bach.BatchNo, bach.ExpireDate, bach.Qty, grid.CbntFid, grid.FID }
                                               ).ToList();

                    List<AlertNotification> alerted = _db.AlertNotification //已通知未處理,要去掉
                    .Where(x => x.FixedTime == null && x.AlertType == alertType && x.SourceTable == tableName)
                    .ToList();

                    foreach (var alert in needAlert)
                    {
                        try
                        {
                            bool goSend = false;
                            string alertmsg = "";
                            //近效期,每月一次
                            DateTime firstday = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date;
                            if (alert.ExpireDate.Date >= DateTime.Now.Date)
                            {
                                goSend = alerted.Where(x => x.SourceGid == alert.GID && x.adddate >= firstday).Count() == 0;
                                alertmsg = "請留意藥品即將到期";
                            }
                            //過期,每天一次
                            if (alert.ExpireDate.Date < DateTime.Now.Date)
                            {
                                goSend = alerted.Where(x => x.SourceGid == alert.GID && x.adddate >= DateTime.Now.Date).Count() == 0;
                                alertmsg = "請注意藥品過期，請儘快處理";
                            }

                            if (goSend)
                            {
                                var info = (from grid in _db.DrugGrid
                                            join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                            join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                            join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                            where grid.FID == alert.FID
                                            select new { draw.No, drug.DrugName, cbnt.CbntName })
                                            .First();
                                string content = $"櫃名：{info.CbntName} {info.No}號抽屜 藥名:{info.DrugName}，批號：{alert.BatchNo}，效期為：{alert.ExpireDate.ToString("yyyy-MM-dd")}，{alertmsg}。";

                                AlertNotification adding = new AlertNotification()
                                .create(alertType, tableName, 0,
                                string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                                "近效期",
                                content);
                                adding.SourceGid = alert.GID;
                                _db.AlertNotification.Add(adding);
                                _db.SaveChanges();
                            }
                        }
                        catch (Exception ex) { qwFunc.logshow($"執行DrugGridBatchNo#{alert.GID}:近效期(迴圈) 出錯:{ex.Message}"); }

                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:近效期 出錯:{ex.Message}"); }
            }

            #endregion

            #region StockBill (盤點錯誤,盤點時段沒盤+Cabinet,在途庫存超時+Cabinet)
            try
            {
                string tableName = "StockBill";
                int alertType = (int)AlertTypeClass.StockTakeError;

                List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                List<StockBill> needAlert = _db.StockBill
                    .Where(x => x.SysChkQty != (x.UserChk2Qty ?? x.SysChkQty) && (x.UserChk2Qty ?? 0) > 0 && !alerted.Contains(x.FID))
                    .ToList();
                foreach (var alert in needAlert)
                {
                    try
                    {
                        var info = (from grid in _db.DrugGrid
                                    join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                    join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                    join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                    where grid.FID == alert.DrugGridFid
                                    select new { draw.No, drug.DrugName, cbnt.CbntName })
                                        .First();                        

                        string content = $"櫃名：{info.CbntName} {info.No}號抽屜 藥名:{info.DrugName}  <br />" +
                            $"原本庫存：{(alert.Qty * (alert.TradeType ? -1 : 1)) + alert.SysChkQty}，異動數量：{(alert.TradeType ? "入" : "出")}{alert.Qty}，系統結存數量：{alert.SysChkQty} <br />" +
                            $"第一次盲盤數量：{alert.UserChk1Qty}，第二次盲盤數量：{alert.UserChk2Qty}。";


                        AlertNotification adding = new AlertNotification()
                        .create(alertType, tableName, alert.FID,
                        string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                        "庫存異動後的盤點錯誤",
                        content);

                        _db.AlertNotification.Add(adding);
                        _db.SaveChanges();
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行StockBill#{alert.FID}:盤點錯誤(迴圈) 出錯:{ex.Message}"); }

                }
            }
            catch (Exception ex) { qwFunc.logshow($"執行:盤點錯誤 出錯:{ex.Message}"); }

            if (ifRun[1])
            {
                try
                {
                    string tableName = "DrugGrid";
                    int alertType = (int)AlertTypeClass.NoStockTaked;

                    List<Cabinet> cbnts = _db.Cabinet
                        .Where(x => (x.StockTakeConfig_Day ?? "").Contains(((int)FlagTimes[0].DayOfWeek).ToString()))
                        .ToList();
                    foreach (var cbnt in cbnts)
                    {
                        //找時段
                        List<int> periods = cbnt.StockTakeConfig_Time?.Replace("~", ",").Replace(":", "").Split(",").Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();
                        int nowtime = Convert.ToInt32(FlagTimes[0].ToString("HHmm"));
                        int indexat = 0; //若是0,就是前一天最後一個時段
                        for (int i = 1; i < periods.Count(); i += 2) { if (periods[i] < nowtime) { indexat = i; break; } }
                        string StartHHmm = indexat == 0 ? periods[periods.Count() - 2].ToString("0000") : periods[indexat - 1].ToString("0000");
                        string EndHHmm = indexat == 0 ? periods[periods.Count() - 1].ToString("0000") : periods[indexat].ToString("0000");
                        DateTime start = Convert.ToDateTime(FlagTimes[0].AddDays((indexat == 0 ? -1 : 0)).ToString("yyyy/MM/dd " + StartHHmm.Substring(0, 2) + ":" + StartHHmm.Substring(2, 2)));
                        DateTime end = Convert.ToDateTime(FlagTimes[0].AddDays((indexat == 0 ? -1 : 0)).ToString("yyyy/MM/dd " + EndHHmm.Substring(0, 2) + ":" + EndHHmm.Substring(2, 2)));

                        //找沒有盤點藥格
                        bool stocktaked = false;
                        List<int> gridFids = _db.DrugGrid.Where(x => x.CbntFid == cbnt.FID).Select(x => x.FID).ToList();
                        List<int> gridTaked = _db.StockBill
                            .Where(x => x.BillType == "STK" && x.JobDone && x.UserChk1Qty != null && x.moddate > start && x.moddate < end)
                            .Select(x => x.DrugGridFid)
                            .ToList();
                        List<int> gridNotTaked = gridFids.Except(gridTaked).ToList();

                        //找沒盤點的 是否已通知過
                        List<int> alerted = _db.AlertNotification
                            .Where(x => x.AlertType == alertType && x.SourceTable == tableName && x.SourceNote == $"{StartHHmm}~{EndHHmm}")
                            .Select(x => x.SourceFid ?? 0)
                            .ToList();
                        List<int> gridNotAlert = gridNotTaked.Except(alerted).ToList();

                        //通知
                        foreach (int gridfid in gridNotAlert)
                        {
                            try
                            {
                                var info = (from grid in _db.DrugGrid
                                            join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                            join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                            where grid.FID == gridfid
                                            select new { draw.No, drug.DrugName })
                                            .First();
                                string content = $"櫃名：{cbnt.CbntName} {info.No}號抽屜 藥名:{info.DrugName}，" +
                                    $"應盤點時段{StartHHmm}~{EndHHmm}，未有盤點紀錄。";

                                AlertNotification adding = new AlertNotification()
                                .create(alertType, tableName, gridfid,
                                string.Join(";", empEmail.Where(x => x.CbntFid == cbnt.FID).Select(x => x.Email)),
                                "盤點時段未有盤點紀錄",
                                content);
                                adding.SourceNote = $"{StartHHmm}~{EndHHmm}";
                                _db.AlertNotification.Add(adding);
                                _db.SaveChanges();
                            }
                            catch (Exception ex) { qwFunc.logshow($"執行DrugGrid#{gridfid}:盤點時段沒盤(迴圈) 出錯:{ex.Message}"); }
                        }
                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:盤點時段沒盤 出錯:{ex.Message}"); }
            }

            try
            {
                string tableName = "StockBill";
                int alertType = (int)AlertTypeClass.InTransitExpired;

                List<int> alerted = getAlertedButNotFixYet(alertType, tableName);
                List<StockBill> needDelete = (from bill in _db.StockBill
                                              join frmb in _db.StockBill on bill.FromFid equals frmb.FID
                                              join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                                              where bill.CbntFid != frmb.CbntFid
                                              && (cbnt.InTransitExpiry ?? 0) > 0 //沒設定,略過
                                              && (((DateTime)frmb.moddate).AddMinutes(cbnt.InTransitExpiry ?? 1440) < DateTime.Now)
                                              select bill).ToList();
                List<int> needAlertFid = needDelete.GroupBy(x=>x.FromFid).Select(x=>x.Key.Value).ToList();
                List<StockBill> needAlert = _db.StockBill.Where(x => needAlertFid.Contains(x.FID)).ToList();
                

                foreach (var alert in needAlert)
                {
                    try
                    {
                        List<StockBill> target = needDelete.Where(x => x.FromFid == alert.FID).ToList();
                        string toTarget = target.Count() > 1 ? "全院區" : _db.Cabinet.Where(x => x.FID == target[0].CbntFid).First().CbntName;

                        var info = (from grid in _db.DrugGrid
                                    join cbnt in _db.Cabinet on grid.CbntFid equals cbnt.FID
                                    join draw in _db.Drawers on grid.DrawFid equals draw.FID
                                    join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                                    where grid.FID == alert.DrugGridFid
                                    select new { draw.No, drug.DrugName, cbnt.CbntName, cbnt.InTransitExpiry })
                                            .First();

                        string content = $"櫃名：{info.CbntName} {info.No}號抽屜 藥名:{info.DrugName} <br />" +
                            $"調出內容：藥名:{info.DrugName}，數量：{alert.TargetQty} <br />" +
                            $"調出對象：{toTarget} <br />" +
                            $"已調入數量：{alert.TargetQty - target[0].TargetQty} <br />" +
                            $"剩餘數量：{target[0].TargetQty} <br />" +
                            $"剩餘數量已超過設定的{info.InTransitExpiry}分鐘未處理，系統已將剩餘數量取消，請人員至原櫃操作，將剩餘數量取消，若還需要調出調入，請在取消異動完成後，再重新申請。";

                        AlertNotification adding = new AlertNotification()
                            .create(alertType, tableName, alert.FID,
                            string.Join(";", empEmail.Where(x => x.CbntFid == alert.CbntFid).Select(x => x.Email)),
                            "在途庫存超時",
                            content);
                        _db.AlertNotification.Add(adding);
                        _db.SaveChanges();
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行StockBill#{alert.FID}:在途庫存超時(迴圈) 出錯:{ex.Message}"); }
                }
                _db.StockBill.RemoveRange(needDelete);
                _db.SaveChanges();

            }
            catch (Exception ex) { qwFunc.logshow($"執行:在途庫存超時 出錯:{ex.Message}"); }

            #endregion

            #region PrscptBill (api連線狀況,贈藥臨採)
            try
            {
                List<AlertNotification> ReSendApi = _db.AlertNotification
                        .Where(x => x.AlertType == (int)AlertTypeClass.HisConnectFailed && x.FixedTime == null).ToList();
                foreach (AlertNotification item in ReSendApi)
                {
                    try
                    {
                        PrscptBill? prcspt = _db.PrscptBill.Find(item.SourceFid);
                        if (prcspt == null) {
                            item.SourceNote = "find null";
                            item.FixedTime = DateTime.Now;
                            item.moddate = DateTime.Now;
                            _db.AlertNotification.Update(item);
                            _db.SaveChanges(true);
                            continue;
                        }
                        HsptlApiService hsptlServ = new HsptlApiService();
                        Qry_OutStorage apiQ = new Qry_OutStorage(prcspt);
                        bool? chkresult = hsptlServ.getOutStorage(apiQ);
                        if (chkresult.HasValue) {
                            item.SendTime = (chkresult ?? false) ? item.SendTime : null;
                            item.FixedTime = (chkresult ?? false) ? DateTime.Now : null;
                            item.AlertType = (int)AlertTypeClass.HisConnectFailed;
                            item.moddate = DateTime.Now;
                            _db.AlertNotification.Update(item);
                            _db.SaveChanges(true);
                        }
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行AlertNotification#{item.FID}:api連線狀況(迴圈:再送api) 出錯:{ex.Message}"); }

                }

                string tableName = "PrscptBill";
                List<int> alertTypes = new List<int> { (int)AlertTypeClass.HisReturnFalse, (int)AlertTypeClass.HisConnectFailed };

                List<AlertNotification> needAlert = _db.AlertNotification
                    .Where(x => x.SendTime == null && alertTypes.Contains(x.AlertType) && string.IsNullOrEmpty(x.AlertContent)) 
                    .ToList();
                foreach (var alert in needAlert)
                {
                    try
                    {
                        bool? retest = null;
                        
                        var info = (from prscpt in _db.PrscptBill
                                    join drug in _db.DrugInfo on prscpt.DrugCode equals drug.DrugCode
                                    where prscpt.FID == alert.SourceFid
                                    select new { drug.DrugName, prscpt.PatientNo, prscpt.PrscptNo, prscpt.TtlQty, prscpt.PrscptDate })
                                        .First();
                        string content = $"藥單資訊： <br />" +
                        $"藥名：{info.DrugName} <br />" +
                        $"領藥號：{info.PrscptNo}，病歷號：{info.PatientNo}，數量：{info.TtlQty}，日期：{Convert.ToDateTime(info.PrscptDate).ToString("yyyy-MM-dd")} <br />" +
                        $"此筆{alert.AlertTitle}。";

                        int cbntfid = Convert.ToInt16(alert.SendTo);
                        alert.SendTo = string.Join(";", empEmail.Where(x => x.CbntFid == cbntfid).Select(x => x.Email));
                        alert.AlertContent = content;

                        _db.AlertNotification.Update(alert);
                        _db.SaveChanges();
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行PrscptBill#{alert.FID}:api連線狀況(迴圈) 出錯:{ex.Message}"); }

                }
            }
            catch (Exception ex) { qwFunc.logshow($"執行:api連線狀況 出錯:{ex.Message}"); }

            if (ifRun[1])
            {
                try
                {
                    string tableName = "PrscptBill";
                    List<int> alertTypes = new List<int> { (int)AlertTypeClass.FreeTrialNotEnough, (int)AlertTypeClass.FreeTrialNotOnList, (int)AlertTypeClass.AdHocProcNotOnList };

                    List<AlertNotification> needAlert = _db.AlertNotification
                        .Where(x => x.SendTime == null && alertTypes.Contains(x.AlertType) && string.IsNullOrEmpty(x.AlertContent))
                        .ToList();
                    foreach (var alert in needAlert)
                    {
                        try
                        {
                            var info = (from prscpt in _db.PrscptBill
                                        join drug in _db.DrugInfo on prscpt.DrugCode equals drug.DrugCode
                                        where prscpt.FID == alert.SourceFid
                                        select new { drug.DrugName, prscpt.PatientNo, prscpt.PrscptNo, prscpt.TtlQty, prscpt.PrscptDate })
                                            .First();
                            string content = $"藥單資訊： <br />" +
                            $"藥名：{info.DrugName} <br />" +
                            $"領藥號：{info.PrscptNo}，病歷號：{info.PatientNo}，數量：{info.TtlQty}，日期：{Convert.ToDateTime(info.PrscptDate).ToString("yyyy-MM-dd")} <br />" +
                            $"此{alert.AlertTitle}。";

                            int cbntfid = Convert.ToInt16(alert.SendTo);
                            alert.SendTo = string.Join(";", empEmail.Where(x => x.CbntFid == cbntfid).Select(x => x.Email));
                            alert.AlertContent = content;

                            _db.AlertNotification.Update(alert);
                            _db.SaveChanges();
                        }
                        catch (Exception ex) { qwFunc.logshow($"執行PrscptBill#{alert.FID}:贈藥臨採(迴圈) 出錯:{ex.Message}"); }

                    }
                }
                catch (Exception ex) { qwFunc.logshow($"執行:贈藥臨採 出錯:{ex.Message}"); }
            }
            
            #endregion

            #region 發信
            try
            {
                List<AlertNotification> alerts = _db.AlertNotification.Where(x => x.SendTime == null).ToList();
                foreach (AlertNotification alert in alerts)
                {
                    MailService mail = new MailService();
                    bool sendresult = mail.Sending(alert.SendTo, alert.AlertTitle, alert.AlertContent);
                    if (sendresult)
                    {
                        alert.SendTime = DateTime.Now;
                        _db.AlertNotification.Update(alert);
                        _db.SaveChanges();
                    }
                    
                }

            }
            catch (Exception ex) { qwFunc.logshow($"執行:發信 出錯:{ex.Message}"); }

            #endregion

        }

        private List<int> getAlertedButNotFixYet(int alertType, string tableName)
        {
            return _db.AlertNotification //已通知未處理,要去掉
                .Where(x => x.FixedTime == null && x.AlertType == alertType && x.SourceTable == tableName)
                .Select(x => x.SourceFid ?? 0).ToList();
        }

    }

    internal class UserEmail
    {
        public int FID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int CbntFid { get; set; }       

    }
}

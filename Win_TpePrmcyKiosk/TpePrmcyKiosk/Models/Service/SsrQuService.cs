using Dapper;
using Microsoft.Data.SqlClient;
using System.Configuration;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.DOM;
using TpePrmcyKiosk.Models.Unit;
using ShareLibrary.Models.Service;

namespace TpePrmcyKiosk.Models.Service
{
    public delegate void SsrQuUpdateHandler(object sender, QuTaskArgs e);


    public class QuTaskArgs : EventArgs
    {
        public SensorComuQuee QuObj = new SensorComuQuee();

    }

    internal class SsrQuService
    {
        private MbusExcServ mbus = new MbusExcServ();
        int AtCbntFid = Convert.ToInt32(ConfigurationManager.AppSettings["AtCbntFid"]);
        private List<string> LedColors = ConfigurationManager.AppSettings["LedColors"].ToString().Split(',').ToList();
        private List<SensorDeviceCtrl> ssrList = new List<SensorDeviceCtrl>();

        public event SsrQuUpdateHandler ssrQuUpdateEvent = null;
        QuTaskArgs args = new QuTaskArgs();
        private List<string> CmdList = new List<string>();
        public bool ActLoop = false; //是否持繼取得數據


        #region loop
        public async void RunLoop() //取得數據迴圈
        {
            ActLoop = true;
            bool doInvoke = false; //該次foreach是否輸出

            while (ActLoop)
            {
                try
                {
                    /*說明:
                     * oprState 0 等led   1 有led   2 完成   D 刪除   H 隱藏
                     * ssrState "" 未加入佇  0 等開門  1 已開門  2 已關門
                     * 
                     * 開關門locker ==================================================

                        第一個
                        01 05 00 00 FF 00 8C 3A 開
                        01 05 00 00 00 00 CD CA 關

                        第二個
                        01 05 00 01 FF 00 DD FA 開
                        01 05 00 01 00 00 9C 0A 關

                       check門 ========================================
                        第一個 01 02 34 00 00 02
                        第二個 01 02 34 01 00 02 
                        3400x = 13312
                     */

                    DBcPharmacy db = new DBcPharmacy();
                    List<SensorComuQuee> qus = db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid).ToList();
                    string cmdstring = "";
                    for (int i = 0; i < qus.Count; i++)
                    {
                        bool UpdateData = false; //有更新,在最後統一存檔
                        string state = qus[i].ssrState + qus[i].oprState;
                        //給LED
                        if (qus[i].LEDColor == "")
                        {
                            foreach (string color in LedColors)
                            {
                                if (!qus.Select(s => s.LEDColor).Contains(color)) { qus[i].LEDColor = color; UpdateData = true; break; }
                            }
                        }

                        //新增感應器
                        if (qus[i].ssrState == "0" && qus[i].oprState == "0" && qus[i].LEDColor != "" && (qus[i].DrawFid ?? 0) != 0 )
                        {
                            qus[i].DrugGridFid = getDrugGridFid((qus[i].DrawFid ?? 0), qus[i].DrugCode);
                            ssrList.AddRange(GetSensors(qus[i].DrawFid ?? 0, qus[i].DrugGridFid ?? 0));
                            qus[i].ssrState = "0";
                            UpdateData = true;
                        }

                        #region MODBUS通訊控制
                        if (state != "" && (qus[i].DrawFid ?? 0) != 0 && qus[i].LEDColor != "")
                        {
                            #region 開LED 00
                            if (qus[i].ssrState == "0" || qus[i].oprState == "0")
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList
                                    .Where(x => x.SensorType == "LED" && x.TargetTable == "Drawer" && x.TargetObjFid == qus[i].DrawFid )
                                    .ToList();
                                string color = qus[i].LEDColor;
                                foreach (SensorDeviceCtrl ssr in ssrs)
                                {
                                    if (!string.IsNullOrEmpty(ssr.NotWork)) { continue; } //壞掉了
                                    try
                                    {
                                        if (setComport(ssr.SerialPort))
                                        {
                                            cmdstring = ssr.genLitCmd(qus[i].LEDColor, true);
                                            string resultcmd = mbus.ExCmd(cmdstring);
                                            if (resultcmd.StartsWith("ERR"))
                                            {
                                                ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                                                qwFunc.savelog($"執行#{ssr.FID}指令{cmdstring}失敗:{resultcmd}");
                                            }
                                        }
                                        else
                                        {
                                            UpdateNotWorking(ssr.FID, $"ERR0:{ssr.SerialPort}失敗");
                                            qwFunc.savelog($"設定comport:{ssr.SerialPort}失敗");
                                        }
                                    }
                                    catch (Exception ex) { qwFunc.savelog($"開LED錯誤#{ssr.FID}：{ex.Message}"); }
                                }
                            }
                            #endregion
                            #region 開門 0||0
                            if ((qus[i].ssrState == "0" || qus[i].oprState == "0") && qus[i].oprState != "2")
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "LOCK" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                foreach (SensorDeviceCtrl ssr in ssrs)
                                {
                                    if (!string.IsNullOrEmpty(ssr.NotWork)) { continue; } //壞掉了
                                    try
                                    {
                                        if (setComport(ssr.SerialPort))
                                        {
                                            cmdstring = ssr.genLockCmd(true);
                                            string resultcmd = mbus.ExCmd(cmdstring);
                                            if (resultcmd.StartsWith("ERR"))
                                            {
                                                ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                                                qwFunc.savelog($"執行#{ssr.FID}指令{cmdstring}失敗:{resultcmd}");
                                            }
                                        }
                                        else
                                        {
                                            UpdateNotWorking(ssr.FID, $"ERR0:{ssr.SerialPort}失敗");
                                            qwFunc.savelog($"設定comport:{ssr.SerialPort}失敗");
                                        }
                                    }
                                    catch (Exception ex) { qwFunc.savelog($"開門錯誤#{ssr.FID}：{ex.Message}"); }
                                }
                            }
                            #endregion
                            #region 關門 ?2
                            if (qus[i].oprState == "2")
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "LOCK" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                foreach (SensorDeviceCtrl ssr in ssrs)
                                {
                                    if (!string.IsNullOrEmpty(ssr.NotWork)) { continue; } //壞掉了
                                    try
                                    {
                                        if (setComport(ssr.SerialPort))
                                        {
                                            cmdstring = ssr.genLockCmd(false);
                                            string resultcmd = mbus.ExCmd(cmdstring);
                                            if (resultcmd.StartsWith("ERR"))
                                            {
                                                ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                                                qwFunc.savelog($"執行#{ssr.FID}指令{cmdstring}失敗:{resultcmd}");
                                            }
                                        }
                                        else
                                        {
                                            UpdateNotWorking(ssr.FID, $"ERR0:{ssr.SerialPort}失敗");
                                            qwFunc.savelog($"設定comport:{ssr.SerialPort}失敗");
                                        }
                                    }
                                    catch (Exception ex) { qwFunc.savelog($"關門錯誤#{ssr.FID}：{ex.Message}"); }
                                }
                            }
                            #endregion
                            #region 關LED 21,22
                            if (qus[i].ssrState == "2")
                            {
                                //LED
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "LOCK" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                string color = qus[i].LEDColor;
                                foreach (SensorDeviceCtrl ssr in ssrs)
                                {
                                    if (!string.IsNullOrEmpty(ssr.NotWork)) { continue; } //壞掉了
                                    try
                                    {
                                        if (setComport(ssr.SerialPort))
                                        {
                                            cmdstring = ssr.genLitCmd(qus[i].LEDColor, false);
                                            string resultcmd = mbus.ExCmd(cmdstring);
                                            if (resultcmd.StartsWith("ERR"))
                                            {
                                                ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                                                qwFunc.savelog($"執行#{ssr.FID}指令{cmdstring}失敗:{resultcmd}");
                                            }
                                        }
                                        else
                                        {
                                            UpdateNotWorking(ssr.FID, $"ERR0:{ssr.SerialPort}失敗");
                                            qwFunc.savelog($"設定comport:{ssr.SerialPort}失敗");
                                        }
                                    }
                                    catch (Exception ex) { qwFunc.savelog($"關LED錯誤#{ssr.FID}：{ex.Message}"); }                                    
                                }
                            }
                            #endregion
                            #region 取磅秤值 00,01,11
                            if (!state.Contains("2"))
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "SCALE" && x.TargetObjFid == qus[i].DrugGridFid).ToList();
                                try
                                {                                    
                                    foreach (SensorDeviceCtrl ssr in ssrs)
                                    {
                                        if (ssrs.Select(x=>!string.IsNullOrEmpty(x.NotWork)).Count() > 0) { break; } //有一個壞掉,就不準了

                                        if (!(ssr.Modbus_Addr > 0)) { continue; }
                                        //清零 若失敗再一次共三次
                                        if (qus[i].ssrState == "00")
                                        {
                                            cmdstring = ssr.genScaleCmd("clean");
                                            for (int t = 0; t < 3; t++) { if (mbus.ExCmd(cmdstring).Split(' ').Length == 8) { break; } else { Thread.Sleep(10); } }
                                        }

                                        cmdstring = ssr.genScaleCmd("data");
                                        string resultcmd = mbus.ExCmd(cmdstring);
                                        if (!resultcmd.StartsWith("ERR")) //若是取得磅秤數據
                                        {
                                            string[] hexarr = resultcmd.Split(' '); //送出並取得回傳結果
                                            string hexvalue = "";
                                            for (int t = 3; t < mbus.HexToInt(hexarr[2]) + 3; t++) { hexvalue += hexarr[t]; } //取hex值
                                            decimal weight = Math.Abs(mbus.HexToPrecision00Decimal(hexvalue)); //轉成重量
                                            if (qus[i].ssrState == "00") { ssr.WeighWeight0 = weight; } //第一次,或有問題
                                            else
                                            { //非第一次,換算數量
                                                decimal LostWeight = weight - ssr.WeighWeight0 ?? 0;
                                                mbus.CalculateWeightToQty(-LostWeight, ssr.UnitWeight, ssr.UnitQty,
                                                    out decimal reQty, out decimal reTolrn, out bool reTrust, out string errmsg_toqty);
                                                ssr.WeighWeight = weight;
                                                ssr.WeighQty = reQty;
                                            }
                                        }
                                        else
                                        {
                                            ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                                        }
                                    }
                                    qus[i].ssrQty =
                                        (ssrs.Count > 0 && ssrs.Where(x => x.WeighQty == null).Count() == 0)
                                        ? ssrs.Select(t => t.WeighQty ?? null).Sum()
                                        : null;
                                    //Random rnd = new Random(); //測試用
                                    //qus[i].ssrQty = rnd.Next(10); //測試用
                                    UpdateData = true;
                                }
                                catch
                                {
                                    qus[i].ssrQty = null;
                                }
                            }
                            #endregion
                            #region 檢查門 00,01,12
                            if (qus[i].ssrState == "0" || qus[i].oprState == "0" || qus[i].oprState == "2")
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "DOORCHK" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                bool lockNotWork = ssrList.Where(x => x.SensorType == "LOCK" && x.TargetObjFid == qus[i].DrawFid && !string.IsNullOrEmpty(x.NotWork)).Count() > 0;
                                bool? finalresult = lockNotWork ? null : CheckDoor(ssrs);                                
                                if (finalresult == null || lockNotWork) //門鎖或偵測壞掉都是,直接對應oprState
                                {
                                    qus[i].ssrState = qus[i].oprState == "2" ? "2" : "1";
                                    UpdateData = true;
                                }
                                else
                                {
                                    bool opened = (bool)finalresult;
                                    switch (qus[i].ssrState)
                                    {
                                        case "0": //未開門
                                            qus[i].ssrState = opened ? "1" : "0";
                                            if (qus[i].ssrState == "1") { UpdateData = true; }
                                            else
                                            { //若15秒沒開,就當門鎖壞掉
                                                TimeSpan passed = DateTime.Now - qus[i].moddate;
                                                if (passed.Seconds > 5)
                                                {
                                                    SensorDeviceCtrl? up = ssrList.Where(x => x.SensorType == "LOCK" && x.TargetObjFid == qus[i].DrawFid ).FirstOrDefault();
                                                    if (up != null && up.FID > 0 && up.SensorVersion.Contains("AutoOpen"))
                                                    { UpdateNotWorking(up.FID, "ERR4:偵測到門無法開啟"); }
                                                }
                                            }
                                            break;
                                        case "1": //開門中
                                            qus[i].ssrState = opened ? "1" : "2";
                                            if (qus[i].ssrState == "2") { UpdateData = true; }
                                            break;
                                        case "2": //又開門
                                            qus[i].ssrState = opened ? "1" : "2";
                                            if (qus[i].ssrState == "1") { UpdateData = true; }
                                            break;
                                    }
                                }
                            }
                            #endregion
                            args.QuObj = qus[i];
                            #region 存檔並刪除佇列
                            if (state == "22")
                            {
                                //暫時沒磅秤
                                //List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "SCALE" && x.TargetObjFid == qus[i].DrugGridFid).ToList();
                                //foreach (SensorDeviceCtrl s in ssrs)
                                //{
                                //    if (qus[i].ssrQty != null)
                                //    {
                                //        db.ScaleWeighQtyLog.Add(new ScaleWeighQtyLog()
                                //        {
                                //            StockBillFid = qus[i].stockBillFid,
                                //            DrugGridFid = qus[i].DrugGridFid ?? 0,
                                //            SensorNo = s.SensorNo,
                                //            Weight = s.UnitWeight ?? 0,
                                //            Qty = s.UnitQty ?? 0,
                                //            logtime = DateTime.Now,
                                //        });
                                //        db.SaveChanges();
                                //    }

                                //    ssrList.Remove(s);
                                //}
                                List<SensorDeviceCtrl> del = ssrList.Where(x => x.TargetTable == "Drawer" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                foreach (SensorDeviceCtrl s in del)
                                {
                                    ssrList.Remove(s);
                                }

                                args.QuObj.ssrState = "D";

                                db.Remove(qus[i]);//這裡刪除後,到view找不到也會跟著刪
                                db.SaveChanges();
                            }

                            #endregion
                            #region 例外
                            if (qus[i].oprState == "H") //資料庫有但介面沒有
                            {
                                List<SensorDeviceCtrl> ssrs = ssrList.Where(x => x.SensorType == "DOORCHK" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                bool? finalresult = CheckDoor(ssrs);
                                if (finalresult == true)
                                {
                                    //MailService mailService = new MailService();
                                    //mailService.AlertMsg(qus[i]);
                                }

                                List<SensorDeviceCtrl> del = ssrList.Where(x => x.TargetTable == "Drawer" && x.TargetObjFid == qus[i].DrawFid).ToList();
                                foreach (SensorDeviceCtrl s in del)
                                {
                                    ssrList.Remove(s);
                                }

                                args.QuObj.ssrState = "D";


                                db.Remove(qus[i]); //這裡刪除後,到view找不到也會跟著刪
                                db.SaveChanges();
                            }
                            #endregion
                            if (UpdateData && args.QuObj.ssrState != "D") //刪了,不用更新
                            {
                                db.Update(qus[i]);
                                db.SaveChanges();
                            }

                        }
                        #endregion
                        if (ssrQuUpdateEvent != null)
                        {
                            ssrQuUpdateEvent.Invoke(this, args);
                        }
                    }
                    db.Dispose();
                }
                catch (Exception ex) { qwFunc.savelog($"回圈內判斷流程出錯：{ex.Message}"); }

                await Task.Delay(2000);
            }

        }

        #endregion

        #region functions

        #region 取得藥格及藥格藥品的感應器
        public List<SensorDeviceCtrl> GetSensors(int drawfid, int druggridfid)
        {
            List<SensorDeviceCtrl> result = new List<SensorDeviceCtrl>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectString"]))
            {
                string sql = $"select FID, TargetTable, TargetObjFid, SensorType, SensorVersion, SensorNo, SerialPort, Modbus_Addr, Modbus_Cmd, Modbus_Rgst, NotWork " +
                        $"from SensorDevice " +
                        $"where TargetTable = 'Drawer' and TargetObjFid = {drawfid} ";
                var recs1 = conn.Query<SensorDeviceCtrl>(sql);
                result.AddRange(recs1.ToList());
                sql = $"select sd.FID, TargetTable, TargetObjFid, SensorType, SensorVersion, SensorNo, SerialPort, Modbus_Addr, Modbus_Cmd, Modbus_Rgst, UnitWeight, UnitQty, NotWork " +
                    $"from SensorDevice sd " +
                    $"left join MapPackOnSensor m on sd.FID = m.SensorFid " +
                    $"left join DrugPackage p on p.FID = m.PackageFid " +
                    $"where TargetTable = 'DrugGrid' and TargetObjFid = {druggridfid} ";
                var recs2 = conn.Query<SensorDeviceCtrl>(sql);
                result.AddRange(recs2.ToList());
            }
            return result;
        }
        #endregion
        #region 檢查門
        public bool? CheckDoor(List<SensorDeviceCtrl> objs)
        {
            bool? isOpen = null;
            string cmdstring = "";
            foreach (SensorDeviceCtrl ssr in objs)
            {
                if (!string.IsNullOrEmpty(ssr.NotWork)) { continue; } //壞掉了
                try
                {
                    if (setComport(ssr.SerialPort))
                    {
                        cmdstring = ssr.genChkCmd();
                        string resultcmd = mbus.ExCmd(cmdstring);
                        if (resultcmd.StartsWith("ERR"))
                        {
                            ssr.NotWork = resultcmd; UpdateNotWorking(ssr.FID, ssr.NotWork);
                            qwFunc.savelog($"執行#{ssr.FID}指令{cmdstring}失敗:{resultcmd}");
                        }
                        else 
                        {
                            isOpen = resultcmd.Split(" ")[3] == "00";
                        }
                    }
                    else
                    {
                        UpdateNotWorking(ssr.FID, $"ERR0:{ssr.SerialPort}失敗");
                        qwFunc.savelog($"設定comport:{ssr.SerialPort}失敗");
                    }
                }
                catch (Exception ex) { qwFunc.savelog($"開LED錯誤#{ssr.FID}：{ex.Message}"); }

            }
            return isOpen;
        }
        #endregion
        #region 取得藥格藥品id
        public int getDrugGridFid(int drawerfid, string DrugCode)
        {
            DBcPharmacy db = new DBcPharmacy();
            int result = (from dg in db.DrugGrid
                          join di in db.DrugInfo on dg.DrugFid equals di.FID
                          where di.DrugCode.Equals(DrugCode) && dg.DrawFid.Equals(drawerfid) && dg.CbntFid.Equals(AtCbntFid)
                          select dg.FID).FirstOrDefault();
            return result;
        }
        #endregion
        #region 方便設定COMPORT
        private bool setComport(string comport)
        {
            try
            {
                return mbus.SetConfig(comport, ConfigurationManager.AppSettings[$"PortName_{comport}"]?.ToString()?.Split(','));
            }
            catch { return false; }
        }
        #endregion
        #region 設備壞掉存檔
        private void UpdateNotWorking(int fid, string reason)
        {
            using (DBcPharmacy db = new DBcPharmacy())
            {
                SensorDevice? sd = db.SensorDevice?.Find(fid);
                if(sd != null)
                {
                    sd.NotWork = reason;
                    sd.NotWorkTime = DateTime.Now;
                    db.SensorDevice?.Update(sd);
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #endregion

    }
}

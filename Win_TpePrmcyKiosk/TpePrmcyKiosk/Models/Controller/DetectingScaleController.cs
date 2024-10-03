using TpePrmcyKiosk.Models.Unit;
using static TpePrmcyKiosk.Models.Controller.DetectingScaleController;
using Microsoft.EntityFrameworkCore;

namespace TpePrmcyKiosk.Models.Controller
{
    public delegate void DetectScaleControllerHandler(Object sender, DetectScaleControllerArgs e);
    public class DetectingScaleController
    {
        ModbusRtuClient RtuClient = new ModbusRtuClient();
        public event DetectScaleControllerHandler CtrlrReceiveHandler = null;
        public event DetectScaleControllerHandler CtrlrOpenHandler = null;
        public event DetectScaleControllerHandler CtrlrCloseHandler = null;

        public List<bool> DeviceEnabled = new List<bool> { false, false, false, false, false, false, false, false }; //可用設備

        private static int LoopStop = 20; //config 每次間隔時間
        private static int DetectRows = 3;
        //controll received a row datas
        private int RowIndex = 0;
        private DateTime? LastTimeReceive = null;
        private decimal[][] RowsWeightValue = new decimal[DetectRows][] ; //現重
        private bool[] GotValueInARow = new bool[8] { false, false, false, false, false, false, false, false };
        private bool ARowDataDone = false;

        public bool[] DetectStable = new bool[8] { false, false, false, false, false, false, false, false };
        private decimal[] StableRow = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //上次穩定的重量
        private decimal[] LostWeight = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //開始到現在所減少的重量
        private decimal[] TtlLostWeight = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //總減少的重量(加資料庫)

        public DetectingScaleController()
        {
            for(int x = 0; x < DetectRows; x++) { RowsWeightValue[x] = new decimal[8]; }
        }
        public class DetectScaleControllerArgs : EventArgs
        {
            public DateTime ActTime = DateTime.Now;
            public bool ConnectState = false; //是否連線
            public bool DetectingState = false; //是否在偵測中            
            public bool[] DetectStable = new bool[8] { false, false, false, false, false, false, false, false };
            public decimal[] NowWeightValues = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            public decimal[] LostWeight = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //開始到現在所減少的重量
            public decimal[] TtlLostWeight = new decimal[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //總減少的重量(加資料庫)
        }

        #region handler
        private void busRtuReceiveHandler(object sender, busRtuResponseArgs e)
        {
            DetectScaleControllerArgs args = new DetectScaleControllerArgs();
            args.ConnectState = true;
            args.DetectingState = true;
            if (!string.IsNullOrEmpty(e.resHexValue))
            {
                decimal value = RtuClient.HexToPrecision00Decimal(e.resHexValue); //值
                string[] cmdarray = e.reqCmd.Split(' ');
                int regist = RtuClient.HexToInt(cmdarray[2] + cmdarray[3]);
                int index = (regist - 80) / 500; //regist位置
                
                if(LastTimeReceive == null) //第一次,沒時間
                {
                    LastTimeReceive = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    DateTime now = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    if( ((TimeSpan)(now - LastTimeReceive)).Seconds > (LoopStop - 2) && !GotValueInARow.Contains(true)) //新的一列
                    {
                        GotValueInARow = new bool[8] { false, false, false, false, false, false, false, false };
                        RowIndex = RowIndex == (DetectRows) ? RowIndex : RowIndex+1;
                        if (RowIndex == (DetectRows)) //刪除舊row,要加新row,到3但row3還沒加資料,所以判斷到4再加
                        {
                            RowsWeightValue[0] = RowsWeightValue[1].ToList().ToArray();
                            RowsWeightValue[1] = RowsWeightValue[2].ToList().ToArray();
                            RowIndex = DetectRows - 1;
                        }
                    }
                    LastTimeReceive = now; //更新上次時間
                }
                RowsWeightValue[RowIndex][index] = value; //給新row值
                GotValueInARow[index] = true; //紀錄row值有更新

                ARowDataDone = true;
                for (int x = 0; x < 8; x++) //判斷新一輪row是否都更新完畢
                {
                    if(GotValueInARow[x] != DeviceEnabled[x]) { ARowDataDone = false; }
                }
            }
            if (ARowDataDone) //新一輪row都更新,一些資料初始化,計算重量變化是否需處理
            {
                GotValueInARow = new bool[8] { false, false, false, false, false, false, false, false };
                args.NowWeightValues = RowsWeightValue[RowIndex];

                if (RowIndex == (DetectRows - 1))
                {
                    try
                    {
                        CalculateLosingWeight(e.ComPort, e.reqCmd.Substring(0, 2)); //計算
                    }
                    catch(Exception ex)
                    {
                        qwFunc.savelog(ex.ToString());
                    }

                    args.NowWeightValues = RowsWeightValue[RowIndex]; //結果
                    args.DetectStable = DetectStable; //結果
                    args.LostWeight = LostWeight; //結果
                    args.TtlLostWeight = TtlLostWeight; //結果
                }
                
            }


            
            if (CtrlrReceiveHandler != null && (ARowDataDone || e.ActName == "Stop Loop"))
            {
                if(e.ActName == "Loop Stop") {                    
                    args.DetectingState = false; 
                }
                CtrlrReceiveHandler.Invoke(this, args);
            }
            
        }
        private void busRtuOpenHandler(object sender, busRtuResponseArgs e)
        {
            DetectScaleControllerArgs args = new DetectScaleControllerArgs();            
            args.ConnectState = e.ActState;
            if (args.ConnectState)
            {
                DeviceEnabled = RtuClient.getPortsStatus();
            }
            if (CtrlrOpenHandler != null)
            {
                CtrlrOpenHandler.Invoke(this, args);
            }
        }
        private void busRtuCloseHandler(object sender, busRtuResponseArgs e)
        {
            DetectScaleControllerArgs args = new DetectScaleControllerArgs();
            if (CtrlrCloseHandler != null)
            {
                CtrlrCloseHandler.Invoke(this, args);
            }
        }
        #endregion

        public bool CalculateLosingWeight(string Comport, string DeivceAddr)
        {
            decimal Tolerance = 0.05M;
            decimal MaxLmtWet = 10;
            decimal MinLmtWet = 1;
            bool valchange = false;
            List<decimal> rows = new List<decimal>();
            for (int reg = 0; reg < RowsWeightValue[0].Length; reg++) //
            {
                if (!DeviceEnabled[reg]) { continue; }
                LostWeight[reg] = 0; //本次值變動,先清空

                #region 計算是否記入變動準備                
                decimal diffper1 = qwFunc.DiffPercent(RowsWeightValue[1][reg], RowsWeightValue[0][reg]);
                decimal diffper2 = qwFunc.DiffPercent(RowsWeightValue[2][reg], RowsWeightValue[1][reg]);
                #endregion

                DetectStable[reg] = diffper2 <= Tolerance;
                if (DetectStable[reg]) //穩定
                {
                    decimal stablevalue = Math.Round(((RowsWeightValue[1][reg] + RowsWeightValue[2][reg]) / 2), 2); //平均一下
                    if (StableRow[reg] == 0) { StableRow[reg] = stablevalue; } //初始
                    else
                    {
                        decimal laststablevalue = StableRow[reg]; //上次穩定值
                        StableRow[reg] = stablevalue;
                        //decimal diffperstable = StaticFunc.DiffPercent(laststablevalue, stablevalue); //變動率        
                        if (Math.Abs(stablevalue) > MinLmtWet) //判定為取藥
                        {
                            LostWeight[reg] = stablevalue - laststablevalue;
                            StableRow[reg] = StableRow[reg] - LostWeight[reg]; //取藥時,此次穩定值就被扣除取量
                            RowsWeightValue[RowIndex][reg] = StableRow[reg]; //目前跑的值也更新一下
                            TtlLostWeight[reg] = TtlLostWeight[reg] + LostWeight[reg];
                            valchange = true;
                            CleanScale(DeivceAddr, reg);
                            #region 資料庫
                            //ScaleModbusConf 的 fid
                            //using (ChineseHerbTest con = new ChineseHerbTest())
                            //{
                            //    ElecDetectRecords insert = new ElecDetectRecords();
                            //    insert.RecWeight = LostWeight[reg];
                            //    insert.RecordTime = DateTime.Now;

                            //    //取 設備資訊FID + 物件重量ID
                            //    ScaleModbusConf conf = con.ScaleModbusConf.Where(x => x.Modbus_Com == Comport && x.Modbus_Addr == DeivceAddr && x.Modbus_RgstNo == reg).FirstOrDefault();
                            //    if (conf == null) { continue; }
                            //    insert.SclMdbsCnfFid = conf.FID;

                            //    //物件重量換算單位數量
                            //    DrugWeight weightinfo = con.DrugWeight.Find(conf.WeightObjFid);
                            //    if (weightinfo == null) { continue; }
                            //    if (StaticFunc.CalculateWeightToQty(-LostWeight[reg], weightinfo.UnitWeight, weightinfo.UnitQty,
                            //        out decimal reQty, out decimal reTolrn, out bool reTrust, out string errmsg_toqty)
                            //    )
                            //    {
                            //        insert.RecQty = reQty;
                            //        decimal getTolrn = reTolrn;
                            //        bool getTrust = reTrust;
                            //    }
                            //    else
                            //    {
                            //        string msg = errmsg_toqty;
                            //    }

                            //    //存檔
                            //    con.ElecDetectRecords.Add(insert);
                            //    con.SaveChanges();
                                
                            //}

                            #endregion
                        }
                        else //保持穩定,但有可能會漂,清零
                        {
                            //漂掉了,清零
                            if (Math.Abs(stablevalue) > 0.1M && Math.Abs(stablevalue) < 0.5M)
                            {
                                CleanScale(DeivceAddr, reg);
                            }

                        }

                    }

                }
            }
            return valchange;
        }

        private async void CleanScale(string DeviceNo, int ChennalNo)
        {
            bool yet = true;
            while(yet) //怕沒成功
            {
                busRtuResponseArgs res = RtuClient.ExcuteCmd($"{DeviceNo} 10 {RtuClient.toHexWithSpace((ChennalNo * 500) + 94)} 00 01 02 00 01");
                yet = res.resHexArray.Count != 8; //成功就不再執行清零
                if (yet) { await Task.Delay(100); }
            }
        }

        #region 介面控制用
        public void ConnectModbus(bool onoff)
        {
            if (!RtuClient.IsOpen() && onoff)
            {
                if (!RtuClient.CheckEvent())
                {
                    RtuClient.busRtuResponseEvent += busRtuReceiveHandler;
                    RtuClient.busRtuOpenEvent += busRtuOpenHandler;
                    RtuClient.busRtuCloseEvent += busRtuCloseHandler;
                }
                RtuClient.Open();

                
            }
            if(RtuClient.IsOpen() && !onoff)
            {
                RtuClient.Close();
                RtuClient.busRtuResponseEvent -= busRtuReceiveHandler;
                RtuClient.busRtuOpenEvent -= busRtuOpenHandler;
                RtuClient.busRtuCloseEvent -= busRtuCloseHandler;
            }
        }

        public bool IsOpen()
        {
            return RtuClient.IsOpen();
        }

        public void RunExecuteCommandLoop(string DeviceAddr) {
            RtuClient.CleanCommandInLoop();
            
            if (RtuClient.IsOpen() && DeviceEnabled.Contains(true))
            {
                for (int x = 0; x < 8; x++)
                {
                    if (DeviceEnabled[x])
                    {
                        RtuClient.AddCommandInLoop($"{DeviceAddr} 03 {RtuClient.toHexWithSpace(x * 500 + 80)} 00 02");
                    }
                }

                RtuClient.ExecuteCmdLoop(LoopStop); //單位:秒
            }

        }

        public void StopCommandLoop()
        {
            RtuClient.StopCmdLoop();
        }

        public void CleanAllUp(string DeviceNo)
        {
            for(int c = 0; c < DeviceEnabled.Count; c++)
            {
                if (DeviceEnabled[c]) { CleanScale(DeviceNo, c); }
            }
        }
        #endregion

    }
}

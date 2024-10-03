using Dapper;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.DOM;
using TpePrmcyKiosk.Models.Service;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk
{
    public partial class FormDeviceDetect : Form
    {
        private MainForm _parent;
        private int AtCbntFid = Convert.ToInt32(ConfigurationManager.AppSettings["AtCbntFid"]);
        private string LedColors = ConfigurationManager.AppSettings["LedColors"].ToString().Split(',').First();

        private MbusExcServ mbus = new MbusExcServ();
        DBcPharmacy _db = new DBcPharmacy();
        List<int> _deviceFids = new List<int>();
        List<GridFid> _gridFids = new List<GridFid>();
        List<Drawers> _Drawers = new List<Drawers>();
        List<SensorDeviceCtrl> _list = new List<SensorDeviceCtrl>();

        public FormDeviceDetect(MainForm parent)
        {
            InitializeComponent();
            _parent = parent;

            setComport("COM4");
            string resultcmd = mbus.ExCmd("01 05 33 05 00 00");
        }

        private void FormComuTaskTest_Load(object sender, EventArgs e)
        {
            txt_ResultList.BackColor = Color.Black;
            txt_ResultList.ForeColor = Color.White;
            btn_Start.Enabled = false; btn_Start.Text = "請選擇動作";

            _gridFids = (from grid in _db.DrugGrid
                         join drug in _db.DrugInfo on grid.DrugFid equals drug.FID
                         where grid.CbntFid == AtCbntFid
                         select new GridFid { Fid = grid.FID, DrawFid = grid.DrawFid, DrugCode = drug.DrugCode })
                        .ToList();

            List<int> _drawFids = _db.Drawers.Where(x => x.CbntFid == AtCbntFid).Select(x => x.FID).ToList();

            _deviceFids = _db.SensorDevice
                .Where(x => (x.TargetTable == "Drawer" && _drawFids.Contains(x.TargetObjFid ?? 0)) ||
                (x.TargetTable == "DrugGrid" && _gridFids.Select(x => x.Fid).Contains(x.TargetObjFid ?? 0))
                ).Select(x => x.FID).ToList();

            if (_deviceFids.Count() == 0) { qwFunc.Alert("本櫃無設定設備資料"); cb_action.Enabled = false; }

            _Drawers = _db.Drawers.Where(x => x.CbntFid == AtCbntFid).ToList();

            cb_action.SelectedIndex = 1;
            cb_action.Enabled = false;
        }

        private void cb_action_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox selecter = (ComboBox)sender;
            switch (selecter.SelectedIndex)
            {
                case 0: //已壞掉設備
                    this.pnl_Drawers.Controls.Clear();
                    getDevices(true);
                    break;
                case 1: //單抽屜
                    txt_ResultList.Text = "讀取本櫃所有抽屜中...";
                    displayDrawers();
                    break;
                case 2: //全櫃設備
                    this.pnl_Drawers.Controls.Clear();
                    getDevices(false);
                    break;
            }
        }

        #region 單抽屜
        private async Task displayDrawers()
        {
            try
            {
                lb_CbntName.Text = _db.Cabinet.Find(AtCbntFid).CbntName;

                List<Drawers> drawers = _db.Drawers.Where(x => x.CbntFid == AtCbntFid).OrderBy(x => x.No).ToList();
                int startX = 5, startY = 5;
                int width = 60, heigh = 50;
                int gap = 5;
                int colcount = 10;

                this.pnl_Drawers.Width = (width + gap) * 10 + (startX * 2);
                this.pnl_Drawers.Height = (heigh + gap) * ((drawers.Count / 10) + 1) + (startY * 2);
                txt_ResultList.Text = "";
                for (int i = 0; i < drawers.Count; i++)
                {
                    //檢查是否有壞的設備
                    bool badDevice = _db.SensorDevice.Where(x => !string.IsNullOrEmpty(x.NotWork) && x.TargetObjFid == drawers[i].FID && x.TargetTable == "Drawer").Count() > 0;
                    if (!badDevice)
                    {
                        badDevice = (from dev in _db.SensorDevice
                                     join grid in _db.DrugGrid on dev.TargetObjFid equals grid.FID
                                     where dev.TargetTable == "DrugGrid" && !string.IsNullOrEmpty(dev.NotWork)
                                     && grid.DrawFid == drawers[i].FID
                                     select dev).ToList().Count() > 0;
                    }

                    int locatX = (width + gap) * (i % 10) + startX;
                    int locatY = (heigh + gap) * (i / 10) + startY;

                    Button newButton = new Button();
                    newButton.Name = $"Drawer_{drawers[i].FID}";
                    newButton.Text = $"# {drawers[i].No}";
                    newButton.Location = new Point(locatX, locatY);
                    newButton.Size = new Size(width, heigh);

                    if (badDevice) { newButton.ForeColor = Color.Red; newButton.FlatAppearance.BorderColor = Color.Red; }
                    newButton.Click += new EventHandler(btn_Drawer_Click);

                    this.pnl_Drawers.Controls.Add(newButton);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("連接資料庫失敗，無法操作！");

            }
        }

        private void btn_Drawer_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.pnl_Drawers.Controls)
            {
                if (ctrl is Button)
                {
                    ((Button)ctrl).BackColor = SystemColors.InactiveCaption;
                }
            }
            Button btn = (Button)sender;
            btn.BackColor = Color.LightGreen;
            int getFid = Convert.ToInt32(btn.Name.Replace("Drawer_", string.Empty));
            getDevices(getFid);

        }

        #endregion

        #region 列出待測清單
        private void getDevices(bool NotWork)
        {
            List<SensorDeviceCtrl> result = new List<SensorDeviceCtrl>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectString"]))
            {
                string sql = $"select sd.FID, TargetTable, TargetObjFid, SensorType, SensorVersion, SensorNo, SerialPort, Modbus_Addr, Modbus_Cmd, Modbus_Rgst, UnitWeight, UnitQty, NotWork " +
                    $"from SensorDevice sd " +
                    $"left join MapPackOnSensor m on sd.FID = m.SensorFid " +
                    $"left join DrugPackage p on p.FID = m.PackageFid " +
                    $"where sd.FID in ({string.Join(",", _deviceFids.Select(x => x.ToString()))}) " +
                    $" {(NotWork ? "and isnull(NotWork,'') <> '' " : "")} ";
                List<SensorDeviceCtrl> list = conn.Query<SensorDeviceCtrl>(sql).ToList();
                ListingData(list);
            }
        }

        private void getDevices(int drawer)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectString"]))
            {
                string gridfids = string.Join(",", _gridFids.Where(x => x.DrawFid == drawer).Select(x => x.Fid.ToString()));
                string sql = $"select sd.FID, TargetTable, TargetObjFid, SensorType, SensorVersion, SensorNo, SerialPort, Modbus_Addr, Modbus_Cmd, Modbus_Rgst, UnitWeight, UnitQty, NotWork " +
                    $"from SensorDevice sd " +
                    $"left join MapPackOnSensor m on sd.FID = m.SensorFid " +
                    $"left join DrugPackage p on p.FID = m.PackageFid " +
                    $"where ((TargetTable = 'Drawer' and TargetObjFid = {drawer}) ";
                if (gridfids != "") { sql += $" or (TargetTable = 'DrugGrid' and TargetObjFid in ({gridfids})) "; }
                sql += ")";
                List<SensorDeviceCtrl> list = conn.Query<SensorDeviceCtrl>(sql).ToList();
                ListingData(list);
            }
        }

        private async void ListingData(List<SensorDeviceCtrl> list)
        {
            txt_ResultList.Clear();
            if (list.Count() == 0) { qwFunc.Alert("查無設定設備資料"); }
            foreach (SensorDeviceCtrl dev in list)
            {
                if (dev.TargetTable == "Drawer")
                {
                    dev.DrawerNo = _Drawers.Find(x => x.FID == dev.TargetObjFid)?.No;
                }
                if (dev.TargetTable == "DrugGrid")
                {
                    GridFid? g = _gridFids.Find(x => x.Fid == dev.TargetObjFid);
                    if (g != null)
                    {
                        dev.DrawerNo = _Drawers.Find(x => x.FID == g.DrawFid)?.No;
                        dev.DrugCode = g.DrugCode;
                    }
                }
                string line = $"#{dev.FID} \t抽屜:{dev.DrawerNo} \t藥碼:{dev.DrugCode.PadRight(7, '_')} \t設備:{dev.SensorType.PadRight(10, '_')}" +
                    $"\t狀態:{(string.IsNullOrEmpty(dev.NotWork) ? "OK" : dev.NotWork)} ";
                txt_ResultList.AppendText(line + Environment.NewLine);
            }
            _list = list.OrderBy(x => x.DrawerNo).ToList();

            btn_Start.Enabled = true; btn_Start.Text = "開始";
            //RunTesting(list, list.GroupBy(x=>x.DrawerNo).Select(x=>x.Key).ToList());
        }
        #endregion

        private async Task RunTesting(List<SensorDeviceCtrl> list, List<int?> Drawers)
        {
            txt_ResultList.Clear();
            List<SensorDeviceCtrl> ssrs = new List<SensorDeviceCtrl>();
            for (int dw = 0; dw < Drawers.Count(); dw++)
            {
                #region 燈
                ssrs = list.Where(x => x.SensorType == "LED" && x.DrawerNo == Drawers[dw]).ToList();
                foreach (SensorDeviceCtrl ssr in ssrs)
                {
                    string errmsg = "";
                    try
                    {
                        if (setComport(ssr.SerialPort))
                        {
                            string resultcmd = mbus.ExCmd(ssr.genLitCmd(LedColors, true));
                            errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            if (string.IsNullOrEmpty(errmsg))
                            {
                                await Task.Delay(500);
                                resultcmd = mbus.ExCmd(ssr.genLitCmd(LedColors, false));
                                errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            }
                        }
                        else
                        {
                            errmsg = $"ERR0:{ssr.SerialPort}失敗";
                        }
                    }
                    catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                    string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                            $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                            $"燈測試: {(string.IsNullOrEmpty(errmsg) ? "OK" : errmsg)}";
                    UpdateIfWorkChange(ssr, errmsg);
                    txt_ResultList.AppendText(line + Environment.NewLine);
                }
                await Task.Delay(400);
                #endregion

                #region 測門關
                bool DoorClosed = false;
                List<bool> DoorClosedSuccess = new List<bool>();
                ssrs = list.Where(x => x.SensorType == "DOORCHK" && x.DrawerNo == Drawers[dw]).ToList();
                foreach (SensorDeviceCtrl ssr in ssrs)
                {
                    string errmsg = "";
                    try
                    {
                        if (setComport(ssr.SerialPort))
                        {
                            string resultcmd = mbus.ExCmd(ssr.genChkCmd());
                            errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            if (errmsg == "")
                            {
                                DoorClosedSuccess.Add(resultcmd.Split(" ")[3] == "03");
                                if (!(resultcmd.Split(" ")[3] == "03"))
                                {
                                    errmsg = "ERR0:門沒關";
                                }
                            }
                        }
                        else
                        {
                            errmsg = $"ERR0:{ssr.SerialPort}失敗";
                        }
                    }
                    catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                    string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                            $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                            $"門關偵測: {(string.IsNullOrEmpty(errmsg) ? "OK" : errmsg)}";
                    UpdateIfWorkChange(ssr, errmsg);
                    txt_ResultList.AppendText(line + Environment.NewLine);
                }
                DoorClosed = DoorClosedSuccess.Where(x => x == true).Count() == ssrs.Count();
                #endregion
                await Task.Delay(400);

                #region 開門
                bool OpenTheDoor = false;
                List<bool> OpenTheDoorSuccess = new List<bool>();
                if (DoorClosed)
                {
                    ssrs = list.Where(x => x.SensorType == "LOCK" && x.DrawerNo == Drawers[dw]).ToList();
                    foreach (SensorDeviceCtrl ssr in ssrs)
                    {
                        string errmsg = "";
                        try
                        {
                            if (setComport(ssr.SerialPort))
                            {
                                string resultcmd = mbus.ExCmd(ssr.genLockCmd(true));
                                errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                                OpenTheDoorSuccess.Add(errmsg == "");
                            }
                            else
                            {
                                errmsg = $"ERR0:{ssr.SerialPort}失敗";
                            }
                        }
                        catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                        string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                                $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                                $"開門測試: {(string.IsNullOrEmpty(errmsg) ? "OK" : errmsg)}";
                        UpdateIfWorkChange(ssr, errmsg);
                        txt_ResultList.AppendText(line + Environment.NewLine);
                    }
                    OpenTheDoor = OpenTheDoorSuccess.Where(x => x == true).Count() == ssrs.Count();
                }
                #endregion
                await Task.Delay(5000);

                #region 檢察門是否開
                bool DoorOpened = false;
                List<bool> DoorOpenedSuccess = new List<bool>();
                if (DoorClosed && OpenTheDoor)
                {
                    ssrs = list.Where(x => x.SensorType == "DOORCHK" && x.DrawerNo == Drawers[dw]).ToList();
                    foreach (SensorDeviceCtrl ssr in ssrs)
                    {
                        string errmsg = "";
                        try
                        {
                            if (setComport(ssr.SerialPort))
                            {
                                string resultcmd = mbus.ExCmd(ssr.genChkCmd());
                                errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                                if (string.IsNullOrEmpty(errmsg))
                                {
                                    DoorOpenedSuccess.Add(resultcmd.Split(" ")[3] == "02");
                                    if (!(resultcmd.Split(" ")[3] == "02"))
                                    {
                                        errmsg = "ERR0:門沒開";
                                    }
                                }
                            }
                            else
                            {
                                errmsg = $"ERR0:{ssr.SerialPort}失敗";
                            }
                        }
                        catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                        string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                                $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                                $"門開偵測: {(string.IsNullOrEmpty(errmsg) ? "OK" : errmsg)}";
                        UpdateIfWorkChange(ssr, errmsg);
                        txt_ResultList.AppendText(line + Environment.NewLine);
                    }
                    DoorOpened = DoorOpenedSuccess.Where(x => x == true).Count() == ssrs.Count();
                }
                #endregion
                await Task.Delay(400);

                #region 關門
                if (DoorClosed && OpenTheDoor && DoorOpened)
                {
                    ssrs = list.Where(x => x.SensorType == "LOCK" && x.DrawerNo == Drawers[dw]).ToList();
                    foreach (SensorDeviceCtrl ssr in ssrs)
                    {
                        string errmsg = "";
                        try
                        {
                            if (setComport(ssr.SerialPort))
                            {
                                string resultcmd = mbus.ExCmd(ssr.genLockCmd(false));
                                errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            }
                            else
                            {
                                errmsg = $"ERR0:{ssr.SerialPort}失敗";
                            }
                        }
                        catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                        string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                                $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                                $"關門測試: {(string.IsNullOrEmpty(errmsg) ? "OK" : errmsg)}";
                        UpdateIfWorkChange(ssr, errmsg);
                        txt_ResultList.AppendText(line + Environment.NewLine);
                    }
                }
                #endregion

                #region 沒開門
                if (DoorClosed && OpenTheDoor && !DoorOpened)
                {
                    ssrs = list.Where(x => x.SensorType == "LOCK" && x.DrawerNo == Drawers[dw] && (x.SensorVersion?.Split(":").Contains("AutoOpen") ?? false)).ToList();
                    foreach (SensorDeviceCtrl ssr in ssrs)
                    {
                        string errmsg = $"ERR4:偵測到門無法開啟";
                        string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                                $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                                $"開門失敗: {errmsg}";
                        UpdateIfWorkChange(ssr, errmsg);
                        txt_ResultList.AppendText(line + Environment.NewLine);
                    }
                }
                #endregion

                #region 磅秤
                ssrs = list.Where(x => x.SensorType == "SCALE" && x.DrawerNo == Drawers[dw]).ToList();
                foreach (SensorDeviceCtrl ssr in ssrs)
                {
                    string errmsg = "";
                    try
                    {
                        if (setComport(ssr.SerialPort))
                        {
                            string resultcmd = mbus.ExCmd(ssr.genScaleCmd("data"));
                            errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            if (string.IsNullOrEmpty(errmsg))
                            {
                                try
                                {
                                    string[] hexarr = resultcmd.Split(' '); //送出並取得回傳結果
                                    string hexvalue = "";
                                    for (int t = 3; t < mbus.HexToInt(hexarr[2]) + 3; t++) { hexvalue += hexarr[t]; } //取hex值
                                    decimal weight = Math.Abs(mbus.HexToPrecision00Decimal(hexvalue)); //轉成重量
                                    decimal LostWeight = weight - ssr.WeighWeight0 ?? 0;
                                    mbus.CalculateWeightToQty(-LostWeight, ssr.UnitWeight, ssr.UnitQty,
                                        out decimal reQty, out decimal reTolrn, out bool reTrust, out string errmsg_toqty);
                                    ssr.WeighWeight = weight;
                                    ssr.WeighQty = reQty;
                                }
                                catch (Exception ex) { errmsg = $"回傳結果轉成重量失敗:{ex}"; }
                            }
                        }
                        else
                        {
                            errmsg = $"ERR0:{ssr.SerialPort}失敗";
                        }
                    }
                    catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                    string line = $"#{ssr.FID} \t抽屜:{ssr.DrawerNo} \t藥碼:{ssr.DrugCode.PadRight(7, '_')} \t設備:{ssr.SensorType.PadRight(10, '_')}" +
                            $"\t狀態:{(string.IsNullOrEmpty(ssr.NotWork) ? "OK" : ssr.NotWork)} \t" +
                            $"磅秤測試: {(string.IsNullOrEmpty(errmsg) ? ssr.WeighWeight : errmsg)}";
                    UpdateIfWorkChange(ssr, errmsg);
                    txt_ResultList.AppendText(line + Environment.NewLine);
                }
                await Task.Delay(400);
                #endregion
            }

            btn_Start.Enabled = false; btn_Start.Text = "完成";
            if (cb_action.SelectedIndex == 1)
            {
                foreach (Control ctrl in this.pnl_Drawers.Controls)
                {
                    if (ctrl is Button)
                    {
                        ((Button)ctrl).Enabled = true;
                    }
                }
            }
        }

        private void FormComuTaskTest_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #region functions
        private bool setComport(string comport)
        {
            try
            {
                return mbus.SetConfig(comport, ConfigurationManager.AppSettings[$"PortName_{comport}"]?.ToString()?.Split(','));
            }
            catch { return false; }
        }
        private void UpdateIfWorkChange(SensorDeviceCtrl ssr, string newResult)
        {
            if (string.IsNullOrEmpty(ssr.NotWork) ^ string.IsNullOrEmpty(newResult)) //變好或變壞
            {
                using (DBcPharmacy db = new DBcPharmacy())
                {
                    SensorDevice? sd = db.SensorDevice?.Find(ssr.FID);
                    if (sd != null)
                    {
                        sd.NotWork = newResult;
                        sd.NotWorkTime = string.IsNullOrEmpty(newResult) ? null : ((sd.NotWorkTime != null) ? sd.NotWorkTime : DateTime.Now);
                        db.SensorDevice?.Update(sd);
                        db.SaveChanges();
                    }
                }
            }



        }
        #endregion 

        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (btn_Start.Enabled && _list.Count() > 0)
            {
                if (cb_action.SelectedIndex == 1)
                {
                    foreach (Control ctrl in this.pnl_Drawers.Controls)
                    {
                        if (ctrl is Button)
                        {
                            ((Button)ctrl).Enabled = false;
                        }
                    }
                }
                btn_Start.Enabled = false; btn_Start.Text = "執行中";
                RunTesting(_list, _list.GroupBy(x => x.DrawerNo).Select(x => x.Key).ToList());
            }

        }
    }

    class GridFid
    {
        public int Fid;
        public int DrawFid;
        public string DrugCode;
    }
}

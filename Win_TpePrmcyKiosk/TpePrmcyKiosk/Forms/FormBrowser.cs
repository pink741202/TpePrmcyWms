using System;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.Service;
using TpePrmcyKiosk.Models.Unit;
using TpePrmcyKiosk.Models.DOM;
using System.Data;
using Dapper;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TpePrmcyKiosk
{
    public partial class FormBrowser : Form
    {
        private MainForm _parent;
        int _AtCbntFid = 0;
        DBcPharmacy _db = new DBcPharmacy();
        private MbusExcServ mbus = new MbusExcServ();
        DateTime chkDoorTime = DateTime.Now.AddMinutes(-15);
        bool sysStandby = false;

        public FormBrowser(MainForm parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void FormBrowser_Load(object sender, EventArgs e)
        {
            string AtCbntFid = ConfigurationManager.AppSettings["AtCbntFid"] ?? "";
            string WebUrl = ConfigurationManager.AppSettings["WebUrl"] ?? "";
            try
            {
                int intAtCbntFid = Convert.ToInt16(AtCbntFid);
                Cabinet? cbnt = _db.Cabinet?.Find(intAtCbntFid);
                if (cbnt != null || string.IsNullOrEmpty(WebUrl))
                {
                    _AtCbntFid = intAtCbntFid;
                    wV_mainsys.Source = new Uri($"{WebUrl}?AtCbntFid=" + AtCbntFid);
                    runDetector();
                }
                else
                {
                    qwFunc.Alert("請先設定存在的櫃號參數 (AtCbntFid)");
                }
            }
            catch
            {
                qwFunc.Alert("請先設定正確的櫃號參數 (AtCbntFid)");
            }
            DoorStatusChecking();
        }

        private void wV_mainsys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                //_parent.Controls.Find("menuStrip1", true)[0].Enabled = false; 
                e.Handled = true;
            }
        }

        private void runDetector()
        {
            SsrQuService quServ = new SsrQuService();
            quServ.RunLoop();
        }


        private void wV_mainsys_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            toolStripMsg.Text = wV_mainsys.Source.ToString();
            List<string> locat = wV_mainsys.Source.ToString().Split("/").ToList();
            TimeSpan passchk = DateTime.Now - chkDoorTime;
            sysStandby = locat[locat.Count() - 1] == "";
            if (sysStandby && passchk.Minutes > 30)
            {
                DoorStatusChecking();
            }
        }

        private async Task DoorStatusChecking()
        {
            chkDoorTime = DateTime.Now;
            toolStripDoorChk.BackColor = Color.LightGreen;
            toolStripDoorChk.Text = $"櫃門偵測開始";
            //找本櫃沒壞的磁鎖感應器
            List<SensorDevice> ssrs = (from dev in _db.SensorDevice
                                      join draw in _db.Drawers on dev.TargetObjFid equals draw.FID
                                      where dev.TargetTable == "Drawer" && draw.CbntFid == _AtCbntFid
                                      && string.IsNullOrEmpty(dev.NotWork) && dev.SensorType == "DOORCHK"
                                      orderby draw.No
                                      select dev).ToList();

            List<int> drawfids = ssrs.GroupBy(x => x.TargetObjFid ?? 0).Select(x => x.Key).ToList();
            List<SensorDeviceCtrl> ssrcs = ssrs.Select(x => new SensorDeviceCtrl(x)).ToList();


            bool DoorOpen = false;
            List<bool> DoorOpenList = new List<bool>();
            foreach (int drawfid in drawfids)
            {
                if (drawfid == 0) { continue; }
                string err = "";
                List<SensorDeviceCtrl> list = ssrcs.Where(x => x.TargetObjFid == drawfid).ToList();
                string errmsg = "";
                foreach (SensorDeviceCtrl ssr in list)
                {
                    try
                    {
                        if (setComport(ssr.SerialPort))
                        {
                            string resultcmd = mbus.ExCmd(ssr.genChkCmd());
                            errmsg = resultcmd.StartsWith("ERR") ? resultcmd : "";
                            if (string.IsNullOrEmpty(resultcmd)) { DoorOpenList.Add(resultcmd.Split(" ")[3] == "00"); }
                        }
                        else
                        {
                            errmsg = $"ERR0:{ssr.SerialPort}失敗";
                            break;
                        }
                    }
                    catch (Exception ex) { errmsg = $"錯誤:{ex.Message}"; }
                    await Task.Delay(300);
                }
                Drawers? draw = _db.Drawers.Find(drawfid);
                toolStripDoorChk.Text = $"{draw.No}號櫃門偵測結果:{(errmsg != "" ? errmsg : (DoorOpen ? "開" : "關"))}";
                
                if (errmsg == "" && DoorOpenList.Count() == ssrs.Count())
                {
                    DoorOpen = DoorOpenList.Where(x => x == true).Count() == ssrs.Count();
                    if (draw != null)
                    {
                        draw.DoorStatus = DoorOpen;
                        draw.DoorChkTime = DateTime.Now;
                        _db.Update(draw);
                        _db.SaveChanges();
                    }
                }
                if (!sysStandby) { break; }
                await Task.Delay(400);
            }
            toolStripDoorChk.Text = $"";
            toolStripDoorChk.BackColor = Color.White;
        }


        private bool setComport(string comport)
        {
            try
            {
                return mbus.SetConfig(comport, ConfigurationManager.AppSettings[$"PortName_{comport}"]?.ToString()?.Split(','));
            }
            catch { return false; }
        }
    }
}

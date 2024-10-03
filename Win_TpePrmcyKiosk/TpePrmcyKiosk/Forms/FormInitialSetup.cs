using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk
{
    public partial class FormInitialSetup : Form
    {
        ModbusRtuClient RtuClient = new ModbusRtuClient();
        List<bool> portsstatus = new List<bool>() { false, false, false, false, false, false, false, false };
        public FormInitialSetup()
        {
            InitializeComponent();
        }

        private void FormInitialSetup_Load(object sender, EventArgs e)
        {
            //ConnectModbus();
            List<string> portNames = SerialPort.GetPortNames().ToList();
            if(portNames.Count == 0)
            {
                btn_conection.Enabled = false;
                btn_getweight.Enabled = false;
            }
            else
            {
                btn_conection.Text = "取得連線:" + portNames[0];
                foreach (var port in portNames)
                {


                }
            }
            
        }

        private void btn_conection_Click(object sender, EventArgs e)
        {
            ConnectModbus();
        }


        private void ConnectModbus()
        {
            if (!RtuClient.IsOpen())
            {
                if (!RtuClient.CheckEvent())
                {
                    RtuClient.busRtuResponseEvent += busRtuResponseHandler;
                    RtuClient.busRtuOpenEvent += busRtuOpenHandler;
                    RtuClient.busRtuCloseEvent += busRtuCloseHandler;
                }
                RtuClient.Open();

                if (RtuClient.IsOpen())
                {
                    //test ports enable
                    portsstatus = RtuClient.getPortsStatus();
                }
            }
            else
            {
                RtuClient.Close();
                RtuClient.busRtuResponseEvent -= busRtuResponseHandler;
                RtuClient.busRtuOpenEvent -= busRtuOpenHandler;
                RtuClient.busRtuCloseEvent -= busRtuCloseHandler;
            }
            setAllPanelObjEnable();
        }

        private void setAllPanelObjEnable()
        {
            btn_setInitial.Enabled = RtuClient.IsOpen();
            btn_setweight.Enabled = RtuClient.IsOpen();
            btn_getweight.Enabled = RtuClient.IsOpen();
            btn_conection.Text = RtuClient.IsOpen() ? "連線中" : "斷線..";
            btn_getweight.Text = "取得重量";
            btn_conection.BackColor = RtuClient.IsOpen() ? Color.LightGreen : Color.HotPink;
            if (!RtuClient.IsOpen())
            {
                ckb_port1.Enabled = false;
                ckb_port2.Enabled = false;
                ckb_port3.Enabled = false;
                ckb_port4.Enabled = false;
                ckb_port5.Enabled = false;
                ckb_port6.Enabled = false;
                ckb_port7.Enabled = false;
                ckb_port8.Enabled = false;
            }
            else
            {
                ckb_port1.Enabled = portsstatus[0];
                ckb_port2.Enabled = portsstatus[1];
                ckb_port3.Enabled = portsstatus[2];
                ckb_port4.Enabled = portsstatus[3];
                ckb_port5.Enabled = portsstatus[4];
                ckb_port6.Enabled = portsstatus[5];
                ckb_port7.Enabled = portsstatus[6];
                ckb_port8.Enabled = portsstatus[7];
            }
        }

        #region modbus handler
        private void busRtuResponseHandler(object sender, busRtuResponseArgs e)
        {
            if (!string.IsNullOrEmpty(e.reqCmd))
            {
                txb_cmdlog.AppendText($"{DateTime.Now.ToString("HH:mm:ss")} > {e.resHexString} ");
                if (e.resHexArray[1] == "03")
                {
                    txb_cmdlog.AppendText($"({RtuClient.HexToPrecision00Decimal(e.resHexValue)})");
                    decimal value = RtuClient.HexToPrecision00Decimal(e.resHexValue);
                    string[] cmdarray = e.reqCmd.Split(' ');
                    int reff = RtuClient.HexToInt(cmdarray[2] + cmdarray[3]);
                    if (reff == 80) { lb_weightval1.Text = value.ToString(); }
                    if (reff == 580) { lb_weightval2.Text = value.ToString(); }
                    if (reff == 1080) { lb_weightval3.Text = value.ToString(); }
                    if (reff == 1580) { lb_weightval4.Text = value.ToString(); }
                }
                txb_cmdlog.AppendText($"{Environment.NewLine}");
            }
            if (e.ActName == "Loop Stop")
            {
                btn_getweight.Enabled = true;
                btn_getweight.Text = "取得重量";
                txb_cmdlog.AppendText($" > {e.ActName} {Environment.NewLine}");
            }
        }
        private void busRtuOpenHandler(object sender, busRtuResponseArgs e)
        {
            txb_cmdlog.AppendText($"Open BusRtu > {(e.ActState ? "成功" : "失敗")} {Environment.NewLine}");
            if (!e.ActState)
            {
                RtuClient.busRtuResponseEvent -= busRtuResponseHandler;
                RtuClient.busRtuOpenEvent -= busRtuOpenHandler;
                RtuClient.busRtuCloseEvent -= busRtuCloseHandler;
                setAllPanelObjEnable();
            }
        }
        private void busRtuCloseHandler(object sender, busRtuResponseArgs e)
        {
            txb_cmdlog.AppendText($"Close BusRtu {Environment.NewLine}");
            setAllPanelObjEnable();
        }

        #endregion


        private void btn_getweight_Click(object sender, EventArgs e)
        {
            if (RtuClient.IsOpen())
            {
                btn_getweight.Enabled = false;
                btn_getweight.Text = "執行中";
                RtuClient.ExecuteCmdLoop(5); //單位:秒
            }
        }
        #region 磅秤check
        private void ckb_port1_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(80)} 00 02";
            if (ckb_port1.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port2_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(580)} 00 02";
            if (ckb_port2.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port3_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(1080)} 00 02";
            if (ckb_port3.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port4_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(1580)} 00 02";
            if (ckb_port4.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port5_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(2080)} 00 02";
            if (ckb_port5.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port6_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(2580)} 00 02";
            if (ckb_port6.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port7_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(3080)} 00 02";
            if (ckb_port7.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }

        private void ckb_port8_CheckedChanged(object sender, EventArgs e)
        {
            string cmdstring = $"{txb_devaddr.Text} 03 {RtuClient.toHexWithSpace(3580)} 00 02";
            if (ckb_port8.Checked) { RtuClient.AddCommandInLoop(cmdstring); }
            else { RtuClient.RemoveCommandInLoop(cmdstring); }
        }
        #endregion

        #region setting initial scales
        private async void btn_setInitial_Click(object sender, EventArgs e)
        {

            List<string> cmdlist = new List<string>();
            if (ckb_port1.Checked) { cmdlist.AddRange(getCmdStringList(0, "initial")); }
            if (ckb_port2.Checked) { cmdlist.AddRange(getCmdStringList(500, "initial")); }
            if (ckb_port3.Checked) { cmdlist.AddRange(getCmdStringList(1000, "initial")); }
            if (ckb_port4.Checked) { cmdlist.AddRange(getCmdStringList(1500, "initial")); }
            if (ckb_port5.Checked) { cmdlist.AddRange(getCmdStringList(2000, "initial")); }
            if (ckb_port6.Checked) { cmdlist.AddRange(getCmdStringList(2500, "initial")); }
            if (ckb_port7.Checked) { cmdlist.AddRange(getCmdStringList(3000, "initial")); }
            if (ckb_port8.Checked) { cmdlist.AddRange(getCmdStringList(3500, "initial")); }

            RtuClient.AddCommandInLoop(cmdlist);
            RtuClient.ExecuteCmdList();
            await Task.Delay(cmdlist.Count * 200);
            RtuClient.RemoveCommandInLoop(cmdlist);
        }

        private async void btn_setempty_Click(object sender, EventArgs e)
        {

            List<string> cmdlist = new List<string>();
            if (ckb_port1.Checked) { cmdlist.AddRange(getCmdStringList(0, "empty")); }
            if (ckb_port2.Checked) { cmdlist.AddRange(getCmdStringList(500, "empty")); }
            if (ckb_port3.Checked) { cmdlist.AddRange(getCmdStringList(1000, "empty")); }
            if (ckb_port4.Checked) { cmdlist.AddRange(getCmdStringList(1500, "empty")); }
            if (ckb_port5.Checked) { cmdlist.AddRange(getCmdStringList(2000, "empty")); }
            if (ckb_port6.Checked) { cmdlist.AddRange(getCmdStringList(2500, "empty")); }
            if (ckb_port7.Checked) { cmdlist.AddRange(getCmdStringList(3000, "empty")); }
            if (ckb_port8.Checked) { cmdlist.AddRange(getCmdStringList(3500, "empty")); }


            RtuClient.AddCommandInLoop(cmdlist);
            RtuClient.ExecuteCmdList();
            await Task.Delay(cmdlist.Count * 200);
            RtuClient.RemoveCommandInLoop(cmdlist);
        }

        private async void btn_setweight_Click(object sender, EventArgs e)
        {

            List<string> cmdlist = new List<string>();
            if (ckb_port1.Checked) { cmdlist.AddRange(getCmdStringList(0, "weight")); }
            if (ckb_port2.Checked) { cmdlist.AddRange(getCmdStringList(500, "weight")); }
            if (ckb_port3.Checked) { cmdlist.AddRange(getCmdStringList(1000, "weight")); }
            if (ckb_port4.Checked) { cmdlist.AddRange(getCmdStringList(1500, "weight")); }
            if (ckb_port5.Checked) { cmdlist.AddRange(getCmdStringList(2000, "weight")); }
            if (ckb_port6.Checked) { cmdlist.AddRange(getCmdStringList(2500, "weight")); }
            if (ckb_port7.Checked) { cmdlist.AddRange(getCmdStringList(3000, "weight")); }
            if (ckb_port8.Checked) { cmdlist.AddRange(getCmdStringList(3500, "weight")); }


            RtuClient.AddCommandInLoop(cmdlist);
            RtuClient.ExecuteCmdList();
            await Task.Delay(cmdlist.Count * 200);
            RtuClient.RemoveCommandInLoop(cmdlist);
        }

        private List<string> getCmdStringList(int ChannelCode, string settype)
        {
            List<string> cmdlist = new List<string>();
            if (settype == "initial")
            {
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 32)} 00 01 02 00 03"); //AD轉換速度
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 34)} 00 01 02 00 01"); //濾波類型(均值)
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 35)} 00 01 02 00 32"); //濾波強度(最強)
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 88)} 00 01 02 00 06"); //精準度/分度值
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 86)} 00 02 04 00 07 A1 20"); //最大重量
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 38)} 00 02 04 00 00 00 00"); //標定空載寫0
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 93)} 00 01 02 00 0A"); //清零範圍
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 96)} 00 01 02 00 0A"); //自動零位跟踪範圍
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 97)} 00 01 02 00 0A"); //自動零位跟踪時間
            }
            if (settype == "weight")
            {
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 42)} 00 02 04 00 00 27 10"); //增益標定(有法碼)
            }
            if (settype == "empty")
            {
                cmdlist.Add($"{txb_devaddr.Text} 10 {RtuClient.toHexWithSpace(ChannelCode + 94)} 00 01 02 00 01"); //清零
            }
            return cmdlist;
        }
        #endregion

        private void FormInitialSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            RtuClient.Close();
            Thread.Sleep(200);
        }
    }
}

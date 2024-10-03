using HslCommunication;
using HslCommunication.ModBus;
using HslCommunication.Serial;
using System;
using System.Configuration;
using System.IO.Ports;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk
{
    public partial class FormLEDStripTest : Form
    {
        private MainForm _parent;
        ModbusRtu busRtuClient = new ModbusRtu();
        List<int> rtuConfig = new List<int>() { 115200, 8, 1, 0 };
        string rtuCom = "";
        bool runLED = false;


        public FormLEDStripTest(MainForm parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void FormLEDStripTest_Load(object sender, EventArgs e)
        {
            int num = 0;
            while (false)
            {
                if (num == 30) { num = 0; }
                int colorno = num / 6;
                SendingCMD(getCMDstr(30, new List<int> { num, num-1, num +1 }, "wit"));
                num++;
                Thread.Sleep(500);
            }
            //SendingCMD(getCMDstr(30, new List<int> {1,3,8,9,11,15,18,19,20,22,25,26 }, "blu"));

        }

        private string getCMDstr(int len, List<int> onPos, string color)
        {
            try
            {
                string colors = "";
                for (int i = 0; i < len; i++)
                {
                    if (onPos.Contains(i)) { colors += $"{getColorCode(color)} "; }
                    else { colors += $"{getColorCode("")} "; }
                }

                string address = "00 00"; //組地址(3~4)
                string equipt = "00 01"; //設備地址(5~6)
                string datalength = $"{(len * 3).ToString("X4").Substring(0, 2)} {(len * 3).ToString("X4").Substring(2, 2)}"; //數據長度(12~13) = N組*3
                string repeattime = "00 01"; //擴展次數(14~15)
                string addfixedpartial = $"DD 55 EE {address} {equipt} 00 99 01 00 00 {datalength} {repeattime} {colors}AA BB";
                return addfixedpartial;
            }
            catch (Exception ex) { return ""; }
        }

        private string getColorCode(string colorName)
        {
            string colorCode = "";
            switch (colorName)
            {
                case "wit": colorCode = "FF FF FF"; break;
                case "red": colorCode = "60 00 00"; break;
                case "org": colorCode = "70 10 00"; break;
                case "ylw": colorCode = "60 50 00"; break;
                case "grn": colorCode = "00 50 00"; break;
                case "blu": colorCode = "00 00 50"; break;
                case "ppl": colorCode = "50 00 50"; break;
                default: colorCode = "00 00 00"; break;
            }
            return colorCode;
        }

        private void SendingCMD(string cmd)
        {
            if (OpenRtu())
            {
                try
                {
                    OperateResult<byte[]> read =
                        busRtuClient.ReadFromCoreServer(SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes(cmd)), true, false);

                    if (read.IsSuccess)
                    {
                        string result = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content, ' ');
                        txtReciveData.AppendText(cmd + " > " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content, ' ') + Environment.NewLine);
                    }
                    else
                    {
                        qwFunc.Alert("回覆失敗：" + read.ToMessageShowString());
                    }
                }
                catch (Exception ex)
                {
                    qwFunc.Alert("執行錯誤：" + ex.Message);
                }
                if (busRtuClient.IsOpen()) { busRtuClient.Close(); }
            }
            else
            {
                qwFunc.Alert("設備連線失敗");
            }

        }


        private bool OpenRtu()
        {
            if (!busRtuClient.IsOpen())
            {
                int stopBits = rtuConfig[2];
                int parity = rtuConfig[3];

                busRtuClient.SerialPortInni(sp =>
                {
                    sp.PortName = $"COM{txt_com.Text}";
                    sp.BaudRate = rtuConfig[0];
                    sp.DataBits = rtuConfig[1];
                    sp.StopBits = stopBits == 0 ? StopBits.None : (stopBits == 1 ? StopBits.One : StopBits.Two);
                    sp.Parity = parity == 0 ? Parity.None : (parity == 1 ? Parity.Odd : Parity.Even);
                });
                busRtuClient.RtsEnable = true;
                busRtuClient.Open();

            }
            else
            {
                busRtuClient.Close();
            }
            return busRtuClient.IsOpen();
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if(txt_LEDcnt.Text == "") { return; }

            try
            {
                int LEDlength = Convert.ToInt32(txt_LEDcnt.Text);
                List<int> onIndex = new List<int>();
                if (txt_OnIndex.Text == "")
                {
                    for(int i = 0; i<LEDlength; i++) { onIndex.Add(i); }
                }
                else
                {
                    onIndex = txt_OnIndex.Text.Split(",").ToList().Select(x => Convert.ToInt32(x)).ToList();
                }
                string color = cb_colorpicker.Text == "" ? "" : cb_colorpicker.Text.Split(':')[1];

                SendingCMD(getCMDstr(LEDlength, onIndex, color));
            }
            catch (Exception ex)
            {
                txtReciveData.AppendText($"設定錯誤：{ex}" + Environment.NewLine);
            }
        }
    }
}

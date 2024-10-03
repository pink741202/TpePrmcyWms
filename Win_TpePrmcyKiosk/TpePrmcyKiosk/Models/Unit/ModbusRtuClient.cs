using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.ModBus;
using HslCommunication.Serial;
using System;
using System.Configuration;
using System.IO.Ports;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;

namespace TpePrmcyKiosk.Models.Unit
{
    public delegate void ModbusRtuResponseEventHandler(Object sender, busRtuResponseArgs e);


    public class busRtuResponseArgs : EventArgs
    {
        public bool ActState = false; //動作是否成功
        public string ActName = "";
        public DateTime ActTime = DateTime.Now;
        public string ComPort { get; set; }
        public string reqCmd = "";
        public string resHexString = "";
        public List<string> resHexArray = new List<string>();
        public string resHexValue = "";
        public string resMessage = "";
        

        public busRtuResponseArgs()
        {
            List<string> portNames = SerialPort.GetPortNames().ToList();
            if(portNames.Count > 0)
            {
                ComPort = ConfigurationManager.AppSettings["COMPORT"] ??= portNames[0];
            }
            
        }
    }

    public class ModbusRtuClient
    {
        private ModbusRtu busRtuClient = new ModbusRtu();

        public event ModbusRtuResponseEventHandler busRtuResponseEvent = null;
        public event ModbusRtuResponseEventHandler busRtuOpenEvent = null;
        public event ModbusRtuResponseEventHandler busRtuCloseEvent = null;

        busRtuResponseArgs args = new busRtuResponseArgs();
        private List<string> CmdList = new List<string>();
        private bool ActCmdLoop = false; //是否持繼取得數據
        private bool SendResponse = false; //是否回傳數據

        #region execute command list loop
        private async void CmdLoop(int times) //取得數據迴圈
        {
            ActCmdLoop = true;
            bool doInvoke = false; //該次foreach是否輸出
            while (ActCmdLoop && busRtuClient.IsOpen()) 
            {
                doInvoke = SendResponse || times == 1; //時間到,這次foreach要輸出 or 只跑一圈
                SendResponse = false; //reset輸出值給 回傳數據迴圈
                for (int x = 0; x < CmdList.Count; x++)
                {
                    try
                    {
                        args = ExcuteCmd(CmdList[x]);
                    }
                    catch (Exception ex)
                    {
                        args = new busRtuResponseArgs();
                        args.resMessage = ex.Message;
                    }
                    if (busRtuResponseEvent != null && doInvoke)
                    {
                        busRtuResponseEvent.Invoke(this, args);
                    }
                    await Task.Delay(10);
                }                
                doInvoke = false; //reset輸出值
                await Task.Delay(300);
                if (times == 1) { ActCmdLoop = false; } //只跑一圈
            }
            if (busRtuResponseEvent != null)
            {
                args = new busRtuResponseArgs();
                args.ActName = "Loop Stop";
                busRtuResponseEvent.Invoke(this, args);
            }
        }
        private async void SetReturnLoopResult(int stopmillonsec) //回傳數據迴圈
        {
            ActCmdLoop = true;
            SendResponse = false;
            await Task.Delay(500); 
            SendResponse = true;
            while (ActCmdLoop && busRtuClient.IsOpen())
            {
                while (ActCmdLoop && SendResponse) //輸出中,等待下一個foreach的開始
                {
                    await Task.Delay(100);
                }
                for(int t=0;t< stopmillonsec / 1000; t++)  //間隔時間 切為每秒判斷一次是否有中斷
                {
                    if (!(ActCmdLoop && busRtuClient.IsOpen()))
                    {
                        break;
                    }
                    await Task.Delay(1000);
                }
                if(ActCmdLoop && busRtuClient.IsOpen()) { SendResponse = true; }
            }
        }
        #endregion

        #region Open Serial port
        public void Open()
        {            
            string BOUDRATE = ConfigurationManager.AppSettings["BOUDRATE"] ??= "9600";
            string DATABITS = ConfigurationManager.AppSettings["DATABITS"] ??= "8";
            string STOPBITS = ConfigurationManager.AppSettings["STOPBITS"] ??= "1";
            string PARITY = ConfigurationManager.AppSettings["PARITY"] ??= "0";
            busRtuResponseArgs args = new busRtuResponseArgs();
            args.ActName = "open";
            if (!busRtuClient.IsOpen())
            {
                int stopBits = Convert.ToInt16(STOPBITS);
                int parity = Convert.ToInt16(PARITY);

                busRtuClient.SerialPortInni(sp =>
                {
                    sp.PortName = args.ComPort;
                    sp.BaudRate = Convert.ToInt16(BOUDRATE);
                    sp.DataBits = Convert.ToInt16(DATABITS);
                    sp.StopBits = stopBits == 0 ? StopBits.None : (stopBits == 1 ? StopBits.One : StopBits.Two);
                    sp.Parity = parity == 0 ? Parity.None : (parity == 1 ? Parity.Odd : Parity.Even);
                });
                busRtuClient.RtsEnable = true;
                busRtuClient.Open();
            }
            
            args.ActState = busRtuClient.IsOpen();
            busRtuClient.RtsEnable = busRtuClient.IsOpen();

            if (busRtuOpenEvent != null)
            {
                busRtuOpenEvent.Invoke(this, args);
            }
        }
        
        #endregion

        #region Close serial port
        public void Close()
        {
            if (busRtuClient.IsOpen())
            {
                busRtuClient.Close();
            }
            busRtuResponseArgs args = new busRtuResponseArgs();
            args.ActState = !busRtuClient.IsOpen();
            args.ActName = "close";

            if (busRtuCloseEvent != null)
            {
                busRtuCloseEvent.Invoke(this, args);
            }
        }
        #endregion

        #region serial port open status
        public bool IsOpen()
        {
            return busRtuClient.IsOpen();
        }
        #endregion

        #region get 8 ports status (ONLY FOR AUTODA DEVICE)
        public List<bool> getPortsStatus()
        {
            List<bool> result = (new bool[] { false,false,false,false,false,false,false,false }).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = ExcuteCmd($"01 03 {toHexWithSpace(500 * i + 80)} 00 02").ActState;
            }
            return result;
        }
        #endregion

        #region excute single command
        public busRtuResponseArgs ExcuteCmd(string cmdstring)
        {
            busRtuResponseArgs res = new busRtuResponseArgs();
            res.reqCmd = cmdstring;
            switch (cmdstring.Split(' ')[1])
            {
                case "03": res.ActName = "r"; break;
                case "10": res.ActName = "w"; break;
            }

            if (!busRtuClient.IsOpen()) { res.resMessage = "busRtu Close"; return res; }

            OperateResult<byte[]> read =
                        busRtuClient.ReadFromCoreServer(SoftCRC16.CRC16(SoftBasic.HexStringToBytes(cmdstring)), true, false);

            res.ActState = read.IsSuccess;
            if (read.IsSuccess)
            {
                res.resHexString = SoftBasic.ByteToHexString(read.Content, ' ');
                res.resHexArray = res.resHexString.Split(' ').ToList();

                if (res.resHexArray[1] == "03")
                {
                    int valuecnt = HexToInt(res.resHexArray[2]);
                    for (int i = 3; i < valuecnt + 3; i++)
                    {
                        res.resHexValue += res.resHexArray[i];
                    }
                }

                if (res.resHexArray[1] == "82" || res.resHexArray[1] == "83")
                {
                    res.ActState = false;
                    res.resMessage = "response error";
                }
            }
            else
            {
                res.resMessage = "read error";
            }
            return res;
        }
        #endregion

        #region autoda modbus 用,16進轉10進,精度為0.00
        public decimal HexToPrecision00Decimal(string hex)
        {
            int getIntValue = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return (decimal)getIntValue / 100;
        }
        public int HexToInt(string hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
        #endregion

        #region int to hex string "00 00"
        public string toHexWithSpace(int val)
        {
            string hex = val.ToString("X4");
            return $"{hex.Substring(0, 2)} {hex.Substring(2, 2)}";
        }
        #endregion

        #region edit command list
        public void AddCommandInLoop(string cmdstring) 
        {            
            CmdList.Add(cmdstring);
        }
        public void RemoveCommandInLoop(string cmdstring) 
        {             
            CmdList.Remove(cmdstring);
        }
        public void AddCommandInLoop(List<string> cmdstring)
        {
            CmdList.AddRange(cmdstring);
        }
        public void RemoveCommandInLoop(List<string> cmdstring)
        {
            foreach(string cmd in cmdstring)
            {
                CmdList.Remove(cmd);
            }            
        }
        public void CleanCommandInLoop()
        {
            CmdList.Clear();
        }
        #endregion

        #region act command list loop
        public void ExecuteCmdLoop(int setseconds)
        {
            CmdLoop(0);
            SetReturnLoopResult(setseconds * 1000);
        }
        public void StopCmdLoop()
        {
            ActCmdLoop = false;
        }
        public void ExecuteCmdList()
        {
            CmdLoop(1);
        }
        #endregion

        public bool CheckEvent()
        {
            return busRtuResponseEvent != null;
        }
    }
}

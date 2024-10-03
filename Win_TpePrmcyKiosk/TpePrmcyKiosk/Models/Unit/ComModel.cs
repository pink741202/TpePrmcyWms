using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace TpePrmcyKiosk.Models.Unit
{
    public delegate void SerialPortEventHandler(Object sender, SerialPortEventArgs e);

    public class SerialPortEventArgs : EventArgs
    {
        public bool isOpen = false;
        public string theComPort = "";
        public Byte[] receivedBytes = null;
        public string EncodedString = null;
        public string LineString = null;
        public string ActName = "";
    }

    public class ComModel
    {
        private SerialPort sp = new SerialPort();

        public event SerialPortEventHandler comReceiveDataEvent = null;
        public event SerialPortEventHandler comOpenEvent = null;
        public event SerialPortEventHandler comCloseEvent = null;

        private Object thisLock = new Object();

        SerialPortEventArgs args = new SerialPortEventArgs();
        #region setting param
        private string NewLineChar = "";
        private string ReceiveLineBuff = "";
        private Encoding encode = null;
        #endregion

        #region When serial received data, will call this method
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {            
            if (sp.BytesToRead <= 0)
            {
                return;
            }

            lock (thisLock)
            {
                int len = sp.BytesToRead;
                Byte[] byt_data = new Byte[len];
                try
                {
                    sp.Read(byt_data, 0, len);
                }
                catch (System.Exception)
                {
                    //catch read exception
                }
                                
                args.receivedBytes = byt_data;
                args.EncodedString = encode == null ? Encoding.Default.GetString(byt_data) : encode.GetString(byt_data);
                args.LineString = "";
                args.isOpen = true;
                args.ActName = "Receive";

                if(NewLineChar != "" && encode != null)
                {
                    ReceiveLineBuff += args.EncodedString;
                    int NewLinePosition = ReceiveLineBuff.LastIndexOf(NewLineChar);
                    if (NewLinePosition > 0)
                    {
                        int NewLineCharCnt = NewLineChar.Length;
                        string aLine = ReceiveLineBuff.Substring(0, NewLinePosition + NewLineCharCnt); //取到最後的換行 = 一行
                        ReceiveLineBuff = ReceiveLineBuff.Substring(NewLinePosition + NewLineCharCnt); //清buffer到最後的換行
                        args.LineString = aLine.Split(NewLineChar)[aLine.Split(NewLineChar).Length - NewLineCharCnt]; //切成陣列取最後一行

                    }
                }

                if (comReceiveDataEvent != null)
                {
                    comReceiveDataEvent.Invoke(this, args);
                }
            }
        }
        #endregion
        
        #region Open Serial port
        public void Open(string portName, int baudRate, int dataBits, string stopBits, string parity, string handshake)
        {
            args.theComPort = portName;
            args.ActName = "Open";
            if (portName != null && portName != "")
            {
                if (sp.IsOpen)
                {
                    Close();
                }
                sp.PortName = portName;
                sp.BaudRate = Convert.ToInt32(baudRate);
                sp.DataBits = Convert.ToInt16(dataBits);

                if (handshake == "None")
                {
                    //Never delete this property
                    sp.RtsEnable = true;
                    sp.DtrEnable = true;
                }
                
                try
                {
                    sp.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopBits);
                    sp.Parity = (Parity)Enum.Parse(typeof(Parity), parity);
                    sp.Handshake = (Handshake)Enum.Parse(typeof(Handshake), handshake);
                    sp.WriteTimeout = 1000; /*Write time out*/
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                    args.isOpen = true;
                }
                catch (System.Exception)
                {
                    args.isOpen = false;
                }
                if (comOpenEvent != null)
                {
                    comOpenEvent.Invoke(this, args);
                }
            }
        }
                
        
        #endregion

        #region Close serial port
        public void Close()
        {
            Thread closeThread = new Thread(new ThreadStart(CloseSpThread));
            closeThread.Start();
        }

        private void CloseSpThread()
        {            
            args.isOpen = false;
            args.ActName = "Close";
            try
            {
                sp.Close(); //close the serial port
                sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
            }
            catch (Exception)
            {
                args.isOpen = true;
            }
            if (comCloseEvent != null)
            {
                comCloseEvent.Invoke(this, args);
            }

        }
        #endregion

        #region serial port open status
        public bool PortStatus()
        {
            return sp.IsOpen;
        }
        #endregion

        #region Hex to byte
        private static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                try
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
                catch (System.Exception)
                {
                    //Do Nothing
                }

            }
            return raw;
        }
        #endregion

        #region Hex string to bytes
        public static Byte[] Hex2Bytes(String hex)
        {
            return FromHex(hex);
        }
        #endregion

        #region Bytes to Hex String
        public static String Bytes2Hex(Byte[] bytes)
        {
            return BitConverter.ToString(bytes);
        }
        #endregion

        #region send to serial port
        public bool SendData(Byte[] data)
        {
            if (!sp.IsOpen)
            {
                return false;
            }

            try
            {
                sp.Write(data, 0, data.Length);
            }
            catch (System.Exception)
            {
                return false;   //write failed
            }
            return true;        //write successfully
        }

        public bool SendData(string data)
        {
            if (!sp.IsOpen)
            {
                return false;
            }

            try
            {
                sp.Write(data);
            }
            catch (System.Exception)
            {
                return false;   //write failed
            }
            return true;        //write successfully
        }
        #endregion

        #region setting param
        public void SetNewLineChar(Encoding setEncode, string setChar)
        {
            NewLineChar = setChar;
            encode = setEncode;
        }
        public void SetEncode(Encoding setEncode)
        {
            encode = setEncode;
        }
        #endregion
    }
}

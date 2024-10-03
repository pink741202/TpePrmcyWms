using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.ModBus;
using HslCommunication.Serial;
using System.IO.Ports;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk.Models.Service
{
    public class MbusExcServ
    {
        private ModbusRtu MbusRtu = new ModbusRtu();

        public MbusExcServ()
        {


        }
        public bool SetConfig(string PortName, string[] config)
        {
            try
            {
                if (PortName != MbusRtu.PortName)
                {
                    MbusRtu.SerialPortInni(sp =>
                    {
                        sp.PortName = PortName;
                        sp.BaudRate = Convert.ToInt32(config[0]);
                        sp.DataBits = Convert.ToInt16(config[1]);
                        sp.StopBits = (StopBits)Convert.ToInt16(config[2]);
                        sp.Parity = (Parity)Convert.ToInt16(config[3]);
                    });
                    MbusRtu.RtsEnable = true;
                }
                return true;
            }
            catch (Exception ex) { qwFunc.savelog($"設定MbusRtu錯誤：{ex.Message}"); return false; }
        }
        public string ExCmd(string cmd)
        {
            //ERR1 = 執行錯誤
            //ERR2 = 讀寫失敗
            //ERR3 = 指令錯誤
            string result = "";
            if (string.IsNullOrEmpty(cmd)) { result = "ERR3:指令錯誤"; return result; }
            try
            {
                MbusRtu.Open();                
                OperateResult<byte[]> read =
                        MbusRtu.ReadFromCoreServer(SoftCRC16.CRC16(SoftBasic.HexStringToBytes(cmd)), true, false);
                if (read.IsSuccess)
                {
                    result = SoftBasic.ByteToHexString(read.Content, ' ');
                    if ((new List<string> { "53", "54" }).Contains(result.Split(" ")[0])
                        ||
                        (new List<string> { "82", "83" }).Contains(result.Split(" ")[1])
                        ) 
                    { result = "ERR3:指令錯誤"; }
                }
                else { result = "ERR2:讀寫失敗"; }
            }
            catch (Exception ex) { qwFunc.savelog($"執行cmd[{cmd}]錯誤：{ex.Message}"); result = "ERR1:執行錯誤"; }
            finally { MbusRtu.Close(); }
            return result;
        }

        #region int to hex string "00 00"
        public string toHexWithSpace(int val)
        {
            string hex = val.ToString("X4");
            return $"{hex.Substring(0, 2)} {hex.Substring(2, 2)}";
        }        
        #endregion
        #region autoda modbus 用,16進轉10進,精度為0.00
        public decimal HexToPrecision00Decimal(string hex)
        {
            int getIntValue = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return (decimal)getIntValue / 100;
        }
        public int HexToInt(string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
        #endregion
        #region 換算單位數量
        public bool CalculateWeightToQty(
            decimal Weight, decimal? UnitW, decimal? UnitQ,
            out decimal ResultQty, out decimal ResultTolerance, out bool ResultTrustable,
            out string msg
            )
        {
            ResultQty = 0;
            ResultTolerance = 0;
            ResultTrustable = false;
            msg = string.Empty;
            int pointnum = 2; //取到小數位

            Weight = Math.Round(Weight, pointnum) * -1;
            int TolerancePercent = 5;
            double Dividend = (double)Weight; //被除數
            double Divisor = (double)UnitW; //除數
            double EachQ = (double)UnitQ; //乘數
            double TakeOrPut = Dividend == 0 ? 1 : Dividend / Math.Abs(Dividend);

            try
            {
                double Quotient = Math.Floor(Math.Abs(Dividend) / Divisor) * TakeOrPut; //商數
                double Remainder = Math.Abs(Dividend) % Divisor; //餘數(差)

                //餘數如果跟單顆重差5%以上 = 重量不準
                double RemainderPersent = Math.Round(Remainder / Divisor * 100, 2);
                if (RemainderPersent > 80) { Quotient += TakeOrPut; } //當做多一顆

                ResultQty = Convert.ToDecimal(Quotient * EachQ);
                ResultTolerance = Convert.ToDecimal(RemainderPersent);
                ResultTrustable = RemainderPersent > 100 - TolerancePercent || RemainderPersent < TolerancePercent;

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            return true;
        }
        #endregion
        
    }


}

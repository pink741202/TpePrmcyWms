using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TpePrmcyWms.Models.DOM;

namespace TpePrmcyKiosk.Models.Unit
{
    public class SensorDeviceCtrl
    {
        public int FID { get; set; }
        public string TargetTable { get; set; } = "";
        public int TargetObjFid { get; set; }
        public int? DrawerNo { get; set; }
        public string? DrugCode { get; set; } = "";
        public string SensorType { get; set; } = "";
        public string? SensorVersion { get; set; }
        public string SensorNo { get; set; } = "";
        public string SerialPort { get; set; } = "";
        public int Modbus_Addr { get; set; } = 0;
        public int Modbus_Rgst { get; set; } = 0;
        public string Modbus_Cmd { get; set; } = "";
        public string? NotWork { get; set; } = "";
        public decimal? UnitWeight { get; set; } //單位重
        public decimal? UnitQty { get; set; } //單位數量
        public decimal? WeighWeight { get; set; } //磅秤結果,現重
        public decimal? WeighWeight0 { get; set; } //磅秤結果,初重
        public decimal? WeighQty { get; set; } //磅秤結果,數量

        public SensorDeviceCtrl()
        {

        }
        public SensorDeviceCtrl(SensorDevice source)
        {
            FID = source.FID;
            TargetTable = source.TargetTable ?? "";
            TargetObjFid = source.TargetObjFid ?? 0;
            SensorType = source.SensorType ?? "";
            SensorVersion = source.SensorVersion;
            SensorNo = source.SensorNo ?? "";
            SerialPort = source.SerialPort ?? "";
            Modbus_Addr = source.Modbus_Addr ?? 0;
            Modbus_Rgst = source.Modbus_Rgst ?? 0;
            Modbus_Cmd = source.Modbus_Cmd ?? "";
            NotWork = source.NotWork;
        }

        public string genLitCmd(string color, bool turnOn)
        {
            if (SensorType != "LED") { return ""; }
            string result = "";
            List<string> ver = SensorVersion?.Split(":").ToList() ?? new List<string>();
            if (ver.Contains("V0"))
            {
                
            }
            if (ver.Contains("V1"))
            {
                if (ver.Contains("Strip"))
                {
                    try
                    {
                        int len = Convert.ToInt32(Modbus_Cmd.Split(":")[0]);
                        List<int> onPos = Modbus_Cmd.Split(":")[1].Split(",").Select(x => Convert.ToInt32(x)).ToList();
                        string colors = "";
                        color = turnOn ? color : "";
                        for (int i = 0; i < len; i++)
                        {
                            if (onPos.Contains(i)) { colors += $"{getV1LedStripColorCode(color)} "; }
                            else { colors += $"{getV1LedStripColorCode("")} "; }
                        }
                        string address = $"{qwFunc.toHexWithSpace(Modbus_Addr)}"; //組地址(3~4) 0開始
                        string equipt = $"{qwFunc.toHexWithSpace(Modbus_Rgst)}"; //設備地址(5~6) 1開始
                        string datalength = $"{qwFunc.toHexWithSpace(len * 3)}"; //數據長度(12~13) = N組*3
                        string repeattime = "00 01"; //擴展次數(14~15)
                        string addfixedpartial = $"DD 55 EE {address} {equipt} 00 99 01 00 00 {datalength} {repeattime} {colors}AA BB";
                        return addfixedpartial;
                    }
                    catch (Exception ex) { return ""; }
                }
            }
            

            return result;
        }

        public string genLockCmd(bool turnOn)
        {
            if (SensorType != "LOCK") { return ""; }
            string result = "";
            List<string> ver = SensorVersion?.Split(":").ToList() ?? new List<string>();
            if (ver.Contains("V0"))
            {
                if (!ver.Contains("CMD"))
                {
                    return $"{Modbus_Addr.ToString("X2")} 05 {qwFunc.toHexWithSpace(Modbus_Rgst + 13056)} {(turnOn ? "FF" : "00")} 00";
                }
                else
                {
                    return Modbus_Cmd;
                }
            }
            
            return result;
        }

        public string genChkCmd()
        {
            if (SensorType != "DOORCHK") { return ""; }
            List<string> ver = SensorVersion?.Split(":").ToList() ?? new List<string>();

            if (ver.Contains("V0"))
            {
                if (!ver.Contains("CMD"))
                {
                    return $"{Modbus_Addr.ToString("X2")} 02 {qwFunc.toHexWithSpace(Modbus_Rgst + 13312)} 00 02";
                }
                else
                {
                    return Modbus_Cmd;
                }
            }
            
            return "";
        }

        public string genScaleCmd(string type)
        {
            if (SensorType != "SCALE") { return ""; }
            if (type == "clean")
            {
                return $"{Modbus_Addr.ToString("X2")} 10 {qwFunc.toHexWithSpace((Modbus_Rgst) * 500 + 94)} 00 01 02 00 01";
                
            }
            if (type == "data")
            {
                return $"{Modbus_Addr.ToString("X2")} 03 {qwFunc.toHexWithSpace((Modbus_Rgst) * 500 + 80)} 00 02";

            }

            return "";
        }

        private string getV1LedStripColorCode(string colorName)
        {
            string colorCode = "";
            switch (colorName)
            {
                case "White": colorCode = "FF FF FF"; break;
                case "Red": colorCode = "60 00 00"; break;
                case "Orange": colorCode = "70 10 00"; break;
                case "Yellow": colorCode = "60 50 00"; break;
                case "Green": colorCode = "00 50 00"; break;
                case "Blue": colorCode = "00 00 50"; break;
                case "Purple": colorCode = "50 00 50"; break;
                default: colorCode = "00 00 00"; break;
            }
            return colorCode;
        }

    }


    


}

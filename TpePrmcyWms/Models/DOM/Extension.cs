using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;

namespace TpePrmcyWms.Models.DOM
{
    public static class Extension
    {
        public static PrscptBill map(this PrscptBill obj, string[] data) {
            obj.Pharmarcy = data[0] == "" ? "" : data[0];
            obj.PrscptNo = data[1];
            obj.PrscptDate = data[2] != "" ? qwServ.EraStringToDate(data[2]) : null;
            obj.DrugCode = data[3];
            obj.DrugName = data[4];
            obj.CtrlDrugGrand = data[5];
            obj.PatientNo = data[6];
            obj.PatientSeq = data[7];
            obj.OrderSeq = data[8] != "" ? Convert.ToDecimal(data[8]) : 0;
            obj.PatientName = data[9];
            obj.TtlQty = data[10] != "" ? Convert.ToDecimal(data[10]) : null;
            obj.PriceUnit = data[11];
            obj.DrName = data[12];
            obj.BedCode = data[13];
            obj.DrugDose = data[14];
            obj.DrugFrequency = data[15];
            obj.DrugDays = data[16];
            obj.DoneFill = false;
            obj.ScanTime = 1;
            obj.HISchk = null;
            return obj;
        }
    }
}

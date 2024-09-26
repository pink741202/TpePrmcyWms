using Microsoft.AspNetCore.Http;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Unit.Back;
using TpePrmcyWms.Models.Unit.Report;

namespace TpePrmcyWms.Models.Service
{
    public class ReportService
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        public IQueryable<StockingLog> queryStockingLog(DateTime qmoddate1, DateTime qmoddate2)
        {
            IQueryable<StockingLog> obj = (from bill in _db.StockBill
                                           join emp in _db.employee on bill.modid equals emp.FID
                                           join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                                           join draw in _db.Drawers on bill.DrawFid equals draw.FID
                                           join mnlf in _db.MenuLeft on bill.BillType equals mnlf.OperCode
                                           where bill.moddate >= qmoddate1.Date && bill.moddate <= qmoddate2.Date
                                           select new StockingLog
                                           {
                                               stockBillFid = bill.FID,
                                               operTime = bill.moddate ?? DateTime.Now,
                                               billType = bill.BillType, 
                                               billTypeName = mnlf.CatelogName, 
                                               empName = emp.name,
                                               empNo = emp.emp_no ?? "",
                                               CbntFid = cbnt.FID,
                                               CbntName = cbnt.CbntName,
                                               DrawNo = draw.No.ToString(),                                               
                                               QtyIn = bill.TradeType ? bill.Qty : 0,
                                               QtyOut = bill.TradeType ? 0 : bill.Qty,
                                               QtyBefore = bill.SysChkQty + (bill.Qty * (bill.TradeType ? -1 : 1)),
                                               UserChk1 = bill.UserChk1Qty,
                                               UserChk2 = bill.UserChk2Qty,
                                               UserChkErr = !(bill.UserChk1Qty == null || bill.SysChkQty == bill.UserChk1Qty || bill.SysChkQty == bill.UserChk2Qty) ,
                                               SysChkQty = bill.SysChkQty,
                                               OperFinish = true,
                                               DrugCode = bill.DrugCode,
                                               DrugName = bill.DrugName.Trim(),
                                               OperQty = bill.Qty,
                                               FromFid = bill.FromFid,
                                           });

            foreach(var item in obj)
            {
                List<string> transCode = new List<string>() { "TG1", "TG2", "TI1", "TI2" };
                if (item.FromFid != null && transCode.Contains(item.billType)) {
                    string? fromCbntName = (from fbill in _db.StockBill
                                   join cbnt in _db.Cabinet on fbill.CbntFid equals cbnt.FID
                                   where fbill.FID == item.FromFid
                                   select cbnt.CbntName).FirstOrDefault();
                    if (item.billType.EndsWith("1"))
                    {
                        item.TrgtCbntFid = item.FromFid;
                        item.TrgtCbntName = fromCbntName;
                    }
                    if (item.billType.EndsWith("2"))
                    {
                        item.FromCbntFid = item.FromFid;
                        item.FromCbntName = fromCbntName;
                    }
                }



            }

            return obj;
        }

         
    }
}

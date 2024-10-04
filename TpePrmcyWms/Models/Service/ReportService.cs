using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System.Xml.Linq;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Unit.Back;
using TpePrmcyWms.Models.Unit.Report;

namespace TpePrmcyWms.Models.Service
{
    public class ReportService
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        public IQueryable<StockingLog> queryStockingLog(string? qKeyString, DateTime? qmoddate1, DateTime? qmoddate2, int? qCbnt, string qDrawFid)
        {
            var query = from bill in _db.StockBill
                        join emp in _db.employee on bill.modid equals emp.FID
                        //join cabinet in _db.Cabinet on bill.CbntFid equals cabinet.FID
                        join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                        join draw in _db.Drawers on cbnt.FID equals draw.CbntFid
                        join mnlf in _db.MenuLeft on bill.BillType equals mnlf.OperCode
                        join mapPrscptOnbill in _db.MapPrscptOnBill on bill.FID equals mapPrscptOnbill.StockbillFid
                        join prscptBill in _db.PrscptBill on mapPrscptOnbill.PrscptFid equals prscptBill.FID
                        where (!qmoddate1.HasValue || bill.moddate >= qmoddate1.Value)
                        && (!qmoddate2.HasValue || bill.moddate <= qmoddate2.Value.AddDays(1))
                        && (qCbnt == null || cbnt.FID.ToString().Equals(qCbnt.ToString()))
                        && (qDrawFid == "" || draw.FID.ToString().Equals(qDrawFid.ToString()))
                        select new StockingLog
                        {
                            stockBillFid = bill.FID,
                            operTime = bill.moddate ?? DateTime.Now, //時間
                            billType = bill.BillType, //類型
                            billTypeName = mnlf.CatelogName, //類型名稱
                            empName = emp.name, //藥師
                            empNo = emp.emp_no ?? "", //藥師編號
                            CbntFid = cbnt.FID, //藥櫃ID
                            CbntName = cbnt.CbntName, //藥櫃
                            DrawNo = draw.No.ToString(), //櫃位號碼
                            QtyIn = bill.TradeType ? bill.Qty : 0, //進貨數量
                            QtyOut = bill.TradeType ? 0 : bill.Qty, //出貨數量
                            QtyBefore = bill.SysChkQty + (bill.Qty * (bill.TradeType ? -1 : 1)), //原始數量
                            UserChk1 = bill.UserChk1Qty, //一次盤點數量
                            UserChk2 = bill.UserChk2Qty, //二次盤點數量
                            UserChkErr = !(bill.UserChk1Qty == null || bill.SysChkQty == bill.UserChk1Qty || bill.SysChkQty == bill.UserChk2Qty), //盤點錯誤
                            SysChkQty = bill.SysChkQty, //剩餘總數
                            OperFinish = true, //操作完成
                            DrugCode = bill.DrugCode, //藥物編碼
                            DrugName = bill.DrugName.Trim(), //藥物名稱
                            OperQty = bill.Qty, //操作總量
                            FromFid = bill.FromFid, //來源藥櫃
                            PharmCode = prscptBill.Pharmarcy, //藥局簡碼
                            PrscptNo = prscptBill.PrscptNo, //領藥號
                            PrscptDate = prscptBill.PrscptDate, //出藥日期
                            OrderSeq = prscptBill.OrderSeq, //醫令序號
                            CtrlDrugGrand = prscptBill.CtrlDrugGrand, //管制藥等級
                            PatientNo = prscptBill.PatientNo, //病歷號碼
                            PatientSeq = prscptBill.PatientSeq, //病人序號
                            PatientName = prscptBill.PatientName, //病人姓名
                            DrName = prscptBill.DrName, //醫師姓名
                            BedCode = prscptBill.BedCode, //床號
                            BatchNo = bill.BatchNo, //藥品批號
                            ExpireDate = bill.ExpireDate, //效期

                        };
            var result = query.ToList(); // 在這裡執行查詢並獲得結果
            foreach (var item in result)
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

            return result.AsQueryable(); // 返回 IQueryable<StockingLog>
        }

         
    }
}

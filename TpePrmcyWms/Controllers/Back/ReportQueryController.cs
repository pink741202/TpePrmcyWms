using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
//using OfficeOpenXml.Core.ExcelPackage;
using OfficeOpenXml;
using System.Security.Cryptography;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using TpePrmcyWms.Models.Unit.Report;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace TpePrmcyWms.Controllers.Back
{
    public class ReportQueryController : BaseController
    {
        private readonly DBcPharmacy _db = new DBcPharmacy();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportQueryController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [ActionName("UserOperLog")]
        public async Task<IActionResult> UserOperLog(int? pageNum, string sortOrder, string qKeyString
            , DateTime? qmoddate1, DateTime? qmoddate2)
        {
            IQueryable<UserOperLog> obj = (from bill in _db.StockBill
                                           join emp in _db.employee on bill.modid equals emp.FID
                                           join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                                           join draw in _db.Drawers on bill.DrawFid equals draw.FID
                                           where emp.name.Contains(qKeyString) || (emp.emp_no ?? "").Contains(qKeyString)
                                           select new UserOperLog
                                           {
                                               stockBillFid = bill.FID,
                                               operTime = bill.moddate ?? DateTime.Now,
                                               operCode = bill.BillType,
                                               empName = emp.name,
                                               empNo = emp.emp_no ?? "",
                                               CbntFid = cbnt.FID,
                                               CbntName = cbnt.CbntName,
                                               DrawNo = draw.No.ToString(),
                                           });

            //改用dapper
            //select bill.BillType, bill.DrugCode, bill.Qty
            //, emp.name, cbnt.CbntName, draw.No,prscpt.PrscptNo, prscpt.PatientName
            //from StockBill bill
            //left join employee emp on bill.modid = emp.FID
            //left join DrugGrid grid on bill.DrugGridFid = grid.FID
            //left join Cabinet cbnt on grid.CbntFid = cbnt.FID
            //left join Drawers draw on grid.DrawFid = draw.FID
            //left join MapPrscptOnBill map on bill.FID = map.StockbillFid and bill.BillType = map.BillType
            //left join PrscptBill prscpt on map.PrscptFid = prscpt.FID

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            ViewData["qmoddate1"] = qmoddate1;
            ViewData["qmoddate2"] = qmoddate2;
            if (qmoddate1 != null)
            {
                DateTime q1 = (DateTime)qmoddate1;
                obj = obj.Where(x => x.operTime.Date >= q1.Date);
            }
            if (qmoddate2 != null)
            {
                DateTime q2 = (DateTime)qmoddate2;
                obj = obj.Where(x => x.operTime.Date < q2.Date);
            }
            #endregion

            foreach (UserOperLog item in obj)
            {

            }

            #region 排序            
            switch (sortOrder)
            {
                case "CbntName_desc": obj = obj.OrderByDescending(s => s.CbntName); break;
                case "CbntName": obj = obj.OrderBy(s => s.CbntName); break;
                case "operTime_desc": obj = obj.OrderByDescending(s => s.operTime); break;
                case "operTime": obj = obj.OrderBy(s => s.operTime); break;
                default: sortOrder = "operTime"; obj = obj.OrderBy(s => s.operTime); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<UserOperLog>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }

        [ActionName("List")]
        public async Task<IActionResult> List(int? pageNum, string sortOrder, string qKeyString)
        {
            IQueryable<DrugInfo> obj = _db.DrugInfo.Where(x => !x.DrugCode.EndsWith("_b"));

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.DrugCode ?? "").Contains(qKeyString)
                      || (s.DrugName ?? "").Contains(qKeyString)
                );
            }

            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                default: sortOrder = "DrugCode"; obj = obj.OrderBy(s => s.DrugCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<DrugInfo>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }

        [ActionName("SysLog")]
        public async Task<IActionResult> SysLog(int? pageNum, string sortOrder, string qKeyString)
        {
            IQueryable<DrugInfo> obj = _db.DrugInfo.Where(x => !x.DrugCode.EndsWith("_b"));

            #region 查詢
            ViewData["qKeyString"] = qKeyString;
            if (!string.IsNullOrEmpty(qKeyString))
            {
                StringComparison comp = StringComparison.OrdinalIgnoreCase;
                obj = obj.Where(s => (s.DrugCode ?? "").Contains(qKeyString)
                      || (s.DrugName ?? "").Contains(qKeyString)
                );
            }

            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                default: sortOrder = "DrugCode"; obj = obj.OrderBy(s => s.DrugCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<DrugInfo>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }

        [ActionName("InvntDrawer")]
        public async Task<IActionResult> InvntDrawer(int? pageNum, string sortOrder, string qKeyString , DateTime? qDate1, DateTime? qDate2)
        {
            IQueryable<StockingLog> obj = (from bill in _db.StockBill
                                           join emp in _db.employee on bill.modid equals emp.FID
                                           join cabinet in _db.Cabinet on bill.CbntFid equals cabinet.FID
                                           join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                                           join draw in _db.Drawers on cabinet.FID equals draw.FID
                                           join mnlf in _db.MenuLeft on bill.BillType equals mnlf.OperCode
                                           join mapPrscptOnbill in _db.MapPrscptOnBill on bill.FID equals mapPrscptOnbill.StockbillFid
                                           join prscptBill in _db.PrscptBill on mapPrscptOnbill.PrscptFid equals prscptBill.FID
                                           where (!qDate1.HasValue || bill.moddate >= qDate1.Value)
                                           && (!qDate2.HasValue || bill.moddate <= qDate2.Value)
                                           && (string.IsNullOrEmpty(qKeyString) || EF.Functions.Like(cbnt.FID.ToString(), $"%{qKeyString}%") ||
                                           EF.Functions.Like(cbnt.CbntName, $"%{qKeyString}%"))
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

                                           });

            #region 查詢
            ViewData["qKeyString"] = qKeyString ?? "";
            
            ViewData["qDate1"] = qDate1;
            if (qDate1 != null)
            {
                ViewData["qDate1"] = ((DateTime)qDate1).ToString("yyyy-MM-dd");
            }

            ViewData["qDate2"] = qDate2;
            if (qDate2 != null)
            {
                ViewData["qDate2"] = ((DateTime)qDate2).ToString("yyyy-MM-dd");
            }


            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                default: sortOrder = "DrugCode"; obj = obj.OrderBy(s => s.DrugCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<StockingLog>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }

        [ActionName("InvntHsptl")]
        public async Task<IActionResult> InvntHsptl(int? pageNum, string sortOrder, string qKeyString, DateTime? qDate1, DateTime? qDate2)
        {
            IQueryable<StockingLog> obj = (from bill in _db.StockBill
                                           join emp in _db.employee on bill.modid equals emp.FID
                                           join cabinet in _db.Cabinet on bill.CbntFid equals cabinet.FID
                                           join cbnt in _db.Cabinet on bill.CbntFid equals cbnt.FID
                                           join draw in _db.Drawers on cabinet.FID equals draw.FID
                                           join mnlf in _db.MenuLeft on bill.BillType equals mnlf.OperCode
                                           join mapPrscptOnbill in _db.MapPrscptOnBill on bill.FID equals mapPrscptOnbill.StockbillFid
                                           join prscptBill in _db.PrscptBill on mapPrscptOnbill.PrscptFid equals prscptBill.FID
                                           where (!qDate1.HasValue || bill.moddate >= qDate1.Value)
                                           && (!qDate2.HasValue || bill.moddate <= qDate2.Value)
                                           && (string.IsNullOrEmpty(qKeyString) || EF.Functions.Like(cbnt.FID.ToString(), $"%{qKeyString}%") ||
                                           EF.Functions.Like(cbnt.CbntName, $"%{qKeyString}%"))
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

                                           });

            #region 查詢
            ViewData["qKeyString"] = qKeyString ?? "";

            ViewData["qDate1"] = qDate1;
            if (qDate1 != null)
            {
                ViewData["qDate1"] = ((DateTime)qDate1).ToString("yyyy-MM-dd");
            }

            ViewData["qDate2"] = qDate2;
            if (qDate2 != null)
            {
                ViewData["qDate2"] = ((DateTime)qDate2).ToString("yyyy-MM-dd");
            }


            #endregion

            #region 排序            
            switch (sortOrder)
            {
                case "DrugCode_desc": obj = obj.OrderByDescending(s => s.DrugCode); break;
                case "DrugCode": obj = obj.OrderBy(s => s.DrugCode); break;
                default: sortOrder = "DrugCode"; obj = obj.OrderBy(s => s.DrugCode); break;
            }
            ViewData["sortOrder"] = sortOrder;
            #endregion

            #region 分頁
            if (qKeyString != null)
            {
                pageNum = 1;
            }
            #endregion

            return View(await PaginatedList<StockingLog>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }


    }
}

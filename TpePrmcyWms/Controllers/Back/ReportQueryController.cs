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

            foreach (UserOperLog item in obj) {
                
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
        public async Task<IActionResult> InvntDrawer(int? pageNum, string sortOrder, string qKeyString)
        {
            IQueryable<StockBill> obj = _db.StockBill;

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

            return View(await PaginatedList<StockBill>.CreateAsync(obj.AsNoTracking(), pageNum ?? 1, Loginfo.User.pagesize));

        }

        


    }
}

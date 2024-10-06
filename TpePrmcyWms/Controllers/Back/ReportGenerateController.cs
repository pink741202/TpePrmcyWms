using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using TpePrmcyWms.Models.Unit.Back;
using static Dapper.SqlMapper;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TpePrmcyWms.Models.Unit.Report;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;
//using OfficeOpenXml.Core.ExcelPackage;

namespace TpePrmcyWms.Controllers.Back
{
    public class ReportGenerateController : NoAuthBackController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportGenerateController(IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(webHostEnvironment), "IWebHostEnvironment is not provided.");
            }
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly DBcPharmacy _db = new DBcPharmacy();

        [HttpGet]
        public IActionResult Index(int AtCbntFid)
        {


            return View();
        }
        [HttpPost]
        public IActionResult ReportDownload([FromBody] ReportDownloadRequestModel? model)
        {
            model ??= new ReportDownloadRequestModel();
            string qKeyString = model.qKeyString ?? "";
            DateTime? qDate1 = model.qDate1;
            DateTime? qDate2 = model.qDate2;
            int? qCbnt = model.qCbnt;
            string qDrawFid = model.qDrawFid ?? "";
            string qbilltype = model.qbilltype ?? "";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HHmmss.fff") + "_system.xlsx";
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "excel", fileName);

            if (!System.IO.Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "excel")))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "excel"));
            }
            try
            {
                using (ExcelPackage ep = new ExcelPackage())
                {
                    ep.Workbook.Worksheets.Add("Inventory");

                    ExcelWorksheet sheet1 = ep.Workbook.Worksheets[1];

                    #region EXCEL DATA

                    // Title
                    sheet1.Cells[1, 1].Value = "時間";
                    sheet1.Cells[1, 2].Value = "類型";
                    sheet1.Cells[1, 3].Value = "藥櫃";
                    sheet1.Cells[1, 4].Value = "藥櫃 ID";
                    sheet1.Cells[1, 5].Value = "櫃位號碼";
                    sheet1.Cells[1, 6].Value = "來源藥櫃";
                    sheet1.Cells[1, 7].Value = "來源藥櫃 ID";
                    sheet1.Cells[1, 8].Value = "目標藥櫃";
                    sheet1.Cells[1, 9].Value = "目標藥櫃 ID";
                    sheet1.Cells[1, 10].Value = "藥師";
                    sheet1.Cells[1, 11].Value = "藥師編號";
                    sheet1.Cells[1, 12].Value = "進貨數量";
                    sheet1.Cells[1, 13].Value = "出貨數量";
                    sheet1.Cells[1, 14].Value = "原始數量";
                    sheet1.Cells[1, 15].Value = "一次盲點數量";
                    sheet1.Cells[1, 16].Value = "二次盲點數量";
                    sheet1.Cells[1, 17].Value = "盲點錯誤";
                    sheet1.Cells[1, 18].Value = "剩餘總數";
                    sheet1.Cells[1, 19].Value = "操作完成";
                    sheet1.Cells[1, 20].Value = "藥物編碼";
                    sheet1.Cells[1, 21].Value = "藥物名稱";
                    sheet1.Cells[1, 22].Value = "操作總量";
                    sheet1.Cells[1, 23].Value = "藥局簡碼";
                    sheet1.Cells[1, 24].Value = "領藥號";
                    sheet1.Cells[1, 25].Value = "出藥日期";
                    sheet1.Cells[1, 26].Value = "醫令序號";
                    sheet1.Cells[1, 27].Value = "管制藥等級";
                    sheet1.Cells[1, 28].Value = "病歷號碼";
                    sheet1.Cells[1, 29].Value = "病人序號";
                    sheet1.Cells[1, 30].Value = "病人姓名";
                    sheet1.Cells[1, 31].Value = "醫師姓名";
                    sheet1.Cells[1, 32].Value = "床號";
                    sheet1.Cells[1, 33].Value = "藥品批號";
                    sheet1.Cells[1, 34].Value = "效期";
                    sheet1.Cells[1, 35].Value = "系統通知內容";

                    int start = 2;
                    //List<StockBill> records = new List<StockBill>();
                    //records = _db.StockBill.ToList();
                    //ViewData["qKeyString"] = qKeyString;
                    //if (!string.IsNullOrEmpty(qKeyString))
                    //{
                    //    records = records.Where(s => (s.DrugCode ?? "").Contains(qKeyString)).ToList();
                    //}
                    ReportService reportServices = new ReportService();
                    var stockingLogs = reportServices.queryStockingLog(qKeyString, qDate1, qDate2, qCbnt, qDrawFid, qbilltype);

                    foreach (var data in stockingLogs)
                    {
                        sheet1.Cells[start, 1].Value = data.operTime.ToString("yyyy-MM-dd HH:mm:ss"); //時間
                        sheet1.Cells[start, 2].Value = data.billTypeName; //類型
                        sheet1.Cells[start, 3].Value = data.CbntName; //藥櫃
                        sheet1.Cells[start, 4].Value = data.CbntFid; //藥櫃 ID
                        sheet1.Cells[start, 5].Value = data.DrawNo; //櫃位號碼
                        sheet1.Cells[start, 6].Value = data.FromCbntName; //來源藥櫃
                        sheet1.Cells[start, 7].Value = data.FromCbntFid; //來源藥櫃 ID
                        sheet1.Cells[start, 8].Value = data.TrgtCbntName; //目標藥櫃
                        sheet1.Cells[start, 9].Value = data.TrgtCbntFid; //目標藥櫃 ID
                        sheet1.Cells[start, 10].Value = data.empName; //藥師
                        sheet1.Cells[start, 11].Value = data.empNo; //藥師編號
                        sheet1.Cells[start, 12].Value = data.QtyIn; //進貨數量
                        sheet1.Cells[start, 13].Value = data.QtyOut; //出貨數量
                        sheet1.Cells[start, 14].Value = data.QtyBefore; //原始數量
                        sheet1.Cells[start, 15].Value = data.UserChk1; //一次盲點數量
                        sheet1.Cells[start, 16].Value = data.UserChk2; //二次盲點數量
                        sheet1.Cells[start, 17].Value = data.UserChkErr; //盲點錯誤
                        sheet1.Cells[start, 18].Value = data.SysChkQty; //剩餘總數
                        sheet1.Cells[start, 19].Value = data.OperFinish; //操作完成
                        sheet1.Cells[start, 20].Value = data.DrugCode; //藥物編碼
                        sheet1.Cells[start, 21].Value = data.DrugName; //藥物名稱
                        sheet1.Cells[start, 22].Value = data.OperQty; //操作總量
                        sheet1.Cells[start, 23].Value = data.PharmCode; //藥局簡碼
                        sheet1.Cells[start, 24].Value = data.PrscptNo; //領藥號
                        sheet1.Cells[start, 25].Value = data.PrscptDate; //出藥日期
                        sheet1.Cells[start, 26].Value = data.OrderSeq; //醫令序號
                        sheet1.Cells[start, 27].Value = data.CtrlDrugGrand; //管制藥等級
                        sheet1.Cells[start, 28].Value = data.PatientNo; //病歷號碼
                        sheet1.Cells[start, 29].Value = data.PatientSeq; //病人序號
                        sheet1.Cells[start, 30].Value = data.PatientName; //病人姓名
                        sheet1.Cells[start, 31].Value = data.DrName; //醫師姓名
                        sheet1.Cells[start, 32].Value = data.BedCode; //床號
                        sheet1.Cells[start, 33].Value = data.BatchNo; //藥品批號
                        sheet1.Cells[start, 34].Value = data.ExpireDate; //效期
                        sheet1.Cells[start, 35].Value = ""; //系統通知內容
                        start++;
                    }

                    #endregion

                    sheet1.Cells[sheet1.Dimension.Address].AutoFitColumns();

                    using (FileStream createStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        ep.SaveAs(createStream);
                    }
                }

                Stream fileStream = new FileStream(filePath, FileMode.Open);

                if (fileStream == null)
                    throw new Exception("路徑檔案未找到:" + fileName);

                //return File(fileStream, "application/octet-stream", fileName);
                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "下載失敗\n" + ex.Message;
                return RedirectToAction("ReportQuery");
            }
        }

        [HttpPost]
        public IActionResult UserLogReportDownload([FromBody] ReportDownloadRequestModel? model)
        {
            model ??= new ReportDownloadRequestModel();
            string qKeyString = model.qKeyString ?? "";
            DateTime? qDate1 = model.qDate1;
            DateTime? qDate2 = model.qDate2;
            int? qCbnt = model.qCbnt;
            string qDrawFid = model.qDrawFid ?? "";
            string qbilltype = model.qbilltype ?? "";
            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HHmmss.fff") + "_system.xlsx";
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "excel", fileName);

            if (!System.IO.Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "excel")))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "excel"));
            }
            try
            {
                using (ExcelPackage ep = new ExcelPackage())
                {
                    ep.Workbook.Worksheets.Add("System");

                    ExcelWorksheet sheet1 = ep.Workbook.Worksheets[1];

                    #region EXCEL DATA

                    // Title
                    sheet1.Cells[1, 1].Value = "訊息時間";
                    sheet1.Cells[1, 2].Value = "使用者ID";
                    sheet1.Cells[1, 3].Value = "使用者";
                    sheet1.Cells[1, 4].Value = "操作頁面";
                    sheet1.Cells[1, 5].Value = "紀錄類型";
                    sheet1.Cells[1, 6].Value = "操作類型";
                    sheet1.Cells[1, 7].Value = "操作訊息";
                    sheet1.Cells[1, 8].Value = "操作結果";
                    sheet1.Cells[1, 9].Value = "發生錯誤的物件";
                    sheet1.Cells[1, 10].Value = "發生錯誤的功能";
                    sheet1.Cells[1, 11].Value = "錯誤訊息";
                    sheet1.Cells[1, 12].Value = "錯誤軌跡";

                    int start = 2;
                    UserLog reportServices = new UserLog();
                    IQueryable<UserLog> obj = from log in _db.OperateLog
                                              join emp in _db.employee on log.empFid equals emp.FID
                                              where log.LogType == "operate"
                                              && (log.OperateType == "L" || log.OperateType == "DrawOpen")
                                              && (!qDate1.HasValue || log.LogTime >= qDate1.Value)
                                              && (!qDate2.HasValue || log.LogTime <= qDate2.Value.AddDays(1))
                                              select new UserLog
                                              {
                                                  UserLogFID = log.FID,
                                                  empFid = log.empFid,
                                                  name = emp.name,
                                                  LinkMethod = log.LinkMethod,
                                                  LogType = log.LogType,
                                                  OperateType = log.OperateType,
                                                  LogMsg = log.LogMsg,
                                                  OperateSuccess = log.OperateSuccess,
                                                  ErrorClass = log.ErrorClass,
                                                  ErrorFunction = log.ErrorFunction,
                                                  ErrorMsg = log.ErrorMsg,
                                                  ErrorTrace = log.ErrorTrace,
                                                  LogTime = log.LogTime,
                                                  comFid = log.comFid,
                                              };

                    foreach (var data in obj)
                    {
                        sheet1.Cells[start, 1].Value = data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"); //訊息時間
                        sheet1.Cells[start, 2].Value = data.empFid; //使用者ID
                        sheet1.Cells[start, 3].Value = data.name; //使用者
                        sheet1.Cells[start, 4].Value = data.LinkMethod; //操作頁面
                        sheet1.Cells[start, 5].Value = data.LogType; //紀錄類型
                        sheet1.Cells[start, 6].Value = data.OperateType; //操作類型
                        sheet1.Cells[start, 7].Value = data.LogMsg; //操作訊息
                        sheet1.Cells[start, 8].Value = data.OperateSuccess; //操作結果
                        sheet1.Cells[start, 9].Value = data.ErrorClass; //發生錯誤的物件
                        sheet1.Cells[start, 10].Value = data.ErrorFunction; //發生錯誤的功能
                        sheet1.Cells[start, 11].Value = data.ErrorMsg; //錯誤訊息
                        sheet1.Cells[start, 12].Value = data.ErrorTrace; //錯誤軌跡
                        start++;
                    }

                    #endregion

                    sheet1.Cells[sheet1.Dimension.Address].AutoFitColumns();

                    using (FileStream createStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        ep.SaveAs(createStream);
                    }
                }

                Stream fileStream = new FileStream(filePath, FileMode.Open);

                if (fileStream == null)
                    throw new Exception("路徑檔案未找到:" + fileName);

                //return File(fileStream, "application/octet-stream", fileName);
                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "下載失敗\n" + ex.Message;
                return RedirectToAction("ReportQuery");
            }
        }



    }
}

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
        public IActionResult ReportDownload([FromBody] int orderCode, string qKeyString)
        {
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
                    ep.Workbook.Worksheets.Add("123");

                    ExcelWorksheet sheet1 = ep.Workbook.Worksheets[1];

                    #region EXCEL DATA

                    // Title
                    sheet1.Cells[1, 1].Value = "藥櫃";
                    sheet1.Cells[1, 2].Value = "操作類別";
                    sheet1.Cells[1, 3].Value = "藥品代碼";
                    sheet1.Cells[1, 4].Value = "藥格藥品";
                    sheet1.Cells[1, 5].Value = "出/入 庫";
                    sheet1.Cells[1, 6].Value = "目標數量";
                    sheet1.Cells[1, 7].Value = "數量";
                    sheet1.Cells[1, 8].Value = "需求樓層";
                    sheet1.Cells[1, 9].Value = "批號";
                    sheet1.Cells[1, 10].Value = "效期";
                    sheet1.Cells[1, 11].Value = "單號來源";
                    sheet1.Cells[1, 12].Value = "備註";
                    sheet1.Cells[1, 13].Value = "所屬公司";
                    sheet1.Cells[1, 14].Value = "操作人員";
                    sheet1.Cells[1, 15].Value = "操作時間";
                    sheet1.Cells[1, 16].Value = "操作完成";
                    sheet1.Cells[1, 17].Value = "盤點類型";
                    sheet1.Cells[1, 18].Value = "系統計算的庫存量";
                    sheet1.Cells[1, 19].Value = "盤點的庫存量第一次";
                    sheet1.Cells[1, 20].Value = "盤點的庫存量第二次";

                    int start = 2;
                    List<StockBill> records = new List<StockBill>();
                    records = _db.StockBill.ToList();
                    ViewData["qKeyString"] = qKeyString;
                    if (!string.IsNullOrEmpty(qKeyString))
                    {
                        records = records.Where(s => (s.DrugCode ?? "").Contains(qKeyString)).ToList();
                    }

                    foreach (var data in records)
                    {
                        sheet1.Cells[start, 1].Value = data.FID;
                        sheet1.Cells[start, 2].Value = data.BillType;
                        sheet1.Cells[start, 3].Value = data.CbntFid;
                        sheet1.Cells[start, 4].Value = data.DrugCode;
                        sheet1.Cells[start, 5].Value = data.TradeType;
                        sheet1.Cells[start, 6].Value = data.TargetQty;
                        sheet1.Cells[start, 7].Value = data.Qty;
                        sheet1.Cells[start, 8].Value = data.ToFloor;
                        sheet1.Cells[start, 9].Value = data.BatchNo;
                        sheet1.Cells[start, 10].Value = data.ExpireDate;
                        sheet1.Cells[start, 11].Value = data.FromFid;
                        sheet1.Cells[start, 12].Value = data.RecNote;
                        sheet1.Cells[start, 13].Value = data.comFid;
                        sheet1.Cells[start, 14].Value = data.modid;
                        sheet1.Cells[start, 15].Value = data.moddate;
                        sheet1.Cells[start, 16].Value = data.JobDone;
                        sheet1.Cells[start, 17].Value = data.TakeType;
                        sheet1.Cells[start, 18].Value = data.SysChkQty;
                        sheet1.Cells[start, 19].Value = data.UserChk1Qty;
                        sheet1.Cells[start, 20].Value = data.UserChk2Qty;
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

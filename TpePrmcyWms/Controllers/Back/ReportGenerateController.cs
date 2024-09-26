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
//using OfficeOpenXml.Core.ExcelPackage;

namespace TpePrmcyWms.Controllers.Back
{
    public class ReportGenerateController : NoAuthBackController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportGenerateController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly DBcPharmacy _db = new DBcPharmacy();

        [HttpGet]
        public IActionResult Index(int AtCbntFid)
        {
            

            return View();
        }
        [HttpPost]
        public IActionResult ReportDownload([FromBody] int orderCode)
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
                    sheet1.Cells[1, 1].Value = "日期";
                    sheet1.Cells[1, 2].Value = "病歷號";
                    sheet1.Cells[1, 3].Value = "診";
                    sheet1.Cells[1, 4].Value = "姓名";
                    sheet1.Cells[1, 5].Value = "處方日";
                    sheet1.Cells[1, 6].Value = "異動日";
                    sheet1.Cells[1, 7].Value = "科別";
                    sheet1.Cells[1, 8].Value = "醫師";
                    sheet1.Cells[1, 9].Value = "床位";
                    sheet1.Cells[1, 10].Value = "代碼";
                    sheet1.Cells[1, 11].Value = "藥名";
                    sheet1.Cells[1, 12].Value = "數量";
                    sheet1.Cells[1, 13].Value = "領藥號";
                    sheet1.Cells[1, 14].Value = "醫序";
                    sheet1.Cells[1, 15].Value = "病序";
                    sheet1.Cells[1, 16].Value = "CHECK";
                    sheet1.Cells[1, 17].Value = "備註";
                    sheet1.Cells[1, 18].Value = "月份";
                    sheet1.Cells[1, 19].Value = "處方條碼";
                    sheet1.Cells[1, 20].Value = "門";
                    sheet1.Cells[1, 21].Value = "急";
                    sheet1.Cells[1, 22].Value = "住";
                    sheet1.Cells[1, 23].Value = "檢查與否";

                    //int start = 2;

                    //foreach (var data in records)
                    //{
                    //    sheet1.Cells[start, 1].Value = (data.RecordTime.Year - 1911).ToString() + data.RecordTime.ToString("MMdd");
                    //    sheet1.Cells[start, 2].Value = data.PatientNo;
                    //    sheet1.Cells[start, 3].Value = "";
                    //    sheet1.Cells[start, 4].Value = data.PatientName;
                    //    sheet1.Cells[start, 5].Value = data.PharmacyDate;
                    //    sheet1.Cells[start, 6].Value = "";
                    //    sheet1.Cells[start, 7].Value = "";
                    //    sheet1.Cells[start, 8].Value = data.DoctorName;
                    //    sheet1.Cells[start, 9].Value = data.BedCode;
                    //    sheet1.Cells[start, 10].Value = data.OrderCode;
                    //    sheet1.Cells[start, 11].Value = data.OrderEnName;
                    //    if (data.InventoryReportType == InventoryReportType.Return)
                    //        sheet1.Cells[start, 12].Value = "-" + data.TotalQuantity;
                    //    else
                    //        sheet1.Cells[start, 12].Value = data.TotalQuantity.ToString();
                    //    sheet1.Cells[start, 13].Value = data.PharmacyNumber;
                    //    sheet1.Cells[start, 14].Value = data.OrderSeq;
                    //    sheet1.Cells[start, 15].Value = data.PatientSeq;
                    //    sheet1.Cells[start, 16].Value = "";
                    //    sheet1.Cells[start, 17].Value = data.Remark;
                    //    sheet1.Cells[start, 18].Value = data.RecordTime.ToString("MM");
                    //    sheet1.Cells[start, 19].Value = "";
                    //    sheet1.Cells[start, 20].Value = "";
                    //    sheet1.Cells[start, 21].Value = "";
                    //    sheet1.Cells[start, 22].Value = "";
                    //    sheet1.Cells[start, 23].Value = data.ManualSheetCheck;

                    //    start++;
                    //}

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

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "下載失敗\n" + ex.Message;
                return RedirectToAction("ReportQuery");
            }
        }



    }
}

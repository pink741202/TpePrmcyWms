using System.ComponentModel.DataAnnotations;

namespace TpePrmcyWms.Models.Unit.Report
{
    public class StockingLog
    {
        [Key]
        public int stockBillFid { get; set; } = 0;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "時間")]
        public DateTime operTime { get; set; }

        [Display(Name = "類型")]
        public string billType { get; set; } = "";

        [Display(Name = "類型名稱")]
        public string billTypeName { get; set; } = ""; 

        [Display(Name = "藥師")]
        public string empName { get; set; } = ""; 

        [Display(Name = "藥師編號")]
        public string empNo { get; set; } = ""; 

        [Display(Name = "藥櫃ID")]
        public int CbntFid { get; set; } 

        [Display(Name = "藥櫃")]
        public string CbntName { get; set; } = "";

        [Display(Name = "櫃位號碼")]
        public string DrawNo { get; set; } = "";

        [Display(Name = "來源藥櫃")]
        public int? FromFid { get; set; }

        [Display(Name = "來源藥櫃ID")]
        public int? FromCbntFid { get; set; }

        [Display(Name = "來源藥櫃")]
        public string? FromCbntName { get; set; } = "";

        [Display(Name = "目標藥櫃ID")]
        public int? TrgtCbntFid { get; set; }

        [Display(Name = "目標藥櫃")]
        public string? TrgtCbntName { get; set; } = "";

        [Display(Name = "進貨數量")]
        public decimal? QtyIn { get; set; }

        [Display(Name = "出貨數量")]
        public decimal? QtyOut { get; set; }

        [Display(Name = "原始數量")]
        public decimal? QtyBefore { get; set; } 

        [Display(Name = "一次盤點數量")]
        public decimal? UserChk1 { get; set; } 

        [Display(Name = "二次盤點數量")]
        public decimal? UserChk2 { get; set; }

        [Display(Name = "盤點錯誤")]
        public bool? UserChkErr { get; set; }

        [Display(Name = "剩餘總數")]
        public decimal? SysChkQty { get; set; }

        [Display(Name = "操作完成")]
        public bool? OperFinish { get; set; }

        [Display(Name = "藥物編碼")]
        public string DrugCode { get; set; } = "";

        [Display(Name = "藥物名稱")]
        public string DrugName { get; set; } = "";

        [Display(Name = "操作總量")]
        public decimal? OperQty { get; set; }

        [Display(Name = "藥局簡碼")]
        public string? PharmCode { get; set; } = "";

        [Display(Name = "領藥號")]
        public string? PrscptNo { get; set; } = "";

        [Display(Name = "出藥日期")]
        public DateTime? PrscptDate { get; set; }

        [Display(Name = "醫令序號")]
        public decimal? OrderSeq { get; set; }

        [Display(Name = "管制藥等級")]
        public string? CtrlDrugGrand { get; set; } = "";

        [Display(Name = "病歷號碼")]
        public string? PatientNo { get; set; } = "";

        [Display(Name = "病人序號")]
        public string? PatientSeq { get; set; } = "";

        [Display(Name = "病人姓名")]
        public string? PatientName { get; set; } = "";

        [Display(Name = "醫師姓名")]
        public string? DrName { get; set; } = "";

        [Display(Name = "床號")]
        public string? BedCode { get; set; } = "";

        [Display(Name = "藥品批號")]
        public string? BatchNo { get; set; } = "";

        [Display(Name = "效期")]
        public DateTime? ExpireDate { get; set; }

    }
}

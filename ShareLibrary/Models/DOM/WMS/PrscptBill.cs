using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("PrscptBill")]
    public partial class PrscptBill
    {
        [Key]
        public int FID { get; set; }

        [StringLength(1)]
        [Display(Name = "藥局簡碼")]
        public string? Pharmarcy { get; set; }

        [StringLength(5)]
        [Display(Name = "領藥號")]
        public string? PrscptNo { get; set; }

        [Display(Name = "出藥日期")]
        public DateTime? PrscptDate { get; set; }

        [StringLength(8)]
        public string? DrugCode { get; set; }

        [StringLength(60)]
        public string? DrugName { get; set; }

        [StringLength(1)]
        [Display(Name = "管制藥等級")]
        public string? CtrlDrugGrand { get; set; }

        [StringLength(8)]
        [Display(Name = "病歷號碼")]
        public string? PatientNo { get; set; }

        [StringLength(6)]
        [Display(Name = "病人序號")]
        public string? PatientSeq { get; set; }

        [Display(Name = "醫令序號")]
        public decimal? OrderSeq { get; set; }

        [StringLength(40)]
        [Display(Name = "病人姓名")]
        public string? PatientName { get; set; }

        public decimal? TtlQty { get; set; }

        [StringLength(20)]
        public string? PriceUnit { get; set; }

        [StringLength(40)]
        [Display(Name = "醫師姓名")]
        public string? DrName { get; set; }

        [StringLength(6)]
        [Display(Name = "床號")]
        public string? BedCode { get; set; }

        [StringLength(4)]
        public string? DrugDose { get; set; }

        [StringLength(20)]
        public string? DrugFrequency { get; set; }

        [StringLength(4)]
        public string? DrugDays { get; set; }
        [StringLength(14)]
        public string? ReturnSheet { get; set; }
        public int ScanTime { get; set; }
        public bool DoneFill { get; set; }
        public bool? HISchk { get; set; }
        [Display(Name = "建立人員")]
        public int? addid { get; set; }
        [Display(Name = "建立時間")]
        public DateTime? adddate { get; set; }
        [Display(Name = "操作人員")]
        public int? modid { get; set; }
        [Display(Name = "操作時間")]
        public DateTime? moddate { get; set; }
        public int? comFid { get; set; }
        public int? dptFid { get; set; }
        


    }
}
// TtlQty = DrugDose * DrugFrequency * DrugDays
// 總量 = 劑量 * 頻次代碼(要轉為頻次) * 天數
// 劑量 * 頻次 <= 病人一日最大服用量(DailyMaxTake)


//strPhaSw CHAR(1)出藥藥局
//strPhaNum CHAR(5)領藥號
//strPhaDate CHAR(7) 出藥日期(民國年YYYMMDD)
//strOdrCode VARCHAR(8)藥物代碼
//strOdrName VARCHAR(60)藥物商品名
//strAnesSw CHAR(1)管制藥等級(1或2或3)
//strPatNo CHAR(8)病歷號碼
//strPatSeq CHAR(6)病人序號
//strOdrSeq NUMBER(5,0)醫令序號
//strPatName VARCHAR(40)病人姓名
//strTotalQty NUMBER(5,0)出藥總量
//strPriceUnit VARCHAR(20)總量單位
//strDrName VARCHAR(40)醫師姓名
//strBedCode VARCHAR(6)床號(只有住院病人有)

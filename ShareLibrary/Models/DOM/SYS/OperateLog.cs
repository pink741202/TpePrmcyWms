using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("OperateLog")]
    public partial class OperateLog
    {
        [Key]
        public int FID { get; set; }
        [Display(Name = "使用者")]
        public int empFid { get; set; }
        [Display(Name = "操作頁面")]
        public string? LinkMethod { get; set; }
        [Display(Name = "紀錄類型")]
        public string? LogType { get; set; } = "";
        [Display(Name = "操作類型")]
        public string? OperateType { get; set; } = "";
        [Display(Name = "操作訊息")]
        public string? LogMsg { get; set; } = "";
        [Display(Name = "操作結果")]
        public bool? OperateSuccess { get; set; }
        [Display(Name = "發生錯誤的物件")]
        public string? ErrorClass { get; set; } = "";
        [Display(Name = "發生錯誤的功能")]
        public string? ErrorFunction { get; set; } = "";
        [Display(Name = "錯誤訊息")]
        public string? ErrorMsg { get; set; } = "";

        [Column(TypeName = "ntext")]
        [Display(Name = "錯誤軌跡")]
        public string? ErrorTrace { get; set; } = "";

        [Required]
        [Display(Name = "訊息時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime LogTime { get; set; }
        [Display(Name = "公司")]
        public int? comFid { get; set; }
    }
}

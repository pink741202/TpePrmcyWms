using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("AlertNotification")]
    public partial class AlertNotification
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "警告類型")]
        public int AlertType { get; set; }
        [StringLength(25)]
        [Display(Name = "警告資料表來源")]
        public string SourceTable { get; set; } = "";
        [Display(Name = "警告資料紀錄來源")]
        public int? SourceFid { get; set; }
        [Display(Name = "警告資料紀錄來源")]
        public Guid? SourceGid { get; set; }
        [Display(Name = "警告資料表來源備註")]
        public string SourceNote { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "警告標題")]
        public string AlertTitle { get; set; } = "";
        [Display(Name = "警告內容")]
        public string AlertContent { get; set; } = "";
        [Display(Name = "寄給人員")]
        public string? SendTo { get; set; } = "";
        [Display(Name = "警告寄出時間")]
        [DataType(DataType.DateTime)]
        public DateTime? SendTime { get; set; }
        [Display(Name = "警告修復時間")]
        [DataType(DataType.DateTime)]
        public DateTime? FixedTime { get; set; }
        [Display(Name = "警告新增時間")]
        [DataType(DataType.DateTime)]
        public DateTime adddate { get; set; }
        [Display(Name = "警告修改時間")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; }
    }
}

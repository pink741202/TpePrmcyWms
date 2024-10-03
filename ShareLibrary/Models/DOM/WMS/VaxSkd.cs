using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("VaxSkd")]
    public partial class VaxSkd
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "場次名稱")]
        [StringLength(100)]
        public string VaxSkdTitle { get; set; } = "";

        [Required]
        [Display(Name = "場次日期")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime VaxDate { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Display(Name = "時段")]
        [StringLength(10)]
        public string? VaxTimePeriod { get; set; } = "1";

        [Display(Name = "院內/院外")]
        [StringLength(10)]
        public string? InOutHsptl { get; set; } = "IN";

        [Display(Name = "地區")]
        [StringLength(10)]
        public string? VaxDist { get; set; } = "";

        [Display(Name = "對象")]
        [StringLength(100)]
        public string? VaxTarget { get; set; } = "";

        [Display(Name = "是否結案")]
        public bool? CaseClose { get; set; } = false;

        [Display(Name = "所屬公司")] //操作人員
        public int? comFid { get; set; }

        [Display(Name = "所屬部門")] //操作人員
        public int? dptFid { get; set; }

        [Display(Name = "操作人員")]
        public int? modid { get; set; }

        [Display(Name = "操作時間")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

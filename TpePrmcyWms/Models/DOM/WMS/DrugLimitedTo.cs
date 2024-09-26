using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("DrugLimitedTo")]
    public partial class DrugLimitedTo
    {
        [Key]
        public int FID { get; set; }
        [Display(Name = "藥品")]
        public int DrugFid { get; set; }
        [Display(Name = "類別")]
        [StringLength(20)]
        public string ActiveType { get; set; } = "";
        [Display(Name = "病歷號")]
        [StringLength(8)]
        public string? TargetPatient { get; set; } = "";

        [Display(Name = "數量")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? Qty { get; set; }

        [Display(Name = "所屬院區")]
        public int comFid { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

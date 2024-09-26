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
        [Display(Name = "�ī~")]
        public int DrugFid { get; set; }
        [Display(Name = "���O")]
        [StringLength(20)]
        public string ActiveType { get; set; } = "";
        [Display(Name = "�f����")]
        [StringLength(8)]
        public string? TargetPatient { get; set; } = "";

        [Display(Name = "�ƶq")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? Qty { get; set; }

        [Display(Name = "���ݰ|��")]
        public int comFid { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

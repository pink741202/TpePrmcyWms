using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("DrugPackage")]
    public partial class DrugPackage
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "�ī~")]        
        public int DrugFid { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "�γB")] //��J�ƶqinput / �w�s����ഫstockdisp
        public string UseFor { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Required]
        [Display(Name = "���q")]
        public decimal UnitWeight { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�ƶq")]
        public decimal UnitQty { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�`��")]
        public decimal? TotalWeight { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�]�˭��q")]
        public decimal? PackageWeight { get; set; }
        [Display(Name = "���X")]
        [StringLength(24)]
        public string? BarcodeNo { get; set; } = "";
        [Required]
        [StringLength(8)]
        [Display(Name = "���")]
        public string UnitTitle { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        [NotMapped]
        public string? DrugName { get; set; } = "";
        
    }
}

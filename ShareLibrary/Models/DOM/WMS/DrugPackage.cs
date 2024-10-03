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
        [Display(Name = "藥品")]        
        public int DrugFid { get; set; }
        [Required]
        [StringLength(16)]
        [Display(Name = "用處")] //輸入數量input / 庫存單位轉換stockdisp
        public string UseFor { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Required]
        [Display(Name = "重量")]
        public decimal UnitWeight { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "數量")]
        public decimal UnitQty { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "總重")]
        public decimal? TotalWeight { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "包裝重量")]
        public decimal? PackageWeight { get; set; }
        [Display(Name = "條碼")]
        [StringLength(24)]
        public string? BarcodeNo { get; set; } = "";
        [Required]
        [StringLength(8)]
        [Display(Name = "單位")]
        public string UnitTitle { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        [NotMapped]
        public string? DrugName { get; set; } = "";
        
    }
}

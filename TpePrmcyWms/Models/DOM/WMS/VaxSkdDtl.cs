using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using TpePrmcyWms.Models.Unit.Back;

namespace TpePrmcyWms.Models.DOM
{
    [Table("VaxSkdDtl")]
    public partial class VaxSkdDtl
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "場次")]
        public int VaxSkdFid { get; set; }
        [Required]
        [Display(Name = "疫苗名稱")]
        public int DrugFid { get; set; }

        [StringLength(20)]
        [Display(Name = "批號")] 
        public string? BatchNo { get; set; } = "";
        [Display(Name = "效期")] 
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }


        [Display(Name = "QRCode")]
        [StringLength(50)]
        public string? QRCodeLink { get; set; } = "";

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "備貨數")]
        public decimal? QtyStockUp { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "實發數")]
        public decimal? QtyStockOut { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "施打數")]
        public decimal? QtyVax { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "歸還數")]
        public decimal? QtyReturn { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "實收數")]
        public decimal? QtyReceive { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "入庫數")]
        public decimal? QtyStockIn { get; set; }

        [Display(Name = "操作人員")]
        public int? modid { get; set; }

        [Display(Name = "操作時間")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; } = DateTime.Now;


        [NotMapped]
        public string? DrugCode { get; set; }
        [NotMapped]
        public string? DrugName { get; set; } = "";
    }
}

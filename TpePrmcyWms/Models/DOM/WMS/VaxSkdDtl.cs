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
        [Display(Name = "����")]
        public int VaxSkdFid { get; set; }
        [Required]
        [Display(Name = "�̭]�W��")]
        public int DrugFid { get; set; }

        [StringLength(20)]
        [Display(Name = "�帹")] 
        public string? BatchNo { get; set; } = "";
        [Display(Name = "�Ĵ�")] 
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }


        [Display(Name = "QRCode")]
        [StringLength(50)]
        public string? QRCodeLink { get; set; } = "";

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "�Ƴf��")]
        public decimal? QtyStockUp { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "��o��")]
        public decimal? QtyStockOut { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "�I����")]
        public decimal? QtyVax { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "�k�ټ�")]
        public decimal? QtyReturn { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "�ꦬ��")]
        public decimal? QtyReceive { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        [Display(Name = "�J�w��")]
        public decimal? QtyStockIn { get; set; }

        [Display(Name = "�ާ@�H��")]
        public int? modid { get; set; }

        [Display(Name = "�ާ@�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; } = DateTime.Now;


        [NotMapped]
        public string? DrugCode { get; set; }
        [NotMapped]
        public string? DrugName { get; set; } = "";
    }
}

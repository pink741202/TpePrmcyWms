using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyKiosk.Models.DOM
{
    [Table("DrugGrid")]
    public partial class DrugGrid
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "���d")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "�Į�")]
        public int DrawFid { get; set; }
        [Required]
        [Display(Name = "�ī~")]
        public int DrugFid { get; set; }
        [Display(Name = "�w�s��")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Qty { get; set; }
        [Display(Name = "�w�s�W��")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? MaxLimitQty { get; set; }

        [Display(Name = "ĵ�٭�")]
        public decimal SafetyStock { get; set; }
        [Display(Name = "�`�ƶq")]
        public decimal? StandingStock { get; set; }
        [StringLength(2)]
        [Display(Name = "�L�I�覡")]
        public string? StockTakeType { get; set; } = "";        
        [Display(Name = "�O�_�ݭn�q��")]
        public bool? Alert { get; set; }

        [Display(Name = "�ĳ洫���~")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? UnitConvert { get; set; }
        [Display(Name = "�帹������")]
        public bool? BatchActive { get; set; }
        [Display(Name = "�ݫ���ĳ�R�P")]
        public bool? OffsetActive { get; set; }
        [Display(Name = "�R�P������m�d��")]
        public int? OffsetTo { get; set; }        
        [Display(Name = "���٪Ų~")]
        public bool? ReturnEmptyBottle { get; set; }
        [Display(Name = "�C�龯�q�W��")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? DailyMaxTake { get; set; }


        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;


        //NotMapped
        [NotMapped]
        public string? CbntName { get; set; } = "";
        [NotMapped]
        [Display(Name = "��P��")]
        public int? DrawerNo { get; set; }
        [NotMapped]
        [Display(Name = "�ī~�N�X")]
        public string? DrugCode { get; set; }
        [NotMapped]
        public string? DrugName { get; set; } = "";
        [NotMapped]
        [Display(Name = "�R�P�ĳ檺���d")]
        public int? OffsetCbntFid { get; set; }
        [NotMapped]
        [Display(Name = "�R�P�ĳ檺�Į�")]
        public int? OffsetDrawFid { get; set; }
    }
}

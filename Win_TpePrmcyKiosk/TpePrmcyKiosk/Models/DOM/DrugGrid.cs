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
        [Display(Name = "藥櫃")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "藥格")]
        public int DrawFid { get; set; }
        [Required]
        [Display(Name = "藥品")]
        public int DrugFid { get; set; }
        [Display(Name = "庫存值")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Qty { get; set; }
        [Display(Name = "庫存上限")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? MaxLimitQty { get; set; }

        [Display(Name = "警戒值")]
        public decimal SafetyStock { get; set; }
        [Display(Name = "常備量")]
        public decimal? StandingStock { get; set; }
        [StringLength(2)]
        [Display(Name = "盤點方式")]
        public string? StockTakeType { get; set; } = "";        
        [Display(Name = "是否需要通知")]
        public bool? Alert { get; set; }

        [Display(Name = "藥單換算整瓶")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? UnitConvert { get; set; }
        [Display(Name = "批號為必填")]
        public bool? BatchActive { get; set; }
        [Display(Name = "需後補藥單沖銷")]
        public bool? OffsetActive { get; set; }
        [Display(Name = "沖銷對應放置櫃位")]
        public int? OffsetTo { get; set; }        
        [Display(Name = "需還空瓶")]
        public bool? ReturnEmptyBottle { get; set; }
        [Display(Name = "每日劑量上限")]
        [Column(TypeName = "decimal(6, 2)")]
        public decimal? DailyMaxTake { get; set; }


        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;


        //NotMapped
        [NotMapped]
        public string? CbntName { get; set; } = "";
        [NotMapped]
        [Display(Name = "抽屜號")]
        public int? DrawerNo { get; set; }
        [NotMapped]
        [Display(Name = "藥品代碼")]
        public string? DrugCode { get; set; }
        [NotMapped]
        public string? DrugName { get; set; } = "";
        [NotMapped]
        [Display(Name = "沖銷藥單的藥櫃")]
        public int? OffsetCbntFid { get; set; }
        [NotMapped]
        [Display(Name = "沖銷藥單的藥格")]
        public int? OffsetDrawFid { get; set; }
    }
}

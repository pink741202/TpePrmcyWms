using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("MapDrugFreqOnCode")]
    public partial class MapDrugFreqOnCode
    {
        [Key]
        public int FID { get; set; }

        [Display(Name = "院頻代碼")]
        [StringLength(20)]
        public string? HsptlFreqCode { get; set; } = "";

        [Display(Name = "頻次英文名稱")]
        [StringLength(200)]
        public string? FreqEnName { get; set; } = "";

        [Display(Name = "頻次中文名稱")]
        [StringLength(200)]
        public string? FreqName { get; set; } = "";

        [Display(Name = "間隔日")]
        public int? FreqDaily { get; set; } 

        [Display(Name = "間隔次")]
        public int? FreqTime { get; set; }

        [Display(Name = "時間點")]
        [StringLength(200)]
        public string? FreqTimePoint { get; set; } = "";

        [Display(Name = "時間點敘述")]
        [StringLength(200)]
        public string? FreqTimeDesc { get; set; } = "";

        [Display(Name = "所屬院區")]
        public int? comFid { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

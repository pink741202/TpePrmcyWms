using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("Cabinet")]
    public partial class Cabinet
    {
        [Key]
        public int FID { get; set; }

        [Required]
        public Guid StoreGuid { get; set; } = new Guid();
        [Required]
        [StringLength(100)]
        [Display(Name = "藥櫃名稱")]
        public string CbntName { get; set; } = "";
        [Display(Name = "藥櫃敘述")]
        public string CbntDesc { get; set; } = "";
        [Display(Name = "盤點設定-星期")]
        [StringLength(16)]
        public string? StockTakeConfig_Day { get; set; } = "";
        [Display(Name = "盤點設定-時段")]
        [StringLength(50)]
        public string? StockTakeConfig_Time { get; set; } = "";


        [Display(Name = "所屬院區")]
        public int? comFid { get; set; }
        [Display(Name = "所屬單位")]
        public int? dptFid { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        //NotMapped
        [NotMapped]
        [Range(0, 200, ErrorMessage = "數量設定範圍{1} ~ {2}")]
        [Display(Name = "藥格數量")]
        public int DrawerCount { get; set; } = 0;

        [NotMapped]
        public List<string> StockTakeConfig_TimeList = new List<string>();
    }
}

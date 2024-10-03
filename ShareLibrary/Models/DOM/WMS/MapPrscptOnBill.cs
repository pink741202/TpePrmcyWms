using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("MapPrscptOnBill")]
    public partial class MapPrscptOnBill
    {
        [Key]
        public int FID { get; set; }
        [Required]
        [Display(Name = "異動單類型")]
        public string BillType { get; set; } = "";        
        [Required]
        [Display(Name = "異動單id")]
        public int StockbillFid { get; set; }
        [Required]
        [Display(Name = "藥單id")]
        public int PrscptFid { get; set; }

        [Display(Name = "沖銷查詢關鍵字")]
        public string? OffsetQryKey { get; set; } = "";
        [Display(Name = "沖銷群組")]
        [StringLength(12)]
        public string? OffsetGroup { get; set; } = "";
        [Display(Name = "住院退藥藥單")]
        [StringLength(13)]
        public string? ReturnSheet { get; set; }
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

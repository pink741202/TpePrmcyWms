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
        [Display(Name = "���ʳ�����")]
        public string BillType { get; set; } = "";        
        [Required]
        [Display(Name = "���ʳ�id")]
        public int StockbillFid { get; set; }
        [Required]
        [Display(Name = "�ĳ�id")]
        public int PrscptFid { get; set; }

        [Display(Name = "�R�P�d������r")]
        public string? OffsetQryKey { get; set; } = "";
        [Display(Name = "�R�P�s��")]
        [StringLength(12)]
        public string? OffsetGroup { get; set; } = "";
        [Display(Name = "��|�h���ĳ�")]
        [StringLength(13)]
        public string? ReturnSheet { get; set; }
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

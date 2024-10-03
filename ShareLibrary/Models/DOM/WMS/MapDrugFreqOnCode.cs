using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("MapDrugFreqOnCode")]
    public partial class MapDrugFreqOnCode
    {
        [Key]
        public int FID { get; set; }

        [Display(Name = "�|�W�N�X")]
        [StringLength(20)]
        public string? HsptlFreqCode { get; set; } = "";

        [Display(Name = "�W���^��W��")]
        [StringLength(200)]
        public string? FreqEnName { get; set; } = "";

        [Display(Name = "�W������W��")]
        [StringLength(200)]
        public string? FreqName { get; set; } = "";

        [Display(Name = "���j��")]
        public int? FreqDaily { get; set; } 

        [Display(Name = "���j��")]
        public int? FreqTime { get; set; }

        [Display(Name = "�ɶ��I")]
        [StringLength(200)]
        public string? FreqTimePoint { get; set; } = "";

        [Display(Name = "�ɶ��I�ԭz")]
        [StringLength(200)]
        public string? FreqTimeDesc { get; set; } = "";

        [Display(Name = "���ݰ|��")]
        public int? comFid { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

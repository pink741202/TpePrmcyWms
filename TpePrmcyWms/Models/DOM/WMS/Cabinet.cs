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
        [Display(Name = "���d�W��")]
        public string CbntName { get; set; } = "";
        [Display(Name = "���d�ԭz")]
        public string CbntDesc { get; set; } = "";
        [Display(Name = "�L�I�]�w-�P��")]
        [StringLength(16)]
        public string? StockTakeConfig_Day { get; set; } = "";
        [Display(Name = "�L�I�]�w-�ɬq")]
        [StringLength(50)]
        public string? StockTakeConfig_Time { get; set; } = "";


        [Display(Name = "���ݰ|��")]
        public int? comFid { get; set; }
        [Display(Name = "���ݳ��")]
        public int? dptFid { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        //NotMapped
        [NotMapped]
        [Range(0, 200, ErrorMessage = "�ƶq�]�w�d��{1} ~ {2}")]
        [Display(Name = "�Į�ƶq")]
        public int DrawerCount { get; set; } = 0;

        [NotMapped]
        public List<string> StockTakeConfig_TimeList = new List<string>();
    }
}

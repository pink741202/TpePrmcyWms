using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("MapMenuOnCbnt")]
    public partial class MapMenuOnCbnt
    {
        [Key]
        public Guid GID { get; set; }

        [Required]
        [Display(Name = "櫃子")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "功能")]
        public int MnLFid { get; set; }
        [Required]
        [Display(Name = "可使用")]
        public bool Able { get; set; } = false;
        
        [Display(Name = "取代原名稱")]
        public string? MnLTitle { get; set; }

    }
}

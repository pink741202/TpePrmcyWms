using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyKiosk.Models.DOM
{
    [Table("Drawers")]
    public partial class Drawers
    {
        [Key]
        public int FID { get; set; }
        [Required]
        [Display(Name = "所屬櫃子")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "藥格編號")]
        public int No { get; set; }
        [Display(Name = "所屬院區")]
        public int? comFid { get; set; }
    }
}

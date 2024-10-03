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
        [Display(Name = "�����d�l")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "�Į�s��")]
        public int No { get; set; }
        [Display(Name = "���ݰ|��")]
        public int? comFid { get; set; }
    }
}

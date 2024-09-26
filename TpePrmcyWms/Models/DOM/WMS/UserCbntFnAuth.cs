using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("UserCbntFnAuth")]
    public partial class UserCbntFnAuth
    {
        [Key]
        public Guid GID { get; set; }
        [Required]
        [Display(Name = "�ؿ�")]
        public int EmpFid { get; set; }
        [Required]
        [Display(Name = "�d�l")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "�ؿ�")]
        public int MnLFid { get; set; }
        [Display(Name = "�ҥ�")]
        public bool Active { get; set; }
    }
}

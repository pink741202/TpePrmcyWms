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
        [Display(Name = "目錄")]
        public int EmpFid { get; set; }
        [Required]
        [Display(Name = "櫃子")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "目錄")]
        public int MnLFid { get; set; }
        [Display(Name = "啟用")]
        public bool Active { get; set; }
    }
}

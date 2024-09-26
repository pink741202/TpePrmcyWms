using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("AuthRole")]
    public partial class AuthRole
    {
        [Key]
        public int FID { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; }
        [Display(Name = "所屬公司")]
        public int? comFid { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; }
    }
}

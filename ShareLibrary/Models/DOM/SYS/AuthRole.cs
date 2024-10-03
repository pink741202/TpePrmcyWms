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
        [Display(Name = "����W��")]
        public string RoleName { get; set; }
        [Display(Name = "���ݤ��q")]
        public int? comFid { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; }
    }
}

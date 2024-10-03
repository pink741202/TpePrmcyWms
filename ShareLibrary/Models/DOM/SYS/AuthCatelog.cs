using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("AuthCatelog")]
    public partial class AuthCatelog
    {
        [Key]
        public int FID { get; set; }
        [Required]
        [Display(Name = "����")]
        public int? RoleFid { get; set; }
        [Display(Name = "�ϥΪ�")]
        public int? EmpFid { get; set; }
        [Required]
        [Display(Name = "�ؿ�")]
        public int MenuLFid { get; set; }
        [Display(Name = "�d���v��")]
        public bool Queryable { get; set; } = false;
        [Display(Name = "�s�W�v��")]
        public bool Creatable { get; set; } = false;
        [Display(Name = "�ק��v��")]
        public bool Updatable { get; set; } = false;
        [Display(Name = "�R���v��")]
        public bool Deletable { get; set; } = false;
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; }
    }
}

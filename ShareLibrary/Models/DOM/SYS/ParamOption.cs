using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("ParamOption")]
    public partial class ParamOption
    {
        [Key]
        public int FID { get; set; }

        [StringLength(20)]
        [Display(Name = "�s�եN�X")]
        [RegularExpression(@"^[A-Za-z0-9]+$")]
        public string GroupCode { get; set; }

        [StringLength(50)]
        [Display(Name = "�s�զW��")]
        public string GroupName { get; set; }

        [StringLength(20)]
        [Display(Name = "�ѼƥN�X")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage ="�u���J���^��r!")]
        public string OptionCode { get; set; }

        [StringLength(100)]
        [Display(Name = "�ѼƦW��")]
        public string OptionName { get; set; }
        [StringLength(10)]
        [Display(Name = "���ݨt��")]
        public string SysType { get; set; }
        [Display(Name = "����")]
        public int Sorting { get; set; }

        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; }
    }
}

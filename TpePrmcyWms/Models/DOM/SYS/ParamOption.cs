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
        [Display(Name = "群組代碼")]
        [RegularExpression(@"^[A-Za-z0-9]+$")]
        public string GroupCode { get; set; }

        [StringLength(50)]
        [Display(Name = "群組名稱")]
        public string GroupName { get; set; }

        [StringLength(20)]
        [Display(Name = "參數代碼")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage ="只能輸入中英文字!")]
        public string OptionCode { get; set; }

        [StringLength(100)]
        [Display(Name = "參數名稱")]
        public string OptionName { get; set; }
        [StringLength(10)]
        [Display(Name = "所屬系統")]
        public string SysType { get; set; }
        [Display(Name = "順序")]
        public int Sorting { get; set; }

        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; }
    }
}

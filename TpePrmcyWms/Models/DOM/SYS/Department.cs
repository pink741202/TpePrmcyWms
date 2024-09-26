using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TpePrmcyWms.Models.DOM
{
    

    [Table("Department")]
    public partial class Department
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "部門代碼")]
        public string dptid { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "部門名稱")]
        public string dpttitle { get; set; }
        [Display(Name = "所屬公司")]
        public int comFid { get; set; }

        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; }
    }
}

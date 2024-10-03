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
        [Display(Name = "角色")]
        public int? RoleFid { get; set; }
        [Display(Name = "使用者")]
        public int? EmpFid { get; set; }
        [Required]
        [Display(Name = "目錄")]
        public int MenuLFid { get; set; }
        [Display(Name = "查詢權限")]
        public bool Queryable { get; set; } = false;
        [Display(Name = "新增權限")]
        public bool Creatable { get; set; } = false;
        [Display(Name = "修改權限")]
        public bool Updatable { get; set; } = false;
        [Display(Name = "刪除權限")]
        public bool Deletable { get; set; } = false;
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; }
    }
}

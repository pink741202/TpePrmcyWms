using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    //驗證屬性
    //https://learn.microsoft.com/zh-tw/aspnet/core/mvc/models/validation?view=aspnetcore-8.0

    [Table("employee")]
    public partial class employee
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "姓名")]
        public string name { get; set; } = "";
        
        [Required]        
        [StringLength(20, ErrorMessage = "{0} 字數長度需在 {2} ~ {1} 之間", MinimumLength = 6)]
        [Display(Name = "登入密碼")]
        public string opensesame { get; set; }  //SHA.GenerateSHA512String(entity.EmpPassword + entity.EmpNo)
        [Required]
        [StringLength(20, ErrorMessage = "{0} 字數長度需在 {2} ~ {1} 之間", MinimumLength = 6)]
        [Display(Name = "登入帳號")]
        public string empacc { get; set; }
        [StringLength(50)]
        [Display(Name = "英文名")]
        public string? enname { get; set; }
        [Display(Name = "使用中")]
        public bool ifuse { get; set; } = true;

        [StringLength(2)]
        public string emptype { get; set; } = "1";

        [StringLength(1)]
        public string empstatus { get; set; } = "1";
        [Required]
        [Display(Name = "所屬公司")]
        [Range(1, Int32.MaxValue, ErrorMessage = "請選擇所屬公司")]
        public int? comFid { get; set; }
        [Display(Name = "所屬部門")]
        public int? dptFid { get; set; }
        [Display(Name = "權限角色")]
        [Range(1, Int32.MaxValue, ErrorMessage = "請選擇權限角色")]
        public int? RoleFid { get; set; }
        [Display(Name = "同步權限角色")]
        public bool SyncAsRole { get; set; } = true;
        [Required]
        [Range(10, 100)]
        [Display(Name = "列表筆數")]
        public int pagesize { get; set; } = 16;
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; }
        [Display(Name = "訂閱通知")]
        public bool? Notified { get; set; } = false;

        //api同步資料
        [StringLength(10)]
        [Display(Name = "員工編號")]
        public string? emp_no { get; set; } = "";
        [StringLength(10)]
        public string? emp_dep_code { get; set; } = "";
        [StringLength(10)]
        public string? emp_pos_code { get; set; } = "";
        [StringLength(20)]
        public string? emp_pos_name { get; set; } = "";
        [StringLength(20)]
        public string? emp_location { get; set; } = "";
        [StringLength(10)]
        public string? emp_cost_center { get; set; } = "";
        [StringLength(10)]
        public string? emp_birth { get; set; } = "";
        [StringLength(12)]
        [Display(Name = "卡號")]
        public string? CardNo { get; set; } = "";
        [StringLength(12)]
        public string? mobile_tel { get; set; } = "";
        [StringLength(10)]
        public string? mobile_code { get; set; } = "";
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "非有效email格式")]
        [Display(Name = "E-mail")]
        public string? email { get; set; } = ""; //emp_no + @tpech.gov.tw

    }
}

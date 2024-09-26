using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    //�����ݩ�
    //https://learn.microsoft.com/zh-tw/aspnet/core/mvc/models/validation?view=aspnetcore-8.0

    [Table("employee")]
    public partial class employee
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "�m�W")]
        public string name { get; set; } = "";
        
        [Required]        
        [StringLength(20, ErrorMessage = "{0} �r�ƪ��׻ݦb {2} ~ {1} ����", MinimumLength = 6)]
        [Display(Name = "�n�J�K�X")]
        public string opensesame { get; set; }  //SHA.GenerateSHA512String(entity.EmpPassword + entity.EmpNo)
        [Required]
        [StringLength(20, ErrorMessage = "{0} �r�ƪ��׻ݦb {2} ~ {1} ����", MinimumLength = 6)]
        [Display(Name = "�n�J�b��")]
        public string empacc { get; set; }
        [StringLength(50)]
        [Display(Name = "�^��W")]
        public string? enname { get; set; }
        [Display(Name = "�ϥΤ�")]
        public bool ifuse { get; set; } = true;

        [StringLength(2)]
        public string emptype { get; set; } = "1";

        [StringLength(1)]
        public string empstatus { get; set; } = "1";
        [Required]
        [Display(Name = "���ݤ��q")]
        [Range(1, Int32.MaxValue, ErrorMessage = "�п�ܩ��ݤ��q")]
        public int? comFid { get; set; }
        [Display(Name = "���ݳ���")]
        public int? dptFid { get; set; }
        [Display(Name = "�v������")]
        [Range(1, Int32.MaxValue, ErrorMessage = "�п���v������")]
        public int? RoleFid { get; set; }
        [Display(Name = "�P�B�v������")]
        public bool SyncAsRole { get; set; } = true;
        [Required]
        [Range(10, 100)]
        [Display(Name = "�C����")]
        public int pagesize { get; set; } = 16;
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; }
        [Display(Name = "�q�\�q��")]
        public bool? Notified { get; set; } = false;

        //api�P�B���
        [StringLength(10)]
        [Display(Name = "���u�s��")]
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
        [Display(Name = "�d��")]
        public string? CardNo { get; set; } = "";
        [StringLength(12)]
        public string? mobile_tel { get; set; } = "";
        [StringLength(10)]
        public string? mobile_code { get; set; } = "";
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "�D����email�榡")]
        [Display(Name = "E-mail")]
        public string? email { get; set; } = ""; //emp_no + @tpech.gov.tw

    }
}

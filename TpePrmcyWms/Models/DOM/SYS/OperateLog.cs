using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("OperateLog")]
    public partial class OperateLog
    {
        [Key]
        public int FID { get; set; }
        [Display(Name = "�ϥΪ�")]
        public int empFid { get; set; }
        [Display(Name = "�ާ@����")]
        public string? LinkMethod { get; set; }
        [Display(Name = "��������")]
        public string? LogType { get; set; } = "";
        [Display(Name = "�ާ@����")]
        public string? OperateType { get; set; } = "";
        [Display(Name = "�ާ@�T��")]
        public string? LogMsg { get; set; } = "";
        [Display(Name = "�ާ@���G")]
        public bool? OperateSuccess { get; set; }
        [Display(Name = "�o�Ϳ��~������")]
        public string? ErrorClass { get; set; } = "";
        [Display(Name = "�o�Ϳ��~���\��")]
        public string? ErrorFunction { get; set; } = "";
        [Display(Name = "���~�T��")]
        public string? ErrorMsg { get; set; } = "";

        [Column(TypeName = "ntext")]
        [Display(Name = "���~�y��")]
        public string? ErrorTrace { get; set; } = "";

        [Required]
        [Display(Name = "�T���ɶ�")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime LogTime { get; set; }
        [Display(Name = "���q")]
        public int? comFid { get; set; }
    }
}

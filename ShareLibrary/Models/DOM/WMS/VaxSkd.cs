using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("VaxSkd")]
    public partial class VaxSkd
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "�����W��")]
        [StringLength(100)]
        public string VaxSkdTitle { get; set; } = "";

        [Required]
        [Display(Name = "�������")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime VaxDate { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Display(Name = "�ɬq")]
        [StringLength(10)]
        public string? VaxTimePeriod { get; set; } = "1";

        [Display(Name = "�|��/�|�~")]
        [StringLength(10)]
        public string? InOutHsptl { get; set; } = "IN";

        [Display(Name = "�a��")]
        [StringLength(10)]
        public string? VaxDist { get; set; } = "";

        [Display(Name = "��H")]
        [StringLength(100)]
        public string? VaxTarget { get; set; } = "";

        [Display(Name = "�O�_����")]
        public bool? CaseClose { get; set; } = false;

        [Display(Name = "���ݤ��q")] //�ާ@�H��
        public int? comFid { get; set; }

        [Display(Name = "���ݳ���")] //�ާ@�H��
        public int? dptFid { get; set; }

        [Display(Name = "�ާ@�H��")]
        public int? modid { get; set; }

        [Display(Name = "�ާ@�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; } = DateTime.Now;

    }
}

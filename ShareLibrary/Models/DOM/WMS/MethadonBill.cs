using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("MethadonBill")]
    public partial class MethadonBill
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "���")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "���Įɶ�")]
        public DateTime? TakeTime { get; set; }
        [Display(Name = "���İ����Įv")]
        public int? TakeEmpFid { get; set; }
        [Display(Name = "�����@�z�v")]
        public int? TakeSuperFid { get; set; }
        [Display(Name = "���ĭ��q")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeWeight { get; set; } = 0;
        [Display(Name = "����cc")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeCC { get; set; } = 0;
        [Display(Name = "���Įɶ�")]
        public DateTime? RetnTime { get; set; }
        [Display(Name = "���İ����Įv")]
        public int? RetnEmpFid { get; set; }
        [Display(Name = "�����@�z�v")]
        public int? RetnSuperFid { get; set; }
        [Display(Name = "���ĭ��q")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RetnWeight { get; set; } = 0;
        [Display(Name = "����cc")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RetnCC { get; set; } = 0;
        [Display(Name = "�A�ĤH��/��")]
        public int? UsedPatientCnt { get; set; }
        [Display(Name = "�ϥζq/��")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? UsedCC { get; set; }
        [Display(Name = "�t�ε��l�q")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? SysRemainCC { get; set; }
        [Display(Name = "�L�I���l�q")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeRemainCC { get; set; }
        [Display(Name = "�L����")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? StockTakeBalance { get; set; }
        [Display(Name = "�s�W�ɶ�")]
        public DateTime adddate { get; set; } = DateTime.Now;
        [Display(Name = "���ʮɶ�")]
        public DateTime? moddate { get; set; }
        
    }
}

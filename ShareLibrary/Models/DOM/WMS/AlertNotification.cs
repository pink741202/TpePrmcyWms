using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("AlertNotification")]
    public partial class AlertNotification
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "ĵ�i����")]
        public int AlertType { get; set; }
        [StringLength(25)]
        [Display(Name = "ĵ�i��ƪ�ӷ�")]
        public string SourceTable { get; set; } = "";
        [Display(Name = "ĵ�i��Ƭ����ӷ�")]
        public int? SourceFid { get; set; }
        [Display(Name = "ĵ�i��Ƭ����ӷ�")]
        public Guid? SourceGid { get; set; }
        [Display(Name = "ĵ�i��ƪ�ӷ��Ƶ�")]
        public string SourceNote { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "ĵ�i���D")]
        public string AlertTitle { get; set; } = "";
        [Display(Name = "ĵ�i���e")]
        public string AlertContent { get; set; } = "";
        [Display(Name = "�H���H��")]
        public string? SendTo { get; set; } = "";
        [Display(Name = "ĵ�i�H�X�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime? SendTime { get; set; }
        [Display(Name = "ĵ�i�״_�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime? FixedTime { get; set; }
        [Display(Name = "ĵ�i�s�W�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime adddate { get; set; }
        [Display(Name = "ĵ�i�ק�ɶ�")]
        [DataType(DataType.DateTime)]
        public DateTime? moddate { get; set; }
    }
}

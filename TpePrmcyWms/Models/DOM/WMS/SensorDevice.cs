using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TpePrmcyWms.Models.DOM
{

    [Table("SensorDevice")]
    public partial class SensorDevice
    {
        [Key]
        public int FID { get; set; }

        [StringLength(20)]
        [Display(Name = "�ؼ�����")]
        public string? TargetTable { get; set; }
        [Display(Name = "�ؼЪ���")]
        public int? TargetObjFid { get; set; }        
        [Required]
        [StringLength(10)]
        [Display(Name = "�P��������")]
        public string SensorType { get; set; }

        [Required]
        [StringLength(12)]
        [Display(Name = "�s��")]
        public string SensorNo { get; set; }

        [StringLength(5)]
        [Display(Name = "��")]
        public string SerialPort { get; set; }
        [Display(Name = "�w���}")]
        [Range(maximum:8,minimum:1,ErrorMessage = "�Ʀr�d��ݤ���{1}��{2}")]
        public int? Modbus_Addr { get; set; }
        [Display(Name = "�u���}")]
        [Range(maximum: 4000, minimum: 0, ErrorMessage = "�Ʀr�d��ݤ���{1}��{2}")]
        public int? Modbus_Rgst { get; set; }
        [Display(Name = "16�i����O")]
        public string? Modbus_X16 { get; set; }
        [Display(Name = "���q")]
        public int? comFid { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        //NotMapped
        [Display(Name = "���d")]
        [NotMapped]
        [ValidateNever]
        public int? CbntFid { get; set; } = 0;
        [Display(Name = "�Į�")]
        [NotMapped]
        [ValidateNever]
        public int? DrawFid { get; set; } = 0;
        [Display(Name = "�ī~")]
        [NotMapped]
        [ValidateNever]
        public int? DrGridFid { get; set; } = 0;
        [NotMapped]
        [ValidateNever]
        [Display(Name = "���⭫�q���")]
        public int? PackageFid { get; set; } = 0;
        [NotMapped]
        [ValidateNever]
        public int? MapPackOnSensorFid { get; set; } = 0;
    }
}

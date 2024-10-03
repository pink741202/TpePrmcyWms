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
        [Display(Name = "目標類型")]
        public string? TargetTable { get; set; }
        [Display(Name = "目標物件")]
        public int? TargetObjFid { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "設備類型")]
        public string? SensorType { get; set; } = "";
        [StringLength(20)]
        [Display(Name = "設備版本")]
        public string? SensorVersion { get; set; } = "";

        [Required]
        [StringLength(12)]
        [Display(Name = "編號")]
        public string? SensorNo { get; set; } = "";

        [StringLength(5)]
        [Display(Name = "埠號")]
        public string? SerialPort { get; set; } = "";
        [Display(Name = "硬體位址")]
        [Range(maximum: 8, minimum: 1, ErrorMessage = "數字範圍需介於{1}到{2}")]
        public int? Modbus_Addr { get; set; }
        [Display(Name = "線圈位址")]
        [Range(maximum: 4000, minimum: 0, ErrorMessage = "數字範圍需介於{1}到{2}")]
        public int? Modbus_Rgst { get; set; }
        [Display(Name = "指令")]
        public string? Modbus_Cmd { get; set; }
        [Display(Name = "壞掉")]
        public string? NotWork { get; set; } = "";
        [Display(Name = "壞掉時間")]
        [DataType(DataType.Date)]
        public DateTime? NotWorkTime { get; set; }
        [Display(Name = "公司")]
        public int? comFid { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "更新日期")]
        public DateTime? moddate { get; set; } = DateTime.Now;

        //NotMapped
        [Display(Name = "藥櫃")]
        [NotMapped]
        public int? CbntFid { get; set; } = 0;
        [Display(Name = "藥格")]
        [NotMapped]
        public int? DrawFid { get; set; } = 0;
        [Display(Name = "藥品")]
        [NotMapped]
        public int? DrGridFid { get; set; } = 0;
        [NotMapped]
        [Display(Name = "換算重量資料")]
        public int? PackageFid { get; set; } = 0;
        [NotMapped]
        public int? MapPackOnSensorFid { get; set; } = 0;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyKiosk.Models.DOM
{
    

    [Table("MapPackOnSensor")]
    public partial class MapPackOnSensor
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [Display(Name = "�P����")]
        public int SensorFid { get; set; }
        [Display(Name = "���⭫�q���")]
        public int? PackageFid { get; set; }

    }
}

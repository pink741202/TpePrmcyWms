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
        [Display(Name = "感應器")]
        public int SensorFid { get; set; }
        [Display(Name = "換算重量資料")]
        public int? PackageFid { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyKiosk.Models.DOM
{
    

    [Table("ScaleWeighQtyLog")]
    public partial class ScaleWeighQtyLog
    {
        [Key]
        public int FID { get; set; }
        public int? StockBillFid { get; set; }

        [Required]
        public int DrugGridFid { get; set; }

        [StringLength(12)]
        public string SensorNo { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Weight { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? Tolerance { get; set; }

        public bool? Trustable { get; set; }

        public DateTime? logtime { get; set; } = DateTime.Now;
        public int? comFid { get; set; }
        public int? dptFid { get; set; }
    }
}

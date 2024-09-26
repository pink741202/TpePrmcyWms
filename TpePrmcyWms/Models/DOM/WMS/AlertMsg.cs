using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("AlertMsg")]
    public partial class AlertMsg
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(16)]
        public string AlertType { get; set; } = "";
        
        public int? StockBillFid { get; set; }
        [DataType(DataType.Text)]
        public string? Msg { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime OccTime { get; set; } = DateTime.Now;
    }
}

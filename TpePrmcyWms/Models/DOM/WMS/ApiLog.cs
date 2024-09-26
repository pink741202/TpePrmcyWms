using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("ApiLog")]
    public partial class ApiLog
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(300)]
        public string UrlString { get; set; }
        [Required]
        public DateTime LogTime { get; set; } = DateTime.Now;
        [Required]
        public bool Success { get; set; }
        public int? StockBillFid { get; set; }
        
    }
}

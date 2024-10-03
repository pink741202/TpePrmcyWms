using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TpePrmcyWms.Models.DOM
{
    [Table("SensorComuQuee")]
    public partial class SensorComuQuee //介面與硬體的控制溝通用
    {
        [Key]
        public Guid GID { get; set; } = new Guid();
        [Required]
        [Display(Name = "藥櫃")]
        public int? CbntFid { get; set; } = 0;
        [Required]
        [StringLength(8)]
        [Display(Name = "藥品代碼")]
        public string DrugCode { get; set; } = "";
        [StringLength(4)]
        [Display(Name = "操作類別")] //同stockBill.BillType
        public string OperType { get; set; } = "";
        [Display(Name = "LED顏色")]
        [StringLength(20)]
        public string LEDColor { get; set; } = "";
        [Display(Name = "藥格")]
        public int? DrawFid { get; set; } = 0;
        [Display(Name = "藥格藥品")]
        public int? DrugGridFid { get; set; } = 0;
        [Display(Name = "Modbus端狀態")]
        [StringLength(1)]
        public string ssrState { get; set; } = "0";
        [Display(Name = "使用者操作端狀態")]
        [StringLength(1)]
        public string oprState { get; set; } = "";
        [Display(Name = "磅秤結果重量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ssrWeight { get; set; }
        [Display(Name = "磅秤結果數量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ssrQty { get; set; }
        [Display(Name = "庫存單")]
        public int? stockBillFid { get; set; } = 0;
        public int modid { get; set; } = 0;
        public DateTime moddate { get; set; } = DateTime.Now;
    }
}

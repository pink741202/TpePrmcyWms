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
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "領藥時間")]
        public DateTime? TakeTime { get; set; }
        [Display(Name = "領藥執行藥師")]
        public int? TakeEmpFid { get; set; }
        [Display(Name = "領藥護理師")]
        public int? TakeSuperFid { get; set; }
        [Display(Name = "領藥重量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeWeight { get; set; } = 0;
        [Display(Name = "領藥cc")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeCC { get; set; } = 0;
        [Display(Name = "還藥時間")]
        public DateTime? RetnTime { get; set; }
        [Display(Name = "還藥執行藥師")]
        public int? RetnEmpFid { get; set; }
        [Display(Name = "還藥護理師")]
        public int? RetnSuperFid { get; set; }
        [Display(Name = "還藥重量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RetnWeight { get; set; } = 0;
        [Display(Name = "還藥cc")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RetnCC { get; set; } = 0;
        [Display(Name = "服藥人數/日")]
        public int? UsedPatientCnt { get; set; }
        [Display(Name = "使用量/日")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? UsedCC { get; set; }
        [Display(Name = "系統結餘量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? SysRemainCC { get; set; }
        [Display(Name = "盤點結餘量")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TakeRemainCC { get; set; }
        [Display(Name = "盤盈虧")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? StockTakeBalance { get; set; }
        [Display(Name = "新增時間")]
        public DateTime adddate { get; set; } = DateTime.Now;
        [Display(Name = "異動時間")]
        public DateTime? moddate { get; set; }
        
    }
}

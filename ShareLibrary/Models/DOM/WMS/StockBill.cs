using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{   
    [Table("StockBill")]
    public partial class StockBill
    {
        [Key]        
        public int FID { get; set; }
        [Display(Name = "藥櫃")]
        public int? CbntFid { get; set; }
        [Display(Name = "藥品代碼")]
        public string DrugCode { get; set; } = "";
        [Display(Name = "藥格藥品")]
        public int DrugGridFid { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "數量")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal Qty { get; set; } = 0;
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "目標數量")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal TargetQty { get; set; } = 0;
        [Required]
        [StringLength(4)]
        [Display(Name = "操作類別")] //DFM:領藥多筆,DFB:領藥批次
        public string BillType { get; set; } = "";
        
        [Required]
        [Display(Name = "出/入 庫")] //false = 出(減項), true = 入(加項)
        public bool TradeType { get; set; }
                
        [Display(Name = "需求樓層")]
        public string? ToFloor { get; set; } = "";
        [StringLength(20)]
        [Display(Name = "批號")] //入庫用
        public string? BatchNo { get; set; } = "";
        [Display(Name = "效期")] //入庫用
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "單號來源")] //兩筆單有關連時的紀錄,如調入調出
        public int? FromFid { get; set; }
        [Column(TypeName = "text")]
        [Display(Name = "備註")]
        public string? RecNote { get; set; } = "";

        [Display(Name = "所屬公司")] //操作人員
        public int? comFid { get; set; }
        [Display(Name = "覆核人員")]
        public int? superFid { get; set; }
        [Display(Name = "建立人員")]        
        public int? addid { get; set; }
        [Display(Name = "建立時間")]
        public DateTime? adddate { get; set; }
        [Display(Name = "更新人員")]
        public int? modid { get; set; }
        [Display(Name = "更新時間")]
        public DateTime? moddate { get; set; }
        [Required]
        [Display(Name = "操作完成")]
        public bool JobDone { get; set; } = false;
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "系統計算的庫存量")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal SysChkQty { get; set; } = 0;
        
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "盤點的庫存量第一次")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? UserChk1Qty { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "盤點的庫存量第二次")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? UserChk2Qty { get; set; }
        [Display(Name = "盤點類型")]
        public string? TakeType { get; set; }

        //以下非資料庫有的欄位 -----------------------------
        
        [NotMapped] //輸入藥單介面用
        [Display(Name = "藥格")]
        public int? DrawFid { get; set; }        
        [NotMapped] //輸入藥單介面用
        [Display(Name = "藥品")]
        public int? DrugFid { get; set; }
        [NotMapped] //輸入藥單介面用
        [Display(Name = "藥品名稱")]
        public string DrugName { get; set; } = "";
        [NotMapped] //取消調入調出用
        public bool rejectBill { get; set; } = false;


        //[NotMapped] //輸入藥單介面用
        //[Display(Name = "掃瞄字串")]
        //public string Scantext { get; set; } = "";

        //[NotMapped] //輸入藥單介面用
        //[StringLength(8, ErrorMessage = "{0} 字數長度需為8個字", MinimumLength = 8)]
        //[Display(Name = "病歷號碼")]
        //public string? PatientNo { get; set; }
        //[NotMapped] //輸入藥單介面用
        //public string? PatientSeq { get; set; }
        //[NotMapped] //輸入藥單介面用
        //[StringLength(5, ErrorMessage = "{0} 字數長度需為5個字", MinimumLength = 5)]
        //[Display(Name = "領藥號碼")]
        //public string? PrscptNo { get; set; }
        //[NotMapped] //輸入藥單介面用
        //[Display(Name = "藥單日")] 
        //[Column(TypeName = "date")]
        //public DateTime? BillDate { get; set; }
        //[NotMapped] //輸入藥單介面用(退藥)
        //[Display(Name = "退藥單號")]
        //[StringLength(13)]
        //public string? ReturnSheet { get; set; }
        //[NotMapped] //存入map
        //[Display(Name = "藥袋")]
        //public List<int>? PrscptFid { get; set; } = null;

    }
}

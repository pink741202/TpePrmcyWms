using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{   
    [Table("StockBill")]
    public partial class StockBill
    {
        [Key]        
        public int FID { get; set; }
        [Display(Name = "���d")]
        public int? CbntFid { get; set; }
        [Display(Name = "�ī~�N�X")]
        public string DrugCode { get; set; } = "";
        [Display(Name = "�Į��ī~")]
        public int DrugGridFid { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�ƶq")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal Qty { get; set; } = 0;
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�ؼмƶq")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal TargetQty { get; set; } = 0;
        [Required]
        [StringLength(4)]
        [Display(Name = "�ާ@���O")] //DFM:���Ħh��,DFB:���ħ妸
        public string BillType { get; set; } = "";
        
        [Required]
        [Display(Name = "�X/�J �w")] //false = �X(�), true = �J(�[��)
        public bool TradeType { get; set; }
                
        [Display(Name = "�ݨD�Ӽh")]
        public string? ToFloor { get; set; } = "";
        [StringLength(20)]
        [Display(Name = "�帹")] //�J�w��
        public string? BatchNo { get; set; } = "";
        [Display(Name = "�Ĵ�")] //�J�w��
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "�渹�ӷ�")] //�ⵧ�榳���s�ɪ�����,�p�դJ�եX
        public int? FromFid { get; set; }
        [Column(TypeName = "text")]
        [Display(Name = "�Ƶ�")]
        public string? RecNote { get; set; } = "";

        [Display(Name = "���ݤ��q")] //�ާ@�H��
        public int? comFid { get; set; }
        [Display(Name = "�Ю֤H��")]
        public int? superFid { get; set; }
        [Display(Name = "�إߤH��")]        
        public int? addid { get; set; }
        [Display(Name = "�إ߮ɶ�")]
        public DateTime? adddate { get; set; }
        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [Display(Name = "��s�ɶ�")]
        public DateTime? moddate { get; set; }
        [Required]
        [Display(Name = "�ާ@����")]
        public bool JobDone { get; set; } = false;
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�t�έp�⪺�w�s�q")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal SysChkQty { get; set; } = 0;
        
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�L�I���w�s�q�Ĥ@��")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? UserChk1Qty { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "�L�I���w�s�q�ĤG��")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? UserChk2Qty { get; set; }
        [Display(Name = "�L�I����")]
        public string? TakeType { get; set; }

        //�H�U�D��Ʈw������� -----------------------------
        
        [NotMapped] //��J�ĳ椶����
        [Display(Name = "�Į�")]
        public int? DrawFid { get; set; }        
        [NotMapped] //��J�ĳ椶����
        [Display(Name = "�ī~")]
        public int? DrugFid { get; set; }
        [NotMapped] //��J�ĳ椶����
        [Display(Name = "�ī~�W��")]
        public string DrugName { get; set; } = "";
        [NotMapped] //�����դJ�եX��
        public bool rejectBill { get; set; } = false;


        //[NotMapped] //��J�ĳ椶����
        //[Display(Name = "���˦r��")]
        //public string Scantext { get; set; } = "";

        //[NotMapped] //��J�ĳ椶����
        //[StringLength(8, ErrorMessage = "{0} �r�ƪ��׻ݬ�8�Ӧr", MinimumLength = 8)]
        //[Display(Name = "�f�����X")]
        //public string? PatientNo { get; set; }
        //[NotMapped] //��J�ĳ椶����
        //public string? PatientSeq { get; set; }
        //[NotMapped] //��J�ĳ椶����
        //[StringLength(5, ErrorMessage = "{0} �r�ƪ��׻ݬ�5�Ӧr", MinimumLength = 5)]
        //[Display(Name = "���ĸ��X")]
        //public string? PrscptNo { get; set; }
        //[NotMapped] //��J�ĳ椶����
        //[Display(Name = "�ĳ��")] 
        //[Column(TypeName = "date")]
        //public DateTime? BillDate { get; set; }
        //[NotMapped] //��J�ĳ椶����(�h��)
        //[Display(Name = "�h�ĳ渹")]
        //[StringLength(13)]
        //public string? ReturnSheet { get; set; }
        //[NotMapped] //�s�Jmap
        //[Display(Name = "�ĳU")]
        //public List<int>? PrscptFid { get; set; } = null;

    }
}

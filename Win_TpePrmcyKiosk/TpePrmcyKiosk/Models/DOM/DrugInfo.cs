using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyKiosk.Models.DOM
{
    [Table("DrugInfo")]
    public partial class DrugInfo
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "藥品代碼")]
        public string DrugCode { get; set; } = string.Empty;

        [Display(Name = "藥品名稱")]
        public string? DrugName { get; set; }        
        [Display(Name = "是疫苗")]
        public bool? isVax { get; set; } = false;
        [Display(Name = "異動時須替代成的藥品")]
        public int? ReplaceTo { get; set; }
        [Display(Name = "重複取藥等級")]
        public int? ChkDrugTakedLv { get; set; } = 1;


        public string? Info_01 { get; set; } = "";
        public string? Info_02 { get; set; } = "";
        public string? Info_03 { get; set; } = "";
        public string? Info_04 { get; set; } = "";
        public string? Info_05 { get; set; } = "";
        public string? Info_06 { get; set; } = "";
        [Display(Name = "廠牌")]
        public string? Info_07 { get; set; } = "";
        public string? Info_08 { get; set; } = "";
        public string? Info_09 { get; set; } = "";
        public string? Info_10 { get; set; } = "";
        public string? Info_11 { get; set; } = "";
        public string? Info_12 { get; set; } = "";
        public string? Info_13 { get; set; } = "";
        public string? Info_14 { get; set; } = "";
        public string? Info_15 { get; set; } = "";
        public string? Info_16 { get; set; } = "";
        public string? Info_17 { get; set; } = "";
        public string? Info_18 { get; set; } = "";
        public string? Info_19 { get; set; } = "";
        public string? Info_20 { get; set; } = "";
        public string? Info_21 { get; set; } = "";
        public string? Info_22 { get; set; } = "";
        public string? Info_23 { get; set; } = "";
        public string? Info_24 { get; set; } = "";
        public string? Info_25 { get; set; } = "";
        public string? Info_26 { get; set; } = "";
        public string? Info_27 { get; set; } = "";
        public string? Info_28 { get; set; } = "";
        public string? Info_29 { get; set; } = "";
        public string? Info_30 { get; set; } = "";

        [NotMapped]
        [Display(Name = "異動時須替代成的藥品")]
        public string? ReplaceToDrugName { get; set; } = "";
    }
}

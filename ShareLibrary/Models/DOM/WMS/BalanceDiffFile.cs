using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("BalanceDiffFile")]
    public partial class BalanceDiffFile
    {
        [Key]
        public int FID { get; set; }
        public DateTime? CreateTime { get; set; }        
        public string? BranchCode { get; set; } = "";
        public string? FileName { get; set; } = "";
        public DateTime? FileTime { get; set; }
        public string? OPDER_SW { get; set; } = "";
        public string? PAT_NO { get; set; } = "";
        public string? ODR_CODE { get; set; } = "";
        public string? TOTAL_QTY { get; set; } = "";
        public string? PRICE_UNIT { get; set; } = "";
        public string? REG_DATE { get; set; } = "";
        public string? DELETE_DATE { get; set; } = "";
        public string? DRUG_NO { get; set; } = "";
        public string? ODR_SEQ { get; set; } = "";
        public string? D_NUMBER { get; set; } = "";
        public string? MED_TYPE { get; set; } = "";
        public string? HR_SEQ { get; set; } = "";
        public string? PAT_SEQ { get; set; } = "";
        
    }
}

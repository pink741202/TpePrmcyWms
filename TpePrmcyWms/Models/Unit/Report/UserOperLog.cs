using System.ComponentModel.DataAnnotations;

namespace TpePrmcyWms.Models.Unit.Report
{
    public class UserOperLog
    {
        [Key]
        public int stockBillFid { get; set; } = 0;
        [Display(Name = "時間")]
        public DateTime operTime { get; set; } //時間

        [Display(Name = "使用者")]
        public string empName { get; set; } = ""; //使用者

        [Display(Name = "使用者ID")]
        public string empNo { get; set; } = ""; //使用者ID

        [Display(Name = "操作代碼")]
        public string operCode { get; set; } = "";//操作代碼
        [Display(Name = "操作")]
        public string operName { get; set; } = "";//操作

        [Display(Name = "櫃子")]
        public int CbntFid { get; set; } //櫃子

        [Display(Name = "櫃子名稱")]
        public string CbntName { get; set; } = "";//櫃子名稱

        [Display(Name = "抽屜號碼")]
        public string DrawNo { get; set; } = "";//抽屜號碼
    }
}

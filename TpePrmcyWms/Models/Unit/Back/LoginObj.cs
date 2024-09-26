using System.ComponentModel.DataAnnotations;

namespace TpePrmcyWms.Models.Unit.Back
{
    public class LoginObj
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string UserAcc { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; } = "";
        public string CardNo { get; set; } = "";
    }
}

using Microsoft.CodeAnalysis.Operations;

namespace TpePrmcyWms.Models.Unit
{
    public class ValidateReturnMsg
    {
        public string Name { get; set; } = "";
        private string TranslatedMsg = "";
        public string ErrorMsg
        {
            get { return TranslatedMsg; }
            set
            {
                if (value.Contains("req")) //這裡可以編輯變更錯誤訊息
                {
                    TranslatedMsg = value;
                }
                else
                {
                    TranslatedMsg = value;
                }
            }
        }

        
    }
}

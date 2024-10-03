using ShareLibrary.Models.Unit;
using System;

namespace TpePrmcyWms.Models.Unit
{
    public class ResponObj<T> : ResponObject<T> 
    {
        public ResponObj() { }
        public ResponObj(string code, T data)
        {
            this.code = code;
            returnData = data;
            string tyle = data.GetType().ToString();
            if (data!=null && data.GetType().ToString() == "System.Collections.Generic.List`1[TpePrmcyWms.Models.Unit.ValidateReturnMsg]") { this.code = "invalid"; } //在js只認這關鍵字,才會顯示錯誤
            if(data!=null && data.GetType().ToString() == "System.String" && message == "") { this.message = data?.ToString()??""; } 
        
        }
        public ResponObj(string code, T data, string msg)
        {
            this.code = code;
            returnData = data;
            if (data != null && data.GetType().ToString() == "ValidateReturnMsg") { this.code = "invalid"; } //在js只認這關鍵字,才會顯示錯誤
            message = msg;
        }
    }
}

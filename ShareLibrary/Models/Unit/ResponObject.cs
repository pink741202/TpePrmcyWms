namespace ShareLibrary.Models.Unit
{
    public class ResponObject<T>
    {
        public string code { get; set; } = "";
        public string message { get; set; } = "";
        public T? returnData { get; set; }

        public ResponObject() { }
        public ResponObject(string code, T data, string msg)
        {
            this.code = code;
            this.returnData = data;
            this.message = msg;
        }
    }
}

using ShareLibrary.Models.Unit;
using TpePrmcyWms.Models.DOM;

namespace ShareLibrary.Models.HsptlApiUnit
{
    #region 出藥
    public class Qry_OutStorage
    {
        public string HospId { get; set; } //院區代碼
        public string PatNo { get; set; } //病歷號碼
        public string PatSeq { get; set; } //病人序號
        public string OdrSeq { get; set; } //醫令序號
        public string PhaDate { get; set; } //出藥日期(民國年7碼，YYYMMDD)
        public string PhaNum { get; set; } //領藥號
        public string OdrCode { get; set; } //藥物代碼
        public string TotalQty { get; set; } //總量
        public string Dose { get; set; } //劑量
        public string Freqcode { get; set; } //頻次
        public string Days { get; set; } //天數

        // http://hq-medwebsrv-vm:8086/PHA/GetOutStorage?HospId=G&PatNo=19999999&PatSeq=G00096&OdrSeq=1&PhaDate=1081216&PhaNum=A0001&OdrCode=OMORP3&TotalQty=56&Dose=1&Freqcode=HS&Days=28
    
        public Qry_OutStorage(string[] scantext)
        {
            HospId = scantext[0];
            PatNo = scantext[6];
            PatSeq = scantext[7];
            OdrSeq = scantext[8];
            PhaDate = scantext[2];
            PhaNum = scantext[1];
            OdrCode = scantext[3];
            TotalQty = scantext[10];
            Dose = scantext[14];
            Freqcode = scantext[15];
            Days = scantext[16];
        }
        public Qry_OutStorage(PrscptBill obj)
        {
            HospId = obj.Pharmarcy ?? "";
            PatNo = obj.PatientNo ?? "";
            PatSeq = obj.PatientSeq ?? "";
            OdrSeq = obj.OrderSeq.ToString() ?? "";
            PhaDate = qwFunc.DateToEraString(obj.PrscptDate ?? DateTime.Now);
            PhaNum = obj.PrscptNo ?? "";
            OdrCode = obj.DrugCode ?? "";
            TotalQty = obj.TtlQty.ToString() ?? "";
            Dose = obj.DrugDose ?? "";
            Freqcode = obj.DrugFrequency ?? "";
            Days = obj.DrugDays ?? "";
        }
    }

    public class Resp_OutStorage
    {
        public bool Succ { get; set; } //有沒有此紀錄
        public string Code { get; set; } //"0000"
        public string Message { get; set; } 
        public DateTime DateTime { get; set; }
        public bool Data { get; set; } //
    }
    #endregion

    #region 門急退藥(QR CODE)
    public class Qry_ReturnByQRCode
    {
        public string HospId { get; set; } //院區代碼
        public string PatNo { get; set; } //病歷號碼
        public string PatSeq { get; set; } //病人序號

        public Qry_ReturnByQRCode(string[] scantext)
        {
            HospId = scantext[0];
            PatNo = scantext[6];
            PatSeq = scantext[7];
        }
    }
    // http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByQRCode?HospId=G&PatNo=66666666&PatSeq=G01009

    public class Resp_ReturnByQRCode
    {
        public bool Succ { get; set; } //有沒有此紀錄
        public string Code { get; set; } //"0000"
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public List<Data_ReturnByQRCode> Data { get; set; } 
    }
    public class Data_ReturnByQRCode
    {
        public string PAT_NO { get; set; } 
        public string PAT_SEQ { get; set; } 
        public int ODR_SEQ { get; set; }
        public int ODR_VER { get; set; }
        public string ODR_CODE { get; set; } 
        public string ODR_NAME { get; set; } 
        public string ANESTHETIC_SW { get; set; } 
        public int TOTAL_QTY { get; set; } 
        public string PRICE_UNIT { get; set; } 
    }
    //因為有可能醫師沒有DC ORDER(被DC的資料其ODR_VER<>0)，但還是有藥物可以退，所以我不能只給(ODR_VER<>0)的資料，當PAT_NO+PAT_SEQ+ODR_SEQ有2筆以上，就是醫師有DC ORDER。
    #endregion

    #region 門急退藥(手動輸入)
    public class Qry_ReturnByHand
    {
        public string HospId { get; set; } //院區代碼
        public string PatNo { get; set; } //病歷號碼
        public string PhaDate { get; set; } //出藥日期(民國年7碼，YYYMMDD) 
        public string PhaNum { get; set; } //報告結束日期(民國年7碼，YYYMMDD)
    }
    // http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByHand?HospId=G&PatNo=66666666&PhaDate=1081212&PhaNum=E0001

    public class Resp_ReturnByHand
    {
        public bool Succ { get; set; } //有沒有此紀錄
        public string Code { get; set; } //"0000"
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public List<Data_ReturnByHand> Data { get; set; }
    }
    public class Data_ReturnByHand
    {
        public string PAT_NO { get; set; } //病歷號碼
        public string PAT_SEQ { get; set; } //病人序號
        public int ODR_SEQ { get; set; } //醫令序號
        public int ODR_VER { get; set; } //醫令版本(0生效中，<>0版本歷程，無效)
        public string ODR_CODE { get; set; } //醫令代碼
        public string ODR_NAME { get; set; } //醫令名稱
        public string ANESTHETIC_SW { get; set; } //管制藥分級
        public int TOTAL_QTY { get; set; }
        public string PRICE_UNIT { get; set; } //藥物總量單位
    }

    #endregion

    #region 取得住院病人序號
    public class Qry_IPDPatSeq
    {
        public string HospId { get; set; } //院區代碼
        public string PatNo { get; set; } //病歷號碼
    }
    // http://hq-medwebsrv-vm:8086/PHA/GetIPDPatSeq?HospId=G&PatNo=41239683

    public class Resp_IPDPatSeq
    {
        public bool Succ { get; set; } //有沒有此紀錄
        public string Code { get; set; } //"0000"
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public List<Data_IPDPatSeq> Data { get; set; }
    }
    public class Data_IPDPatSeq
    {
        public string PAT_NO { get; set; } //病歷號碼
        public string PAT_SEQ { get; set; } //病人住院序號
        public string BED_CODE { get; set; } //床號
        public string? REAL_OUT_DATE { get; set; } //實際出院日期(民國年7碼，YYYMMDD)
    }

    #endregion

    #region 住院退藥
    public class Qry_IPDReturn
    {
        public string HospId { get; set; } //院區代碼
        public string PatNo { get; set; } //病歷號碼
        public string PatSeq { get; set; } //病人序號
        public string ReturnSheet { get; set; } //退藥單號
    }
    // http://hq-medwebsrv-vm:8086/PHA/GetIPDReturnStorage?HospId=G&PatNo=41239683&PatSeq=G01033&ReturnSheet=1081212142605

    public class Resp_IPDReturn
    {
        public bool Succ { get; set; } //有沒有此紀錄
        public string Code { get; set; } //"0000"
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public List<Data_IPDReturn> Data { get; set; }
    }
    public class Data_IPDReturn
    {
        public string PAT_NO { get; set; } //病歷號碼
        public string PAT_SEQ { get; set; } //病人序號
        public int ODR_SEQ { get; set; } //醫令序號
        public string ODR_CODE { get; set; } //醫令代碼
        public string ODR_NAME { get; set; } //醫令名稱
        public string ANESTHETIC_SW { get; set; } //管制藥分級
        public string PRICE_UNIT { get; set; } //藥物總量單位
        public string DRUGBAG_SEQ { get; set; } //原領藥號
    }
    //同一PAT_NO+PAT_SEQ+ODR_SEQ的總退藥量(可多次退藥)不可大於出藥量
    #endregion
}

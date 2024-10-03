using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TpePrmcyWms.Models.DOM;
using System.Net;

namespace TpePrmcyWms.Models.Unit.Front
{
    //由StockBill紀錄 取得藥格資訊給View (每台電腦只控制該櫃的抽屜)
    public class QryDrawers
    {
        public QryDrawers() { }
        public QryDrawers(string drugcode)
        {
            stockBill = new StockBill_Prscpt();
            DrugCode = drugcode;
        }
        public bool isValid = true;
        public string InvalidMsg { get; set; } = string.Empty;
        public string DrugCode { get; set; } = string.Empty;
        public StockBill_Prscpt stockBill { get; set; }
        public PrscptBillInfo bill { get; set; }
        public List<DrugGridInfo>? drGridinfo { get; set; } = new List<DrugGridInfo>();
        public int PackageCnt { get; set; } = 0;
    }
    public class QryBatchDrawers
    {
        public QryBatchDrawers() { }
        public QryBatchDrawers(string drugcode)
        {
            DrugCode = drugcode;
        }
        public bool isValid = true;
        public string InvalidMsg { get; set; } = string.Empty;
        public string DrugCode { get; set; } = string.Empty;
        public StockBill_Prscpt stockBill { get; set; } = new StockBill_Prscpt();
        public List<PrscptBillInfo> bills { get; set; } = new List<PrscptBillInfo>();
        public List<DrugGridInfo>? drGridinfo { get; set; } = new List<DrugGridInfo>();        
        public int PackageCnt { get; set; } = 0;
    }
    public class QryDrawersForTrans
    {
        public bool isValid = true;
        public string DrugCode { get; set; } = string.Empty;
        public string sBillType { get; set; } = string.Empty;
        public string InvalidMsg { get; set; } = string.Empty;
        public StockBill? TransInBill { get; set; } 
        public StockBill? TransGoBill { get; set; } 
        public List<DrugGridInfo>? drGridinfo { get; set; } = new List<DrugGridInfo>();
        public int PackageCnt { get; set; } = 0;
    }
    //沖銷用
    public class QryOffsetDrawers
    {
        public QryOffsetDrawers() { }
        public QryOffsetDrawers(string offsetQryKey)
        {
            DrugCode = offsetQryKey.Split('+')[0];
            OffsetQryKey = offsetQryKey;
        }
        public bool isValid = true;
        public string InvalidMsg { get; set; } = string.Empty;
        public string DrugCode { get; set; } = string.Empty;
        public string OffsetQryKey { get; set; } = string.Empty;
        public decimal Qty_Prscpt { get; set; } = 0;
        public decimal Qty_Apply { get; set; } = 0;
        public decimal Qty_ReturnEmpty { get; set; } = 0;
        public decimal Qty_ReturnDrug { get; set; } = 0;
        public decimal? UnitConvert { get; set; }
        public List<StockBill> stockBills { get; set; } = new List<StockBill>();
        public List<PrscptBillInfo> bills { get; set; } = new List<PrscptBillInfo>();
        public List<DrugGridInfo>? drGridinfo { get; set; } = new List<DrugGridInfo>();
        public int PackageCnt { get; set; } = 0;
    }
    //藥單資訊,加msg(批次用)
    public class PrscptBillInfo : PrscptBill
    {
        public string msg { get; set; } = "";
        public string Scantext { get; set; } = "";
        public decimal? DailyTake { get; set; }
        public PrscptBillInfo() : base() { }
        public PrscptBillInfo(PrscptBill data)
        {
            this.FID = data.FID;
            this.Pharmarcy = data.Pharmarcy;
            this.PrscptNo = data.PrscptNo;
            this.PrscptDate = data.PrscptDate;
            this.DrugCode = data.DrugCode;
            this.DrugName = data.DrugName;
            this.CtrlDrugGrand = data.CtrlDrugGrand;
            this.PatientNo = data.PatientNo;
            this.PatientSeq = data.PatientSeq;
            this.OrderSeq = data.OrderSeq;
            this.PatientName = data.PatientName;
            this.TtlQty = data.TtlQty;
            this.PriceUnit = data.PriceUnit;
            this.DrName = data.DrName;
            this.BedCode = data.BedCode;
            this.DrugDose = data.DrugDose;
            this.DrugFrequency = data.DrugFrequency;
            this.DrugDays = data.DrugDays;
            this.ReturnSheet = data.ReturnSheet;
            this.ScanTime = data.ScanTime;
            this.DoneFill = data.DoneFill;
            this.comFid = data.comFid;
            this.dptFid = data.dptFid;
            this.HISchk = data.HISchk;
        }
    }
    //庫存異動單,加藥單資訊
    public class StockBill_Prscpt : StockBill
    {
        [Display(Name = "掃瞄字串")]
        public string Scantext { get; set; } = "";

        [StringLength(8, ErrorMessage = "{0} 字數長度需為8個字", MinimumLength = 8)]
        [Display(Name = "病歷號碼")]
        public string? PatientNo { get; set; }
        public string? PatientSeq { get; set; }
        [StringLength(5, ErrorMessage = "{0} 字數長度需為5個字", MinimumLength = 5)]
        [Display(Name = "領藥號碼")]
        public string? PrscptNo { get; set; }
        [Display(Name = "藥單日")]
        [Column(TypeName = "date")]
        public DateTime? BillDate { get; set; } = DateTime.Now;
        [Display(Name = "退藥單號")]
        [StringLength(13)]
        public string? ReturnSheet { get; set; }
        [Display(Name = "藥袋")]
        public List<int>? PrscptFid { get; set; } = null;
        

        public StockBill_Prscpt() { }
        public StockBill_Prscpt(StockBill obj)
        {
            this.FID = obj.FID;
            this.CbntFid = obj.CbntFid;
            this.DrugGridFid = obj.DrugGridFid;
            this.Qty = obj.Qty;
            this.TargetQty = obj.TargetQty;
            this.BillType = obj.BillType;
            this.TradeType = obj.TradeType;
            this.ToFloor = obj.ToFloor;
            this.BatchNo = obj.BatchNo;
            this.ExpireDate = obj.ExpireDate;
            this.FromFid = obj.FromFid;
            this.RecNote = obj.RecNote;
            this.comFid = obj.comFid;
            this.modid = obj.modid;
            this.moddate = obj.moddate;
            this.JobDone = obj.JobDone;
            this.SysChkQty = obj.SysChkQty;
            this.UserChk1Qty = obj.UserChk1Qty;
            this.UserChk2Qty = obj.UserChk2Qty;
            this.TakeType = obj.TakeType;
        }
    }

    //整合的藥格資訊
    public class DrugGridInfo
    {
        public int FID { get; set; }
        public int DrawFid { get; set; }
        public int DrugFid { get; set; }
        public decimal StockQty { get; set; }
        public decimal SafetyStock { get; set; }
        public string DrawerNo { get; set; }
        public string StockTakeType { get; set; }
        public string StoreNo { get; set; }
        public decimal? UnitConvert { get; set; }
        public int? OffsetCbntFid { get; set; }
        public int? OffsetDrawFid { get; set; }
        public bool? ReturnEmptyBottle { get; set; }
        public decimal? DailyMaxTake { get; set; }
        public bool? BatchActive { get; set; }
        public decimal? MaxLimitQty { get; set; }
        public List<DrugGridBatchNo> gridBatches { get; set; } = new List<DrugGridBatchNo>();
    }
    
    //控制sensor流程並存在session用
    public class SensorDeviceCtrl
    {
        public int FID { get; set; }
        public string TargetTable { get; set; }
        public int TargetObjFid { get; set; }
        public string SensorType { get; set; }
        public string SensorNo { get; set; }
        public string SerialPort { get; set; }
        public string Modbus_Addr { get; set; }
        public string Modbus_X16 { get; set; }
        public int? Modbus_Rgst { get; set; }
        public decimal? UnitWeight { get; set; } //單位重
        public decimal? UnitQty { get; set; } //單位數量
        public decimal? WeighWeight { get; set; } //磅秤結果,現重
        public decimal? WeighWeight0 { get; set; } //磅秤結果,初重
        public decimal? WeighQty { get; set; } //磅秤結果,數量
    }

    //整合美沙酮
    public class StockBill_MSD : StockBill
    {
        [Display(Name = "執行藥師")]
        public string AddEmpName { get; set; } = "";
        public int? superFid { get; set; }
        [Display(Name = "護理師")]
        public string SuperEmpName { get; set; } = "";
        [DataType(DataType.Date)]
        [Display(Name = "執行日期")]
        public DateTime RecordDate { get; set; } = DateTime.Now.Date;


        [Display(Name = "重量(g)")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? This_Weight { get; set; } = 0;
        [Display(Name = "體積(cc)")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? This_CC { get; set; } = 0;
        [Display(Name = "重量(g)")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? Last_Weight { get; set; } = 0;
        [Display(Name = "體積(cc)")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? Last_CC { get; set; } = 0;



        [Display(Name = "服用人數/日")]
        public int? UsedPatientCnt { get; set; }
        [Display(Name = "使用量(cc)/日")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? UsedCC { get; set; }
        [Display(Name = "盤盈虧(cc)")]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true)]
        public decimal? StockTakeBalance { get; set; }


        public List<PrscptBill> prscpts = new List<PrscptBill>();
        [Display(Name = "掃瞄字串")]
        public string Scantext { get; set; } = "";
        public decimal? BottleWegiht { get; set; } = 400; //秤重 優化按鈕用

    }

}

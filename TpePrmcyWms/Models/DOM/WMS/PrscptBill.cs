using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TpePrmcyWms.Models.Service;

namespace TpePrmcyWms.Models.DOM
{
    [Table("PrscptBill")]
    public partial class PrscptBill
    {
        [Key]
        public int FID { get; set; }

        [StringLength(1)]
        [Display(Name = "�ħ�²�X")]
        public string? Pharmarcy { get; set; }

        [StringLength(5)]
        [Display(Name = "���ĸ�")]
        public string? PrscptNo { get; set; }

        [Display(Name = "�X�Ĥ��")]
        public DateTime? PrscptDate { get; set; }

        [StringLength(8)]
        public string? DrugCode { get; set; }

        [StringLength(60)]
        public string? DrugName { get; set; }

        [StringLength(1)]
        [Display(Name = "�ި��ĵ���")]
        public string? CtrlDrugGrand { get; set; }

        [StringLength(8)]
        [Display(Name = "�f�����X")]
        public string? PatientNo { get; set; }

        [StringLength(6)]
        [Display(Name = "�f�H�Ǹ�")]
        public string? PatientSeq { get; set; }

        [Display(Name = "��O�Ǹ�")]
        public decimal? OrderSeq { get; set; }

        [StringLength(40)]
        [Display(Name = "�f�H�m�W")]
        public string? PatientName { get; set; }

        public decimal? TtlQty { get; set; }

        [StringLength(20)]
        public string? PriceUnit { get; set; }

        [StringLength(40)]
        [Display(Name = "��v�m�W")]
        public string? DrName { get; set; }

        [StringLength(6)]
        [Display(Name = "�ɸ�")]
        public string? BedCode { get; set; }

        [StringLength(4)]
        public string? DrugDose { get; set; }

        [StringLength(20)]
        public string? DrugFrequency { get; set; }

        [StringLength(4)]
        public string? DrugDays { get; set; }
        [StringLength(14)]
        public string? ReturnSheet { get; set; }
        public int ScanTime { get; set; }
        public bool DoneFill { get; set; }
        public int? comFid { get; set; }
        public int? dptFid { get; set; }
        public bool? HISchk { get; set; }

        public PrscptBill() { }
        public PrscptBill(string[] data) {
            Pharmarcy = data[0] == "" ? "" : data[0];
            PrscptNo = data[1];
            PrscptDate = data[2] != "" ? qwServ.EraStringToDate(data[2]) : null;
            DrugCode = data[3];
            DrugName = data[4];
            CtrlDrugGrand = data[5];
            PatientNo = data[6];
            PatientSeq = data[7];
            OrderSeq = data[8] != "" ? Convert.ToDecimal(data[8]) : 0;
            PatientName = data[9];
            TtlQty = data[10] != "" ? Convert.ToDecimal(data[10]) : null;
            PriceUnit = data[11];
            DrName = data[12];
            BedCode = data[13];
            DrugDose = data[14];
            DrugFrequency = data[15];
            DrugDays = data[16];
            DoneFill = false;
            ScanTime = 1;
            HISchk = null;
        }
    }
}
// TtlQty = DrugDose * DrugFrequency * DrugDays
// �`�q = ���q * �W���N�X(�n�ର�W��) * �Ѽ�
// ���q * �W�� <= �f�H�@��̤j�A�ζq(DailyMaxTake)


//strPhaSw CHAR(1)�X���ħ�
//strPhaNum CHAR(5)���ĸ�
//strPhaDate CHAR(7) �X�Ĥ��(����~YYYMMDD)
//strOdrCode VARCHAR(8)�Ī��N�X
//strOdrName VARCHAR(60)�Ī��ӫ~�W
//strAnesSw CHAR(1)�ި��ĵ���(1��2��3)
//strPatNo CHAR(8)�f�����X
//strPatSeq CHAR(6)�f�H�Ǹ�
//strOdrSeq NUMBER(5,0)��O�Ǹ�
//strPatName VARCHAR(40)�f�H�m�W
//strTotalQty NUMBER(5,0)�X���`�q
//strPriceUnit VARCHAR(20)�`�q���
//strDrName VARCHAR(40)��v�m�W
//strBedCode VARCHAR(6)�ɸ�(�u����|�f�H��)

namespace TpePrmcyWms.Models.Unit.Report
{
    public class ReportDownloadRequestModel
    {
        public string? qKeyString { get; set; }
        public DateTime? qDate1 { get; set; }
        public DateTime? qDate2 { get; set; }
        public int? qCbnt { get; set; }
        public string? qDrawFid { get; set; }
    }
}

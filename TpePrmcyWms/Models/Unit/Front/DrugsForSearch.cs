using System.ComponentModel.DataAnnotations;

namespace TpePrmcyWms.Models.Unit.Front
{
    public class DrugsForSearch
    {
        public int DrugFid { get; set; }
        public string DrugCode { get; set; } = "";
        public string DrugName { get; set; } = "";
        public string BarcodeNo { get; set; } = "";
    }
}

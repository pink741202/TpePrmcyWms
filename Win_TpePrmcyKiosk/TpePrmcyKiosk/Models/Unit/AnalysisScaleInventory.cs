using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TpePrmcyKiosk.Models.Unit
{
    public class AnalysisScaleInventory
    {
        public string ScaleNo { get; set; }
        public string DrugCode { get; set; } = string.Empty;
        public int ContenorCount { get; set; } = 0;
        public decimal? BeginInQty { get; set;} = 0;
        public decimal? SumOutQty { get; set;} = 0;
        public decimal? RemainingQty { get; set; } = 0;

        public AnalysisScaleInventory(string _ScaleNo, string _drugcode, int _cnt)
        {
            ScaleNo = _ScaleNo;
            DrugCode = _drugcode;
            ContenorCount = _cnt;
        }
    }
}

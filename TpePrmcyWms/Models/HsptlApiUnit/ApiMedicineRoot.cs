using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TpePrmcyWms.Models.HsptlApiUnit
{
    public class ESMCATC
    {
        public int sn_sort { get; set; }
        [JsonProperty("ATC_NAME")]
        public string ATC_NAME { get; set; }
        public string atc_code { get; set; }
        public string odr_code { get; set; }
    }

    public class ESMCImage
    {
        public int sn_sort { get; set; }
        public string odr_code { get; set; }
        public string image_original { get; set; }
        public int img_file_size { get; set; }
        public int img_size_width { get; set; }
        public int img_size_length { get; set; }
        public string file_name { get; set; }
        public string image_thumbnail { get; set; }
        public int img_key { get; set; }
    }

    public class ESMCODRSTATU
    {
        public string locate { get; set; }
        [JsonProperty("ODR_STATUS_SW")]
        public string ODR_STATUS_SW { get; set; }
        public DateTime sync_date { get; set; }
    }

    public class ApiMedicine
    {
        public string odr_code { get; set; }
        public string textinfo_01 { get; set; }
        public string textinfo_02 { get; set; }
        public string textinfo_03 { get; set; }
        public string textinfo_04 { get; set; }
        public string textinfo_05 { get; set; }
        public string textinfo_06 { get; set; }
        public string textinfo_07 { get; set; }
        public string textinfo_08 { get; set; }
        public string textinfo_09 { get; set; }
        public string textinfo_10 { get; set; }
        public string textinfo_11 { get; set; }
        public string textinfo_12 { get; set; }
        public string textinfo_13 { get; set; }
        public string textinfo_14 { get; set; }
        public string textinfo_15 { get; set; }
        public string textinfo_16 { get; set; }
        public string textinfo_17 { get; set; }
        public string textinfo_18 { get; set; }
        public string textinfo_19 { get; set; }
        public string textinfo_20 { get; set; }
        public string textinfo_21 { get; set; }
        public string textinfo_22 { get; set; }
        public string textinfo_23 { get; set; }
        public string textinfo_24 { get; set; }
        public string textinfo_25 { get; set; }
        public string textinfo_26 { get; set; }
        public string textinfo_27 { get; set; }
        public string textinfo_28 { get; set; }
        public string textinfo_29 { get; set; }
        public string textinfo_30 { get; set; }
        [JsonProperty("ESMC_ATC")]
        public List<ESMCATC> ESMC_ATC { get; set; }
        [JsonProperty("ESMC_Health_Education_Pamphlets")]
        public List<object> ESMC_Health_Education_Pamphlets { get; set; }
        [JsonProperty("ESMC_Image")]
        public List<ESMCImage> ESMC_Image { get; set; }
        [JsonProperty("ESMC_Move_Url")]
        public List<object> ESMC_Move_Url { get; set; }
        [JsonProperty("ESMC_ODR_STATUS")]
        public List<ESMCODRSTATU> ESMC_ODR_STATUS { get; set; }
    }
    public class ApiMedicineRoot
    {
        public string status { get; set; }
        public List<ApiMedicine> rows { get; set; }
        public string msg { get; set; }
    }
}

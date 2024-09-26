using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{   
    [Table("Company")]
    public partial class Company
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "統編")]
        public string comid { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "名稱")]
        public string comtitle { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "簡稱")]
        public string comsttitle { get; set; }

        [StringLength(100)]
        [Display(Name = "網站")]
        [Url]
        public string? comhttp { get; set; }

        [StringLength(20)]
        [Display(Name = "電話")]
        [Phone]
        public string? comtel { get; set; }

        [StringLength(20)]
        [Display(Name = "傳真")]
        [Phone]
        public string? comfax { get; set; }


        [Display(Name = "備註")]
        public string? cnote { get; set; } = "";

        [StringLength(50)]
        [Display(Name = "LOGO")]
        public string? logopic { get; set; } = "";
    }
}

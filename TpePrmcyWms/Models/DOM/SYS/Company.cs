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
        [Display(Name = "�νs")]
        public string comid { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "�W��")]
        public string comtitle { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "²��")]
        public string comsttitle { get; set; }

        [StringLength(100)]
        [Display(Name = "����")]
        [Url]
        public string? comhttp { get; set; }

        [StringLength(20)]
        [Display(Name = "�q��")]
        [Phone]
        public string? comtel { get; set; }

        [StringLength(20)]
        [Display(Name = "�ǯu")]
        [Phone]
        public string? comfax { get; set; }


        [Display(Name = "�Ƶ�")]
        public string? cnote { get; set; } = "";

        [StringLength(50)]
        [Display(Name = "LOGO")]
        public string? logopic { get; set; } = "";
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TpePrmcyWms.Models.DOM
{
    

    [Table("Department")]
    public partial class Department
    {
        [Key]
        public int FID { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "�����N�X")]
        public string dptid { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "�����W��")]
        public string dpttitle { get; set; }
        [Display(Name = "���ݤ��q")]
        public int comFid { get; set; }

        [Display(Name = "��s�H��")]
        public int? modid { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "��s���")]
        public DateTime? moddate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("MapMenuOnCbnt")]
    public partial class MapMenuOnCbnt
    {
        [Key]
        public Guid GID { get; set; }

        [Required]
        [Display(Name = "�d�l")]
        public int CbntFid { get; set; }
        [Required]
        [Display(Name = "�\��")]
        public int MnLFid { get; set; }
        [Required]
        [Display(Name = "�i�ϥ�")]
        public bool Able { get; set; } = false;
        
        [Display(Name = "���N��W��")]
        public string? MnLTitle { get; set; }

    }
}

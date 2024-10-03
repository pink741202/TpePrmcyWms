using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    [Table("MenuTop")]
    public partial class MenuTop
    {
        [Key]
        public int FID { get; set; }
        [StringLength(20)]
        public string MenuName { get; set; }
        [StringLength(10)]
        public string System { get; set; }
        [StringLength(50)]
        public string Link { get; set; }
        public int? Sorting { get; set; }
        public int? comFid { get; set; }
        public int? modid { get; set; }
        public DateTime? moddate { get; set; }
    }
}

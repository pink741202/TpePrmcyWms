using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("MenuLeft")]
    public partial class MenuLeft
    {
        [Key]
        public int FID { get; set; }        
        public int MnTFid { get; set; }        
        public int Layer { get; set; }        
        public int UpFid { get; set; }     

        [StringLength(20)]
        public string CatelogName { get; set; } = "";

        [StringLength(50)]
        public string Link { get; set; } = "";
        [StringLength(4)]
        public string? OperCode { get; set; } = "";
        public int Sorting { get; set; }
        public bool OnDisplay { get; set; } = true;
        public int? comFid { get; set; }
        public int? dptFid { get; set; }
        public int? modid { get; set; }
        public DateTime? moddate { get; set; }
    }
}

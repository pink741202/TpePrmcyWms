using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM
{
    

    [Table("DrugPickLog")]
    public partial class DrugPickLog
    {
        [Key]
        public int FID { get; set; }

        [Required]
        public int DrugFid { get; set; }

        public int PickTimes { get; set; }

        public int? comFid { get; set; }
        public int? dptFid { get; set; }
    }
}

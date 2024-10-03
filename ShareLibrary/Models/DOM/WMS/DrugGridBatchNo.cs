using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace TpePrmcyWms.Models.DOM
{
    [Table("DrugGridBatchNo")]
    public partial class DrugGridBatchNo
    {
        [Key]
        public Guid GID { get; set; }
        public int GridFid { get; set; }

        [StringLength(16)]
        [Display(Name = "�帹")]
        public string BatchNo { get; set; } = "";
        [DataType(DataType.Date)]
        [Display(Name = "�Ĵ�")]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "�ƶq")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Qty { get; set; }
    }
}

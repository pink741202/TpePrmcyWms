using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpePrmcyWms.Models.DOM.DBView
{
    [Table("ViewTable")]
    public partial class ViewTable //�Žd��
    {
        [Key]
        public int FID { get; set; }
        
    }
}

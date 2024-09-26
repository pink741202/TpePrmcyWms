using TpePrmcyWms.Models.DOM;

namespace TpePrmcyWms.Models.Unit.Back
{
    public class DrugLimitedToPostObj
    {
        public int fid { get; set; } = 0;
        public List<DrugLimitedTo> list { get; set; } = new List<DrugLimitedTo>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMParcel
    {
        public string description { get; set; }
        public string box_type { get; set; }
        public PMWeight weight { get; set; }
        public PMDimension dimension { get; set; }
        public List<PMItems> items { get; set; }

    }
}

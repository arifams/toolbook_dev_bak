using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMDimension
    {
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal depth { get; set; }
        public string unit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMDetailCharge
    {
        public string type { get; set; }
        public PMCharge charge { get; set; }
    }
}

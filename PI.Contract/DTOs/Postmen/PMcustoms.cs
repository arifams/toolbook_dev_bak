using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Postmen
{
    public class PMcustoms
    {
        public PMbilling billing { get; set; }
        public string terms_of_trade { get; set; }
        public string purpose { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Carrier : ShortIdBaseEntity
    {
        public string Name { get; set; }
        public string CarrierNameLong { get; set; }
    }
}

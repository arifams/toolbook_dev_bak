using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
     public class Currency: ShortIdBaseEntity
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
    }
}

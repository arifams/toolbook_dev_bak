using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
     public class Currency: ShortIdBaseEntity
    {
        [MaxLength(10)]
        public string CurrencyCode { get; set; }
        [MaxLength(10)]
        public string CurrencyName { get; set; }
    }
}

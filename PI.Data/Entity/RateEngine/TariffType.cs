using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class TariffType:LongIdBaseEntity
    {
        [MaxLength(50)]
        public string TarrifName { get; set; }
    }
}

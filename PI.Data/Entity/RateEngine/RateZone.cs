using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class RateZone : LongIdBaseEntity
    {
        public long RateId { get; set; }
        public virtual Rate Rate { get; set; }

        public long ZoneId { get; set; }
        public virtual Zone Zone { get; set; }

        public decimal Price { get; set; }
    }
}

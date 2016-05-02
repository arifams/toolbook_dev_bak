using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class TransitTimeProduct : LongIdBaseEntity
    {
        public long TransmitTimeId { get; set; }
        public TransmitTime TransmitTime { get; set; }

        public ProductType ProductType { get; set; }

        public short Days { get; set; }
    }
}

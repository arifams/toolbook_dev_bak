using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Carrier : LongIdBaseEntity
    {
        public string CarrierName { get; set; }

        public string CarrierNameLong { get; set; }

        public string CarrierType { get; set; }

        public string ServiceLevel { get; set; }

        public string CarrierCountryCode { get; set; }

        public string CarrierAccountNumber { get; set; }

    }
}

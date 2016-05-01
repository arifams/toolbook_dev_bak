using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class TransmitTime : LongIdBaseEntity
    {
        public string CarrierName { get; set; }

        public string ServiceLevel { get; set; }

        public string CountryFrom { get; set; }

        public string CountryTo { get; set; }

        public string ZoneName { get; set; }
      
    }
}

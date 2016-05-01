using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Zone : LongIdBaseEntity
    {
        public string CarrierName { get; set; }

        public string ServiceLevel { get; set; }

        public string CountryFrom { get; set; }

        public string CountryTo { get; set; }

        public string ZoneName { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }

        public string Inbound { get; set; }

        public string TariffType { get; set; }
    }
}

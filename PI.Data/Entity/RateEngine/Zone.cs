using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Zone : LongIdBaseEntity
    {
        public string CountryFrom { get; set; }

        public string CountryTo { get; set; }

        public string ZoneName { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }

        public bool IsInbound { get; set; }

        public long CarrierId { get; set; }
        public Carrier Carrier { get; set; }

        public long TariffTypeId { get; set; }
        public virtual TariffType TariffType { get; set; }
    }
}

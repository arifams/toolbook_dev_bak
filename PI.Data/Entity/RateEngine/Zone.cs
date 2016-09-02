using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Zone : LongIdBaseEntity
    {
        [MaxLength(2)]
        public string CountryFrom { get; set; }
        [MaxLength(2)]
        public string CountryTo { get; set; }
        [MaxLength(50)]
        public string ZoneName { get; set; }
        [MaxLength(20)]
        public string LocationFrom { get; set; }
        [MaxLength(20)]
        public string LocationTo { get; set; }

        public bool IsInbound { get; set; }

        public long CarrierId { get; set; }
        public CarrierService Carrier { get; set; }

        public long TariffTypeId { get; set; }

        public virtual TariffType TariffType { get; set; }

        public virtual IList<TransmitTime> TransmitTimeList { get; set; }

    }
}

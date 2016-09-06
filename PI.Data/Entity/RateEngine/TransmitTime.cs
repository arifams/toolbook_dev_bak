using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class TransmitTime : LongIdBaseEntity
    {
        public long CarrierId { get; set; }
        public CarrierService Carrier { get; set; }

        [MaxLength(2)]
        public string CountryFrom { get; set; }
        [MaxLength(2)]
        public string CountryTo { get; set; }
        
        public long ZoneId { get; set; }
        public virtual Zone Zone { get; set; }

        public virtual IList<TransitTimeProduct> TransitTimeProductList { get; set; }
    }
}

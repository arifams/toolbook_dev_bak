using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class CarrierService : LongIdBaseEntity
    {
        public CarrierType CarrierType { get; set; }
        [MaxLength(20)]
        public string ServiceLevel { get; set; }
        [MaxLength(2)]
        public string CarrierCountryCode { get; set; }
        [MaxLength(20)]
        public string CarrierAccountNumber { get; set; }
        public short CarrierId { get; set; }
        public Carrier Carrier { get; set; }
    }
}

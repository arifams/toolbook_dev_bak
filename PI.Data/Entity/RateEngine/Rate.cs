using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Rate : LongIdBaseEntity
    {
        public string CountryFrom { get; set; }

        public bool IsInbound { get; set; }

        public ProductType Service { get; set; }

        public decimal WeightMin { get; set; }

        public decimal WeightMax { get; set; }

        public CurrencyType Currency { get; set; }

        public RatesCalculationMethod CalculationMethod { get; set; }

        public int VolumeFactor { get; set; }

        public decimal MaxLength { get; set; }

        public decimal MaxWeightPerPiece { get; set; }

        public RatesSell SellOrBuy { get; set; }

        public decimal MaxDimension { get; set; }

        public long CarrierId { get; set; }
        public virtual Carrier Carrier { get; set; }

        public long TariffTypeId { get; set; }
        public virtual TariffType TariffType { get; set; }

        public IList<RateZone> RateZoneList { get; set; }
    }
}

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

        public double WeightMin { get; set; }

        public double WeightMax { get; set; }

        public CurrencyType Currency { get; set; }

        public RatesCalculationMethod CalculationMethod { get; set; }

        public int VolumeFactor { get; set; }

        public double MaxLength { get; set; }

        public double MaxWeightPerPiece { get; set; }

        public RatesSell SellOrBuy { get; set; }

        public double MaxDimension { get; set; }

        public long CarrierId { get; set; }
        public virtual Carrier Carrier { get; set; }

        public long TariffTypeId { get; set; }
        public virtual TariffType TariffType { get; set; }

        public IList<RateZone> RateZoneList { get; set; }
    }
}

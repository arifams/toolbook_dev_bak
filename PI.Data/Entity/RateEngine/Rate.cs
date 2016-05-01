using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.RateEngine
{
    public class Rate : LongIdBaseEntity
    {
        public string Carrier { get; set; }

        public string ServiceLevel { get; set; }

        public string ServiceType { get; set; }

        public string CountryFrom { get; set; }

        public string Inbound { get; set; }

        public double WeightMin { get; set; }

        public double WeightMax { get; set; }

        public string Currency { get; set; }

        public string CalculationMethod { get; set; }

        public int VolumeFactor { get; set; }

        public double MaxLength { get; set; }

        public double MaxWeightPerPiece { get; set; }

        public string SellOrBuy { get; set; }

        public string TariffType { get; set; }

        public double MaxDimension { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AccountSettings : LongIdBaseEntity
    {

        public long CustomerId { get; set; }
        public short DefaultLanguageId { get; set; }
        public short DefaultCurrencyId { get; set; }
        public short DefaultTimeZoneId { get; set; }
        public short WeightMetricId { get; set; }
        public short VolumeMetricId { get; set; }

        #region Navigation Property

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("DefaultLanguageId")]
        public virtual Language DefaultLanguage { get; set; }

        [ForeignKey("DefaultCurrencyId")]
        public virtual Currency DefaultCurrency { get; set; }

        [ForeignKey("DefaultTimeZoneId")]
        public virtual TimeZone DefaultTimeZone { get; set; }

        [ForeignKey("WeightMetricId")]
        public virtual WeightMetric DefaultWeightMetric { get; set; }

        [ForeignKey("VolumeMetricId")]
        public virtual VolumeMetric DefaultVolumeMetric { get; set; }


        #endregion
    }
}

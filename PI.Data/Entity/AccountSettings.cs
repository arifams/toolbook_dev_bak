using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AccountSettings: LongIdBaseEntity
    {
           
        public long CustomerId { get; set; }
        public short DefaultLanguageId { get; set; }
        public short DefaultCurrencyId { get; set; }
        public short DefaultTimeZoneId { get; set; }

        #region Navigation Property

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("DefaultLanguageId")]
        public Language DefaultLanguage { get; set; }

        [ForeignKey("DefaultCurrencyId")]
        public Currency DefaultCurrency { get; set; }

        [ForeignKey("DefaultTimeZoneId")]
        public TimeZone DefaultTimeZone { get; set; }

        #endregion
    }
}

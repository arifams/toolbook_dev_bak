using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AccountSettings:BaseEntity
    {
           
        public long CustomerId { get; set; }
        public long DefaultLanguageId { get; set; }
        public long DefaultCurrencyId { get; set; }
        public long DefaultTimeZoneId { get; set; }

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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Company : LongIdBaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public long TenantId { get; set; }
        [MaxLength(100)]
        public string COCNumber { get; set; }
        [MaxLength(100)]
        public string VATNumber { get; set; }
        [MaxLength(100)]
        public string CompanyCode { get; set; }
        public bool IsInvoiceEnabled { get; set; }
        public string LogoUrl { get; set; }

        #region Navigation Property

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

       // public Division Division { get; set; }

        //public CostCenter CostCenter { get; set; }

        #endregion
    }
}

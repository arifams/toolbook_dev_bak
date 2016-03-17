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
        public string Name { get; set; }

        public long TenantId { get; set; }
        public string COCNumber { get; set; }
        public string VATNumber { get; set; }
        public string CompanyCode { get; set; }

        #region Navigation Property

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        //public Division Division { get; set; }

        //public CostCenter CostCenter { get; set; }

        #endregion
    }
}

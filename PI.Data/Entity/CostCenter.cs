using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class CostCenter : LongIdBaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        public long BillingAddressId { get; set; }
        public int Status { get; set; }
        public long CompanyId { get; set; }
        [MaxLength(6)]
        public string Type { get; set; }

        #region Navigation Property

       // [ForeignKey("CompanyId")]
        //public Company Company { get; set; }

        [ForeignKey("BillingAddressId")]
        public virtual Address BillingAddress { get; set; }

        public virtual IList<DivisionCostCenter> DivisionCostCenters { get; set; }

        #endregion
    }
}

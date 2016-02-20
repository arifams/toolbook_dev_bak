using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class CostCenter : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public long BillingAddressId { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }

        public string Type { get; set; }

        //public List<Division> AssignedDivisions { get; set; }

        #region Navigation Property

       // [ForeignKey("CompanyId")]
        //public Company Company { get; set; }

        [ForeignKey("BillingAddressId")]
        public Address BillingAddress { get; set; }

        public IList<DivisionCostCenter> DivisionCostCenters { get; set; }

        #endregion
    }
}

using PI.Contract.DTOs.Address;
using PI.Contract.DTOs.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.CostCenter
{
    public class CostCenterDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public long BillingAddressId { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }

        public string Type { get; set; }

        public IList<DivisionDto> AllDivisions { get; set; }

        public IList<DivisionDto> AssignedDivisions { get; set; } 

        public AddressDto BillingAddress { get; set; }

        public String FullBillingAddress { get; set; }

        public IList<long> AssignedDivisionIdList { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }
    }

}

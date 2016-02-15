using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Company
{
   public class CompanyDto
    {
       public long Id { get; set; }
        public string Name { get; set; }

        public long TenantId { get; set; }
        public string COCNumber { get; set; }
        public string VATNumber { get; set; }

    }
}

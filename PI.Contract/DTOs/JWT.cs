using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs
{
    //jason web token
    public class JWT
    {       
        public string UserId { get; set; }
        public string role { get; set; }

        public string TenantId { get; set; }
        public string UserName { get; set; }
        public string CompanyId { get; set; }


        public string iss { get; set; }
        public string aud { get; set; }
        public string exp { get; set; }
        public string nbf { get; set; }

    }
}

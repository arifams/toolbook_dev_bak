using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Admin
{
    public class CustomerListDto
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CorporateName { get; set; }

        public string City { get; set; }

        public bool Status { get; set; }

        public bool IsInvoiceEnabled { get; set; }        

        public int AssignedUserCount { get; set; }

        public int ActiveShipments { get; set; }

        public string CreatedDate { get; set; }


    }
}

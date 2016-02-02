using PI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PI.Data.Entity
{
    public class Customer : BaseEntity
    {
        public Salutation Salutation { get; set; } // TODO: Convert to Enum.
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCorporateAccount { get; set; }
        public string CompanyName { get; set; }

        public Address CustomerAddress { get; set; }
    }
}

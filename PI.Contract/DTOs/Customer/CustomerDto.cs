using PI.Common;
using PI.Contract.DTOs.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Customer
{
    public class CustomerDto
    {
        public long Id { get; set; }
        public string Salutation { get; set; } /// TODO: Convert to Enum.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool IsCorporateAccount { get; set; }
        public AddressDto CustomerAddress { get; set; }
        public string CompanyName { get; set; }

        public string UserName { get; set; } // TODO: To be removed
        public string Password { get; set; } // TODO: To be removed

        public string UserId { get; set; }
        public string Code { get; set; }
        public bool IsConfirmEmail { get; set; }    // Explicitly set this, if user going to confirm his email address, rather than depend on the userid and code.

        public string TemplateLink { get; set; }

        public string CompanyCode { get; set; }
    }
}

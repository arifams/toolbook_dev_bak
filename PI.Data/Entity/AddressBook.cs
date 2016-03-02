using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AddressBook:BaseEntity
    {
        public string CompanyName { get; set; }
        public string UserId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }        
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }


        //Address Status      
        public int? Status { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Number { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }
}

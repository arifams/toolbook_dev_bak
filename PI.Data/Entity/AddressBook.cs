using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class AddressBook: LongIdBaseEntity
    {
        [MaxLength(100)]
        public string CompanyName { get; set; }
        [MaxLength(50)]
        public string UserId { get; set; }
        [MaxLength(5)]
        public string Salutation { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(256)]
        public string EmailAddress { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string AccountNumber { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(10)]
        public string Number { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(50)]
        public string State { get; set; }
    }
}

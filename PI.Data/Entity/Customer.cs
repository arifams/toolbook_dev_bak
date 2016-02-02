using PI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Customer : BaseEntity
    {
        public Salutation Salutation { get; set; } // TODO: Convert to Enum.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        // Navigation property.
        public long AddressId { get; set; }
        [ForeignKey("AddressId")]

        public Address CustomerAddress { get; set; }
    }
}

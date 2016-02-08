﻿using PI.Common;
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
        public string Salutation { get; set; } // TODO: Convert to Enum.
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }

        public long AddressId { get; set; }

        public string UserName { get; set; } // TODO: To be removed
        public string Password { get; set; } // TODO: To be removed

        #region Navigation property

        [ForeignKey("AddressId")]
        public Address CustomerAddress { get; set; }

        #endregion
    }
}

using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PI.Data.Entity
{
    public class Customer : LongIdBaseEntity
    {
        [MaxLength(5)]
        public string Salutation { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string MiddleName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
        [MaxLength(256)]
        public string SecondaryEmail { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [MaxLength(20)]
        public string MobileNumber { get; set; }
        public bool IsCorpAddressUseAsBusinessAddress { get; set; }
        [MaxLength(100)]
        public string JobCapacity { get; set; }
        public long AddressId { get; set; }
        [MaxLength(50)]
        public string UserId { get; set; }

        public string UserName { get; set; } // TODO: To be removed
        public string Password { get; set; } // TODO: To be removed
        [MaxLength(50)]
        public string SelectedColour { get; set; }

        #region Navigation property

        [ForeignKey("AddressId")]
        public virtual Address CustomerAddress { get; set; }

        //[ForeignKey("CompanyId")]
        //public Company CustomerCompany { get; set; }

        [ForeignKey("UserId")]
        //[Required]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}

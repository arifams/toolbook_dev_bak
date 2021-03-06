﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public long TenantId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Salutation { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }


        [MaxLength(100)]
        public string MiddleName { get; set; }


        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public byte Level { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string MobileVerificationCode { get; set; }

        public System.DateTime? MobileVerificationExpiry { get; set; }

        #region Navigation property

        //[Required]
        [ForeignKey("TenantId")]
        public  Tenant Tenant { get; set; }

        //public virtual Customer Customer { get; set; }

        public virtual IList<UserInDivision> UserInDivisions { get; set; }

        #endregion

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here

            return userIdentity;
        }

    }
}

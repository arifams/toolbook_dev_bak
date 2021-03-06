﻿using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Tenant : LongIdBaseEntity
    {
        [MaxLength(100)]
        public string TenancyName { get; set; }
        public bool IsCorporateAccount { get; set; }

        #region Navigation Property

        //public Company Company { get; set; }

        //public ApplicationUser User { get; set; } 

        #endregion
    }
}

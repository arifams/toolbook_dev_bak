﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }

        public long TenantId { get; set; }

        #region Navigation Property

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; } 

        #endregion
    }
}

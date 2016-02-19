﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class DivisionCostCenter : BaseEntity
    {
        public long CostCenterId { get; set; }
        public long DivisionId { get; set; }
        public bool IsAssigned { get; set; }

        [ForeignKey("DivisionId")]
        public Division Divisions { get; set; }
        [ForeignKey("CostCenterId")]
        public CostCenter CostCenters { get; set; }
    }
}
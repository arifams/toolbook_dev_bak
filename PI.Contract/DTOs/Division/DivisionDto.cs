﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Division
{
    public class DivisionDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int DefaultCostCenterId { get; set; }

        public int Status { get; set; }

        public long CompanyId { get; set; }
    }
}
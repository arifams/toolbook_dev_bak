﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class ShipmentTerm : BaseEntity
    {
        public new short Id { get; set; }
        public string Name { get; set; }
    }
}

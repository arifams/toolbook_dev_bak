﻿using PI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Payment : LongIdBaseEntity
    {
        public long ReferenceId { get; set; }

        public PI.Contract.Enums.PaymentType PaymentType { get; set; }

        public string PaymentId { get; set; }
        
        public Status Status { get; set; }

        public string StatusCode { get; set; }

        public decimal Amount { get; set; }

        public CurrencyType CurrencyType { get; set; }
    }
}
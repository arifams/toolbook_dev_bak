﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Invoice
{
    public class InvoiceDto
    {
        public long Id { get; set; }

        public string InvoiceNumber { get; set; }

        public long? ShipmentId { get; set; }

        public string InvoiceValue { get; set; }

        public string InvoiceStatus { get; set; }

        public string CreatedBy { get; set; }

        public string URL { get; set; }
    }
}

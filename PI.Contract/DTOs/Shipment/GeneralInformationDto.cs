﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Shipment
{
    public class GeneralInformationDto
    {
        public string ShipmentId { get; set; }

        public string ShipmentCode { get; set; }

        public string ShipmentName { get; set; }

        public long DivisionId { get; set; }

        public long CostCenterId { get; set; }        

        public string ShipmentMode { get; set; }

        public string ShipmentTypeCode { get; set; }

        public string ShipmentTermCode { get; set; }

        public string shipmentModeName { get; set; }

        public string shipmentTypeName { get; set; }

        public string ShipmentServices { get; set; }

        public string TrackingNumber { get; set; }

        public string Status { get; set; }

        public long CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public short ShipmentPaymentTypeId { get; set; }

        public string ParentShipmentId { get; set; }
    }
}

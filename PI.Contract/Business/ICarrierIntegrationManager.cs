﻿using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface ICarrierIntegrationManager
    {
       ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters);

        string SubmitShipment(ShipmentDto addShipment);

        void DeleteShipment(string shipmentCode);       

        string GetShipmentStatus(string URL);

        string TrackAndTraceShipment(string URL);

    }
}

﻿using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace PI.Contract.Business
{
    public interface ICarrierIntegrationManager
    {
       ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters);

        AddShipmentResponse SendShipmentDetails(ShipmentDto addShipment);        

        void DeleteShipment(string shipmentCode);       

        string GetShipmentStatus(string URL, string shipmentCode);

        string TrackAndTraceShipment(string URL);

        string GetLabel(string shipmentCode);

        StatusHistoryResponce GetUpdatedShipmentStatusehistory(string carrier, string trackingNumber, string codeShipment, string environment);

       
    }
}

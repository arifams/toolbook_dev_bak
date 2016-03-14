using PI.Contract.Business;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class ShipmentsManagement
    {
        public ShipmentcostList GetRateSheet(ShipmentDto currentShipment)
        {
            SISIntegrationManager sisManager = new SISIntegrationManager();
            RateSheetParametersDto currentRateSheetDetails = new RateSheetParametersDto();

           





            return sisManager.GetRateSheetForShipment(currentRateSheetDetails);

        }

        public string SubmitShipment(ShipmentDto addShipment)
        {
            ICarrierIntegrationManager sisManager = new SISIntegrationManager();
           
            return sisManager.SubmitShipment(addShipment);
        }

        
    }
}

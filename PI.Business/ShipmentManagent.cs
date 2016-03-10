using PI.Contract.DTOs.RateSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class ShipmentManagent
    {
        public ShipmentcostList GetRateSheet(RateSheetParametersDto rateSheet)
        {
            SISIntegrationManager sisManager = new SISIntegrationManager();
            return sisManager.GetRateSheetForShipment(rateSheet);

        }
    }
}

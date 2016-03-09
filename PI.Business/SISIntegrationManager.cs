using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class SISIntegrationManager : ICarrierIntegrationManager
    {

        string ICarrierIntegrationManager.GetRateSheetForShipment(string URL)
        {
            throw new NotImplementedException();
        }

        string ICarrierIntegrationManager.SubmitShipment(string xmlDetail)
        {
            throw new NotImplementedException();
        }

        string ICarrierIntegrationManager.DeleteShipment(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        string ICarrierIntegrationManager.GetShipmentStatus(string URL)
        {
            throw new NotImplementedException();
        }

        string ICarrierIntegrationManager.TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }
}

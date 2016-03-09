using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface ICarrierIntegrationManager
    {
        string GetRateSheetForShipment(string URL);

        string SubmitShipment(string xmlDetail);

        string DeleteShipment(string shipmentCode);       

        string GetShipmentStatus(string URL);

        string TrackAndTraceShipment(string URL);

    }
}

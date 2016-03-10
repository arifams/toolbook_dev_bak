using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public class SISIntegrationManager : ICarrierIntegrationManager
    {

        public string GetRateSheetForShipment(string URL)
        {
            throw new NotImplementedException();
        }

        public string SubmitShipment(string xmlDetail)
        {
            throw new NotImplementedException();
        }

        public string DeleteShipment(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public string GetShipmentStatus(string URL)
        {
            throw new NotImplementedException();
        }

        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }
}

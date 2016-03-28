using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.Business
{
    public interface IShipmentManagement
    {
        ShipmentcostList GetRateSheet(ShipmentDto currentShipment);
        string GetInboundoutBoundStatus(string userId, string fromCode, string toCode);
        long SaveShipment(ShipmentDto addShipment);
        PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto);
        ShipmentOperationResult SendShipmentDetails(long shipmentId);
    }
}

using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PI.Business;

namespace PI.Service.Controllers
{
    [RoutePrefix("api/shipments")]
    public class ShipmentsController : BaseApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetRatesforShipment")]
        public ShipmentcostList GetRatesforShipment([FromBody]ShipmentDto currentShipment)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetRateSheet(currentShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("SubmitShipment")]
        public string SubmitShipment([FromBody]ShipmentDto addShipment)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.SubmitShipment(addShipment);
        }
    }
}

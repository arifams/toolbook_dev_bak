﻿
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
using PI.Contract.DTOs.AccountSettings;
using PI.Contract.DTOs.Common;

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
        public ShipmentOperationResult SubmitShipment([FromBody]ShipmentDto addShipment)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.SubmitShipment(addShipment);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAllCurrencies")]
        public IQueryable<CurrencyDto> GetAllCurrencies()
        {
            ProfileManagement userprofile = new ProfileManagement();
            IQueryable<CurrencyDto> currencies = userprofile.GetAllCurrencies();
            return currencies;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("GetHashForPayLane")]
        public PayLaneDto GetHashForPayLane(PayLaneDto payLaneDto)
        {
            ShipmentsManagement shipment = new ShipmentsManagement();
            return shipment.GetHashForPayLane(payLaneDto);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Authorize]
        [HttpGet]
        [Route("GetAllShipments")]
        public PagedList GetAllShipments(string status = null, string userId = null, DateTime? startDate = null, DateTime? endDate = null,
                                         string number = null, string source = null, string destination = null)
        {
            ShipmentsManagement shipmentManagement = new ShipmentsManagement();
            var pagedRecord = new PagedList();
            return pagedRecord = shipmentManagement.GetAllShipmentsbyUser(status, userId, startDate, endDate, number, source, destination);

        }
    }
}

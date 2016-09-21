using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PI.Contract.DTOs.Shipment;
using PI.Data.Entity;

namespace PI.Common
{
    public class ConfigAutoMapper
    {
        public ConfigAutoMapper()
        {
            //Mapper.Initialize(cfg => cfg.CreateMap<Shipment, ShipmentDto>());
        }
    }
}

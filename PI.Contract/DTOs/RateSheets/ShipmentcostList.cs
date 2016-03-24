using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PI.Contract.DTOs.RateSheets
{
    [XmlRoot("ShipmentcostList")]
    public class ShipmentcostList
    {
        public ShipmentcostList() { Items = new List<Shipmentcost>(); }
        [XmlElement("Shipmentcost")]
        public List<Shipmentcost> Items { get; set; }

        public string RateCalculateURL { get; set; }
    }
    public class Shipmentcost
    {
        [XmlElement("Transport_mode")]
        public string Transport_mode { get; set; }

        [XmlElement("Network_info")]
        public Network_info Network_info { get; set; }

        [XmlElement("Price")]
        public string Price { get; set; }

        [XmlElement("Price_excl_uplift")]
        public string Price_excl_uplift { get; set; }

        [XmlElement("Currency")]
        public string Currency { get; set; }

        [XmlElement("Rate")]
        public string Rate { get; set; }

        [XmlElement("Weight")]
        public string Weight { get; set; }

        [XmlElement("Service_level")]
        public string Service_level { get; set; }

        [XmlElement("Tariff_text")]
        public string Tariff_text { get; set; }

        [XmlElement("Tariff_type")]
        public string Tariff_type { get; set; }

        [XmlElement("Carrier_name")]
        public string Carrier_name { get; set; }

        [XmlElement("Transit_time")]
        public string Transit_time { get; set; }

        [XmlElement("Transit_hours")]
        public string Transit_hours { get; set; }

        [XmlElement("Pickup_date")]
        public string Pickup_date { get; set; }

        [XmlElement("Pickup_time")]
        public string Pickup_time { get; set; }

        [XmlElement("Delivery_date")]
        public string Delivery_date { get; set; }

        [XmlElement("Delivery_time")]
        public string Delivery_time { get; set; }

        [XmlElement("Pickup_date_tariff")]
        public string Pickup_date_tariff { get; set; }

        [XmlElement("Date_exchange_price")]
        public string Date_exchange_price { get; set; }

        [XmlElement("Date_exchange_price_supplier")]
        public string Date_exchange_price_supplier { get; set; }

        [XmlElement("Emission_costs")]
        public string Emission_costs { get; set; }

        [XmlElement("Emission_text")]
        public string Emission_text { get; set; }

        [XmlElement("Currency_tariff")]
        public string Currency_tariff { get; set; }

        [XmlElement("Rate_USD")]
        public string Rate_USD { get; set; }

        [XmlElement("Price_detail")]
        public Price_detail Price_detail { get; set; }

        [XmlElement("Price_base4discount")]
        public string Price_base4discount { get; set; }

        [XmlElement("Perc_fuel4discount")]
        public string Perc_fuel4discount { get; set; }

        [XmlElement("Fuel_price")]
        public string Fuel_price { get; set; }

        [XmlElement("C41_priority")]
        public string C41_priority { get; set; }

        [XmlElement("Ind_hide_transittime")]
        public string Ind_hide_transittime { get; set; }

        [XmlElement("Time_delivery_plan")]
        public string Time_delivery_plan { get; set; }

    }

    public class Network_info
    {
        [XmlElement("Network_info")]
        public string Network_inf { get; set; }
        [XmlElement("PRICE")]
        public PRICE price { get; set; }
    }

    public class PRICE
    {
        [XmlElement("PL")]
        public PL pl { get; set; }
    }

    public class PL
    {
        [XmlElement("CODE")]
        public string CODE { get; set; }
        [XmlElement("AMT")]
        public string AMT { get; set; }
        [XmlElement("TYPE")]
        public string TYPE { get; set; }
    }

    public class Price_detail
    {
        [XmlElement("Description")]
        public string Description { get; set; }
    }
}

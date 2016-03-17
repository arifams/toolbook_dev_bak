using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PI.Contract.DTOs.Shipment
{
    [XmlRoot("response")]
    public class AddShipmentResponse
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("code_shipment")]
        public string CodeShipment { get; set; }

        [XmlElement("reference")]
        public string Reference { get; set; }

        [XmlElement("awb")]
        public string Awb { get; set; }

        [XmlElement("carrier")]
        public string Carrier { get; set; }

        [XmlElement("service_level")]
        public string ServiceLevel { get; set; }

        [XmlElement("pieces")]
        public string Pieces { get; set; }

        [XmlElement("weight")]
        public string Weight { get; set; }

        [XmlElement("weight_uom")]
        public string WeightUom { get; set; }

        [XmlElement("volume_weight")]
        public string VolumeWeight { get; set; }

        [XmlElement("delivery_condition")]
        public string DeliveryCondition { get; set; }

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlElement("code_currency")]
        public string CodeCurrency { get; set; }

        [XmlElement("date_delivery")]
        public string DateDelivery { get; set; }

        [XmlElement("time_delivery")]
        public string TimeDelivery { get; set; }

        [XmlElement("accepted_by")]
        public string AcceptedBy { get; set; }

        [XmlElement("date_pickup")]
        public string DatePickup { get; set; }

        [XmlElement("status_shipment")]
        public string StatusShipment { get; set; }

        [XmlElement("tracking_url")]
        public TrackingUrl TrackingUrl { get; set; }

        [XmlElement("PDF")]
        public string PDF { get; set; }
    }

    public class TrackingUrl
    {
        [XmlElement("input")]
        public string Input { get; set; }
    }
}

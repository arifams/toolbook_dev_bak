using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PI.Contract.DTOs.Shipment
{
    [XmlRoot("root")]
    public class StatusHistoryResponce
    {
        [XmlElement("info")]
        public info info { get; set; }

        [XmlElement("history")]
        public history history { get; set; }
    }
       
    public class info
    {
        [XmlElement("carrier")]
        public string carrier { get; set; }

        [XmlElement("awb")]
        public string awb { get; set; }

        [XmlElement("link")]
        public string link { get; set; }

        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("system")]
        public system system { get; set; }
    }

    public class system
    {
        [XmlElement("codeshipment")]
        public string codeshipment { get; set; }

        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("reference")]
        public string reference { get; set; }

        [XmlElement("expected_arrival")]
        public string expected_arrival { get; set; }

        [XmlElement("consignee")]
        public consignee consignee { get; set; }

        [XmlElement("consignor")]
        public consignor consignor { get; set; }
    }

    public class consignor
    {
        [XmlElement("geo")]
        public geo geo { get; set; }

        [XmlElement("company")]
        public  string company { get; set; }

        [XmlElement("address")]
        public string address { get; set; }

        [XmlElement("address_nr")]
        public string address_nr { get; set; }

        [XmlElement("address_extra")]
        public string address_extra { get; set; }

        [XmlElement("zipcode")]
        public string zipcode { get; set; }

        [XmlElement("city")]
        public string city { get; set; }

        [XmlElement("country")]
        public string country { get; set; }

        [XmlElement("state")]
        public string state { get; set; }

    }


    public class consignee
    {
        [XmlElement("geo")]
        public geo geo { get; set; }

        [XmlElement("company")]
        public string company { get; set; }

        [XmlElement("address")]
        public string address { get; set; }

        [XmlElement("address_nr")]
        public string address_nr { get; set; }

        [XmlElement("address_extra")]
        public string address_extra { get; set; }

        [XmlElement("zipcode")]
        public string zipcode { get; set; }

        [XmlElement("city")]
        public string city { get; set; }

        [XmlElement("country")]
        public string country { get; set; }

        [XmlElement("state")]
        public string state { get; set; }
    }

    public class geo
    {
        [XmlElement("lat")]
        public string lat { get; set; }

        [XmlElement("lng")]
        public string lng { get; set; }
    }

    public class history
    {
        public history() { Items = new List<items>(); }

        [XmlElement("item")]
        public List<items> Items { get; set; }
    }

    public class items
    {
        [XmlElement("location")]
        public location location { get; set; }

    }

    public class location
    {
        [XmlElement("country")]
        public string country { get; set; }

        [XmlElement("city")]
        public string city { get; set; }

        [XmlElement("geo")]
        public geo geo { get; set; }
    }

    public class activity
    {
        public activity() { Items = new List<item>(); }

        [XmlElement("item")]
        public List<item> Items { get; set; }
    }

    public class item
    {
        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("timestamp")]
        public timestamp timestamp { get; set; }
    }

    public class timestamp
    {
        [XmlElement("date")]
        public string date { get; set; }

        [XmlElement("time")]
        public string time { get; set; }
    }
}

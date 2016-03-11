using PI.Contract.Business;
using PI.Contract.DTOs.RateSheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PI.Business
{
    public class SISIntegrationManager : ICarrierIntegrationManager
    {

        public ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters)
        {
            var requestURL = GetRateRequestURL(rateParameters);

            XmlDocument doc1 = new XmlDocument();
           // doc1.Load("http://www2.shipitsmarter.com/taleus/ec_shipmentcost_v2.asp?userid=User@Mitrai.com&password=Mitrai462&output=XML&type_xml=LIST&vat=NO&default_off=YES&type=selectkmnetworkroad&fieldname4=shipment_price&fieldname1=price&sell_buy=&courier=UPSDHLFEDTNT&courier_km=&courier_air=EME&courier_road=&courier_tariff_base=&courier_sea=&courier_date_pickup_transition=&language=EN&print_button=&country_distance=&courier_tariff_type=NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE&country_to=AS&code_country_to=AS&weight=0.45&code_currency=USD&pieces=1&length=25&width=38.1&height=2.54&volume=2458.0596&max_dimension=106.67999999999999&max_length=38.1&max_weight=0.45&surface=967.74&ind_pallet=&max_actual_length=25.4&max_width=38.1&max_height=2.54&max_volume=2458.0596&package=DOCUMENT&address1=Tale%20United%20States&address2=Mariners%20Island%20Boulevard&street_number=901&address3=Suite%20595&address4=San%20Mateo&postcode=94404&country_from=US&code_country_from=US&address11=fgdfg&address12=dfgdfg&address13=dfgd&address14=dfgd&street_number_delivery=dfgdg&postcode_delivery=dfgd&date_pickup=09-Mar-2016%2000:00&time_pickup=09:30&date_delivery_request=24-Mar-2016%2000:00&delivery_condition=DD-DDU-PP&value=2&weight_unit=kg&insurance_instruction=N&sort=PRICE&volume_unit=cm&inbound=N&dg=NO&dg_type=&account=&code_customer=&ind_delivery_inside=&url=www2.shipitsmarter.com/taleus/");
            doc1.Load(requestURL);

            ShipmentcostList myObject = null;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer mySerializer =
            new XmlSerializer(typeof(ShipmentcostList));          

            // Call the Deserialize method and cast to the object type.       

           return   myObject = (ShipmentcostList)mySerializer.Deserialize(new StringReader(doc1.OuterXml.ToString()));

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

        public string GetRateRequestURL(RateSheetParametersDto rateParameters)
        {            
            string baseSISUrl = "http://www2.shipitsmarter.com/taleus/ec_shipmentcost_v2.asp?";
            if (rateParameters==null)
            {
                return string.Empty;
            }
            StringBuilder rateRequestUrl = new StringBuilder(baseSISUrl);
            rateRequestUrl.Append("userid=" + rateParameters.userid);
            rateRequestUrl.Append("&password=" + rateParameters.password);
            rateRequestUrl.Append("&output=" + rateParameters.output);
            rateRequestUrl.Append("&type_xml=" + rateParameters.type_xml);
            rateRequestUrl.Append("&vat=" + rateParameters.vat);
            rateRequestUrl.Append("&default_off=" + rateParameters.default_off);
            rateRequestUrl.Append("&fieldname4=" + rateParameters.fieldname4);
            rateRequestUrl.Append("&fieldname1=" + rateParameters.fieldname1);
            rateRequestUrl.Append("&sell_buy=" + rateParameters.sell_buy);
            rateRequestUrl.Append("&courier=" + rateParameters.courier);
            rateRequestUrl.Append("&courier_km=" + rateParameters.courier_km);
            rateRequestUrl.Append("&courier_air=" + rateParameters.courier_air);
            rateRequestUrl.Append("&courier_road=" + rateParameters.courier_road);
            rateRequestUrl.Append("&courier_tariff_base=" + rateParameters.courier_tariff_base);
            rateRequestUrl.Append("&courier_sea=" + rateParameters.courier_sea);
            rateRequestUrl.Append("&courier_date_pickup_transition=" + rateParameters.courier_date_pickup_transition);
            rateRequestUrl.Append("&language=" + rateParameters.language);
            rateRequestUrl.Append("&print_button=" + rateParameters.print_button);
            rateRequestUrl.Append("&country_distance=" + rateParameters.country_distance);
            rateRequestUrl.Append("&courier_tariff_type=" + rateParameters.courier_tariff_type);
            rateRequestUrl.Append("&country_to=" + rateParameters.country_to);
            rateRequestUrl.Append("&code_country_to=" + rateParameters.code_country_to);
            rateRequestUrl.Append("&weight=" + rateParameters.weight);
            rateRequestUrl.Append("&code_currency=" + rateParameters.code_currency);
            rateRequestUrl.Append("&pieces=" + rateParameters.pieces);
            rateRequestUrl.Append("&length=" + rateParameters.length);
            rateRequestUrl.Append("&width=" + rateParameters.width);
            rateRequestUrl.Append("&height=" + rateParameters.height);
            rateRequestUrl.Append("&volume=" + rateParameters.volume);
            rateRequestUrl.Append("&max_dimension=" + rateParameters.max_dimension);
            rateRequestUrl.Append("&max_length=" + rateParameters.max_length);
            rateRequestUrl.Append("&max_weight=" + rateParameters.max_weight);
            rateRequestUrl.Append("&surface=" + rateParameters.surface);
            rateRequestUrl.Append("&ind_pallet=" + rateParameters.ind_pallet);
            rateRequestUrl.Append("&max_actual_length=" + rateParameters.max_actual_length);
            rateRequestUrl.Append("&max_width=" + rateParameters.max_width);
            rateRequestUrl.Append("&max_height=" + rateParameters.max_height);
            rateRequestUrl.Append("&max_volume=" + rateParameters.max_volume);
            rateRequestUrl.Append("&package=" + rateParameters.package);
            rateRequestUrl.Append("&address1=" + rateParameters.address1);
            rateRequestUrl.Append("&address2=" + rateParameters.address2);
            rateRequestUrl.Append("&address3=" + rateParameters.address3);
            rateRequestUrl.Append("&address4=" + rateParameters.address4);
            rateRequestUrl.Append("&street_number=" + rateParameters.street_number);           
            rateRequestUrl.Append("&postcode=" + rateParameters.postcode);
            rateRequestUrl.Append("&country_from=" + rateParameters.country_from);
            rateRequestUrl.Append("&code_country_from=" + rateParameters.code_country_from);
            rateRequestUrl.Append("&address11=" + rateParameters.address11);
            rateRequestUrl.Append("&address12=" + rateParameters.address12);
            rateRequestUrl.Append("&address13=" + rateParameters.address13);
            rateRequestUrl.Append("&address13=" + rateParameters.address14);
            rateRequestUrl.Append("&street_number_delivery=" + rateParameters.street_number_delivery);
            rateRequestUrl.Append("&postcode_delivery=" + rateParameters.postcode_delivery);
            rateRequestUrl.Append("&date_pickup=" + rateParameters.date_pickup);
            rateRequestUrl.Append("&time_pickup=" + rateParameters.time_pickup);
            rateRequestUrl.Append("&date_delivery_request=" + rateParameters.date_delivery_request);
            rateRequestUrl.Append("&delivery_condition=" + rateParameters.delivery_condition);
            rateRequestUrl.Append("&weight_unit=" + rateParameters.weight_unit);
            rateRequestUrl.Append("&insurance_instruction" + rateParameters.insurance_instruction);
            rateRequestUrl.Append("&sort=" + rateParameters.sort);
            rateRequestUrl.Append("&volume_unit=" + rateParameters.volume_unit);
            rateRequestUrl.Append("&inbound=" + rateParameters.inbound);
            rateRequestUrl.Append("&dg=" + rateParameters.dg);
            rateRequestUrl.Append("&dg_type=" + rateParameters.dg_type);
            rateRequestUrl.Append("&account=" + rateParameters.account);
            rateRequestUrl.Append("&max_actual_length=" + rateParameters.max_actual_length);
         

            rateRequestUrl.Append("&code_customer=" + rateParameters.code_customer);
            rateRequestUrl.Append("&ind_delivery_inside=" + rateParameters.ind_delivery_inside);
            rateRequestUrl.Append("&url=" + rateParameters.url);

          
            return rateRequestUrl.ToString();
        }
    }
}

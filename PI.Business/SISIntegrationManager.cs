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

        public ShipmentcostList ICarrierIntegrationManager.GetRateSheetForShipment(RateSheetParametersDto rateParameters)
        {
            XmlDocument doc1 = new XmlDocument();
            doc1.Load("http://www2.shipitsmarter.com/taleus/ec_shipmentcost_v2.asp?userid=User@Mitrai.com&password=Mitrai462&output=XML&type_xml=LIST&vat=NO&default_off=YES&type=selectkmnetworkroad&fieldname4=shipment_price&fieldname1=price&sell_buy=&courier=UPSDHLFEDTNT&courier_km=&courier_air=EME&courier_road=&courier_tariff_base=&courier_sea=&courier_date_pickup_transition=&language=EN&print_button=&country_distance=&courier_tariff_type=NLPARUPS:NLPARFED:USPARDHL2:USPARTNT:USPARUPS:USPARFED2:USUPSTNT:USPAREME:USPARPAE&country_to=AS&code_country_to=AS&weight=0.45&code_currency=USD&pieces=1&length=25&width=38.1&height=2.54&volume=2458.0596&max_dimension=106.67999999999999&max_length=38.1&max_weight=0.45&surface=967.74&ind_pallet=&max_actual_length=25.4&max_width=38.1&max_height=2.54&max_volume=2458.0596&package=DOCUMENT&address1=Tale%20United%20States&address2=Mariners%20Island%20Boulevard&street_number=901&address3=Suite%20595&address4=San%20Mateo&postcode=94404&country_from=US&code_country_from=US&address11=fgdfg&address12=dfgdfg&address13=dfgd&address14=dfgd&street_number_delivery=dfgdg&postcode_delivery=dfgd&date_pickup=09-Mar-2016%2000:00&time_pickup=09:30&date_delivery_request=24-Mar-2016%2000:00&delivery_condition=DD-DDU-PP&value=2&weight_unit=kg&insurance_instruction=N&sort=PRICE&volume_unit=cm&inbound=N&dg=NO&dg_type=&account=&code_customer=&ind_delivery_inside=&url=www2.shipitsmarter.com/taleus/");
           
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
    }
}

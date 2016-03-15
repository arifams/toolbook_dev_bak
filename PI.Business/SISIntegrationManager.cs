using PI.Contract.Business;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
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
      private string sisUserId = null;
      private string password = null;

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

        public string SubmitShipment(ShipmentDto addShipment)
        {
            string xmlData = string.Format("{0}{1}", "http://www2.shipitsmarter.com/taleus/insert_shipment.asp?data_xml=", BuildAddShipmentXMLString(addShipment));

            //xmlData = "http://www2.shipitsmarter.com/taleus/insert_shipment.asp?data_xml=<insert_shipment password='mitrai462' userid='User@mitrai.com' code_company='122' version='1.0'><output_type>XML</output_type><action>STORE_AWB</action><reference>refh1012011cv300</reference><account>000001</account><carrier_name>UPS</carrier_name><address11>Comp1</address11><address12>dfdf</address12><address14>Beverly hills</address14><postcode_delivery>90210</postcode_delivery><code_state_to>CA</code_state_to><code_country_to>US</code_country_to><weight>1</weight><shipment_line id='1'><package>BOX</package><description>1</description><weight>1</weight><quantity>1</quantity><width>1</width><length>1</length><height>1</height></shipment_line><commercial_invoice_line id='1'><content>Electronics</content><quantity>2</quantity><value>150.50</value><quantity>2</quantity><country_of_origin>CN</country_of_origin></commercial_invoice_line></insert_shipment>";

            WebRequest webRequest = WebRequest.Create(xmlData);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            WebResponse webResp = webRequest.GetResponse();

            return null;
        }

        public void DeleteShipment(string shipmentCode)
        {
            shipmentCode = "37733403";

            string deleteURL = @"http://book.parcelinternational.nl/taleus/admin-shipment.asp?userid=user@mitrai.com&password=mitrai462&action=delete&code_shipment="+ shipmentCode;


            WebRequest webRequest = WebRequest.Create(deleteURL);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            WebResponse webResp = webRequest.GetResponse();          
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

        private string BuildAddShipmentXMLString(ShipmentDto addShipment)
        {
            // Retrieve username and password from service web.config file.
            string sisUserName = "user@mitrai.com", sisPassword = "mitrai462", sisCompanyCode = "122";

            // TODO : Get this from db.
            string referenceNo = "testhp123hp123";
            
            StringBuilder shipmentStr = new StringBuilder();

            shipmentStr.AppendFormat("<insert_shipment password='{0}' userid='{1}' code_company='{2}' version='1.0'>",sisPassword,sisUserName,sisCompanyCode);
            shipmentStr.AppendFormat("<output_type>XML</output_type>");
            shipmentStr.AppendFormat("<action>STORE_AWB</action>");
            shipmentStr.AppendFormat("<reference>{0}</reference>", referenceNo);
            shipmentStr.AppendFormat("<account>{0}</account>", 000001);  // Should be cost center - But for now send this value-: 000001
            shipmentStr.AppendFormat("<carrier_name>{0}</carrier_name>",addShipment.CarrierInformation.CarrierName);
            shipmentStr.AppendFormat("<service_level>{0}</service_level>", addShipment.CarrierInformation.serviceLevel);
            shipmentStr.AppendFormat("<ind_dangerous>{0}</ind_dangerous>", "N");   // TODO: sprint 3 doesn't support for dangerous goods. So for this sprint this should be No
            shipmentStr.AppendFormat("<ind_insurance>{0}</ind_insurance>", addShipment.PackageDetails.IsInsuared == "true" ? "Y" : "N");
            shipmentStr.AppendFormat("<code_currency>{0}</code_currency>",addShipment.PackageDetails.ValueCurrency);
            shipmentStr.AppendFormat("<tariff_type>{0}</tariff_type>", addShipment.CarrierInformation.tarriffType);
            shipmentStr.AppendFormat("<tariff_text>{0}</tariff_text>", addShipment.CarrierInformation.tariffText);
            //shipmentStr.AppendFormat("<price>{0}</price>",);    // TODO: Get price from summary total
            //shipmentStr.AppendFormat("<price_insurance>{0}</price_insurance>", ); // TODO: Comment this for now. - Will get clarification later.

            // Get values for following fields from Dilshan code. --------------------------------------------------------------------------
            //shipmentStr.AppendFormat("<weight>{0}</weight>",addShipment.PackageDetails.TotalWeight);
            //shipmentStr.AppendFormat("<weight_unit>{0}</weight_unit>",);    // TODO: Get Unit
            //shipmentStr.AppendFormat("<length>{0}</length>",); // TODO: Total length.
            //shipmentStr.AppendFormat("<height>0</height>"); // TODO: Total height.
            //shipmentStr.AppendFormat("<width>0</width>");   // TODO: Total width
            //shipmentStr.AppendFormat("<volume_unit>{0}</volume_unit>",); // TODO:Volume unit

            //shipmentStr.AppendFormat("<package>{0}</package>",);    // TODO: This represent summary of package. So has different package types, use DIVERSE

            // Consignor details.
            shipmentStr.AppendFormat("<code_country_from>{0}</code_country_from>", addShipment.AddressInformation.Consigner.Country);
            shipmentStr.AppendFormat("<address1>{0}</address1>", addShipment.AddressInformation.Consigner.Name);
            shipmentStr.AppendFormat("<address2>{0}</address2>", addShipment.AddressInformation.Consigner.Address1);
            shipmentStr.AppendFormat("<address3>{0}</address3>", addShipment.AddressInformation.Consigner.Address2);
            shipmentStr.AppendFormat("<address4>{0}</address4>", addShipment.AddressInformation.Consigner.City);
            shipmentStr.AppendFormat("<postcode>{0}</postcode>", addShipment.AddressInformation.Consigner.Postalcode);
            shipmentStr.AppendFormat("<street_number>{0}</street_number>", addShipment.AddressInformation.Consigner.Number);
            shipmentStr.AppendFormat("<code_state_from>{0}</code_state_from>", addShipment.AddressInformation.Consigner.State);

            // Consignee details.
            shipmentStr.AppendFormat("<code_country_to>{0}</code_country_to>", addShipment.AddressInformation.Consignee.Country);
            shipmentStr.AppendFormat("<address11>{0}</address11>", addShipment.AddressInformation.Consignee.Name);
            shipmentStr.AppendFormat("<address12>{0}</address12>", addShipment.AddressInformation.Consignee.Address1);
            shipmentStr.AppendFormat("<address13>{0}</address13>", addShipment.AddressInformation.Consignee.Address2);
            shipmentStr.AppendFormat("<address14>{0}</address14>", addShipment.AddressInformation.Consignee.City);
            shipmentStr.AppendFormat("<postcode_delivery>{0}</postcode_delivery>", addShipment.AddressInformation.Consignee.Postalcode);
            shipmentStr.AppendFormat("<street_number_delivery>{0}</street_number_delivery>", addShipment.AddressInformation.Consignee.Number);
            shipmentStr.AppendFormat("<code_state_to>{0}</code_state_to>", addShipment.AddressInformation.Consignee.State);

            // Summary of packages.
            shipmentStr.AppendFormat("<description>{0}</description>", addShipment.PackageDetails.ShipmentDescription);
            //shipmentStr.AppendFormat("<delivery_condition>{0}</delivery_condition>", "");
            shipmentStr.AppendFormat("<value>{0}</value>", addShipment.PackageDetails.DeclaredValue);
            shipmentStr.AppendFormat("<code_currency_value>{0}</code_currency_value>", addShipment.PackageDetails.ValueCurrency);

            double totalCountOfPackages = 0;
            addShipment.PackageDetails.ProductIngredients.Select(e => totalCountOfPackages += e.Quantity);

            shipmentStr.AppendFormat("<pieces>{0}</pieces>", totalCountOfPackages);
            shipmentStr.AppendFormat("<shipment_lines>{0}</shipment_lines>", addShipment.PackageDetails.ProductIngredients.Count);

            short lineCount = 1;
            foreach (var lineItem in addShipment.PackageDetails.ProductIngredients)
            {
                shipmentStr.AppendFormat("shipment_line id='" + lineCount + "'");
                //shipmentStr.AppendFormat("<package>{0}</package>", addShipment.PackageDetails.ProductIngredients[0].);
                shipmentStr.AppendFormat("<weight>{0}</weight>", lineItem.Weight);
                shipmentStr.AppendFormat("<quantity>{0}</quantity>", lineItem.Quantity);
                shipmentStr.AppendFormat("<width>{0}</width>", lineItem.Width);
                shipmentStr.AppendFormat("<length>{0}</length>", lineItem.Length);
                shipmentStr.AppendFormat("<height>{0}</height>", lineItem.Height);
                shipmentStr.AppendFormat("</shipment_line>");
                lineCount++;
            }
            shipmentStr.AppendFormat("</insert_shipment>");

            return null;
        }
    }
}

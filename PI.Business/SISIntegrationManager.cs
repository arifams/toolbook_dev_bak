using PI.Contract;
using PI.Contract.Business;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using PI.Contract.Enums;
using PI.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.ServiceBus.Messaging;
using PI.Common;

namespace PI.Business
{
    public class SISIntegrationManager : ICarrierIntegrationManager
    {

        private PIContext context;
        private ILogger logger;
        public SISIntegrationManager(ILogger logger, PIContext _context = null)
        {
            context = _context ?? PIContext.Get();
            this.logger = logger;
        }

      

        public string ServiceBusConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceBusConnectionString"].ToString();
            }
        }


        public string AddShipmentQueueName
        {
            get
            {
                return ConfigurationManager.AppSettings["AddShipmentQueueName"].ToString();
            }
        }


        public string SISWebURLUS
        {
            get
            {
                return ConfigurationManager.AppSettings["SISWebURLUS"].ToString();
            }
        }

        public string SISWebURLNL
        {
            get
            {
                return ConfigurationManager.AppSettings["SISWebURLNL"].ToString();
            }
        }

        public string SISCostCenterUS
        {
            get
            {
                return ConfigurationManager.AppSettings["SISCostCenterUS"].ToString();
            }
        }

        public string SISCostCenterNL
        {
            get
            {
                return ConfigurationManager.AppSettings["SISCostCenterNL"].ToString();
            }
        }

        public string SISUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["SISUserName"].ToString();
            }
        }

        public string SISPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SISPassword"].ToString();
            }
        }

        public string SISCompanyCodeUS
        {
            get
            {
                return ConfigurationManager.AppSettings["SISCompanyCodeUS"].ToString();
            }
        }

        public string SISCompanyCodeNL
        {
            get
            {
                return ConfigurationManager.AppSettings["SISCompanyCodeNL"].ToString();
            }
        }

        public string IsSendShipmentDebugData
        {
            get
            {
                return ConfigurationManager.AppSettings["IsSendShipmentDebugData"].ToString();
            }
        }





        public ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters)
        {
            // Set SIS credentials.
            rateParameters.userid = SISUserName;
            rateParameters.password = SISPassword;

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
            myObject = (ShipmentcostList)mySerializer.Deserialize(new StringReader(doc1.OuterXml.ToString()));

            using (PIContext context = new PIContext())
            {
                var weight = decimal.Parse(rateParameters.weight);
                var maxWeight = decimal.Parse(rateParameters.max_weight);
                var productType = (ProductType)Enum.Parse(typeof(ProductType), rateParameters.package == "DIVERSE" ? "Box" : rateParameters.package, true);
                var currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), rateParameters.code_currency.ToUpper(), true);
                var maxLength = decimal.Parse(rateParameters.max_length);
                var sellOrBuy = RatesSell.Sell;
                var max_dimension = decimal.Parse(rateParameters.max_dimension);
               // var volume = int.Parse(rateParameters.volume);
              //  var volumeFactor = (rateParameters.volume_unit == "cm" ? volume / 5000 : (volume * 2.54) / 5000);

                var result = context.Rate.Where(x => x.CountryFrom == rateParameters.country_from
                                                    && x.IsInbound == (rateParameters.inbound == "N" ? false : true)
                                                    && x.Service == productType
                                                    && (x.WeightMin <= weight && x.WeightMax > weight)
                                                    && x.Currency == currencyType



                    // && x.VolumeFactor == volumeFactor
                                                    && x.MaxLength >= maxLength
                    ////x.MaxWeightPerPiece > rateParameters.we
                                                    && x.SellOrBuy >= sellOrBuy
                                                    && x.MaxDimension >= max_dimension
                                                    ).Distinct().ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    var rateZoneResult = (result[i].RateZoneList == null) ? null : result[i].RateZoneList.Where(z => z.Zone.CountryFrom == rateParameters.country_from &&
                            z.Zone.CountryTo == rateParameters.country_to &&
                          int.Parse(z.Zone.LocationFrom.Split(new char[] { '-' })[0]) <= int.Parse(rateParameters.postcode) &&
                          int.Parse(rateParameters.postcode) <= int.Parse(z.Zone.LocationFrom.Split(new char[] { '-' })[1]) &&
                          int.Parse(z.Zone.LocationTo.Split(new char[] { '-' })[0]) <= int.Parse(rateParameters.postcode_delivery) &&
                          int.Parse(rateParameters.postcode_delivery) <= int.Parse(z.Zone.LocationTo.Split(new char[] { '-' })[1])).FirstOrDefault();

                    var transitTime = rateZoneResult.Zone.TransmitTimeList.Where(t => t.CarrierId == result[i].CarrierId)
                                      .Select(x => x.TransitTimeProductList.Where(p => p.ProductType == productType).SingleOrDefault()).SingleOrDefault();

                  myObject.Items.Add(new Shipmentcost
                  {
                      Carrier_name = result[i].Carrier.Carrier.Name,
                      Transport_mode = result[i].Carrier.CarrierType.ToString(),
                      Service_level = result[i].Carrier.ServiceLevel,
                      // Tariff_text = rate.TariffType.ToString(),
                      // Tariff_type = "?",                      
                      Currency = result[i].Currency.ToString(),

                      Price = (result[i].RateZoneList.Count == 0 || rateZoneResult == null) ? null : rateZoneResult.Price.ToString(),
                      Delivery_date = (DateTime.Parse(rateParameters.date_pickup)
                                      .AddDays((transitTime != null) ? transitTime.Days : 0)).ToString("dd-MMM-yyyy"),
                      Pickup_date = rateParameters.date_pickup,
                      Price_detail = new Price_detail { Description = result[i].Carrier.Carrier.Name + ", " + result[i].Carrier.ServiceLevel },
                      Transit_time = (transitTime != null) ? transitTime.Days.ToString() +" days" : null
                  });
                }

                // Add rates from XRates for USPS
                //result.ForEach(rate => myObject.Items.Add(new Shipmentcost
                //  {
                //      Carrier_name = rate.Carrier.CarrierName,
                //      Transport_mode = rate.Carrier.CarrierType.ToString(),
                //      Service_level = rate.Carrier.ServiceLevel,
                //      // Tariff_text = rate.TariffType.ToString(),
                //      // Tariff_type = "?",
                //      // Pickup_date = "",
                //      Currency = rate.Currency.ToString(),

                //      Price = (rate.RateZoneList.Count == 0) ? null : rate.RateZoneList.Where(z => z.Zone.CountryFrom == rateParameters.country_from &&
                //             z.Zone.CountryTo == rateParameters.country_to &&
                //             int.Parse(z.Zone.LocationFrom.Split(new char[] { '-' })[0]) <= int.Parse(rateParameters.postcode) &&
                //             int.Parse(rateParameters.postcode) <= int.Parse(z.Zone.LocationFrom.Split(new char[] { '-' })[1]) &&
                //             int.Parse(z.Zone.LocationTo.Split(new char[] { '-' })[0]) <= int.Parse(rateParameters.postcode_delivery) &&
                //             int.Parse(rateParameters.postcode_delivery) <= int.Parse(z.Zone.LocationTo.Split(new char[] { '-' })[1])).FirstOrDefault().Price.ToString(),
                //      Delivery_date = "",
                //      Price_detail = new Price_detail { Description = rate.Carrier.CarrierName + ", " + rate.Carrier.ServiceLevel },
                //      // Transit_time = //rate.Zon
                //  }));

            }

            // Set rate calculate url.
            if (IsSendShipmentDebugData == "true")
                myObject.RateCalculateURL = requestURL;

            return myObject;
        }

        public AddShipmentResponse SendShipmentDetails(ShipmentDto addShipment)
        {
            // Working sample xml data
            // "<insert_shipment password='mitrai462' userid='User@mitrai.com' code_company='122' version='1.0'><output_type>XML</output_type><action>STORE_AWB</action><reference>jhftuh11</reference><account>000001</account><carrier_name>UPS</carrier_name><address11>Comp1</address11><address12>dfdf</address12><address14>Beverly hills</address14><postcode_delivery>90210</postcode_delivery><code_state_to>CA</code_state_to><code_country_to>US</code_country_to><weight>1</weight><shipment_line id='1'><package>BOX</package><description>1</description><weight>1</weight><quantity>1</quantity><width>1</width><length>1</length><height>1</height></shipment_line><commercial_invoice_line id='1'><content>Electronics</content><quantity>2</quantity><value>150.50</value><quantity>2</quantity><country_of_origin>CN</country_of_origin></commercial_invoice_line></insert_shipment>"
            string sisUrl = string.Empty;
            using (PIContext context = new PIContext())
            {
                var tarrifTextCode = context.TarrifTextCodes.Where(t => t.TarrifText == addShipment.CarrierInformation.tariffText && t.IsActive && !t.IsDelete).FirstOrDefault();

                if (tarrifTextCode != null && tarrifTextCode.CountryCode == "NL")
                { 
                    sisUrl = SISWebURLNL;
                    addShipment.SISCompanyCode = SISCompanyCodeNL;
                }
                else
                { 
                    sisUrl = SISWebURLUS;
                    addShipment.SISCompanyCode = SISCompanyCodeUS;
                }
            }

            string addShipmentXML = string.Format("{0}", BuildAddShipmentXMLString(addShipment));
            AddShipmentResponse addShipmentResponse = null;

            QueueMessageSender messageSender = new QueueMessageSender();

            try
            {

               messageSender.SendQueueMessage<string>(addShipmentXML,AddShipmentQueueName, "addShipmentXML", addShipment.GeneralInformation.ShipmentId.ToString());
               
            }
            catch (Exception e)
            {

                throw;
            }


            //using (var wb = new WebClient())
            //{
            //    var data = new NameValueCollection();
            //    data["data_xml"] = addShipmentXML;

            //    var response = wb.UploadValues(sisUrl + "insert_shipment.asp", "POST", data);
            //    var responseString = Encoding.Default.GetString(response);

            //    XDocument doc = XDocument.Parse(responseString);

            //    XmlSerializer mySerializer = new XmlSerializer(typeof(AddShipmentResponse));
            //    addShipmentResponse = (AddShipmentResponse)mySerializer.Deserialize(new StringReader(responseString));
            //}

            //return myObject != null ? myObject.StatusShipment : "Error";

            if (IsSendShipmentDebugData == "true")
                addShipmentResponse.AddShipmentXML = addShipmentXML;

            return addShipmentResponse;
        }

        

        public void DeleteShipment(string shipmentCode)
        {
            // Sample url with data send format
            //@"http://book.parcelinternational.nl/taleus/admin-shipment.asp?userid=user@mitrai.com&password=mitrai462&action=delete&code_shipment=" + shipmentCode;

            string sisUrl = string.Empty;
            using (PIContext context = new PIContext())
            {
                var shipmentTarrifText = context.Shipments.Where(s => s.ShipmentCode == shipmentCode).Select(s => s.TariffText).First();
                var tarrifTextCode = context.TarrifTextCodes.Where(t => t.TarrifText == shipmentTarrifText && t.IsActive && !t.IsDelete).FirstOrDefault();

                if (tarrifTextCode != null && tarrifTextCode.CountryCode == "NL")
                    sisUrl = SISWebURLNL;
                else
                    sisUrl = SISWebURLUS;
            }

            string deleteURL = string.Format("{0}/admin-shipment.asp?userid={1}&password={2}&action=delete&code_shipment={3}", sisUrl, SISUserName, SISPassword, shipmentCode);

            WebRequest webRequest = WebRequest.Create(deleteURL);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            WebResponse webResp = webRequest.GetResponse();
        }

        public string GetShipmentStatus(string URL, string shipmentCode)
        {
            // Reference Link - Change following method in most suitable way described in following article
            // http://stackoverflow.com/questions/4015324/http-request-with-post

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["userid"] = SISUserName;
                values["password"] = SISPassword;
                values["codeshipment"] = shipmentCode;

                //  var response = client.UploadValues("http://parcelinternational.pro/status/UPS/1Z049A0X6797782690", values);
                var response = client.UploadValues(URL, values);

                var responseString = Encoding.Default.GetString(response);
            }

            return "";
        }

        //get the update 
        public StatusHistoryResponce GetUpdatedShipmentStatusehistory(string carrier, string trackingNumber, string codeShipment, string environment)
        {
            //TODO change the web url to production 
            string userID = SISUserName;
            string password = SISPassword;
            StatusHistoryResponce statusHistoryResponce = null;
            // string URL = "http://parcelinternational.pro/status/DHL/9167479650";
            string URL = "http://parcelinternational.pro/status/" + carrier + "/" + trackingNumber;
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                if (carrier!="USP")
                {
                    data["codeshipment"] = codeShipment;
                    data["userid"] = userID;
                    data["password"] = password;
                    data["environment"] = environment;
                }
                else
                {

                    Data.Entity.Shipment currentShipment = context.Shipments.Where(s=>s.TrackingNumber== trackingNumber).SingleOrDefault();
                    if (currentShipment==null)
                    {
                        return null;
                    }

                    data["country_from"] = currentShipment.ConsignorAddress.Country;
                    data["city_from"] = currentShipment.ConsignorAddress.City;
                    data["country_to"] = currentShipment.ConsigneeAddress.Country;
                    data["city_to"] = currentShipment.ConsigneeAddress.City;
                    data["userid"] = userID;
                    data["password"] = password;
                    data["environment"] = environment;
                }
               

                var response = wb.UploadValues(URL, "POST", data);
                var responseString = Encoding.Default.GetString(response);
                if (string.IsNullOrWhiteSpace(responseString))
                    statusHistoryResponce = null;
                else
                {
                    XDocument doc = XDocument.Parse(responseString);

                    XmlSerializer mySerializer = new XmlSerializer(typeof(StatusHistoryResponce));
                    statusHistoryResponce = (StatusHistoryResponce)mySerializer.Deserialize(new StringReader(responseString));
                }


            }
            return statusHistoryResponce;
        }

        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }

        public string GetRateRequestURL(RateSheetParametersDto rateParameters)
        {
            string baseSISUrl = SISWebURLUS + "ec_shipmentcost_v2.asp?";
            if (rateParameters == null)
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
            rateRequestUrl.Append("&dg_accessible=" + rateParameters.dg_accessible);
            rateRequestUrl.Append("&account=" + rateParameters.account);
            rateRequestUrl.Append("&max_actual_length=" + rateParameters.max_actual_length);


            rateRequestUrl.Append("&code_customer=" + rateParameters.code_customer);
            rateRequestUrl.Append("&ind_delivery_inside=" + rateParameters.ind_delivery_inside);
            rateRequestUrl.Append("&url=" + rateParameters.url);


            return rateRequestUrl.ToString();
        }

        private string BuildAddShipmentXMLString(ShipmentDto addShipment)
        {
            //string referenceNo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); addShipment.GeneralInformation.ShipmentName + "-" + referenceNo

            string codeCurrenyString = "";
            //using (var context = new PIContext())
            //{
                codeCurrenyString = context.Currencies.Where(c => c.Id == addShipment.PackageDetails.ValueCurrency).Select(c => c.CurrencyCode).ToList().First();
          //  }

            string costCenterNumber = string.Empty;
            //using (PIContext context = new PIContext())
            //{
                var tarrifTextCode = context.TarrifTextCodes.Where(t => t.TarrifText == addShipment.CarrierInformation.tariffText && t.IsActive && !t.IsDelete).FirstOrDefault();

                if (tarrifTextCode != null && tarrifTextCode.CountryCode == "NL")
                    costCenterNumber = SISCostCenterNL;
                else
                    costCenterNumber = SISCostCenterUS;
            // }

            var customer = context.Customers.Where(c => c.UserId == addShipment.GeneralInformation.CreatedBy).SingleOrDefault();


            StringBuilder shipmentStr = new StringBuilder();

            shipmentStr.AppendFormat("<insert_shipment password='{0}' userid='{1}' code_company='{2}' version='1.0'>", SISPassword, SISUserName, addShipment.SISCompanyCode);
            shipmentStr.AppendFormat("<output_type>XML</output_type>");
            shipmentStr.AppendFormat("<action>STORE_AWB</action>");
            shipmentStr.AppendFormat("<reference>{0}</reference>", addShipment.GeneralInformation.ShipmentReferenceName);
            shipmentStr.AppendFormat("<account>{0}</account>", costCenterNumber);  // Should be cost center - But for now send this value-: 000001
            shipmentStr.AppendFormat("<carrier_name>{0}</carrier_name>", addShipment.CarrierInformation.CarrierName);
            shipmentStr.AppendFormat("<service_level>{0}</service_level>", addShipment.CarrierInformation.serviceLevel);  // TODO: With this pickup date issue encounter.
            shipmentStr.AppendFormat("<ind_dangerous>{0}</ind_dangerous>", "N");   // TODO: sprint 3 doesn't support for dangerous goods. So for this sprint this should be No
            shipmentStr.AppendFormat("<ind_insurance>{0}</ind_insurance>", addShipment.PackageDetails.IsInsuared == "true" ? "Y" : "N");
            shipmentStr.AppendFormat("<code_currency>{0}</code_currency>", codeCurrenyString);
            shipmentStr.AppendFormat("<date_pickup>{0}</date_pickup>", addShipment.PackageDetails.PreferredCollectionDate);   //"18-Mar-2016"
            shipmentStr.AppendFormat("<tariff_type>{0}</tariff_type>", addShipment.CarrierInformation.tarriffType);
            shipmentStr.AppendFormat("<tariff_text>{0}</tariff_text>", addShipment.CarrierInformation.tariffText);
            
            if (customer != null)
            {
                shipmentStr.AppendFormat("<transaction_id>"+ customer.Id.ToString()+ "</transaction_id>");
            }              

            shipmentStr.AppendFormat("<price>{0}</price>", (addShipment.CarrierInformation.Price + addShipment.CarrierInformation.Insurance));    // TODO: Get price from summary total
            //shipmentStr.AppendFormat("<price_insurance>{0}</price_insurance>", ); // TODO: Comment this for now. - Will get clarification later.

            shipmentStr.AppendFormat("<weight>{0}</weight>", addShipment.PackageDetails.ProductIngredients.Max(p => p.Weight));
            shipmentStr.AppendFormat("<weight_unit>{0}</weight_unit>", addShipment.PackageDetails.CmLBS ? "KG" : "LBS");    // TODO: Is LBS word correct?
            shipmentStr.AppendFormat("<length>{0}</length>", addShipment.PackageDetails.ProductIngredients.Max(p => p.Length));
            shipmentStr.AppendFormat("<height>0</height>", addShipment.PackageDetails.ProductIngredients.Max(p => p.Height));
            shipmentStr.AppendFormat("<width>0</width>", addShipment.PackageDetails.ProductIngredients.Max(p => p.Width));
            shipmentStr.AppendFormat("<volume_unit>{0}</volume_unit>", addShipment.PackageDetails.VolumeCMM ? "CM" : "M");

            shipmentStr.AppendFormat("<package>{0}</package>", ProductType(addShipment.PackageDetails.ProductIngredients));    // This represent summary of package. So has different package types, use DIVERSE

            // Consignor details.
            shipmentStr.AppendFormat("<code_country_from>{0}</code_country_from>", addShipment.AddressInformation.Consigner.Country);
            shipmentStr.AppendFormat("<address1>{0}</address1>", string.Format("{0} {1}", addShipment.AddressInformation.Consigner.FirstName, addShipment.AddressInformation.Consigner.LastName));
            shipmentStr.AppendFormat("<address2>{0}</address2>", addShipment.AddressInformation.Consigner.Address1);
            shipmentStr.AppendFormat("<address3>{0}</address3>", addShipment.AddressInformation.Consigner.Address2);
            shipmentStr.AppendFormat("<address4>{0}</address4>", addShipment.AddressInformation.Consigner.City);
            shipmentStr.AppendFormat("<postcode>{0}</postcode>", addShipment.AddressInformation.Consigner.Postalcode);
            shipmentStr.AppendFormat("<street_number>{0}</street_number>", addShipment.AddressInformation.Consigner.Number);
            shipmentStr.AppendFormat("<reference_list><reference_tag>STATE_FROM</reference_tag><reference_value>{0}</reference_value></reference_list>", addShipment.AddressInformation.Consigner.State);
            shipmentStr.AppendFormat("<notify>{0}</notify>", addShipment.AddressInformation.Consigner.ContactName);
            shipmentStr.AppendFormat("<notify_fax>{0}</notify_fax>", addShipment.AddressInformation.Consigner.ContactNumber);
            shipmentStr.AppendFormat("<notify_email>{0}</notify_email>", addShipment.AddressInformation.Consigner.Email);

            // Consignee details.
            shipmentStr.AppendFormat("<code_country_to>{0}</code_country_to>", addShipment.AddressInformation.Consignee.Country);
            shipmentStr.AppendFormat("<address11>{0}</address11>", string.Format("{0} {1}", addShipment.AddressInformation.Consignee.FirstName, addShipment.AddressInformation.Consignee.LastName));
            shipmentStr.AppendFormat("<address12>{0}</address12>", addShipment.AddressInformation.Consignee.Address1);
            shipmentStr.AppendFormat("<address13>{0}</address13>", addShipment.AddressInformation.Consignee.Address2);
            shipmentStr.AppendFormat("<address14>{0}</address14>", addShipment.AddressInformation.Consignee.City);
            shipmentStr.AppendFormat("<postcode_delivery>{0}</postcode_delivery>", addShipment.AddressInformation.Consignee.Postalcode);
            shipmentStr.AppendFormat("<street_number_delivery>{0}</street_number_delivery>", addShipment.AddressInformation.Consignee.Number);
            shipmentStr.AppendFormat("<code_state_to>{0}</code_state_to>", addShipment.AddressInformation.Consignee.State);
            shipmentStr.AppendFormat("<deliver>{0}</deliver>", addShipment.AddressInformation.Consignee.ContactName);
            shipmentStr.AppendFormat("<deliver_fax>{0}</deliver_fax>", addShipment.AddressInformation.Consignee.ContactNumber);
            shipmentStr.AppendFormat("<deliver_email>{0}</deliver_email>", addShipment.AddressInformation.Consignee.Email);

            // Summary of packages.
            shipmentStr.AppendFormat("<description>{0}</description>", addShipment.PackageDetails.ShipmentDescription);
            shipmentStr.AppendFormat("<delivery_condition>{0}</delivery_condition>", addShipment.GeneralInformation.ShipmentServices);
            shipmentStr.AppendFormat("<value>{0}</value>", addShipment.PackageDetails.DeclaredValue);
            shipmentStr.AppendFormat("<code_currency_value>{0}</code_currency_value>", codeCurrenyString);

            double totalCountOfPackages = 0;
            addShipment.PackageDetails.ProductIngredients.ForEach(e => totalCountOfPackages += e.Quantity);

            shipmentStr.AppendFormat("<pieces>{0}</pieces>", totalCountOfPackages);
            shipmentStr.AppendFormat("<shipment_lines>{0}</shipment_lines>", addShipment.PackageDetails.ProductIngredients.Count);

            short lineCount = 1;
            foreach (var lineItem in addShipment.PackageDetails.ProductIngredients)
            {
                shipmentStr.AppendFormat("<shipment_line id='" + lineCount + "'>");
                shipmentStr.AppendFormat("<package>{0}</package>", lineItem.ProductType);
                shipmentStr.AppendFormat("<weight>{0}</weight>", lineItem.Weight);
                shipmentStr.AppendFormat("<quantity>{0}</quantity>", lineItem.Quantity);
                shipmentStr.AppendFormat("<width>{0}</width>", lineItem.Width);
                shipmentStr.AppendFormat("<length>{0}</length>", lineItem.Length);
                shipmentStr.AppendFormat("<height>{0}</height>", lineItem.Height);
                shipmentStr.AppendFormat("<description>{0}</description>", lineItem.Description);
                shipmentStr.AppendFormat("</shipment_line>");
                lineCount++;
            }
            shipmentStr.AppendFormat("</insert_shipment>");

            return shipmentStr.ToString();
        }

        private string ProductType(IList<ProductIngredientsDto> productList)
        {
            bool isFirst = true;
            string productType = "";

            foreach (var item in productList)
            {
                if (isFirst)
                {
                    productType = item.ProductType;
                    isFirst = false;
                }
                else
                {
                    if (productType != item.ProductType)
                    {
                        productType = "DIVERSE";
                        break;
                    }
                }
            }

            return productType;
        }

        public string GetLabel(string shipmentCode)
        {
            string URL = "http://parcelinternational.pro/labelsis/" + shipmentCode;

            string responseString = string.Empty;
            using (var client = new WebClient())
            {
                responseString = client.DownloadString(URL);
            }

            return responseString;
        }
    }
}

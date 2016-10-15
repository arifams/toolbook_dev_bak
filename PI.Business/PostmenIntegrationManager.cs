using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using PI.Contract;
using System.Net;
using System.IO;
using PI.Contract.DTOs.Postmen;
using System.Web.Script.Serialization;
using PI.Data;
using Microsoft.Owin.Logging;

namespace PI.Business
{
    public class PostmenIntegrationManager : ICarrierIntegrationManager
    {

        private PIContext context;
        private Contract.ILogger logger;
        public PostmenIntegrationManager(Contract.ILogger logger, PIContext _context = null)
        {
            context = _context ?? PIContext.Get();
            this.logger = logger;
        }

        public void DeleteShipment(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public string GetLabel(string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public ShipmentcostList GetRateSheetForShipment(RateSheetParametersDto rateParameters)
        {
            throw new NotImplementedException();
        }

        public string GetShipmentStatus(string URL, string shipmentCode)
        {
            throw new NotImplementedException();
        }

        public StatusHistoryResponce GetUpdatedShipmentStatusehistory(string carrier, string trackingNumber, string codeShipment, string environment)
        {
            throw new NotImplementedException();
        }

        public AddShipmentResponse SendShipmentDetails(ShipmentDto addShipment)
        {
            throw new NotImplementedException ();
        }

        public AddShipmentResponsePM SendShipmentDetailsPM(ShipmentDto addShipment)
        {
            AddShipmentResponsePM response = new AddShipmentResponsePM();
            WebRequest httpWebRequest = WebRequest.Create("https://sandbox-api.postmen.com/v3/labels");
            var request = this.CreateRequestJson(addShipment);
            string json = request;

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["postmen-api-key"] = "8d418aba-abc6-41b3-99db-159cfefe6137";

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                try
                {
                    ShipmentResponceDto shipmentResult = serializer.Deserialize<ShipmentResponceDto>(result);
                    

                    if (shipmentResult.data.tracking_numbers==null)
                    {
                        //creating error message
                        if (shipmentResult.meta!=null && shipmentResult.meta.code!=null)
                        {
                            StringBuilder errorMessage = new StringBuilder();
                            errorMessage.Append(shipmentResult.meta.code + ",");
                            errorMessage.Append(shipmentResult.meta.message + ",");

                            foreach (var item in shipmentResult.meta.details)
                            {
                                errorMessage.Append(item.info + ",");
                            }
                            response.ErrorMessage = errorMessage.ToString();
                        }
                      
                    }
                    else
                    {
                        response.Awb = shipmentResult.data.tracking_numbers[0];
                        response.DatePickup = shipmentResult.data.ship_date;
                        response.CodeShipment = shipmentResult.data.id;
                        response.PDF = shipmentResult.data.files.label.url;

                    }
                   

                }
                catch (Exception e)
                {
                  
                }             
              
            }


            return response;
        }

        private string CreateRequestJson(ShipmentDto addShipment)
        {
            string Json = "";
            ShipmentRequestDto request = new ShipmentRequestDto();

            var toCountry = context.Countries.SingleOrDefault(c => c.Code == addShipment.AddressInformation.Consignee.Country).ThreeLetterCode;
            var fromCountry = context.Countries.SingleOrDefault(c => c.Code == addShipment.AddressInformation.Consigner.Country).ThreeLetterCode;
            
            request.async = false;
            request.is_document = false;
            //request.service_type = addShipment.CarrierInformation.serviceLevel;
            request.service_type = "fedex_ground";
            request.paper_size = "4x6";
            request.shipper_account = new PMShipperAccount()
            {               
                 id = "23f73d65-11e9-4c7a-9b2d-9fe8117fe6bb"                 
            };
           
            request.billing = new PMbilling() {
                paid_by = "shipper"
            };           

            request.customs = new PMcustoms() {
                billing = new PMbilling() {
                paid_by = "recipient",
            },
                // terms_of_trade = addShipment.GeneralInformation.ShipmentServices,
                terms_of_trade = "ddu",
                purpose = "merchandise"
            };

            request.shipment = new PMShipment()
            {
                ship_from = new PMAddress()
                {

                    contact_name = addShipment.AddressInformation.Consigner.ContactName,
                    street1 = addShipment.AddressInformation.Consigner.Address1,
                    street2 = addShipment.AddressInformation.Consigner.Address2,
                    postal_code = addShipment.AddressInformation.Consigner.Postalcode,
                    state = addShipment.AddressInformation.Consigner.State,
                    country = fromCountry,
                    phone = addShipment.AddressInformation.Consigner.ContactNumber,
                    email = addShipment.AddressInformation.Consigner.Email,
                    city = addShipment.AddressInformation.Consigner.City,
                    company_name = addShipment.AddressInformation.Consigner.CompanyName

                },


            ship_to = new PMAddress()
            {
                contact_name = addShipment.AddressInformation.Consignee.ContactName,
                street1 = addShipment.AddressInformation.Consignee.Address1,
                street2 = addShipment.AddressInformation.Consignee.Address2,
                postal_code = addShipment.AddressInformation.Consignee.Postalcode,
                state = addShipment.AddressInformation.Consignee.State,
                country = toCountry,
                phone = addShipment.AddressInformation.Consignee.ContactNumber,
                email = addShipment.AddressInformation.Consignee.Email,
                city = addShipment.AddressInformation.Consignee.City,
                company_name = addShipment.AddressInformation.Consignee.CompanyName
            }

            };


            request.shipment.parcels = new List<PMParcel>();

            foreach (var products in addShipment.PackageDetails.ProductIngredients)
            {

                request.shipment.parcels.Add(new PMParcel()
                {
                    box_type= "custom",
                    description= products.Description,
                    
                    dimension= new PMDimension()
                    {
                       depth=products.Length,
                       height=products.Height,
                       width=products.Width,
                       unit=addShipment.PackageDetails.VolumeCMM==true?"cm":"inch"                   
                        
                    },
                    items= new List<PMItems>()
                    {
                        new PMItems() {
                           description= products.Description,
                           hs_code=addShipment.PackageDetails.HsCode,
                           origin_country=fromCountry,
                           quantity=products.Quantity,
                           price= new PMPrice
                           {
                               amount=0,
                               currency="USD"

                           },
                           sku="parcel2016",
                           weight= new PMWeight()
                           {
                               unit=addShipment.PackageDetails.CmLBS==true?"kg":"lbs",
                               value=products.Weight
                           },
                          


                        }
                    },
                    weight= new PMWeight()
                    {
                        unit = addShipment.PackageDetails.CmLBS == true ? "kg" : "lbs",
                        value = products.Weight
                    }


                });
            }
            
            request.shipment.parcels.FirstOrDefault().items.FirstOrDefault().price.amount = addShipment.CarrierInformation.Price;
            
           JavaScriptSerializer serializer = new JavaScriptSerializer();
           return serializer.Serialize(request);                     
        }


        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }

}


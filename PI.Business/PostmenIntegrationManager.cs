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
using System.Configuration;

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

        public string PostMenAPIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["PostMenAPIKey"].ToString();
            }
        }

        public string USPSAccountKey
        {
            get
            {
                return ConfigurationManager.AppSettings["USPSAccountKey"].ToString();
            }
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
            httpWebRequest.Headers["postmen-api-key"] = PostMenAPIKey;

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
                    

                    if (shipmentResult.data.tracking_numbers==null || shipmentResult.data.tracking_numbers.Length==0)
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
                    var x = e.Message;
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

            var termsOfTrade = "";
            var serviceType = "";
            var paidBy = "";

            if (addShipment.CarrierInformation.serviceLevel== "First-Class Package International")
            {
               // serviceType = "usps_first_class_package_international";
                serviceType = "usps_first_class_mail";
            }


            if (addShipment.GeneralInformation.ShipmentServices.Contains("DDU"))
            {
                termsOfTrade = "ddu";
                paidBy = "recipient";
            }
            else if (addShipment.GeneralInformation.ShipmentServices.Contains("DDP"))
            {
                termsOfTrade = "ddp";
                paidBy = "shipper";
            }

            request.async = false;
            request.is_document = false;
            //request.service_type = addShipment.CarrierInformation.serviceLevel;
            //request.service_type = "fedex_ground";
            request.service_type = serviceType;
            request.paper_size = "4x6";
            request.shipper_account = new PMShipperAccount()
            {               
                 id = USPSAccountKey                 
            };
           
            request.billing = new PMbilling() {
                paid_by = "shipper"
            };           

            request.customs = new PMcustoms() {
                billing = new PMbilling() {
                paid_by = paidBy,
            },
                // terms_of_trade = addShipment.GeneralInformation.ShipmentServices,
                terms_of_trade = termsOfTrade,
                purpose = "merchandise"
            };

            request.shipment = new PMShipment()
            {
                ship_from = new PMAddress()
                {

                    contact_name = addShipment.AddressInformation.Consigner.ContactName,
                    street1 = addShipment.AddressInformation.Consigner.Address1,
                   // street2 = addShipment.AddressInformation.Consigner.Address2,
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
              //  street2 = addShipment.AddressInformation.Consignee.Address2,
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
                var packageType = this.GetPackageTypesFromCarrirerAndPackegType(addShipment.CarrierInformation.CarrierName, products, addShipment.PackageDetails.VolumeCMM, addShipment.PackageDetails.CmLBS);

                request.shipment.parcels.Add(new PMParcel()
                {
                    box_type= packageType,
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


        private string GetPackageTypesFromCarrirerAndPackegType(String Carrier,ProductIngredientsDto product, bool isCm, bool isKg)
        {
            

            if (Carrier=="USPS")
            {
                return this.GetPackageTypeUSPS(product, isCm, isKg);
            }
            else
            {
                return "";
            }


        }


        private string GetPackageTypeUSPS(ProductIngredientsDto product, bool isCm, bool isKg)
        {
            string packageType = "";
            var width = 0.0;
            var height = 0.0;
            var length = 0.0;
            var weight = 0.0;
            if (isCm)
            {
                width = Convert.ToDouble(product.Width) * 0.39;
                height = Convert.ToDouble(product.Height) * 0.39;
                length = Convert.ToDouble(product.Length) * 0.39;
            }
            else
            {
                width = Convert.ToDouble(product.Width);
                height = Convert.ToDouble(product.Height);
                length = Convert.ToDouble(product.Length);
            }

            if (isKg)
            {
                weight = Convert.ToDouble(product.Weight) * 2.2;
            }
            else
            {
                weight = Convert.ToDouble(product.Weight);
            }

            if (product.ProductType=="Document")
            {
                packageType = "usps_large_envelope";

            }

            else if (product.ProductType=="Box")
            {
                if (length<12 && width<12 && height<12 && weight<=25)
                {
                    packageType = "usps_parcel";
                }
                else if (length+ (2* (width+height)) <=108 && weight<=70)
                {
                    packageType = "usps_large_parcel";
                }
                else if (weight>70)
                {
                    packageType = "usps_irregular_parcel";
                }
              

            }

            return packageType;

        }

        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }

}


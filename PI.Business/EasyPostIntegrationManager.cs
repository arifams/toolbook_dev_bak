using PI.Contract.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Contract.DTOs.RateSheets;
using PI.Contract.DTOs.Shipment;
using EasyPost;
using System.Configuration;

namespace PI.Business
{
    public class EasyPostIntegrationManager : ICarrierIntegrationManager
    {
        public string EasyPostKey
        {
            get
            {
                return ConfigurationManager.AppSettings["EasyPostKey"].ToString();
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
            EasyPost.ClientManager.SetCurrent(EasyPostKey);

            Dictionary<string, object> fromAddress = new Dictionary<string, object>() {
            {"name", addShipment.AddressInformation.Consigner.FirstName+" "+addShipment.AddressInformation.Consigner.LastName},
            {"street1", addShipment.AddressInformation.Consigner.Address1 },
            {"street2", addShipment.AddressInformation.Consigner.Address2 },
            {"city", addShipment.AddressInformation.Consigner.City},
            {"state", addShipment.AddressInformation.Consigner.State},
            {"country",  addShipment.AddressInformation.Consigner.Country},
            {"phone", addShipment.AddressInformation.Consigner.ContactNumber},
            {"email",addShipment.AddressInformation.Consigner.Email},
            {"zip", addShipment.AddressInformation.Consigner.Postalcode},
            {"mode", "test" }
            };

            Dictionary<string, object> toAddress = new Dictionary<string, object>() {
            {"name", addShipment.AddressInformation.Consignee.FirstName+" "+addShipment.AddressInformation.Consignee.LastName},
            {"street1", addShipment.AddressInformation.Consignee.Address1 },
            {"street2", addShipment.AddressInformation.Consignee.Address2 },
            {"city", addShipment.AddressInformation.Consignee.City},
            {"state", addShipment.AddressInformation.Consignee.State},
            {"country",  addShipment.AddressInformation.Consignee.Country},
            {"phone", addShipment.AddressInformation.Consignee.ContactNumber},
            {"email",addShipment.AddressInformation.Consigner.Email},
            {"zip", addShipment.AddressInformation.Consignee.Postalcode},
            {"mode", "test" }
            };


            //looping through the product ingrediants
            //foreach (var item in addShipment.PackageDetails.ProductIngredients)
            //{

            //}

            var parcelItem = addShipment.PackageDetails.ProductIngredients.FirstOrDefault();

            Dictionary<string, object> parcel = new Dictionary<string, object>() {
              {"length", parcelItem.Length}, {"width", parcelItem.Width}, {"height", parcelItem.Height}, {"weight", parcelItem.Weight}
            };

            CustomsInfo customs = new CustomsInfo()
            {
                contents_explanation = null,
                contents_type = "merchandise",
                customs_certify = "false",
                non_delivery_option = "return",
                restriction_comments = null,
                restriction_type = "none",
                customs_items = new List<CustomsItem>()
                {
                    new CustomsItem
                    {
                       description= "Many, many EasyPost stickers.",
                       hs_tariff_number= "123456",
                       origin_country= "US",
                       quantity= 1,
                       value= 10,
                       weight= 10,
                    }
                }
            };

            Shipment shipment = Shipment.Create(new Dictionary<string, object>() {           
            {"parcel", parcel},
            {"to_address", toAddress},
            {"from_address", fromAddress},
            {"customs_info",customs },
            {"reference", "PI002"}
           });
            
            Rate selectedRate = shipment.LowestRate();

            AddShipmentResponse response = new AddShipmentResponse();
                     
            shipment.Buy(selectedRate);
            Shipment shipmentCreated = Shipment.Retrieve("PI002");

            //if (shipmentCreated!=null)
            //{
               
            //}

            return response;
        }

        public string TrackAndTraceShipment(string URL)
        {
            throw new NotImplementedException();
        }
    }
}
